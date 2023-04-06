using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSContentPageTemplateListModel : BaseListModel
    {
        public List<CMSContentPageTemplateModel> ContentPageTemplateList { get; set; }
    }
}
