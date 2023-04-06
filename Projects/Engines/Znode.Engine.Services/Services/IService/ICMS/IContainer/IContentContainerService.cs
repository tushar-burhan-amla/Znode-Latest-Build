using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IContentContainerService
    {

        /// <summary>
        /// Get the List of Content Containers
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>ContentContainerListModel model</returns>
        ContentContainerListModel List(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create Content Container
        /// </summary>
        /// <param name="model">ContentContainerCreateModel model</param>
        /// <returns>ContentContainerResponseModel model</returns>
        ContentContainerResponseModel Create(ContentContainerCreateModel model);

        /// <summary>
        /// Update Content Container
        /// </summary>
        /// <param name="model">ContentContainerUpdateModel model</param>
        /// <returns>ContentContainerResponseModel</returns>
        ContentContainerResponseModel Update(ContentContainerUpdateModel model);

        /// <summary>
        /// Get Content Container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>ContentContainerResponseModel</returns>
        ContentContainerResponseModel GetContentContainer(string containerKey);

        /// <summary>
        /// Get Associated Variants to a Content Container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>List of AssociatedVariantModel</returns>
        List<AssociatedVariantModel> GetAssociatedVariants(string containerKey);

        /// <summary>
        ///  Associate Variant to a Content Container
        /// </summary>
        /// <param name="variant">AssociatedVariantModel model</param>
        /// <returns>List of AssociatedVariantModel</returns>
        List<AssociatedVariantModel> AssociateVariant(AssociatedVariantModel variant);


        /// <summary>
        /// Delete Associated Variant
        /// </summary>
        /// <param name="variantIds">variantIds</param>
        /// <returns>Status</returns>
        bool DeleteAssociatedVariant(ParameterModel variantIds);

        /// <summary>
        /// Delete Content Container
        /// </summary>
        /// <param name="contentContainerIds">ParameterModel model</param>
        /// <returns>Status</returns>
        bool DeleteContentContainer(ParameterModel contentContainerIds);

        /// <summary>
        /// Delete Content Container by Container Key
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>Status</returns>
        bool DeleteContentContainerByContainerKey(string containerKey);

        /// <summary>
        /// Verify if the Container Key Exist
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>Status</returns>
        bool IsContainerKeyExists(string containerKey);

        /// <summary>
        /// Associate Container Template
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <param name="ContainerTemplateId">ContainerTemplateId</param>
        /// <returns>Status</returns>
        bool AssociateContainerTemplate(int variantId, int containerTemplateId);

        /// <summary>
        /// Get the List of Associated Variants
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>AssociatedVariantListModel model</returns>
        AssociatedVariantListModel GetAssociatedVariantList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate container variant data
        /// </summary>
        ///  <param name="localeId">localId</param>
        /// <param name="templateId">templateId</param>
        /// <param name="variantId">variantId</param>
        /// <param name="isActive">isActive</param>
        /// <returns>Status</returns>
        bool SaveVariantData(int localeId, int? templateId, int variantId, bool isActive);
        
        /// <summary>
        /// Get Content Container Attribute Values based on container variants.
        /// </summary>
        /// <param name="containerKey">Container Key.</param>
        /// <param name="localeId">LocaleId</param>
        /// <param name="portalId">PortalId</param>
        /// <param name="profileId">ProfileId</param>
        /// <returns>Return the Attribute Values.</returns>
        ContentContainerDataModel GetContentContainerData(string containerKey, int localeId, int portalId = 0, int profileId = 0);

        /// <summary>
        /// Get Content Container variant locale data
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <returns>ContentContainerResponseModel</returns>
        ContentContainerResponseModel GetVariantLocaleData(int variantId);

        /// <summary>
        /// Activate/Deactivate Variant
        /// </summary>
        /// <param name="variantIds">variantIds</param>
        /// <param name="isActivate">isActivate</param>
        /// <returns>Status</returns>
        bool ActivateDeactivateVariant(ParameterModel variantIds, bool isActivate);
    }
}
