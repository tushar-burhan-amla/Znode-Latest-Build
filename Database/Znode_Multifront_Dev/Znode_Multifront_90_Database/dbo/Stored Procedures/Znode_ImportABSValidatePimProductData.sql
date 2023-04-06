CREATE PROCEDURE [dbo].[Znode_ImportABSValidatePimProductData]
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
	@PortalId int = 0 )
AS
     SET NOCOUNT ON;

/*
    Summary :   Import PimProduct / Price / Inventory / Category / Category Associated Data 
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
             BEGIN TRAN TRN_ImportValidProductData;
             DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
             DECLARE @SQLQuery NVARCHAR(MAX), @AttributeTypeName NVARCHAR(10), @AttributeCode NVARCHAR(300), @AttributeId INT, @IsRequired BIT, @SourceColumnName NVARCHAR(600), @ControlName VARCHAR(300), @ValidationName VARCHAR(100), @SubValidationName VARCHAR(300), @ValidationValue VARCHAR(300), @RegExp VARCHAR(300), @CreateDateString NVARCHAR(300), @DefaultLocaleId INT, @ImportHeadId INT, @CheckedSourceColumn NVARCHAR(600)= '', @Status BIT= 0,
			    @CsvColumnString nvarchar(max)
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
		  -- If Exists ( Select  count(1)  from #GlobalTempTableColumns GROUP BY ColumnName  Having count(1) > 1 )
		  -- Begin
			 --   INSERT INTO ZnodeImportLog(ErrorDescription,ColumnName,Data,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
    --               Select  46,ColumnName,'',@newGUID,@UserId,@GetDate,@UserId,@GetDate, @ImportProcessLogId  from #GlobalTempTableColumns GROUP BY ColumnName  Having count(1) > 1 
				
				----'Multiple occurance of column are not allow for'
		  -- END
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
             --DELETE FROM ZnodeImportLog WHERE ImportProcessLogId in (select ImportProcessLogId  FROM ZnodeImportProcessLog  WHERE ImportTemplateId  = @TemplateId )
             --GUID = @NewGUID;
             --Delete FROM ZnodeImportProcessLog  WHERE ImportTemplateId  = @TemplateId 
		
             IF NOT EXISTS
             (
                 SELECT TOP 1 1  FROM ZnodeImportLog
                 WHERE Guid = @NewGUID
                       AND ErrorDescription IN(43, 42)
                 AND ImportProcessLogId = @ImportProcessLogId
             )
                 BEGIN
                     IF @ImportHeadName = 'Product'
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
                 END;
				
             --Generate new process for current import 
             --INSERT INTO ZnodeImportProcessLog(ImportTemplateId,Status,ProcessStartedDate,ProcessCompletedDate,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
             --SELECT @TemplateId,dbo.Fn_GetImportStatus(0),@GetDate,NULL,@UserId,@GetDate,@UserId,@GetDate;
             --SET @ImportProcessLogId = @@IDENTITY;
			 if  @ImportHeadName = 'Product'
			 Begin
			    INSERT INTO ZnodeImportLog(ErrorDescription,ColumnName, Data,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
				 Select 
				 Distinct '47' AS ErrorDescription,Attribute,'',@NewGUID,@UserId,@GetDate,@UserId,@GetDate,@ImportProcessLogId
				   from PRDDA where NOT EXISTS (Select TOP 1 1 FROM ZnodePimAttribute where IsCategory = 0 
						 and ZnodePimAttribute.AttributeCode = PRDDA.Attribute )
             End 

             SET @CreateDateString = CONVERT(VARCHAR(100), @UserId)+','''+CONVERT(VARCHAR(100), @GetDate)+''','+CONVERT(VARCHAR(100), @UserId)+','''+CONVERT(VARCHAR(100), @GetDate)+''', '+CONVERT(VARCHAR(100), @ImportProcessLogId);

             SELECT TOP 1 @ImportHeadId = ImportHeadId FROM ZnodeImportTemplate WHERE ImportTemplateId = @TemplateId;
             IF @DefaultFamilyId = 0
                AND @ImportHeadName IN('Product', 'Category')
                 BEGIN 
                     --Get all default attribute values in attribute 
                     INSERT INTO @FamilyAttributeDetail
                     (PimAttributeId,
                      AttributeTypeName,
                      AttributeCode,
                      SourceColumnName,
                      IsRequired,
                      PimAttributeFamilyId
                     )
                     --Call Process to insert data of defeult family with source column name and target column name 
                     EXEC Znode_ImportGetTemplateDetails
                          @TemplateId = @TemplateId,
                          @IsValidationRules = 0,
                          @IsIncludeRespectiveFamily = 1,
                          @IsCategory = @IsCategory,
                          @DefaultFamilyId = @DefaultFamilyId;
                 END;
             ELSE
             IF @ImportHeadName IN('Product', 'Category')
                 BEGIN
                     --Get all default attribute values in attribute 
                     INSERT INTO @FamilyAttributeDetail
                     (PimAttributeId,
                      AttributeTypeName,
                      AttributeCode,
                      SourceColumnName,
                      IsRequired,
                      PimAttributeFamilyId
                     )
                     --Call Process to insert data of defeult family with source column name and target column name 
                     EXEC Znode_ImportGetTemplateDetails
                          @TemplateId = @TemplateId,
                          @IsValidationRules = 0,
                          @IsIncludeRespectiveFamily = 1,
                          @IsCategory = @IsCategory,
                          @DefaultFamilyId = @DefaultFamilyId;
                 END;      
             -- Check attributes are manditory and not provided with source table
		   	 
			if @TABLENAME	like '%tempdb..%'
				SET @SQLQuery = 'SELECT 42 AS ErrorDescription , SourceColumnName , '''' , '''+@NewGUID+''','+@CreateDateString+' from ZnodeImportTemplateMapping where ImportTemplateId = '+CONVERT(VARCHAR(100), @TemplateId)+' and ltrim(rtrim(SourceColumnName)) <> '''' AND ltrim(rtrim(SourceColumnName)) not in ( select isnull(Name ,'''') from tempdb.sys.columns where object_id = object_id('''+@TABLENAME+'''));';
			else 
				SET @SQLQuery = 'SELECT 42 AS ErrorDescription , SourceColumnName , '''' , '''+@NewGUID+''','+@CreateDateString+' from ZnodeImportTemplateMapping where ImportTemplateId = '+CONVERT(VARCHAR(100), @TemplateId)+' and ltrim(rtrim(SourceColumnName)) <> '''' AND ltrim(rtrim(SourceColumnName)) not in ( select isnull(Name ,'''') from sys.columns where object_id = object_id('''+@TABLENAME+'''));';

			Declare @Tbl_CsvDynamicColulmns TABLE (ColumnName nvarchar(300), SequenceNumber int, DataType nvarchar(50),IsRequired bit )

			INSERT INTO @Tbl_CsvDynamicColulmns(ColumnName , SequenceNumber , DataType ,IsRequired)
			SELECT DISTINCT ZITM.SourceColumnName ,ZIAV.SequenceNumber, ZIAV.AttributeTypeName, ZIAV.IsRequired
			FROM ZnodeImportAttributeValidation ZIAV LEFT OUTER JOIN 
			ZnodeImportTemplate  ZIT ON ZIT.ImportHeadId =  ZIAV.ImportHeadId AND ZIT.ImportTemplateId  = @TemplateId
			LEFT OUTER JOIN ZnodeImportTemplateMapping  ZITM ON ZITM.ImportTemplateId = ZIT.ImportTemplateId  
			and ZIAV.AttributeCode = ZITM.TargetColumnName
			AND ZITM.ImportTemplateId  = @TemplateId
			WHERE ZIAV.ImportHeadId = @ImportHeadId --ORDER BY ZIAV.SequenceNumber

		    SELECT @CsvColumnString = SUBSTRING ((Select ',' +  ISNULL(ColumnName ,'NULL') from @Tbl_CsvDynamicColulmns ORDER BY SequenceNumber FOR XML PATH ('')),2,4000) 

     		INSERT INTO ZnodeImportLog(ErrorDescription, ColumnName, Data, GUID,CreatedBy, CreatedDate,  ModifiedBy,ModifiedDate,ImportProcessLogId
             )
             EXEC sys.sp_sqlexec
                  @SQLQuery;
             IF NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeImportLog
                 WHERE Guid = @NewGUID
                       AND ErrorDescription IN(43, 42)
                 AND ImportProcessLogId = @ImportProcessLogId
             )
             BEGIN
                     --Get all default attribute values in attribute 
                     IF @ImportHeadName IN('Product', 'Category')
                         BEGIN
                             -- Check attributes are manditory and not provided with source table
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
                                    SELECT '14' AS ErrorDescription,
                                           AttributeCode,
                                           '',
                                           @NewGUID,
                                           @UserId,
                                           @GetDate,
                                           @UserId,
                                           @GetDate,
                                           @ImportProcessLogId
                                    FROM @FamilyAttributeDetail
                                    WHERE ISNULL(SourceColumnName, '') = ''
                                          AND IsRequired = 1;  

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
                                  @TemplateId;
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
                     ELSE
                         BEGIN
					
					
                             --Read all attribute details with their datatype
                             INSERT INTO @AttributeDetail
                             (AttributeTypeName,
                              AttributeCode,
                              SourceColumnName,
                              IsRequired,
                              ControlName,
                              ValidationName,
                              SubValidationName,
                              ValidationValue,
                              RegExp
                             )
                             EXEC [Znode_ImportGetOtherTemplateDetails]
                                  @TemplateId = @TemplateId,
                                  @ImportHeadId = @ImportHeadId;
						
                             --Check attributes are not mapped with any family of Pim Product
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
                                    SELECT DISTINCT
                                           '14' AS ErrorDescription,
                                           AttributeCode,
                                           '',
                                           @NewGUID,
                                           @UserId,
                                           @GetDate,
                                           @UserId,
                                           @GetDate,
                                           @ImportProcessLogId
                                    FROM @AttributeDetail
                                    WHERE ISNULL(SourceColumnName, '') = ''   AND IsRequired = 1;  ;

                         END;
				     
                     --	Check attributes are not mapped with (Default / Other) family of Pim Product
                     --	INSERT INTO ZnodeImportLog ( ErrorDescription , ColumnName , Data , GUID , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate , ImportProcessLogId)
                     --	SELECT '1' AS ErrorDescription , SourceColumnName , '' , @NewGUID , @UserId , @GetDate , @UserId , @GetDate , @ImportProcessLogId
                     --	FROM @AttributeDetail WHERE PimAttributeId NOT IN ( SELECT zpfgm.PimAttributeId FROM dbo.ZnodePimFamilyGroupMapper AS zpfgm);
                     --	Verify data in global temporary table (column wise)

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
									EXEC Znode_ImportValidateManditoryData
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
                                          @ImportHeadId = @ImportHeadId;
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
                             --Check Default data value is valid 
                             IF @ImportHeadName IN('Product', 'Category')
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
             COMMIT TRAN TRN_ImportValidProductData;
			 
  			 SET @SQLQuery = 'Delete FROM  '+@TableName+' Where Rownumber in (Select Rownumber from ZnodeImportLog  WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND Rownumber is not null)';
             EXEC sys.sp_sqlexec  @SQLQuery;
			 

			 --SET @SQLQuery = 'Select * FROM  '+@TableName
    --         EXEC sys.sp_sqlexec  @SQLQuery;

             IF @ImportHeadName IN('Product', 'Category')
                 BEGIN
                     IF NOT EXISTS
                     (
                         SELECT TOP 1 1
                         FROM @FamilyAttributeDetail
                         WHERE ISNULL(SourceColumnName, '') = ''
                               AND IsRequired = 1
                     ) AND NOT EXISTS
					 (
						 SELECT TOP 1 1
						 FROM ZnodeImportLog
						 WHERE Guid = @NewGUID
							   AND ErrorDescription IN(43, 42)
						 AND ImportProcessLogId = @ImportProcessLogId
					 )
                         BEGIN
                             IF @IsCategory = 0
                                 BEGIN
			
                                     EXEC Znode_ImportPimProductData
                                          @TableName = @TableName,
                                          @NewGUID = @NewGUID,
                                          @TemplateId = @TemplateId,
                                          @ImportProcessLogId = @ImportProcessLogId,
                                          @UserId = @UserId,
                                          @LocaleId = @LocaleId,
                                          @DefaultFamilyId = @DefaultFamilyId;

                                 END;
                             ELSE
                                 BEGIN
                                     EXEC Znode_ImportPimCategoryData
                                          @TableName = @TableName,
                                          @NewGUID = @NewGUID,
                                          @TemplateId = @TemplateId,
                                          @ImportProcessLogId = @ImportProcessLogId,
                                          @UserId = @UserId,
                                          @LocaleId = @LocaleId,
                                          @DefaultFamilyId = @DefaultFamilyId;
                                 END;
                         END;
                 END;
				IF NOT EXISTS
					 (
						 SELECT TOP 1 1
						 FROM ZnodeImportLog
						 WHERE Guid = @NewGUID
							   AND ErrorDescription IN(43, 42)
						 AND ImportProcessLogId = @ImportProcessLogId
					 )
             BEGIN
                 IF @ImportHeadName = 'Pricing'
                     BEGIN
                         EXEC [Znode_ImportPriceList]
                              @TableName = @TableName,
                              @Status = @Status,
                              @UserId = @UserId,
                              @ImportProcessLogId = @ImportProcessLogId,
                              @NewGUID = @NewGUID,
                              @PriceListId = @PriceListId;
                     END;

                 IF @ImportHeadName = 'Inventory'
                     BEGIN
				
                         EXEC Znode_ImportInventory_Ver1
                              @TableName = @TableName,
                              @Status = @Status,
                              @UserId = @UserId,
                              @ImportProcessLogId = @ImportProcessLogId,
                              @NewGUID = @NewGUID;
                     END;
                 IF @ImportHeadName = 'ZipCode'
                     BEGIN
						 EXEC Znode_ImportZipCode
                              @TableName = @TableName,
                              @Status = @Status,
                              @UserId = @UserId,
                              @ImportProcessLogId = @ImportProcessLogId,
                              @NewGUID = @NewGUID,
							  @CountryCode = @CountryCode;
                     END;
					 IF @ImportHeadName = 'CategoryAssociation'
                     BEGIN
						 EXEC Znode_ImportCatalogCategory
                              @TableName = @TableName,
                              @Status = @Status,
                              @UserId = @UserId,
                              @ImportProcessLogId = @ImportProcessLogId,
                              @NewGUID = @NewGUID,
							  @PimCatalogId = @PimCatalogId;
                     END;
					 IF @ImportHeadName = 'ProductAssociation'
                     BEGIN
						 EXEC Znode_ImportAssociateProducts
                              @TableName = @TableName,
                              @Status = @Status,
                              @UserId = @UserId,
                              @ImportProcessLogId = @ImportProcessLogId,
                              @NewGUID = @NewGUID
                     END;
			
					 IF @ImportHeadName = 'SEODetails' AND @PortalId > 0 
                     BEGIN
						 EXEC Znode_ImportSEODetails
                              @TableName = @TableName,
                              @Status = @Status,
                              @UserId = @UserId,
							  @LocaleId = @LocaleId,
							  @PortalId =@PortalId,
                              @ImportProcessLogId = @ImportProcessLogId,
                              @NewGUID = @NewGUID
				
                     END;
					 
					 IF @ImportHeadName = 'Attribute' 
                     BEGIN
						 EXEC Znode_ImportAttributes
                              @TableName = @TableName,
                              @Status = @Status,
                              @UserId = @UserId,
							  @ImportProcessLogId = @ImportProcessLogId,
                              @NewGUID = @NewGUID
				
                     END;

					 IF @ImportHeadName = 'Customer' AND @PortalId > 0 
                     BEGIN
						 EXEC Znode_ImportCustomer
                              @TableName = @TableName,
                              @Status	 = @Status,
                              @UserId	 = @UserId,
							  @LocaleId	 = @LocaleId,
							  @PortalId  = @PortalId,
                              @ImportProcessLogId = @ImportProcessLogId,
                              @NewGUID	 = @NewGUID,
							  @CsvColumnString =@CsvColumnString
				
                     END;

					 IF @ImportHeadName = 'CustomerAddress' --AND @PortalId > 0 
                     BEGIN
						 EXEC Znode_ImportCustomerAddress
                              @TableName = @TableName,
                              @Status	 = @Status,
                              @UserId	 = @UserId,
							  @LocaleId	 = @LocaleId,
							  @PortalId  = 1, -- not implemented from forntend 
                              @ImportProcessLogId = @ImportProcessLogId,
                              @NewGUID	 = @NewGUID,
							  @CsvColumnString =@CsvColumnString
				
                     END;
 
				 
             END;

             EXEC Znode_ImportReadErrorLog
                  @ImportProcessLogId = @ImportProcessLogId,
                  @NewGUID = @NewGUID;
             DROP TABLE #GlobalTempTableColumns;

             -- Finally call product insert process if error not found in error log table 
             IF EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeImportLog
                 WHERE ImportProcessLogId = @ImportProcessLogId
                       AND Guid = @NewGUID
             )
                 BEGIN
                     --Update process with completed status for current import 
                     UPDATE ZnodeImportProcessLog
                       SET
                           Status = dbo.Fn_GetImportStatus(3),
                           ProcessCompletedDate = @GetDate
                       WHERE ImportProcessLogId = @ImportProcessLogId;
                 END;
				 SET @SQLQuery = 'Drop Table ' + @TableName
                 EXEC sys.sp_sqlexec @SQLQuery;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE(),
                    ERROR_LINE(),
                    ERROR_PROCEDURE();
             EXEC Znode_ImportReadErrorLog
                  @ImportProcessLogId = @ImportProcessLogId,
                  @NewGUID = @NewGUID; 
             --Update process with failed status for current import 
             UPDATE ZnodeImportProcessLog
               SET
                   Status = dbo.Fn_GetImportStatus(3),
                   ProcessCompletedDate = @GetDate
             WHERE ImportProcessLogId = @ImportProcessLogId;
			 				 SET @SQLQuery = 'Drop Table ' + @TableName
                 EXEC sys.sp_sqlexec @SQLQuery;
             ROLLBACK TRAN TRN_ImportValidProductData;
         END CATCH;
     END;