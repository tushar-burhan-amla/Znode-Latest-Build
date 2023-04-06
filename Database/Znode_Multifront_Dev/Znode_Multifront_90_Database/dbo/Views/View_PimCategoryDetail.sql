


CREATE  View [dbo].[View_PimCategoryDetail]
AS
WITH CategoryFamilty AS( 

SELECT a.PimCategoryId ,a.PimAttributeFamilyId , zpafl.AttributeFamilyName , zpafl.LocaleId 
FROM ZnodePimCategoryAttributeValue a 
    INNER JOIN  ZnodePimAttributeFamily zpaf ON (a.PimAttributeFamilyId=zpaf.PimAttributeFamilyId  AND IsDefaultFamily <> 1 )
    INNER JOIN  ZnodePimFamilyGroupMapper zpfgm ON  zpaf.PimAttributeFamilyId   = zpfgm.PimAttributeFamilyId
    INNER JOIN ZnodePimFamilyLocale zpafl ON (zpafl.PimAttributeFamilyId=zpaf.PimAttributeFamilyId )  )
    , AttributeValuePvot AS (

    SELECT zpav.PimCategoryId,  CASE WHEN  zat.AttributeTypeName IN ('Image','Audio','Video','File','GalleryImages')
		THEN dbo.FN_GetThumbnailMediaPath(zpavl.CategoryValue,0) ELSE zpavl.CategoryValue END  CategoryValue 
		, zpa.AttributeCode,zpafl.AttributeFamilyName,zpal.LocaleId
		  FROM ZnodePimAttribute zpa INNER JOIN
		  ZnodePimAttributeLocale zpal ON (zpa.PimAttributeId = zpal.PimAttributeId) INNER JOIN
		  ZnodePimCategoryAttributeValue zpav ON (zpa.PimAttributeId = zpav.PimAttributeId) INNER JOIN
		  ZnodePimCategoryAttributeValueLocale zpavl ON (zpavl.PimCategoryAttributeValueId = zpav.PimCategoryAttributeValueId AND zpal.LocaleId = zpavl.LocaleId )
		  LEFT JOIN CategoryFamilty zpafl ON ( zpafl.PimCategoryId = zpav.PimCategoryId AND  zpal.LocaleId = zpafl.LocaleId  )
		  INNER JOIN ZnodeAttributeType zat ON (zat.AttributeTypeid = zpa.attributetypeid  )
WHERE        zpa.AttributeCode IN ('CategoryName', 'Status','Image')
)

-- SELECT * FROM ZnodePimCategoryAttributeValue


SELECT zpp.PimCategoryId PimCategoryId, [CategoryName],[Status],[Image] CategoryImage,LocaleId,AttributeFamilyName
FROM ZNodePimCategory zpp 
INNER JOIN  AttributeValuePvot 
PIVOT 
(
 Max(CategoryValue) FOR AttributeCode  IN ([CategoryName],[Status] ,[Image])
)Piv  ON (Piv.PimCategoryId = zpp.PimCategoryId)