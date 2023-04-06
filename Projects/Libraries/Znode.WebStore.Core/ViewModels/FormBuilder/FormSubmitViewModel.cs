using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class FormSubmitViewModel:BaseViewModel
    {
        public int? FormBuilderId { get; set; }
        public int LocaleId { get; set; }
        public int PortalId { get; set; }
        public string FormCode { get; set; }
        public bool IsSuccess { get; set; }
        public string CustomerEmail { get; set; }
        public List<FormSubmitAttributeViewModel> Attributes { get; set; }
        public FormSubmitViewModel()
        {
            Attributes = new List<FormSubmitAttributeViewModel>();
        }
    }
}
