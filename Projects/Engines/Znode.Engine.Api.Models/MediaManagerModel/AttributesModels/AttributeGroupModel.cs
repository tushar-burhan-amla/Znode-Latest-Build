using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class AttributeGroupModel : BaseModel
    {
        public int MediaAttributeGroupId { get; set; }
        [Required]
        public string GroupCode { get; set; }
        public string AttributeGroupName { get; set; }
        public bool IsSystemDefined { get; set; }
        public List<AttributeGroupLocaleModel> GroupLocaleListModel { get; set; }
        public int? DisplayOrder { get; set; }
        public List<AttributesDataModel> AttributeModel { get; set; }
        public bool IsHidden { get; set; }
        public AttributeGroupModel()
        {
            GroupLocaleListModel = new List<AttributeGroupLocaleModel>();
            AttributeModel = new List<AttributesDataModel>();
        }
    }
}
