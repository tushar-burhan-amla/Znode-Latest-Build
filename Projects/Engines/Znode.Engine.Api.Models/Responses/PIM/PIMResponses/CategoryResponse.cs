namespace Znode.Engine.Api.Models.Responses
{
    public class CategoryResponse : BaseResponse
    {
        public CategoryModel Category { get; set; }
        public CategoryValuesListModel CategoryValues { get; set; }
    }
}
