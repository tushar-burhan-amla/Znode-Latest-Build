namespace Znode.Engine.Api.Models
{
    public class RequestStatusModel : BaseModel
    {
        public int RmaRequestStatusId { get; set; }

        public int RmaReasonForReturnId { get; set; }

        public string Name { get; set; }

        public string CustomerNotification { get; set; }

        public string AdminNotification { get; set; }

        public bool? IsActive { get; set; }

        public string Reason { get; set; }

        public bool? IsDefault { get; set; } = false;
        public string RequestCode { get; set; }
    }
}
