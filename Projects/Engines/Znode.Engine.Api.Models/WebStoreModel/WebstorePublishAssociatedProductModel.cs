using System;

namespace Znode.Engine.Api.Models
{
    public class WebstorePublishAssociatedProductModel : BaseModel
    {
        public Nullable<int> PimProductId { get; set; }    
        public Nullable<int> DisplayOrder { get; set; }
        public Nullable<int> PimParentProductId { get; set; }
    }
}

