CREATE PROC [dbo].[Znode_UpdateShowOnGridColumInternal]  
(  
@PimAttributeId INT = 0   
)  
AS   
BEGIN   
BEGIN TRY   
SET NOCOUNT ON   
DECLARE @sql NVARCHAr(max) = ''  
  
  
  SELECT a.PimProductId , b.AttributeValue , c.AttributeCode    
INTO #Temp_value   
FROM ZnodePimAttributeValue a   
INNER JOIN ZnodePimAttributeValueLocale b ON (b.PimAttributeValueId = a.PimAttributeValueId )  
INNER JOIN ZnodePimAttribute c ON (c.PimAttributeId = a.PimAttributeId)  
WHERE LocaleId = 1   
AND c.PimAttributeId = @PimAttributeId  
AND AttributeCode IN (SELECT b.AttributeCode   
FROM INFORMATION_SCHEMA.COLUMNS a   
INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )  
WHERE TABLE_NAME = 'ZnodePimProduct'  
AND IsCategory = 0   
)  
UNION ALL   
  
SELECT a.PimProductId , b.AttributeValue , c.AttributeCode    
FROM ZnodePimAttributeValue a   
INNER JOIN ZnodePimProductAttributeTextAreaValue b ON (b.PimAttributeValueId = a.PimAttributeValueId )  
INNER JOIN ZnodePimAttribute c ON (c.PimAttributeId = a.PimAttributeId)  
WHERE LocaleId = 1   
AND c.PimAttributeId = @PimAttributeId  
AND AttributeCode IN (SELECT b.AttributeCode   
FROM INFORMATION_SCHEMA.COLUMNS a   
INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )  
WHERE TABLE_NAME = 'ZnodePimProduct'  
AND IsCategory = 0   
 )  
UNION ALL   
SELECT a.PimProductId , mp.AttributeDefaultValueCode , c.AttributeCode    
FROM ZnodePimAttributeValue a   
INNER JOIN ZnodePimProductAttributeDefaultValue b ON (b.PimAttributeValueId = a.PimAttributeValueId )  
INNER JOIN ZnodePimAttributeDefaultValue mp ON (mp.PimAttributeDefaultValueId = b.PimAttributeDefaultValueId)
INNER JOIN ZnodePimAttributeDefaultValueLocale g ON (g.PimAttributeDefaultValueId = b.PimAttributeDefaultValueId)  
INNER JOIN ZnodePimAttribute c ON (c.PimAttributeId = a.PimAttributeId)  
WHERE b.LocaleId = 1    
AND c.PimAttributeId = @PimAttributeId  
AND g.LocaleId = 1   
AND AttributeCode IN (SELECT b.AttributeCode   
FROM INFORMATION_SCHEMA.COLUMNS a   
INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )  
WHERE TABLE_NAME = 'ZnodePimProduct'  
AND IsCategory = 0   
 )  
UNION ALL   
SELECT a.PimProductId ,CAST( b.MediaId AS VARCHAR(500)) , c.AttributeCode    
FROM ZnodePimAttributeValue a   
INNER JOIN ZnodePimProductAttributeMedia b ON (b.PimAttributeValueId = a.PimAttributeValueId )  
INNER JOIN ZnodePimAttribute c ON (c.PimAttributeId = a.PimAttributeId)  
WHERE b.LocaleId = 1   
AND c.PimAttributeId = @PimAttributeId   
AND AttributeCode IN (SELECT b.AttributeCode   
FROM INFORMATION_SCHEMA.COLUMNS a   
INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )  
WHERE TABLE_NAME = 'ZnodePimProduct'  
AND IsCategory = 0   
 )  
  
 SET @sql = ''   
  
--SELECT * FROM #Temp_value WHERE AttributeCode = 'IsActive' AND AttributeValue = 'Yes'  
--UPDATE #Temp_value SET attributeValue = 'true' WHERE AttributeCode = 'IsActive' AND AttributeValue = 'Yes'  
  
  
DECLARE @AttributeCodeAtt VARCHAR(600) , @PimAttributeIdAttr int   
  
DECLARE Cur_AttributeDataUpdate CURSOR FOR   
  
  
  
SELECT b.AttributeCode , PimAttributeId   
FROM INFORMATION_SCHEMA.COLUMNS a   
INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )  
WHERE TABLE_NAME = 'ZnodePimProduct'  
AND IsCategory = 0   
  
  
  
OPEN Cur_AttributeDataUpdate   
FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr   
WHILE @@FETCH_STATUS = 0   
BEGIN   
  
 SET @sql = 'UPDATE a    
 SET '+@AttributeCodeAtt+'= AttributeValue   
 FROM ZnodePimProduct a   
 INNER JOIN #Temp_value m ON(m.PimProductId = a.pimProductId )   
 WHERE m.AttributeCode = '''+@AttributeCodeAtt+'''  
 '   
  
 EXEC (@sql)  
  
FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr   
END   
CLOSE Cur_AttributeDataUpdate  
DEALLOCATE Cur_AttributeDataUpdate   
  
  
  
END TRY   
BEGIN CATCH   
SELECT ERROR_MESSAGE()  
END CATCH   
END