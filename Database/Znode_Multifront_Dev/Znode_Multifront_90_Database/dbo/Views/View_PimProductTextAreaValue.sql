CREATE View View_PimProductTextAreaValue 
With SchemaBinding 
AS
SELECT PimProductId ,AttributeCode , ZPAVL.AttributeValue ,ZPAVL.LocaleId
FROM dbo.ZnodePimAttribute ZPA
INNER JOIN dbo.ZnodePimAttributeValue ZPAV ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
INNER JOIN dbo.ZnodePimProductAttributeTextAreaValue ZPAVL ON (ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
INNER JOIN dbo.ZnodeAttributeType ZTA ON (ZTA.AttributeTypeId = ZPA.AttributeTypeId)
WHERE ZPA.IsCategory =0 
AND IsShowOnGrid =1 
AND AttributeTypeName IN ('Text Area')