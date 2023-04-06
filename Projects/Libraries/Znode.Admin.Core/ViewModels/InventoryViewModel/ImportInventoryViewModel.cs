namespace Znode.Engine.Admin.ViewModels
{
    public class ImportInventoryViewModel
    {       
        public string SKU { get; set; }
        public string Quantity { get; set; }
        public string ReOrderLevel { get; set; }
        public int RowNumber { get; set; }
        public string ListCode { get; set; }
        public string ListName { get; set; }
        public string Exceptions { get; set; }
    }
}