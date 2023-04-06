CREATE View View_ProductAttributeValue
WITH SCHEMABINDING
AS 
SELECT PimProductId ,CASE WHEN IsProductPublish = 1 THEN   'Published' 
				WHEN IsProductPublish = 0 THEN 'Draft'  
				ELSE 'Not Published' END PublishStatus,ZPFL.AttributeFamilyName AttributeFamily, ZPFL.LocaleId 
FROM dbo.ZnodePimProduct ZPP 
INNER JOIN dbo.ZnodePimAttributeFamily ZPAF ON (ZPAF.PimAttributeFamilyId = ZPP.PimAttributeFamilyId)
INNER JOIN dbo.ZnodePimFamilyLocale ZPFL ON (ZPAF.PimAttributeFamilyId = ZPFL.PimAttributeFamilyId)