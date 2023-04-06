using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class UrlRedirectResponse : BaseListResponse
    {
        public UrlRedirectModel UrlRedirect { get; set; }

        public List<UrlRedirectModel> UrlRedirectList { get; set; }
    }
}
