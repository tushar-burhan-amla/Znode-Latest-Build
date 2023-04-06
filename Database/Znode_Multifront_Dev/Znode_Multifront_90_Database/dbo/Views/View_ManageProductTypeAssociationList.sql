

CREATE  View [dbo].[View_ManageProductTypeAssociationList]
AS
with AttributeValuePvot AS (
SELECT        zpald.PimProductId,  zpavl1.AttributeValue AttributeValue, zpa.AttributeCode,zpafl.AttributeFamilyName,Zpald.PimProductTypeAssociationId, zpav.PimProductId RelatedProductId,zpal.LocaleId
FROM            ZnodePimAttribute zpa INNER JOIN
                         ZnodePimAttributeLocale zpal ON (zpa.PimAttributeId = zpal.PimAttributeId) INNER JOIN
						 ZnodePimAttributeValue zpav ON (zpa.PimAttributeId = zpav.PimAttributeId) INNER JOIN
                         ZnodePimAttributeValueLocale zpavl ON (zpavl.PimAttributeValueId = zpav.PimAttributeValueId AND zpal.LocaleId = zpavl.LocaleId)
						 LEFT JOIN  ZnodePimProductTypeAssociation zpald ON (zpald.PimParentProductId = zpav.PimProductId )
						  Left JOIN  ZnodePimAttributeValue zpav1 ON (zpa.PimAttributeId = zpav1.PimAttributeId AND zpav1.PimProductId = zpald.PimProductId) INNER JOIN
                         ZnodePimAttributeValueLocale zpavl1 ON (zpavl1.PimAttributeValueId = zpav1.PimAttributeValueId AND zpavl.localeid = zpal.localeid)
						 LEFT JOIN ZnodePimAttributeFamily zpaf ON (zpav.PimAttributeFamilyId=zpaf.PimAttributeFamilyId )
						 INNER JOIN ZnodePimFamilyLocale zpafl ON (zpafl.PimAttributeFamilyId=zpaf.PimAttributeFamilyId AND zpal.LocaleId=zpafl.LocaleId )
						 --LEFT JOIN ZnodePimAttributeValueLocale zpavl1 ON (zpa.PimAttributeValueId = zpav.PimAttributeValueId AND zpa.LocaleId = zpavl.LocaleId)
WHERE        zpa.AttributeCode IN ('ProductName','ProductType', 'SKU', 'Price', 'Quantity', 'Status','Assortment')
)

-- SEELCT * FROM ZnodePimAttributeValue


SELECT zpp.PimProductid ProductId, piv.[ProductName], piv.ProductType,piv.AttributeFamilyName  AttributeFamily , piv.[SKU], [Price], [Quantity], [Status],PIV.[Assortment],PimProductTypeAssociationId,RelatedProductId,Piv.LocaleId
FROM ZNodePimProduct zpp 
INNER JOIN  AttributeValuePvot 
PIVOT 
(
 Max(AttributeValue) FOR AttributeCode  IN ([ProductName],[SKU],[Price],[Quantity],[Status],[Assortment],[ProductType] )
)Piv  ON (Piv.PimProductId = zpp.PimProductid)