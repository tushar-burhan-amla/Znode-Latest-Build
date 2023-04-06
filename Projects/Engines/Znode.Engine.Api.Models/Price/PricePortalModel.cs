namespace Znode.Engine.Api.Models
{
    public class PricePortalModel : BaseModel
    {
        public int PriceListPortalId { get; set; }
        public int PriceListProfileId { get; set; }
        public int PriceListId { get; set; }
        public int PortalId { get; set; }
        public int ProfileId { get; set; }
        public int PortalProfileId { get; set; }
        public int Precedence { get; set; }
        public string StoreName { get; set; }
        public string CatalogName { get; set; }
        public string Name { get; set; }
    }
}