

  CREATE view [dbo].[View_PimAttributeLocale]
  AS
  SELECT a.PimAttributeId
,a.ParentPimAttributeId
,a.AttributeTypeId
,a.AttributeCode
,a.IsRequired
,a.IsLocalizable
,a.IsFilterable
,a.IsSystemDefined
,a.IsConfigurable
,a.IsPersonalizable
,a.DisplayOrder
,a.HelpDescription
,a.IsCategory
,a.CreatedBy
,CONVERT(DATE,a.CreatedDate)CreatedDate
,a.ModifiedBy
,CONVERt(DATE,a.ModifiedDate)ModifiedDate,b.LocaleId,b.AttributeName
  FROM ZnodePimAttribute a 
  INNER JOIN  ZnodePimAttributeLocale b ON (a.PimAttributeId = b.PimAttributeId)