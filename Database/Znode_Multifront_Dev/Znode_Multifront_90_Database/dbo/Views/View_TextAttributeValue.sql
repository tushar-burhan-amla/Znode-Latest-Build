
CREATE  VIEW View_TextAttributeValue
WITH SCHEMABINDING
AS 
with Cte_DataWork 
AS
(
  SELECT ZPAV.PimProductId , ZPA.AttributeCode , ZPAVL.AttributeValue,ZPAVL.LocaleId
  FROM dbo.ZnodePimAttribute ZPA 
  INNER JOIN dbo.ZnodePimAttributeValue ZPAV ON (ZPA.PimAttributeId =ZPAV.PimAttributeId   )
  INNER JOIN dbo.ZnodePimAttributeValueLocale ZPAVL ON (ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId)
  WHERE ZPA.AttributeCode IN ('ProductName' ,'SKU' ,'ProductCode','Weight') 
)
SELECT PimProductId,[ProductName],[SKU] ,[ProductCode],[Weight],[LocaleId]
FROM Cte_DataWork A 
PIVOT (
max(AttributeValue) FOR AttributeCode IN ([ProductName] ,[SKU],[ProductCode],[Weight] )
) PIV