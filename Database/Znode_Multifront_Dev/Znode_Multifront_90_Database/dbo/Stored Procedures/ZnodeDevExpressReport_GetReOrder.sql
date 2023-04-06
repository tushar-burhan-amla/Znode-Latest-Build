CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetReOrder]
(   
	--@PortalId VARCHAR(max) = '',
--	@WarehouseId    VARCHAR(MAX)   = '',
--    @SKU         VARCHAR(300)   = '',
--    @ProductName NVARCHAR(4000) = ''
	@WhereClause NVARCHAR(max) = ''
	)
AS 
/*
     Summary :- THis Procedure is used to find the ReOrder level of order 
     Unit Testing
     EXEC ZnodeReport_GetReOrder   1
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @SQL NVARCHAR(MAX)

		 SET @SQL ='
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultValue(''Locale'');
             DECLARE @TBL_PortalWarehouse TABLE (PortalId INT , WarehouseId INT )
			 DECLARE @TBL_PortalId TABLE (PortalId INT )
		
			 ;With Cte_WareHouse AS 
			  (  SELECT WarehouseId ,PortalId,ZPW.PortalWarehouseId
				FROM ZnodePortalWarehouse ZPW 
				--WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@PortalId,'','') SP WHERE SP.Item = ZPW.PortalId OR @PortalId = ''''  )			    
			  )	
			  , Cte_AlternetPortal AS 
			  ( SELECT ZPAW.WarehouseId,CTW.PortalId
			    FROM ZnodePortalAlternateWarehouse ZPAW 
				INNER JOIN Cte_WareHouse CTW ON (CTW.PortalWarehouseId = ZPAW.PortalWarehouseId)  )
			  ,Cte_WarehouseDetails AS 
			  (
			    SELECT portalId , WarehouseId 
				FROM Cte_WareHouse
				UNION 
				SELECT portalId , WarehouseId 
				FROM Cte_AlternetPortal
			  )

			  INSERT INTO @TBL_PortalWarehouse (PortalId ,WarehouseId)
			  SELECT PortalId ,WarehouseId FROM Cte_WarehouseDetails
				
			 ;WITH Cte_PortalWareHouse
                  AS (SELECT ZPW.WarehouseId,ZPW.WarehouseName
                      FROM ZnodeWarehouse ZPW )

				,CTE_FilteredPortalWareHouse AS (
                  SELECT DISTINCT  ZI.SKU AS ''SKU'',
                         dbo.Fn_GetDefaultInventoryRoundOff(ZI.Quantity) AS ''Quantity'',
                         dbo.Fn_GetDefaultInventoryRoundOff(ZI.Reorderlevel )AS ''ReOrderLevel'',
                         ZPPD.ProductName AS ''ProductName''  ,CTPW.WarehouseName ,ZP.StoreName,TBPW.PortalId
                  FROM ZnodeInventory ZI
                       INNER JOIN Cte_PortalWareHouse CTPW ON(CTPW.WarehouseId = ZI.WarehouseId )
                       INNER JOIN ZnodePublishProductDetail ZPPD ON(ZPPD.SKu = ZI.SkU AND ZPPD.LOcaleId = @DefaultLocaleId)
				       INNER JOIN @TBL_PortalWarehouse TBPW ON (TBPW.WarehouseId = CTPw.WarehouseId)
					   INNER JOIN ZnodePortal ZP ON (ZP.PortalId = TBPW.PortalId)
                  WHERE ZI.Reorderlevel IS NOT NULL AND ZI.Quantity <= ZI.Reorderlevel 
				)

				  ,CTE_FilteredWareHouse AS (
				  SELECT SKU,Quantity,ReOrderLevel,ProductName,WarehouseName,StoreName,PortalId
				  FROM CTE_FilteredPortalWareHouse
				  WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+' )

				  SELECT SKU,Quantity,ReOrderLevel,ProductName,WarehouseName,StoreName
				  FROM CTE_FilteredWareHouse
				  ORDER BY StoreName				  
				  '

				  PRINT @SQL
				  EXEC(@SQL)


         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetReOrder @Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetReOrder',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;