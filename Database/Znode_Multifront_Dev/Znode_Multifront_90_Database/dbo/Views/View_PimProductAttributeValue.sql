


CREATE View [dbo].[View_PimProductAttributeValue]
AS 
SELECT ISNULL(NULL,0) RowId,a.PimProductId , c.AttributeCode , b.AttributeValue , b.LocaleId  
FROM ZnodePimAttributeValue a 
LEFT JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
LEFT JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )