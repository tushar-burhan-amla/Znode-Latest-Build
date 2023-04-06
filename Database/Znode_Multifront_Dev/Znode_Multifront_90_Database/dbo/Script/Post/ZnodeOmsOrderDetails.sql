update a set a.Email = b.Email, a.PhoneNumber = b.PhoneNumber from ZnodeOmsOrderDetails a
inner join ZnodeUser b on a.UserId = b.UserId
where a.Email is null

update a set a.PhoneNumber = b.PhoneNumber from ZnodeOmsOrderDetails a
inner join ZnodeUser b on a.UserId = b.UserId

IF NOT EXISTS (
SELECT *
FROM sys.columns
WHERE object_id = OBJECT_ID(N'[dbo].[ZnodeOmsOrderDetails]')
AND name = 'OrderTotalWithoutVoucher'
)
alter table ZnodeOmsOrderDetails add OrderTotalWithoutVoucher decimal(18,6) null;
update ZOOD
set OrderTotalWithoutVoucher =total+isnull(Dis.DiscountAmount,0)
from ZnodeOmsOrderDetails ZOOD left join ZnodeOmsOrderDiscount Dis
on ZOOD.OmsOrderDetailsId =Dis.OmsOrderDetailsId
ANd dis.OmsDiscountTypeId = (select OmsDiscountTypeId from ZnodeOmsDiscountType where name='GIFTCARD')
where OrderTotalWithoutVoucher is null
update a set a.Email = b.Email, a.PhoneNumber = b.PhoneNumber from ZnodeOmsOrderDetails a
inner join ZnodeUser b on a.UserId = b.UserId
where a.Email is null

update a set a.PhoneNumber = b.PhoneNumber from ZnodeOmsOrderDetails a
inner join ZnodeUser b on a.UserId = b.UserId
where a.PhoneNumber is null

DECLARE @PimAttributeId INT=(SELECT PimAttributeId FROM ZnodePimAttribute WHERE IsCategory = 0 AND AttributeCode='OutOfStockOptions');
DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
INSERT INTO dbo.ZnodeOmsOrderAttribute
(OmsOrderLineItemsId,AttributeCode,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,AttributeValueCode)
SELECT DISTINCT OLI.OmsOrderLineItemsId,PA.AttributeCode,ADV.AttributeDefaultValueCode,2,@GetDate,2,@GetDate,ADV.AttributeDefaultValueCode
FROM ZnodeOmsOrder O WITH (NOLOCK)
INNER JOIN ZnodeOmsOrderDetails OD WITH (NOLOCK) ON O.OmsOrderId=OD.OmsOrderId
INNER JOIN ZnodeOmsOrderLineItems OLI WITH (NOLOCK) ON OD.OmsOrderDetailsId=OLI.OmsOrderDetailsId
INNER JOIN View_LoadManageProduct LMP WITH (NOLOCK) ON LMP.AttributeCode ='SKU' AND OLI.Sku=LMP.AttributeValue
INNER JOIN ZnodePimAttribute PA WITH (NOLOCK) ON PA.PimAttributeId=@PimAttributeId
INNER JOIN ZnodePimAttributeValue AV WITH (NOLOCK) ON AV.PimAttributeId=@PimAttributeId AND lmp.PimProductId=av.PimProductId
INNER JOIN ZnodePimProductAttributeDefaultValue AVL WITH (NOLOCK) ON AVL.PimAttributeValueId=AV.PimAttributeValueId
INNER JOIN ZnodePimAttributeDefaultValue ADV WITH (NOLOCK) ON ADV.PimAttributeId=PA.PimAttributeId
	AND ADV.PimAttributeDefaultValueId=AVL.PimAttributeDefaultValueId
WHERE NOT EXISTS (SELECT * FROM ZnodeOmsOrderAttribute zoal WHERE oli.OmsOrderLineItemsId=zoal.OmsOrderLineItemsId
					AND zoal.AttributeCode='OutOfStockOptions');

UPDATE OLI
SET OLI.BundleQuantity=BPE.AssociatedProductBundleQuantity
FROM ZnodeOmsOrderLineItems OLI
INNER JOIN ZnodeOmsOrderDetails OD ON OD.OmsOrderDetailsId=OLI.OmsOrderDetailsId
INNER JOIN ZnodeOmsOrder O ON O.OmsOrderId=OD.OmsOrderId
INNER JOIN View_LoadManageProduct LMP ON LMP.AttributeCode ='SKU' AND OLI.Sku=LMP.AttributeValue
INNER JOIN ZnodePublishProduct PP ON LMP.PimProductId=PP.PimProductId
INNER JOIN ZnodeOmsOrderLineItemRelationshipType OLIRT WITH (NOLOCK) ON OLIRT.OrderLineItemRelationshipTypeId=OLI.OrderLineItemRelationshipTypeId
	AND OLIRT.Name='Bundles'
INNER JOIN
(
	SELECT VersionId,AssociatedZnodeProductId,ROW_NUMBER() OVER (PARTITION BY AssociatedZnodeProductId ORDER BY VersionId DESC) As Rn,AssociatedProductBundleQuantity
	FROM ZnodePublishBundleProductEntity
) BPE ON BPE.AssociatedZnodeProductId=PP.PublishProductId AND BPE.Rn=1
WHERE OLI.BundleQuantity IS NULL;

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsOrderDetails_PortalId_UserId_IsActive' AND object_id = OBJECT_ID('ZnodeOmsOrderDetails'))
BEGIN
	CREATE INDEX IX_ZnodeOmsOrderDetails_PortalId_UserId_IsActive
		ON ZnodeOmsOrderDetails (PortalId, UserId,IsActive);
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ZnodeOmsSavedCartLineItem_OmsSavedCartId_SKU_OrderLineItemRelationshipTypeId' AND object_id = OBJECT_ID('ZnodeOmsSavedCartLineItem'))
BEGIN
	CREATE INDEX IX_ZnodeOmsSavedCartLineItem_OmsSavedCartId_SKU_OrderLineItemRelationshipTypeId
		ON ZnodeOmsSavedCartLineItem (OmsSavedCartId,SKU ,OrderLineItemRelationshipTypeId);
END