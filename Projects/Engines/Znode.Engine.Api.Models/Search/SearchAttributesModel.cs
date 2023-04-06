namespace Znode.Engine.Api.Models
{
    public class SearchAttributesModel : BaseModel
    {
        public string AttributeName { get; set; }
        public string AttributeCode { get; set; }
        public int? BoostValue { get; set; }
        public int SearchProfileId { get; set; }
        public int SearchProfileAttributeMappingId { get; set; }
        public bool IsFacets { get; set; }
        public bool IsUseInSearch { get; set; }
        
        public int DisplayOrder { get; set; }
        public bool IsNgramEnabled { get; set; }
    }
}
