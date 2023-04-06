CREATE VIEW [dbo].[View_GetConfigureAttributeDetail]
AS
     SELECT a.PimAttributeFamilyId,
            a.PimAttributeGroupId,
            q.AttributeGroupName,
            b.PimAttributeId,
            b.AttributeName,
            b.AttributeCode,
            w.AttributeTypeId,
            b.IsFilterable,
            b.IsRequired,
            b.IsLocalizable,
            g.IsEditable,
            w.AttributeTypeName,
            e.Name,
            e.ControlName,
            f.ValidationName,
            c.Name SubValidationName,
            f.RegExp,
            g.AttributeDefaultValue,
            CASE
                WHEN hj.PimAttributeId IS NULL
                THEN 0
                ELSE 1
            END IsConfigurableAttribute
     FROM ZnodePimFamilyGroupMapper a
          INNER JOIN ZnodePimAttributeGroupLocale q ON(q.PimAttributeGroupId = a.PimAttributeGroupId
                                                       AND q.LocaleId = 1)
          INNER JOIN View_PimAttributeLocale b ON(b.PimAttributeId = a.PimAttributeId
                                                  AND b.IsConfigurable = 1
                                                  AND b.LocaleId = q.LocaleId)
          INNER JOIN ZnodeAttributeType w ON(w.AttributeTypeId = b.AttributeTypeId
                                             AND w.IsPimAttributeType = 1)
          LEFT JOIN ZnodePimAttributeValidation c ON(c.PimAttributeId = b.PimAttributeId)
          LEFT JOIN ZnodeAttributeInputValidation e ON(e.InputValidationId = c.InputValidationId)
          LEFT JOIN ZnodeAttributeInputValidationRule f ON(f.InputValidationRuleId = c.InputValidationRuleId)
          LEFT JOIN View_PimDefaultValue g ON(b.PimAttributeId = g.PimAttributeId
                                              AND g.LocaleId = b.LocaleId)
          LEFT JOIN ZnodePimConfigureProductAttribute hj ON(hj.PimFamilyId = a.PimAttributeFamilyId
                                                            AND hj.PimAttributeId = b.PimAttributeId);--AND hj.PimProductId = @PimProductId  )