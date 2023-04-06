using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PublishCategoryListResponse : BaseListResponse
    {
        public List<PublishCategoryModel> PublishCategories { get; set; }
    }
}
