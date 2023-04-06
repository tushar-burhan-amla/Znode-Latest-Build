using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class HighlightTypeListResponse : BaseListResponse
    {
        public List<HighlightTypeModel> HighlightTypeList { get; set; }
        public string TemplateTokens { get; set; }
    }
}
