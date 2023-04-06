namespace Znode.Libraries.ECommerce.Utilities
{
    public enum ZnodeOrderStatusEnum
    {
        /// <summary>
        /// Order state is Submitted
        /// </summary>
        SUBMITTED,

        /// <summary>
        /// Order state is Shipped
        /// </summary>
        SHIPPED,

        /// <summary>
        /// Order state is Returned
        /// </summary>       
        RETURNED,

        /// <summary>
        /// Order state is Cancelled
        /// </summary>
        CANCELED,

        /// <summary>
        /// Order state is Pending Approval.
        /// </summary>
        PENDING_APPROVAL,

        /// <summary>
        /// Quote state is Approved
        /// </summary>
        APPROVED,

        /// <summary>
        /// Quote state is Rejected
        /// </summary>
        REJECTED,

        /// <summary>
        /// Quote state is In Review
        /// </summary>
        IN_REVIEW,

        /// <summary>
        /// Quote state is Draft
        /// </summary>
        DRAFT,

        /// <summary>
        /// Quote state is Ordered
        /// </summary>
        ORDERED,

        /// <summary>
        /// Quote state is Expired
        /// </summary>
        EXPIRED,

        /// <summary>
        /// Order state is failed
        /// </summary>
        FAILED
    }
}
