using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IHighlightService
    {
        /// <summary>
        ///Gets the list of highlights.
        /// </summary>
        /// <param name="expands"> Expands for highlight list.</param>
        /// <param name="filters">Filters for highlight list.</param>
        /// <param name="sorts">Sorts for highlight list.</param>
        /// <param name="page">Paging information about highlight list.</param>
        /// <returns>Highlight list model.</returns>
        HighlightListModel GetHighlightList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Creates an highlight.
        /// </summary>
        /// <param name="highlightModel">Highlight model to be created.</param>
        /// <returns>Created highlight.</returns>
        HighlightModel CreateHighlight(HighlightModel highlightModel);

        /// <summary>
        /// Gets an highlight according to ID.
        /// </summary>
        /// <param name="highlightId">ID of the highlight.</param>
        /// <param name="filters">Filter to pass.</param>
        /// <param name="productId">ID of the product.</param>
        /// <returns>Highlight of the specified ID.</returns>
        HighlightModel GetHighlight(int highlightId, int productId, FilterCollection filters);

        /// <summary>
        /// Gets an highlight according to Highlight Code.
        /// </summary>
        /// <param name="highlightCode">Code of the highlight.</param>
        /// <param name="filters">Filter to pass.</param>
        /// <returns>Highlight of the specified Code.</returns>
        HighlightModel GetHighlightByCode(string highlightCode, FilterCollection filters);

        /// <summary>
        /// Updates an highlight.
        /// </summary>
        /// <param name="highlightModel">Highlight model to be updated.</param>
        /// <returns>Returns true if model updated sucessfully else return false.</returns>
        bool UpdateHighlight(HighlightModel highlightModel);

        /// <summary>
        /// Delete an highlight.
        /// </summary>
        /// <param name="highlightIds">Highlight Id.</param>
        /// <returns>Returns true if deleted sucessfully else return false.</returns>
        bool DeleteHighlight(ParameterModel highlightIds);

        /// <summary>
        /// Get available highlight code for creating brands. 
        /// </summary>
        /// <param name="attributeCode">Code of attributes</param>
        /// <returns>Return highlight list</returns>
        HighlightListModel GetAvailableHighlightCodes(string attributeCode);

        #region Highlight Rule Type

        /// <summary>
        /// Get Highlight type list.
        /// </summary>
        /// <param name="filters">filtter list</param>
        /// <param name="sorts">sort list</param>
        /// <returns>Returns HighlightTypeListModel </returns>
        /// <returns></returns>
        HighlightTypeListModel GetHighlightTypeList(FilterCollection filters, NameValueCollection sorts);

        #endregion

        #region Highlight Product

        /// <summary>
        /// Associate/Unassociate product from highlight. 
        /// </summary>
        /// <param name="highlightProductModel">Model to Associate new Highlight Product.</param>
        /// <returns>Returns true if product associated sucessfully else return false.</returns>
        bool AssociateAndUnAssociateProduct(HighlightProductModel highlightProductModel);

        #endregion
    }
}
