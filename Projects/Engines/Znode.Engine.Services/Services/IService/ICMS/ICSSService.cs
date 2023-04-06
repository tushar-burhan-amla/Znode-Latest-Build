using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICSSService
    {
        CSSListModel GetCssListByThemeId(int cmsThemeId, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create new Css
        /// </summary>
        /// <param name="cssModel">CssModel</param>
        /// <returns>CSSModel<returns>
        CSSModel CreateCSS(CSSModel cssModel);

        /// <summary>
        /// Delete css
        /// </summary>
        /// <param name="cssIds">Ids of css.</param>
        /// <returns>true if deleted else false.</returns>
        bool DeleteCSS(ParameterModel cssIds);
    }
}
