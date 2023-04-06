CREATE PROCEDURE [dbo].[Znode_ReturnOrderLineItem]
(	@OrderLineItemIds nvarchar(500),
	@OmsOrderDetailsId int,
	@OrderStateName nvarchar(100) ,
	@ReasonForReturnId int,
	@Quantity [numeric](28, 6),
	@IsShippingReturn bit,
	@ShippingCost [numeric](28, 6),
	@Status BIT OUT
)
AS

/*
begin tran
exec Znode_DeleteOrderById 6
rollback tran
*/
BEGIN
  SET NOCOUNT ON
   BEGIN  TRAN _TranReturnOrderLineItem
  BEGIN TRY 


			DECLARE @RETURNSTATEID INT, @ORDERSHIPMENTID INT
			DECLARE @OrderLineItemRelationshipTypeId INT =(SELECT TOP 1 OrderLineItemRelationshipTypeId FROM ZnodeOmsOrderLineItemRelationshipType WHERE NAME='Simple')
			
			select  top 1 @ORDERSHIPMENTID = OmsOrderShipmentId from ZNODEOMSORDERLINEITEMS where OmsOrderDetailsId = @OmsOrderDetailsId and IsActive = 1

			SELECT @RETURNSTATEID=OMSORDERSTATEID FROM ZNODEOMSORDERSTATE WHERE ORDERSTATENAME = @OrderStateName

			Declare @OrderLineItemId table (Lineitemid int)

			Insert into @OrderLineItemId
			SELECT ITEM FROM DBO.SPLIT(@OrderLineItemIds,',')
			
			UPDATE ZNODEOMSORDERLINEITEMS
			SET ISACTIVE = 1 ,OMSORDERDETAILSID = @OmsOrderDetailsId ,
			ORDERLINEITEMSTATEID = @RETURNSTATEID , 
			RmaReasonForReturnId = @ReasonForReturnId,
			OmsOrderShipmentId= @ORDERSHIPMENTID,
			Quantity = CASE WHEN Quantity =0 THEN 0 ELSE @Quantity END,
			ShippingCost = CASE 
								WHEN Isnull(OrderLineItemRelationshipTypeId,0) != @OrderLineItemRelationshipTypeId
								THEN @ShippingCost 
								ELSE 
								CASE 
									WHEN ISNULL(ShippingCost,0) =0 
									THEN 0 
									ELSE @ShippingCost 
								 END
							END,
			IsShippingReturn =  @IsShippingReturn
			WHERE exists (select * from @OrderLineItemId where Lineitemid = OMSORDERLINEITEMSID)
			or exists (select * from @OrderLineItemId where Lineitemid = PARENTOMSORDERLINEITEMSID)
			
        SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
        SET @Status = 1;    
		 COMMIT  TRAN _TranReturnOrderLineItem
	END TRY
	BEGIN CATCH
	   SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
	    SET @Status = 0;
		ROLLBACK TRAN _TranReturnOrderLineItem
	SELECT ERROR_MESSAGE()
	END CATCH

END
