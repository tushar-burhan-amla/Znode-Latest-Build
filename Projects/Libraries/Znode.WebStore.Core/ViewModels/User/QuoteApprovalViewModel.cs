using System;

namespace Znode.Engine.WebStore.ViewModels
{
    public class QuoteApprovalViewModel : BaseViewModel
    {
        public int OmsQuoteApprovalId { get; set; }
        public int OmsQuoteId { get; set; }
        public int ApproverUserId { get; set; }
        public string Comments { get; set; }
        public string ApproverUserName { get; set; }
        public DateTime CommentModifiedDateTime { get; set; }
    }
}