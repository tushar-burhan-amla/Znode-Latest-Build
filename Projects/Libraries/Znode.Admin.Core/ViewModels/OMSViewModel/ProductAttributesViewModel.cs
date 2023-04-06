using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductAttributesViewModel : BaseViewModel
    {
        public string AttributeValue { get; set; }
        public string ImageName { get; set; }
        public bool IsDisabled { get; set; }
        public string ImagePath { get; set; }
        public int DisplayOrder { get; set; }
        public List<AttributesSelectValuesViewModel> SelectValues { get; set; }
        public string SwatchText { get; set; }
    }
}