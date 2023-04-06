using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeGroupModel : BaseModel
    {
        public int PimAttributeGroupId { get; set; }
        [Required]
        public string GroupCode { get; set; }
        public bool IsSystemDefined { get; set; }
        public string AttributeGroupName { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsCategory { get; set; }
        public int PimAttributeFamilyId { get; set; }
        public bool IsNonEditable { get; set; }
        public List<PIMAttributeModel> Attributes { get; set; }
        public List<PIMAttributeGroupLocaleModel> AttributeGroupLocales { get; set; }
        public string GroupType { get; set; }
    }
}
