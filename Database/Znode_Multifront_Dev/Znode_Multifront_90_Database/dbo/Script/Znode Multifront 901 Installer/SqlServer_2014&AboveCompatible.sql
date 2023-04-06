
/****** Object:  StoredProcedure [dbo].[Znode_GetProductIdForPaging]    Script Date: 6/12/2017 12:10:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Znode_GetProductIdForPaging]
(
	@WhereClauseXML  XML           = NULL,
	@Rows            INT           = 10,
	@PageNo          INT           = 1,
	@Order_BY        VARCHAR(1000) = '',
	@RowsCount       INT OUT,
	@LocaleId        INT           = 1,
	@AttributeCode   VARCHAR(MAX)  = '',
	@PimProductId    TransferId READONLY , 
	@IsProductNotIn  BIT           = 0,
	@OutProductId    VARCHAR(max)	= 0 OUT )
AS	
 /* Summary :- This procedure is used to find the product ids with paging 
     Unit Testing
	 begin tran
	 DECLARE @ttr NVARCHAR(max)
	 DECLARE @PimProductId TransferId 
	 INSERT INTO @PimProductId
	 SELECT -1
     EXEC Znode_GetProductIdForPaging   N'' ,  10 ,  2 ,'',0, 1, '',@PimProductId ,0 ,@ttr OUT SELECT @ttr
	 rollback tran

	Create Index ZnodePimAttributeValue_ForPaging_Include ON  ZnodePimAttributeValue(PimAttributeId) include (PimAttributeValueId  ,PimProductId,CreatedDate,ModifiedDate )
	Create Index ZnodePimProductAttributeDefaultValue_ForPaging_Include ON  ZnodePimProductAttributeDefaultValue(PimAttributeValueId) include (PimAttributeDefaultValueId)
	Create Index ZnodePimFamilyGroupMapper_PimAttributeFamilyId ON ZnodePimFamilyGroupMapper(PimAttributeFamilyId,PimAttributeId)

	create  index IDX_ZnodePimAttributeValue_PimAttributeId on ZnodePimAttributeValue(PimAttributeId)
*/
BEGIN
 BEGIN TRY 
  SET NOCOUNT ON 
       DECLARE @SQL NVARCHAR(max) = '',@InternalSQL NVARCHAR(max) = ''
	   DECLARE @UseCtePart VARCHAR(1000) = ''
	   DECLARE @InternalOrderby VARCHAR(1000) = ''
	   DECLARE @InternalWhereClause NVARCHAr(MAx) = '',@InternalProductWhereClause NVARCHAr(MAx) = '',@InternalUpperWhereClause NVARCHAr(max)=''
	           ,@InternaleProductJoin NVARCHAr(MAx) = ''
	   DECLARE @TBL_PimProductId  TABLE (PimProductId INT  , CountNo INT)
	   DECLARE @TBL_DefaultAttributeId TABLE (PimAttributeId INT PRIMARY KEY , AttributeCode VARCHAR(600))
	  
	   DECLARE @DefaultLocaleId INT = dbo.FN_GetDefaultLocaleId()
	   DECLARE @PimProductIds TransferId 
	   INSERT INTO @TBL_DefaultAttributeId (PimAttributeId,AttributeCode)
	   SELECT PimAttributeId,AttributeCode FROM  [dbo].[Fn_GetDefaultAttributeId] ()

	   DECLARE @TBL_Attributeids TABLE(PimAttributeId INT , AttributeCode VARCHAr(600))

	   INSERT INTO @TBL_Attributeids (PimAttributeId, AttributeCode )
	   SELECT PimAttributeId,AttributeCode
	   FROM [dbo].[Fn_GetProductGridAttributes]() FNGA  
	   WHERE EXISTS (SELECT Top 1 1 FROM dbo.split(@AttributeCode , ',') SP WHERE sp.Item = FNGA.AttributeCode OR @AttributeCode = '' )

	    DECLARE @PimAttributeIds VARCHAR(max) = SUBSTRING((SELECT ','+CAST(PimAttributeId AS VARCHAR(50)) 
																		FROM @TBL_Attributeids FNGA  
																		FOR XML PATH ('') ) ,2,4000)

	   INSERT INTO @PimProductIds (id)
	   SELECT Id
	   FROM @PimProductId 


	   IF  EXISTS (SELECT TOP 1 1 FROM @PimProductId )  AND @IsProductNotIn = 1 
	   BEGIN 
	    	     SET @InternalWhereClause = ' WHERE NOT EXISTS ( SELECT TOP 1 1 FROM @TBL_ProductFilter TBLFP WHERE  TBLFP.PimProductId = ZPP.PimProductId )'
	   END 
	   ELSE IF @IsProductNotIn = 0 AND EXISTS (SELECT TOP 1 1 FROM @PimProductId )  
	   BEGIN 
		  SET @InternalWhereClause = ''
		  SET @InternalUpperWhereClause = ' A '
	   END 	  

	   SET @SQl = ' 
				DECLARE @TBL_ProductFilter TABLE (PimProductId INT PRIMARY KEY,RowId INT IDENTITY(1,1)  )
										   INSERT INTO @TBL_ProductFilter  (PimProductId )
											SELECT Id 
											FROM @TransferId

				DECLARE @TBL_PimProductId TABLE (PimProductId  INT PRIMARY KEY,RowId INT, CountNo INT)
				;With Cte_PimProductId AS
		               (  
							 SELECT ZPP.PimProductId , ZPP.PimAttributeFamilyId , '+CASE WHEN @InternalUpperWhereClause <> '' THEN ' TBPF.RowId ' ELSE ' 1 ' END+'  DisplayOrder 
							 FROM ZnodePimProduct ZPP 
							'+CASE WHEN @InternalUpperWhereClause <> '' AND @InternalWhereClause = '' THEN ' INNER JOIN @TBL_ProductFilter TBPF ON (TBPF.PimProductId = ZPP.PimProductId ) ' ELSE '' END +'
							 '+@InternalWhereClause+' 
					   ) '
	
	SET @InternalOrderby = dbo.FN_trim(REPLACE(REPLACE(@Order_By,'DESC',''),'ASC',''))
	
	IF EXISTS (SELECT TOP 1 1 FROM [dbo].[Fn_GetProductGridAttributes]() FN WHERE FN.AttributeCode = @InternalOrderby) OR @InternalOrderby = 'AttributeFamily'
	BEGIN
		--If @InternalOrderby = 'assortment'
		--Begin
		--	SET @InternalWhereClause = ' AttributeCode = ''SKU'' ' 
		--End
	 --   Else
		--Begin 
		SET @InternalWhereClause = 'AttributeCode = '''+@InternalOrderby+''''
		--END 
				SET @InternalOrderby = 'AttributeValue '+CASE WHEN @Order_By LIKE '% DESC' THEN 'DESC' ELSE 'ASC' END
	END
	ELSE IF  @InternalOrderby IN ('CreatedDate','ModifiedDate')
	BEGIN
	 SET @InternalOrderby = @Order_By
	 SET @InternalWhereClause = ' AttributeCode = ''SKU'' ' 
	END 
	ELSE 
	BEGIN 
	 SET @InternalOrderby = CASE WHEN @InternalOrderby = 'DisplayOrder' THEN @Order_By ELSE ' CTLA.PimProductId DESC ' END  
	 SET @InternalWhereClause = '' 
	END    
	  
    IF CAST(@WhereClauseXML AS NVARCHAR(max)) NOT LIKE '%AttributeCode%' AND @InternalWhereClause = '' 
	BEGIN 

	 SET @SQL = @SQL + ' , Cte_FilterData AS 
								( SELECT PimProductId ,'+[dbo].[Fn_GetPagingRowId](@InternalOrderby,'PimProductId DESC')+',Count(*)Over() CountNo
								   FROM Cte_PimProductId CTLA
								 )
								 INSERT INTO  @TBL_PimProductId  (PimProductId,RowId,CountNo )
								 SELECT DISTINCT PimProductId,RowId ,CountNo
								 FROM Cte_FilterData 
								 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)
	END 
	ELSE IF CAST(@WhereClauseXML AS NVARCHAR(max)) NOT LIKE '%AttributeCode%' AND @InternalWhereClause <> '' 
	BEGIN

						 
	   SET @InternalSQL = '
						   DECLARE @TBL_FamilyLocale  TABLE(PimAttributeFamilyId INT PRIMARY KEY ,FamilyCode NVARCHAR(600),IsSystemDefined BIT,IsDefaultFamily BIT,IsCategory BIT ,AttributeFamilyName NVARCHAR(max) ) 
						   DECLARE @TBL_DefaultValue  TABLE(PimAttributeId INT,AttributeDefaultValueCode NVARCHAR(600),IsEditable BIT,AttributeDefaultValue NVARCHAR(max),DisplayOrder INT,PimAttributeDefaultValueId INT 
						   index ind_101 (PimAttributeDefaultValueId))
						   
						   INSERT INTO @TBL_FamilyLocale(PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName)
						   EXEC [dbo].[Znode_GetFamilyValueLocale] '''','+CAST(@LocaleId AS VARCHAR(50))+'	
						   
						   INSERT INTO @TBL_DefaultValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder,PimAttributeDefaultValueId)
						   EXEC [dbo].[Znode_GetAttributeDefaultValueLocaleNew] '''+@PimAttributeIds+''','+CAST(@LocaleId AS VARCHAR(50))+'	
							
							'

							SET @SQL = @SQL + ' ,Cte_getAttributeValue AS 
									(	
										SELECT CTP.PimProductId , ZPA.AttributeCode  ,ZPAV.PimAttributeValueId  ,ZPA.PimAttributeId
										,ZPAV.CreatedDate,ZPAV.ModifiedDate,CTP.DisplayOrder     
										FROM Cte_PimProductId CTP             
										INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = CTP.PimProductId)             
										INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)    
										INNER JOIN ZnodePimFamilyGroupMapper ZPFGM ON (ZPFGM.PimAttributeFamilyId = CTP.PimAttributeFamilyId                      
										AND ZPFGM.PimAttributeId = ZPA.PimAttributeId)   
									)
		                              , Cte_CollectData AS 
									( 
										SELECT ZPA.PimProductId , ZPA.AttributeCode  ,  ZPAVL.AttributeValue ,ZPA.CreatedDate,ZPA.ModifiedDate,ZPA.DisplayOrder             
										FROM Cte_getAttributeValue   ZPA INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPA.PimAttributeValueId = ZPAVL.PimAttributeValueId ) 
										WHERE  ZPA.PimAttributeId IN ('+@PimAttributeIds+') AND ZPAVL.LocaleId in  (' + CAST(@DefaultLocaleId AS VARCHAR(50)) +')
										
										UNION ALL 
										
										SELECT CTP.PimProductId , ''AttributeFamily'', TBFM.AttributeFamilyName AttributeValue,NULL,NULL,CTP.DisplayOrder
										FROM Cte_PimProductId CTP INNER JOIN @TBL_FamilyLocale  TBFM ON (TBFM.PimAttributeFamilyId = CTP.PimAttributeFamilyId)
										

										UNION ALL 
										
										SELECT DISTINCT PimProductId, AttributeCode , SUBSTRING((SELECT '',''+AttributeDefaultValue 
										FROM @TBL_DefaultValue TBDV WHERE (TBDV.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )                            
										FOR XML PATH ('''')),2,4000) AttributeValue  , CTETY.CreatedDate,CTETY.ModifiedDate,DisplayOrder         
										FROM ZnodePimProductAttributeDefaultValue ZPPADV INNER JOIN Cte_getAttributeValue CTETY 
										ON (CTETY.PimAttributeValueId = ZPPADV.PimAttributeValueId)
										WHERE  CTETY.PimAttributeId IN ('+@PimAttributeIds+')
							 )
							  , Cte_GetAttributeValueI AS 
							  (
							    SELECT PimProductId,'+CASE WHEN @InternalOrderby LIKE '%DisplayOrder%' THEN 'DisplayOrder' ELSE   'AttributeValue'  END+'  , 1 DefaultOrderBy      
								FROM  Cte_CollectData CTCD
								WHERE '+@InternalWhereClause+'
								UNION ALL 
								SELECT CTP.PimProductId , NULL , 2 
								FROM Cte_PimProductId CTP 	
								where NOT EXISTS (SELECT TOP 1 1 FROM  Cte_CollectData CTCD
								WHERE '+@InternalWhereClause+' AND CTCD.PimProductId = CTP.PimProductId  )				  
							  )
							   , Cte_FilterData As 
								 (
								   SELECT DISTINCT PimProductId,'+[dbo].[Fn_GetPagingRowId](' DefaultOrderBy , '+@InternalOrderby+' ','PimProductId DESC')+'
								   FROM Cte_GetAttributeValueI CTCD
							 )
								  ,Cte_GetAllData AS 
								 (
								  SELECT PimProductId ,RowId ,Count(*)Over() CountNo
								  FROM Cte_FilterData
								  GROUP BY PimProductId ,RowId 
							
								  
								 )

								 INSERT INTO  @TBL_PimProductId  (PimProductId,RowId,CountNo )
								 SELECT  PimProductId,RowId ,CountNo
								 FROM  Cte_GetAllData
								 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)

		SET @SQL =  @InternalSQL + @SQL
			
	    END
		ELSE 
		BEGIN 
		  SET @InternalSQL = ''
		  DECLARE @AttachINDefault  VARCHAr(max) = ''
		  SET  @InternalProductWhereClause = 
							STUFF( (  SELECT ' INNER JOIN Cte_AttributeValueLocale AS CTAL'+CAST(ID AS VARCHAR(50))
							+' ON ( CTAL'+CAST(ID AS VARCHAR(50))+'.PimProductId = CTAL'+CASE WHEN ID-1= 0 THEN '' ELSE CAST(ID-1 AS VARCHAR(50)) END +'.PimProductId AND '+
							REPLACE(REPLACE(WhereClause,'AttributeCode ',' CTAL'+CAST(ID AS VARCHAR(50))+'.AttributeCode '),' AttributeValue ',' CTAL'+CAST(ID AS VARCHAR(50))+'.AttributeValue ')+' )'
							FROM dbo.Fn_GetWhereClauseXML(@WhereClauseXML)      
							FOR XML PATH (''), TYPE).value('.', ' Nvarchar(max)'), 1, 0, '')

          SET @SQL = @SQL + '   ,Cte_getAttributeValue AS 
										(	
											 SELECT CTP.PimProductId , ZPA.AttributeCode  ,ZPAV.PimAttributeValueId  ,ZPA.PimAttributeId
																,ZPAV.CreatedDate,ZPAV.ModifiedDate,CTP.DisplayOrder     
											 FROM Cte_PimProductId CTP             
											 INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = CTP.PimProductId)       
											 INNER JOIN ZnodePimAttribute ZPA ON (ZPAV.PimAttributeId = ZPA.PimAttributeId)    
											 INNER JOIN ZnodePimFamilyGroupMapper ZPFGM ON 
											 (CTP.PimAttributeFamilyId = ZPFGM.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPA.PimAttributeId) 
											 WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split('''+@AttributeCode+''','','') SP WHERE Sp.Item = ZPA.AttributeCode  OR '''+@AttributeCode+''' = '''')
										)'
		  IF EXISTS (SELECT Top 1 1 FROM @TBL_Attributeids TBH WHERE NOT EXISTS (SELECT Top 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBH.PimAttributeId)
		  AND TBH.AttributeCode <> 'AttributeFamily' )
         BEGIN 
		    SET @SQL = @SQL+' , Cte_CollectData AS 
										( 
											SELECT ZPA.PimProductId , ZPA.AttributeCode  ,  ZPAVL.AttributeValue,ZPAVL.LocaleId ,
												   ZPA.PimAttributeId,ZPA.CreatedDate,ZPA.ModifiedDate,ZPA.DisplayOrder             
											FROM Cte_getAttributeValue ZPA          
											INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPA.PimAttributeValueId  = ZPAVL.PimAttributeValueId )
											where ZPA.PimAttributeId IN ('+@PimAttributeIds+')
											AND  ZPAVL.LocaleId IN ( '+CAST(@LocaleId AS VARCHAR(50))+','+CAST(@DefaultLocaleId AS VARCHAR(50))+')
										)
									, Cte_FilterDataLocale AS 
									(
										   SELECT PimProductId,AttributeCode,AttributeValue,PimAttributeId,CreatedDate,ModifiedDate,DisplayOrder
										   FROM Cte_CollectData
										   WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'
										   UNION All 
										   SELECT M.PimProductId,M.AttributeCode,M.AttributeValue ,M.PimAttributeId,M.CreatedDate,M.ModifiedDate,M.DisplayOrder
										   FROM Cte_CollectData M 
										   WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
										   AND NOT Exists (Select TOP 1 1  from Cte_CollectData X where  M.PimProductId = X.PimProductId AND M.AttributeCode = X.AttributeCode And X.localeId =  '+CAST(@LocaleId AS VARCHAR(50))+' )
									)'

						SET @AttachINDefault =			'SELECT PimProductId,AttributeCode,CTFD.AttributeValue ,CreatedDate,ModifiedDate,CTFD.DisplayOrder	 
										FROM Cte_FilterDataLocale CTFD'

		 END 
		 
		 IF EXISTS (SELECT Top 1 1 FROM @TBL_Attributeids TBH WHERE EXISTS (SELECT Top 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBH.PimAttributeId))
         BEGIN 
		 SET  @InternalSQL = 'DECLARE @TBL_DefaultValue  TABLE(PimAttributeId INT,AttributeDefaultValueCode NVARCHAR(600),IsEditable BIT,AttributeDefaultValue NVARCHAR(max),DisplayOrder INT,PimAttributeDefaultValueId INT  ) 
		                     INSERT INTO @TBL_DefaultValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder,PimAttributeDefaultValueId)
						     EXEC [dbo].[Znode_GetAttributeDefaultValueLocaleNew] '''+@PimAttributeIds+''','+CAST(@LocaleId AS VARCHAR(50))
	   
			  SET @AttachINDefault = CASE WHEN @AttachINDefault = '' THEN '' ELSE @AttachINDefault+' UNION ALL ' END + ' SELECT PimProductId, AttributeCode , 
										SUBSTRING(
											(SELECT '',''+AttributeDefaultValue FROM @TBL_DefaultValue TBDV 
											WHERE (TBDV.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )                            
											FOR XML PATH (''''))
										,2,4000) 
										AttributeValue  , CTETY.CreatedDate,CTETY.ModifiedDate,DisplayOrder         
										FROM Cte_getAttributeValue CTETY
										INNER JOIN  ZnodePimProductAttributeDefaultValue ZPPADV ON (CTETY.PimAttributeValueId =ZPPADV.PimAttributeValueId  )
										WHERE  CTETY.PimAttributeId IN ('+@PimAttributeIds+') '

		 END
		 IF EXISTS (SELECT Top 1 1 FROM @TBL_Attributeids TBH WHERE AttributeCode = 'AttributeFamily')
         BEGIN 
		 SET  @InternalSQL = @InternalSQL +' DECLARE @TBL_FamilyLocale  TABLE(PimAttributeFamilyId INT PRIMARY KEY ,FamilyCode NVARCHAR(600),IsSystemDefined BIT,IsDefaultFamily BIT,IsCategory BIT ,AttributeFamilyName NVARCHAR(max) , index ind_103(PimAttributeFamilyId)) 
		                    INSERT INTO @TBL_FamilyLocale(PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName)
						   EXEC [dbo].[Znode_GetFamilyValueLocale] '''','+CAST(@LocaleId AS VARCHAR(50))
						   
			SET @AttachINDefault = CASE WHEN @AttachINDefault = '' THEN '' ELSE @AttachINDefault+' UNION ALL '	END + 'SELECT CTP.PimProductId , ''AttributeFamily'', TBFM.AttributeFamilyName AttributeValue,NULL, NULL,DisplayOrder	
										FROM Cte_PimProductId CTP 
										INNER JOIN @TBL_FamilyLocale  TBFM ON (TBFM.PimAttributeFamilyId = CTP.PimAttributeFamilyId)'
		 END
		

								--SET  @InternalProductWhereClause =' INNER JOIN Cte_AttributeValueLocale AS CTAL1 ON ( CTAL1.PimProductId = CTAL.PimProductId AND  CTAL1.AttributeCode = ''ProductType'' and CTAL1.AttributeValue  = ''Simple Product'' )'
								--Print '---------1--------'
								--Print @SQL
								--Print '---------2--------'
		  
									--Print @SQL
									--Print '---------3--------'
									SET @SQL = @SQL + ' 
									,Cte_AttributeValueLocale AS
									(
									 '+@AttachINDefault+'										
									)'
									--Print @SQL
									--Print '---------4--------'
									SET @SQL = @SQL + ' 

									, Cte_AttributeLocale AS
									(
										 SELECT CTAL.PimProductId 
										 FROM Cte_AttributeValueLocale  CTAL
										 '+@InternalProductWhereClause+'
										 GROUP BY CTAL.PimProductId 
									)
									, Cte_FinalProductData AS
									(
									   SELECT DISTINCT CTLA.PimProductId, 1 DefaultOrderBy  ,'+[dbo].[Fn_GetPagingRowId](@InternalOrderby,' CTLA.PimProductId DESC')+' 
									   FROM Cte_AttributeValueLocale  CTAVL
									   INNER JOIN Cte_AttributeLocale CTLA ON (CTLA.PimProductId = CTAVL.PimProductId)
							        	'+CASE WHEN @InternalWhereClause <> '' THEN  ' WHERE CTAVL.'+dbo.FN_Trim(@InternalWhereClause) ELSE '' END +'
									)
									,Cte_GEtSortingProduct AS 
									(
									  SELECT PimProductId, DefaultOrderBy,RowId 
									  FROM Cte_FinalProductData 
									  UNION ALL 
									  SELECT PimProductId ,2 DefaultOrderBy,'+[dbo].[Fn_GetPagingRowId](' DERE.PimProductId ',' DERE.PimProductId ')+'
									  FROM Cte_AttributeLocale DERE 
									  WHERE NOT EXISTS (SELECT TOP 1 1 FROM Cte_FinalProductData TTRR WHERE TTRR.PimProductId = DERE.PimProductId  ) 
									  
									 )
									 ,Cte_getPagingData 
									 AS 
									 (
									  SELECT PimProductId ,'+[dbo].[Fn_GetPagingRowId](' DefaultOrderBy ',' RowId ')+' ,Count(*)Over() CountNo
									  FROM Cte_GEtSortingProduct
									 )

									,Cte_GetAllData AS 
									(
									  SELECT PimProductId ,RowId ,CountNo
									  FROM Cte_getPagingData
									  GROUP BY PimProductId ,RowId ,CountNo
									)
									'
									--Print @SQL
									--Print '---------5--------'
									SET @SQL = @SQL + ' 

								 INSERT INTO  @TBL_PimProductId  (PimProductId,RowId,CountNo )
								 SELECT  PimProductId,RowId , CountNo
								 FROM Cte_GetAllData '
								 +[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)

			SET @SQl = @InternalSQL+@SQL
     		END
			SET @SQl = @SQl + ' 
								SET @OutProductId = ISNULL(SUBSTRING((SELECT '',''+CAST(PimProductid AS VARCHAR(50)) 
										FROM @TBL_PimProductId TBPP
										ORDER BY RowId
										FOR XML PATH ('''')   ),2,4000),'''')
								SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM  @TBL_PimProductId TBPP),0)
							 '
			
			--PRINT  @SQl
          --  SELECT  @SQl
		
			EXEC SP_EXECUTESQl  @SQL ,N' @OutProductId VARCHAR(max) OUT ,@RowsCount INT OUT ,@TransferId TransferId READONLY  ',  @OutProductId = @OutProductId OUT, @RowsCount = @RowsCount OUT ,@TransferId  = @PimProductIds
		

	 -- SELECT  @OutProductId,@RowsCount

     END TRY 
    BEGIN CATCH
	 DECLARE @Status BIT ;
		     SET @Status = 0;
			 SELECT ERROR_MESSAGE()
		  --   DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductIdForPaging @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@AccountList='+CAST(@AccountList AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
    --         SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
    --         EXEC Znode_InsertProcedureErrorLog
				--@ProcedureName = 'Znode_GetProductIdForPaging',
				--@ErrorInProcedure = @Error_procedure,
				--@ErrorMessage = @ErrorMessage,
				--@ErrorLine = @ErrorLine,
				--@ErrorCall = @ErrorCall;
	 END CATCH 
     END

	 GO 

ALTER PROCEDURE [dbo].[Znode_ImportPimProductData]
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
             --BEGIN TRAN ImportProducts;
             DECLARE @SQLQuery NVARCHAR(MAX);
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
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
                 END;
             ELSE
                 BEGIN
                     INSERT INTO @FamilyAttributeDetail(PimAttributeId,AttributeTypeName,AttributeCode,SourceColumnName,IsRequired,PimAttributeFamilyId)
                     --Call Process to insert data of defeult family with cource column name and target column name 
                     EXEC Znode_ImportGetTemplateDetails @TemplateId = @TemplateId,@IsValidationRules = 0,@IsIncludeRespectiveFamily = 1,@DefaultFamilyId = @DefaultFamilyId;
                 END;  

            -- Retrive PimProductId on the basis of SKU for update product 
			SET @SQLQuery = 'UPDATE tlb SET tlb.PimProductId = ZPAV.PimProductId 
							FROM ZnodePimAttributeValue AS ZPAV INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL ON 
							(ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId) 
							INNER JOIN [dbo].[ZnodePimAttribute] ZPA on ZPAV.PimAttributeId = ZPA.PimAttributeId AND ZPA.AttributeCode= ''SKU'' 
							INNER JOIN '+@TableName+' tlb ON ZPAVL.AttributeValue = ltrim(rtrim(tlb.SKU)) ';
			EXEC sys.sp_sqlexec	@SQLQuery	 	
				 	
					
             --Read all attribute details with their datatype 
			 IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#DefaultAttributeValue')
				BEGIN
					   --CREATE TABLE #DefaultAttributeValue (AttributeTypeName  VARCHAR(300),PimAttributeDefaultValueId INT,PimAttributeId INT,
					   --AttributeDefaultValueCode  VARCHAR(100));
					   -- ELSE 
					    CREATE TABLE #DefaultAttributeValue (AttributeTypeName  VARCHAR(300),PimAttributeDefaultValueId INT,PimAttributeId INT,
					    AttributeDefaultValueCode  VARCHAR(100)
					    Index Ix_Default (PimAttributeId, AttributeDefaultValueCode));
					   --IF @@VERSION LIKE '%Azure%' OR @@VERSION LIKE '%Express Edition%'
					   --Begin
						  --Select 'Without Index'
					   --END
					   --Else
						  --Alter TABLE #DefaultAttributeValue ADD Index Ix_Default (PimAttributeId, AttributeDefaultValueCode);
					


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
          
             -- Split horizontal table into verticle table by column name and attribute Value with their 
             -- corresponding AttributeId, Default family , Default AttributeValue Id  
             DECLARE @PimProductDetail TABLE 
			 (
			      
				 PimAttributeId INT, PimAttributeFamilyId INT,ProductAttributeCode VARCHAR(300) NULL,
				  ProductAttributeDefaultValueId INT NULL,PimAttributeValueId  INT NULL,LocaleId INT,
				  PimProductId INT NULL,AttributeValue NVARCHAR(MAX) NULL,AssociatedProducts NVARCHAR(4000) NULL,ConfigureAttributeIds VARCHAR(2000) NULL,
				  ConfigureFamilyIds VARCHAR(2000) NULL,RowNumber INT  INDEX Ix CLUSTERED (RowNumber) 
                );

			--DECLARE @PimProductDetail TABLE 
			-- (
			      
			--	  PimAttributeId INT, PimAttributeFamilyId INT,ProductAttributeCode VARCHAR(300) NULL,
			--	  ProductAttributeDefaultValueId INT NULL,PimAttributeValueId  INT NULL,LocaleId INT,
			--	  PimProductId INT NULL,AttributeValue NVARCHAR(MAX) NULL,AssociatedProducts NVARCHAR(4000) NULL,ConfigureAttributeIds VARCHAR(2000) NULL,
			--	  ConfigureFamilyIds VARCHAR(2000) NULL,RowNumber INT  
   --             );

		
			
             -- Column wise split data from source table ( global temporary table ) and inserted into temporary table variable @PimProductDetail
             -- Add PimAttributeDefaultValue 
             DECLARE Cr_AttributeDetails CURSOR LOCAL FAST_FORWARD
             FOR SELECT PimAttributeId,AttributeTypeName,AttributeCode,IsRequired,SourceColumnName,PimAttributeFamilyId FROM @FamilyAttributeDetail  WHERE ISNULL(SourceColumnName, '') <> '';
             OPEN Cr_AttributeDetails;
             FETCH NEXT FROM Cr_AttributeDetails INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @PimAttributeFamilyId;
             WHILE @@FETCH_STATUS = 0
                 BEGIN
                    SET @NewProductId = 0;
                    SET @SQLQuery = ' SELECT '''+CONVERT(VARCHAR(100), @PimAttributeFamilyId)+''' PimAttributeFamilyId , PimProductId PimProductId ,'''+CONVERT(VARCHAR(100), @AttributeId)+''' AttributeId ,
									(SELECT TOP 1  PimAttributeDefaultValueId FROM #DefaultAttributeValue Where PimAttributeId =  '
									+ CONVERT(VARCHAR(100), @AttributeId)+'AND  AttributeDefaultValueCode = TN.'+@SourceColumnName+' ) PimAttributeDefaultValueId ,'
									+ @SourceColumnName+','+CONVERT(VARCHAR(100), @LocaleId)+'LocaleId , RowNumber FROM '+@TableName+' TN';
                    INSERT INTO @PimProductDetail( PimAttributeFamilyId, PimProductId, PimAttributeId, ProductAttributeDefaultValueId, AttributeValue, LocaleId, RowNumber )
				EXEC sys.sp_sqlexec @SQLQuery;
                    FETCH NEXT FROM Cr_AttributeDetails INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @PimAttributeFamilyId;
                 END;
             CLOSE Cr_AttributeDetails;
             DEALLOCATE Cr_AttributeDetails;
             -- In case of Yes/No : If value is not TRUE OR  1 then it will be  False else True
			 --If default Value set not need of hard code for IsActive
			 UPDATE ppdti SET ppdti.AttributeValue = CASE WHEN Upper(ISNULL(ppdti.AttributeValue, '')) in ( 'TRUE','1')  THEN 'true'  ELSE 'false' END FROM @PimProductDetail ppdti
                INNER JOIN #DefaultAttributeValue dav ON ppdti.PimAttributeId = dav.PimAttributeId WHERE   dav.AttributeTypeName = 'Yes/No';

             -- Pass product records one by one 
             DECLARE @IncrementalId INT= 1;
             DECLARE @SequenceId INT=
             (
                 SELECT MAX(RowNumber) FROM @PimProductDetail
             );
             DECLARE @PimProductDetailToInsert PIMPRODUCTDETAIL;  --User define table type to pass multiple records of product in single step

             WHILE @IncrementalId <= @SequenceId
                 BEGIN
					   	INSERT INTO @PimProductDetailToInsert(PimAttributeId,PimAttributeFamilyId,ProductAttributeCode,ProductAttributeDefaultValueId,
						PimAttributeValueId,LocaleId,PimProductId,AttributeValue,AssociatedProducts,ConfigureAttributeIds,ConfigureFamilyIds)
						SELECT PimAttributeId,PimAttributeFamilyId,ProductAttributeCode,ProductAttributeDefaultValueId,PimAttributeValueId,LocaleId,
						PimProductId,AttributeValue,AssociatedProducts,ConfigureAttributeIds,ConfigureFamilyIds FROM @PimProductDetail
						WHERE [@PimProductDetail].RowNumber = @IncrementalId; --AND RTRIM(LTRIM(AttributeValue)) <> '';

						Delete from @PimProductDetailToInsert where RTRIM(LTRIM(AttributeValue)) = '';
	                    --ORDER BY [@PimProductDetail].RowNumber;
                        ----Call process to finally insert data into 
                        ----------------------------------------------------------
						--1. [dbo].[ZnodePimProduct]
						--2. [dbo].[ZnodePimAttributeValue]
						--3. [dbo].[ZnodePimAttributeValueLocale]
						if Exists (select TOP 1 1 from @PimProductDetailToInsert)
							EXEC [Znode_ImportInsertUpdatePimProduct] @PimProductDetail = @PimProductDetailToInsert,@UserID = @UserID,@status = @status OUT,@IsNotReturnOutput = 1;
						DELETE FROM @PimProductDetailToInsert;
						SET @IncrementalId = @IncrementalId + 1;
						
                 END;
             UPDATE ZnodeImportProcessLog SET Status = dbo.Fn_GetImportStatus(2), ProcessCompletedDate = @GetDate WHERE ImportProcessLogId = @ImportProcessLogId;
            -- COMMIT TRAN ImportProducts;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE(),ERROR_LINE(),ERROR_PROCEDURE();
             UPDATE ZnodeImportProcessLog SET Status = dbo.Fn_GetImportStatus(3), ProcessCompletedDate = @GetDate WHERE ImportProcessLogId = @ImportProcessLogId;
            -- ROLLBACK TRAN ImportProducts;
         END CATCH;
     END;

	 GO 
	 
ALTER PROCEDURE [dbo].[Znode_ImportCatalogCategory](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200), @PimCatalogId int= 0)
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import Catalog Category Product association
	
	-- Unit Testing : 
	--BEGIN TRANSACTION;
	--update ZnodeGlobalSetting set FeatureValues = '5' WHERE FeatureName = 'InventoryRoundOff' 
	--    DECLARE @status INT;
	--    EXEC [Znode_ImportInventory] @InventoryXML = '<ArrayOfImportInventoryModel>
	-- <ImportInventoryModel>
	--   <SKU>S1002</SKU>
	--   <Quantity>999998.33</Quantity>
	--   <ReOrderLevel>10</ReOrderLevel>
	--   <RowNumber>1</RowNumber>
	--   <ListCode>TestInventory</ListCode>
	--   <ListName>TestInventory</ListName>
	-- </ImportInventoryModel>
	--</ArrayOfImportInventoryModel>' , @status = @status OUT , @UserId = 2;
	--    SELECT @status;
	--    ROLLBACK TRANSACTION;
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max);
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		-- Retrive RoundOff Value from global setting 
		DECLARE @InsertCatalogCategory TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, SKU varchar(300), CategoryName varchar(200), DisplayOrder int, IsActive bit, GUID nvarchar(400),
			Index Ind_SKU1 (SKU),Index Ind_CategoryName (CategoryName)
		);

		DECLARE @CategoryAttributId int;

		SET @CategoryAttributId =
		(
			SELECT TOP 1 PimAttributeId
			FROM ZnodePimAttribute AS ZPA
			WHERE ZPA.AttributeCode = 'CategoryName' AND 
				  ZPA.IsCategory = 1
		);

		DECLARE @InventoryListId int;

		SET @SSQL = 'Select RowNumber,SKU,CategoryName,DisplayOrder ,IsActive,GUID FROM '+@TableName;
		INSERT INTO @InsertCatalogCategory( RowNumber, SKU, CategoryName, DisplayOrder, IsActive, GUID )
		EXEC sys.sp_sqlexec @SSQL;


		--@MessageDisplay will use to display validate message for input inventory value  
		DECLARE @SKU TABLE
		( 
		   SKU nvarchar(300), PimProductId int, Index Ins_SKU (SKU)
		);
		INSERT INTO @SKU
			   SELECT b.AttributeValue, a.PimProductId
			   FROM ZnodePimAttributeValue AS a
					INNER JOIN
					ZnodePimAttributeValueLocale AS b
					ON a.PimAttributeId = dbo.Fn_GetProductSKUAttributeId() AND 
					   a.PimAttributeValueId = b.PimAttributeValueId;


		DECLARE @CategoryName TABLE
		( 
			CategoryName nvarchar(300), PimCategoryId int index ind_101 (CategoryName)
		);
		INSERT INTO @CategoryName
			   SELECT ZPCAL.CategoryValue, ZPCA.PimCategoryId
			   FROM ZnodePimCategoryAttributeValue AS ZPCA
					INNER JOIN
					ZnodePimCategoryAttributeValueLocale AS ZPCAL
					ON ZPCA.PimAttributeId = 5 AND 
					ZPCA.PimCategoryAttributeValueId = ZPCAL.PimCategoryAttributeValueId;
					
		-- start Functional Validation 
		
		-----------------------------------------------
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'SKU', SKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertCatalogCategory AS ii
			   WHERE ii.SKU NOT in 
			   (
				   SELECT SKU FROM @SKU 
			   );
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'CategoryName', CategoryName, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertCatalogCategory AS ii
			   WHERE ii.CategoryName NOT in 
			   (
				   SELECT CategoryName FROM @CategoryName 
			   );
		-- End Function Validation 	
		-----------------------------------------------
		--- Delete Invalid Data after functional validatin  
		DELETE FROM @InsertCatalogCategory
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);

		IF(ISNULL(@PimCatalogId, 0) <> 0)
		BEGIN
			WITH Cte_CategorySKUAssociation
				 AS(SELECT SKU.PimProductId, 
				   (Select top 1 PimCategoryId from @CategoryName where ICC.CategoryName = CategoryName )  
				   PimCategoryId
				   , DisplayOrder, IsActive FROM @InsertCatalogCategory AS ICC INNER JOIN @SKU AS SKU ON ICC.SKU = SKU.SKU)
				 MERGE INTO ZnodePimCatalogCategory TARGET
				 USING Cte_CategorySKUAssociation SOURCE
				 ON( TARGET.PimCategoryId = SOURCE.PimCategoryId AND 
					 Target.PimCatalogId = @PimCatalogId
				   )
				 WHEN MATCHED
					   THEN UPDATE SET TARGET.PimProductId = SOURCE.PimProductId, TARGET.IsActive = SOURCE.IsActive, TARGET.DisplayOrder = SOURCE.DisplayOrder, TARGET.CreatedBy = @UserId, TARGET.CreatedDate = @GetDate, TARGET.ModifiedBy = @UserId, TARGET.ModifiedDate = @GetDate
				 WHEN NOT MATCHED
					   THEN INSERT(PimCatalogId, PimCategoryId, PimProductId, IsActive, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUES( @PimCatalogId, SOURCE.PimCategoryId, SOURCE.PimProductId, SOURCE.IsActive, SOURCE.DisplayOrder, @UserId, @GetDate, @UserId, @GetDate );
		END;
		ELSE
		BEGIN
		  
		  -- INSERT into ZnodePimCategoryProduct ( PimProductId, PimCategoryId, Status, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) 
		  ----VALUES(  SOURCE.PimCategoryId, SOURCE.PimProductId, SOURCE.IsActive, SOURCE.DisplayOrder, @UserId, @GetDate, @UserId, @GetDate );
		  --SELECT SKU.PimProductId, (Select top 1 PimCategoryId from @CategoryName where ICC.CategoryName = CategoryName )  PimCategoryId
				-- , IsActive , DisplayOrder , @UserId, @GetDate, @UserId, @GetDate FROM @InsertCatalogCategory AS ICC INNER JOIN	 @SKU AS SKU ON ICC.SKU = SKU.SKU 

			WITH Cte_CategorySKUAssociation
				 AS
				 (
					   SELECT SKU.PimProductId, (Select top 1 PimCategoryId from @CategoryName where ICC.CategoryName = CategoryName )  PimCategoryId
					   , DisplayOrder, IsActive FROM @InsertCatalogCategory AS ICC INNER JOIN	 @SKU AS SKU ON ICC.SKU = SKU.SKU )

					   MERGE INTO ZnodePimCategoryProduct TARGET
					   USING Cte_CategorySKUAssociation SOURCE
					   ON 
					   ( TARGET.PimCategoryId = SOURCE.PimCategoryId ) 
					   WHEN MATCHED
					   THEN UPDATE SET TARGET.PimProductId = SOURCE.PimProductId, TARGET.Status = SOURCE.IsActive, TARGET.DisplayOrder = SOURCE.DisplayOrder, TARGET.CreatedBy = @UserId, TARGET.CreatedDate = @GetDate, TARGET.ModifiedBy = @UserId, TARGET.ModifiedDate = @GetDate
					   WHEN NOT MATCHED
					   THEN INSERT( PimCategoryId, PimProductId, Status, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUEs
					   (  SOURCE.PimCategoryId, SOURCE.PimProductId, SOURCE.IsActive, SOURCE.DisplayOrder, @UserId, @GetDate, @UserId, @GetDate );
		END;										 
		--select 'End'
		--      SET @Status = 1;
		UPDATE ZnodeImportProcessLog
		  SET Status = dbo.Fn_GetImportStatus( 2 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN A;
	END TRY
	BEGIN CATCH

		UPDATE ZnodeImportProcessLog
		  SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();
		ROLLBACK TRAN A;
	END CATCH;
END;

GO 

ALTER PROCEDURE [dbo].[Znode_ImportInventory_Ver1](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200))
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import Inventory data 
	--		   Input data in XML format Validate data with all scenario 
	-- Unit Testing : 
	--BEGIN TRANSACTION;
	--update ZnodeGlobalSetting set FeatureValues = '5' WHERE FeatureName = 'InventoryRoundOff' 
	--    DECLARE @status INT;
	--    EXEC [Znode_ImportInventory] @InventoryXML = '<ArrayOfImportInventoryModel>
	-- <ImportInventoryModel>
	--   <SKU>S1002</SKU>
	--   <Quantity>999998.33</Quantity>
	--   <ReOrderLevel>10</ReOrderLevel>
	--   <RowNumber>1</RowNumber>
	--   <ListCode>TestInventory</ListCode>
	--   <ListName>TestInventory</ListName>
	-- </ImportInventoryModel>
	--</ArrayOfImportInventoryModel>' , @status = @status OUT , @UserId = 2;
	--    SELECT @status;
	--    ROLLBACK TRANSACTION;
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @RoundOffValue int, @MessageDisplay nvarchar(100), @MessageDisplayForFloat nvarchar(100);
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		-- Retrive RoundOff Value from global setting 
		SELECT @RoundOffValue = FeatureValues
		FROM ZnodeGlobalSetting
		WHERE FeatureName = 'InventoryRoundOff';

		--@MessageDisplay will use to display validate message for input inventory value  

		DECLARE @sSql nvarchar(max);
		SET @sSql = ' Select @MessageDisplay_new = Convert(Numeric(28, '+CONVERT(nvarchar(200), @RoundOffValue)+'), 123.12345699 ) ';
		EXEC SP_EXecutesql @sSql, N'@MessageDisplay_new NVARCHAR(100) OUT', @MessageDisplay_new = @MessageDisplay OUT;
		SET @sSql = ' Select @MessageDisplay_new = Convert(Numeric(28, '+CONVERT(nvarchar(200), @RoundOffValue)+'), 0.999999 ) ';
		EXEC SP_EXecutesql @sSql, N'@MessageDisplay_new NVARCHAR(100) OUT', @MessageDisplay_new = @MessageDisplayForFloat OUT;
		DECLARE @InserInventoryForValidation TABLE
		( 
				RowNumber int, SKU varchar(max), Quantity varchar(max), ReOrderLevel varchar(max), WarehouseCode varchar(max), GUID nvarchar(400)
		);
		DECLARE @InsertInventory TABLE
		( 
				InsertInventoryId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, SKU varchar(300) INDEX Ix CLUSTERED (SKU), Quantity numeric(28, 6), ReOrderLevel numeric(28, 6), WarehouseCode varchar(200), GUID nvarchar(400) 
		);
		--DECLARE @InsertInventory TABLE
		--( 
		--		InsertInventoryId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, SKU varchar(300) , Quantity numeric(28, 6), ReOrderLevel numeric(28, 6), WarehouseCode varchar(200), GUID nvarchar(400) 
		--);

		DECLARE @ErrorLogForInsertInventory TABLE
		( 
				SKU varchar(max), Quantity varchar(max), ReOrderLevel varchar(max), WarehouseCode varchar(max), RowNumber bigint, ErrorDescription varchar(max), GUID nvarchar(400), ImportProcessLogId int, SourceColumnName varchar(max)
		);
		DECLARE @SKU TABLE
		( 
				SKU nvarchar(300)
		);
		INSERT INTO @SKU
			   SELECT b.AttributeValue
			   FROM ZnodePimAttributeValue AS a
					INNER JOIN
					ZnodePimAttributeValueLocale AS b
					ON a.PimAttributeId = dbo.Fn_GetProductSKUAttributeId() AND 
					   a.PimAttributeValueId = b.PimAttributeValueId;


		DECLARE @InventoryListId int;
		SET @SSQL = 'Select RowNumber,SKU,Quantity,ReOrderLevel,WarehouseCode ,GUID FROM '+@TableName;
		INSERT INTO @InserInventoryForValidation( RowNumber, SKU, Quantity, ReOrderLevel, WarehouseCode, GUID )
		EXEC sys.sp_sqlexec @SSQL;


		--Required Validation 
		--UomName should not be null 
		--Data for this Inventory list is already available  
		-- 
		-- 1)  Validation for SKU is pending Proper data not found and 
		--Discussion still open for Publish version where we create SKU and use thi SKU code for validation 
		--Select * from ZnodePimAttributeValue  where PimAttributeId =248
		--select * from View_ZnodePimAttributeValue Vzpa Inner join ZnodePimAttribute Zpa on Vzpa.PimAttributeId=Zpa.PimAttributeId where Zpa.AttributeCode = 'SKU'
		--Select * from ZnodePimAttribute where AttributeCode = 'SKU'
		--2)  Start Data Type Validation for XML Data  
		--SELECT * FROM ZnodeInventory
		--SELECT * FROM ZNodeInventoryList
		UPDATE @InserInventoryForValidation
		  SET ReOrderLevel = 0
		WHERE ReOrderLevel = '';

		DELETE FROM @InserInventoryForValidation
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId AND 
				  GUID = @NewGUID
		);

		INSERT INTO @InsertInventory( RowNumber, SKU, Quantity, ReOrderLevel, WarehouseCode )
			   SELECT RowNumber, SKU, Quantity, ReOrderLevel, WarehouseCode
			   FROM @InserInventoryForValidation;
					 
		-- start Functional Validation 
		-----------------------------------------------
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'SKU', SKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertInventory AS ii
			   WHERE ii.SKU NOT IN
			   (
				   SELECT SKU
				   FROM @SKU
			   );
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'WarehouseCode', WarehouseCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertInventory AS ii
			   WHERE NOT EXISTS
			   (
				   SELECT TOP 1 1
				   FROM ZnodeWarehouse AS zw
				   WHERE zw.WarehouseCode = ii.WarehouseCode
			   );

		-- End Function Validation 	
		-----------------------------------------------
		--- Delete Invalid Data after functional validatin  
		DELETE FROM @InsertInventory
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId AND 
				  GUID = @NewGUID
		);

		DECLARE @TBL_ReadyToInsertInventory TABLE
		( 
												  RowNumber int, SKU varchar(300), Quantity numeric(28, 6), ReOrderLevel numeric(28, 6), WarehouseId int
		);

		INSERT INTO @TBL_ReadyToInsertInventory( RowNumber, SKU, Quantity, ReOrderLevel, WarehouseId )
			   SELECT ii.RowNumber, ii.SKU, ii.Quantity, ISNULL(ii.ReOrderLevel, 0), zw.WarehouseId
			   FROM @InsertInventory AS ii
					INNER JOIN
					ZnodeWarehouse AS zw
					ON ii.WarehouseCode = zw.WarehouseCode AND 
					   ii.RowNumber IN
			   (
				   SELECT MAX(ii1.RowNumber)
				   FROM @InsertInventory AS ii1
				   WHERE ii1.WarehouseCode = ii.WarehouseCode AND 
						 ii1.SKU = ii.SKU
			   );
		--select 'update started'  
		UPDATE zi
		  SET Quantity = rtii.Quantity, ReOrderLevel = ISNULL(rtii.ReOrderLevel, 0), ModifiedBy = @UserId, ModifiedDate = @GetDate
		FROM ZNodeInventory zi
			 INNER JOIN
			 @TBL_ReadyToInsertInventory rtii
			 ON( zi.WarehouseId = rtii.WarehouseId AND 
				 zi.SKU = rtii.SKU
			   );
		--select 'update End'                
		INSERT INTO ZnodeInventory( WarehouseId, SKU, Quantity, ReOrderLevel, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
			   SELECT WarehouseId, SKU, Quantity, ISNULL(ReOrderLevel, 0), @UserId, @GetDate, @UserId, @GetDate
			   FROM @TBL_ReadyToInsertInventory AS rtii
			   WHERE NOT EXISTS
			   (
				   SELECT TOP 1 1
				   FROM ZnodeInventory AS zi
				   WHERE zi.WarehouseId = rtii.WarehouseId AND 
						 zi.SKU = rtii.SKU
			   ); 
		--select 'End'
		--      SET @Status = 1;
		UPDATE ZnodeImportProcessLog
		  SET Status = dbo.Fn_GetImportStatus( 2 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN A;
	END TRY
	BEGIN CATCH

		UPDATE ZnodeImportProcessLog
		  SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();
		ROLLBACK TRAN A;
	END CATCH;
END;

GO 

ALTER  PROCEDURE [dbo].[Znode_GetPublishAssociatedAddons](@PublishCatalogId NVARCHAR(MAX) = '',
                                                         @PimProductId     NVARCHAR(MAX) = '',
                                                         @VersionId        INT           = 0,
                                                         @UserId           INT)
AS 
   
/*
    Summary : If PimcatalogId is provided get all products with Addons and provide above mentioned data
              If PimProductId is provided get all Addons if associated with given product id and provide above mentioned data
    			Input: @PublishCatalogId or @PimProductId
    		    output should be in XML format
              sample xml5
              <AddonEntity>
              <ZnodeProductId></ZnodeProductId>
              <ZnodeCatalogId></ZnodeCatalogId>
              <AddonGroupName></AddonGroupName>
              <TempAsscociadedZnodeProductIds></TempAsscociadedZnodeProductIds>
              </AddonEntity>
    <AddonEntity>
      <ZnodeProductId>6</ZnodeProductId>
      <ZnodeCatalogId>2</ZnodeCatalogId>
      <AddonGroupName>RadioButton</AddonGroupName>
      <TempAsscociadedZnodeProductIds>53,54,55,56,57,82</TempAsscociadedZnodeProductIds>
      <ZnodeProductId>14</ZnodeProductId>
      <ZnodeCatalogId>2</ZnodeCatalogId>
      <AddonGroupName>RadioButton</AddonGroupName>
      <TempAsscociadedZnodeProductIds>6,7</TempAsscociadedZnodeProductIds>
      <ZnodeProductId>16</ZnodeProductId>
      <ZnodeCatalogId>2</ZnodeCatalogId>
      <AddonGroupName>RadioButton</AddonGroupName>
      <TempAsscociadedZnodeProductIds>7,14,54,6</TempAsscociadedZnodeProductIds>
    </AddonEntity>
    Unit Testing 
     SELECT * FROM ZnodePublishcatalog
	begin tran
     EXEC [dbo].[Znode_GetPublishAssociatedAddons] @PublishCatalogId = '3' ,@PimProductId=  '' ,@UserId=2
	rollback tran
     EXEC [dbo].[Znode_GetPublishAssociatedAddons] @PublishCatalogId = 3 ,@PimProductId=  '' ,@UserId=2
     EXEC [dbo].[Znode_GetPublishAssociatedAddons] @PublishCatalogId =null ,@PimProductId=  6   
   
	*/

     BEGIN
        -- BEGIN TRANSACTION GetPublishAssociatedAddons;
         BEGIN TRY
          SET NOCOUNT ON 
			 DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
             DECLARE @LocaleId INT, @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId()
			 , @Counter INT= 1
			 , @MaxRowId INT= 0;

            -- DECLARE @PimAddOnGroupId VARCHAR(MAX);

			 DECLARE @TBL_PublisshIds TABLE (PublishProductId INT , PimProductId INT , PublishCatalogId INT)

             DECLARE @TBL_LocaleId TABLE
             (RowId    INT IDENTITY(1, 1),
              LocaleId INT
             );


			 IF  @PublishCatalogId IS NULL  OR @PublishCatalogId = 0 
			 BEGIN 
			 		 
			   INSERT INTO @TBL_PublisshIds 
			   EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId
			   
			  -- SET @PimProductId = SUBSTRING((SELECT DISTINCT ','+CAST(PimProductId AS VARCHAr(50)) FROM @TBL_PublisshIds FOR XML PATH ('')),2,8000 )

			  -- SELECT 	@PimProductId	
			 END 
			
			
           
              DECLARE @TBL_PublishCatalogId TABLE(PublishCatalogId INT,PublishProductId INT,PimProductId  INT , VersionId INT , INDEX IND_909 (PimProductId), INDEX IND_99 (VersionId) );

			 INSERT INTO @TBL_PublishCatalogId 
			 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,CASE WHEN @VersionId = 0 OR @VersionId IS NULL THEN  MAX(PublishCatalogLogId) ELSE @VersionId END 
			 FROM ZnodePublishProduct ZPP 
			 INNEr JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
			 WHERE (EXISTS (SELECT TOP 1 1 FROM @TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId  AND  @PublishCatalogId = '' ) 
			 OR (ZPP.PublishCatalogId =  @PublishCatalogId ))
			 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId

             DECLARE @TBL_AddonGroupLocale TABLE
             (PimAddonGroupId INT,
              DisplayType     NVARCHAR(400),
              AddonGroupName  NVARCHAR(MAX),
			  LocaleId INT 
             );
           
             INSERT INTO @TBL_LocaleId(LocaleId)
                    SELECT LocaleId
                    FROM ZnodeLocale
                    WHERE IsActive = 1;

          
             SET @MaxRowId = ISNULL(
                                   (
                                       SELECT MAX(RowId)
                                       FROM @TBL_LocaleId
                                   ), 0);
    
             WHILE @Counter <= @MaxRowId
                 BEGIN
                     SET @LocaleId =
                     (
                         SELECT LocaleId
                         FROM @TBL_LocaleId
                         WHERE RowId = @Counter
                     );
                     INSERT INTO @TBL_AddonGroupLocale
                     (PimAddonGroupId,
                      DisplayType,
                      AddonGroupName					  
                     )
                     EXEC Znode_GetAddOnGroupLocale
                          '',
                          @LocaleId;

					UPDATE @TBL_AddonGroupLocale SET LocaleId = @LocaleId WHERE LocaleId IS NULL 

                    SET @Counter = @Counter + 1;
                 END;
				     
					  IF  @PublishCatalogId IS NULL  OR @PublishCatalogId = 0 
			           BEGIN 
			 		 
			         DELETE FROM ZnodePublishedXML WHERE IsAddOnXML =1 
					 AND EXISTS (SELECT TOP 1 1 FROM @TBL_PublishCatalogId TBLV WHERE ZnodePublishedXML.PublishedId = TBLV.PublishProductId   AND ZnodePublishedXML.PublishCatalogLogId = TBLV.VersionId )
			    
			  
					 END 
					 ELSE 
					 BEGIN 

					 SET @versionid  =(SELECT TOP 1 VersionId FROM @TBL_PublishCatalogId TBLV )

					 DELETE FROM ZnodePublishedXML WHERE IsAddOnXML =1 
					 AND PublishCatalogLogId  = @versionid
					 END 
				      
					 MERGE INTO ZnodePublishedXML TARGET 
					 USING (
					 SELECT   ZPPP.PublishProductId,ZPPP.PublishCatalogId,ZPPD.LocaleId,ZPPP.VersionId,'<AddonEntity><VersionId>'+CAST(ZPPP.VersionId AS VARCHAR(50))+'</VersionId><ZnodeProductId>'+CAST(ZPPP.[PublishProductId] AS VARCHAR(50))+'</ZnodeProductId><ZnodeCatalogId>'
				     +CAST(ZPPP.[PublishCatalogId] AS VARCHAR(50))+'</ZnodeCatalogId><AssociatedZnodeProductId>'+CAST(ZPP.PublishProductId  AS VARCHAR(50))
					 +'</AssociatedZnodeProductId><DisplayOrder>'+CAST( ISNULL(ZPOPD.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder><AssociatedProductDisplayOrder>'
					 +CAST(ISNULL(ZPAOP.DisplayOrder,'') AS VARCHAR(50))+'</AssociatedProductDisplayOrder><RequiredType>'+ISNULL(RequiredType,'')+'</RequiredType><DisplayType>'
					 + ISNULL(DisplayType,'')+'</DisplayType><GroupName>'+ISNULL(AddonGroupName,'')+'</GroupName><LocaleId>'+CAST(ZPPD.LocaleId AS VARCHAR(50))+'</LocaleId><IsDefault>'+CAST(ISNULL(IsDefault,0) AS VARCHAR(50))+'</IsDefault></AddonEntity>'  ReturnXML		   
				 
                      FROM [ZnodePimAddOnProductDetail] AS ZPOPD
                           INNER JOIN [ZnodePimAddOnProduct] AS ZPAOP ON ZPOPD.[PimAddOnProductId] = ZPAOP.[PimAddOnProductId]
						    INNER JOIN @TBL_PublishCatalogId ZPPP ON (ZPPP.PimProductId = ZPAOP.PimProductId )
                           INNER JOIN @TBL_PublishCatalogId ZPP ON(ZPP.PimProductId = ZPOPD.[PimChildProductId] AND ZPP.PublishCatalogId = ZPPP.PublishCatalogId )
						   INNER JOIN ZnodePublishProductDetail ZPPD ON (ZPPD.PublishProductId = ZPPP.PublishProductId)
						   INNER JOIN @TBL_AddonGroupLocale TBAG ON (TBAG.PimAddonGroupId   = ZPAOP.PimAddonGroupId AND TBAG.LocaleId = ZPPD.LocaleId )
						   
					) SOURCE 
					ON (
						 TARGET.PublishCatalogLogId = SOURCE.VersionId 
						 AND TARGET.PublishedId = SOURCE.PublishProductId
						 AND TARGET.IsAddonXML = 1 
						 AND TARGET.LocaleId = SOURCE.LocaleId 
					)
					WHEN MATCHED THEN 
					UPDATE 
					SET  PublishedXML = ReturnXML
					   , ModifiedBy = @userId 
					   ,ModifiedDate = @GetDate
					WHEN NOT MATCHED THEN 
					INSERT (PublishCatalogLogId
					,PublishedId
					,PublishedXML
					,IsAddonXML
					,LocaleId
					,CreatedBy
					,CreatedDate
					,ModifiedBy
					,ModifiedDate)
					
					VALUES (Source.VersionId , Source.publishProductid,Source.ReturnXML,1,@localeid,@userId,@getDate,@userId,@getDate);
					
					
					SELECT PublishedXML ReturnXML
					FROM @TBL_PublishCatalogId TBLPP 
					INNER JOIN ZnodePublishedXML ZPX ON (ZPX.PublishCatalogLogId = TBLPP.VersionId AND ZPX.PublishedId = TBLPP.publishProductid )
					WHERE ZPX.IsAddonXML = 1
             --SELECT ReturnXML
             --FROM @TBL_AddonXML;
		
           --  COMMIT TRANSACTION GetPublishAssociatedAddons;
         END TRY
         BEGIN CATCH
		     SELECT ERROR_MESSAGE(),ERROR_PROCEDURE()
             DECLARE @Status BIT;
             SET @Status = 0;
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishAssociatedAddons @PublishCatalogId = '+@PublishCatalogId+',@PimProductId='+@PimProductId+',@VersionId='+CAST(@VersionId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
           --  ROLLBACK TRANSACTION GetPublishAssociatedAddons;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_GetPublishAssociatedAddons',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;

	 GO 
	 


ALTER PROCEDURE [dbo].[Znode_GetPublishProductbulk]
(
@PublishCatalogId INT = 0 
,@VersionId       VARCHAR(50) = 0 
,@PimProductId    VARCHAR(2000) = 0 
,@UserId		  INT = 0 

)
AS
-- EXEC Znode_GetPublishProductbulk 0, 0 , '110' , 2  
BEGIN 
  
 SET NOCOUNT ON 

EXEC Znode_InsertUpdatePimAttributeXML 1 
EXEC Znode_InsertUpdateCustomeFieldXML 1
EXEC Znode_InsertUpdateAttributeDefaultValue 1 

DECLARE @PimProductAttributeXML TABLE(PimAttributeXMLId INT  PRIMARY KEY ,PimAttributeId INT,LocaleId INT  )
DECLARE @PimDefaultValueLocale  TABLE (PimAttributeDefaultXMLId INT  PRIMARY KEY ,PimAttributeDefaultValueId INT ,LocaleId INT ) 
DECLARE @ProductNamePimAttributeId INT = dbo.Fn_GetProductNameAttributeId(),@DefaultLocaleId INT= Dbo.Fn_GetDefaultLocaleId(),@LocaleId INT = 0 
		,@SkuPimAttributeId  INT =  dbo.Fn_GetProductSKUAttributeId() , @IsActivePimAttributeId INT =  dbo.Fn_GetProductIsActiveAttributeId()
DECLARE @GetDate DATETIME =dbo.Fn_GetDate()
DECLARE @TBL_LocaleId  TABLE (RowId INT IDENTITY(1,1) PRIMARY KEY  , LocaleId INT )
			INSERT INTO @TBL_LocaleId (LocaleId)
			SELECT  LocaleId
			FROM ZnodeLocale 
			WHERE IsActive = 1
DECLARE @Counter INT =1 ,@maxCountId INT = (SELECT max(RowId) FROM @TBL_LocaleId ) 

 DECLARE @TBL_PublishCatalogId TABLE(PublishCatalogId INT,PublishProductId INT,PimProductId  INT   , VersionId INT , INDEX IND_301 (PimProductId),INDEX IND_701 (VersionId))

			 INSERT INTO @TBL_PublishCatalogId 
			 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,CASE WHEN @versionId = 0 OR @versionId IS NULL THEN  
										MAX(PublishCatalogLogId) ELSE @versionId END 
			 FROM ZnodePublishProduct ZPP 
			 INNER JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
			 WHERE (EXISTS (SELECT TOP 1 1 FROM dbo.Split(@PimProductId,',') SP WHERE SP.Item = ZPP.PimProductId  AND  (@PublishCatalogId IS NULL OR @PublishCatalogId = 0 ))
			 OR  (ZPP.PublishCatalogId = @PublishCatalogId ))
			 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId
             DECLARE   @TBL_ZnodeTempPublish TABLE (PimProductId INT , AttributeCode VARCHAR(300) ,AttributeValue NVARCHAR(max) ) 			
			 DECLARE @TBL_AttributeVAlueLocale TABLE(PimProductId INT,PimAttributeId INT,ZnodePimAttributeValueLocaleId INT,LocaleId INT 
			 ,INDEX IND_309 (PimProductId,PimAttributeId) , INDEX IND_310 (LocaleId))

			 INSERT INTO @TBL_AttributeVAlueLocale
			 SELECT VIR.PimProductId,PimAttributeId,ZnodePimAttributeValueLocaleId,VIR.LocaleId
			 FROM View_LoadManageProductInternal VIR
			 INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = VIR.PimProductId)
			  
 
WHILE @Counter <= @maxCountId
BEGIN
 SET @LocaleId = (SELECT TOP 1 LocaleId FROM @TBL_LocaleId WHERE RowId = @Counter)


  INSERT INTO @PimProductAttributeXML 
  SELECT PimAttributeXMLId ,PimAttributeId,LocaleId
  FROM ZnodePimAttributeXML
  WHERE LocaleId = @LocaleId

  INSERT INTO @PimProductAttributeXML 
  SELECT PimAttributeXMLId ,PimAttributeId,LocaleId
  FROM ZnodePimAttributeXML ZPAX
  WHERE ZPAX.LocaleId = @DefaultLocaleId  
  AND NOT EXISTS (SELECT TOP 1 1 FROM @PimProductAttributeXML ZPAXI WHERE ZPAXI.PimAttributeId = ZPAX.PimAttributeId )

  INSERT INTO @PimDefaultValueLocale
  SELECT PimAttributeDefaultXMLId,PimAttributeDefaultValueId,LocaleId 
  FROM ZnodePimAttributeDefaultXML
  WHERE localeId = @LocaleId

  INSERT INTO @PimDefaultValueLocale 
   SELECT PimAttributeDefaultXMLId,PimAttributeDefaultValueId,LocaleId 
  FROM ZnodePimAttributeDefaultXML ZX
  WHERE localeId = @DefaultLocaleId
  AND NOT EXISTS (SELECT TOP 1 1 FROM @PimDefaultValueLocale TRTR WHERE TRTR.PimAttributeDefaultValueId = ZX.PimAttributeDefaultValueId)
  
  DECLARE @TBL_AttributeVAlue TABLE(PimProductId INT,PimAttributeId INT,ZnodePimAttributeValueLocaleId INT , INDEX IND_307(PimAttributeId,ZnodePimAttributeValueLocaleId),INDEX IND_308 (PimProductId,PimAttributeId) )
  DECLARE @TBL_CustomeFiled TABLE (PimCustomeFieldXMLId INT ,CustomCode VARCHAR(300),PimProductId INT ,LocaleId INT )

  INSERT INTO @TBL_CustomeFiled (PimCustomeFieldXMLId,PimProductId ,LocaleId,CustomCode)
  SELECT  PimCustomeFieldXMLId,RTR.PimProductId ,LocaleId,CustomCode
  FROM ZnodePimCustomeFieldXML RTR 
  INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = RTR.PimProductId)
  WHERE RTR.LocaleId = @LocaleId
 

  INSERT INTO @TBL_CustomeFiled (PimCustomeFieldXMLId,PimProductId ,LocaleId,CustomCode)
  SELECT  PimCustomeFieldXMLId,ITR.PimProductId ,LocaleId,CustomCode
  FROM ZnodePimCustomeFieldXML ITR
  INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ITR.PimProductId)
  WHERE ITR.LocaleId = @DefaultLocaleId
  AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_CustomeFiled TBL  WHERE ITR.CustomCode = TBL.CustomCode AND ITR.PimProductId = TBL.PimProductId)
  

    INSERT INTO @TBL_AttributeVAlue
    SELECT PimProductId,PimAttributeId,ZnodePimAttributeValueLocaleId
	FROM @TBL_AttributeVAlueLocale
    WHERE LocaleId = @LocaleId

    
	INSERT INTO @TBL_AttributeVAlue
	SELECT VI.PimProductId,PimAttributeId,ZnodePimAttributeValueLocaleId
	FROM @TBL_AttributeVAlueLocale VI 
    WHERE VI.LocaleId = @DefaultLocaleId 
	AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_AttributeVAlue  CTE WHERE CTE.PimProductId = VI.PimProductId AND CTE.PimAttributeId = VI.PimAttributeId )
 
 IF @PublishCatalogId IS NULL OR @PublishCatalogId = 0 
 BEGIN 
	
INSERT INTO @TBL_ZnodeTempPublish  
SELECT  a.PimProductId,a.AttributeCode , '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+ISNULL(a.AttributeValue,'')+'</AttributeValues> </AttributeEntity>  </Attributes>'  AttributeValue
FROM View_LoadManageProductInternal a 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = a.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
INNER JOIN @TBL_AttributeValue CTE ON (Cte.PimAttributeId = a.PimAttributeId AND Cte.ZnodePimAttributeValueLocaleId = a.ZnodePimAttributeValueLocaleId)
UNION ALL 
SELECT  a.PimProductId,c.AttributeCode , '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+''+'</AttributeValues> </AttributeEntity>  </Attributes>'  AttributeValue
                 
FROM ZnodePimAttributeValue  a 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = a.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
INNER JOIN ZnodePImAttribute ZPA  ON (ZPA.PimAttributeId = a.PimAttributeId)
INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = a.PimProductId)
WHERE ZPA.IsPersonalizable = 1 
AND NOT EXISTS ( SELECT TOP 1 1 FROM ZnodePimAttributeValueLocale q WHERE q.PimAttributeValueId = a.PimAttributeValueId) 
UNION ALL 
SELECT THB.PimProductId,'','<Attributes><AttributeEntity>'+CustomeFiledXML +'</AttributeEntity></Attributes>' 
FROM ZnodePimCustomeFieldXML THB 
INNER JOIN @TBL_CustomeFiled TRTE ON (TRTE.PimCustomeFieldXMLId = THB.PimCustomeFieldXMLId)
UNION ALL 
SELECT ZPAV.PimProductId,c.AttributeCode,'<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues></AttributeValues>'+'<SelectValues>'+
			   STUFF((
                    SELECT '  '+ DefaultValueXML  FROM ZnodePimAttributeDefaultXML AA 
				 INNER JOIN @PimDefaultValueLocale GH ON (GH.PimAttributeDefaultXMLId = AA.PimAttributeDefaultXMLId)
				 INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId )
				 WHERE (ZPADV.PimAttributeValueId = ZPAV.PimAttributeValueId)
    FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</SelectValues> </AttributeEntity></Attributes>' AttributeValue
 
FROM ZnodePimAttributeValue ZPAV  With (NoLock)
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue ZPADVL WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
UNION ALL 
SELECT ZPAV.PimProductId,c.AttributeCode,'<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+SUBSTRING((SELECT ',' +MediaPath FROM ZnodePimProductAttributeMedia ZPPG
     WHERE ZPPG.PimAttributeValueId = ZPAV.PimAttributeValueId 
	 FOR XML PATH ('')
 ),2,4000)+'</AttributeValues></AttributeEntity></Attributes>' AttributeValue
 	 
FROM ZnodePimAttributeValue ZPAV 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia ZPADVL WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
UNION ALL 
SELECT ZPLP.PimParentProductId ,c.AttributeCode, '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+ISNULL(SUBSTRING((SELECT ','+CAST(PublishProductId AS VARCHAR(50)) 
							 FROM @TBL_PublishCatalogId ZPPI 
							 INNER JOIN ZnodePimLinkProductDetail ZPLPI ON (ZPLPI.PimProductId = ZPPI.PimProductId)
							 WHERE ZPLPI.PimParentProductId = ZPLP.PimParentProductId
							 AND ZPLPI.PimAttributeId   = ZPLP.PimAttributeId
							 FOR XML PATH ('') ),2,4000),'')+'</AttributeValues></AttributeEntity></Attributes>'   AttributeValue 
							
FROM ZnodePimLinkProductDetail ZPLP 
INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPLP.PimParentProductId)
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPLP.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
GROUP BY ZPLP.PimParentProductId , ZPP.PublishProductId  ,ZPLP.PimAttributeId,c.AttributeCode,c.AttributeXML,ZPP.PublishCatalogId

 DELETE FROM ZnodePublishedXML WHERE  IsProductXML = 1  AND LocaleId = @localeId 
								AND  EXISTS ( SELECT TOP 1 1 FROM  @TBL_PublishCatalogId  TBL WHERE TBL.VersionId  = ZnodePublishedXML.PublishCatalogLogId AND TBL.PublishProductId = ZnodePublishedXML.PublishedId)


;WITH CTE AS
(
SELECT ROW_NUMBER() OVER (PARTITION BY PimProductId	,AttributeCode
ORDER BY PimProductId	,AttributeCode) AS RN
FROM @TBL_ZnodeTempPublish
)

DELETE FROM CTE WHERE RN<>1

  
 MERGE INTO ZnodePublishedXML TARGET 
 USING (
 SELECT zpp.PublishProductId,zpp.VersionId ,'<ProductEntity><VersionId>'+CAST(zpp.VersionId AS VARCHAR(50)) +'</VersionId><ZnodeProductId>'+CAST(zpp.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><ZnodeCategoryIds>'+CAST(ISNULL(ZPC.PublishCategoryId,'')  AS VARCHAR(50))+'</ZnodeCategoryIds><Name>'+CAST(ISNULL((SELECT ''+ZPPDFG.ProductName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</Name>'+'<SKU>'+CAST(ISNULL((SELECT ''+ZPPDFG.SKU FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKU>'+'<IsActive>'+CAST(ISNULL(ZPPDFG.IsActive ,'0') AS VARCHAR(50))+'</IsActive>' 
+'<ZnodeCatalogId>'+CAST(ZPP.PublishCatalogId  AS VARCHAR(50))+'</ZnodeCatalogId><IsParentProducts>'+CASE WHEN ZPCD.PublishCategoryId IS NULL THEN '0' ELSE '1' END  +'</IsParentProducts><CategoryName>'+CAST(ISNULL((SELECT ''+PublishCategoryName FOR XML PATH ('')),'') AS NVARCHAR(2000)) +'</CategoryName><CatalogName>'+CAST(ISNULL((SELECT ''+CatalogName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</CatalogName><LocaleId>'+CAST( @LocaleId AS VARCHAR(50))+'</LocaleId>'
+'<TempProfileIds>'+ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
					FROM ZnodeProfileCatalog ZPFC 
					INNER JOIN ZnodeProfileCatalogCategory ZPCCH  ON ( ZPCCH.ProfileCatalogId = ZPFC.ProfileCatalogId )
					WHERE ZPCCH.PimCatalogCategoryId = ZPCCF.PimCatalogCategoryId  FOR XML PATH('')),2,8000),'')+'</TempProfileIds><ProductIndex>'+CAST(ROW_NUMBER()Over(Partition BY zpp.PublishProductId Order BY ISNULL(ZPC.PublishCategoryId,'0') ) AS VARCHAr(100))+'</ProductIndex>'+
'<DisplayOrder>'+CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'+
STUFF(( SELECT '  '+ AttributeValue  FROM @TBL_ZnodeTempPublish TY WHERE TY.PimProductId = ZPP.PimProductId   
    FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</ProductEntity>' xmlvalue
FROM  @TBL_PublishCatalogId zpp
INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPP.PublishCatalogId)
INNER JOIN ZnodePublishProductDetail ZPPDFG ON (ZPPDFG.PublishProductId =  ZPP.PublishProductId)
LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId)
LEFT JOIN ZnodePublishCategory ZPC ON (ZPC.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId)
LEFT JOIN ZnodePimCatalogCategory ZPCCF ON (ZPCCF.PimCatalogId = ZPCV.PimCatalogId AND ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId )
LEFT JOIN ZnodePublishCategoryDetail ZPCD ON (ZPCD.PublishCategoryId = ZPCP.PublishCategoryId AND ZPCD.LocaleId = @LocaleId )
WHERE ZPPDFG.LocaleId = @LocaleId
) SOURCE 
ON (
     TARGET.PublishCatalogLogId = SOURCE.versionId 
	 AND TARGET.PublishedId = SOURCE.PublishProductId
	 AND TARGET.IsProductXML = 1 
	 AND TARGET.LocaleId = @localeId 
)
WHEN MATCHED THEN 
UPDATE 
SET  PublishedXML = xmlvalue
   , ModifiedBy = @userId 
   ,ModifiedDate = @GetDate
WHEN NOT MATCHED THEN 
INSERT (PublishCatalogLogId
,PublishedId
,PublishedXML
,IsProductXML
,LocaleId
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate)

VALUES (SOURCE.versionid , Source.publishProductid,Source.xmlvalue,1,@localeid,@userId,@getDate,@userId,@getDate);

DELETE FROM @TBL_ZnodeTempPublish
END 
ELSE 
BEGIN 
  	
IF  EXISTS (SELECT TOP 1 1 FROM SYS.Tables WHERE Name = 'ZnodeTempPublish' )
BEGIN 
    DROP TABLE ZnodeTempPublish
END 
CREATE TABLE ZnodeTempPublish (PimProductId INT , AttributeCode VARCHAR(300) ,AttributeValue NVARCHAR(max) )

INSERT INTO ZnodeTempPublish 
SELECT  a.PimProductId,a.AttributeCode , '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+ISNULL(a.AttributeValue,'')+'</AttributeValues> </AttributeEntity>  </Attributes>'  AttributeValue
FROM View_LoadManageProductInternal a 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = a.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
INNER JOIN @TBL_AttributeValue CTE ON (Cte.PimAttributeId = a.PimAttributeId AND Cte.ZnodePimAttributeValueLocaleId = a.ZnodePimAttributeValueLocaleId)
UNION ALL 
SELECT  a.PimProductId,c.AttributeCode , '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+''+'</AttributeValues> </AttributeEntity>  </Attributes>'  AttributeValue
                 
FROM ZnodePimAttributeValue  a 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = a.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
INNER JOIN ZnodePImAttribute ZPA  ON (ZPA.PimAttributeId = a.PimAttributeId)
INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = a.PimProductId)
WHERE ZPA.IsPersonalizable = 1 
AND NOT EXISTS ( SELECT TOP 1 1 FROM ZnodePimAttributeValueLocale q WHERE q.PimAttributeValueId = a.PimAttributeValueId) 
UNION ALL 
SELECT THB.PimProductId,'','<Attributes><AttributeEntity>'+CustomeFiledXML +'</AttributeEntity></Attributes>' 
FROM ZnodePimCustomeFieldXML THB 
INNER JOIN @TBL_CustomeFiled TRTE ON (TRTE.PimCustomeFieldXMLId = THB.PimCustomeFieldXMLId)
UNION ALL 
SELECT ZPAV.PimProductId,c.AttributeCode,'<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues></AttributeValues>'+'<SelectValues>'+
			   STUFF((
                    SELECT '  '+ DefaultValueXML  FROM ZnodePimAttributeDefaultXML AA 
				 INNER JOIN @PimDefaultValueLocale GH ON (GH.PimAttributeDefaultXMLId = AA.PimAttributeDefaultXMLId)
				 INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId )
				 WHERE (ZPADV.PimAttributeValueId = ZPAV.PimAttributeValueId)
    FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</SelectValues> </AttributeEntity></Attributes>' AttributeValue
 
FROM ZnodePimAttributeValue ZPAV  
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue ZPADVL WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
UNION ALL 
SELECT ZPAV.PimProductId,c.AttributeCode,'<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+SUBSTRING((SELECT ',' +MediaPath FROM ZnodePimProductAttributeMedia ZPPG
     WHERE ZPPG.PimAttributeValueId = ZPAV.PimAttributeValueId 
	 FOR XML PATH ('')
 ),2,4000)+'</AttributeValues></AttributeEntity></Attributes>' AttributeValue
 	 
FROM ZnodePimAttributeValue ZPAV 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia ZPADVL WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
UNION ALL 
SELECT ZPLP.PimParentProductId ,c.AttributeCode, '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+ISNULL(SUBSTRING((SELECT ','+CAST(PublishProductId AS VARCHAR(50)) 
							 FROM @TBL_PublishCatalogId ZPPI 
							 INNER JOIN ZnodePimLinkProductDetail ZPLPI ON (ZPLPI.PimProductId = ZPPI.PimProductId)
							 WHERE ZPLPI.PimParentProductId = ZPLP.PimParentProductId
							 AND ZPLPI.PimAttributeId   = ZPLP.PimAttributeId
							 FOR XML PATH ('') ),2,4000),'')+'</AttributeValues></AttributeEntity></Attributes>'   AttributeValue 
							
FROM ZnodePimLinkProductDetail ZPLP  
INNER JOIN @TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPLP.PimParentProductId)
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPLP.PimAttributeId )
INNER JOIN @PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId )
GROUP BY ZPLP.PimParentProductId , ZPP.PublishProductId  ,ZPLP.PimAttributeId,c.AttributeCode,c.AttributeXML,ZPP.PublishCatalogId


 CREATE NONCLUSTERED INDEX ID_ZnodeTempPublish_PimProductId   
    ON ZnodeTempPublish (PimProductId)

SET @versionId = (SELECT TOP 1 VersionId FROM @TBL_PublishCatalogId) 

 DELETE FROM ZnodePublishedXML WHERE  IsProductXML = 1  AND LocaleId = @localeId 
								AND   ZnodePublishedXML.PublishCatalogLogId =@versionId 


 MERGE INTO ZnodePublishedXML TARGET 
 USING (
SELECT zpp.PublishProductId,zpp.VersionId ,'<ProductEntity><VersionId>'+CAST(zpp.VersionId AS VARCHAR(50)) +'</VersionId><ZnodeProductId>'+CAST(zpp.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><ZnodeCategoryIds>'+CAST(ISNULL(ZPC.PublishCategoryId,'')  AS VARCHAR(50))+'</ZnodeCategoryIds><Name>'+CAST(ISNULL((SELECT ''+ZPPDFG.ProductName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</Name>'+'<SKU>'+CAST(ISNULL((SELECT ''+ZPPDFG.SKU FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKU>'+'<IsActive>'+CAST(ISNULL(ZPPDFG.IsActive ,'0') AS VARCHAR(50))+'</IsActive>' 
+'<ZnodeCatalogId>'+CAST(ZPP.PublishCatalogId  AS VARCHAR(50))+'</ZnodeCatalogId><IsParentProducts>'+CASE WHEN ZPCD.PublishCategoryId IS NULL THEN '0' ELSE '1' END  +'</IsParentProducts><CategoryName>'+CAST(ISNULL((SELECT ''+PublishCategoryName FOR XML PATH ('')),'') AS NVARCHAR(2000)) +'</CategoryName><CatalogName>'+CAST(ISNULL((SELECT ''+CatalogName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</CatalogName><LocaleId>'+CAST( @LocaleId AS VARCHAR(50))+'</LocaleId>'
+'<TempProfileIds>'+ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
					FROM ZnodeProfileCatalog ZPFC 
					INNER JOIN ZnodeProfileCatalogCategory ZPCCH  ON ( ZPCCH.ProfileCatalogId = ZPFC.ProfileCatalogId )
					WHERE ZPCCH.PimCatalogCategoryId = ZPCCF.PimCatalogCategoryId  FOR XML PATH('')),2,8000),'')+'</TempProfileIds><ProductIndex>'+CAST(ROW_NUMBER()Over(Partition BY zpp.PublishProductId Order BY ISNULL(ZPC.PublishCategoryId,'0') ) AS VARCHAr(100))+'</ProductIndex>'+
'<DisplayOrder>'+CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'+
STUFF(( SELECT '  '+ AttributeValue  FROM ZnodeTempPublish TY WHERE TY.PimProductId = ZPP.PimProductId   
    FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</ProductEntity>' xmlvalue
FROM  @TBL_PublishCatalogId zpp
INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPP.PublishCatalogId)
INNER JOIN ZnodePublishProductDetail ZPPDFG ON (ZPPDFG.PublishProductId =  ZPP.PublishProductId)
LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId)
LEFT JOIN ZnodePublishCategory ZPC ON (ZPC.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId)
LEFT JOIN ZnodePimCatalogCategory ZPCCF ON (ZPCCF.PimCatalogId = ZPCV.PimCatalogId AND ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId )
LEFT JOIN ZnodePublishCategoryDetail ZPCD ON (ZPCD.PublishCategoryId = ZPCP.PublishCategoryId AND ZPCD.LocaleId = @LocaleId )
WHERE ZPPDFG.LocaleId = @LocaleId
) SOURCE 
ON (
     TARGET.PublishCatalogLogId = SOURCE.versionId 
	 AND TARGET.PublishedId = SOURCE.PublishProductId
	 AND TARGET.IsProductXML = 1 
	 AND TARGET.LocaleId = @localeId 
)
WHEN MATCHED THEN 
UPDATE 
SET  PublishedXML = xmlvalue
   , ModifiedBy = @userId 
   ,ModifiedDate = @GetDate
WHEN NOT MATCHED THEN 
INSERT (PublishCatalogLogId
,PublishedId
,PublishedXML
,IsProductXML
,LocaleId
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate)

VALUES (SOURCE.versionid , Source.publishProductid,Source.xmlvalue,1,@localeid,@userId,@getDate,@userId,@getDate);

TRUNCATE TABLE ZnodeTempPublish
DROP TABLE ZnodeTempPublish

END 
DELETE FROM @PimProductAttributeXML
DELETE FROM @TBL_CustomeFiled
DELETE FROM @PimDefaultValueLocale
DELETE FROM @TBL_AttributeValue 

SET @Counter = @counter + 1 
END 

END