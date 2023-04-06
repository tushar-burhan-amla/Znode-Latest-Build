namespace Znode.Engine.Api.Cache
{
    public interface IPromotionTypeCache
    {
        /// <summary>
        /// Get the list of all  Promotion types.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>list of all  Promotion type in string format.</returns>
        string GetPromotionTypeList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get a  PromotionType.
        /// </summary>
        /// <param name="promotionTypeId">PromotionType Id to get  PromotionType.</param>
        /// <param name="routeUri">route URL.</param>
        /// <param name="routeTemplate">route Template.</param>
        /// <returns>Returns PromotionType Model in string format.</returns>
        string GetPromotionType(int promotionTypeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get all  Promotion Types which are not present in database.
        /// </summary>
        /// <param name="routeUri">route URL.</param>
        /// <param name="routeTemplate">route Template.</param>
        /// <returns>Return List  Promotion Type.</returns>
        string GetAllPromotionTypesNotInDatabase(string routeUri, string routeTemplate);
    }
}
