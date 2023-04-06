using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    //This mapper code needs to Refactor.
    //It will get refactored as it is getting used.
    //To Do.
    public static class CouponMap
    {
        public static CouponModel ToModel(ZnodeCoupon znodeCoupon)
            => HelperUtility.IsNull(znodeCoupon) ? new CouponModel() :
             new CouponModel
             {
                 Code = znodeCoupon.Coupon,
                 PromotionMessage = znodeCoupon.CouponMessage,
                 CouponApplied = znodeCoupon.CouponApplied,
                 CouponValid = znodeCoupon.CouponValid,
                 IsExistInOrder = znodeCoupon.IsExistInOrder,
                 CouponPromotionType= znodeCoupon.CouponPromotionType
             };


        public static ZnodeCoupon ToZnodeCoupon(CouponModel couponModel)
           => HelperUtility.IsNull(couponModel) ? null :
             new ZnodeCoupon
             {
                 Coupon = couponModel.Code,
                 CouponMessage = couponModel.PromotionMessage,
                 CouponApplied = couponModel.CouponApplied,
                 CouponValid = couponModel.CouponValid,
                 IsExistInOrder = couponModel.IsExistInOrder
             };
    }

}
