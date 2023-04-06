namespace Znode.Engine.Api.Models
{
    public class RMAReturnHistoryModel : BaseModel
    {
        public int RmaReturnHistoryId { get; set; }
        public int RmaReturnDetailsId { get; set; }
        public int? RmaReturnNotesId { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public string Notes { get; set; }
        public string UpdatedBy { get; set; }
        public decimal? ReturnAmount { get; set; }

        public AddressModel BillingAddress { get; set; }

        public AddressModel ShippingAddress { get; set; }
    }
}
