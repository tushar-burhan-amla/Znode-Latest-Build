using System;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class CatalogModel : BaseModel
    {
        public int PimCatalogId { get; set; }
        public string CatalogName { get; set; }
        public bool IsActive { get; set; }
        public int? PortalId { get; set; }
        public bool? IsAllowIndexing { get; set; }
        public string DefaultStore { get; set; }
        public bool CopyAllData { get; set; }
        public string PublishStatus { get; set; }
        public int? PublishCategoryCount { get; set; }
        public int? PublishProductCount { get; set; }
        public  DateTime? PublishCreatedDate { get; set; }     
        public  DateTime? PublishModifiedDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorCatalogCodeRequired)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeApi_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.CatalogCodeMaxLength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string CatalogCode { get; set; }
    }
}
