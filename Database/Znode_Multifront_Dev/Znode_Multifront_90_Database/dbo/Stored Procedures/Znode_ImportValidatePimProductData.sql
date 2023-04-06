CREATE PROCEDURE [dbo].[Znode_ImportValidatePimProductData]
(   
	@ImportHeadName     VARCHAR(200),
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
        --BEGIN TRAN TRN_ImportValidProductData;
        DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
        DECLARE @SQLQuery NVARCHAR(MAX), @AttributeTypeName NVARCHAR(100), @AttributeCode NVARCHAR(300), @AttributeId INT, @IsRequired BIT, @SourceColumnName NVARCHAR(600), @ControlName VARCHAR(300), @ValidationName VARCHAR(100), @SubValidationName VARCHAR(300), @ValidationValue VARCHAR(300), @RegExp VARCHAR(300), @CreateDateString NVARCHAR(300), @DefaultLocaleId INT, @ImportHeadId INT, @CheckedSourceColumn NVARCHAR(600)= '', @Status BIT= 0,
		@CsvColumnString nvarchar(max),
		@FailedRecordCount BIGINT,
		@SuccessRecordCount BIGINT
    --To get the total record count for update purpose in catch block
    SET @SQLQuery = 'SELECT '+CAST(@ImportProcessLogId AS VARCHAR(10))+',COUNT(*) FROM '+@TableName
	INSERT INTO Znode_ImportCsvRowCount
	EXEC (@SQLQuery) 

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

	CREATE TABLE #DefaultAttributeCode
	(AttributeTypeName          VARCHAR(300),
	PimAttributeDefaultValueId INT,
	PimAttributeId             INT,
	AttributeDefaultValueCode  VARCHAR(100)
	);

	IF( @ImportHeadName = 'B2BCustomer' )
	BEGIN
		EXEC ZnodeB2BCustomerMapping @ImportHeadName = @ImportHeadName, @TableName = @TableName
	END
		
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

			---- Deleted Attribute which are not provided in product import CSV and required attribute not mapped with AttributeGroup
			Delete FAD from @FamilyAttributeDetail FAD
			where AttributeCode not in (select Name from tempdb.sys.columns where object_id = object_id(@TableName))
			and not exists(select * from ZnodePimAttributeGroupMapper ZPAGM inner join ZnodePimFamilyGroupMapper ZPFGM on ZPAGM.PimAttributeGroupId = ZPFGM.PimAttributeGroupId 
					        inner join ZnodePimAttribute ZPA on ZPAGM.PimAttributeId = ZPA.PimAttributeId and FAD.AttributeCode = ZPA.AttributeCode)
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

			---- Deleted Attribute which are not provided in product import CSV and required attribute not mapped with AttributeGroup
			Delete FAD from @FamilyAttributeDetail FAD
			where AttributeCode not in (select Name from tempdb.sys.columns where object_id = object_id(@TableName))
			and not exists(select * from ZnodePimAttributeGroupMapper ZPAGM inner join ZnodePimFamilyGroupMapper ZPFGM on ZPAGM.PimAttributeGroupId = ZPFGM.PimAttributeGroupId 
					        inner join ZnodePimAttribute ZPA on ZPAGM.PimAttributeId = ZPA.PimAttributeId and FAD.AttributeCode = ZPA.AttributeCode)
            END;      
        -- Check attributes are manditory and not provided with source table
		   	 
	if @TABLENAME	like '%tempdb..%'
		SET @SQLQuery = 'SELECT 42 AS ErrorDescription , SourceColumnName , '''' , '''+@NewGUID+''','+@CreateDateString+' from ZnodeImportTemplateMapping where ImportTemplateId = '+CONVERT(VARCHAR(100), @TemplateId)+' and ltrim(rtrim(SourceColumnName)) <> '''' AND ltrim(rtrim(SourceColumnName)) not in ( select isnull(Name ,'''') from tempdb.sys.columns where object_id = object_id('''+@TABLENAME+'''));';
	else 
		SET @SQLQuery = 'SELECT 42 AS ErrorDescription , SourceColumnName , '''' , '''+@NewGUID+''','+@CreateDateString+' from ZnodeImportTemplateMapping where ImportTemplateId = '+CONVERT(VARCHAR(100), @TemplateId)+' and ltrim(rtrim(SourceColumnName)) <> '''' AND ltrim(rtrim(SourceColumnName)) not in ( select isnull(Name ,'''') from sys.columns where object_id = object_id('''+@TABLENAME+'''));';
	 
	Declare @Tbl_CsvDynamicColulmns TABLE (ColumnName nvarchar(300), SequenceNumber int, DataType nvarchar(50),IsRequired bit )
	IF @ImportHeadName = 'Promotions'
	BEGIN
			
		INSERT INTO @Tbl_CsvDynamicColulmns(ColumnName)
		EXEC Znode_ImportGetDefaultFamilyAttribute @importHeadId=@importHeadId,@PimAttributeFamilyId=0,@PromotionTypeId=@PromotionTypeId
	END
	ELSE
	BEGIN
		INSERT INTO @Tbl_CsvDynamicColulmns(ColumnName , SequenceNumber , DataType ,IsRequired)
		SELECT DISTINCT ZITM.SourceColumnName ,ZIAV.SequenceNumber, ZIAV.AttributeTypeName, ZIAV.IsRequired
		FROM ZnodeImportAttributeValidation ZIAV LEFT OUTER JOIN 
		ZnodeImportTemplate  ZIT ON ZIT.ImportHeadId =  ZIAV.ImportHeadId AND ZIT.ImportTemplateId  = @TemplateId
		LEFT OUTER JOIN ZnodeImportTemplateMapping  ZITM ON ZITM.ImportTemplateId = ZIT.ImportTemplateId  
		and ZIAV.AttributeCode = ZITM.TargetColumnName
		AND ZITM.ImportTemplateId  = @TemplateId
		WHERE ZIAV.ImportHeadId = @ImportHeadId
		AND ISNULL(ZITM.SourceColumnName,'') <> ''--ORDER BY ZIAV.SequenceNumber
	END

	SELECT @CsvColumnString = SUBSTRING ((Select ',' +  ISNULL(ColumnName ,'NULL') from @Tbl_CsvDynamicColulmns ORDER BY SequenceNumber FOR XML PATH ('')),2,4000) 


    INSERT INTO ZnodeImportLog(ErrorDescription, ColumnName, Data, GUID,CreatedBy, CreatedDate,  ModifiedBy,ModifiedDate,ImportProcessLogId
        )
        EXEC sys.sp_sqlexec  @SQLQuery;
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
					DELETE FROM @AttributeDetail
                        WHERE AttributeTypeName = 'File'
                            AND ValidationName <> 'IsAllowMultiUpload';

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

					IF @ImportHeadName IN('B2BCustomer')
					BEGIN

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
							EXEC [Znode_ImportGetGlobalTemplateDetails]
								@TemplateId = @TemplateId,
								@ImportHeadId = @ImportHeadId;

								
						INSERT INTO #DefaultAttributeCode
						(AttributeTypeName,
						PimAttributeDefaultValueId,
						PimAttributeId,
						AttributeDefaultValueCode
						)
						--Call Process to insert default data value 
						EXEC Znode_ImportGetGlobalAttributeDefaultValue;

						DELETE FROM #DefaultAttributeCode
						WHERE AttributeTypeName = 'Yes/No';

					END
						
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
                        IF @AttributeTypeName in ( 'Image','File')
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
                        IF @ImportHeadName IN('Product', 'Category','B2BCustomer')
                            BEGIN
                                IF @AttributeId IN
                                (
                                    SELECT PimAttributeId
                                    FROM #DefaultAttributeCode
                                )
                                    BEGIN
							
                                    IF  @AttributeTypeName = 'Multi Select'
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
										END
										ELSE IF @AttributeTypeName = 'Simple Select'
										BEGIN
						
										---Verify Image file is exists in media table or not 
											SET @SQLQuery = ' INSERT INTO #InvalidDefaultData (RowNumber, Value, ColumnName) 
											SELECT ROWNUMBER , ' + @SourceColumnName + ' , ''' + @SourceColumnName + ''' as [ColumnName]  FROM ' + @TableName
											+ ' SP Where ISnull(' + @SourceColumnName +  ','''') <> '''' AND 
											NOT EXISTS 
											(Select TOP 1 1 FROM #DefaultAttributeCode DAC WHERE 
											DAC.AttributeTypeName <> ''Yes/No'' AND DAC.AttributeDefaultValueCode IS NOT NULL AND DAC.PimAttributeId = 
											' + CONVERT(VARCHAR(100), @AttributeId) + ' AND ltrim(rtrim(SP.' + @SourceColumnName + ') ) = DAC.AttributeDefaultValueCode ) '
							
										EXEC sys.sp_sqlexec @SQLQuery;
										END   
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
             
			 
			  
------------------------------------------------------------------------------------------
	Declare @SQLQueryNew NVARCHAR(4000)
	Declare @SourceColumnNameProduct nvarchar(4000) 
    IF @ImportHeadName IN('Product','Pricing','ProductAssociation','Inventory')
	BEGIN
		 	 
	SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'SKU'
	AND ImportTemplateId = @TemplateId


	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  SKU - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]'' 
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;			
END
ELSE IF @ImportHeadName IN('ProductAttribute')
BEGIN
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'AttributeCode'
AND ImportTemplateId = @TemplateId

	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  Attribute - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]''  
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;
END
ELSE IF @ImportHeadName = 'ZipCode'
BEGIN
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'ZIP'
AND ImportTemplateId = @TemplateId

	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  ZIPCode - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]'' 
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;
END
ELSE IF @ImportHeadName = 'Category'
BEGIN
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'CategoryCode'
AND ImportTemplateId = @TemplateId

	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  CategoryCode - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]'' 
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;
END
ELSE IF @ImportHeadName = 'CategoryAssociation'
BEGIN
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'CategoryName'
AND ImportTemplateId = @TemplateId

	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  CategoryName - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]'' 
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;

END
ELSE IF @ImportHeadName IN ('Customer','CustomerAddress')
BEGIN
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'UserName'
AND ImportTemplateId = @TemplateId

	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  UserName - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]''  
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;
END
ELSE IF @ImportHeadName = 'SEODetails'
BEGIN
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'Code'
AND ImportTemplateId = @TemplateId

	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  Code - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]'' 
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;
END
ELSE IF @ImportHeadName = 'Highlight'
BEGIN
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'HighlightCode'
AND ImportTemplateId = @TemplateId

	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  HighlightCode - '' + ' + ' cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]''  
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;
END
ELSE IF @ImportHeadName = 'AddonAssociation'
BEGIN
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'SKU'
AND ImportTemplateId = @TemplateId

	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  SKU - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]''  
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;
END
ELSE IF @ImportHeadName = 'AttributeDefaultValue'
BEGIN
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'AttributeDefaultValueCode'
AND ImportTemplateId = @TemplateId

	SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  AttributeDefaultValueCode - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]'' 
	FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber 
	WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';
    PRINT @SQLQueryNew
	EXEC sys.sp_sqlexec  @SQLQueryNew;
END
ELSE IF @ImportHeadName = 'Brands'  
BEGIN  
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'BrandCode'  
AND ImportTemplateId = @TemplateId  
  
    SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  BrandCode - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(50)) +' + ''' ]''   
    FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber   
    WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';  
    PRINT @SQLQueryNew  
    EXEC sys.sp_sqlexec  @SQLQueryNew;  
