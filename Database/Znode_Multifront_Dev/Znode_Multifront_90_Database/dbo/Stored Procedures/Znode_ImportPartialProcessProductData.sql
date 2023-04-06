CREATE PROCEDURE [dbo].[Znode_ImportPartialProcessProductData]
(@TblGUID       NVARCHAR(255), 
 @UserId        INT, 
 @IsAutoPublish BIT           = 0
)
AS
    BEGIN

/*
	Summary :   Import PimProduct ( for partial attribute import ) 
	Process :   Read table ##ProductUpdate_GUID table, create import template with columns which is exists  in 
	ZnodePimAttribute table call import which is not dependent on family.
  	                  		  
	SourceColumnName: CSV file column headers
	Unit testing 
	drop table ##ProductUpdate_3 
  
	-- 	Create TABLE ##ProductUpdate_3 (SKU nvarchar(max), ProductName nvarchar(100),ProductType nvarchar(100))
	-- 	insert into ##ProductUpdate_3  Values ('apz231','sdsfd','SimpleProduct')
	-- 	insert into ##ProductUpdate_3  Values ('gr990', 'test','SimpleProduct')
	-- 	insert into ##ProductUpdate_3  Values ('ORRK3456','Test009','SimpleProduct')
	-- 	insert into ##ProductUpdate_3  Values ('hhhhhhh','Test009','dsfsdfsdf')
	-- 	select * from ##ProductUpdate_3
	
	--EXEC [Znode_ImportPartialProcessProductData] @TblGUID = '3', @UserId =2 

	--Select * from View_ManageLinkProductList 
	--select * from ZnodeImportSuccessLog
	--select * from ZnodeImportLog
  
	*/

        BEGIN TRY 
        SET NOCOUNT ON;
        DECLARE @NewuGuId NVARCHAR(255), @SpId BIGINT, @ImportHeadId INT;
        SET @NewuGuId = @TblGUID;-- NEWID()
        DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
        DECLARE @DefaultFamilyId INT= dbo.Fn_GetDefaultPimProductFamilyId();
        DECLARE @LocaleId INT= dbo.Fn_GetDefaultLocaleId();
        DECLARE @TemplateId INT, @Sql NVARCHAR(MAX)= '', @GlobalTableName NVARCHAR(500);
        DECLARE @ImportProcessLogId INT;

		SELECT @TemplateId = ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'ProductUpdate'

        SET @SpId = @@SPID;
        SET @GlobalTableName = 'tempdb..[##PartialProductDetail' + CONVERT(NVARCHAR(500), @SpId) + ']';
        IF OBJECT_ID(@GlobalTableName, 'U') IS NOT NULL
            BEGIN
                SET @Sql = 'DROP TABLE ' + @GlobalTableName;
                EXEC sp_executesql 
                     @SQL;
        END;
        DECLARE @GlobalTemporaryTable NVARCHAR(255);
        DECLARE @CreateTableScriptSql NVARCHAR(MAX)= '', @InsertColumnName NVARCHAR(MAX), @UpdateTable2Column NVARCHAR(MAX), @UpdateTable3Column NVARCHAR(MAX), @UpdateTable4Column NVARCHAR(MAX), @ImportTableColumnName NVARCHAR(MAX), @ImportTableName VARCHAR(200), @TableName NVARCHAR(255)= 'tempdb..[##ProductUpdate_' + @TblGUID + ']', @Attribute NVARCHAR(MAX);
        DECLARE @Attributecode TABLE(Attrcode NVARCHAR(255));
        CREATE TABLE #Attributecode(Attrcode NVARCHAR(255));
        CREATE TABLE #ConfigurableAttributecode
        (SKU            NVARCHAR(255), 
         PimAttributeId INT, 
         DefaultValue   NVARCHAR(255), 
         AttributeCode  NVARCHAR(255), 
         ParentSKU      NVARCHAR(255)
        );
        SELECT @ImportHeadId = ImportHeadId
        FROM dbo.ZnodeImportHead
        WHERE Name = 'ProductUpdate';

        DELETE FROM ZnodeImportTemplateMapping
        WHERE ImportTemplateId = @TemplateId

		if (isnull(@TemplateId,0) = 0 ) 
		Begin 
        INSERT INTO ZnodeImportTemplate
        (ImportHeadId,TemplateName, TemplateVersion,PimAttributeFamilyId, IsActive,  CreatedBy, 
         CreatedDate, 
         ModifiedBy, 
         ModifiedDate
        )
        VALUES
        (@ImportHeadId, 
         'ProductUpdate', 
         1, 
         NULL, 
         1, 
         2, 
         @GetDate, 
         2, 
         @GetDate
        );
        SET @TemplateId = @@Identity;
		END
		
        SET @SQL = '
		INSERT INTO ZnodeImportTemplateMapping ( ImportTemplateId, SourceColumnName, TargetColumnName, DisplayOrder, IsActive, IsAllowNull, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
		select ' + CONVERT(NVARCHAR(100), @TemplateId) + ', a.name, PA.AttributeCode,1,1,1,' + CONVERT(NVARCHAR(100), @UserId) + ' , ''' + CONVERT(NVARCHAR(100), @GetDate) + ''' , ' + CONVERT(NVARCHAR(100), @UserId) + ', ''' + CONVERT(NVARCHAR(100), @GetDate) + '''  
		from tempdb.sys.columns a
		inner join tempdb.sys.tables b on a.object_id = b.object_id 
		inner join ZnodePimAttribute PA on a.name = PA.AttributeCode AND PA.IsCategory =0  
		where b.name in (''##ProductUpdate_' + @TblGUID + ''') ';
        EXEC (@SQL);
        --------------------------------------------


		

        DECLARE @SQLQuery NVARCHAR(MAX);
        IF OBJECT_ID('#WrongData', 'U') IS NOT NULL
            BEGIN
                DROP TABLE #WrongData;
        END;
        CREATE TABLE #WrongData(ColumnName NVARCHAR(100));
        SET @SQLQuery = ' INSERT INTO #WrongData (ColumnName )
		Select a.Name from tempdb.sys.columns a
		inner join tempdb.sys.tables b on a.object_id = b.object_id 
		where b.name in (''##ProductUpdate_' + @TblGUID + ''') 
		and NOT EXISTS (Select TOP 1 1 FROM ZnodePimAttribute PA WHERE a.name = PA.AttributeCode) AND a.Name not in (''SKU'',''guid'') ';
        EXEC sys.sp_sqlexec 
             @SQLQuery;
        SET @ImportProcessLogId = 0;

		
        IF EXISTS
        (
            SELECT TOP 1 1
            FROM #WrongData
        )
            BEGIN
			
                INSERT INTO ZnodeImportProcessLog
                (ImportTemplateId, 
                 STATUS, 
                 ProcessStartedDate, 
                 ProcessCompletedDate, 
                 CreatedBy, 
                 CreatedDate, 
                 ModifiedBy, 
                 ModifiedDate, 
                 ERPTaskSchedulerId
                )
                       SELECT @TemplateId, 
                              dbo.Fn_GetImportStatus(3), 
                              @GetDate, 
                              NULL, 
                              @UserId, 
                              @GetDate, 
                              @UserId, 
                              @GetDate, 
                              NULL;
                SET @ImportProcessLogId = @@IDENTITY;
                SET @SQLQuery = '
			INSERT INTO ZnodeImportLog
					(ErrorDescription,ColumnName,Data,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId,RowNumber )
			Select 19 ,''Attribute '', ColumnName, ''' + @TblGUID + ''',' + CONVERT(NVARCHAR(100), @UserId) + ',''' + CONVERT(NVARCHAR(100), @GetDate) + ''',' + CONVERT(NVARCHAR(100), @UserId) + ',''' + CONVERT(NVARCHAR(100), @GetDate) + ''',' + CONVERT(NVARCHAR(100), @ImportProcessLogId) + ',' + ' NULL  from #WrongData ';
                EXEC sys.sp_sqlexec 
                     @SQLQuery;

                --SELECT	'Job create successfully.'
                --Return 0 

        END;
        --------------------------------------------

        SELECT @InsertColumnName = SUBSTRING(
        (
            SELECT ',[' + [TargetColumnName] + ']'
            FROM [dbo].[ZnodeImportTemplateMapping]
            WHERE [ImportTemplateId] = @TemplateId FOR XML PATH('')
        ), 2, 4000);
	
        SELECT @CreateTableScriptSql = 'CREATE TABLE ' + @GlobalTableName + ' (' + SUBSTRING(
        (
            SELECT ',[' + ISNULL([TargetColumnName], 'NULL') + '] nvarchar(max)'
            FROM [dbo].[ZnodeImportTemplateMapping]
            WHERE [ImportTemplateId] = @TemplateId FOR XML PATH('')
        ), 2, 4000) + ' , GUID nvarchar(255))';
		
        EXEC (@CreateTableScriptSql);
        IF(LEN(@InsertColumnName) > 0)
            BEGIN
                SET @SQL = 'INSERT INTO ' + @GlobalTableName + ' ( ' + @InsertColumnName + ' )	SELECT ' + @InsertColumnName + ' FROM ' + @TableName;
                PRINT @sql;
                EXEC sp_executesql 
                     @SQL;
        END;
		
		
        DECLARE @UpdateTableColumn VARCHAR(MAX);
        SET @Sql = 'UPDATE ' + @GlobalTableName + ' SET GUID= ''' + @NewuGuId + '''';
        EXEC sp_executesql 
             @SQL;

        SET @Sql = 'Update ' + @GlobalTableName + ' SET SKU = Ltrim(Rtrim(SKU)) ';
        EXEC sp_executesql 
             @SQL;

        ---- Import product    
        EXEC Znode_ImportData 
             @TableName = @GlobalTableName, 
             @NewGUID = @TblGUID, 
             @TemplateId = @TemplateId, 
             @UserId = @UserId, 
             @LocaleId = @LocaleId, 
             @DefaultFamilyId = @DefaultFamilyId, 
             @PriceListId = 0, 
             @CountryCode = '',
             --,@IsDoNotCreateJob = 0
             --,@IsDoNotStartJob = 0
             --,@StepName = 'Import'-- 	,@IsDebug = 1  
             @IsAutoPublish = @IsAutoPublish, 
             @ImportProcessLogId = @ImportProcessLogId;
        SELECT 'Job create successfully.';
        END TRY
        BEGIN CATCH 
        DECLARE @Status BIT ;
        select ERROR_MESSAGE ()
        SET @Status = 0;
        DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
        @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportPartialProcessProductData @TblGUID = '''+ISNULL(@TblGUID,'''''')+''',@UserId='+ISNULL(CAST(@UserId AS
        VARCHAR(50)),'''''')+',@IsAutoPublish='+ISNULL(CAST(@IsAutoPublish AS VARCHAR(50)),'''')
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
        EXEC Znode_InsertProcedureErrorLog
        @ProcedureName = 'Znode_ImportPartialProcessProductData',
        @ErrorInProcedure = 'Znode_ImportPartialProcessProductData',
        @ErrorMessage = @ErrorMessage,
        @ErrorLine = @ErrorLine,
        @ErrorCall = @ErrorCall;
        END CATCH 
END;