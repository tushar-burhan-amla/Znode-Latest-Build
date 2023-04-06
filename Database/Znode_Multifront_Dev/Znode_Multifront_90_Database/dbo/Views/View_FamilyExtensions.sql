

CREATE VIEW 
[dbo].[View_FamilyExtensions] AS
  SELECT DISTINCT e.InputValidationRuleId ,e.ValidationName,g.name 
   'Extension' ,
  	(select Name  from ZnodeMediaAttributeValidation where MediaAttributeId= c.MediaAttributeId and InputValidationId
		in (select InputValidationId from ZnodeAttributeInputValidation where  Name = 'MaxFileSize') )
		'MaxFileSize',a.MediaAttributeFamilyId,a.FamilyCode

  FROM ZnodeMediaAttributeFamily a 
  INNER JOIN ZnodeMediaFamilyLocale f ON (a.MediaAttributeFamilyId = f.MediaAttributeFamilyId)-- AND f.LocaleId = @LocaleId)
  INNER JOIN ZnodeMediaFamilyGroupMapper b ON (a.MediaAttributeFamilyId = b.MediaAttributeFamilyId  and  b.IsSystemDefined = 1)
  INNER JOIN ZnodeMediaAttributeGroupMapper c ON (b.MediaAttributeGroupId = c.MediaAttributeGroupId  and  c.IsSystemDefined = 1 )
  INNER JOIN ZnodeMediaAttributeValidation d ON (c.MediaAttributeId = d.MediaAttributeId)
  INNER JOIN ZnodeAttributeInputValidationRule e ON (d.InputValidationRuleId = e.InputValidationRuleId)
  INNER JOIN ZnodeAttributeInputValidation g ON (e.InputValidationId = g.InputValidationId) 
  WHERE a.IsSystemDefined = 1 AND g.Name = 'Extensions'