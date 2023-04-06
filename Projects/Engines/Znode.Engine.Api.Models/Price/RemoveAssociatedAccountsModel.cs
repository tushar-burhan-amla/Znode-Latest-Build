namespace Znode.Engine.Api.Models
{
    public class RemoveAssociatedAccountsModel : BaseModel
    {
        public int PriceListId { get; set; }
        public string PriceListAccountIds { get; set; }
    }
}
