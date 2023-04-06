using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreBlogNewsModel : BaseModel
    {
        public int BlogNewsId { get; set; }
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public int CMSContentPagesId { get; set; }
        public int MediaId { get; set; }
        public int BlogNewsLocaleId { get; set; }
        public int CMSSEODetailId { get; set; }
        public int CMSSEODetailLocaleId { get; set; }

        public int? SEOId { get; set; }
        public int? CMSSEOTypeId { get; set; }
        public int? BlogNewsContentId { get; set; }
        public int CountComments { get; set; }

        public string BlogNewsType { get; set; }
        public string BlogNewsTitle { get; set; }
        public string BodyOverview { get; set; }
        public string Tags { get; set; }
        public string SEOTitle { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOUrl { get; set; }
        public string BlogNewsContent { get; set; }
        public string MediaPath { get; set; } = "";
        public string BlogNewsComment { get; set; }

        public bool IsBlogNewsActive { get; set; }
        public bool IsAllowGuestComment { get; set; }
        public string BlogNewsCode { get; set; }

        public string ActivationDate { get; set; }
        public List<WebStoreBlogNewsCommentModel> Comments { get; set; }
    }
}
