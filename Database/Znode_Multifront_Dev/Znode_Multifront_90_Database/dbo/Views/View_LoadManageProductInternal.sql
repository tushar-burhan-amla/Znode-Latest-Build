

CREATE  View [dbo].[View_LoadManageProductInternal]
AS 

SELECT a.PimProductId ,CAST((SELECT ''+ltrim(rtrim((b.AttributeValue))) FOR XML PATH(''))  AS NVARCHAR(max)) AttributeValue , b.LocaleId  ,a.PimAttributeId,c.AttributeCode ,b.ZnodePimAttributeValueLocaleId
FROM ZnodePimAttributeValue a 
INNER JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )
INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )

UNION ALL

SELECT a.PimProductId,CAST((SELECT ''+ltrim(rtrim((ZPPATAV.AttributeValue))) FOR XML PATH(''),TYPE) AS NVARCHAR(max)) AS     AttributeValue  

,ZPPATAV.LocaleId,a.PimAttributeId,c.AttributeCode  ,ZPPATAV.PimProductAttributeTextAreaValueId
FROM ZnodePimAttributeValue a 
INNER JOIN ZnodePimProductAttributeTextAreaValue ZPPATAV ON (ZPPATAV.PimAttributeValueId = a.PimAttributeValueId )
INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )