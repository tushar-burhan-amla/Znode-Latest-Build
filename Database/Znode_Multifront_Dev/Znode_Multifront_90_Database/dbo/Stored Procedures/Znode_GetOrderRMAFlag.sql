
CREATE PROCEDURE [dbo].[Znode_GetOrderRMAFlag]
( @OmsOrderDetailsId INT = 0,
  @Status            BIT = 0 OUT,
  @IsDebug bit = 0)
AS 
 /*
  Summary :-  This Procedure Is Used to get the RMA Flag on the basis of OrderDetailsId 			  
			  If Ordered Quantity of the product > RMA Item request At that time only Records are fetched
			  and Flag is set.
  Unit Testing
  for @Status = 1
  begin tran
  EXEC  [dbo].[Znode_GetOrderRMAFlag] 9
  rollback tran

  for @Status = 0
  begin tran
  EXEC  [dbo].[Znode_GetOrderRMAFlag] 21
  rollback tran

*/  
     BEGIN
	     
	    
         BEGIN TRY
		 BEGIN TRAN OrderRMAFlag
		 SET NOCOUNT ON;
             DECLARE @Count INT;
             SELECT @Count = COUNT(ZOOLI.OmsOrderLineItemsId)
             FROM [dbo].[ZnodeOmsOrderLineItems] ZOOLI
             WHERE ZOOLI.OmsOrderDetailsId = @OmsOrderDetailsId
                   AND ZOOLI.[Price] > 0
                   AND ZOOLI.[Quantity] > ISNULL(
                                                (
                                                    SELECT SUM(Quantity)
                                                    FROM ZNodeRMARequestItem ZRRI
                                                         INNER JOIN ZNodeRMARequest ZRR ON(ZRR.RmaRequestId = ZRRI.RmaRequestId)
                                                    WHERE OmsOrderLineItemsId = ZOOLI.OmsOrderLineItemsId
													AND ZRRI.IsReturnable = 0
                                                ), 0)
		 ;  
            

             IF(@Count > 0)
                 BEGIN
                     SELECT 1 ID,
                            CAST(1 AS BIT) [Status];
                 END;
             ELSE
                 BEGIN
                     SELECT 1 ID,
                            CAST(0 AS BIT) [Status];
                 END;
             SET @Status = 1;
			 
         COMMIT TRAN OrderRMAFlag;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOrderRMAFlag @OmsOrderDetailsId = '+cast (@OmsOrderDetailsId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(50));
             SET @Status = 0;			 
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
					ROLLBACK TRAN OrderRMAFlag;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_GetOrderRMAFlag',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
			
			 
         END CATCH;
     END;