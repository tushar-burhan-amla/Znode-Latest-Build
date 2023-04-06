using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributesListModel:BaseListModel
    {
        public List<AttributesDataModel> Attributes { get; set; }

        public AttributesListModel()
        {
            Attributes = new List<AttributesDataModel>();
        }
    }
}
