CREATE View [dbo].[View_LoadManageProduct]
AS 
SELECT a.PimProductId , c.AttributeCode , b.AttributeValue , b.LocaleId  
FROM ZnodePimAttributeValue a 
LEFT JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
LEFT JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )

UNION ALL

SELECT a.PimProductId,c.AttributeCode,ZPPATAV.AttributeValue,ZPPATAV.LocaleId 
FROM ZnodePimProductAttributeTextAreaValue ZPPATAV
LEFT JOIN ZnodePimAttributeValue a ON (a.PimAttributeValueId = ZPPATAV.PimAttributeValueId )
LEFT JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
--LEFT JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )

UNION ALL

SELECT a.PimProductId,c.AttributeCode,ZPPAM.MediaPath,ZPPAM.LocaleId
FROM ZnodePimProductAttributeMedia ZPPAM
LEFT JOIN ZnodePimAttributeValue a ON (a.PimAttributeValueId = ZPPAM.PimAttributeValueId )
LEFT JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
--LEFT JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )

UNION ALL

SELECT a.PimProductId,c.AttributeCode,ZPADV.AttributeDefaultValueCode,ZPPADV.LocaleId
FROM ZnodePimProductAttributeDefaultValue ZPPADV
INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON (ZPPADV.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId)
LEFT JOIN ZnodePimAttributeValue a ON (a.PimAttributeValueId = ZPPADV.PimAttributeValueId )
LEFT JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
--LEFT JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )