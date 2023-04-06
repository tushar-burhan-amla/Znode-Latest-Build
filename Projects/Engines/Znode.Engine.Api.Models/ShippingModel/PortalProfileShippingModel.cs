using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class PortalProfileShippingModel : BaseModel
    {
        public int ProfileId { get; set; }
        public int ProfileShippingId { get; set; }
        public int PortalShippingId { get; set; }
        public int PortalId { get; set; }
        [RegularExpression(@"^[0-9,\,]*$",ErrorMessageResourceName = ZnodeApi_Resources.MessageNumericValueAllowed, ErrorMessageResourceType = typeof(Api_Resources))]
        public string ShippingIds { get; set; }
        public int DisplayOrder { get; set; }
        public string PublishState { get; set; }
    }
}
