using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IContentContainerAgent
    {
        /// <summary>
        /// List type of Content Containers
        /// </summary>
        /// <param name="filters">filters</param>
        /// <param name="sortCollection">sortCollection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="recordPerPage">recordPerPage</param>
        /// <returns>ContentContainerListViewModel model</returns>
        ContentContainerListViewModel List(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// List type of Content Containers variants
        /// </summary>
        /// <param name="filters">filters</param>
        /// <param name="sortCollection">sortCollection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="recordPerPage">recordPerPage</param>
        /// <returns>ContainerVariantListViewModel model</returns>
        ContainerVariantListViewModel GetAssociatedVariantList(string containerKey, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Bind the Content Container Model
        /// </summary>
        /// <param name="model">ContentContainerViewModel model</param>
        void BindContentContainerModel(ContentContainerViewModel model);

        /// <summary>
        /// Create Content Container
        /// </summary>
        /// <param name="model">BindDataModel model</param>
        /// <returns>ContentContainerViewModel model</returns>
        ContentContainerViewModel Create(BindDataModel model);

        /// <summary>
        /// Edit Content Container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>ContentContainerViewModel model</returns>
        ContentContainerViewModel Edit(string containerKey);

        /// <summary>
        /// Update Content Container
        /// </summary>
        /// <param name="model">ContentContainerViewModel model</param>
        /// <returns>ContentContainerViewModel model</returns>
        ContentContainerViewModel Update(ContentContainerViewModel model);

        /// <summary>
        /// Get Locale List
        /// </summary>
        /// <returns>List of Available Locales</returns>
        List<SelectListItem> GetAvailableLocales();

        /// <summary>
        /// Get Unassociated Profiles of a Content Container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>List of Unassociated Profiles</returns>
        List<SelectListItem> GetUnassociatedProfiles(string containerKey);

        /// <summary>
        /// Associate Variant to a Content Container
        /// </summary>
        /// <param name="model">ContainerVariantViewModel model</param>
        /// <returns>List of Associated Variants</returns>
        List<ContainerVariantViewModel> AssociateVariant(ContainerVariantViewModel model, out string message);

        /// <summary>
        /// Delete Associated Content Container Variant
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <param name="containerKey">containerKey</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Status of Deletion</returns>
        bool DeleteAssociatedVariant(string variantId, string containerKey, out string errorMessage);

        /// <summary>
        /// Delete Content Container
        /// </summary>
        /// <param name="contentContainerIds">contentContainerIds</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Status of Deletion</returns>
        bool DeleteContentContainer(string contentContainerIds, out string errorMessage);

        /// <summary>
        /// Verify if the Content Container Key Exist
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>Status of presence</returns>
        bool IsContainerExist(string containerKey);

        /// <summary>
        /// Associate Container Template
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="containerTemplateId"></param>
        bool AssociateContainerTemplate(int variantId, int containerTemplateId);

        /// <summary>
        /// Get Portal List
        /// </summary>
        /// <returns>List of Available Portals</returns>
        List<SelectListItem> GetAvailablePortals();

        /// <summary>
        /// Associate Container Template
        /// <param name="model">BindDataModel</param>
        /// <returns>returns Entity Attribute View Model</returns>
        bool SaveAssociatedVariantData([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model);

        /// <summary>
        /// Activate/Deactivate Associated Content Container Variant
        /// </summary>
        /// <param name="containerProfileVariantIds">containerProfileVariantIds</param>
        /// <param name="isActivate">true/false if need to activate/deactivate respectively</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>Status of Deletion</returns>
        bool ActivateDeactivateVariant(string containerProfileVariantIds, bool isActivate, out string errorMessage);

        /// <summary>
        /// Publish content container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="errorMessage">errorMessage</param>
        /// /// <param name="targetPublishState">target publish site(preview or production)</param>
        /// <returns>true/false value if container is published or not.</returns>
        bool PublishContentContainer(string containerKey, out string errorMessage, string targetPublishState = null);

        /// <summary>
        /// Publish content container variant
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="containerProfileVariantId">containerProfileVariantId</param>
        /// <param name="errorMessage">errorMessage</param>
        /// /// <param name="targetPublishState">target publish site(preview or production)</param>
        /// <returns>true/false value if container variant is published or not.</returns>
        bool PublishContainerVariant(string containerKey, int containerProfileVariantId, out string errorMessage, string targetPublishState = null);

    }
}
