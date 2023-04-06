using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AttributesViewModel : BaseViewModel
    {
        public string AttributeCode { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValues { get; set; }
        public string AttributeTypeName { get; set; }
        public bool IsPromoRuleCondition { get; set; }
        public bool IsComparable { get; set; }
        public bool IsHtmlTags { get; set; }
        public bool IsConfigurable { get; set; }
        public bool IsCustomField { get; set; }
        public bool IsPersonalizable { get; set; }
        public bool IsUseInSearch { get; set; }
        public string IsSwatch { get; set; }
        public int DisplayOrder { get; set; }
        public string[] SelectedAttributeValue { get; set; }
        public List<AttributesSelectValuesViewModel> SelectValues { get; set; }
        public List<ProductAttributesViewModel> ConfigurableAttribute { get; set; }
    }
}