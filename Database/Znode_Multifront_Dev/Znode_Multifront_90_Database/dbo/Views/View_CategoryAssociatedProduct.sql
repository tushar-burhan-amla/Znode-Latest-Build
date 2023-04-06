--SELECT * FROM [View_CategoryAssociatedProduct]

CREATE View [dbo].[View_CategoryAssociatedProduct]
AS 

With Attribute_values AS (
SELECT a.PimProductId,CAST(NULL AS INT )PimCategoryProductId,PimCategoryId,CAST(null AS INT ) DisplayOrder,CAST(null AS BIT ) Status,b.AttributeValue , c.AttributeCode
FROM ZnodePimCategoryProduct a 
INNER JOIN View_ZnodePimAttributeValue b ON (b.PimProductId = a.PimProductId)
INNER JOIN View_PimAttributeLocale c ON (c.PimAttributeId = b.PimAttributeId)
WHERE c.AttributeCode IN ('ProductName','ImagePath')
)

SELECT * 
FROM Attribute_values a 
PIVOT 
(

Max(AttributeValue) FOR AttributeCode IN (ProductName,ImagePath)
) Piv