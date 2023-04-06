namespace Znode.Engine.Admin.ViewModels
{
    public class BoostDataViewModel : BaseViewModel
    {
        public decimal Boost { get; set; }
        public int PublishProductId { get; set; }
        public int PublishCategoryId { get; set; }
        public string PropertyName { get; set; }
        public string BoostType { get; set; }
        public int ID { get; set; }
        public int CatalogId { get; set; }
    }
}