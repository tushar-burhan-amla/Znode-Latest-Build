using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeDefaultValueModel : BaseModel
    {
        public int PimDefaultAttributeValueId { get; set; }
        public Nullable<int> PimAttributeId { get; set; }
        public Nullable<bool> IsEditable { get; set; }

        public List<PIMAttributeDefaultValueLocaleModel> ValueLocales { get; set; }
        [Required(ErrorMessageResourceName = ZnodeApi_Resources.AttributeCodeValidation, ErrorMessageResourceType = typeof(Api_Resources))]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeApi_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Api_Resources))]
        public string AttributeDefaultValueCode { get; set; }
        public int? DisplayOrder { get; set; }
        public PIMAttributeDefaultValueModel()
        {
            ValueLocales = new List<PIMAttributeDefaultValueLocaleModel>();
        }
        public Nullable<bool> IsDefault { get; set; }
        public string SwatchText { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }

        public Nullable<bool> IsSwatch { get; set; }        
    }
}
