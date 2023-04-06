CREATE PROCEDURE [dbo].[Znode_ImportProcessAttributeData](@TblGUID nvarchar(255), @ERPTaskSchedulerId int )
AS
BEGIN
	
	SET NOCOUNT ON;
	Declare @NewuGuId nvarchar(255),@TemplateId INT , @PortalId INT 
	DECLARE @LocaleId  int = dbo.Fn_GetDefaultLocaleId()
	SELECT TOP 1 @PortalId  = PortalId FROM dbo.ZnodePortal
	set @NewuGuId = newid()
	Declare @GlobalTemporaryTable nvarchar(255)
	IF OBJECT_ID('tempdb.dbo.##AttributeDetail', 'U') IS NOT NULL 
		DROP TABLE tempdb..##AttributeDetail
	SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'AttributeTemplate'
	DECLARE @CreateTableScriptSql NVARCHAR(MAX) = '', 
			@TableName NVARCHAR(200) = 'AttributeRawData',	
		    @InsertColumnName   NVARCHAR(MAX), 
			@ImportTableColumnName NVARCHAR(MAX),
			@Sql NVARCHAR(MAX) = ''
	SET @GlobalTemporaryTable = 'tempdb..[##' + @TableName + '_' + @TblGUID + ']' 
	SELECT @CreateTableScriptSql = 'CREATE TABLE tempdb..##AttributeDetail (GUID nvarchar(255),'+SUBSTRING ((Select ',' +  ISNULL([TargetColumnName] ,'NULL')+ ' nvarchar(max)' 
	FROM [dbo].[ZnodeImportTemplateMapping]
	WHERE [ImportTemplateId]= @TemplateId FOR XML PATH ('')),2,4000)+' )'
	 		
	EXEC ( @CreateTableScriptSql )
	
	 SET @Sql = '
	SELECT @InsertColumnName = COALESCE(@InsertColumnName + '', '', '''') + ''[''+ ITCD.BaseImportColumn +'']'' ,
		   @ImportTableColumnName = COALESCE(@ImportTableColumnName + '', '', '''') +''[''+ImportTableColumnName+'']''
	FROM [ZnodeImportTableColumnDetail] ITCD 
	INNER JOIN [ZnodeImportTableDetail] ITD ON ITCD.ImportTableId = ITD.ImportTableId
	WHERE  ITD.ImportTableName = @TableName  AND ITCD.BaseImportColumn is not null '
	
	EXEC sp_executesql @SQL, N'@TableName Nvarchar(200), @InsertColumnName NVARCHAR(MAX) OUTPUT, @ImportTableColumnName  NVARCHAR(MAX) OUTPUT', @TableName = @TableName,  @InsertColumnName = @InsertColumnName OUTPUT, @ImportTableColumnName = @ImportTableColumnName OUTPUT

    IF( LEN(@InsertColumnName) > 0 )
	BEGIN
	
		SET @SQL = 
		'INSERT INTO tempdb..[##AttributeDetail] ( '+@InsertColumnName+' )
		 SELECT '+ @ImportTableColumnName +' 
		 FROM '+ @GlobalTemporaryTable+    ' PRD '
		EXEC sp_executesql @SQL

		SET @SQL = 'update tempdb..##AttributeDetail SET AttributeCode= AttributeName, GUID= '''+@NewuGuId  + ''''
		EXEC sp_executesql @SQL
		
	END
	EXEC Znode_ImportData @TableName = 'tempdb..[##AttributeDetail]',	@NewGUID = @TblGUID ,@TemplateId = @TemplateId,
	     @UserId = 2,@PortalId = @PortalId,@LocaleId = @LocaleId,@DefaultFamilyId = 0,@PriceListId = 0, @CountryCode = '' ,
		 @ERPTaskSchedulerId  = @ERPTaskSchedulerId 

	select 'Job started successfully.' 
END