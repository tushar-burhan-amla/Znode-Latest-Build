using System.Collections.Generic;
namespace Znode.Engine.Api.Models
{
    public class AttributesListDataModel : BaseListModel
    {
        public List<AttributesDataModel> Attributes { get; set; }
        public List<AttributeTypeDataModel> AttributeTypes { get; set; }
    }
}
