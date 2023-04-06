namespace Znode.Engine.Api.Models
{
    public class OrderNotesModel : BaseModel
    {
        public int OmsNotesId { get; set; }
        public int? OmsOrderDetailsId { get; set; }
        public int? OmsQuoteId { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
    }
}
