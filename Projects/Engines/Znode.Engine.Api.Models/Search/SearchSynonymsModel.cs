namespace Znode.Engine.Api.Models
{
    public class SearchSynonymsModel : BaseModel
    {
        public int SearchSynonymsId { get; set; }
        public int PublishCatalogId { get; set; }
        public string OriginalTerm { get; set; }
        public string ReplacedBy { get; set; }
        public bool IsBidirectional { get; set; }

        // Synonym code is unique identification for Synonym
        public string SynonymCode { get; set; }
    }
}
