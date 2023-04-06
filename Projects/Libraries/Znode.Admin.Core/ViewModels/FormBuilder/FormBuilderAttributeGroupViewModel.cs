using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class FormBuilderAttributeGroupViewModel : BaseViewModel
    {
        public int FormBuilderId { get; set; }
        public int LocaleId { get; set; }
        public string FormCode { get; set; }
        public string ButtonText { get; set; }
        public string TextMessage { get; set; }
        public string RedirectURL { get; set; }
        public bool IsTextMessage { get; set; }
        public List<GlobalAttributeGroupViewModel> Groups { get; set; }
        public List<GlobalAttributeValuesViewModel> Attributes { get; set; }
    }
}
