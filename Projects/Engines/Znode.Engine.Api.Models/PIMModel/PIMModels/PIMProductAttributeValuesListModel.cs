using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMProductAttributeValuesListModel:BaseListModel
    {
        public List<PIMProductAttributeValuesModel> ProductAttributeValues { get; set; }

        public PIMProductAttributeValuesListModel()
        {
            ProductAttributeValues = new List<PIMProductAttributeValuesModel>();
        }
    }
}
