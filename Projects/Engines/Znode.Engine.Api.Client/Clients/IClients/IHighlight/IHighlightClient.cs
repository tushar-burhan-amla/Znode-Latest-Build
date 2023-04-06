using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IHighlightClient : IBaseClient
    {
        /// <summary>
        ///Gets the list of highlights.
        /// </summary>
        /// <param name="expands"> Expands for highlight list.</param>
        /// <param name="filters">Filters for highlight list.</param>
        /// <param name="sorts">Sorts for highlight list.</param>
        /// <returns>Highlight list model.</returns>
        HighlightListModel GetHighlight(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        ///Gets the list of highlights.
        /// </summary>
        /// <param name="expands"> Expands for highlight list.</param>
        /// <param name="filters">Filters for highlight list.</param>
        /// <param name="sorts">Sorts for highlight list.</param>
        /// <param name="pageIndex">Start page index for highlight list.</param>
        /// <param name="pageSize">Page size of highlight list.</param>
        /// <returns>Highlight list model.</returns>
        HighlightListModel GetHighlight(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Creates highlight.
        /// </summary>
        /// <param name="highlightModel">Highlight model to be created.</param>
        /// <returns>Created highlight.</returns>
        HighlightModel CreateHighlight(HighlightModel highlightModel);

        /// <summary>
        /// Gets an highlight according to highlightId.
        /// </summary>
        /// <param name="highlightId">ID of the highlight.</param>
        /// <param name="filters">Filter to pass.</param>
        /// <param name="productId">ID of the product.</param>
        /// <returns>Highlight of the specified ID.</returns>
        HighlightModel GetHighlight(int highlightId, FilterCollection filters, int productId = 0);

        /// <summary>
        /// Gets an highlight according to HighlightCode.
        /// </summary>
        /// <param name="highlightCode">Code of the highlight.</param>
        /// <param name="filters">Filter to pass.</param>
        /// <returns>Highlight of the specified Code.</returns>
        HighlightModel GetHighlightByCode(string highlightCode, FilterCollection filters);

        /// <summary>
        /// Updates highlight.
        /// </summary>
        /// <param name="highlightModel">Highlight model to be updated.</param>
        /// <returns>True if highlight is updated else False.</returns>
        HighlightModel UpdateHighlight(HighlightModel highlightModel);

        /// <summary>
        /// Deletes highlight.
        /// </summary>
        /// <param name="highlightId">highlightId to be deleted.</param>
        /// <returns>True if highlight is deleted else False.</returns>
        bool DeleteHighlight(ParameterModel highlightId);

        /// <summary>
        /// Get highlight type list 
        /// </summary>
        /// <returns>Return highlight type list. </returns>
        HighlightTypeListModel GetHighlightTypeList();

        /// <summary>
        /// Get highlight code list.
        /// </summary>
        /// <param name="attributeCode">Attbute code.</param>       
        /// <returns>Highlight List Model</returns>
        HighlightListModel GetHighlightCodeList(string attributeCode);

        /// <summary>
        /// Associate/Unassociate product from highlight. 
        /// </summary>
        /// <param name="hihglightProductModel">Highlight Product Model</param>
        /// <returns>Returns status as true/false</returns>
        bool AssociateAndUnAssociateProduct(HighlightProductModel highlightProductModel);
    }
}
