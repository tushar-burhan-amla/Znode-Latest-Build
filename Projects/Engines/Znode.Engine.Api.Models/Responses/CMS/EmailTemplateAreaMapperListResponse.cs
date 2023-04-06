using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class EmailTemplateAreaMapperListResponse : BaseListResponse
    {
        public List<EmailTemplateAreaMapperModel> EmailTemplateAreaMapper { get; set; }
    }
}
