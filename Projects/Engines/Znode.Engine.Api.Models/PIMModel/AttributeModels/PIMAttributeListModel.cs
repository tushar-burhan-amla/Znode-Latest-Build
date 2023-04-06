using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeListModel : BaseListModel
    {
        public List<PIMAttributeModel> Attributes { get; set; }

        public PIMAttributeListModel()
        {
            Attributes = new List<PIMAttributeModel>();
        }
    }
}
