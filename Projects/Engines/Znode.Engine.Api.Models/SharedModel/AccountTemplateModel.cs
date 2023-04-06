using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AccountTemplateModel : BaseModel
    {
        public int OmsTemplateId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public int LocaleId { get; set; }
        public int PublishedCatalogId { get; set; }
        public string TemplateName { get; set; }
        public int? Items { get; set; }
        public string TemplateType { get; set; }

        public List<TemplateCartItemModel> TemplateCartItems { get; set; }
        public string OmsTemplateLineItemId { get; set; }
    }
}
