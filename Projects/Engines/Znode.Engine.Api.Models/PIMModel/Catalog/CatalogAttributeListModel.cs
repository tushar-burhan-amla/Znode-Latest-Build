namespace Znode.Engine.Api.Models
{
    public class CatalogAttributeListModel : BaseModel
    {
        public int ZnodeCatalogId { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeTypeName { get; set; }
        public bool IsPromoRuleCondition { get; set; }
        public bool IsComparable { get; set; }
        public bool IsHtmlTags { get; set; }
        public bool IsFacets { get; set; }
        public bool IsUseInSearch { get; set; }
        public bool IsPersonalizable { get; set; }
        public bool IsConfigurable { get; set; }
        public string AttributeName { get; set; }
        public int LocaleId { get; set; }
        public int DisplayOrder { get; set; }
        public int? DefaultValueDisplayOrder { get; set; }
        public string AttributeDefaultValue { get; set; }

    }
}

