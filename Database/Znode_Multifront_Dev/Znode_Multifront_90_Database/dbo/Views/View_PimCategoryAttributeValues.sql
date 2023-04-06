


  CREATE View [dbo].[View_PimCategoryAttributeValues]
   AS
   SELECT h.PimAttributeFamilyId, h.FamilyCode,
	a.PimAttributeId,f.PimAttributeGroupId,a.AttributeTypeId,j.AttributeTypeName,a.AttributeCode,a.IsRequired,a.IsLocalizable,a.IsFilterable,
	a.AttributeName ,  i.CategoryValue AttributeValue, i.PimCategoryAttributeValueId,
	b.PimAttributeDefaultValueId,b.AttributeDefaultValueCode,b.AttributeDefaultValue,ISNULL(NULL,0) RowId,ISNULL(b.IsEditable,1) AS IsEditable
	,d.ControlName, d.Name ValidationName,e.ValidationName SubValidationName,e.RegExp,c.Name ValidationValue,CAST (CASE WHEN  e.RegExp IS NULL THEN  0 ELSE 1  END AS BIT )  IsRegExp ,a.HelpDescription
   FROM View_PimAttributeLocale a 
   lEFT JOIN ZnodeAttributeType j ON (a.AttributeTypeId = j.AttributeTypeId)
   LEFT JOIN View_PimDefaultValue b ON (a.PimAttributeId = b.PimAttributeId)
   LEFT JOIN ZnodePimAttributeValidation c ON (a.PimAttributeId = c.PimAttributeId) 
   LEFT JOIN ZnodeAttributeInputValidation d ON (c.InputValidationId=d.InputValidationId)
   LEFT JOIN ZnodeAttributeInputValidationRule e ON (c.InputValidationRuleId=e.InputValidationRuleId And e.InputValidationId= c.InputValidationId)  
   LEFT JOIN ZnodePimAttributeGroupMapper f ON ( a.PimAttributeId = f.PimAttributeId )
   LEFT JOIN ZnodePimFamilyGroupMapper g ON (g.PimAttributeGroupId=f.PimAttributeGroupId OR g.PimAttributeId = a.PimAttributeId)
   LEFT JOIN View_PimAttributeFamilyLocale h ON (h.PimAttributeFamilyid = g.PimAttributeFamilyId)
   LEFT JOIN View_PimCategoryAttributeValueLocale i ON ( i.PimAttributeFamilyId = h.PimAttributeFamilyId and i.PimAttributeId = a.PimAttributeId)