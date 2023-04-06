CREATE View View_TextAreaAttributeValue 
WITH SCHEMABINDING
AS 
with Cte_DataWork 
AS
(
  SELECT ZPAV.PimProductId , ZPA.AttributeCode , ZPAVL.AttributeValue,ZPAVL.LocaleId
  FROM dbo.ZnodePimAttribute ZPA 
  INNER JOIN dbo.ZnodePimAttributeValue ZPAV ON (ZPA.PimAttributeId =ZPAV.PimAttributeId   )
  INNER JOIN dbo.ZnodePimProductAttributeTextAreaValue ZPAVL ON (ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId)
  WHERE ZPA.AttributeCode IN ('') 
)
SELECT PimProductId
FROM Cte_DataWork A 
PIVOT (
max(AttributeValue) FOR AttributeCode IN ( [a] )
) PIV