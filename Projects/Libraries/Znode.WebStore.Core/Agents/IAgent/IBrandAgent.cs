using System.Collections.Generic;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IBrandAgent
    {
        /// <summary>
        /// Get Filtered Brand List
        /// </summary>
        /// <param name="brandName">brandName</param>
        /// <returns>list of brands.</returns>
        List<BrandViewModel> BrandList(string brandName=null);

        /// <summary>
        /// Get Brand Details
        /// </summary>
        /// <param name="brandId">brand id.</param>
        /// <returns>brand view model.</returns>
        BrandViewModel GetBrandDetails(int brandId);

        /// <summary>
        /// Sets brand breadcrumb html.
        /// </summary>
        /// <param name="brand">BrandViewModel.</param>
        void SetBreadCrumbHtml(BrandViewModel brand);
    }
}