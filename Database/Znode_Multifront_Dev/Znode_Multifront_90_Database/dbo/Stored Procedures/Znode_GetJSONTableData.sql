/*
EXEC Znode_GetJSONTableData ''

*/
CREATE PROCEDURE [dbo].[Znode_GetJSONTableData]
(
 @JSONString NVARCHAR(max),
 @JsonColumn VARCHAR(max) 
)
AS 
BEGIN 
BEGIN TRY 
SET NOCOUNT ON 
   DECLARE @SQL NVARCHAR(max)= ''
   DECLARE @ColumnJSON NVARCHAr(max)
    DECLARE @ColumnJSONWhere NVARCHAr(max)
   SET @ColumnJSON = SUBSTRING((
   SELECT ','+item+' NVARCHAR(max)'+'''$.'+Item+'''' 
   FROM dbo.Split(@JsonColumn,',') SP 
   FOR XML PATH('')),2,4000)

   SET @ColumnJSONWhere = SUBSTRING((
   SELECT ' OR '+item+' IS NOT NULL'
   FROM dbo.Split(@JsonColumn,',') SP 
   FOR XML PATH('')),4,4000)

   SET @SQL = '
   SELECT '+@JsonColumn+'
   FROM OPENJSON('''+@JSONString+''')
   WITH (
    '+@ColumnJSON+'
       )
   
   WHERE 1=1 AND ('+@ColumnJSONWhere+' )
     ' 

PRINT  @SQL 
 EXEC (@SQL)


END TRY 

BEGIN CATCH 
 SELECT ERROR_MESSAGE()
END CATCH 


END