CREATE VIEW [dbo].[View_PimCategoryAttributeValue]
AS
     SELECT ZPAV.PimCategoryId,
            ZPA.AttributeCode,
            ZPAVL.CategoryValue,
            ZPAVL.LocaleId
     FROM ZnodePimCategoryAttributeValueLocale ZPAVL
          LEFT JOIN ZnodePimCategoryAttributeValue ZPAV ON(ZPAVL.PimCategoryAttributeValueId = ZPAV.PimCategoryAttributeValueId)
          LEFT JOIN ZnodePimAttribute ZPA ON(ZPAV.PimAttributeId = ZPA.PimAttributeId);