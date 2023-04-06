CREATE PROCEDURE [dbo].[Znode_ImportPartialValidatePimProductData]
(   @ImportHeadName     VARCHAR(200),
    @TableName          VARCHAR(200),
    @NewGUID            NVARCHAR(200),
    @TemplateId         INT,
    @UserId             INT,
    @LocaleId           INT           = 1,
    @IsCategory         INT           = 0,
    @DefaultFamilyId    INT           = 0,
    @ImportProcessLogId INT,
    @PriceListId        INT,
	@CountryCode VARCHAR(100) = '',
	@PimCatalogId         INT    = 0 ,
	@PortalId int = 0,
	@IsAccountAddress bit = 0,
	@PromotionTypeId	INT=0 
)
AS
     SET NOCOUNT ON;

/*
    Summary :   Import PimProduct ( for partial attribute import ) 
    Process :   Admin site will upload excel / csv file in database and create global temporary table
				Procedure Znode_ImportValidatePimProductData will validate data with attribute validation rule
				If datatype validation issue found in input daata will logged into table "ZnodeImportLog"
				If Data is correct and record count in table ZnodeImportLog will be 0 then process for import data into Base tables
				To import data call procedure "Znode_ImportPimProductData"
    		  
				SourceColumnName: CSV file column headers
				TargetColumnName: Attributecode from ZnodePimAttribute Table (Consider those Attributecodes configured with default family only)
*/

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             --BEGIN TRAN TRN_ImportValidProductData;
             DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
             DECLARE @SQLQuery NVARCHAR(MAX), @AttributeTypeName NVARCHAR(10), @AttributeCode NVARCHAR(300), @AttributeId INT, @IsRequired BIT, @SourceColumnName NVARCHAR(600), @ControlName VARCHAR(300), @ValidationName VARCHAR(100), @SubValidationName VARCHAR(300), @ValidationValue VARCHAR(300), @RegExp VARCHAR(300), @CreateDateString NVARCHAR(300), @DefaultLocaleId INT, @ImportHeadId INT, @CheckedSourceColumn NVARCHAR(600)= '', @Status BIT= 0,
			    @CsvColumnString nvarchar(max), @FailedRecordCount BIGINT,
				@SuccessRecordCount BIGINT

			DECLARE @TableNameNew NVARCHAR(100);
			SET @TableNameNew = REPLACE(@TableName,']','')+CAST(NEWID() AS NVARCHAR(100))+']'

			SET @SQLQuery='	SELECT * INTO '+@TableNameNew+' FROM '+@TableName+'	';

			--PRINT (@SQLQuery);
			EXEC (@SQLQuery);
			SET @TableName=@TableNameNew;

              --To get the total record count for update purpose in catch block
             SET @SQLQuery = 'SELECT '+CAST(@ImportProcessLogId AS VARCHAR(10))+',COUNT(*) FROM '+@TableName
			 INSERT INTO Znode_ImportCsvRowCount
			 EXEC (@SQLQuery) 

			UPDATE ZnodeImportProcessLog
			SET Status = dbo.Fn_GetImportStatus(0)
			WHERE ImportProcessLogId = @ImportProcessLogId AND Status IS NULL;

             DECLARE @FamilyAttributeDetail TABLE
             (PimAttributeId       INT,
              AttributeTypeName    VARCHAR(300),
              AttributeCode        VARCHAR(300),
              SourceColumnName     NVARCHAR(600),
              IsRequired           BIT,
              PimAttributeFamilyId INT
             );
             DECLARE @AttributeDetail TABLE
             (PimAttributeId    INT,
              AttributeTypeName VARCHAR(300),
              AttributeCode     VARCHAR(300),
              SourceColumnName  NVARCHAR(600),
              IsRequired        BIT,
              ControlName       VARCHAR(300),
              ValidationName    VARCHAR(100),
              SubValidationName VARCHAR(300),
              ValidationValue   VARCHAR(300),
              RegExp            VARCHAR(300)
             );

             DECLARE @GlobalTempTableColumns TABLE(ColumnName NVARCHAR);
             IF NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#InvalidDefaultData'
             )
                 CREATE TABLE #InvalidDefaultData
                 (RowNumber  INT,
                  Value      NVARCHAR(MAX),
                  ColumnName NVARCHAR(600)
                 );
             ELSE
             DROP TABLE #InvalidDefaultData;
             IF NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM INFORMATION_SCHEMA.TABLES
                 WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#GlobalTempTableColumns'
             )
                 BEGIN

                     SET @SQLQuery = 'SELECT Column_Name, '''+@ImportHeadName+''' AS ImportHeadName  from tempdb.INFORMATION_SCHEMA.COLUMNS	where table_name = object_name(object_id('''+@TableName+'''),
					(select database_id from sys.databases where name = ''tempdb''))';
                     CREATE TABLE #GlobalTempTableColumns
                     (ColumnName   NVARCHAR(MAX),
                      TypeOfImport NVARCHAR(100)
                     );
                     INSERT INTO #GlobalTempTableColumns
                     (ColumnName,
                      TypeOfImport
                     )
                     EXEC sys.sp_sqlexec
                          @SQLQuery;
                 END;

             IF EXISTS
             (
                 SELECT TOP 1 1
                 FROM #GlobalTempTableColumns
                 WHERE ColumnName IN('PimCategoryId', 'PimProductId', 'RowNumber')
             )
                 BEGIN
                     INSERT INTO ZnodeImportLog
                     (ErrorDescription,
                      ColumnName,
                      Data,
                      GUID,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate,
                      ImportProcessLogId
                     )
                     VALUES
                     (43,
                      '',
                      '',
                      @newGUID,
                      @UserId,
                      @GetDate,
                      @UserId,
                      @GetDate,
                      @ImportProcessLogId
                     );
                 END;
             SET @DefaultLocaleId = dbo.Fn_GetDefaultLocaleId();
             --Remove old error log 

             IF NOT EXISTS
             (
                 SELECT TOP 1 1  FROM ZnodeImportLog
                 WHERE Guid = @NewGUID
                       AND ErrorDescription IN(43, 42)
                 AND ImportProcessLogId = @ImportProcessLogId
             )
                 BEGIN
                     IF @ImportHeadName = 'ProductUpdate'
                      BEGIN
						  IF @@VERSION LIKE '%Azure%' OR @@VERSION LIKE '%Express Edition%'
							  SET @SQLQuery = 'Alter table '+@TableName+' Add  RowNumber BIGINT Identity(1,1),PimProductId int null ';
						  ELSE 
							 SET @SQLQuery = 'Alter table '+@TableName+' Add  RowNumber BIGINT Identity(1,1),PimProductId int null Primary KEY CLUSTERED(RowNumber)';
						 
						  EXEC sys.sp_sqlexec @SQLQuery;
			         END;
                     ELSE
                     IF @ImportHeadName = 'Category'
                         BEGIN
							  IF @@VERSION LIKE '%Azure%' OR @@VERSION LIKE '%Express Edition%'
								SET @SQLQuery = 'Alter table '+@TableName+' Add  RowNumber BIGINT Identity(1,1),PimCategoryId int null ';
							  ElSE
								SET @SQLQuery = 'Alter table '+@TableName+' Add  RowNumber BIGINT Identity(1,1),PimCategoryId int null Primary KEY CLUSTERED(RowNumber) ';
						  
							  EXEC sys.sp_sqlexec @SQLQuery;
                         END;
                     ELSE
                         BEGIN
							IF @@VERSION LIKE '%Azure%' OR @@VERSION LIKE '%Express Edition%'
								SET @SQLQuery = 'Alter table '+@TableName+' Add  RowNumber BIGINT Identity(1,1) ';
							Else 
								SET @SQLQuery = 'Alter table '+@TableName+' Add  RowNumber BIGINT Identity(1,1) Primary KEY CLUSTERED(RowNumber)';
							
							EXEC sys.sp_sqlexec @SQLQuery;
                         END;;
                 END



			--Retrive PimProductId on the basis of SKU for update product 
			SET @SQLQuery = 'UPDATE tlb SET tlb.PimProductId = ZPAV.PimProductId 
							FROM ZnodePimAttributeValue AS ZPAV INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL ON 
							(ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId) 
							INNER JOIN [dbo].[ZnodePimAttribute] ZPA on ZPAV.PimAttributeId = ZPA.PimAttributeId AND ZPA.AttributeCode= ''SKU'' 
							INNER JOIN '+@TableName+' tlb ON ZPAVL.AttributeValue = ltrim(rtrim(tlb.SKU)) ';
			EXEC sys.sp_sqlexec	@SQLQuery	 	
	
			SET @SQLQuery = 'Select 98 ,''SKU'', SKU, '''+ @newGUID + ''',' + Convert(nvarchar(100),@UserId) + ',''' +  Convert(nvarchar(100),@GetDate) + ''',' + Convert(nvarchar(100),@UserId) + ',''' + Convert(nvarchar(100),@GetDate) + ''',' +  Convert(nvarchar(100),@ImportProcessLogId)  + ',RowNumber   from  '+ @TableName + ' where PimProductId Is null ';
			INSERT INTO ZnodeImportLog
                     (ErrorDescription,ColumnName,Data,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId,RowNumber )
        	EXEC sys.sp_sqlexec	@SQLQuery	 	

			SET @SQLQuery = 'Delete from  '+@TableName+ ' where PimProductId Is null ';
			EXEC sys.sp_sqlexec	@SQLQuery	 	
			
			DECLARE @RecordCount Bigint 
			SET @SQLQuery = ' Select @RecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;
			EXEC sp_executesql @SQLQuery, N'@RecordCount BIGINT out' , @RecordCount=@RecordCount out

             SET @CreateDateString = CONVERT(VARCHAR(100), @UserId)+','''+CONVERT(VARCHAR(100), @GetDate)+''','+CONVERT(VARCHAR(100), @UserId)+','''+CONVERT(VARCHAR(100), @GetDate)+''', '+CONVERT(VARCHAR(100), @ImportProcessLogId);

             SELECT TOP 1 @ImportHeadId = ImportHeadId FROM ZnodeImportTemplate WHERE ImportTemplateId = @TemplateId;
             IF @ImportHeadName IN('ProductUpdate') AND @RecordCount > 0  
                 BEGIN 
					SET @IsCategory = 0 
				    --Get all default attribute values in attribute 
                    INSERT INTO @FamilyAttributeDetail
                    (PimAttributeId,AttributeTypeName,AttributeCode,SourceColumnName,IsRequired,PimAttributeFamilyId)
                    --Call Process to insert data of defeult family with source column name and target column name 
					SELECT distinct zpa.PimAttributeId, zat.AttributeTypeName, zpa.AttributeCode, zitm.SourceColumnName, zpa.IsRequired ,0
					FROM dbo.ZnodePimAttribute AS zpa INNER JOIN dbo.ZnodeAttributeType AS zat ON zat.AttributeTypeId = zpa.AttributeTypeId 
					LEFT OUTER JOIN dbo.ZnodeImportTemplateMapping AS zitm
					ON zpa.AttributeCode = zitm.SourceColumnName AND zitm.ImportTemplateId = @TemplateId
					WHERE zpa.IsCategory = 0 
	             END;
            -- Check attributes are manditory and not provided with source table
		   	if @TABLENAME	like '%tempdb..%'
				SET @SQLQuery = 'SELECT 42 AS ErrorDescription , SourceColumnName , '''' , '''+@NewGUID+''','+@CreateDateString+' from ZnodeImportTemplateMapping where ImportTemplateId = '+CONVERT(VARCHAR(100), @TemplateId)+' and ltrim(rtrim(SourceColumnName)) <> '''' AND ltrim(rtrim(SourceColumnName)) not in ( select isnull(Name ,'''') from tempdb.sys.columns where object_id = object_id('''+@TABLENAME+'''));';
			else 
				SET @SQLQuery = 'SELECT 42 AS ErrorDescription , SourceColumnName , '''' , '''+@NewGUID+''','+@CreateDateString+' from ZnodeImportTemplateMapping where ImportTemplateId = '+CONVERT(VARCHAR(100), @TemplateId)+' and ltrim(rtrim(SourceColumnName)) <> '''' AND ltrim(rtrim(SourceColumnName)) not in ( select isnull(Name ,'''') from sys.columns where object_id = object_id('''+@TABLENAME+'''));';
		 
		 
     		INSERT INTO ZnodeImportLog(ErrorDescription, ColumnName, Data, GUID,CreatedBy, CreatedDate,  ModifiedBy,ModifiedDate,ImportProcessLogId )
            EXEC sys.sp_sqlexec  @SQLQuery;
            IF NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeImportLog
                 WHERE Guid = @NewGUID
                       AND ErrorDescription IN(43, 42)
                 AND ImportProcessLogId = @ImportProcessLogId
             )  AND @RecordCount > 0  
                 BEGIN
                     --Get all default attribute values in attribute 
                     IF @ImportHeadName IN('ProductUpdate', 'Category')
                         BEGIN
                             -- Check attributes are manditory and not provided with source table

                             -- Read all attribute details with their datatype
                             INSERT INTO @AttributeDetail
                             (PimAttributeId,
                              AttributeTypeName,
                              AttributeCode,
                              SourceColumnName,
                              IsRequired,
                              ControlName,
                              ValidationName,
                              SubValidationName,
                              ValidationValue,
                              RegExp
                             )
                             EXEC Znode_ImportGetTemplateDetails
                                  @TemplateId=@TemplateId,
								  @DefaultFamilyId=@DefaultFamilyId;

							 ---- Deleted Attribute which are not provided in product import CSV and required attribute not mapped with AttributeGroup
							 Delete FAD from @AttributeDetail FAD
							 where AttributeCode not in (select Name from tempdb.sys.columns where object_id = object_id(@TableName))
							 and not exists(select * from ZnodePimAttributeGroupMapper ZPAGM inner join ZnodePimFamilyGroupMapper ZPFGM on ZPAGM.PimAttributeGroupId = ZPFGM.PimAttributeGroupId 
										   inner join ZnodePimAttribute ZPA on ZPAGM.PimAttributeId = ZPA.PimAttributeId and FAD.AttributeCode = ZPA.AttributeCode)

                             DELETE FROM @AttributeDetail
                             WHERE AttributeTypeName = 'Image'
                                   AND ValidationName <> 'IsAllowMultiUpload';
                             IF NOT EXISTS
                             (
                                 SELECT TOP 1 1
                                 FROM INFORMATION_SCHEMA.TABLES
                                 WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#DefaultAttributeCode'
                             )
                                 BEGIN
                                     CREATE TABLE #DefaultAttributeCode
                                     (AttributeTypeName          VARCHAR(300),
                                      PimAttributeDefaultValueId INT,
                                      PimAttributeId             INT,
                                      AttributeDefaultValueCode  VARCHAR(100)
                                     );
                                     INSERT INTO #DefaultAttributeCode
                                     (AttributeTypeName,
                                      PimAttributeDefaultValueId,
                                      PimAttributeId,
                                      AttributeDefaultValueCode
                                     )
                                     --Call Process to insert default data value 
                                     EXEC Znode_ImportGetPimAttributeDefaultValue;
                                     DELETE FROM #DefaultAttributeCode
                                     WHERE AttributeTypeName = 'Yes/No';
                                 END;
                             ELSE
                                 BEGIN
                                     DROP TABLE #DefaultAttributeCode;
                                 END;
                         END;

                     --	Check attributes are not mapped with (Default / Other) family of Pim Product

                     DECLARE Cr_Attribute CURSOR LOCAL FAST_FORWARD
                     FOR SELECT PimAttributeId,
                                AttributeTypeName,
                                AttributeCode,
                                IsRequired,
                                SourceColumnName,
                                ControlName,
                                ValidationName,
                                SubValidationName,
                                ValidationValue,
                                RegExp
                         FROM @AttributeDetail
                         WHERE ISNULL(SourceColumnName, '') <> '';
                     OPEN Cr_Attribute;
                     FETCH NEXT FROM Cr_Attribute INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @ControlName, @ValidationName, @SubValidationName, @ValidationValue, @RegExp;
                     WHILE @@FETCH_STATUS = 0
                         BEGIN
				             IF @AttributeTypeName = 'Number'
                                 BEGIN
							      EXEC Znode_ImportValidateNumber
                                          @TableName = @TableName,
                                          @SourceColumnName = @SourceColumnName,
                                          @CreateDateString = @CreateDateString,
                                          @ValidationName = @ValidationName,
                                          @ControlName = @ControlName,
                                          @ValidationValue = @ValidationValue,
                                          @NewGUID = @NewGUID,
                                          @ImportHeadId = @ImportHeadId,
                                          @ImportProcessLogId = @ImportProcessLogId;
                                 END;
							 -- Check invalid date
							
                             IF @AttributeTypeName = 'Date'
                                 BEGIN
                                     EXEC Znode_ImportValidateDate
                                          @TableName = @TableName,
                                          @SourceColumnName = @SourceColumnName,
                                          @CreateDateString = @CreateDateString,
                                          @ValidationName = @ValidationName,
                                          @ControlName = @ControlName,
                                          @ValidationValue = @ValidationValue,
                                          @NewGUID = @NewGUID,
                                          @ImportHeadId = @ImportHeadId,
                                          @ImportProcessLogId = @ImportProcessLogId;
                                 END;
							 -- Check Manditory Data
		 					 IF @IsRequired = 1 AND @CheckedSourceColumn <> @SourceColumnName
								BEGIN
									SET @CheckedSourceColumn = @SourceColumnName;
									EXEC Znode_ImportValidateMandatoryData
									@TableName = @TableName,
									@SourceColumnName = @SourceColumnName,
									@CreateDateString = @CreateDateString,
									@ValidationName = @ValidationName,
									@ControlName = @ControlName,
									@ValidationValue = @ValidationValue,
									@NewGUID = @NewGUID,
									@ImportHeadId = @ImportHeadId;
								END;
							 --END 
							
                             IF @AttributeTypeName = 'Text'
                                 BEGIN
									--For link product
									DECLARE @IsIgnoreProcess BIT = CASE WHEN EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute WHERE AttributeCode = (SELECT TOP 1 TargetColumnName
								    FROM ZnodeImportTemplateMapping a 
									INNER JOIN ZnodeImportTemplate b ON (b.ImportTemplateId = a.ImportTemplateId )
									WHERE TemplateName = 'ProductUpdate'
									AND a.TargetColumnName <> 'SKU'
								   )
								    AND AttributeTypeId = (SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE  AttributeTypeName = 'link' )
								   ) THEN 1 ELSE 0 END 
								 
						              EXEC Znode_ImportValidateManditoryText
                                          @TableName = @TableName,
                                          @SourceColumnName = @SourceColumnName,
                                          @CreateDateString = @CreateDateString,
                                          @ValidationName = @ValidationName,
                                          @ControlName = @ControlName,
                                          @ValidationValue = @ValidationValue,
                                          @NewGUID = @NewGUID,
                                          @LocaleId = @LocaleId,
                                          @DefaultLocaleId = @DefaultLocaleId,
                                          @AttributeId = @AttributeId,
                                          @ImportProcessLogId = @ImportProcessLogId,
                                          @ImportHeadId = @ImportHeadId,
										  @IsIgnoreProcess = @IsIgnoreProcess;
                                 END;
                             IF @AttributeTypeName = 'Image'
                                 BEGIN
                                     EXEC Znode_ImportValidateImageData
                                          @TableName = @TableName,
                                          @SourceColumnName = @SourceColumnName,
                                          @CreateDateString = @CreateDateString,
                                          @ValidationName = @ValidationName,
                                          @ControlName = @ControlName,
                                          @ValidationValue = @ValidationValue,
                                          @NewGUID = @NewGUID,
                                          @LocaleId = @LocaleId,
                                          @DefaultLocaleId = @DefaultLocaleId,
                                          @AttributeId = @AttributeId,
                                          @ImportProcessLogId = @ImportProcessLogId,
                                          @ImportHeadId = @ImportHeadId;
                                 END;

								 --For link product
								 IF @AttributeTypeName = 'Link'
                                 BEGIN
										--To get the product SKU in temp table
										SELECT ZPAV.PimProductId, ZPAVL.AttributeValue as SKU
										INTO #ProductSKU
										FROM ZnodePimAttributeValue ZPAV
										INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId 
										WHERE EXISTS(SELECT * FROM ZnodePimAttribute ZPA WHERE ZPAV.PimAttributeId = zpa.PimAttributeId AND ZPA.AttributeCode = 'SKU')

                                     	SET @SQLQuery = 'SELECT ''98'' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], '+@SourceColumnName+' AS  AttributeValue,RowNumber ,GUID,  '+@CreateDateString+' 
											FROM '+@TableName+' a 
											CROSS APPLY DBO.SPLIT('+@SourceColumnName+','','')S WHERE RowNumber in (SELECT RowNumber FROM '+@TableName+' WHERE  NOT EXISTS  (Select TOP 1 1  FROM #ProductSKU WHERE SKU = S.Item ) 
											)
											';
                     
											INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, RowNumber, GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
											EXEC sys.sp_sqlexec @SQLQuery;
				
                                 END;

                             --Check Default data value is valid 
                             IF @ImportHeadName IN('ProductUpdate', 'Category')
                                 BEGIN
                                     IF @AttributeId IN
                                     (
                                         SELECT PimAttributeId
                                         FROM #DefaultAttributeCode
                                     )
                                         BEGIN
							
                                                   ---Verify Image file is exists in media table or not 
                                             SET @SQLQuery = ' INSERT INTO #InvalidDefaultData (RowNumber, Value, ColumnName) 
                                             SELECT ROWNUMBER , (Select TOP 1 Item from dbo.split(' + @SourceColumnName + ','','')  SP WHERE NOT EXISTS 
                                             (Select ToP 1 1 FROM #DefaultAttributeCode DAC WHERE 
                                              DAC.AttributeTypeName <> ''Yes/No'' AND DAC.AttributeDefaultValueCode IS NOT NULL AND DAC.PimAttributeId = 
                                             ' + CONVERT(VARCHAR(100), @AttributeId) + ' AND ltrim(rtrim(SP.Item) ) = DAC.AttributeDefaultValueCode
                                             )), ''' + @SourceColumnName + ''' as [ColumnName]  FROM ' + @TableName
                                             + ' Where ISnull(' + @SourceColumnName +  ','''') <> '''''

						
                                             EXEC sys.sp_sqlexec @SQLQuery;
                                             -- Check Invalid Image 
                                             
											 SET @SQLQuery = 'SELECT ''9 '' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], 
                                             Value AS  AttributeValue,RowNumber ,'''+@NewGUID+''',  '+@CreateDateString+' FROM #InvalidDefaultData Where Value IS NOT NULL'
                                             INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, RowNumber, GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
                                             EXEC sys.sp_sqlexec @SQLQuery;

											 Delete from #InvalidDefaultData

       
                                         END;
                                 END;
							
                             FETCH NEXT FROM Cr_Attribute INTO @AttributeId, @AttributeTypeName, @AttributeCode, @IsRequired, @SourceColumnName, @ControlName, @ValidationName, @SubValidationName, @ValidationValue, @RegExp;
                         END;
                     CLOSE Cr_Attribute;
                     DEALLOCATE Cr_Attribute;
                     --SELECT top 1 1 FROM @FamilyAttributeDetail where  iSNULL(SourceColumnName,'') = ''  and IsRequired = 1
                 END;
            --COMMIT TRAN TRN_ImportValidProductData;
			 

		IF @ImportHeadName IN('ProductUpdate')
		 BEGIN
		 Declare @SQLQueryNew NVARCHAR(4000)
		 Declare @SourceColumnNameProduct nvarchar(4000)   	 
		 SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'SKU'
		 AND ImportTemplateId = @TemplateId

            SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  SKU - '' + ' + '  ' +@SourceColumnNameProduct+ '+' + ''' ]'' 
		    FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
			WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
            PRINT @SQLQueryNew

			EXEC sys.sp_sqlexec  @SQLQueryNew;
			
		END 

					 	 		 
  			 SET @SQLQuery = 'Delete FROM  '+@TableName+' Where Rownumber in (Select Rownumber from ZnodeImportLog  WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND Rownumber is not null)';
             EXEC sys.sp_sqlexec  @SQLQuery;

             IF @ImportHeadName IN('ProductUpdate')
                 BEGIN
                     IF NOT EXISTS
                
					 (
						 SELECT TOP 1 1
						 FROM ZnodeImportLog
						 WHERE Guid = @NewGUID
							   AND ErrorDescription IN (43, 42)
						 AND ImportProcessLogId = @ImportProcessLogId
					 ) AND @RecordCount > 0 
                         BEGIN
                             IF @IsCategory = 0
                                 BEGIN
                                     EXEC Znode_ImportPartialPimProductData
                                          @TableName = @TableName,
                                          @NewGUID = @NewGUID,
                                          @TemplateId = @TemplateId,
                                          @ImportProcessLogId = @ImportProcessLogId,
                                          @UserId = @UserId,
                                          @LocaleId = @LocaleId,
                                          @DefaultFamilyId = @DefaultFamilyId;
	
                                 END;
                            
                         END;

					ELSE 
					BEGIN
					-- Update Record count in log 					
					SET @SQLQuery = ' Select @FailedRecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;
					EXEC	sp_executesql @SQLQuery , N'@FailedRecordCount BIGINT out' , @FailedRecordCount =@FailedRecordCount out
					
					SELECT @SuccessRecordCount = 0
					UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount , TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
					WHERE ImportProcessLogId = @ImportProcessLogId;
					END

                 END
				
			SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
			SET @SQLQuery = ' Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;
			
			EXEC	sp_executesql @SQLQuery, N'@SuccessRecordCount BIGINT out' , @SuccessRecordCount=@SuccessRecordCount out

			UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount, TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
			WHERE ImportProcessLogId = @ImportProcessLogId;

		   EXEC Znode_ImportReadErrorLog
                  @ImportProcessLogId = @ImportProcessLogId,
                  @NewGUID = @NewGUID;
             DROP TABLE #GlobalTempTableColumns;

             ---- Finally call product insert process if error not found in error log table 

			 SET @GetDate = dbo.Fn_GetDate();
                    --Updating the import process status
					UPDATE ZnodeImportProcessLog
					SET Status = CASE WHEN ISNULL(FailedRecordcount,0) > 0 AND ISNULL(SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
										WHEN ISNULL(FailedRecordcount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
										WHEN ISNULL(FailedRecordcount,0) > 0 AND ISNULL(FailedRecordcount,0) = ISNULL(TotalProcessedRecords,0) THEN dbo.Fn_GetImportStatus( 3 )
									END, 
						ProcessCompletedDate = @GetDate
					WHERE ImportProcessLogId = @ImportProcessLogId;
        END TRY
      
		BEGIN CATCH 
		DECLARE @TempCount TABLE (Id INT)

		Declare @SQL Varchar(max) = 'Select Count(*) As Id From '+@TableName
		INSERT INTO @TempCount
		EXEC (@SQL)

			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportPartialValidatePimProductData @ImportHeadName = '''+ISNULL(@ImportHeadName,'''''')+''',@TableName='''+ISNULL(CAST(@TableName AS
			VARCHAR(50)),'''''')+''',@TemplateId='+ISNULL(CAST(@TemplateId AS VARCHAR(50)),'''')+',@NewGUID='''+ISNULL(@NewGUID,'''''')+''',@UserId='+ISNULL(CAST(@UserId AS VARCHAR(50)),'''')+',@LocaleId='+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''')+',
			@IsCategory='+ISNULL(CAST(@IsCategory AS VARCHAR(50)),'''')+',@DefaultFamilyId='+ISNULL(CAST(@DefaultFamilyId AS VARCHAR(50)),'''')+',@ImportProcessLogId='+ISNULL(CAST(@ImportProcessLogId AS VARCHAR(50)),'''')+',
			@PriceListId='+ISNULL(CAST(@PriceListId AS VARCHAR(50)),'''')+',@CountryCode='''+ISNULL(CAST(@CountryCode AS VARCHAR(50)),'''''')+''',@PimCatalogId='+ISNULL(CAST(@PimCatalogId AS VARCHAR(50)),'''')+',
			@PortalId='+ISNULL(CAST(@PortalId AS VARCHAR(50)),'''')+',@IsAccountAddress='+ISNULL(CAST(@IsAccountAddress AS VARCHAR(50)),'''')

			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;       
			
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
			@ProcedureName = 'Znode_ImportPartialValidatePimProductData',
			@ErrorInProcedure = 'Znode_ImportPartialValidatePimProductData',
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
		END CATCH 

     END;