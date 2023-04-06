CREATE PROCEDURE [dbo].[Znode_GetProductWarehouseDetailInfoForWebStoreIndexing]
(   
	@PortalId         INT,
    @LocaleId         INT,
	@UserId			  INT = 2,
	@currentUtcDate    VARCHAR(200) = '',
	@ProductDetailsFromWebStore   DBO.ProductDetailsFromWebStore READONLY,
	@IsAllLocation bit =0
)
AS 
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 
			 DECLARE @Tlb_SKU TABLE (SKU VARCHAR(100))
			 DECLARE @PublishProductIds  NVARCHAR(max) ,@SKU NVARCHAR(max) 

			 DECLARE @Fn_GetDefaultLocaleId int = Dbo.Fn_GetDefaultLocaleId()

			 CREATE TABLE #Tlb_ProductData 
             (
				  PublishProductId INT,
				  SKU              NVARCHAR(100),
				  SEOTitle         NVARCHAR(200),
				  SEODescription   NVARCHAR(MAX),
				  SEOKeywords      NVARCHAR(MAX),
				  SEOUrl           NVARCHAR(MAX),
				  Rating           Numeric(28,6),
				  TotalReviews     INT,
				  RetailPrice      NUMERIC(28, 6),
				  SalesPrice       NUMERIC(28, 6),
				  TierPrice        NUMERIC(28, 6),
				  TierQuantity     NUMERIC(28, 6),
				  CurrencyCode     Varchar(100),
				  CurrencySuffix   Varchar(1000),
				
				  ExternalId       NVARCHAR(2000),
				  Quantity    NUMERIC(28, 6),
				  ReOrderLevel     NUMERIC(28, 6),
				  Custom1        NVARCHAR(MAX),
				  Custom2        NVARCHAR(MAX),
				  Custom3        NVARCHAR(MAX),
				  CanonicalURL VARCHAR(200),   
				  RobotTag VARCHAR(50)
			   );


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
			 
			 Create TABLE #Tbl_WarehouseWiseInventory
             (
				  SKU            VARCHAR(300),
				  Quantity    NUMERIC(28, 6),
				  ReOrderLevel NUMERIC(28, 6),
				  PortalId      int,
				  WarehouseCode VARCHAR(100),
				  WarehouseName VARCHAR(100),
				  IsDefaultWarehouse BIT

             );

            INSERT INTO #Tlb_ProductData (PublishProductId,SKU)
            SELECT id,SKU FROM @ProductDetailsFromWebStore
			  		
			SELECT @SKU = Substring((SELECT ',' + SKU FROM @ProductDetailsFromWebStore FOR XML PAth('')),2,4000) 

			SELECT @PublishProductIds = Substring((SELECT ',' + CONVERT(NVARCHAR(100),id ) FROM @ProductDetailsFromWebStore FOR XML PAth('')),2,4000) 

			IF @IsAllLocation=1
			BEGIN 
				Insert into #Tbl_WarehouseWiseInventory(SKU,	Quantity,ReOrderLevel,PortalId,	WarehouseCode,	WarehouseName,	IsDefaultWarehouse)
				EXEC Znode_GetWarehouseInventoryBySkus  @SKUs=@SKU,@PortalId=@PortalId
			END

			insert into #Tbl_Inventory (SKU,	Quantity,	ReOrderLevel,	PortalId, WarehouseName, WarehouseCode, DefaultInventoryCount)
			EXEC Znode_GetInventoryBySkus @SKUs=@SKU,@PortalId=@PortalId

			Update PD SET 
			 PD.Quantity = TLI.Quantity,
			 PD.ReOrderLevel= TLI.ReOrderLevel
			 FROM #Tlb_ProductData PD 
			 Inner join #Tbl_Inventory TLI on PD.SKU = TLI.SKU

	
			 DECLARE @Tlb_CustomerAverageRatings TABLE
             (PublishProductId INT,
              Rating           NUMERIC(28,6),
              TotalReviews     INT
             ); 


			   SELECT  a.PublishCatalogId ,Max(PublishCatalogLogId) PublishCatalogLogId
			   into #Cte_Catalogdaata
			   FROM ZnodePortalCatalog a 
			   INNER JOIN ZnodePublishCatalogLog b ON (b.PublishCatalogId = a.PublishCatalogId )
			   WHERE a.PortalId = @PortalId
			   GROUP BY a.PublishCatalogId 
	 
			 SELECT Row_Number()Over( PARTITION BY  BTY.SKU ORDER BY ZPAP.DisplayOrder, ZPAP.PublishAssociatedProductId) RowId ,
					BTY.SKU ParentSKU, BTY1.SKU --, UI.Quantity, UI.ReOrderLevel ,UI.WarehouseId
			 INTO #TempPublishData
			 FROM ZnodePublishProduct CTR 
			 INNER JOIN ZnodePublishProductDetail BTY ON (BTY.PublishProductId = CTR.PublishProductId)
			 INNER JOIN ZnodePublishAssociatedProduct ZPAP ON (ZPAP.ParentPimProductId = CTR.PimProductId)	
			 INNER JOIN ZnodePublishProduct CTR1 ON (ZPAP.PimProductId = CTR1.PimProductId)
			 INNER JOIN ZnodePublishProductDetail BTY1 ON (BTY1.PublishProductId = CTR1.PublishProductId)
			 WHERE ZPAP.IsConfigurable = 1  
			 AND EXISTS (SELECT TOP 1 1 FROM #Cte_Catalogdaata TY WHERE TY.PublishCatalogId = CTR.PublishCatalogId )-- AND TY.PublishCatalogLogId =ZPXML.PublishCatalogLogId)
			 AND EXISTS (SELECT TOP 1 1 FROM #Tlb_ProductData TU WHERE TU.SKU = BTY.SKU)
			 AND BTY.LocaleId = @Fn_GetDefaultLocaleId
			 AND BTY1.LocaleId = @Fn_GetDefaultLocaleId

			 alter table #TempPublishData add Quantity numeric(28,6), ReOrderLevel numeric(28,6), WarehouseId int

			 update TPD set Quantity = UI.Quantity ,  ReOrderLevel = UI.ReOrderLevel , WarehouseId = UI.WarehouseId
			 from #TempPublishData TPD
			 inner join ZnodeInventory UI ON (UI.SKU = TPD.SKU)

			DELETE FROM #TempPublishData WHERE RowId <> 1
			IF @IsAllLocation=1
			BEGIN 
				SELECT a.SKU,
				CASE WHEN TYI.ParentSKU IS NULL AND ZPCPA.PimProductId IS NULL THEN  b.Quantity ELSE ISNULL(TYI.Quantity,0) END as Quantity , 
				CASE WHEN TYI.ParentSKU IS NULL THEN  b.ReOrderLevel ELSE TYI.ReOrderLevel END ReOrderLevel,INV.Quantity AllLocationQuantity,
				b.WarehouseCode,b.WarehouseName,ISNULL(b.IsDefaultWarehouse,0) AS IsDefaultWarehouse, INV.DefaultInventoryCount
				FROM #Tlb_ProductData a
				LEFT JOIN #TempPublishData TYI ON (TYI.ParentSKU = a.SKU AND  TYI.WarehouseId  IN (SELECT  WarehouseId FROM ZnodePortalWarehouse WHERE PortalId = @PortalId))
				LEFT JOIN  #Tbl_WarehouseWiseInventory   b ON b.SKU  = a.SKU   
				LEFT JOIN ZnodePublishProduct ZPP on a.PublishProductId = ZPP.PublishProductId 
				LEFT join ZnodePimConfigureProductAttribute ZPCPA on ZPP.PimProductId = ZPCPA.PimProductId
				LEFT JOIN #Tbl_Inventory INV ON b.SKU = INV.SKU
			END
			ELSE 
			Begin 
			 SELECT a.SKU,
			 CASE WHEN TYI.ParentSKU IS NULL AND ZPCPA.PimProductId IS NULL THEN  b.Quantity ELSE ISNULL(TYI.Quantity,0) END as Quantity , 
			 CASE WHEN TYI.ParentSKU IS NULL THEN  b.ReOrderLevel ELSE TYI.ReOrderLevel END ReOrderLevel,INV.Quantity AllLocationQuantity,
			 b1.WarehouseCode,b1.WarehouseName,ISNULL(b1.IsDefaultWarehouse,0) AS IsDefaultWarehouse
			 FROM #Tlb_ProductData a
			 LEFT JOIN #TempPublishData TYI ON (TYI.ParentSKU = a.SKU AND  TYI.WarehouseId  IN (SELECT  WarehouseId FROM ZnodePortalWarehouse WHERE PortalId = @PortalId))
			 LEFT JOIN ZnodeInventory b ON (b.SKU  = a.SKU  AND  b.WarehouseId  IN (SELECT  WarehouseId FROM ZnodePortalWarehouse WHERE PortalId = @PortalId)) 
			 LEFT JOIN  #Tbl_WarehouseWiseInventory   b1 ON b.SKU  = a.SKU   
			 LEFT JOIN ZnodePublishProduct ZPP on a.PublishProductId = ZPP.PublishProductId 
			 LEFT join ZnodePimConfigureProductAttribute ZPCPA on ZPP.PimProductId = ZPCPA.PimProductId
			 LEFT JOIN #Tbl_Inventory INV ON b.SKU = INV.SKU
			 END
			
			
         END TRY
         BEGIN CATCH
		
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
			 @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
			  @ErrorLine VARCHAR(100)= ERROR_LINE(),
			   @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProduct WarehouseDetailInfoForWebStoreIndexing @PortalId='+CAST(@PortalId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@currentUtcDate='+@currentUtcDate;
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetProduct WarehouseDetailInfoForWebStoreIndexing',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;