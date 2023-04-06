CREATE PROCEDURE [dbo].[Znode_ImportProcessCustomerData](@TblGUID nvarchar(255))
AS
BEGIN
	
	SET NOCOUNT ON;
	Declare @NewuGuId nvarchar(255)
	SET @NewuGuId = newid()
	DECLARE @GlobalTemporaryTable nvarchar(255)

	IF OBJECT_ID('tempdb.dbo.##CustomerDetail', 'U') IS NOT NULL 
		DROP TABLE ##CustomerDetail
	
	DECLARE @CreateTableScriptSql NVARCHAR(MAX) = '', 
			@TableName NVARCHAR(200) = 'CustomerRawData',	
		    @InsertColumnName   NVARCHAR(MAX), 
			@ImportTableColumnName NVARCHAR(MAX),
			@Sql NVARCHAR(MAX) = ''
	SET @GlobalTemporaryTable = 'tempdb..[##' + @TableName + '_' + @TblGUID + ']' 
	SELECT @CreateTableScriptSql = 'CREATE TABLE ##CustomerDetail (GUID nvarchar(255),'+SUBSTRING ((Select ',' +  ISNULL([TargetColumnName] ,'NULL')+ ' nvarchar(max)' 
	FROM [dbo].[ZnodeImportTemplateMapping]
	WHERE [ImportTemplateId]= 13 FOR XML PATH ('')),2,4000)+' )'
	 
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
		'INSERT INTO ##CustomerDetail ( '+@InsertColumnName+' )
		SELECT '+@ImportTableColumnName +' 
		FROM '+@GlobalTemporaryTable+ ' PRD '
		EXEC sp_executesql @SQL

		SET @SQL = 'update ##CustomerDetail SET AttributeCode= AttributeName, GUID= '''+@NewuGuId  + ''''
		EXEC sp_executesql @SQL

	END
		 
	SELECT * FROM ##CustomerDetail
END