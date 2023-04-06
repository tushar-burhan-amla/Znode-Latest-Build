using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ConfigurableAttributeViewModel:BaseViewModel
    {
        public List<AttributesViewModel> ConfigurableAttributes { get; set; }
        public List<SwatchImageViewModel> SwatchImages { get; set; }
        public string CombinationErrorMessage { get; set; }
    }
}