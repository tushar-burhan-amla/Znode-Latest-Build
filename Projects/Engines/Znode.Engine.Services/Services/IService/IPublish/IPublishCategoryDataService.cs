using System.Data;

namespace Znode.Engine.Services
{
    public interface IPublishCategoryDataService
    {
        /// <summary>
        /// Perform single category publish operation.
        /// </summary>
        /// <param name="pimCategoryId">pimCategoryId</param>
        /// <param name="pimCatalogId">pimCatalogId</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="status">status</param>
        /// <returns></returns>
        DataSet ProcessSingleCategoryPublish(int pimCategoryId, int pimCatalogId, string revisionType, out bool status, out int isAssociate);
    }
}
