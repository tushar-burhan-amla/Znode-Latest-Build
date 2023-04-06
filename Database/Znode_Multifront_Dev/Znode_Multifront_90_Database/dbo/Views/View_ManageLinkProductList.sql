






-- SELECT   * FROM [View_ManageLinkProductList]

CREATE View [dbo].[View_ManageLinkProductList]
AS
with AttributeValuePvot AS (
SELECT        zpald.PimParentProductId,  zpavl1.AttributeValue , zpa.AttributeCode,Zpald.PimLinkProductDetailid,zpald.PimProductId RelatedProductId,zpavl1.LocaleId,zpald.PimAttributeId
FROM ZnodePimLinkProductDetail zpald
INNER JOIN  ZnodePimAttributeValue zpav1 ON (zpav1.PimProductId = zpald.PimProductId) 
INNER JOIN ZnodePimAttribute zpa ON (zpa.PimAttributeId = zpav1.PimAttributeId) 						
LEFT JOIN ZnodePimAttributeValueLocale zpavl1 ON (zpavl1.PimAttributeValueId = zpav1.PimAttributeValueId)
WHERE zpa.AttributeCode IN ('ProductName','ProductType', 'SKU', 'Price', 'Quantity', 'Status','Assortment')
)

SELECT zpp.PimProductId Productid, Piv.[ProductName],Piv.ProductType, ''  AttributeFamily , Piv.[SKU], [Price], [Quantity][Qty], [Status],PimLinkProductDetailid,RelatedProductId,Piv.[Assortment],piv.LocaleId,Piv.PimAttributeId
FROM ZNodePimProduct zpp 
INNER JOIN  AttributeValuePvot 
PIVOT 
(
 Max(AttributeValue) FOR AttributeCode  IN ([ProductName],[SKU],[Price],[Quantity],[Status],[ProductType],[Assortment] )
)Piv  ON (Piv.PimParentProductId = zpp.PimProductId)