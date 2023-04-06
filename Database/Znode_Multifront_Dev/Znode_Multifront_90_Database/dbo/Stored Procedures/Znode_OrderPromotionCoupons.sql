CREATE PROCEDURE [dbo].[Znode_OrderPromotionCoupons]
(
@OrderId INT,
 @CouponCode  VARCHAR(max)= ''
)
AS
BEGIN
BEGIN TRY
print 'abc'
DECLARE @Codedata TABLE(code varchar(50));
             INSERT INTO @Codedata
                    SELECT item
                    FROM dbo.Split(@CouponCode, ',') AS zs
					WHERE @CouponCode <> '';
					

Select distinct zp.PromotionId, zp.PromoCode, zp.Name,
zp.Description, zp.PromotionTypeId,
zp.Discount,zp.StartDate,zp.EndDate,zp.OrderMinimum,
zp.QuantityMinimum,zp.IsCouponRequired,zp.DisplayOrder,zp.IsUnique,zp.PortalId,zp.ProfileId,zp.PromotionProductQuantity,
zp.ReferralPublishProductId ,zp.PromotionMessage,zp.IsAllowedWithOtherCoupons,zpt.ClassName,zpt.ClassType,zpt.IsActive,zpt.Name,zpt.Description,
zpt.PromotionTypeId, CAST(1 AS BIT) AS IsUsedInOrder from ZnodePromotion zp left join
ZnodePromotionCoupon Zpc on zp.PromotionId =Zpc.PromotionId
inner join ZnodePromotionType zpt on zp.PromotionTypeId=zpt.PromotionTypeId
inner join ZnodeOmsOrderDiscount od on zp.PromoCode =od.DiscountCode or ZPc.Code =od.DiscountCode
inner join ZnodeOmsOrderDetails odet on od.OmsOrderDetailsId =odet.OmsOrderDetailsId and odet.IsActive=1
where  odet.OmsOrderId=@OrderId --order by zp.IsCouponRequired,zp.PromotionId
       
union All

Select distinct zp.PromotionId, zp.PromoCode, zp.Name,
zp.Description, zp.PromotionTypeId,
zp.Discount,zp.StartDate,zp.EndDate,zp.OrderMinimum,
zp.QuantityMinimum,zp.IsCouponRequired,zp.DisplayOrder,zp.IsUnique,zp.PortalId,zp.ProfileId,zp.PromotionProductQuantity,
zp.ReferralPublishProductId ,zp.PromotionMessage,zp.IsAllowedWithOtherCoupons,zpt.ClassName,zpt.ClassType,zpt.IsActive,zpt.Name,zpt.Description,
zpt.PromotionTypeId, CAST(0 AS BIT) AS IsUsedInOrder from ZnodePromotion zp inner join
ZnodePromotionCoupon Zpc on zp.PromotionId =Zpc.PromotionId
inner join ZnodePromotionType zpt on zp.PromotionTypeId=zpt.PromotionTypeId
where  Zpc.Code in (select code from @Codedata)    --='AmtOffOrder'
order by zp.IsCouponRequired,zp.PromotionId


END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()	, ERROR_LINE()
		
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_OrderPromotionCoupons @Code = '+ @couponcode+ ', @OrderId ' +CAST(@OrderId AS varchar(max))
		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		ROLLBACK TRAN DeleteSaveCartLineItem;
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_OrderPromotionCoupons', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;

END