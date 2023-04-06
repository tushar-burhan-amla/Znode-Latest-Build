using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductPromotionModel : BaseModel
    {
        public int ProductPromotionId { get; set; }
        public int PublishProductId { get; set; }
        public int PromotionId { get; set; }
        public int ReferralPublishProductId { get; set; }
        public int PromotionProductQuantity { get; set; }

        public string PromotionType { get; set; }
        public string PromotionMessage { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime ActivationDate { get; set; }

        public List<int> ProductIds { get; set; }
    }
}
