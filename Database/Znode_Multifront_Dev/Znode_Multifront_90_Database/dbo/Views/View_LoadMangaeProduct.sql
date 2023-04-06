Create View View_LoadMangaeProduct
AS 
SELECT a.PimProductId , c.AttributeCode , b.AttributeValue , b.LocaleId  
FROM ZnodePimAttributeValue a 
INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
INNER JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )