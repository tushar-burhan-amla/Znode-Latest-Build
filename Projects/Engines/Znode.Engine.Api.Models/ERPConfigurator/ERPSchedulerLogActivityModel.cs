namespace Znode.Engine.Api.Models
{
    public class ERPSchedulerLogActivityModel : BaseModel
    {
        public string ErrorMessage { get; set; }
        public bool SchedulerStatus { get; set; }
        public int PortalId { get; set; }
        public int ERPTaskSchedulerId { get; set; }
    }
}
