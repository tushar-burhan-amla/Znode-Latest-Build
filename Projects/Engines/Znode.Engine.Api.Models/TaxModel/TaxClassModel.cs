using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class TaxClassModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.PleaseSelectTaxClass)]
        public string Name { get; set; }
        public string PortalName { get; set; }
        public string ExternalId { get; set; }

        public int TaxClassId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredDisplayOrder)]
        [Range(1, 999999999, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.InvalidDisplayOrder)]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string ImportedSKUs { get; set; }
        public string SKU { get; set; }
        public int? PortalId { get; set; }
    }
}
