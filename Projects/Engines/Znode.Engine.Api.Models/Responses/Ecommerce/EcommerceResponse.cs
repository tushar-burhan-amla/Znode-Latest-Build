namespace Znode.Engine.Api.Models.Responses
{
    public class EcommerceResponse : BaseResponse
    {
        public PublishCatalogModel PublishCatalog { get; set; }
        public PublishCategoryModel PublishCategory { get; set; }
        public PublishProductModel PublishProduct { get; set; }
    }
}
