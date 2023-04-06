using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class CategoryServiceV2 : CategoryService, ICategoryServiceV2
    {
        public virtual CategoryProductListModelV2 GetCategoryProducts(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string requiredAttributes)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            PublishProductListModel productList = ZnodeDependencyResolver.GetService<IPublishProductServiceV2>().GetPublishProductListV2(expands, filters, sorts, page, requiredAttributes);
            ZnodeLogging.LogMessage("PublishProducts list count: ", string.Empty, TraceLevel.Verbose, productList?.PublishProducts?.Count);
            return new CategoryProductListModelV2 { CategoryProducts = productList.PublishProducts.Select(x => x.ToEntity<CategoryProductModelV2>()).ToList() };
        }
    }
}