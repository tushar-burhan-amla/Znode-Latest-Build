namespace Znode.Engine.Admin.ViewModels
{
    public class DefaultGlobalConfigViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string SelectedIds { get; set; }
        public string Action { get; set; }
        public int PortalId { get; set; }
        public string PortalLocaleId { get; set; }
        public string LocaleId { get; set; }
    }
}
