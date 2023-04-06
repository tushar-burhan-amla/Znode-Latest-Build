using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeInputValidationListModel : BaseListModel
    {
        public List<PIMAttributeInputValidationModel> InputValidations { get; set; }
    }
}
