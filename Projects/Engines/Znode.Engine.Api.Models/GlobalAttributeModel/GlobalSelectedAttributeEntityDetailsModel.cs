using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalSelectedAttributeEntityDetailsModel
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public List<GlobalSelectedAttributeGroupModel> Groups { get; set; }
    }

    public class GlobalSelectedAttributeGroupModel
    {
        public int? GlobalAttributeGroupId { get; set; }
        public string GroupCode { get; set; }
        public string AttributeGroupName { get; set; }
        public List<GlobalSelectedAttributeModel> Attributes { get; set; }
    }

    public class GlobalSelectedAttributeModel
    {
        public string AttributeName { get; set; }
        public string AttributeTypeName { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValue { get; set; }
        public int? GlobalAttributeGroupId { get; set; }
    }

}
