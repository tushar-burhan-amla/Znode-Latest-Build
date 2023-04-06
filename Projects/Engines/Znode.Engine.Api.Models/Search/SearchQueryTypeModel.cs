namespace Znode.Engine.Api.Models
{
    public class SearchQueryTypeModel : BaseModel
    {
        public int SearchQueryTypeId { get; set; }
        public string ParentSearchQueryTypeId { get; set; }
        public string QueryTypeName { get; set; }
        public string QueryBuilderClassName { get; set; }
        public string HelpDescription { get; set; }
    }
}
