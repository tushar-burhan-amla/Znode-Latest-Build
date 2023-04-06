namespace Znode.Engine.Admin.ViewModels
{
    public class DynamicContentViewModel : BaseViewModel
    {
        public int PortalCustomCssId { get; set; }
        public int PortalId { get; set; }
        public string DynamicCssStyle { get; set; }
        public string WYSIWYGFormatStyle { get; set; }
        public bool IsActive { get; set; } = true;
        public byte PublishStateId { get; set; } = 1;
    }
}
