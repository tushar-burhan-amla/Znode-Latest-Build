CREATE PROCEDURE [dbo].[Znode_DeleteOrderById]
(
	@OrderDetailId INT = 0 ,
	@Status   BIT OUT,
	@OmsOrderIds TransferId READONLY 
)
AS
/*
	begin tran
	exec Znode_DeleteOrderById 6
	rollback tran
*/
BEGIN
  SET NOCOUNT ON
   BEGIN  TRAN DeleteOrderById
  BEGIN TRY 
		   	DECLARE @OmsOrderId TABLE (OmsOrderId INT ) 
			DECLARE @OmsOrderDetailsId TABLE (OmsOrderDetailsId  INT ) 
			
			INSERT INTO  @OmsOrderId 
			SELECT Id 
			FROM  @OmsOrderIds


			Insert into @OmsOrderId
				Select OmsOrderId from ZnodeOmsOrderDetails a
				Where OmsOrderDetailsId = @OrderDetailId and not exists 
				(select * from @OmsOrderId b where b.OmsOrderId = a.OmsOrderId)


			INSERT INTO @OmsOrderDetailsId 
			SELECT OmsOrderDetailsId 
			FROM ZnodeOmsOrderDetails  ZP 
			WHERE (OmsOrderDetailsId = @OrderDetailId OR 
			EXISTS (SELECT TOP 1 1  FROM @OmsOrderId WHERE OmsOrderId = ZP.OmsOrderId)  ) 

			
			DECLARE @TBL_OmsOrderLineItems TABLE (OmsOrderLineItemsId INT,OmsOrderShipmentId INT, OmsOrderDetailsId INT)
			INSERT INTO @TBL_OmsOrderLineItems
			SELECT OmsOrderLineItemsId,OmsOrderShipmentId, OmsOrderDetailsId 
			FROM ZnodeOmsOrderLineItems S 
			WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId TR WHERE TR.OmsOrderDetailsId = S.OmsOrderDetailsId )
			DELETE FROM ZnodeOmsOrderAttribute WHERE EXISTS (SELECT OmsOrderLineItemsId FROM @TBL_OmsOrderLineItems TY WHERE TY.OmsOrderLineItemsId = ZnodeOmsOrderAttribute.OmsOrderLineItemsId)
			DELETE FROM ZnodeOmsOrderDiscount WHERE EXISTS (SELECT OmsOrderLineItemsId FROM @TBL_OmsOrderLineItems TY WHERE TY.OmsOrderDetailsId = ZnodeOmsOrderDiscount.OmsOrderDetailsId   or TY.OmsOrderLineItemsId = ZnodeOmsOrderDiscount.OmsOrderLineItemId  )
			DELETE FROM ZnodeOmsOrderWarehouse WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_OmsOrderLineItems TBLOLI WHERE TBLOLI.OmsOrderLineItemsId = ZnodeOmsOrderWarehouse.OmsOrderLineItemsId  )
			DELETE FROM ZnodeRmaRequestItem WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_OmsOrderLineItems TBLOLI WHERE TBLOLI.OmsOrderLineItemsId = ZnodeRmaRequestItem.OmsOrderLineItemsId  )
			DELETE FROM ZnodeOmsOrderLineItemsAdditionalCost WHERE EXISTS ( SELECT TOP 1 1 FROM 
			ZnodeOmsOrderLineItems WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_OmsOrderLineItems TBLOLI WHERE TBLOLI.OmsOrderDetailsId = ZnodeOmsOrderLineItems.OmsOrderDetailsId)
			AND ZnodeOmsOrderLineItems.OmsOrderLineItemsId = ZnodeOmsOrderLineItemsAdditionalCost.OmsOrderLineItemsId)

			DELETE FROM ZnodeOmsDownloadableProductKey
			WHERE EXISTS(SELECT * FROM ZnodeOmsOrderLineItems WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId rt WHERE rt.OmsOrderDetailsId =ZnodeOmsOrderLineItems.OmsOrderDetailsId)
						AND ZnodeOmsDownloadableProductKey.OmsOrderLineItemsId = ZnodeOmsOrderLineItems.OmsOrderLineItemsId)

			DELETE FROM ZnodeOmsPersonalizeItem WHERE EXISTS (SELECT TOP 1 1  FROM @TBL_OmsOrderLineItems TBLOLI WHERE TBLOLI.OmsOrderLineItemsId = ZnodeOmsPersonalizeItem.OmsOrderLineItemsId)
		   	DELETE FROM ZnodeOmsTaxOrderLineDetails WHERE EXISTS (SELECT TOP 1 1  FROM @TBL_OmsOrderLineItems TBLOLI WHERE TBLOLI.OmsOrderLineItemsId = ZnodeOmsTaxOrderLineDetails.OmsOrderLineItemsId)
		   	DELETE FROM znodeGiftCardHistory WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId rt WHERE rt.OmsOrderDetailsId =znodeGiftCardHistory.OmsOrderDetailsId)
		   	DELETE FROM znodeOmsEmailHistory WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId rt WHERE rt.OmsOrderDetailsId =znodeOmsEmailHistory.OmsOrderDetailsId)
		   	DELETE FROM ZnodeOmsReferralCommission WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId rt WHERE rt.OmsOrderDetailsId =ZnodeOmsReferralCommission.OmsOrderDetailsId)
		   	DELETE FROM ZnodeOmsTaxOrderDetails WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId rt WHERE rt.OmsOrderDetailsId =ZnodeOmsTaxOrderDetails.OmsOrderDetailsId)
			DELETE FROM ZnodeOmsHistory   WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId rt WHERE rt.OmsOrderDetailsId =ZnodeOmsHistory.OmsOrderDetailsId)
			DELETE FROM znodeOmsNotes WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId rt WHERE rt.OmsOrderDetailsId =znodeOmsNotes.OmsOrderDetailsId)
			DELETE FROM ZnodeOmsOrderLineItems WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId TBLOLI WHERE TBLOLI.OmsOrderDetailsId = ZnodeOmsOrderLineItems.OmsOrderDetailsId)
			DELETE FROM ZnodeOmsOrderShipment WHERE EXISTS (SELECT TOP 1 1  FROM @TBL_OmsOrderLineItems TBLOLI WHERE TBLOLI.OmsOrderShipmentId = ZnodeOmsOrderShipment.OmsOrderShipmentId )
		   	DELETE FROM ZnodeOmsCustomerShipping WHERE EXISTS(SELECT TOP 1 1  FROM @TBL_OmsOrderLineItems TBLOLI WHERE TBLOLI.OmsOrderDetailsId =ZnodeOmsCustomerShipping.OmsOrderDetailsId)
			DELETE FROM ZnodeOmsOrderDetails  WHERE EXISTS (SELECT TOP 1 1 FROM @OmsOrderDetailsId rt WHERE rt.OmsOrderDetailsId =ZnodeOmsOrderDetails.OmsOrderDetailsId)
			DELETE FROM ZnodeOmsTaxRule WHERE EXISTS (SELECT TOP 1 1  FROM @OmsOrderId T WHERE T.OmsOrderId = ZnodeOmsTaxRule.OmsOrderId)
            DELETE FROM ZnodeOrderPayment WHERE EXISTS (SELECT TOP 1 1  FROM @OmsOrderId T WHERE T.OmsOrderId = ZnodeOrderPayment.OmsOrderId)
		   	DELETE FROM ZnodeOmsOrder WHERE EXISTS (SELECT TOP 1 1  FROM @OmsOrderId T WHERE T.OmsOrderId = ZnodeOmsOrder.OmsOrderId)
            
		SELECT 1 AS ID , CAST(1 AS BIT) AS Status;

        SET @Status = 1;    
		 COMMIT  TRAN DeleteOrderById
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE	()
		SELECT 0 AS ID , CAST(0 AS BIT) AS Status;

		SET @Status = 0;
		
		ROLLBACK TRAN DeleteOrderById
		SELECT ERROR_MESSAGE()
	END CATCH
END