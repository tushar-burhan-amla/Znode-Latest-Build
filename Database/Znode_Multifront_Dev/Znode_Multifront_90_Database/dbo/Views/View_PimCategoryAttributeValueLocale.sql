
 CREATE View [dbo].[View_PimCategoryAttributeValueLocale] As 
   SELECT a.PimCategoryAttributeValueId
,a.PimCategoryId
,a.PimAttributeFamilyId
,a.PimAttributeId
,a.PimAttributeDefaultValueId
,a.CreatedBy
,CONVERT(DATE,a.CreatedDate)CreatedDate
,a.ModifiedBy
,CONVERt(DATE,a.ModifiedDate)ModifiedDate,b.CategoryValue,b.LocaleId FROM ZnodePimCategoryAttributeValue a
   INNER JOIN ZnodePimCategoryAttributeValueLocale b ON (a.PimCategoryAttributeValueId = b.PimCategoryAttributeValueId)