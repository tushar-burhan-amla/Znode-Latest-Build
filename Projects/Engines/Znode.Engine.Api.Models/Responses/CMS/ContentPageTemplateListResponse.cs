using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ContentPageTemplateListResponse : BaseListResponse
    {
        public List<CMSContentPageTemplateModel> ContentPageTemplateList { get; set; }
    }
}
