CREATE PROCEDURE [dbo].[ZnodeReport_DashboardLowInventoryProductCount]
(
	 @PortalId       VARCHAR(MAX)  = '',
	 @BeginDate      DATE          = NULL,
	 @EndDate        DATE          = NULL
)
AS
	/* Summary : -This Procedure is used to get the dashboard report on basis of portal 
	 Unit Testing 
	 EXEC ZnodeReport_DashboardLowInventoryProductCOunt
	 
	*/
 BEGIN 
	BEGIN TRY
	SET NOCOUNT ON
             DECLARE @TBL_ProtalIds TABLE(PortalId INT);
			 DECLARE @CountDetail INT = 0 
             --INSERT INTO @TBL_SKUs
             --       SELECT item
             --       FROM dbo.split(@SKUs, ',');
             INSERT INTO @TBL_ProtalIds
                    SELECT item
                    FROM dbo.split(@PortalId, ',');
             DECLARE @TLB_SKUSumInventory TABLE
             (SKU          VARCHAR(600),
              Quantity     NUMERIC(28, 6),
              ReOrderLevel NUMERIC(28, 6),
              PortalId     INT
             );
           
			 CREATE TABLE #TBL_AllwareHouseToportal 
             (WarehouseId       INT,
              PortalId          INT,
              PortalWarehouseId INT
			  
             );

			 CREATE INDEX idx_#TBL_AllwareHouseToportal ON #TBL_AllwareHouseToportal(WarehouseId)

             INSERT INTO #TBL_AllwareHouseToportal
                    SELECT ZPw.WarehouseId,
                           zp.PortalId,
                           zpw.PortalWarehouseId
                    FROM [dbo].ZnodePortal AS zp
                         INNER JOIN [ZnodePortalWarehouse] AS zpw ON(zpw.PortalId = zp.PortalId)
                    WHERE EXISTS
                    (
                        SELECT TOP 1 1
                        FROM @TBL_ProtalIds AS tp
                        WHERE tp.PortalId = zp.PortalId OR @PortalId= ''
                    );

             INSERT INTO #TBL_AllwareHouseToportal
                    SELECT zpaw.WarehouseId,
                           a.PortalId,
                           zpaw.PortalWarehouseId
                    FROM [dbo].[ZnodePortalAlternateWarehouse] AS zpaw
                         INNER JOIN #TBL_AllwareHouseToportal AS a ON(zpaw.PortalWarehouseId = a.PortalWarehouseId);
  
			 
		   SELECT ZI.SKU , zpw.PortalId,ZW.WarehouseName,ZP.StoreName ,zi.Quantity,Zi.ReOrderLevel
           into #AllwareHouseToportalCnt
		   FROM #TBL_AllwareHouseToportal AS zpw
            INNER JOIN #TBL_AllwareHouseToportal AS ziw ON(ziw.WarehouseId = zpw.WarehouseId)
            INNER JOIN [dbo].[ZnodeInventory] AS ZI ON(zi.WarehouseId = ziw.WarehouseId)
			INNER JOIN ZnodeWareHouse ZW ON (ZW.WarehouseId = ziw.WarehouseId)
			INNER JOIN ZnodePortal ZP ON (ZP.PortalId = ZPW.PortalId)

		    create index idx_#AllwareHouseToportalCnt_SKU on #AllwareHouseToportalCnt (SKU)
		    
			select top 1 @CountDetail =  COUNT(*)Over()
			from #AllwareHouseToportalCnt a
			INNER JOIN dbo.ZnodePublishProductDetail ZPPD ON (ZPPD.SKU = a.SKU)
            GROUP BY a.SKU , a.PortalId,ZPPD.ProductName,a.WarehouseName,a.StoreName 
		    HAVING SUM(ISNULL(a.Quantity, 0)) <= SUM(ISNULL(a.ReOrderLevel, 0))

		   SELECT @CountDetail  ProductCount
		END TRY
		BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardLowInventoryProductCount @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_DashboardLowInventoryProductCount',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH

END