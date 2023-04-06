CREATE VIEW [dbo].[View_AttributeValidationList]
AS
     SELECT DISTINCT
            a.MediaAttributeId,
            d.AttributeTypeId,
            j.AttributeTypeName,
            d.AttributeCode,
            d.IsRequired,
            d.IsLocalizable,
            d.IsFilterable,
            h.AttributeName,
            b.ControlName,
            b.Name AS ValidationName,
            c.ValidationName AS SubValidationName,
            a.Name AS ValidationValue,
            c.RegExp,
            CAST(CASE
                     WHEN c.RegExp IS NULL
                     THEN 0
                     ELSE 1
                 END AS BIT) AS IsRegExp,
            ISNULL(NULL, 0) AS RowId
     FROM dbo.ZnodeMediaAttribute AS d
          INNER JOIN dbo.ZnodeMediaAttributeLocale AS h ON d.MediaAttributeId = h.MediaAttributeId
          INNER JOIN dbo.ZnodeMediaAttributeGroupMapper AS f ON d.MediaAttributeId = f.MediaAttributeId
          LEFT OUTER JOIN dbo.ZnodeMediaFamilyGroupMapper AS g ON f.MediaAttributeGroupId = g.MediaAttributeGroupId
          LEFT OUTER JOIN dbo.ZnodeAttributeType AS j ON d.AttributeTypeId = j.AttributeTypeId
          LEFT OUTER JOIN dbo.ZnodeMediaAttributeValidation AS a ON d.MediaAttributeId = a.MediaAttributeId
          LEFT OUTER JOIN dbo.ZnodeAttributeInputValidation AS b ON a.InputValidationId = b.InputValidationId
          LEFT OUTER JOIN dbo.ZnodeAttributeInputValidationRule AS c ON a.InputValidationRuleId = c.InputValidationRuleId;

GO
