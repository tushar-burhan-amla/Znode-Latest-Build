namespace Znode.Libraries.ECommerce.Utilities
{
    public enum OrderDiscountTypeEnum
    {
        PROMOCODE = 1,
        COUPONCODE = 2,
        GIFTCARD = 3,
        CSRDISCOUNT = 4,
        PARTIALREFUND = 5
    }


    // 
    public enum DiscountLevelTypeIdEnum
    {
        OrderLevel = 1,
        ShippingLevel = 2,
        LineItemLevel = 3,
        CSRLevel = 4,
        VoucherLevel=5
    }

    public enum PromotionTypeEnum
    {
        ZnodeCartPromotionPercentOffXifYPurchased = 4,
        ZnodeCartPromotionAmountOffXifYPurchased = 8,
    }
}
