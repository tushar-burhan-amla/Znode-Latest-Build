-- SELECT * FROM View_PimAttributeLocale
CREATE VIEW [dbo].[View_CategoryUnAssociatedProduct]
AS
     WITH Attribute_values
          AS (SELECT NULL PimCategoryProductId,
                     NULL PimCategoryId,
                     b.PimProductId,
                     ISNULL(NULL, 1) DisplayOrder,
                     ISNULL(NULL, 1) Status,
                     b.ProductName AttributeValue,
                     c.AttributeCode,
                     b.LocaleId
              FROM View_ZnodePimAttributeValue b
                   INNER JOIN View_PimAttributeLocale c ON(c.PimAttributeId = b.PimAttributeId)
              WHERE c.AttributeCode IN('ProductName', 'Image'))
          SELECT PimCategoryProductId,
                 PimCategoryId,
                 PimProductId,
                 DisplayOrder,
                 [Status],
                 ProductName,
                 [Image] ImagePath,
                 LocaleId
          FROM Attribute_values a PIVOT(MAX(AttributeValue) FOR AttributeCode IN(ProductName,
                                                                                 Image)) Piv;

-- SELECT * FROM View_ZnodePimAttributeValue WHERE PimProductId = 2