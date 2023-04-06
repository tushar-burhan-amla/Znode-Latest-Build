CREATE PROCEDURE [dbo].[ZnodeReport_DashboardLowInventoryProduct]
(
	 @PortalId       VARCHAR(MAX)  = '',
	 @BeginDate      DATE          = NULL,
	 @EndDate        DATE          = NULL
)
AS
	/*  Summary : -This Procedure is used to get the dashboard report on basis of portal 
	    Unit Testing 
	    EXEC ZnodeReport_DashboardLowInventoryProduct
	*/
 BEGIN 
	 
	 BEGIN TRY
	 SET NOCOUNT ON
             DECLARE @TBL_ProtalIds TABLE(PortalId INT);
            
             INSERT INTO @TBL_ProtalIds SELECT item FROM dbo.split(@PortalId, ',');
             DECLARE @TLB_SKUSumInventory TABLE
             (SKU          VARCHAR(600),
              Quantity     NUMERIC(28, 6),
              ReOrderLevel NUMERIC(28, 6),
              PortalId     INT
             );
             DECLARE @TBL_AllwareHouseToportal TABLE
             (WarehouseId       INT,
              PortalId          INT,
              PortalWarehouseId INT
			  
             );
            INSERT INTO @TBL_AllwareHouseToportal
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

			-- Inserting records from alternate warehouse portal

            INSERT INTO @TBL_AllwareHouseToportal
            SELECT zpaw.WarehouseId,
            a.PortalId,
            zpaw.PortalWarehouseId
            FROM [dbo].[ZnodePortalAlternateWarehouse] AS zpaw
            INNER JOIN @TBL_AllwareHouseToportal AS a ON(zpaw.PortalWarehouseId = a.PortalWarehouseId);

			--  Fetching records when Quantity Is Less than Or Equals to ReOrderLevel        
			 
			SELECT ZI.SKU,SUM(ISNULL(zi.Quantity, 0)) AS Quantity,SUM(ISNULL(Zi.ReOrderLevel, 0)) AS ReOrderLevel,zpw.PortalId,ZPPD.ProductName,ZW.WarehouseName,ZP.StoreName 
            FROM @TBL_AllwareHouseToportal AS zpw
            INNER JOIN @TBL_AllwareHouseToportal AS ziw ON(ziw.WarehouseId = zpw.WarehouseId)
            INNER JOIN [dbo].[ZnodeInventory] AS ZI ON(zi.WarehouseId = ziw.WarehouseId)
			INNER JOIN dbo.ZnodePublishProductDetail ZPPD ON (ZPPD.SKU = ZI.SKU)
			INNER JOIN ZnodeWareHouse ZW ON (ZW.WarehouseId = ziw.WarehouseId)
			INNER JOIN ZnodePortal ZP ON (ZP.PortalId = ZPW.PortalId)
            GROUP BY ZI.SKU , zpw.PortalId,ZPPD.ProductName,ZW.WarehouseName,ZP.StoreName 
		    HAVING SUM(ISNULL(zi.Quantity, 0)) <= SUM(ISNULL(Zi.ReOrderLevel, 0))
		END TRY
		BEGIN CATCH
			DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardLowInventoryProduct @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_DashboardLowInventoryProduct',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH
END