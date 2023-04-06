using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributesTypeListModel:BaseListModel
    {
        public List<AttributeTypeDataModel> Types { get; set; }
    }
}
