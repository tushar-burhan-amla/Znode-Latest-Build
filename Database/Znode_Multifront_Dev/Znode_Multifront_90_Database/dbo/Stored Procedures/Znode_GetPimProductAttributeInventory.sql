CREATE PROCEDURE [dbo].[Znode_GetPimProductAttributeInventory]
(   @SKU    SelectColumnList READONLY,
	@LocaleId INT = 1
    --@IsMultipleProduct BIT          = 0
	)
AS
/*
     Summary : - This procedure is used to find the inventory of product 
     Unit Testing 
	 begin tran
     Exec Znode_GetPimProductAttributeInventory 7
	 rollback tran
*/
     BEGIN
	 BEGIN TRAN PimProductAttributeFamilyId
         BEGIN TRY
             SET NOCOUNT ON;
			
			 IF OBJECT_ID('tempdb.dbo.#TBL_PimProductId', 'U') IS NOT NULL 
		     DROP TABLE #TBL_PimProductId
		 
			 SELECT StringColumn AS SKU INTO #TBL_PimProductId  FROM @SKU 
			
			-- DontTrackInventory cant have inventory
			SELECT sum(ZI.Quantity ) quantity  ,a.PimProductId 
			INTO #TBL_InventoryDetails
			FROM ZnodePimAttributeValue a 
			INNER JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )
			INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
			LEFT JOIN  ZnodeInventory ZI ON ZI.SKU = b.AttributeValue 
			LEFT JOIN ZnodeWarehouse ZW on (ZI.WarehouseId = ZW.WarehouseId)
			WHERE EXISTS(SELECT TOP 1 1 FROM #TBL_PimProductId TBP WHERE b.AttributeValue = TBP.SKU )
			AND c.AttributeCode = 'SKU'
			GROUP BY a.PimProductId 

			SELECT DISTINCT CASE WHEN  AttributeDefaultValueCode = 'DontTrackInventory' THEN 'DTI' 
			ELSE [dbo].[Fn_GetDefaultInventoryRoundOff](ISNULL(Quantity, 0))     END Quantity  ,tb.PimProductId 
			INTO #CTE_Getdetails
			FROM #TBL_InventoryDetails tb
			INNER JOIN ZnodePimAttributeValue PAV ON (PAV.PimProductId = tb.PimProductId)
			INNER JOIN ZnodePimProductAttributeDefaultValue a on (a.PimAttributeValueId = pav.PimAttributeValueId)
			INNER JOIN ZnodePimAttributeDefaultValue dv on (a.PimAttributeDefaultValueId = dv.PimAttributeDefaultValueId)
			INNER JOIN ZnodePimAttribute PA ON (PA.pimattributeId = PAV.pimAttributeid )
			WHERE  PA.Attributecode = 'OutOfStockOptions' AND (a.LocaleId=@LocaleId OR @LocaleId=0)
			 
		
			 SELECT   CAST(quantity AS NVARCHAR(100))  quantity ,PimProductId
			 FROM #CTE_Getdetails

			IF OBJECT_ID('tempdb.dbo.#TBL_PimProductId', 'U') IS NOT NULL 
			DROP TABLE #TBL_PimProductId
	
		 COMMIT TRAN PimProductAttributeFamilyId;
         END TRY
         BEGIN CATCH
				
          DECLARE @Status BIT ;
		  SET @Status = 0;
		  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
		  @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		  @ErrorLine VARCHAR(100)= ERROR_LINE(), 
		  @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimProductAttributeInventory' ;
              			 
          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  ROLLBACK TRAN PimProductAttributeFamilyId;

          EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_GetPimProductAttributeInventory',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;
         END CATCH;
END;