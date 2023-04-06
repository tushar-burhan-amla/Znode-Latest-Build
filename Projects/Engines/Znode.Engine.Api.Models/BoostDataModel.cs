﻿namespace Znode.Engine.Api.Models
{
    public class BoostDataModel : BaseModel
    {
        public decimal Boost { get; set; }
        public int PublishProductId { get; set; }
        public int PublishCategoryId { get; set; }
        public string PropertyName { get; set; }
        public string BoostType { get; set; }
        public int ID { get; set; }
        public int CatalogId { get; set; }
    }
}
