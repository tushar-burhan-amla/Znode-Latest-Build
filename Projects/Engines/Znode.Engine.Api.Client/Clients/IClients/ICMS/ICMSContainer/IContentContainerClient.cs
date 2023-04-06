using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IContentContainerClient : IBaseClient
    {
        /// <summary>
        /// Get the List of Content Container
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>ContentContainerListModel model</returns>
        ContentContainerListModel List(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create Content Container
        /// </summary>
        /// <param name="model">ContentContainerResponseModel model</param>
        /// <returns>ContentContainerResponseModel model</returns>
        ContentContainerResponseModel Create(ContentContainerCreateModel model);

        /// <summary>
        /// Get Content Container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>ContentContainerResponseModel model</returns>
        ContentContainerResponseModel GetContentContainer(string containerKey);

        /// <summary>
        /// Update Content Container
        /// </summary>
        /// <param name="model">ContentContainerUpdateModel model</param>
        /// <returns>ContentContainerResponseModel model</returns>
        ContentContainerResponseModel Update(ContentContainerUpdateModel model);

        /// <summary>
        /// Get Associated Variants to a Content Container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>List ofAssociatedVariantModel</returns>
        List<AssociatedVariantModel> GetAssociatedVariants(string containerKey);

        /// <summary>
        /// Associate Variant to a Content Container
        /// </summary>
        /// <param name="model">AssociatedVariantModel model</param>
        /// <returns>List of AssociatedVariantModel</returns>
        List<AssociatedVariantModel> AssociateVariant(AssociatedVariantModel model);

        /// <summary>
        /// Delete Associated Variant
        /// </summary>
        /// <param name="variantIds">variantId</param>
        /// <returns>status of deletion</returns>
        bool DeleteAssociatedVariant(ParameterModel variantIds);

        /// <summary>
        /// Delete Content Container
        /// </summary>
        /// <param name="contentContainerIds">contentContainerIds</param>
        /// <returns>status of deletion</returns>
        bool DeleteContentContainer(ParameterModel contentContainerIds);

        /// <summary>
        /// Verify if the Container Key Exist
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>status of presence</returns>
        bool IsContainerKeyExist(string containerKey);

        /// <summary>
        /// Associate Container Template
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <param name="containerTemplateId">containerTemplateId</param>
        /// <returns>status</returns>
        bool AssociateContainerTemplate(int variantId, int containerTemplateId);

        /// <summary>
        /// Get the List of Content Container Variants
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>AssociatedVariantListModel model</returns>
        AssociatedVariantListModel GetAssociatedVariantList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate variants data
        /// </summary>
        /// <param name="localeId">localeId</param>
        /// <param name="templateId">templateId</param>
        /// <param name="variantId">variantId</param>
        /// <param name="isActive">IsActive</param>
        /// <returns>status</returns>
        bool SaveVariantData(int localeId, int? templateId, int variantId, bool isActive);

        /// <summary>
        /// Get Content Container Locale Data
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <returns>ContentContainerResponseModel model</returns>
        ContentContainerResponseModel GetVariantLocaleData(int variantId);

        /// <summary>
        /// Activate/Deactivate Associated Variant
        /// </summary>
        /// <param name="variantIds">variantId</param>
        /// <param name="isActivate">isActivate</param>
        /// <returns>status of activation/deactivation</returns>
        bool ActivateDeactivateVariant(ParameterModel variantIds, bool isActivate);

        /// <summary>
        /// Publish content container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="targetPublishState">targetPublishState</param>
        /// <returns>status if container is published or not along with the error message</returns>
        PublishedModel PublishContentContainer(string containerKey, string targetPublishState);

        /// <summary>
        /// Publish container variant
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="containerProfileVariantId">containerProfileVariantId</param>
        /// <param name="targetPublishState">targetPublishState</param>
        /// <returns>status if variant is published or not along with the error message</returns>
        PublishedModel PublishContainerVariant(string containerKey, int containerProfileVariantId, string targetPublishState);
    }
}
