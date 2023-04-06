CREATE VIEW [dbo].[View_ManageProductList]
AS
     WITH AttributeValuePvot
          AS (SELECT zpav.PimProductId,
                     zpavl.AttributeValue AttributeValue,
                     zpa.AttributeCode,
                     ISNULL(zpafl.AttributeFamilyName, '') AttributeFamilyName,
                     zpaL.LocaleId
              FROM ZnodePimAttribute zpa
                   INNER JOIN ZnodePimAttributeLocale zpal ON(zpa.PimAttributeId = zpal.PimAttributeId)
                   INNER JOIN ZnodePimAttributeValue zpav ON(zpa.PimAttributeId = zpav.PimAttributeId)
                   INNER JOIN ZnodePimAttributeValueLocale zpavl ON(zpavl.PimAttributeValueId = zpav.PimAttributeValueId
                                                                    AND zpal.LOcaleid = zpavl.LOcaleid)
                   LEFT JOIN ZnodePimAttributeFamily zpaf ON((EXISTS
                                                             (
                                                                 SELECT DISTINCT
                                                                        PimAttributeFamilyId
                                                                 FROM ZnodePimAttributeValue qwq
                                                                 WHERE qwq.PimProductId = zpav.PimProductId
                                                                       AND qwq.PimAttributeFamilyId = zpaf.PimAttributeFamilyId
                                                             )
                                                              OR EXISTS
                                                             (
                                                                 SELECT TOP 1 1
                                                                 FROM ZnodePimConfigureProductAttribute zpc
                                                                 WHERE zpc.PimProductId = zpav.PimProductId
                                                                       AND zpc.PimFamilyId = zpaf.PimAttributeFamilyId
                                                             ))
                                                             AND zpaf.IsDefaultFamily <> 1)
                   LEFT JOIN ZnodePimFamilyLocale zpafl ON(zpafl.PimAttributeFamilyId = zpaf.PimAttributeFamilyId
                                                           AND zpal.LOcaleid = zpafl.LOcaleid)
              WHERE zpa.AttributeCode IN('ProductName', 'SKU', 'Price', 'Quantity', 'IsActive', 'ProductType', 'Image', 'Assortment'))
          SELECT DISTINCT
                 zpp.PimProductid ProductId,
                 Piv.[ProductName],
                 Piv.ProductType,
                 piv.AttributeFamilyName AttributeFamily,
                 Piv.[SKU],
                 [Price],
                 [Quantity],
                 CASE
                     WHEN Piv.[IsActive] IS NULL
                     THEN CAST(0 AS BIT)
                     ELSE CAST(Piv.[IsActive] AS BIT)
                 END [IsActive],
                 [Image] ImagePath,
                 Piv.[Assortment],
                 Piv.LocaleId
          FROM ZNodePimProduct zpp
               INNER JOIN AttributeValuePvot PIVOT(MAX(AttributeValue) FOR AttributeCode IN([ProductName],
                                                                                            [SKU],
                                                                                            [Price],
                                                                                            [Quantity],
                                                                                            [IsActive],
                                                                                            [ProductType],
                                                                                            [Image],
                                                                                            [Assortment])) Piv ON(Piv.PimProductId = zpp.PimProductid);