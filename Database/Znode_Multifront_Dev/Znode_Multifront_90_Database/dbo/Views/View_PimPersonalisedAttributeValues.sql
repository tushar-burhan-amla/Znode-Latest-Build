



   CREATE View [dbo].[View_PimPersonalisedAttributeValues]
   AS
SELECT b.PimProductId, qq.PimAttributeFamilyId,c.PimAttributeId,w.PimAttributeGroupId,c.AttributeTypeId,Q.AttributeTypeName,c.AttributeCode,c.IsRequired,c.IsLocalizable,c.IsFilterable,
			c.AttributeName ,  b.AttributeValue, b.PimAttributeValueId,	h.PimAttributeDefaultValueId,h.AttributeDefaultValueCode,h.AttributeDefaultValue,ISNULL(NULL,0) RowId,ISNULL(h.IsEditable,1) AS IsEditable
			,i.ControlName, i.Name ValidationName,j.ValidationName SubValidationName,j.RegExp,k.Name ValidationValue,CAST (CASE WHEN  j.RegExp IS NULL THEN  0 ELSE 1  END AS BIT ) IsRegExp,c.HelpDescription

		FROM [dbo].[View_PimAttributeLocale] c 
		--INNER JOIN @PimAttributeId gh ON (gh.PimAttributeId = c.PimAttributeId AND c.LocaleId = @LocaleId AND c.IsPersonalizable =1  )
		LEFT JOIN dbo.ZnodePimFamilyGroupMapper w ON ( w.PimAttributeId= c.PimAttributeId )--AND EXISTS (SELECT TOP 1 1 FROM @PIMfamilyIds sd WHERE sd.PimAttributeFamilyId = w.PimAttributeFamilyId) )
		LEFT JOIN dbo.ZnodePimAttributeFamily qq ON (qq.PimAttributeFamilyId = w.PimAttributeFamilyId )
		LEFT OUTER JOIN [dbo].ZnodeAttributeType q ON (c.AttributeTypeId = Q.AttributeTypeId)
		LEFT JOIN View_PimProductAttributeValueLocale b ON ( b.PimAttributeId = c.PimAttributeId  AND b.LocaleId = c.LocaleId)
		LEFT JOIN View_PimDefaultValue h ON (h.PimAttributeDefaultValueId = b.PimAttributeDefaultValueId OR h.PimAttributeId = c.PimAttributeId AND h.LocaleId=c.LocaleId )
		LEFT  OUTER JOIN [dbo].ZnodePimAttributeValidation k  ON (k.PimAttributeId = c.PimAttributeId  )
		LEFT  OUTER JOIN [dbo].ZnodeAttributeInputValidation i ON (k.InputValidationId = i.InputValidationId )
		LEFT  OUTER JOIN [dbo].ZnodeAttributeInputValidationRule j ON (k.InputValidationRuleId = j.InputValidationRuleId )