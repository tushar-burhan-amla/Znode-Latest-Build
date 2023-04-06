using System;

namespace Znode.Engine.Api.Models
{
    public class ContentPageModel : BaseModel
    {
        public int CMSContentPagesId { get; set; }
        public int PortalId { get; set; }
        public int CMSContentPageGroupId { get; set; }
        public string CMSContentPageGroupName { get; set; }
        public string ProfileIds { get; set; }
        public int? CMSTemplateId { get; set; }
        public int LocaleId { get; set; }
        public int CMSContentPagesLocaleId { get; set; }
        public bool IsActive { get; set; }
        public string PageTitle { get; set; }
        public string PageName { get; set; }
        public string SEOTitle { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOUrl { get; set; }
        public string MetaInformation { get; set; }
        public string ContentPageHtml { get; set; }
        public string PortalName { get; set; }
        public string PageTemplateName { get; set; }
        public string PageTemplateFileName { get; set; }
        public bool? IsRedirect { get; set; }
        public bool IsConfigurable { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ContentPageTemplateCode { get; set; }
        public string OldSEOURL { get; set; }
        public string PublishStatus { get; set; }
        public string SEOPublishStatus { get; set; }
        public bool IsPublished { get; set; }
        public string SEOCode { get; set; }
        public string CanonicalURL { get; set; }
        public string RobotTag { get; set; }
        public string MediaPath { get; set; }
    }
}
