

CREATE View [dbo].[View_LoadCategoryProductList]
AS 
SELECT a.PimProductId , c.AttributeCode , b.AttributeValue , b.LocaleId , pcc.PimCategoryId , Case when pcc.PimProductId Is NULl THEN 0 ELSE 1 END IsAssociatedProduct
FROM ZnodePimAttributeValue a 
INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
INNER JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId ) 
LEFT JOIN ZnodePimCategoryProduct pcc ON (pcc.PimProductId = a.PimProductId )

UNION ALL

SELECT a.PimProductId, b.AttributeCode,ZPPATAV.AttributeValue,ZPPATAV.LocaleId,pcc.PimCategoryId,Case when pcc.PimProductId Is NULl THEN 0 ELSE 1 END IsAssociatedProduct
FROM  ZnodePimProductAttributeTextAreaValue ZPPATAV
INNER JOIN ZnodePimAttributeValue a ON (a.PimAttributeValueId= ZPPATAV.PimAttributeValueId)
INNER JOIN ZnodePimAttribute b ON (a.PimAttributeId= b.PimAttributeId)
LEFT JOIN ZnodePimCategoryProduct pcc ON (pcc.PimProductId = a.PimProductId)

UNION ALL

SELECT a.PimProductId,b.AttributeCode,ZPPAM.MediaPath,ZPPAM.LocaleId,pcc.PimCategoryId,Case when pcc.PimProductId Is NULl THEN 0 ELSE 1 END IsAssociatedProduct
FROM ZnodePimProductAttributeMedia ZPPAM 
INNER JOIN ZnodePimAttributeValue a ON (a.PimAttributeValueId= ZPPAM.PimAttributeValueId)
INNER JOIN ZnodePimAttribute b ON (a.PimAttributeId= b.PimAttributeId)
LEFT JOIN ZnodePimCategoryProduct pcc ON (pcc.PimProductId = a.PimProductId)

UNION ALL

SELECT a.PimProductId,b.AttributeCode,ZPADV.AttributeDefaultValueCode,ZPPADV.LocaleId,pcc.PimCategoryId,Case when pcc.PimProductId Is NULl THEN 0 ELSE 1 END IsAssociatedProduct
FROM ZnodePimProductAttributeDefaultValue ZPPADV
INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON (ZPPADV.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId)
INNER JOIN ZnodePimAttributeValue a ON (a.PimAttributeValueId= ZPPADV.PimAttributeValueId)
INNER JOIN ZnodePimAttribute b ON (a.PimAttributeId= b.PimAttributeId)
LEFT JOIN ZnodePimCategoryProduct pcc ON (pcc.PimProductId = a.PimProductId)
GO
