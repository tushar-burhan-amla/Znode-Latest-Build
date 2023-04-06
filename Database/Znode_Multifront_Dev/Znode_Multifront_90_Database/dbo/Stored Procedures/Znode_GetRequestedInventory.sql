Create PROCEDURE [dbo].[Znode_GetRequestedInventory]

AS
BEGIN
	BEGIN TRY
		 DECLARE @SKU NVARCHAR(max) 
		 	
		DECLARE @PortalId int
		 		IF OBJECT_ID('tempdb..#Tbl_Inventory') IS NOT NULL
				DROP TABLE #Tbl_Inventory
				Create TABLE #Tbl_Inventory
				(
					  SKU            VARCHAR(300),
					  Quantity    NUMERIC(28, 6),
					  ReOrderLevel     NUMERIC(28, 6),
					  PortalId      int,
					  WarehouseName	varchar(100),
					  WarehouseCode	varchar(100),
					  DefaultInventoryCount varchar(1000),
			    );

				IF OBJECT_ID('tempdb..#Tbl_InStockProduct') IS NOT NULL
				DROP TABLE #Tbl_InStockProduct
				 		Create TABLE #Tbl_InStockProduct
				(
					  StockNoticeId int,
					  ParentSKU VARCHAR(300),
					  SKU            VARCHAR(300),
					  AvailableQuantity  NUMERIC(28, 6),
					  InStockQty NUMERIC(28, 6),
					  PublishProductId int,
				      ImageSmallPath varchar(500),
				      SeoUrl varchar(1000),
					  ProductName varchar(1000),
					  ProductAttributes nvarchar(MAX)
			    );

				IF OBJECT_ID('tempdb..#Tbl_PublishProduct') IS NOT NULL
				DROP TABLE #Tbl_PublishProduct
				Create TABLE #Tbl_PublishProduct
				(
					  SKU  VARCHAR(300),
					  PublishProductId int,
					  ImageSmallPath varchar(500),
					  SeoUrl varchar(1000),
					  ProductName varchar(1000),
					  ProductAttributes nvarchar(MAX)
				)

				SELECT ZPE.SKU, ZPE.ZnodeProductId, ZPE.ImageSmallPath, ZPE.SeoUrl, ZPE.Name, ZPE.Attributes ,ZPE.IsActive, ZPE.VersionId
				INTO #PublishProductEntity
				FROM ZnodePublishProductEntity ZPE WITH (NOLOCK)
				WHERE EXISTS(SELECT * FROM ZnodeStockNotice ZSN WHERE ZPE.SKU = ZSN.SKU OR ZPE.SKU = ZSN.ParentSKU AND ZPE.ZnodeCatalogId = ZSN.CatalogId )
				AND ZPE.IsActive = 1
				AND EXISTS(SELECT * FROM ZnodePublishVersionEntity ZPV WHERE ZPV.ZnodeCatalogId = ZPE.ZnodeCatalogId AND ZPV.RevisionType = 'Production' AND ZPV.VersionId = ZPE.VersionId)


				INSERT INTO #Tbl_PublishProduct(SKU, PublishProductId, ImageSmallPath, SeoUrl, ProductName, ProductAttributes)
				SELECT DISTINCT ZPE.SKU, ZPE.ZnodeProductId, ZPE.ImageSmallPath, ZPE.SeoUrl, ZPE.Name, ZPE.Attributes  
				FROM ZnodeStockNotice  ZSN 
				inner JOIN #PublishProductEntity ZPE ON ZPE.SKU = ZSN.SKU  or ZPE.SKU = ZSN.ParentSKU

				DECLARE CUR_PortalId Cursor For
				SELECT DISTINCT PortalId
				FROM ZnodeStockNotice WHERE IsEmailSent = 0

				OPEN CUR_PortalId
				FETCH NEXT FROM CUR_PortalId INTo @PortalId

				WHILE @@FETCH_STATUS = 0
				BEGIN
					SET @SKU = ''
					SELECT @SKU =  Substring((SELECT distinct ',' + SKU  FROM ZnodeStockNotice WHERE PortalId = @PortalId AND IsEmailSent = 0  FOR XML PAth('')), 2, 4000)
					
					INSERT INTO #Tbl_Inventory (SKU,	Quantity,	ReOrderLevel,	PortalId, WarehouseName, WarehouseCode, DefaultInventoryCount)
					EXEC Znode_GetInventoryBySkus @SKUs=@SKU,@PortalId=@PortalId
					FETCH NEXT FROM CUR_PortalId INTO @PortalId

					
			    END
				CLOSE CUR_PortalId
				DEALLOCATE CUR_PortalId

				INSERT INTO #Tbl_InStockProduct(StockNoticeId, ParentSKU, SKU, AvailableQuantity, InStockQty, PublishProductId,
				ImageSmallPath, SeoUrl, ProductName, ProductAttributes)
				SELECT ZSN.StockNoticeId, ZSN.ParentSKU, ZSN.SKU, TBLI.Quantity, CASE WHEN TBLI.Quantity >= ZSN.Quantity THEN TBLI.Quantity  END, 
					PP1.PublishProductId, PP.ImageSmallPath, 
					PP1.SeoUrl,
					PP.ProductName, PP.ProductAttributes
				FROM ZnodeStockNotice ZSN 
				INNER JOIN #Tbl_Inventory TBLI ON ZSN.SKU = TBLI.SKU 
				INNER JOIN #Tbl_PublishProduct PP ON PP.SKU = ZSN.SKU
				INNER JOIN #Tbl_PublishProduct PP1 ON PP1.SKU = ZSN.ParentSKU
				where TBLI.PortalId = ZSN.PortalId
		
			   SELECT DISTINCT ZSN.SKU as ProductSKU,  ZSN.*,TSP. * 
			   FROM ZnodeStockNotice ZSN 
			   INNER JOIN #Tbl_InStockProduct TSP ON ZSN.SKU = TSP.SKU  and ZSN.StockNoticeId = TSP.StockNoticeId
			   WHERE TSP.InStockQty IS NOT NULL AND ZSN.IsEmailSent = 0 



	END TRY

	BEGIN CATCH
		DECLARE @Status BIT;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000) = ERROR_PROCEDURE()
			,@ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE()
			,@ErrorLine VARCHAR(100) = ERROR_LINE()
			,@ErrorCall NVARCHAR(MAX) = 'EXEC Znode_GetRequestedInventory'
		SELECT 0 AS ID
			,CAST(0 AS BIT) AS STATUS;
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_GetRequestedInventory'
			,@ErrorInProcedure = ''
			,@ErrorMessage = @ErrorMessage
			,@ErrorLine = @ErrorLine
			,@ErrorCall = @ErrorCall;
	END CATCH;
END;