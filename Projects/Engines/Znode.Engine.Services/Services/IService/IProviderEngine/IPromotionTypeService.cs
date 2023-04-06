using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPromotionTypeService
    {
        /// <summary>
        /// Get the list of all promotion types.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection to generate orderby clause.</param>
        /// <param name="page">Collection  of paging parameters.</param>
        /// <returns>List of PromotionType Model.</returns>
        PromotionTypeListModel GetPromotionTypeList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get a Promotion Type.
        /// </summary>
        /// <param name="promotionTypeId">ID of PromotionType to get details of perticular PromotionType.</param>
        /// <returns>Returns PromotionType Model.</returns>
        PromotionTypeModel GetPromotionType(int promotionTypeId);

        /// <summary>
        /// Creates a new Promotion Type.
        /// </summary>
        /// <param name="promotionTypeModel">PromotionType Model to create new promotion type.</param>
        /// <returns>Returns newly created PromotionType Model.</returns>
        PromotionTypeModel CreatePromotionType(PromotionTypeModel promotionTypeModel);

        /// <summary>
        /// Update a Promotion Type.
        /// </summary>
        /// <param name="promotionTypeModel">PromotionType Model to update promotion type.</param>
        /// <returns></returns>
        bool UpdatePromotionType(PromotionTypeModel promotionTypeModel);

        /// <summary>
        /// Delete a Promotion Type.
        /// </summary>
        /// <param name="promotionTypeIds">ID of promotion type to delete.</param>
        /// <returns>Returns true if PromotionType deleted else returns false.</returns>
        bool DeletePromotionType(ParameterModel promotionTypeIds);

        /// <summary>
        /// Get all promotion types which are not present in database.
        /// </summary>
        /// <returns>Return List promotion type.</returns>
        PromotionTypeListModel GetAllPromotionTypesNotInDatabase();

        /// <summary>
        /// Bulk enable disable promotion types
        /// </summary>
        /// <param name="promotionTypeIds">Ids of promotion type to enable/disable</param>
        /// <param name="isEnable">true if you want to enable and false if you want disable promotion type.</param>
        /// <returns>true/ false status for success or not.</returns>
        bool EnableDisablePromotionType(ParameterModel promotionTypeIds, bool isEnable);
    }
}
