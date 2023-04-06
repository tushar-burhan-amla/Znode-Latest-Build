CREATE VIEW [dbo].[View_PimAttributeValues]
AS
    SELECT qq.PimAttributeFamilyId,
           qq.FamilyCode,
           w.PimAttributeGroupId,
           c.PimAttributeId,
           c.AttributeTypeId,
           q.AttributeTypeName,
           c.AttributeCode,
           c.IsRequired,
           c.IsLocalizable,
           c.IsFilterable,
           f.AttributeName,
           b.AttributeValue,
           b.PimAttributeValueId,
           h.PimAttributeDefaultValueId,
           h.AttributeDefaultValueCode,
           g.AttributeDefaultValue,
           ISNULL(NULL, 0) AS RowId,
           ISNULL(h.IsEditable, 1) AS IsEditable,
           i.ControlName,
           i.Name AS ValidationName,
           j.ValidationName AS SubValidationName,
           j.RegExp,
           k.Name AS ValidationValue,
           CAST(CASE
                    WHEN j.RegExp IS NULL
                    THEN 0
                    ELSE 1
                END AS BIT) AS IsRegExp,
           c.HelpDescription,
		   '' as FilesName,h.IsDefault
    FROM dbo.ZnodePimAttributeFamily AS qq
         INNER JOIN dbo.ZnodePimFamilyGroupMapper AS w ON qq.PimAttributeFamilyId = w.PimAttributeFamilyId
         INNER JOIN dbo.ZnodePimAttribute AS c ON w.PimAttributeId = c.PimAttributeId
         INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
         INNER JOIN dbo.ZnodePimAttributeLocale AS f ON c.PimAttributeId = f.PimAttributeId
         LEFT OUTER JOIN dbo.ZnodePimAttributeValue AS b ON c.PimAttributeId = b.PimAttributeId
                                                            AND b.PimAttributeFamilyId = qq.PimAttributeFamilyId
         LEFT OUTER JOIN dbo.ZnodePimAttributeDefaultValue AS h ON h.PimAttributeDefaultValueId = b.PimAttributeDefaultValueId
                                                                   OR h.PimAttributeId = c.PimAttributeId
         LEFT OUTER JOIN dbo.ZnodePimAttributeDefaultValueLocale AS g ON h.PimAttributeDefaultValueId = g.PimAttributeDefaultValueId
         LEFT OUTER JOIN dbo.ZnodePimAttributeValidation AS k ON k.PimAttributeId = c.PimAttributeId
         LEFT OUTER JOIN dbo.ZnodeAttributeInputValidation AS i ON k.InputValidationId = i.InputValidationId
         LEFT OUTER JOIN dbo.ZnodeAttributeInputValidationRule AS j ON k.InputValidationRuleId = j.InputValidationRuleId;
GO