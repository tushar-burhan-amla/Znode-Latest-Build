namespace Znode.Engine.Admin.ViewModels
{
    public class NavigationViewModel
    {
        public string Controller { get; set; }
        public string EditAction { get; set; }
        public string DeleteAction { get; set; }
        public string DetailAction { get; set; }
        public string AreaName { get; set; }
        public string ID { get; set; }
        public string NextID { get; set; }
        public string PreviousID { get; set; }
        public int? CurrentIndex { get; set; }
        public int TotalCount { get; set; }
        public string PreviousQueryString { get; set; }
        public string EditDeleteQueryString { get; set; }
        public string NextQueryString { get; set; }
    }
}