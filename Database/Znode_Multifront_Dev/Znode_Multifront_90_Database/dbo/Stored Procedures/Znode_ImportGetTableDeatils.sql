--Znode_ImportGetTableDeatils 'PRDHA'

CREATE PROC Znode_ImportGetTableDeatils
(
	@TableName Nvarchar(200)
)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @ImportTableColumnName NVARCHAR(2000) = '',
			@SQL NVARCHAR(MAX) = '';

	SET @SQL = 
		'SELECT @ImportTableColumnNameS = SUBSTRING ((Select '','' +  ''[''+ISNULL(ImportTableColumnName ,''NULL'')+'']'' FROM ZnodeImportTableDetail ITD 
		 INNER JOIN ZnodeImportTableColumnDetail ITCD ON ITD.ImportTableId = ITCD.ImportTableId
		 WHERE ImportTableName = @TableName FOR XML PATH ('''')),2,4000)'

	--print @SQL
	EXEC sp_executesql @SQL, N'@TableName Nvarchar(200), @ImportTableColumnNameS NVARCHAR(MAX) OUTPUT ', @TableName = @TableName,  @ImportTableColumnNameS = @ImportTableColumnName OUTPUT

	SELECT @ImportTableColumnName AS ImportTableColumnName

END