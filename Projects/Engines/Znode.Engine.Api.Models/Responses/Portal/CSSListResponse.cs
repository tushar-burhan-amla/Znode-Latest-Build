using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CSSListResponse : BaseListResponse
    {
        public List<CSSModel> CSSs { get; set; }
        public string CMSThemeName { get; set; }
    }
}
