namespace Znode.Engine.Api.Models
{
    public class ProfileCatalogModel : BaseModel
    {
        public int ProfileId { get; set; }
        public int PimCatalogId { get; set; }
        public string CatalogName { get; set; }       
    }
}
