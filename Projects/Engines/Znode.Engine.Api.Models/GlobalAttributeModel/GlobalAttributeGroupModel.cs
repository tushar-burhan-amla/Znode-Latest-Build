using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeGroupModel : BaseModel
    {
        public int GlobalAttributeGroupId { get; set; }

        [Required]
        public string GroupCode { get; set; }
        public string AttributeGroupName { get; set; }
        public int? DisplayOrder { get; set; }
        public List<GlobalAttributeModel> Attributes { get; set; }
        public List<GlobalAttributeGroupLocaleModel> AttributeGroupLocales { get; set; }
        public int GlobalEntityId { get; set; }

        [Required]
        public string EntityName { get; set; }
    }
}
