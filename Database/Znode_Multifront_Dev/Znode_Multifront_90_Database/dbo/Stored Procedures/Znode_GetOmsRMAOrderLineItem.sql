
CREATE PROCEDURE [dbo].[Znode_GetOmsRMAOrderLineItem]
( @RMARequestItemIDs VARCHAR(MAX) = '',
  @Status bit=0)
AS 
  /*
  Summary :- This Procedure is used to get the list RMA Request item 
			 Result is Fetched based on RMARequestItemIDs passed as parameter 
  Unit Testing 
	 begin tran
     EXEC Znode_GetOmsRMAOrderLineItem  1,1
	 rollback tran

 */
 
     BEGIN
			BEGIN TRY		
				SET NOCOUNT ON;
			
				SELECT RMARequestId,RmaRequestItemId,[OmsOrderDetailsId],
			    (SELECT UserId FROM ZnodeOmsOrderDetails WHERE OmsOrderDetailsId = OItem.OmsOrderDetailsId)                                
				UserId,[ProductName],[SKU],[Description],RItem.[Quantity],OItem.[Price],[SKU],
				ISNULL((SELECT TOP 1 GCExpirationPeriod FROM ZnodeRMAConfiguration), 0) GCExpirationPeriod
				FROM [ZnodeOmsOrderLineItems] OItem
				LEFT JOIN ZNodeRMARequestItem RItem ON(OItem.OmsOrderLineItemsId = RItem.OmsOrderLineItemsId)
				WHERE RItem.OmsOrderLineItemsId IN (SELECT Item FROM dbo.split(@RMARequestItemIDs, ','));
      
			END TRY
			BEGIN CATCH
		     
				DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOmsRMAOrderLineItem @RMARequestItemIDs = '+@RMARequestItemIDs+',@S
tatus='+CAST(@Status AS VARCHAR(200));           
				SET @Status = 0;
				SELECT 0 AS ID, CAST(0 AS BIT) AS Status;                  
 			
				EXEC Znode_InsertProcedureErrorLog
					@ProcedureName    = 'Znode_GetOmsRMAOrderLineItem',
					@ErrorInProcedure = @Error_procedure,
					@ErrorMessage     = @ErrorMessage,
					@ErrorLine        = @ErrorLine,
					@ErrorCall        = @ErrorCall;
                     
			END CATCH;
		END;