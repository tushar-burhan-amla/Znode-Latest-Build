namespace Znode.Engine.Api.Models
{
    public class ActivateDeactivateProductsModel
    {
        public string ProductIds { get; set; }
        public bool IsActive { get; set; }
        public int LocaleId { get; set; }
    }
}
