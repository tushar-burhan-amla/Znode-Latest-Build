using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishStateService
    {
        /// <summary>
        /// Gets a list of all available publish state to application type mappings.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with city list.</param>
        /// <param name="filters">Filters to be applied on city list.</param>
        /// <param name="sorts">Sorting to be applied on city list.</param>
        /// <returns>publish state mapping list model.</returns>
        PublishStateMappingListModel GetPublishStateApplicationTypeMappingList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Enable/Disable publish state to application type mapping.
        /// </summary>
        /// <param name="mappingId">PublishStateMappingId to disable.</param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        bool EnableDisableMapping(int mappingId, bool isEnabled);
    }
}
