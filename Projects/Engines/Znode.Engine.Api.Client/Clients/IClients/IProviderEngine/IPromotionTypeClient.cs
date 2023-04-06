using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IPromotionTypeClient : IBaseClient
    {
        /// <summary>
        /// Get the list of promotion type.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns Promotion Type list model.</returns>
        PromotionTypeListModel GetPromotionTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get a promotion type.
        /// </summary>
        /// <param name="promotionTypeId">ID of PromotionType to get details of perticular PromotionType.</param>
        /// <returns>Returns promotion type model.</returns>
        PromotionTypeModel GetPromotionType(int promotionTypeId);

        /// <summary>
        /// Create new promotion type.
        /// </summary>
        /// <param name="promotionTypeModel">PromotionType Model.</param>
        /// <returns>Returns newly created PromotionType Model.</returns>
        PromotionTypeModel CreatePromotionType(PromotionTypeModel promotionTypeModel);

        /// <summary>
        /// Update a Promotion Type.
        /// </summary>
        /// <param name="promotionTypeModel">PromotionType Model.</param>
        /// <returns>Returns true if PromotionType updated else returns false.</returns>
        PromotionTypeModel UpdatePromotionType(PromotionTypeModel promotionTypeModel);

        /// <summary>
        /// Delete a Promotion Type.
        /// </summary>
        /// <param name="entityIds">ID of promotion type to delete.</param>
        /// <returns>Returns true if PromotionType deleted else returns false.</returns>
        bool DeletePromotionType(ParameterModel entityIds);

        /// <summary>
        /// Get all promotion Types which are not present in database.
        /// </summary>
        /// <returns>Returns Promotion Type list ViewModel which are not in database.</returns>
        PromotionTypeListModel GetAllPromotionTypesNotInDatabase();

        /// <summary>
        /// Bulk enable disable promotion types
        /// </summary>
        /// <param name="entityIds">Ids of taxrule type to enable/disable</param>
        /// <param name="isEnable">true if you want to enable and false if you want disable promotion type.</param>
        /// <returns>true/ false status for success or not.</returns>
        bool BulkEnableDisablePromotionTypes(ParameterModel entityIds, bool isEnable);
    }
}
