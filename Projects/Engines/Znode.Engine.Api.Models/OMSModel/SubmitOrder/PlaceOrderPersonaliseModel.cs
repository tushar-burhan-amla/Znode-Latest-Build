

namespace Znode.Engine.Api.Models
{
    public class PlaceOrderPersonaliseModel
    {
        public string PersonalizeCode { get; set; }
        public string PersonalizeValue { get; set; }
        public string DesignId { get; set; }
        public string ThumbnailURL { get; set; }
        public string Sku { get; set; }

        //This property is used for product customization
        public string GroupId { get; set; }
        public int GroupIdentifier { get; set; }
    }
}
