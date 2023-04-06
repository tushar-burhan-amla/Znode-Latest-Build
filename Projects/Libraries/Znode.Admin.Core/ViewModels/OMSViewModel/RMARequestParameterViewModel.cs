namespace Znode.Engine.Admin.ViewModels
{
    public class RMARequestParameterViewModel
    {
        public int OmsOrderDetailsId { get; set; }
        public int OMSOrderId { get; set; }
        public int RMARequestID { get; set; }
        public string RequestDate { get; set; }
        public string requestNumber { get; set; }
        public string customerName { get; set; }
        public string orderNumber { get; set; }
        public int PortalId { get; set; }
    }
}