END  

ELSE IF @ImportHeadName = 'Promotions'  
BEGIN  
SELECT @SourceColumnNameProduct =  SourceColumnName from ZnodeImportTemplateMapping where TargetColumnName = 'PromoCode'  
AND ImportTemplateId = @TemplateId  
  
    SET @SQLQueryNew = 'Update ZIL SET ZIL.ColumnName =   ZIL.ColumnName ' + '  ' + ' + ' + ' '  + ''' [  PromoCode - '' + ' + '  cast( ' + @SourceColumnNameProduct + ' as varchar(300)) +' + ''' ]''   
    FROM  '+@TableName+' T Inner join  ZnodeImportLog  ZIL ON T.Rownumber = ZIL.RowNumber   
    WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND ZIL.Rownumber IS NOT NULL';  
    PRINT @SQLQueryNew  
    EXEC sys.sp_sqlexec  @SQLQueryNew;  
END 
-------------------------------------------------------------------------------------------------------------
	
--------------------------------------------------------------------------------------------------------------------
			 
SET @SQLQuery = 'Delete FROM  '+@TableName+' Where Rownumber IN (Select Rownumber FROM ZnodeImportLog  WHERE ImportProcessLogId = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+' AND Rownumber IS NOT NULL)';
EXEC sys.sp_sqlexec  @SQLQuery;
			 			
   
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
                    END
					ELSE
					BEGIN
						-- Update Record count in log 
								
							
						--SET @SQLQuery = ' Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;
						--EXEC	sp_executesql @SQLQuery, N'@SuccessRecordCount BIGINT out' , @SuccessRecordCount=@SuccessRecordCount out
						--UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount, TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
						--WHERE ImportProcessLogId = @ImportProcessLogId;

						SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
						SET @SQLQuery = ' Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;
						EXEC	sp_executesql @SQLQuery, N'@SuccessRecordCount BIGINT out' , @SuccessRecordCount=@SuccessRecordCount out
						UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount, TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
						WHERE ImportProcessLogId = @ImportProcessLogId;
					END

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
                        @NewGUID = @NewGUID,
						@CsvColumnString = @CsvColumnString 

				
                END;
				
				IF @ImportHeadName = 'ProductAttribute' 
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
					 
				IF @ImportHeadName = 'UserApprovers' AND @PortalId > 0 
                BEGIN
					EXEC Znode_ImportUserApproval
                        @TableName = @TableName,
                        @Status	 = @Status,
                        @UserId	 = @UserId,
						@LocaleId	 = @LocaleId,
						@PortalId  = @PortalId,
                        @ImportProcessLogId = @ImportProcessLogId,
                        @NewGUID	 = @NewGUID,
						@CsvColumnString =@CsvColumnString
				
                END;

				IF @ImportHeadName = 'B2BCustomer' AND @PortalId > 0 
                BEGIN

						EXEC Znode_ImportB2BCustomer
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
						--@PortalId  = 7, -- not implemented from forntend 
                        @ImportProcessLogId = @ImportProcessLogId,
                        @NewGUID	 = @NewGUID,
						@CsvColumnString =@CsvColumnString,
						@IsAccountAddress = @IsAccountAddress
				
                END;
				IF @ImportHeadName = 'ShippingAddress' --AND @PortalId > 0 
                BEGIN
					EXEC Znode_ImportCustomerAddress
                        @TableName = @TableName,
                        @Status	 = @Status,
                        @UserId	 = @UserId,
						@LocaleId	 = @LocaleId,
						--@PortalId  = 7, -- not implemented from forntend 
                        @ImportProcessLogId = @ImportProcessLogId,
                        @NewGUID	 = @NewGUID,
						@CsvColumnString =@CsvColumnString,
						@IsAccountAddress = @IsAccountAddress
				
                END;
				IF @ImportHeadName = 'StoreLocator' --AND @PortalId > 0 
                BEGIN
					EXEC Znode_ImportStoreLocatorAddress
                        @TableName = @TableName,
                        @Status	 = @Status,
                        @UserId	 = @UserId,
						@ImportProcessLogId = @ImportProcessLogId,
                        @NewGUID	 = @NewGUID,
						@CsvColumnString =@CsvColumnString
                END;

			IF @ImportHeadName = 'Highlight'
			BEGIN
			EXEC Znode_ImportHighlight 
			@TableName = @TableName, 
			@Status = @Status, 
			@UserId = @UserId, 
			@ImportProcessLogId = @ImportProcessLogId, 
			@NewGUID = @NewGUID 
			END;

			IF @ImportHeadName = 'AddonAssociation'
			BEGIN
			EXEC Znode_ImportAddonAssociation 
			@TableName = @TableName, 
			@Status = @Status, 
			@UserId = @UserId, 
			@ImportProcessLogId = @ImportProcessLogId, 
			@NewGUID = @NewGUID,
			@PimCatalogId = @PimCatalogId
			END;

			IF @ImportHeadName = 'AttributeDefaultValue'
			BEGIN
			EXEC Znode_ImportAttributeDefaultValue 
			@TableName = @TableName, 
			@Status = @Status, 
			@UserId = @UserId, 
			@ImportProcessLogId = @ImportProcessLogId, 
			@NewGUID = @NewGUID
					
			END;

			IF @ImportHeadName = 'Voucher'
			BEGIN
				EXEC Znode_ImportVoucher 
				@TableName = @TableName, 
				@Status = @Status, 
				@UserId = @UserId, 
				@ImportProcessLogId = @ImportProcessLogId, 
				@NewGUID = @NewGUID
					
			END;

			IF @ImportHeadName = 'Account'
			BEGIN
						
				EXEC Znode_ImportAccount 
				@TableName = @TableName, 
				@Status = @Status, 
				@UserId = @UserId, 
				@ImportProcessLogId = @ImportProcessLogId, 
				@NewGUID = @NewGUID,
				@CsvColumnString = @CsvColumnString,
				@PortalId = @PortalId
					
			END;

			IF @ImportHeadName = 'Synonyms'
			BEGIN
				EXEC Znode_ImportSynonyms 
				@TableName = @TableName, 
				@Status = @Status, 
				@UserId = @UserId, 
				@ImportProcessLogId = @ImportProcessLogId, 
				@NewGUID = @NewGUID
			END

			IF @ImportHeadName = 'CatalogCategoryAssociation'
			BEGIN
				EXEC Znode_ImportCatalogCategoryHierarchyAssociation 
				@TableName = @TableName, 
				@Status = @Status, 
				@UserId = @UserId, 
				@ImportProcessLogId = @ImportProcessLogId, 
				@NewGUID = @NewGUID,
				@PimCatalogId = @PimCatalogId
			END
				 

            IF @ImportHeadName = 'Brands'
			BEGIN
				EXEC Znode_ImportBrands 
				@TableName = @TableName, 
				@Status = @Status, 
				@UserId = @UserId, 
				@ImportProcessLogId = @ImportProcessLogId, 
				@NewGUID = @NewGUID,
				@LocaleId	 = @LocaleId,
                @CsvColumnString = @CsvColumnString
			END;

			IF @ImportHeadName = 'Promotions'
			BEGIN
				EXEC Znode_ImportPromotions 
				@TableName = @TableName,
				@Status = @Status, 
				@UserId = @UserId, 
				@ImportProcessLogId = @ImportProcessLogId, 
				@NewGUID = @NewGUID,
				@LocaleId	 = @LocaleId,
                @CsvColumnString = @CsvColumnString,
				@PromotionTypeId = @PromotionTypeId
			END;
				 
        END
		ELSE 
			BEGIN
			-- Update Record count in log 	
			SET @SQLQuery = ' Select @FailedRecordCount = count(DISTINCT RowNumber) FROM '+ @TableName ;
			EXEC	sp_executesql @SQLQuery , N'@FailedRecordCount BIGINT out' , @FailedRecordCount =@FailedRecordCount out
			SELECT @SuccessRecordCount = 0
									
			UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount, TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
			WHERE ImportProcessLogId = @ImportProcessLogId;

			END

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

			SET @GetDate = dbo.Fn_GetDate();
            --Updating the import process status
			UPDATE ZnodeImportProcessLog
			SET Status = CASE WHEN ISNULL(FailedRecordcount,0) > 0 AND ISNULL(SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
								WHEN ISNULL(FailedRecordcount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
								WHEN ISNULL(FailedRecordcount,0) > 0 AND ISNULL(FailedRecordcount,0) = ISNULL(TotalProcessedRecords,0) THEN dbo.Fn_GetImportStatus( 3 )
							END, 
				ProcessCompletedDate = @GetDate
			WHERE ImportProcessLogId = @ImportProcessLogId;
            END;
			SET @SQLQuery = 'IF Object_id(''+@TableName+'') IS NOT NULL  DROP TABLE ' + @TableName
            EXEC sys.sp_sqlexec @SQLQuery;

			print 'end';
    END TRY
    BEGIN CATCH
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE()
		DECLARE @TempCount TABLE (Id INT)

		Declare @SQL Varchar(max) = 'Select Count(*) As Id From '+@TableName
		INSERT INTO @TempCount
		EXEC (@SQL)

			SELECT ERROR_MESSAGE(),
				ERROR_LINE(),
				ERROR_PROCEDURE();
			EXEC Znode_ImportReadErrorLog
				@ImportProcessLogId = @ImportProcessLogId,
				@NewGUID = @NewGUID; 
             
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

		SET @SQLQuery = 'Drop Table ' + @TableName
		EXEC sys.sp_sqlexec @SQLQuery;
			--ROLLBACK TRAN TRN_ImportValidProductData;
    END CATCH;
END;