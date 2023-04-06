namespace Znode.Engine.Admin.ViewModels
{
    public class CloudflareErrorViewModel : BaseViewModel
    {
        public int DomainId { get; set; }
        public bool Status { get; set; }
    }
}