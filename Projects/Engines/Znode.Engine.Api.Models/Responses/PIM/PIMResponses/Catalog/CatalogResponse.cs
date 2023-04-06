namespace Znode.Engine.Api.Models.Responses
{
    public class CatalogResponse : BaseResponse
    {
        public CatalogModel Catalog { get; set; }
        public CatalogAssociateCategoryModel AssociateCategory { get; set; }
    }
}
