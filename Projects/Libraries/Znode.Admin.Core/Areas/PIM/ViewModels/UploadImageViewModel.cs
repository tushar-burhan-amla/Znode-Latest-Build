namespace Znode.Engine.Admin.ViewModels
{
    public class UploadImageViewModel
    {
        public int ProductId { get; set; }
        public string AssociatedMediaIds { get; set; }
        public int AttributeId { get; set; }
        public int LastSelectedMediaId { get; set; }
    }
}