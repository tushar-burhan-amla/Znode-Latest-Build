using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TemplateListResponse : BaseListResponse
    {
        public TemplateModel Template { get; set; }

        public List<TemplateModel> Templates { get; set; }
    }
}
