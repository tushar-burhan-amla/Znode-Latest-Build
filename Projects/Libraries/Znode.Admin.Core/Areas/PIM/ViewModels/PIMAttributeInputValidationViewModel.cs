using System.Collections.Generic;
using Znode.Engine.Admin.AttributeValidationHelpers;

namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeInputValidationViewModel : BaseViewModel
    {
        public List<AttributeInputValidationModel> Validations { get; set; }
        public List<ValidationControlModel> Controls { get; set; }
    }
}