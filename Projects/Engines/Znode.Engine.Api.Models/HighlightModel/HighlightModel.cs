using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class HighlightModel : BaseModel
    {
        public int HighlightId { get; set; }
        public int HighlightLocaleId { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }
        public bool DisplayPopup { get; set; }
        public string Hyperlink { get; set; }
        public int HighlightTypeId { get; set; }
        public bool IsActive { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeApi_Resources.PleaseEnterDisplayOrder, ErrorMessageResourceType = typeof(Api_Resources))]
        [Range(1, 999, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.InvalidShippingDisplayOrder)]
        public int DisplayOrder { get; set; }
        public string ImageAltTag { get; set; }        
        public string HighlightName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public int? LocaleId { get; set; }
        public string HighlightType { get; set; }
        public string MediaFileName{ get; set; }
        [Required(ErrorMessageResourceName = ZnodeApi_Resources.HighlightAlreadyExists, ErrorMessageResourceType = typeof(Api_Resources))]
        public string HighlightCode { get; set; }
        public string SEOUrl { get; set; }
        public int PublishProductId { get; set; }
        public string SKU { get; set; }
    }
}
