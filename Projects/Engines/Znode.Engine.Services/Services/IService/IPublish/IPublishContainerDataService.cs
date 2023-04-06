using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IPublishContainerDataService
    {
        /// <summary>
        /// Publish the content container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="targetPublishState">targetPublishState</param>
        /// <returns></returns>
        PublishedModel PublishContentContainer(string containerKey, string targetPublishState);

        /// <summary>
        /// Publish the content container Variant
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="containerProfileVariantId">containerProfileVariantId</param>
        /// <param name="targetPublishState">targetPublishState</param>
        /// <returns></returns>
        PublishedModel PublishContainerVariant(string containerKey, int containerProfileVariantId, string targetPublishState);
    }
}
