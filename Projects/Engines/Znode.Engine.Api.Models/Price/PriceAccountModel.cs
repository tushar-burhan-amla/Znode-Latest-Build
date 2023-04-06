namespace Znode.Engine.Api.Models
{
    public class PriceAccountModel : BaseModel
    {
        public int? PriceListAccountId { get; set; }
        public int PriceListId { get; set; }
        public int AccountId { get; set; }
        public int? Precedence { get; set; }
        public string AccountName { get; set; }
        public string Name { get; set; }
        public string PriceListIds { get; set; }
        public string ExternalId { get; set; }
        public string ParentAccountName { get; set; }
        public string AccountCode { get; set; }
    }
}
