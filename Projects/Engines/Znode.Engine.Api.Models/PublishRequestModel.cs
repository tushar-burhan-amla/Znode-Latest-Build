namespace Znode.Engine.Api.Models
{
    public class PublishRequestModel
    {
        /// <summary>
        /// Portal Id of the portal being published.
        /// </summary>
        public int PortalId { get; set; }

        /// <summary>
        /// Specifies the state to which the content has to be pushed during publish to a non default state.
        /// </summary>
        public string TargetPublishState { get; set; }

        /// <summary>
        /// A comma separated string which specifies what content types have to be included during publish.
        /// </summary>
        public string PublishContent { get; set; }

        /// <summary>
        /// Specifies whether the data saved as draft has to be included during publish.
        /// </summary>
        public bool TakeFromDraftFirst { get; set; }
    }
}
