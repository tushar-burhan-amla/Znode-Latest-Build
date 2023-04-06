CREATE PROCEDURE [dbo].[Znode_GetInventoryBySkus]
( 
	@SKUs     NVARCHAR(MAX),
	@PortalId VARCHAR(2000)
)
AS 
  /* 
    Summary: This procedure is used to get inventory details of sku portal wise    		   
    Unit Testing   
     EXEC Znode_GetInventoryBySkus_r @SKUs='ap1234,LI001',@PortalId=1
 
   */ 
BEGIN
BEGIN TRY
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	CREATE TABLE #TBL_SKUs (SKU NVARCHAR(MAX));
	CREATE TABLE #TBL_PortalIds (PortalId INT);
			 
	INSERT INTO #TBL_SKUs 
	SELECT item
	FROM dbo.split(@SKUs, ',');

	INSERT INTO #TBL_PortalIds
	SELECT item
	FROM dbo.split(@PortalId, ',');

	CREATE TABLE #TLB_SKUSumInventory 
	(
		SKU          VARCHAR(600),
		Quantity     NUMERIC(28, 6),
		ReOrderLevel NUMERIC(28, 6),
		PortalId     INT
	);
			 
	CREATE TABLE #TBL_AllwareHouseToportal 
	(
		WarehouseId       INT,
		PortalId          INT,
		PortalWarehouseId INT,
		Id INT, ----For PortalWareHouse = 1 and AlternatePortalWarehouse = 2
		IsDefaultWarehouse BIT,
		DefaultWarehouse INT
	);

	INSERT INTO #TBL_AllwareHouseToportal(WarehouseId,PortalId,PortalWarehouseId,Id,IsDefaultWarehouse, DefaultWarehouse)
	SELECT zpw.WarehouseId,zp.PortalId,zpw.PortalWarehouseId, 1 as Id ,1 as IsDefaultWarehouse, zpw.WarehouseId as DefaultWarehouse
	FROM [dbo].ZnodePortal AS zp
	INNER JOIN [ZnodePortalWarehouse] AS zpw ON(zpw.PortalId = zp.PortalId)
	WHERE EXISTS
	(
		SELECT TOP 1 1
		FROM #TBL_PortalIds AS tp
		WHERE tp.PortalId = zp.PortalId
	);

	INSERT INTO #TBL_AllwareHouseToportal(WarehouseId,PortalId,PortalWarehouseId,Id, IsDefaultWarehouse, DefaultWarehouse)
	SELECT zpaw.WarehouseId,a.PortalId,zpaw.PortalWarehouseId, 2 as Id, 0 as IsDefaultWarehouse, null
	FROM [dbo].[ZnodePortalAlternateWarehouse] AS zpaw
	INNER JOIN #TBL_AllwareHouseToportal AS a ON(zpaw.PortalWarehouseId = a.PortalWarehouseId);

	UPDATE a SET a.DefaultWarehouse = b.DefaultWarehouse
	FROM #TBL_AllwareHouseToportal a
	INNER JOIN #TBL_AllwareHouseToportal b on a.PortalWarehouseId = b.PortalWarehouseId
	WHERE b.IsDefaultWarehouse = 1

	SELECT a.SKU, (SELECT  COUNT (1) FROM ZnodePimDownloadableProductKey RT WHERE RT.PimDownloadableProductId = a.PimDownloadableProductId
	AND RT.IsUsed = 0 ) Quantity
	INTO #Temp_DownloadableProduct
	FROM ZnodePimDownloadableProduct a 
	WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_SKUs R WHERE R.SKU = a.SKU  )

	-- Added temp table for optimization
	CREATE TABLE #ZnodeInventory (Sku NVARCHAR(300), Quantity NUMERIC(28,6), ReOrderLevel NUMERIC(28,6), WarehouseId INT)
    INSERT INTO #ZnodeInventory
    select Sku, Quantity, ReOrderLevel, WarehouseId
    FROM [dbo].[ZnodeInventory]
    WHERE Sku IN (SELECT Sku FROM #TBL_SKUs);
			  
	SELECT TY.SKU,CASE WHEN T.SKU IS NOT NULL  THEN SUM(ISNULL(T.Quantity, 0))  ELSE SUM(ISNULL( zi.Quantity, 0)) END   AS Quantity,
		SUM(ISNULL(Zi.ReOrderLevel, 0)) AS ReOrderLevel,zpw.PortalId, ZW.WarehouseName, ZW.WarehouseCode, 
		CAST(ZI1.Quantity AS varchar) as DefaultInventoryCount
	FROM #TBL_AllwareHouseToportal AS zpw
	CROSS APPLY #TBL_SKUs  TY 
	LEFT JOIN #ZnodeInventory AS ZI ON (  ZI.SKU=TY.SKU )--(SELECT ''+ZI.SKU FOR XML PATH ('')) = TY.SKU )
	LEFT JOIN #ZnodeInventory AS ZI1 ON (  ZI1 .SKU=TY.SKU and zpw.DefaultWarehouse = ZI1.WarehouseId )
	LEFT JOIN ZnodeWarehouse ZW on zpw.DefaultWarehouse = ZW.WarehouseId
	LEFT JOIN  #Temp_DownloadableProduct T ON (T.SKU = TY.SKU )
	WHERE zpw.Id = 1 AND Zi.WarehouseId is null
	GROUP BY TY.SKU,T.SKU, zpw.PortalId, ZW.WarehouseName, ZW.WarehouseCode, ZI1.Quantity
	UNION ALL	
	SELECT TY.SKU,CASE WHEN T.SKU IS NOT NULL  THEN SUM(ISNULL(T.Quantity, 0))  ELSE SUM(ISNULL( zi.Quantity, 0)) END   AS Quantity,
	SUM(ISNULL(Zi.ReOrderLevel, 0)) AS ReOrderLevel,zpw.PortalId, ZW.WarehouseName, ZW.WarehouseCode, CAST(ZI1.Quantity AS varchar) as DefaultInventoryCount
	FROM #TBL_AllwareHouseToportal AS zpw
	CROSS APPLY #TBL_SKUs  TY 
	LEFT JOIN #ZnodeInventory AS ZI ON (  ISNULL(Zi.WarehouseId,-1) = CASE WHEN ISNULL(Zi.WarehouseId,-1) = -1 THEN -1 ELSE  zpw.WarehouseId END  AND   ZI.SKU=TY.SKU )--(SELECT ''+ZI.SKU FOR XML PATH ('')) = TY.SKU )
	LEFT JOIN #ZnodeInventory AS ZI1 ON (  ZI1 .SKU=TY.SKU and zpw.DefaultWarehouse = ZI1.WarehouseId )
	LEFT JOIN ZnodeWarehouse ZW on zpw.DefaultWarehouse = ZW.WarehouseId
	LEFT JOIN  #Temp_DownloadableProduct T ON (T.SKU = TY.SKU )
	WHERE Zi.WarehouseId is not null
	GROUP BY TY.SKU,T.SKU, zpw.PortalId,ZW.WarehouseName, ZW.WarehouseCode, ZI1.Quantity
                     
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