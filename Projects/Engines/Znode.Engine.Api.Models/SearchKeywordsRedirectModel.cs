namespace Znode.Engine.Api.Models
{
    public class SearchKeywordsRedirectModel : BaseModel
    {
        public int SearchKeywordsRedirectId { get; set; }
        public int? PublishCatalogId { get; set; }
        public string Keywords { get; set; }
        public string URL { get; set; }
        public int? LocaleId { get; set; }
    }
}
