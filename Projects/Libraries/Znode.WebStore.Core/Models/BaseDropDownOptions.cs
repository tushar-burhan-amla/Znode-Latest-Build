namespace Znode.Engine.WebStore.Models
{
    public class BaseDropDownOptions
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public bool Status { get; set; }
        public bool CustomStatus { get; set; }
        public int PortalPaymentGroupId { get; set; }
        public bool IsSelected { get; set; }
    }
}