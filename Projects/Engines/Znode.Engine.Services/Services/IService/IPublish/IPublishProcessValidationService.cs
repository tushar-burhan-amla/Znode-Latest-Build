
namespace Znode.Engine.Services
{
    public interface IPublishProcessValidationService
    {
        /// <summary>
        /// Check any other catalog is in progress or not.
        /// </summary>
        /// <returns>True if any catalog is progress, other wise false </returns>
        bool IsCatalogPublishInProgress();

        /// <summary>
        /// Check export status
        /// </summary>
        /// <returns>bool</returns>
        bool IsExportPublishInProgress();
    }
}
