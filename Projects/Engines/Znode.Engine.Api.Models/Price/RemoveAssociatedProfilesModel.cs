namespace Znode.Engine.Api.Models
{
    public class RemoveAssociatedProfilesModel : BaseModel
    {
        public int PriceListId { get; set; }
        public string PriceListProfileIds { get; set; }
    }
}
