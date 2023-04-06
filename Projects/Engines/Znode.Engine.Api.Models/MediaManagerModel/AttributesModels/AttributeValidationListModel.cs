using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributeValidationListModel : BaseListModel
    {
        public AttributeValidationListModel()
        {
            AttributeValidationDataModelList = new List<AttributeValidationDataModel>();
        }
        public List<AttributeValidationDataModel> AttributeValidationDataModelList { get; set; }
    }
}
