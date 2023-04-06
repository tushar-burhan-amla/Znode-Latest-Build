using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class EmailTemplateAreaListResponse : BaseListResponse
    {
        public List<EmailTemplateAreaModel> EmailTemplateAreas { get; set; }
    }
}
