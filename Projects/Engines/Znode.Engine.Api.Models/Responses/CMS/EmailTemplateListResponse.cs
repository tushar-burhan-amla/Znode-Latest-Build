using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class EmailTemplateListResponse : BaseListResponse
    {
        public List<EmailTemplateModel> EmailTemplates { get; set; }
    }
}
