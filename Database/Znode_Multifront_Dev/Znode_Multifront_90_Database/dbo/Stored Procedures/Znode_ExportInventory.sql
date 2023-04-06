 
CREATE PROCEDURE [dbo].[Znode_ExportInventory]
( @InventoryListIds  VARCHAR(1000)
)
AS
  /*
   Summary : This procedure is used to get inventory from warehouse table when inventory ids are passed as parameter
   Unit Testing:  
   EXEC Znode_ExportInventory 4
  
   */

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             SELECT zi.SKU,zi.Quantity,zi.ReOrderLevel,zw.WarehouseCode
             FROM ZnodeInventory AS zi
             INNER JOIN Split(@InventoryListIds, ',') AS qq ON(qq.Item = zi.WarehouseId)
             INNER JOIN dbo.ZnodeWarehouse AS zw ON zi.WarehouseId = zw.WarehouseId;
			
         END TRY
         BEGIN CATCH
		     DECLARE @Status BIT ;
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ExportInventory @InventoryListIds = '+@InventoryListIds+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_ExportInventory',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;