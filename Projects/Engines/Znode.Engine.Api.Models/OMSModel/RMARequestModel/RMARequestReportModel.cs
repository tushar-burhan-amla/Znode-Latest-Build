namespace Znode.Engine.Api.Models
{
    public class RMARequestReportModel : BaseModel
    {
        public int OmsOrderDetailsId { get; set; }
        public string CustomerName { get; set; }
        public string BillingEmailId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string SKU { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentEmail { get; set; }
        public string DepartmentAddress { get; set; }
        public string CustomerNotification { get; set; }
        public bool IsEmailNotification { get; set; }
    }
}
