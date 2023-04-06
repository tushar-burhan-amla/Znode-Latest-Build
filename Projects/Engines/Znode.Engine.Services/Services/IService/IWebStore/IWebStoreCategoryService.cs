using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using ZNode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IWebStoreCategoryService
    {
        /// <summary>
        /// Gets a list of categories.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with category list.</param>
        /// <param name="filters">Filters to be applied on category list.</param>
        /// <param name="sorts">Sorting to be applied on category list.</param>
        /// <returns>Category list model.</returns>
       WebStoreCategoryListModel GetCategories(ParameterModel filterIds);              
    }
}
