namespace Znode.Engine.Api.Models
{
    public class UserDashboardPendingOrdersModel : BaseModel
    {
        public int PendingOrdersCount { get; set; }
        public int PendingPaymentsCount { get; set; }
    }
}