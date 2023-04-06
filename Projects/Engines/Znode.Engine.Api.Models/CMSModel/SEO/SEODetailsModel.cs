using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Znode.Engine.Api.Models
{
    public class SEODetailsModel : BaseModel
    {
        public string ItemName { get; set; }
        public string ProductName { get; set; }
        public int CMSSEODetailId { get; set; }
        public int CMSSEODetailLocaleId { get; set; }
        public int CMSSEOTypeId { get; set; }
        public int? SEOId { get; set; }
        public string SEOTypeName { get; set; }
        [MaxLength(100)]
        public string SEOTitle { get; set; }
        [MaxLength(1000)]
        public string SEODescription { get; set; }
        [MaxLength(100)]
        public string SEOKeywords { get; set; }
        [MaxLength(1000)]
        public string SEOUrl { get; set; }
        public bool? IsRedirect { get; set; } 
        public string MetaInformation { get; set; }
        public string OldSEOURL { get; set; }
        public int LocaleId { get; set; }
        public int CMSContentPagesId { get; set; }
        public int PortalId { get; set; }
        public string PublishStatus { get; set; }
        public Nullable<bool> IsPublish { get; set; }
        public List<SelectListItem> Portals { get; set; }
        public List<SelectListItem> Locales { get; set; }
        public bool IsAllStore { get; set; }
        public int PimProductId { get; set; }
        public string SEOCode { get; set; }
        public string IsActive { get; set; }
        public byte PublishStateId { get; set; }
        public string CanonicalURL { get; set; }
        public string RobotTag { get; set; }
    }
}
