


   CREATE View [dbo].[View_PimAttributeFamilyLocale] AS
   SELECT a.PimAttributeFamilyId
,a.FamilyCode
,a.IsSystemDefined
,a.IsDefaultFamily
,a.IsCategory
,a.CreatedBy
,CONVERT(DATE,a.CreatedDate)CreatedDate
,a.ModifiedBy
,CONVERt(DATE,a.ModifiedDate)ModifiedDate,b.LocaleId,b.AttributeFamilyName
   FROM ZnodePimAttributeFamily a 
   INNER JOIN ZnodePimFamilyLocale b  ON (a.PimAttributeFamilyId = b.PimAttributeFamilyId)

   WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper sews WHERE sews.PimAttributeFamilyId = a.PimAttributeFamilyId)