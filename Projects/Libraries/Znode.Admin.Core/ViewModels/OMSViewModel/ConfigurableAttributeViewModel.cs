using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ConfigurableAttributeViewModel:BaseViewModel
    {
        public List<PublishAttributeViewModel> ConfigurableAttributes { get; set; }
        public string CombinationErrorMessage { get; set; }
    }
}