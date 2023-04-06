namespace Znode.Engine.Admin.ViewModels
{
    public class CouponExportViewModel
    {
        public string Code { get; set; }
        public bool CouponApplied { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsActive { get; set; }
    }
}