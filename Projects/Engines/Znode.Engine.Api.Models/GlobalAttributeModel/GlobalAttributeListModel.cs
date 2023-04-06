using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeListModel : BaseListModel
    {
        public List<GlobalAttributeModel> Attributes { get; set; }

        public GlobalAttributeListModel()
        {
            Attributes = new List<GlobalAttributeModel>();
        }
    }
}
