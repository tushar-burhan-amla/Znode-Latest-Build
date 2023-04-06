using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class ParameterProductModel : BaseModel
    {
        [Required]
        public int ParentProductId { get; set; }
        public int LocaleId { get; set; }
        public int PublishCatalogId { get; set; }
        public int PortalId { get; set; }
        public string HighLightsCodes { get; set; }
        public string SelectedCode { get; set; }
        public string SelectedValue { get; set; }
        public string Codes { get; set; }
        public string Values { get; set; }
        public string SKU { get; set; }
        public string ParentProductSKU { get; set; }
        public Dictionary<string, string> SelectedAttributes { get; set; }
        public bool IsQuickView { get; set; }
        public int UserId { get; set; }

        public bool IsProductEdit { get; set; }

        public int? ParentOmsSavedCartLineItemId { get; set; }
    }
}
