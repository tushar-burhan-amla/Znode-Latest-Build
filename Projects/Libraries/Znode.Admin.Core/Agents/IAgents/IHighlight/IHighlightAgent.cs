using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Agents
{
    public interface IHighlightAgent
    {
        /// <summary>
        /// Gets the list of all highlights.
        /// </summary>
        /// <param name="filters">Filters for highlights.</param>
        /// <param name="sortCollection">Sorts for highlight.</param>
        /// <param name="pageIndex">Start page index of highlight list.</param>
        /// <param name="recordPerPage">Record per page of highlights.</param>
        /// <returns>List of highlights.</returns>
        HighlightListViewModel Highlights(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null,int localeId=0);

        /// <summary>
        /// Get highlight.
        /// </summary>
        /// <param name="highlightId">Highlight ID of model.</param>
        /// <param name="localeId">Locale id.</param>
        /// <returns>Highlight view model.</returns>
        HighlightViewModel GetHighlight(int highlightId, int localeId);

        /// <summary>
        /// Create an Highlight.
        /// </summary>
        /// <param name="highlightViewModel">View model of the highlight to be created.</param>
        /// <returns>Created highlight.</returns>
        HighlightViewModel CreateHighlight(HighlightViewModel highlightViewModel);

        /// <summary>
        /// Update highlight.
        /// </summary>
        /// <param name="highlightViewModel">Update values of highlight.</param>
        /// <returns>Updated Highlight view model.</returns>
        HighlightViewModel UpdateHighlight(HighlightViewModel highlightViewModel);

        /// <summary>
        /// Delete highlight.
        /// </summary>
        /// <param name="highlightId">Id of the highlight to be deleted.</param>      
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteHighlight(string highlightId);

        /// <summary>
        /// Get Highlight types as List of SelectListItem.
        /// </summary>      
        void GetHighlightTypeList(HighlightViewModel highlightViewModel);

        /// <summary>
        /// Bind Dropdown values for Locale, HighlightType.
        /// </summary>
        /// <param name="highlightViewModel">HighlightViewModel</param>
        void BindDropdownValues(HighlightViewModel highlightViewModel);

        /// <summary>
        /// Bind default values for LocaleID, IsHyperlink and IsActive.
        /// </summary>
        /// <param name="highlightViewModel">HighlightViewModel</param>
        void BindDefaultValues(HighlightViewModel highlightViewModel);

        /// <summary>
        /// Get highlight Product list.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="localeId">Locale ID of the selected Highlight.</param>
        /// <param name="highlightId">highlightId</param>
        /// <param name="highlightCode">highlightCode</param>
        /// <returns>Highlight List ViewModel.</returns>
        HighlightListViewModel GetHighlightProductList(FilterCollectionDataModel model,int localeId, int highlightId, string highlightCode);

        /// <summary>
        /// Get Unassociated Product list.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="highlightCode">highlight Code</param>
        /// <param name="localeId">Locale ID of the selected highlight.</param>
        /// <returns>Highlight List ViewModel.</returns>
        HighlightListViewModel GetUnAssociatedProductList(FilterCollectionDataModel model,int localeId, string highlightCode);

        /// <summary>
        /// Associate highlight to product.
        /// </summary>
        /// <param name="highlightCode">code of highlight</param>
        /// <param name="productIds">productIds</param>
        /// <returns>Returns true if product associated successfully else return false.</returns>
        bool AssociateHighlightProducts(string highlightCode, string productIds);

        /// <summary>
        /// UnAssociate highlight to product.
        /// </summary>
        /// <param name="highlightCode">code of highlight</param>
        /// <param name="productIds">productIds</param>
        /// <returns>return status as true/false.</returns>
        bool UnAssociateHighlightProduct(string highlightCode, string productIds);
    }
}
