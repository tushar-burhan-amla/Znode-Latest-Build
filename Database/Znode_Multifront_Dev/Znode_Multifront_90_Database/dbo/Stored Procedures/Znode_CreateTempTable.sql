CREATE PROCEDURE [dbo].[Znode_CreateTempTable]
(
  @TableName NVARCHAR(150),
  @ColumnList NVARCHAR(MAX)
)
AS
BEGIN
DECLARE @SQLString NVARCHAR(MAX)
iF @TableName <> '' AND @ColumnList <> ''
SET @SQLString = 'CREATE TABLE '+@TableName + '  '+@ColumnList+' '

EXEC (@SQLString)

END