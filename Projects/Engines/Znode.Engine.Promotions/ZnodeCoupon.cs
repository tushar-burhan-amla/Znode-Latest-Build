using System;
using System.Xml.Serialization;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Promotions
{
    [Serializable]
    [XmlRoot("ZnodeCoupon")]
    public class ZnodeCoupon : ZnodeBusinessBase
    {
        #region Private Variables
        private string _promotionMessage;
        #endregion

        #region Public Properties
        [XmlElement]
        public int PortalId { get; set; }

        [XmlElement]
        public int ProfileId { get; set; }

        [XmlElement]
        public decimal Discount { get; set; }

        [XmlElement]
        public int DiscountTypeId { get; set; }

        [XmlElement]
        public decimal MinimumOrderAmount { get; set; }

        [XmlElement]
        public int RequiredBrandId { get; set; }

        [XmlElement]
        public decimal RequiredBrandMinimumQuantity { get; set; }

        [XmlElement]
        public int RequiredCatalogId { get; set; }

        [XmlElement]
        public decimal RequiredCatalogMinimumQuantity { get; set; }

        [XmlElement]
        public int RequiredCategoryId { get; set; }

        [XmlElement]
        public decimal RequiredCategoryMinimumQuantity { get; set; }

        [XmlElement]
        public int RequiredProductId { get; set; }

        [XmlElement]
        public decimal RequiredProductMinimumQuantity { get; set; }

        [XmlElement]
        public int? DiscountedProductId { get; set; }

        [XmlElement]
        public decimal DiscountedProductQuantity { get; set; }

        [XmlElement]
        public int CouponId { get; set; }

        [XmlElement]
        public string CouponCode { get; set; }

        [XmlElement]
        public int CouponQuantityAvailable { get; set; }

        [XmlElement]
        public bool CouponApplied { get; set; }

        [XmlElement]
        public bool CouponValid { get; set; }

        [XmlElement]
        public DateTime StartDate { get; set; }

        [XmlElement]
        public DateTime EndDate { get; set; }

        [XmlElement]
        public bool IsCouponAllowedWithOtherCoupons { get; set; }

        [XmlElement]
        public string PromotionMessage
        {
            get
            {
                if (string.IsNullOrEmpty(_promotionMessage))
                    return "Coupon Successfully Applied";

                return _promotionMessage;
            }
            set
            {
                _promotionMessage = value;
            }
        } 
        #endregion
    }
}