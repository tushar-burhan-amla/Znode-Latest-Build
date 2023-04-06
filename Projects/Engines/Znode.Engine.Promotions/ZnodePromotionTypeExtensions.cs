using System.Collections.Generic;
using System.Linq;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    public static class ZnodePromotionTypeExtensions
    {
        /// <summary>
        /// Extension method that converts an IZnodePromotionType list to an IZnodeProviderType list.
        /// </summary>
        /// <param name="list">The list of IZnodePromotionType items.</param>
        /// <returns>A list of IZnodeProviderType items.</returns>
        public static List<IZnodeProviderType> ToProviderTypeList(this List<IZnodePromotionsType> list) 
            => list.Cast<IZnodeProviderType>().ToList();
    }
}