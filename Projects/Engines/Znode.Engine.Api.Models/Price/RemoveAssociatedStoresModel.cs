namespace Znode.Engine.Api.Models
{
    public class RemoveAssociatedStoresModel : BaseModel
    {
        public int PriceListId { get; set; }
        public string PriceListPortalIds { get; set; }
    }
}
