CREATE PROCEDURE [dbo].[Znode_GetWarehouseDetailBySku]
(
@SKUs        VARCHAR(max)
,@PortalId   INT 
)
-- Summary 
--- This procedure is used to find the portals and skus warehouses 
--- EXEC Znode_GetWarehouseDetailBySku 'Apple',2
AS 
BEGIN 
 --BEGIN TRY 
 -- SET NOCOUNT oN 
	--	DECLARE @TBL_Skus    TABLE (SKU NVARCHAR(1000)) 
			

       		
	--	DECLARE @TBL_AllwareHouseToportal TABLE (WarehouseId INT , PortalId INT ,PortalWarehouseId INT  )
		
	--	 INSERT INTO @TBL_Skus (SKU)
	--	 SELECT Item
	--	 FROM dbo.split(@SKUs,',')
		
	--	 INSERT INTO @TBL_AllwareHouseToportal 
	--	 SELECT ZPw.WarehouseId,   zp.PortalId,zpw.PortalWarehouseId
	--	 from  [dbo].ZnodePortal zp 
	--	 INNER JOIN [ZnodePortalWarehouse] zpw ON (zpw.PortalId = zp.PortalId AND zp.PortalId = @PortalId )
 

	--	 INSERT INTO @TBL_AllwareHouseToportal
	--	 SELECT zpaw.WarehouseId, @PortalId PortalId,zpaw.PortalWarehouseId
	--	 FROM [dbo].[ZnodePortalAlternateWarehouse] zpaw 		
	--	 INNER JOIN @TBL_AllwareHouseToportal a  ON (zpaw.PortalWarehouseId =  a.PortalWarehouseId)
		

	--	SELECT a.WarehouseId,mq.WarehouseCode,WarehouseName,FirstName,LastName,Address1,Address2,Address3,CountryName,StateName,CityName,PostalCode,PhoneNumber,Mobilenumber,AlternateMobileNumber,IsDefaultBilling,IsDefaultShipping
	--	FROM ZnodeInventoryWarehouse  a 
	--	INNER JOIN ZnodeWarehouse mq ON (mq.WarehouseId =a.WarehouseId)
	--	LEFT JOIN ZnodeWarehouseAddress q ON (q.WarehouseId = a.WarehouseId )
	--	LEFT JOIN ZnodeAddress m ON (m.AddressId = q.AddressId )
	--	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeInventoryList b 
	--						 INNER JOIN ZnodeInventory c ON (b.InventoryListId = c.InventoryListId)	
	--					WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Skus tsk WHERE tsk.SKU = c.SKU)
	--				   )
	--	AND EXISTS (SELECT TOP 1 1 FROM @TBL_AllwareHouseToportal tawp WHERE tawp.WarehouseId = a.WarehouseId)

 -- END TRY 
 -- BEGIN CATCH
 -- SELECT ERROR_MESSAGE(),ERROR_LINE() 
 -- END CATCH 
 PRINT '41122'
END