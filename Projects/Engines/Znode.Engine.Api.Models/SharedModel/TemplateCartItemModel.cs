namespace Znode.Engine.Api.Models
{
    public class TemplateCartItemModel: ShoppingCartItemModel
    {
        public TemplateCartItemModel()
        {
            ExternalId = System.Guid.NewGuid().ToString();
        }
        public int OmsTemplateLineItemId { get; set; }
        public int? ParentOmsTemplateLineItemId { get; set; }
        public int OmsTemplateId { get; set; }
    }
}
