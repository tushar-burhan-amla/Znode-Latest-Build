using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class BlogNewsViewModel : BaseViewModel
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
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ErrorCommentRequired)]
        public string BlogNewsComment { get; set; }

        public bool IsBlogNewsActive { get; set; }
        public bool IsAllowGuestComment { get; set; }
        public List<BlogNewsCommentViewModel> Comments { get; set; }
        public string ActivationDate { get; set; }
    }
}
