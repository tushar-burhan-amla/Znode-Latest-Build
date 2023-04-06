CREATE  PROCEDURE [dbo].[Znode_GetWarehouseInventoryBySkus]
( @SKUs     NVARCHAR(MAX),
  @PortalId VARCHAR(2000))
AS 
  /* 
    Summary: This procedure is used to get inventory details of sku portal wise 
	         including their warehouse details      		   
    Unit Testing   
     EXEC Znode_GetWarehouseInventoryBySku  @SKUs='ap1234,LI001',@PortalId=1
 
   */ 
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
           
			 Create table #TBL_SKUs (SKU NVARCHAR(MAX));
             Create table #TBL_PortalIds (PortalId INT);
			 

             INSERT INTO #TBL_SKUs 
                    SELECT item
                    FROM dbo.split(@SKUs, ',');

             INSERT INTO #TBL_PortalIds
                    SELECT item
                    FROM dbo.split(@PortalId, ',');

           
			 CREATE TABLE #TLB_SKUSumInventory 
             (SKU          VARCHAR(600),
              Quantity     NUMERIC(28, 6),
              ReOrderLevel NUMERIC(28, 6),
              PortalId     INT,
			  WarehouseCode VARCHAR(100),
			  WarehouseName VARCHAR(100),
			  IsDefaultWarehouse Bit Default 0 );
			 
			 CREATE TABLE #TBL_AllwareHouseToportal 
             (WarehouseId       INT,
              PortalId          INT,
              PortalWarehouseId INT,
			  IsDefaultWarehouse Bit
             );
		
             INSERT INTO #TBL_AllwareHouseToportal
                    SELECT zpw.WarehouseId,zp.PortalId,zpw.PortalWarehouseId,1
                    FROM [dbo].ZnodePortal AS zp
                         INNER JOIN [ZnodePortalWarehouse] AS zpw ON(zpw.PortalId = zp.PortalId)
                    WHERE EXISTS
                    (
                        SELECT TOP 1 1
                        FROM #TBL_PortalIds AS tp
                        WHERE tp.PortalId = zp.PortalId
                    );
             INSERT INTO #TBL_AllwareHouseToportal
                    SELECT zpaw.WarehouseId,a.PortalId,zpaw.PortalWarehouseId,0
                    FROM [dbo].[ZnodePortalAlternateWarehouse] AS zpaw
                         INNER JOIN #TBL_AllwareHouseToportal AS a ON(zpaw.PortalWarehouseId = a.PortalWarehouseId);

			
             SELECT TY.SKU,isnull(zi.Quantity,0) Quantity,zi.ReOrderLevel,zpw.PortalId,ZW.WarehouseCode,ZW.WarehouseName,ZPW.IsDefaultWarehouse
             FROM #TBL_AllwareHouseToportal AS zpw
			 CROSS APPLY #TBL_SKUs  TY 
             LEFT JOIN [dbo].[ZnodeInventory] AS ZI ON (  ISNULL(Zi.WarehouseId,-1) = CASE WHEN ISNULL(Zi.WarehouseId,-1) = -1 THEN -1 ELSE  zpw.WarehouseId END ) AND   (ZI.SKU=TY.SKU  )--(SELECT ''+ZI.SKU FOR XML PATH ('')) = TY.SKU )
			 Inner join ZnodeWarehouse ZW on zpw.WarehouseId = ZW.WarehouseId
           
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetInventoryBySkus @SKUs = '+@SKUs+',@PortalId='+@PortalId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetInventoryBySkus',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;