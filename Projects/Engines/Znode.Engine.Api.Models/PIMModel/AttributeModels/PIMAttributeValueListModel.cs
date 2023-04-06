using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeValueListModel : BaseListModel
    {
        public List<PIMAttributeValueModel> AttributeValues { get; set; }

        public PIMAttributeValueListModel()
        {
            AttributeValues = new List<PIMAttributeValueModel>();
        }
    }
}
