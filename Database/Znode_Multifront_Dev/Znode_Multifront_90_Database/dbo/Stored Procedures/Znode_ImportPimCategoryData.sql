CREATE PROCEDURE [dbo].[Znode_ImportPimCategoryData]
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
    		    Create group of product with their attribute code and values and inseerted one by one products. 	   
    
      SourceColumnName : CSV file column headers
      TargetColumnName : Attributecode from ZnodePimAttribute Table 

	 ***  Need to log error if transaction failed during insertion of records into table.

	EXEC Znode_ImportPimCategoryData 
    */

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             
             DECLARE @SQLQuery NVARCHAR(MAX);
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @AttributeTypeName NVARCHAR(10), @AttributeCode NVARCHAR(300), @AttributeId INT, @IsRequired BIT, @SourceColumnName NVARCHAR(600), @PimAttributeFamilyId INT, @NewProductId INT, @PimAttributeValueId INT, @status BIT= 0; 
             --Declare error Log Table 

			 IF OBJECT_ID('tempdb.dbo.#DuplicateCategory', 'U') IS NOT NULL 
		     DROP TABLE tempdb.dbo.#DuplicateCategory

			 IF OBJECT_ID('tempdb.dbo.#DefaultHideCategoryonMenu', 'U') IS NOT NULL   
			 DROP TABLE tempdb.dbo.#DefaultHideCategoryonMenu

             DECLARE @FamilyAttributeDetail TABLE
             (PimAttributeId       INT,
              AttributeTypeName    VARCHAR(300),
              AttributeCode        VARCHAR(300),
              SourceColumnName     NVARCHAR(600),
              IsRequired           BIT,
              PimAttributeFamilyId INT
             );
             IF @DefaultFamilyId = 0
                 BEGIN
                     INSERT INTO @FamilyAttributeDetail
                     (PimAttributeId,
                      AttributeTypeName,
                      AttributeCode,
                      SourceColumnName,
                      IsRequired,
                      PimAttributeFamilyId
                     )
                     --Call Process to insert data of defeult family with cource column name and target column name 
                     EXEC Znode_ImportGetTemplateDetails
                          @TemplateId = @TemplateId,
                          @IsValidationRules = 0,
                          @IsIncludeRespectiveFamily = 1,
                          @IsCategory = 1,
						  @DefaultFamilyId=@DefaultFamilyId;
                     UPDATE @FamilyAttributeDetail
                       SET
                           PimAttributeFamilyId = DBO.Fn_GetCategoryDefaultFamilyId();

					---- Deleted Attribute which are not provided in product import CSV and required attribute not mapped with AttributeGroup
					Delete FAD from @FamilyAttributeDetail FAD
					where AttributeCode not in (select Name from tempdb.sys.columns where object_id = object_id(@TableName))
					and not exists(select * from ZnodePimAttributeGroupMapper ZPAGM inner join ZnodePimFamilyGroupMapper ZPFGM on ZPAGM.PimAttributeGroupId = ZPFGM.PimAttributeGroupId 
					               inner join ZnodePimAttribute ZPA on ZPAGM.PimAttributeId = ZPA.PimAttributeId and FAD.AttributeCode = ZPA.AttributeCode)
                 END;
             ELSE
                 BEGIN
                     INSERT INTO @FamilyAttributeDetail
                     (PimAttributeId,
                      AttributeTypeName,
                      AttributeCode,
                      SourceColumnName,
                      IsRequired,
                      PimAttributeFamilyId
                     )
                     --Call Process to insert data of defeult family with cource column name and target column name 
                     EXEC Znode_ImportGetTemplateDetails
                          @TemplateId = @TemplateId,
                          @IsValidationRules = 0,
                          @IsIncludeRespectiveFamily = 1,
                          @DefaultFamilyId = @DefaultFamilyId,
                          @IsCategory = 1;

					---- Deleted Attribute which are not provided in product import CSV and required attribute not mapped with AttributeGroup
					Delete FAD from @FamilyAttributeDetail FAD
					where AttributeCode not in (select Name from tempdb.sys.columns where object_id = object_id(@TableName))
					and not exists(select * from ZnodePimAttributeGroupMapper ZPAGM inner join ZnodePimFamilyGroupMapper ZPFGM on ZPAGM.PimAttributeGroupId = ZPFGM.PimAttributeGroupId 
					               inner join ZnodePimAttribute ZPA on ZPAGM.PimAttributeId = ZPA.PimAttributeId and FAD.AttributeCode = ZPA.AttributeCode)
                 END;  
			 --Read all matched CategoryId with respect to their CategoryCode 

             --Read all attribute details with their datatype 

             IF NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#DefaultAttributeValue'
             )
                 BEGIN
                     CREATE TABLE #DefaultAttributeValue
                     (AttributeTypeName          VARCHAR(300),
                      PimAttributeDefaultValueId INT,
                      PimAttributeId             INT,
                      AttributeDefaultValueCode  VARCHAR(100)
                     );
                     INSERT INTO #DefaultAttributeValue
                     (AttributeTypeName,
                      PimAttributeDefaultValueId,
                      PimAttributeId,
                      AttributeDefaultValueCode
                     )
                     --Call Process to insert default data value 
                     EXEC Znode_ImportGetPimAttributeDefaultValue;
                 END;
             ELSE
                 BEGIN
                     DROP TABLE #DefaultAttributeValue;
                 END;  
         
             -- Split horizontal table into verticle table by column name and attribute Value with their 
             -- corresponding AttributeId, Default family , Default AttributeValue Id  
             DECLARE @PimCategoryDetail TABLE
             ([PimCategoryId]              [INT] NULL,
              [PimAttributeId]             [INT] NULL,
              [PimAttributeValueId]        [INT] NULL,
              [PimAttributeDefaultValueId] [INT] NULL,
              [PimAttributeFamilyId]       [INT] NULL,
              [LocaleId]                   [INT] NULL,
              [AttributeCode]              [VARCHAR](500) NULL,
              [AttributeValue]             [NVARCHAR](MAX) NULL,
              [RowNumber]                  INT 
             );
             -- Column wise split data from source table ( global temporary table ) and inserted into temporary table variable @PimCategoryDetail
             -- Add PimAttributeDefaultValue 

             DECLARE Cr_AttributeDetails CURSOR LOCAL FAST_FORWARD
             FOR SELECT PimAttributeId,
                        AttributeTypeName,
                        AttributeCode,
                        IsRequired,
                        SourceColumnName,
                        PimAttributeFamilyId
                 FROM @FamilyAttributeDetail
                 WHERE ISNULL(SourceColumnName, '') <> '';
             OPEN Cr_AttributeDetails;
             FETCH NEXT FROM Cr_AttributeDetails INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @PimAttributeFamilyId;
             WHILE @@FETCH_STATUS = 0
                 BEGIN
                     SET @NewProductId = 0;
                     SET @SQLQuery = 'SELECT '''+CONVERT(VARCHAR(100), @PimAttributeFamilyId)+''' PimAttributeFamilyId, 0 PimCategoryId,'''+CONVERT(VARCHAR(100), @AttributeId)+''' AttributeId ,
						   (SELECT TOP 1 PimAttributeDefaultValueId FROM #DefaultAttributeValue Where PimAttributeId =  '+CONVERT(VARCHAR(100), @AttributeId)+'AND  AttributeDefaultValueCode = TN.'+@SourceColumnName+' ) PimAttributeDefaultValueId,'+@SourceColumnName+','+CONVERT(VARCHAR(100), @LocaleId)+'LocaleId,RowNumber FROM '+@TableName+' TN';
                     INSERT INTO @PimCategoryDetail
                     ([PimAttributeFamilyId],
                      [PimCategoryId],
                      [PimAttributeId],
                      [PimAttributeDefaultValueId],
                      AttributeValue,
                      LocaleId,
                      RowNumber
                     )
                     EXEC sys.sp_sqlexec
                          @SQLQuery;
                     FETCH NEXT FROM Cr_AttributeDetails INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @PimAttributeFamilyId;
                 END;
             CLOSE Cr_AttributeDetails;
             DEALLOCATE Cr_AttributeDetails;
             UPDATE ppdti
               SET
                   ppdti.AttributeValue = CASE
                                              WHEN ppdti.AttributeValue = 'Yes/No'
                                              THEN 'False'
                                          END
             FROM @PimCategoryDetail ppdti
                  INNER JOIN #DefaultAttributeValue dav ON dav.PimAttributeDefaultValueId = ppdti.PimAttributeDefaultValueId
             WHERE ISNULL(ppdti.AttributeValue, '') = '';

			 SELECT PCD.AttributeValue
			 INTO #DuplicateCategory
			 FROM @PimCategoryDetail PCD
			 INNER JOIN ZnodePimAttribute PA ON (PCD.PimAttributeId = PA.PimAttributeId)
			 where PA.AttributeCode = 'CategoryCode' 
			 GROUP BY PCD.AttributeValue
			 Having Count(*) > 1


			INSERT INTO ZnodeImportLog
			(ErrorDescription,ColumnName,Data,RowNumber,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
			 SELECT '53' ,'CategoryCode',PCD.AttributeValue,PCD.RowNumber,@NewGUID,2,@GetDate,2,@GetDate,@ImportProcessLogId
			 FROM @PimCategoryDetail PCD
			 WHERE RowNumber IN (SELECT RowNumber frOM #DuplicateCategory DC 
								 INNER JOIN ZnodePimAttribute PA ON (PCD.PimAttributeId = PA.PimAttributeId)
								 WHERE PA.AttributeCode = 'CategoryCode' 
								 AND PCD.AttributeValue = DC.AttributeValue )

			SELECT PIMDTL.AttributeValue,PA.AttributeCode 
			INTO #DefaultHideCategoryonMenu
			FROM @PimCategoryDetail PIMDTL INNER JOIN ZnodePimAttribute PA ON (PIMDTL.PimAttributeId = PA.PimAttributeId)
			WHERE PA.AttributeCode='HideCategoryonMenu'

			INSERT INTO ZnodeImportLog  
			(ErrorDescription,ColumnName,Data,RowNumber,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId) 
			SELECT '68' ,'HideCategoryonMenu',PCD.AttributeValue,PCD.RowNumber,@NewGUID,2,@GetDate,2,@GetDate,@ImportProcessLogId  
			FROM @PimCategoryDetail PCD  
			WHERE RowNumber IN (SELECT RowNumber frOM #DefaultHideCategoryonMenu DC   
			INNER JOIN ZnodePimAttribute PA ON (PCD.PimAttributeId = PA.PimAttributeId)  
			WHERE PA.AttributeCode = 'HideCategoryonMenu'   
			AND PCD.AttributeValue = DC.AttributeValue ) AND PCD.AttributeValue NOT IN ('1','0','yes','no','true','false')

            UPDATE PIMDTL
		    SET PIMDTL.AttributeValue= CASE PIMDTL.AttributeValue WHEN 'NO' THEN 'FALSE' WHEN 'YES' THEN 'TRUE' ELSE PIMDTL.AttributeValue END
		    FROM @PimCategoryDetail PIMDTL 
		    INNER JOIN ZnodePimAttribute PA ON (PIMDTL.PimAttributeId = PA.PimAttributeId)
		    WHERE PA.AttributeCode='HideCategoryonMenu';
			
			 ----update mediaid for CategoryImage
			 select zm.FileName,max(MediaId) as MediaId, PCD.RowNumber
			 into #CategoryImage
			 from ZnodeMedia ZM
			 inner join @PimCategoryDetail PCD on ZM.FileName = PCD.AttributeValue
			 where exists(select * from ZnodePimAttribute c where PCD.PimAttributeId = c.PimAttributeId and c.AttributeCode = 'CategoryImage')
			 group by zm.FileName, PCD.RowNumber 
			 
			update a set a.AttributeValue = b.MediaId
			from @PimCategoryDetail a
			inner join #CategoryImage b on a.AttributeValue = b.FileName and a.RowNumber = b.RowNumber
				
			 ----update PimCategoryId present in znode on basis of CategoryCode
			 update d set  d.PimCategoryId = a.PimCategoryId
			 from ZnodePimCategoryAttributeValue a
			 inner join ZnodePimCategoryAttributeValueLocale b on a.PimCategoryAttributeValueId = b.PimCategoryAttributeValueId
			 inner join ZnodePimAttribute c on a.PimAttributeId = c.PimAttributeId
			 inner join @PimCategoryDetail d on d.AttributeValue = b.CategoryValue and c.PimAttributeId = d.PimAttributeId
			 where c.AttributeCode = 'CategoryCode' 

			 ----update PimCategoryId for other attributes if category is present
			 ;WITH CTE_UpdateCategoryId AS
			(
				select PimCategoryId, RowNumber from @PimCategoryDetail where isnull(PimCategoryId,0) <> 0
			)
			UPDATE PCD set PCD.PimCategoryId = UC.PimCategoryId
			FROM @PimCategoryDetail PCD
			INNER JOIN CTE_UpdateCategoryId UC on PCD.RowNumber = UC.RowNumber
			 ---------------------------

             -- Pass product records one by one 
             DECLARE @IncrementalId INT= 1;
             DECLARE @SequenceId INT=
             (
                 SELECT MAX(RowNumber)
                 FROM @PimCategoryDetail
             );
             DECLARE @PimCategoryDetailToInsert PIMCATEGORYDETAIL;  --User define table type to pass multiple records of product in single step

             WHILE @IncrementalId <= @SequenceId
                 BEGIN
                     INSERT INTO @PimCategoryDetailToInsert
                     ([PimCategoryId],
                      [PimAttributeId],
                      [PimAttributeValueId],
                      [PimAttributeDefaultValueId],
                      [PimAttributeFamilyId],
                      [LocaleId],
                      [AttributeCode],
                      [AttributeValue]
                     )
                            SELECT [PimCategoryId],
                                   [PimAttributeId],
                                   [PimAttributeValueId],
                                   [PimAttributeDefaultValueId],
                                   [PimAttributeFamilyId],
                                   [LocaleId],
                                   [AttributeCode],
                                   [AttributeValue]
                            FROM @PimCategoryDetail
                            WHERE [@PimCategoryDetail].RowNumber = @IncrementalId AND LTRIM(RTRIM([AttributeValue])) <> '';
                     --ORDER BY [@PimCategoryDetail].RowNumber;
                     ----Call process to finally insert data into 
                     ----------------------------------------------------------
                     --1. [dbo].[ZnodePimProduct]
                     --2. [dbo].[ZnodePimAttributeValue]
                     --3. [dbo].[ZnodePimAttributeValueLocale]

                     EXEC [Znode_ImportInsertUpdatePimCategory]
                          @InsertCategory = @PimCategoryDetailToInsert,
                          @UserID = @UserID,
                          @status = @status OUT,
						  @IsImport=1;--,@IsNotReturnOutput=1;
                     DELETE FROM @PimCategoryDetailToInsert;
                     SET @IncrementalId = @IncrementalId + 1;
                 END;

			
				-- Update Record count in log 
				DECLARE @FailedRecordCount BIGINT
				DECLARE @SuccessRecordCount BIGINT
				SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
				SET @SQLQuery = ' Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;
				EXEC	sp_executesql @SQLQuery, N'@SuccessRecordCount BIGINT out' , @SuccessRecordCount=@SuccessRecordCount OUTPUT
				UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount, TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
				WHERE ImportProcessLogId = @ImportProcessLogId;
				
			SET @GetDate = dbo.Fn_GetDate();
             --Updating the import process status
			UPDATE ZnodeImportProcessLog
			SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
								WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
								WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
							END, 
				ProcessCompletedDate = @GetDate
			WHERE ImportProcessLogId = @ImportProcessLogId;	   	  
            
			IF OBJECT_ID('tempdb.dbo.#DuplicateCategory', 'U') IS NOT NULL 
		     DROP TABLE tempdb.dbo.#DuplicateCategory

			IF OBJECT_ID('tempdb.dbo.#DefaultHideCategoryonMenu', 'U') IS NOT NULL   
			DROP TABLE tempdb.dbo.#DefaultHideCategoryonMenu

         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE(),ERROR_LINE(),ERROR_PROCEDURE();
 
			 DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportPimCategoryData @TableName = '+CAST(@TableName AS VARCHAR(max))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200)) +',@TemplateId='+CAST(@TemplateId AS VARCHAR(200)) +',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(10))+',@DefaultFamilyId='+CAST(@DefaultFamilyId AS VARCHAR(200));
            ---Import process updating fail due to database error
		UPDATE ZnodeImportProcessLog
		SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		---Loging error for Import process due to database error
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '93', '', '', @NewGUId,  @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

		--Updating total and fail record count
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
		TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) Where ImportProcessLogId = @ImportProcessLogId)
		WHERE ImportProcessLogId = @ImportProcessLogId;

		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ImportPimCategoryData',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
            
         END CATCH;
     END;