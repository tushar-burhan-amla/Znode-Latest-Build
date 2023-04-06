namespace Znode.Engine.ABSConnector
{
    public class ABSOrderHeaderModel : ABSRequestBaseModel
    {
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string StartDate { get; set; }
        public string CancelDate { get; set; }
        public string OrderHeaderStatus { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string OrderRemark { get; set; }

        public ABSOrderCommentsModel OrderComments { get; set; }
        public ABSOrderDetailLineModel OrderDetailLine { get; set; }
    }
}
