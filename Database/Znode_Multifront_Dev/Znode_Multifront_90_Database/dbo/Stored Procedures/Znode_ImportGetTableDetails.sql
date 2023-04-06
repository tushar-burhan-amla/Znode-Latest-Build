
--Znode_ImportGetTableDeatils 'MAGSHIP',7  
--Znode_ImportGetTableDeatils 'MAGSOLD',6  

CREATE PROC [dbo].[Znode_ImportGetTableDetails]  
(  
 @TableName Nvarchar(200),  
 @ImportHeadId int  
)  
AS  
BEGIN  
  
 SET NOCOUNT ON;  
  
 DECLARE @ImportTableColumnName NVARCHAR(2000) = '',  
   @SQL NVARCHAR(MAX) = '';  
  
 SET @SQL =   
  'SELECT @ImportTableColumnNameS = SUBSTRING ((Select '','' +  ''[''+ISNULL(ImportTableColumnName ,''NULL'')+'']'' FROM ZnodeImportTableDetail ITD   
   INNER JOIN ZnodeImportTableColumnDetail ITCD ON ITD.ImportTableId = ITCD.ImportTableId  
   WHERE ITD.ImportTableName = @TableName AND ITD.ImportHeadId = @ImportHeadId Order by ColumnSequence FOR XML PATH ('''') ),2,4000) '  
  
 print @SQL  
 EXEC sp_executesql @SQL, N'@TableName Nvarchar(200), @ImportHeadId INT, @ImportTableColumnNameS NVARCHAR(MAX) OUTPUT ', @TableName = @TableName, @ImportHeadId = @ImportHeadId,  @ImportTableColumnNameS = @ImportTableColumnName OUTPUT  
  
 SELECT @ImportTableColumnName AS ImportTableColumnName  
  
END