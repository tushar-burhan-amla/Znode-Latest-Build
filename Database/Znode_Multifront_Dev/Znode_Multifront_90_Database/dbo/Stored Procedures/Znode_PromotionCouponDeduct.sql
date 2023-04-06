CREATE PROCEDURE [dbo].[Znode_PromotionCouponDeduct]
(
	@PromotionCoupons DBO.PromotionCoupons READONLY
)
AS
BEGIN
SET NOCOUNT ON  
BEGIN TRY

		----Quantity decuction for promotion coupons
		UPDATE ZPC SET ZPC.AvailableQuantity = ZPC.AvailableQuantity-1 
		FROM ZnodePromotionCoupon ZPC
		WHERE EXISTS(SELECT * FROM @PromotionCoupons PC WHERE ZPC.Code = PC.Code AND ( PC.IsExistInOrder IN ('0','FALSE') OR ISNULL(PC.OmsOrderId,0) = 0 ))

		IF EXISTS(SELECT * FROM @PromotionCoupons WHERE IsExistInOrder IN ('1','TRUE') AND ISNULL(OmsOrderId,0) > 0)
		BEGIN
			--For existing order if coupon not used
			UPDATE ZPC SET ZPC.AvailableQuantity = ZPC.AvailableQuantity-1 
			FROM ZnodeOmsOrderDetails dtls 
			INNER JOIN ZnodeOmsOrderLineItems item on dtls.OmsOrderDetailsId = item.OmsOrderDetailsId 
			INNER JOIN [ZnodeOmsOrderDiscount] disc on (disc.OmsOrderLineItemId = item.OmsOrderLineItemsId or disc.OmsOrderDetailsId = dtls.OmsOrderDetailsId)
			INNER JOIN ZnodeOmsDiscountType typ on disc.OmsDiscountTypeId = typ.OmsDiscountTypeId
			INNER JOIN @PromotionCoupons PC ON BINARY_CHECKSUM(disc.discountcode) <> BINARY_CHECKSUM(PC.Code) AND ISNULL(PC.OmsOrderId,0) = dtls.OmsOrderId
			INNER JOIN ZnodePromotionCoupon ZPC ON PC.Code = ZPC.Code
			WHERE dtls.IsActive = 1 and typ.Name = 'COUPONCODE'
			AND PC.IsExistInOrder IN ('1','TRUE') 
		END

END TRY
BEGIN CATCH

	DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PromotionCouponDeduct '

	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName    = 'Znode_PromotionCouponDeduct',
	@ErrorInProcedure = @ERROR_PROCEDURE,
	@ErrorMessage     = @ErrorMessage,
	@ErrorLine        = @ErrorLine,
	@ErrorCall        = @ErrorCall;
END CATCH

END