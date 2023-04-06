using System.Collections.Generic;


namespace Znode.Engine.klaviyo.Models
{
    public class KlaviyoProductDetailModel : KlaviyoBaseModel
    {
        public List<OrderLineItemDetailsModel> OrderLineItems { get; set; }
      
        public int PropertyType { get; set; }
        public string StoreName { get; set; }
    }
}
