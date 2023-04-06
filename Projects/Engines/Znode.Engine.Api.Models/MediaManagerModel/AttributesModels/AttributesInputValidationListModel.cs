using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributesInputValidationListModel:BaseListModel
    {
        public List<AttributeInputValidationDataModel> InputValidations { get; set; }
    }
}
