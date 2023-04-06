
CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_LowInventoryProduct]
(   
	@WarehouseName NVARCHAR(max) = '',
	@ShowOnlyActiveProducts BIT = 0
)
AS 
/*
     Summary :- THis Procedure is used to find the ReOrder level of order 
     Unit Testing
     EXEC ZnodeDevExpressReport_LowInventoryProduct  @WarehouseName = '' , @ShowOnlyActiveProducts =  1
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultValue('Locale');
             DECLARE @TBL_PortalWarehouse TABLE (PortalId INT , WarehouseId INT,WarehouseName VARCHAR(100) )
			 DECLARE @TBL_PortalId TABLE (PortalId INT )
		     DECLARE @TBL_WarehouseId TABLE (WarehouseId INT )
					

			IF OBJECT_ID('TEMPDB..#ProductDetailInventory') IS NOT NULL
			    DROP TABLE #ProductDetailInventory	 

			
			  INSERT INTO @TBL_WarehouseId
			  SELECT WarehouseId
			  FROM ZnodeWarehouse ZW
			  INNER JOIN dbo.split(@WarehouseName,'|') SP ON (SP.Item = ZW.WarehouseName)
			

				;With Cte_WareHouse AS 
				(  
				SELECT WarehouseId ,PortalId,ZPW.PortalWarehouseId
				FROM ZnodePortalWarehouse ZPW 
				WHERE (EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId rt WHERE rt.PortalId = ZPW.PortalId)
				OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId ))  
				)	
				, Cte_AlternetPortal AS 
				( 
				SELECT ZPAW.WarehouseId,CTW.PortalId
				FROM ZnodePortalAlternateWarehouse ZPAW 
				INNER JOIN Cte_WareHouse CTW ON (CTW.PortalWarehouseId = ZPAW.PortalWarehouseId)  
				)
				
				INSERT INTO @TBL_PortalWarehouse(portalId , WarehouseId )
				SELECT portalId , WarehouseId 
				FROM Cte_WareHouse ZPW
				WHERE  (EXISTS (SELECT TOP 1 1 FROM @TBL_WarehouseId rt WHERE rt.WarehouseId = ZPW.WarehouseId)
				OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_WarehouseId ))  
				UNION 
				SELECT portalId , WarehouseId 
				FROM Cte_AlternetPortal ZPWP
				WHERE  (EXISTS (SELECT TOP 1 1 FROM @TBL_WarehouseId rt WHERE rt.WarehouseId = ZPWP.WarehouseId)
				OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_WarehouseId ))  

		        UPDATE  TBPW
				SET WarehouseName = a.WarehouseName
				FROM @TBL_PortalWarehouse TBPW
				INNER JOIN  ZnodeWarehouse a  ON (a.WarehouseId = TBPW.WarehouseId )
				  
				  create table #ProductDetailInventory(PimProductId int,SKU varchar(600),ProductName varchar(600),IsActive varchar(2000),UOM varchar(1000),LocaleId int)

				  insert into #ProductDetailInventory(PimProductId,SKU, LocaleId)
				  select PimProductId, e.AttributeValue, e.LocaleId
				  from ZnodePimAttributeValue d 
				  INNER JOIN ZnodePimAttributeValueLocale e on (d.PimAttributeValueId = e.PimAttributeValueId)
				  INNER JOIN ZnodePimAttribute PA ON (PA.PimAttributeId = d.PimAttributeId)
				  WHERE  AttributeCode = 'SKU'
				  AND exists(select SKU from ZnodeInventory ZI WHERE ZI.Reorderlevel IS NOT NULL and ZI.Quantity <= ZI.Reorderlevel AND ZI.SKU = e.AttributeValue )

				  update c set ProductName = e.AttributeValue, LocaleId = e.LocaleId
				  FROM #ProductDetailInventory c 
				  INNER JOIN ZnodePimAttributeValue d on (c.PimProductid = d.PimProductid)
				  INNER JOIN ZnodePimAttributeValueLocale e on (d.PimAttributeValueId = e.PimAttributeValueId)
				  INNER JOIN ZnodePimAttribute PA ON (PA.PimAttributeId = d.PimAttributeId)
				  WHERE  AttributeCode = 'ProductName'

				  update c set IsActive = e.AttributeValue, LocaleId = e.LocaleId
				  FROM #ProductDetailInventory c 
				  INNER JOIN ZnodePimAttributeValue d on (c.PimProductid = d.PimProductid)
				  INNER JOIN ZnodePimAttributeValueLocale e on (d.PimAttributeValueId = e.PimAttributeValueId)
				  INNER JOIN ZnodePimAttribute PA ON (PA.PimAttributeId = d.PimAttributeId)
				  WHERE  AttributeCode = 'IsActive'

				  update c set UOM = a.AttributeDefaultValueCode, LocaleId = ZPPADV.LocaleId
				  FROM #ProductDetailInventory c
				  inner join ZnodePimAttributeValue  ZPADV on (c.PimProductid = ZPADV.PimProductid)
				  INNER JOIN ZnodePimProductAttributeDefaultValue ZPPADV ON (ZPPADV.PimAttributeValueId = ZPADV.PimAttributeValueId)
				  INNER JOIN ZnodePimAttributeDefaultValue a ON (a.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )
				  INNER JOIN ZnodePimAttribute PA ON ( PA.PimAttributeId=a.PimAttributeId  )
				  WHERE AttributeCode = 'UOM'


		       SELECT  ZPPD.SKU,dbo.Fn_GetDefaultInventoryRoundOff(ZI.Quantity)  Quantity,dbo.Fn_GetDefaultInventoryRoundOff(ZI.Reorderlevel ) ReOrderLevel,
						ZPPD.ProductName    ,TBPW.WarehouseName ,
						CASE WHEN ZPPD.IsActive = 'true' Then 'Active' ELSE 'Inactive' END AS [ProductStatus],
						ZPPD.UOM UnitOfMeasurement
				FROM ZnodeInventory ZI
				INNER JOIN #ProductDetailInventory ZPPD ON(ZPPD.SKu = ZI.SkU AND ZPPD.LOcaleId = @DefaultLocaleId)
				INNER JOIN @TBL_PortalWarehouse TBPW ON (TBPW.WarehouseId = ZI.WarehouseId)
				INNER JOIN ZnodePortal ZP ON (ZP.PortalId = TBPW.PortalId)
				WHERE ZI.Reorderlevel IS NOT NULL 
				AND ZI.Quantity <= ZI.Reorderlevel 
				AND ZPPD.IsActive = CASE WHEN @ShowOnlyActiveProducts = 1 THEN  'true' ELSE ZPPD.IsActive END
				GROUP BY  ZPPD.SKU  ,ZI.Quantity,ZI.Reorderlevel,ZPPD.ProductName ,TBPW.WarehouseName ,ZPPD.IsActive,ZPPD.UOM
                ORDER BY TBPW.WarehouseName,  ZI.Quantity, ZPPD.ProductName ASC


				IF OBJECT_ID('TEMPDB..#ProductDetailInventory') IS NOT NULL
			    DROP TABLE #ProductDetailInventory

				
		 END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeDevExpressReport_LowInventoryProduct @Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeDevExpressReport_LowInventoryProduct',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;