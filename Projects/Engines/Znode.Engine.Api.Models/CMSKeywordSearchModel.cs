using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSKeywordSearchModel : BaseModel
    {
        //For CMS search result
        public List<SearchCMSPageModel> CMSPages { get; set; }

        public int TotalCMSPageCount { get; set; }
    }
}
