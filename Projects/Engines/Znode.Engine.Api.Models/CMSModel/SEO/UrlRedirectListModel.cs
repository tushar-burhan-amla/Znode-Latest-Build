using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class UrlRedirectListModel : BaseListModel
    {
        public List<UrlRedirectModel> UrlRedirects { get; set; }
    }
}
