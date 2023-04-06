namespace Znode.Libraries.ECommerce.Utilities
{
    public enum ZnodePublishStatesEnum
    {
        /// <summary>
        /// Entity has just been created and hasn't been changed since.
        /// </summary>
        NOT_PUBLISHED = 1,

        /// <summary>
        /// Entity has pending changes which have not been published.
        /// </summary>
        DRAFT = 2,

        /// <summary>
        /// Entity has been published to production. This means the entity has no pending changes.
        /// </summary>
        PRODUCTION = 3,

        /// <summary>
        /// Entity has been published on stage 1. This means the entity has no pending changes.
        /// </summary>       
        PREVIEW = 4,

        /// <summary>
        /// The recent attempt to publish this entity has failed.
        /// </summary>
        PUBLISH_FAILED = 5,

        /// <summary>
        /// Entity has been published on stage 1. This means the entity is processing.
        /// </summary> 
        PROCESSING = 6 
    }
}
