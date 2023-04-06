using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CookieMappingModel : BaseModel
    {
        public int OmsCookieMappingId { get; set; }
        public int? UserId { get; set; }
        public int PortalId { get; set; }
        public SavedCartModel SavedCart { get; set; }
        public List<SavedCartLineItemModel> SavedCartLineItems { get; set; }
    }
}
