CREATE PROCEDURE [dbo].[Znode_ImportPimProductData]
(   @TableName          VARCHAR(200),
    @NewGUID            NVARCHAR(200),
    @TemplateId         NVARCHAR(200),
    @ImportProcessLogId INT,
    @UserId             INT,
    @LocaleId           INT,
    @DefaultFamilyId    INT)
AS
    
/*
    Summary : Finally Import data into ZnodePimProduct, ZnodePimAttributeValue and ZnodePimAttributeValueLocale Table 
    Process : Flat global temporary table will split into cloumn wise and associted with Znode Attributecodes,
    		    Create group of product with their attribute code and values and inserted one by one products. 	   
    
    SourceColumnName : CSV file column headers
    TargetColumnName : Attributecode from ZnodePimAttribute Table 

	***  Need to log error if transaction failed during insertion of records into table.
*/

BEGIN
	SET NOCOUNT ON
    BEGIN TRY
        BEGIN TRAN ImportProducts;
        DECLARE @SQLQuery NVARCHAR(MAX);
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
        DECLARE @AttributeTypeName NVARCHAR(10), @AttributeCode NVARCHAR(300), @AttributeId INT, @IsRequired BIT, @SourceColumnName NVARCHAR(600), @PimAttributeFamilyId INT, @NewProductId INT, @PimAttributeValueId INT, @status BIT= 0; 
        --Declare error Log Table 
			    
			
		DECLARE @FamilyAttributeDetail TABLE
		( 
		PimAttributeId int, AttributeTypeName varchar(300), AttributeCode varchar(300), SourceColumnName nvarchar(600), IsRequired bit, PimAttributeFamilyId int
		);
        IF @DefaultFamilyId = 0
            BEGIN
			INSERT INTO @FamilyAttributeDetail( PimAttributeId, AttributeTypeName, AttributeCode, SourceColumnName, IsRequired, PimAttributeFamilyId )
			--Call Process to insert data of defeult family with cource column name and target column name 
			EXEC Znode_ImportGetTemplateDetails @TemplateId = @TemplateId, @IsValidationRules = 0, @IsIncludeRespectiveFamily = 1,@DefaultFamilyId = @DefaultFamilyId;
            UPDATE @FamilyAttributeDetail SET PimAttributeFamilyId = DBO.Fn_GetCategoryDefaultFamilyId();

			---- Deleted Attribute which are not provided in product import CSV and required attribute not mapped with AttributeGroup
			Delete FAD from @FamilyAttributeDetail FAD
			where AttributeCode not in (select Name from tempdb.sys.columns where object_id = object_id(@TableName))
			and not exists(select * from ZnodePimAttributeGroupMapper ZPAGM inner join ZnodePimFamilyGroupMapper ZPFGM on ZPAGM.PimAttributeGroupId = ZPFGM.PimAttributeGroupId 
					        inner join ZnodePimAttribute ZPA on ZPAGM.PimAttributeId = ZPA.PimAttributeId and FAD.AttributeCode = ZPA.AttributeCode)
            END;
        ELSE
            BEGIN
                INSERT INTO @FamilyAttributeDetail(PimAttributeId,AttributeTypeName,AttributeCode,SourceColumnName,IsRequired,PimAttributeFamilyId)
                --Call Process to insert data of defeult family with cource column name and target column name 
                EXEC Znode_ImportGetTemplateDetails @TemplateId = @TemplateId,@IsValidationRules = 0,@IsIncludeRespectiveFamily = 1,@DefaultFamilyId = @DefaultFamilyId;

				---- Deleted Attribute which are not provided in product import CSV and required attribute not mapped with AttributeGroup
			Delete FAD from @FamilyAttributeDetail FAD
			where AttributeCode not in (select Name from tempdb.sys.columns where object_id = object_id(@TableName))
			and not exists(select * from ZnodePimAttributeGroupMapper ZPAGM inner join ZnodePimFamilyGroupMapper ZPFGM on ZPAGM.PimAttributeGroupId = ZPFGM.PimAttributeGroupId 
					        inner join ZnodePimAttribute ZPA on ZPAGM.PimAttributeId = ZPA.PimAttributeId and FAD.AttributeCode = ZPA.AttributeCode)
            END;  
				
		-- Retrive PimProductId on the basis of SKU for update product 
		SET @SQLQuery = 'UPDATE tlb SET tlb.PimProductId = ZPAV.PimProductId 
						FROM ZnodePimAttributeValue AS ZPAV INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL ON 
						(ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId) 
						INNER JOIN [dbo].[ZnodePimAttribute] ZPA on ZPAV.PimAttributeId = ZPA.PimAttributeId AND ZPA.AttributeCode= ''SKU'' 
						INNER JOIN '+@TableName+' tlb ON ZPAVL.AttributeValue = ltrim(rtrim(tlb.SKU)) ';
		EXEC sys.sp_sqlexec	@SQLQuery	
			 
			 
		SET @SQLQuery='
		INSERT INTO ZnodeImportLog( ErrorDescription,ColumnName, Data, GUID,RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT ''135'',''SKU'', SKU, '''+@NewGUId+''',RowNumber, '+cast(@UserId as varchar(10))+', '''+CONVERT(NVARCHAR(30),@GetDate,121)+''', '+cast(@UserId as varchar(10))+', '''+CONVERT(NVARCHAR(30),@GetDate,121)+''', '+cast(@ImportProcessLogId as varchar(10))+'
		FROM '+@TableName +' WHERE LEN(SKU)>600';
		EXEC sys.sp_sqlexec @SQLQuery;

		SET @SQLQuery='Delete B from '+@TableName +' B WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeImportLog A where B.RowNumber=A.RowNumber and A.ImportProcessLogId 
		='+cast(@ImportProcessLogId as varchar(10))+')'
		EXEC (@SQLQuery)

        --Read all attribute details with their datatype 
        IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#DefaultAttributeValue')
		BEGIN
				CREATE TABLE #DefaultAttributeValue (AttributeTypeName  VARCHAR(300),PimAttributeDefaultValueId INT,PimAttributeId INT,
				AttributeDefaultValueCode  VARCHAR(100));
					 
			INSERT INTO #DefaultAttributeValue(AttributeTypeName,PimAttributeDefaultValueId,PimAttributeId,AttributeDefaultValueCode)
			--Call Process to insert default data value 
			EXEC Znode_ImportGetPimAttributeDefaultValue;
		END;
        ELSE
        BEGIN
            DROP TABLE #DefaultAttributeValue;
        END;
        EXEC sys.sp_sqlexec
            @SQLQuery;
       
		DECLARE @PimProductDetail TABLE 
		(
			      
			PimAttributeId INT, PimAttributeFamilyId INT,ProductAttributeCode VARCHAR(300) NULL,
			ProductAttributeDefaultValueId INT NULL,PimAttributeValueId  INT NULL,LocaleId INT,
			PimProductId INT NULL,AttributeValue NVARCHAR(MAX) NULL,AssociatedProducts NVARCHAR(4000) NULL,ConfigureAttributeIds VARCHAR(2000) NULL,
			ConfigureFamilyIds VARCHAR(2000) NULL,RowNumber INT  
        );

		-- Update Record count in log 
		DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
		SET @SQLQuery = ' Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;
		EXEC	sp_executesql @SQLQuery, N'@SuccessRecordCount BIGINT out' , @SuccessRecordCount=@SuccessRecordCount OUTPUT
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount, TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- End

			
        -- Column wise split data from source table ( global temporary table ) and inserted into temporary table variable @PimProductDetail
        -- Add PimAttributeDefaultValue 
        DECLARE Cr_AttributeDetails CURSOR LOCAL FAST_FORWARD
        FOR SELECT PimAttributeId,AttributeTypeName,AttributeCode,IsRequired,SourceColumnName,PimAttributeFamilyId FROM @FamilyAttributeDetail  WHERE ISNULL(SourceColumnName, '') <> '';
        OPEN Cr_AttributeDetails;
        FETCH NEXT FROM Cr_AttributeDetails INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @PimAttributeFamilyId;
        WHILE @@FETCH_STATUS = 0
        BEGIN
			SET @NewProductId = 0;
			SET @SQLQuery = ' SELECT '''+CONVERT(VARCHAR(100), @PimAttributeFamilyId)+''' PimAttributeFamilyId , PimProductId PimProductId,'''+'['+@AttributeCode+']'+''' ProductAttributeCode ,'''+CONVERT(VARCHAR(100), @AttributeId)+''' AttributeId ,
							(SELECT TOP 1  PimAttributeDefaultValueId FROM #DefaultAttributeValue Where PimAttributeId =  '
							+ CONVERT(VARCHAR(100), @AttributeId)+'AND  AttributeDefaultValueCode = TN.['+@SourceColumnName+'] ) PimAttributeDefaultValueId ,['
							+ @SourceColumnName+'],'+CONVERT(VARCHAR(100), @LocaleId)+'LocaleId
						, RowNumber FROM '+@TableName+' TN';
						print @SQLQuery
			INSERT INTO @PimProductDetail( PimAttributeFamilyId, PimProductId,ProductAttributeCode, PimAttributeId, ProductAttributeDefaultValueId, AttributeValue, LocaleId, RowNumber )
					
			EXEC sys.sp_sqlexec @SQLQuery;
			FETCH NEXT FROM Cr_AttributeDetails INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @PimAttributeFamilyId;
        END;
        CLOSE Cr_AttributeDetails;
        DEALLOCATE Cr_AttributeDetails;
			 
			
		if object_id('tempdb..#PimProductDetail1') is not null
		drop table #PimProductDetail1

		Select * into #PimProductDetail1 from @PimProductDetail

		DELETE FROM #PimProductDetail1 WHERE RTRIM(LTRIM(ISNULL(AttributeValue, ''))) = '';

		UPDATE a 
		SET ConfigureAttributeIds =  SUBSTRING((SELECT ','+CAST(c.PimAttributeId As VARCHAR(100)) 
		FROM #PimProductDetail1 c 
		INNER JOIN ZnodePimAttribute b ON (b.PimAttributeId = c.PimAttributeId)
		WHERE IsConfigurable =1  AND c.RowNumber = a.RowNumber  FOR XML PATH('')),2,4000) 
		FROM #PimProductDetail1 a 
		WHERE EXISTS (SELECT TOP 1 1 FROM #PimProductDetail1 ab  WHERE ab.RowNumber = a.RowNumber AND	ab.ProductAttributeCode = '[ProductType]' 
		AND ab.AttributeValue = 'ConfigurableProduct' )
				
        -- In case of Yes/No : If value is not TRUE OR  1 then it will be  False else True
		--If default Value set not need of hard code for IsActive
		UPDATE ppdti SET ppdti.AttributeValue = CASE WHEN Upper(ISNULL(ppdti.AttributeValue, '')) in ( 'TRUE','1')  THEN 'true'  ELSE 'false' END FROM #PimProductDetail1 ppdti
        INNER JOIN #DefaultAttributeValue dav ON ppdti.PimAttributeId = dav.PimAttributeId WHERE   dav.AttributeTypeName = 'Yes/No';
   	
		-----------Added Performance patch 
		DECLARE @PublishStateIdForDraft INT= [dbo].[Fn_GetPublishStateIdForDraftState]();
		DECLARE @PimDefaultFamily INT= dbo.Fn_GetDefaultPimProductFamilyId();
		DECLARE @pimSkuAttributeId VARCHAR(50)= [dbo].[Fn_GetProductSKUAttributeId]();
		DECLARE @PimIsDownlodableAttributeId VARCHAR(50)= [dbo].[Fn_GetIsDownloadableAttributeId]();
		DECLARE @PublishStateIdForNotPublished INT= [dbo].[Fn_GetPublishStateIdForForNotPublishedState]();
        
		CREATE INDEX Inx_PimProductDetail_Bulk1 ON #PimProductDetail1(RowNumber);

		------------------------------------Bulk Row Process
		DECLARE @MaxCount INT, @MinRow INT, @MaxRow INT, @Rows numeric(10,2);
		---- Count of total rows for import
		SELECT @MaxCount = COUNT(*) FROM #PimProductDetail1;
		
		---- Count of rows in loop for import
		SELECT @Rows = (select top 1 FeatureValues from ZnodeGlobalSetting where FeatureName = 'ProductImportBulk')  --ceiling(@MaxCount/100.0)
        
		SELECT @MaxCount = CEILING(@MaxCount / @Rows);

		IF OBJECT_ID('tempdb..#Temp_ImportLoop') IS NOT NULL
			DROP TABLE #Temp_ImportLoop;
        
		---- To get the min and max rows for import in loop
		;WITH cte AS 
		(
			SELECT RowId = 1, 
					MinRow = 1, 
					MaxRow = cast(@Rows as int)
			UNION ALL
			SELECT RowId + 1, 
					MinRow + cast(@Rows as int), 
					MaxRow + cast(@Rows as int)
			FROM cte
			WHERE RowId + 1 <= @MaxCount
		)
		SELECT RowId, MinRow, MaxRow
		INTO #Temp_ImportLoop
		FROM cte
		option (maxrecursion 0);

		--while @MaxCount <= @minRow
		DECLARE cur_BulkData CURSOR LOCAL FAST_FORWARD
		FOR SELECT MinRow, MaxRow FROM #Temp_ImportLoop;

		OPEN cur_BulkData;
		FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow;

		WHILE @@FETCH_STATUS = 0
		BEGIN
		
		BEGIN TRAN ImportProducts;

		if object_id ('tempdb..#PimProductDetail_Bulk_Process')is not null
				drop table tempdb..#PimProductDetail_Bulk_Process

		CREATE TABLE #PimProductDetail_Bulk_Process
		([PimAttributeId]                 [INT] NULL, 
			[PimAttributeFamilyId]           [INT] NULL, 
			[ProductAttributeCode]           [VARCHAR](300) NULL, 
			[ProductAttributeDefaultValueId] [INT] NULL, 
			[PimAttributeValueId]            [INT] NULL, 
			[LocaleId]                       [INT] NULL, 
			[PimProductId]                   [INT] NULL, 
			[AttributeValue]                 [NVARCHAR](MAX) NULL, 
			[AssociatedProducts]             [NVARCHAR](4000) NULL, 
			[ConfigureAttributeIds]          [VARCHAR](2000) NULL, 
			[ConfigureFamilyIds]             [VARCHAR](2000) NULL, 
			[RowNumber]                      [INT] NULL, 
			SKU1                             VARCHAR(600),
			Id Int Identity(1,1)Primary Key
		);

		CREATE INDEX Inx_PimProductDetail_Bulk_Process ON #PimProductDetail_Bulk_Process(ProductAttributeCode, PimProductId);
		CREATE INDEX Inx_PimProductDetail_Bulk_Process1 ON #PimProductDetail_Bulk_Process(RowNumber);
		CREATE INDEX Inx_PimProductDetail_Bulk_Process2 ON #PimProductDetail_Bulk_Process(ProductAttributeCode)
		CREATE INDEX Inx_PimProductDetail_Bulk_Process3 ON #PimProductDetail_Bulk_Process(PimAttributeId, PimProductId);
		
		---- Insert rows for import in bulk
		INSERT INTO #PimProductDetail_Bulk_Process
		([PimAttributeId], 
			[PimAttributeFamilyId], 
			[ProductAttributeCode], 
			[ProductAttributeDefaultValueId], 
			[PimAttributeValueId], 
			[LocaleId], 
			[PimProductId], 
			[AttributeValue], 
			[AssociatedProducts], 
			[ConfigureAttributeIds], 
			[ConfigureFamilyIds], 
			[RowNumber]
		)
		SELECT [PimAttributeId], 
				[PimAttributeFamilyId], 
				[ProductAttributeCode], 
				[ProductAttributeDefaultValueId], 
				[PimAttributeValueId], 
				[LocaleId], 
				[PimProductId], 
				ltrim(rtrim([AttributeValue])), 
				[AssociatedProducts], 
				[ConfigureAttributeIds], 
				[ConfigureFamilyIds], 
				[RowNumber]
		FROM #PimProductDetail1 a
		WHERE a.[RowNumber] BETWEEN @MinRow AND @MaxRow;

		--select * from @PimProductDetail

		--select * from #PimProductDetail1

			    

		---------------------------Start Importing 
		if object_id ('tempdb..#TBL_DefaultAttributeId')is not null
			drop table #TBL_DefaultAttributeId

		if object_id ('tempdb..#TBL_MediaAttributeId')is not null
			drop table #TBL_MediaAttributeId

		if object_id ('tempdb..#TBL_TextAreaAttributeId')is not null
			drop table #TBL_TextAreaAttributeId

		if object_id ('tempdb..#TBL_MediaAttributeValue')is not null
			drop table #TBL_MediaAttributeValue

		if object_id ('tempdb..#TBL_DefaultAttributeValue')is not null
			drop table #TBL_DefaultAttributeValue

		if object_id ('tempdb..#ZnodePimAttributeValue')is not null
			drop table #ZnodePimAttributeValue

				
		CREATE TABLE #TBL_DefaultAttributeId ( PimAttributeId INT PRIMARY KEY, AttributeCode  VARCHAR(600) );

		CREATE TABLE #TBL_MediaAttributeId ( PimAttributeId INT PRIMARY KEY, AttributeCode  VARCHAR(600) );

		CREATE TABLE #TBL_TextAreaAttributeId ( PimAttributeId INT PRIMARY KEY, AttributeCode  VARCHAR(600) );
           
		CREATE TABLE #TBL_MediaAttributeValue ( PimAttributeValueId INT, LocaleId INT, AttributeValue VARCHAR(300), MediaId INT );

		CREATE TABLE #TBL_DefaultAttributeValue ( PimAttributeValueId INT, LocaleId INT, AttributeValue INT );

		CREATE TABLE #ZnodePimAttributeValue (PimAttributeValueId  INT, PimAttributeFamilyId INT, PimAttributeId INT, PimProductId INT );

		DECLARE @ConfigureFamilyId VARCHAR(4000);

		INSERT INTO #TBL_DefaultAttributeId ( PimAttributeId, AttributeCode )
		SELECT PimAttributeId, AttributeCode
		FROM [dbo].[Fn_GetDefaultAttributeId]();

		INSERT INTO #TBL_MediaAttributeId (PimAttributeId, AttributeCode )
		SELECT PimAttributeId, AttributeCode
		FROM [dbo].[Fn_GetProductMediaAttributeId]();

		INSERT INTO #TBL_TextAreaAttributeId ( PimAttributeId, AttributeCode )
		SELECT PimAttributeId, AttributeCode
		FROM [dbo].[Fn_GetTextAreaAttributeId]();

		SELECT TOP 1 @PimAttributeFamilyId = PimAttributeFamilyId FROM #PimProductDetail_Bulk_Process;

		if object_id ('tempdb..#cte')is not null
			drop table #cte

		SELECT AttributeValue AS SKU, RowNumber
		INTO #cte
		FROM #PimProductDetail_Bulk_Process
		WHERE ProductAttributeCode = '[SKU]';
              
		CREATE INDEX Inx_cte_RowNumber ON #cte(RowNumber);
		UPDATE a SET a.SKU1 = B.SKU
		FROM #PimProductDetail_Bulk_Process a
		INNER JOIN #cte b ON a.RowNumber = b.RowNumber;

		SELECT TOP 1 @LocaleId = LocaleId FROM #PimProductDetail_Bulk_Process;

		----Update ZNodePimProduct 
		UPDATE ZNodePimProduct
		SET PimAttributeFamilyId = DP.PimAttributeFamilyId, 
			PublishStateId = @PublishStateIdForDraft, 
			ModifiedBy = @UserId, 
			ModifiedDate = @GetDate
		FROM ZNodePimProduct ZPP
		INNER JOIN #PimProductDetail_Bulk_Process DP ON ZPP.PimProductId = DP.PimProductId;
      
		if object_id ('tempdb..#ZnodePimProduct')is not null
			drop table #ZnodePimProduct

		CREATE TABLE #ZnodePimProduct(PimProductId INT,ExternalId INT  Primary key)

		--create index Idx_ZnodePimProduct_ExternalId on #ZnodePimProduct(ExternalId)

		----Insert into ZNodePimProduct 
		INSERT INTO ZnodePimProduct
		(PimAttributeFamilyId, 
			ExternalId, 
			CreatedBy, 
			CreatedDate, 
			ModifiedBy, 
			ModifiedDate, 
			PublishStateId
		)
		output inserted.PimProductId, inserted.ExternalId into #ZnodePimProduct(PimProductId,ExternalId)
		SELECT PimAttributeFamilyId, 
				RowNumber, 
				@UserId, 
				@GetDate, 
				@UserId, 
				@GetDate, 
				@PublishStateIdForNotPublished
		FROM #PimProductDetail_Bulk_Process
		WHERE ProductAttributeCode = '[SKU]'
		AND PimProductId IS NULL;
            
		----Update newly created productIds
		UPDATE a SET a.PimProductId = b.PimProductId
		FROM #PimProductDetail_Bulk_Process a
		INNER JOIN #ZnodePimProduct b ON a.RowNumber = b.ExternalId;

		----Insert Downloadable products into ZnodePimDownloadableProduct
		INSERT INTO ZnodePimDownloadableProduct (SKU, ProductName, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
		SELECT PDSKU.AttributeValue, PDProdName.AttributeValue, @UserId, @GetDate, @UserId, @GetDate
		FROM #PimProductDetail_Bulk_Process PDSKU
		INNER JOIN #PimProductDetail_Bulk_Process PDProdName ON PDProdName.RowNumber = PDSKU.RowNumber
		INNER JOIN #PimProductDetail_Bulk_Process PDDownload ON PDDownload.RowNumber = PDSKU.RowNumber
		WHERE PDSKU.ProductAttributeCode = @pimSkuAttributeId
		AND PDProdName.ProductAttributeCode = '[SKU]'
		AND PDDownload.PimAttributeId = @PimIsDownlodableAttributeId
		AND PDDownload.AttributeValue = 'true'
		AND NOT EXISTS ( SELECT TOP 1 1 FROM ZnodePimDownloadableProduct WHERE ZnodePimDownloadableProduct.SKU = PDSKU.AttributeValue );

		---- update ZnodePimAttributeValue : attribute data for Product
		UPDATE TARGET
		SET TARGET.PimAttributeFamilyId = CASE
												WHEN Source.PimAttributeFamilyId = 0
												THEN NULL
												ELSE Source.PimAttributeFamilyId
											END, 
				TARGET.ModifiedBy = @UserId, 
				TARGET.ModifiedDate = @GetDate, 
				TARGET.PimProductId = SOURCE.PimProductId
		OUTPUT INSERTED.PimAttributeValueId, 
				INSERTED.PimAttributeFamilyId, 
				INSERTED.PimAttributeId, 
				INSERTED.PimProductId
				INTO #ZnodePimAttributeValue
		FROM ZnodePimAttributeValue TARGET
		INNER JOIN #PimProductDetail_Bulk_Process SOURCE ON TARGET.PimProductId = SOURCE.PimProductId AND TARGET.PimAttributeId = SOURCE.PimAttributeId;
             
		---- Inserting attribute data for Product 
		INSERT INTO ZnodePimAttributeValue 
		( 
			PimAttributeFamilyId, 
			PimProductId, PimAttributeId, 
			PimAttributeDefaultValueId, 
			CreatedBy, 
			CreatedDate, 
			ModifiedBy, 
			ModifiedDate 
		)
		OUTPUT INSERTED.PimAttributeValueId, 
				INSERTED.PimAttributeFamilyId, 
				INSERTED.PimAttributeId, 
				INSERTED.PimProductId
				INTO #ZnodePimAttributeValue
		SELECT 
			CASE
				WHEN Source.PimAttributeFamilyId = 0
				THEN @PimDefaultFamily
				ELSE Source.PimAttributeFamilyId
			END, 
			SOURCE.PimProductId, 
			ISNULL(SOURCE.PimAttributeId, 0),
			CASE
				WHEN SOURCE.ProductAttributeDefaultValueId = 0
				THEN NULL
				ELSE SOURCE.ProductAttributeDefaultValueId
			END, 
			@UserId, 
			@GetDate, 
			@UserId, 
			@GetDate
		FROM #PimProductDetail_Bulk_Process SOURCE
		WHERE NOT EXISTS
		(
			SELECT *
			FROM ZnodePimAttributeValue TARGET
			WHERE TARGET.PimProductId = SOURCE.PimProductId
					AND TARGET.PimAttributeId = SOURCE.PimAttributeId
		);
		-------------------------
		if object_id ('tempdb..#MediaData')is not null
			drop table #MediaData

		CREATE TABLE #MediaData (MediaId INT, PimProductId INT, PimAttributeId INT, PimAttributeFamilyId INT, LocaleId INT );

		---- Get Product Media Data
		INSERT INTO #MediaData ( MediaId , PimProductId , PimAttributeId , PimAttributeFamilyId , LocaleId )
		SELECT SP.Item, a.PimProductId, a.PimAttributeId, PimAttributeFamilyId, a.LocaleId
		FROM #PimProductDetail_Bulk_Process a
		INNER JOIN #TBL_MediaAttributeId c ON(c.PimAttributeId = a.PimAttributeId)
		CROSS APPLY dbo.split(a.AttributeValue, ',') SP;

		---- Get product media attribute data
		INSERT INTO #TBL_MediaAttributeValue ( PimAttributeValueId, LocaleId, AttributeValue, MediaId )
		SELECT a.PimAttributeValueId, b.LocaleId, zm.Path AttributeValue, ZM.MediaId
		FROM #ZnodePimAttributeValue AS a
		INNER JOIN #MediaData AS b ON(a.PimAttributeId = b.PimAttributeId
										AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0)
										AND a.PimProductId = b.PimProductId)
		INNER JOIN ZnodeMedia ZM ON(b.MediaId = ZM.MediaId);
     
		---- Deleting product media attribute
		DELETE FROM ZnodePimProductAttributeMedia
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM #TBL_MediaAttributeValue TBLM
			WHERE ZnodePimProductAttributeMedia.PimAttributeValueId = TBLM.PimAttributeValueId
					AND TBLM.MediaId <> ZnodePimProductAttributeMedia.MediaId
					AND ZnodePimProductAttributeMedia.Localeid = @LocaleId
		);

		---- update ZnodePimProductAttributeMedia : attribute data for Product
		UPDATE TARGET
			SET 
				TARGET.MediaPath = SOURCE.AttributeValue, 
				TARGET.MediaId = SOURCE.MediaId, 
				TARGET.ModifiedBy = @UserId, 
				TARGET.ModifiedDate = @GetDate
		FROM ZnodePimProductAttributeMedia TARGET
		INNER JOIN #TBL_MediaAttributeValue SOURCE ON TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
													AND TARGET.MediaPAth = SOURCE.AttributeValue
													AND TARGET.LocaleId = SOURCE.LocaleId;
    
		---- inserting Media attribute data for Product
		INSERT INTO ZnodePimProductAttributeMedia 
		( 
			PimAttributeValueId, 
			LocaleId, MediaPath, 
			MediaId, 
			CreatedBy, 
			CreatedDate, 
			ModifiedBy, 
			ModifiedDate
		)
		SELECT SOURCE.PimAttributeValueId, 
				SOURCE.LocaleId, 
				SOURCE.AttributeValue, 
				SOURCE.MediaId, 
				@UserId, 
				@GetDate, 
				@UserId, 
				@GetDate
		FROM #TBL_MediaAttributeValue SOURCE
		WHERE NOT EXISTS
		(
			SELECT *
			FROM ZnodePimProductAttributeMedia TARGET
			WHERE TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
					AND TARGET.MediaPAth = SOURCE.AttributeValue
					AND TARGET.LocaleId = SOURCE.LocaleId
		);

		--------------------------
		if object_id ('tempdb..#Cte_TextAreaAttributeValue')is not null
			drop table #Cte_TextAreaAttributeValue

		---- Getting text area data in temp #Cte_TextAreaAttributeValue
		SELECT a.PimAttributeValueId, 
				b.LocaleId, 
				AttributeValue
		INTO #Cte_TextAreaAttributeValue
		FROM #ZnodePimAttributeValue AS a
		INNER JOIN #PimProductDetail_Bulk_Process AS b ON(a.PimAttributeId = b.PimAttributeId
														AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0)
														AND a.PimProductId = b.PimProductId)
		INNER JOIN #TBL_TextAreaAttributeId c ON(c.PimAttributeId = b.PimAttributeId);

		---- update ZnodePimProductAttributeTextAreaValue : attribute data for Product
		UPDATE TARGET
		SET TARGET.AttributeValue = SOURCE.AttributeValue, 
			TARGET.CreatedBy = @UserId, 
			TARGET.ModifiedBy = @UserId, 
			TARGET.ModifiedDate = @GetDate
		FROM ZnodePimProductAttributeTextAreaValue TARGET
		INNER JOIN #Cte_TextAreaAttributeValue SOURCE ON TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
																	AND TARGET.LocaleId = SOURCE.LocaleId;

		---- inserting TextAreaValue attribute data for Product
		INSERT INTO ZnodePimProductAttributeTextAreaValue
		(
			PimAttributeValueId, 
			LocaleId, 
			AttributeValue, 
			CreatedBy, 
			CreatedDate, 
			ModifiedBy, 
			ModifiedDate
		)
		SELECT SOURCE.PimAttributeValueId, 
				SOURCE.LocaleId, 
				SOURCE.AttributeValue, 
				@UserId, 
				@GetDate, 
				@UserId, 
				@GetDate
		FROM #Cte_TextAreaAttributeValue SOURCE
		WHERE NOT EXISTS
		(
			SELECT *
			FROM ZnodePimProductAttributeTextAreaValue TARGET
			WHERE TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
					AND TARGET.LocaleId = SOURCE.LocaleId
		);
           
		---- Getting attribute default values for product
		INSERT INTO #TBL_DefaultAttributeValue ( PimAttributeValueId, LocaleId, AttributeValue )
		SELECT a.PimAttributeValueId, b.LocaleId, d.PimAttributeDefaultValueId AttributeValue
		FROM #ZnodePimAttributeValue AS a
		INNER JOIN #PimProductDetail_Bulk_Process AS b ON(a.PimAttributeId = b.PimAttributeId
															AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0)
															AND a.PimProductId = b.PimProductId)
		INNER JOIN #TBL_DefaultAttributeId c ON(c.PimAttributeId = b.PimAttributeId)
		CROSS APPLY dbo.split(b.AttributeValue, ',') SP
		INNER JOIN ZnodePimAttributeDefaultValue d ON d.PimAttributeId = b.PimAttributeId
														AND SP.Item = d.AttributeDefaultValueCode;
		---- Deleting prodyuct attribute default value
		DELETE FROM ZnodePimProductAttributeDefaultValue
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM #TBL_DefaultAttributeValue TBLAV
			WHERE TBLAV.PimAttributeValueId = ZnodePimProductAttributeDefaultValue.PimAttributeValueId
					AND TBLAV.AttributeValue <> ZnodePimProductAttributeDefaultValue.PimAttributeDefaultValueId
					AND ZnodePimProductAttributeDefaultValue.LocaleId = @LocaleId
		);

		---- update ZnodePimProductAttributeDefaultValue : attribute data for Product
		UPDATE TARGET
		SET TARGET.PimAttributeDefaultValueId = SOURCE.AttributeValue, 
			TARGET.ModifiedBy = @UserId, 
			TARGET.ModifiedDate = @GetDate
		FROM ZnodePimProductAttributeDefaultValue TARGET
				INNER JOIN #TBL_DefaultAttributeValue SOURCE ON TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
																AND TARGET.PimAttributeDefaultValueId = SOURCE.AttributeValue
																AND TARGET.LocaleId = SOURCE.LocaleId;

		---- insert ZnodePimProductAttributeDefaultValue : attribute data for Product
		INSERT INTO ZnodePimProductAttributeDefaultValue
		(
			PimAttributeValueId, 
			LocaleId, 
			PimAttributeDefaultValueId, 
			CreatedBy, 
			CreatedDate, 
			ModifiedBy, 
			ModifiedDate
		)
		SELECT 
			SOURCE.PimAttributeValueId, 
			SOURCE.LocaleId, 
			SOURCE.AttributeValue, 
			@UserId, 
			@GetDate, 
			@UserId, 
			@GetDate
		FROM #TBL_DefaultAttributeValue SOURCE
		WHERE NOT EXISTS
		(
			SELECT *
			FROM ZnodePimProductAttributeDefaultValue TARGET
			WHERE TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
					AND TARGET.PimAttributeDefaultValueId = SOURCE.AttributeValue
					AND TARGET.LocaleId = SOURCE.LocaleId
		);
               

		IF OBJECT_ID('tempdb..#cte_ZnodePimAttributeValue') IS NOT NULL
			DROP TABLE #cte_ZnodePimAttributeValue;

		CREATE TABLE #cte_ZnodePimAttributeValue(PimAttributeValueId int, LocaleId int, AttributeValue nvarchar(max))

		CREATE INDEX Idx_cte_ZnodePimAttributeValue on #cte_ZnodePimAttributeValue(PimAttributeValueId, LocaleId)

		INSERT INTO #cte_ZnodePimAttributeValue (PimAttributeValueId, LocaleId, AttributeValue)
		SELECT a.PimAttributeValueId, b.LocaleId,AttributeValue                
		FROM #ZnodePimAttributeValue AS a
		INNER JOIN #PimProductDetail_Bulk_Process AS b ON(a.PimAttributeId = b.PimAttributeId
															AND ISNULL(a.PimAttributeFamilyId, 0) = ISNULL(b.PimAttributeFamilyId, 0)
															AND a.PimProductId = b.PimProductId)
		WHERE NOT EXISTS ( SELECT TOP 1 1 FROM #TBL_DefaultAttributeId TBLDA WHERE TBLDA.PimAttributeId = b.PimAttributeId )
		AND NOT EXISTS ( SELECT TOP 1 1 FROM #TBL_MediaAttributeId TBLMA WHERE TBLMA.PimAttributeId = b.PimAttributeId )
		AND NOT EXISTS ( SELECT TOP 1 1 FROM #TBL_TextAreaAttributeId TBLTA WHERE TBLTA.PimAttributeId = b.PimAttributeId );

		---- update ZnodePimAttributeValueLocale : attribute data for Product
		UPDATE TARGET
		SET TARGET.AttributeValue = SOURCE.AttributeValue, 
			TARGET.ModifiedBy = @UserId, 
			TARGET.ModifiedDate = @GetDate
		FROM ZnodePimAttributeValueLocale TARGET
		INNER JOIN #cte_ZnodePimAttributeValue SOURCE ON TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
														AND TARGET.LocaleId = SOURCE.LocaleId;

		---- inserting AttributeDefaultValue : attribute data for Product
		INSERT INTO ZnodePimAttributeValueLocale
		(
			PimAttributeValueId, 
			LocaleId, 
			AttributeValue, 
			CreatedBy, 
			CreatedDate, 
			ModifiedBy, 
			ModifiedDate
		)
		SELECT 
			SOURCE.PimAttributeValueId, 
			SOURCE.LocaleId, 
			SOURCE.AttributeValue, 
			@UserId, 
			@GetDate, 
			@UserId, 
			@GetDate
		FROM #cte_ZnodePimAttributeValue SOURCE
		WHERE NOT EXISTS
		(
			SELECT *
			FROM ZnodePimAttributeValueLocale TARGET
			WHERE TARGET.PimAttributeValueId = SOURCE.PimAttributeValueId
					AND TARGET.LocaleId = SOURCE.LocaleId
		);

		---- Inserting configurable products into ZnodePimConfigureProductAttribute
		INSERT INTO [ZnodePimConfigureProductAttribute]
		(PimProductId, 
			PimFamilyId, 
			PimAttributeId, 
			CreatedBy, 
			CreatedDate, 
			ModifiedBy, 
			ModifiedDate
		)
		SELECT DISTINCT PD.PimProductId, 
				NULL, 
				q.PimAttributeId, 
				@UserId, 
				@GetDate, 
				@UserId, 
				@GetDate
		FROM #PimProductDetail_Bulk_Process PD
			CROSS APPLY dbo.Split([ConfigureAttributeIds], ',') AS b
			INNER JOIN ZnodePimAttribute AS q ON(q.PimAttributeId = b.Item)
		WHERE NOT EXISTS
		(
			SELECT TOP 1 1
			FROM ZnodePimConfigureProductAttribute RTR
			WHERE RTR.PimProductId = PD.PimProductId
					AND RTR.PimAttributeId = q.PimAttributeId
		);

		IF @LocaleId = @DefaultLocaleId
		BEGIN 	 
			DECLARE @sqlt NVARCHAr(max) = ''
			DECLARE @AttributeCodeAtt VARCHAR(600) , @PimAttributeIdAttr int 

			DECLARE Cur_AttributeDataUpdate CURSOR FOR 

			SELECT b.AttributeCode , PimAttributeId 
			FROM INFORMATION_SCHEMA.COLUMNS a 
			INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )
			WHERE TABLE_NAME = 'ZnodePimProduct'
			AND IsCategory = 0 
			AND IsShowOnGrid = 1 
			AND EXISTS (SELECT TOP 1 1 FROM #PimProductDetail_Bulk_Process n  WHERE Replace(Replace(n.ProductAttributeCode, '[',''), ']','') = b.AttributeCode  )

			OPEN Cur_AttributeDataUpdate 
			FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
			WHILE @@FETCH_STATUS = 0 
			BEGIN 

			SET @sqlt = 'UPDATE a  
			SET '+@AttributeCodeAtt+'= AttributeValue 
			FROM ZnodePimProduct a 
			INNER JOIN #PimProductDetail_Bulk_Process m ON(m.PimProductId = a.pimProductId ) 
			WHERE  Replace(Replace(m.ProductAttributeCode, ''['',''''), '']'','''') = '''+@AttributeCodeAtt+'''
			' 

			EXEC (@sqlt)

			FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
			END 
			CLOSE Cur_AttributeDataUpdate
			DEALLOCATE Cur_AttributeDataUpdate 

		END 



		COMMIT TRAN ImportProducts;

		FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow;
		END;
		CLOSE cur_BulkData;
		DEALLOCATE cur_BulkData;
		-----------Added Performance patch end

		

		---- Update family of Product in table ZnodePimConfigureProductAttribute 
		UPDATE ZnodePimConfigureProductAttribute
		SET PimFamilyId = b.PimAttributeFamilyId
		FROM ZnodePimConfigureProductAttribute a
				INNER JOIN ZnodePimProduct b ON a.PimProductId = b.PimProductId;

		SET @GetDate = dbo.Fn_GetDate();
		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN ImportProducts;

		DELETE FROM ZnodePimConfigureProductAttribute  
		WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePimAttributeValue  a WHERE a.PimProductId = ZnodePimConfigureProductAttribute.PimProductId
		AND a.PimAttributeID = ZnodePimConfigureProductAttribute.PimAttributeID )
		--AND EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeValue a 
		--INNER JOIN ZnodePimAttribute ty ON (ty.PimAttributeId = a.PimAttributeId)
		--INNER JOIN ZnodePimProductAttributeDefaultValue t ON (t.PimAttributeValueId = a.PimAttributeValueId )
		--INNER JOIN ZnodePimAttributeDefaultValue y ON (y.PimAttributeDefaultValueId = t.PimAttributeDefaultValueId)
		--INNER JOIN View_loadmanageProductInternal  TU ON (TU.AttributeCode = 'SKU' AND TU.PimProductId = a.PimProductId  )
		--WHERE ty.AttributeCode = 'ProductType' AND y.AttributeDefaultValueCode = 'ConfigurableProduct'
		--AND a.PimProductId = ZnodePimConfigureProductAttribute.PimProductId
		AND EXISTS (SELECT TOP 1 1 FROM #PimProductDetail1 TM WHERE TM.PimProductId = ZnodePimConfigureProductAttribute.PimProductId )
   			
		----Delete simple products if inserted in table ZnodePimConfigureProductAttribute 
		DELETE FROM ZnodePimConfigureProductAttribute
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM ZnodePimAttributeValue a
					INNER JOIN ZnodePimAttribute ty ON(ty.PimAttributeId = a.PimAttributeId)
					INNER JOIN ZnodePimProductAttributeDefaultValue t ON(t.PimAttributeValueId = a.PimAttributeValueId)
					INNER JOIN ZnodePimAttributeDefaultValue y ON(y.PimAttributeDefaultValueId = t.PimAttributeDefaultValueId)
			WHERE ty.AttributeCode = 'ProductType'
					AND y.AttributeDefaultValueCode = 'SimpleProduct'
					AND a.PimProductId = ZnodePimConfigureProductAttribute.PimProductId
		);

	END TRY
    BEGIN CATCH
		ROLLBACK TRAN ImportProducts;
		INSERT INTO ZnodeImportLog(ErrorDescription,ColumnName,Data,RowNumber,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
		Select  46,ERROR_PROCEDURE(),ERROR_MESSAGE(),ERROR_LINE(),@newGUID,@UserId,@GetDate,@UserId,@GetDate, @ImportProcessLogId  
		
		SET @SQLQuery = ' Select @FailedRecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;

		EXEC sp_executesql @SQLQuery , N'@FailedRecordCount BIGINT out' , @FailedRecordCount =@FailedRecordCount out
		--SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS  NULL AND  ImportProcessLogId = @ImportProcessLogId;
		SELECT @SuccessRecordCount = 0
									
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount, TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
		WHERE ImportProcessLogId = @ImportProcessLogId;

		UPDATE ZnodeImportProcessLog
		SET STATUS = dbo.Fn_GetImportStatus(3), 
		ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

        SELECT ERROR_MESSAGE(),ERROR_LINE(),ERROR_PROCEDURE();
		-- UPDATE ZnodeImportProcessLog SET Status = dbo.Fn_GetImportStatus(3), ProcessCompletedDate = @GetDate WHERE ImportProcessLogId = @ImportProcessLogId;
		-- ROLLBACK TRAN ImportProducts;
    END CATCH;
END;