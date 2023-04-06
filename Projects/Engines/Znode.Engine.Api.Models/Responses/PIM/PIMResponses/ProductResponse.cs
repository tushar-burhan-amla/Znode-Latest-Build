namespace Znode.Engine.Api.Models.Responses
{
    public class ProductResponse : BaseResponse
    {
        public ProductModel Product { get; set; }
        public bool IsSuccess { get; set; }
    }
}
