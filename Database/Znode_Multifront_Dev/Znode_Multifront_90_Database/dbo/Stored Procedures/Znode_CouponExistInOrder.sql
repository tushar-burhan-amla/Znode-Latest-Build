CREATE PROCEDURE [dbo].[Znode_CouponExistInOrder]
(@OrderId INT,
 @Couponcode varchar(100),
 @Status                     BIT OUT

)
AS


BEGIN
  SET NOCOUNT ON  


		select @Status = count(*) 
		from znodeomsorderdetails dtls inner join 
		ZnodeOmsOrderLineItems item on dtls.OmsOrderDetailsId = item.OmsOrderDetailsId 
		inner join [ZnodeOmsOrderDiscount] disc on (disc.OmsOrderLineItemId = item.OmsOrderLineItemsId or disc.OmsOrderDetailsId = dtls.OmsOrderDetailsId)
		inner join ZnodeOmsDiscountType typ on disc.OmsDiscountTypeId = typ.OmsDiscountTypeId
		where dtls.OmsOrderId = @OrderID and dtls.IsActive = 1 and typ.Name = 'COUPONCODE'
		and BINARY_CHECKSUM(disc.discountcode) = BINARY_CHECKSUM(@Couponcode)
            
	    SELECT 1 AS ID,
         CAST(@Status AS BIT) AS Status;

        


END