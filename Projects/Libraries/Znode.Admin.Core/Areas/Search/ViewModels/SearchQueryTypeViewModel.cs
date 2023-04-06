namespace Znode.Engine.Admin.ViewModels
{
    public class SearchQueryTypeViewModel
    {
        public int SearchQueryTypeId { get; set; }
        public string ParentSearchQueryTypeId { get; set; }
        public string QueryTypeName { get; set; }
        public string QueryBuilderClassName { get; set; }
        public string HelpDescription { get; set; }
    }
}
