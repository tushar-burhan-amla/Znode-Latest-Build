namespace Znode.Engine.WebStore.ViewModels
{
    public class SuggestionViewModel:BaseViewModel
    {
        public string SearchText { get; set; }
        public int PublishProductId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
    }
}