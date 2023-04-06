using System;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class BlogNewsModel : BaseModel
    {
        public int BlogNewsId { get; set; }
        public int PortalId { get; set; }
        public int? MediaId { get; set; }
        public int LocaleId { get; set; }
        public int BlogNewsLocaleId { get; set; }
        public int? CMSContentPagesId { get; set; }
        public int CMSSEODetailId { get; set; }
        public int CMSSEODetailLocaleId { get; set; }
        public int? SEOId { get; set; }
        public int CMSSEOTypeId { get; set; }
        public int? BlogNewsContentId { get; set; }
        public string CountComments { get; set; }

        public bool IsBlogNewsActive { get; set; }
        public bool IsAllowGuestComment { get; set; }

        public string StoreName { get; set; }
        public string PageName { get; set; }

        [Required(ErrorMessageResourceName = ZnodeApi_Resources.RequiredField, ErrorMessageResourceType = typeof(Api_Resources))]
        public string BlogNewsType { get; set; }

        [Required(ErrorMessageResourceName = ZnodeApi_Resources.RequiredField, ErrorMessageResourceType = typeof(Api_Resources))]
        public string BlogNewsTitle { get; set; }

        public string BodyOverview { get; set; }
        public string Tags { get; set; }
        public string BlogNewsContent { get; set; }
        public string SEOTitle { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOUrl { get; set; }
        public string MediaPath { get; set; }

        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        [Required(ErrorMessageResourceName = ZnodeApi_Resources.RequiredField, ErrorMessageResourceType = typeof(Api_Resources))]
        public string BlogNewsCode { get; set; }
        public string SEOCode { get; set; }
        public int PublishStateId { get; set; }
        public string PublishStatus { get; set; }
    }
}
