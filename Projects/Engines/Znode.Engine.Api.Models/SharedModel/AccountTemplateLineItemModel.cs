namespace Znode.Engine.Api.Models
{
    public class AccountTemplateLineItemModel : SavedCartLineItemModel
    {
        public int OmsTemplateLineItemId { get; set; }
        public int? ParentOmsTemplateLineItemId { get; set; }
        public int OmsTemplateId { get; set; }       
    }
}
