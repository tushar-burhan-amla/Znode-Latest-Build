namespace Znode.Engine.Admin.ViewModels
{
    public class ImpersonationLogViewModel : BaseViewModel
    {
        public string ImpersonationLogId { get; set; }

        public int PortalId { get; set; }

        public string StoreName { get; set; }

        public int CSRId { get; set; }

        public int ShopperId { get; set; }

        public int ActivityId { get; set; }

        public string CSRName { get; set; }

        public string WebstoreUserName { get; set; }

        public string ActivityType { get; set; }

        public string OperationType { get; set; }

        public string ImpersonationOperationType {
            get ; set;

        }
    }
}
