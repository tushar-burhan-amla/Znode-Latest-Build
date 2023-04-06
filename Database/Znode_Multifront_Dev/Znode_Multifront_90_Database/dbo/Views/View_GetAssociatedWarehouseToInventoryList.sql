
CREATE View [dbo].[View_GetAssociatedWarehouseToInventoryList]
AS 
SELECT  a.InventoryListId,a.ListCode ,a.ListName ,b.WarehouseId,b.WarehouseCode,b.WarehouseName ,d.InventoryWarehouseId, CASE WHEN d.InventoryListId IS NULL THEN 0 ELSE 1 END IsAssociated
FROM ZnodeInventoryList a 
CROSS JOIN ZnodeWarehouse b 
LEFT JOIN ZnodeInventoryWarehouse d ON (a.InventoryListId = d.InventoryListId AND b.Warehouseid = d.WarehouseId )