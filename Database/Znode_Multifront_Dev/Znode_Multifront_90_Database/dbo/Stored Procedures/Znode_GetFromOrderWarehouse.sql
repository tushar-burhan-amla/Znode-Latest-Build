CREATE  Procedure [dbo].[Znode_GetFromOrderWarehouse]
(
@OmsOrderLineItemsId NVARCHAR(MAX),
@Sku nvarchar(max)
--@RowsCount INT = 0 OUT
)
As
begin

select zow.OmsOrderLineItemsId, zow.WarehouseId, DRE.Quantity ,ZCP.SKU
from ZnodeOmsOrderWarehouse zow 
INNER JOIN ZNodeOmsOrderLineItems DRE ON (DRE.OmsOrderLineItemsId = ZOW.OmsOrderLineItemsId)
INNER JOIN ZnodeOmsOrderDetails TTY ON (TTY.OmsOrderDetailsId = DRE.OmsOrderDetailsId )
INNER JOIN ZnodeInventory ZCP ON zow.WarehouseId = ZCP.WarehouseId 
where EXISTS (SELECT TOP 1 1 FROM dbo.split(@OmsOrderLineItemsId,',') SP WHERE  zow.OmsOrderLineItemsId = Sp.Item)
AND EXISTS (SELECT TOP 1 1 FROM dbo.Split(@Sku,',') SP WHERE SP.item = ZCP.SKU AND SP.Item = DRE.SKU )
AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsOrderState THDD WHERE THDD.OmsOrderStateId = TTY.OmsOrderStateId  AND THDD.OrderStateName='SHIPPED' )

--SELECT @RowsCount  RowsCount 

End


 --var skus = (from orderlineItem in orderModel.OrderLineItems
 --                      join warehouse in _orderWarehouseRepository.Table on orderlineItem.OmsOrderLineItemsId equals warehouse.OmsOrderLineItemsId
 --                      join inventory in _inventoryRepository.Table on warehouse.WarehouseId equals inventory.WarehouseId
 --                      where inventory.SKU == orderlineItem.Sku & orderlineItem.OrderLineItemState != ZNodeOrderStatusEnum.SHIPPED.ToString()
 --                      select new { sku = orderlineItem.Sku, warehouseId = warehouse.WarehouseId, quantity = orderlineItem.Quantity });