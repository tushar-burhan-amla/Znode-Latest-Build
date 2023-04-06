using System.Collections.Generic;
using System.Linq;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Shipping
{
    public static class ZnodeShippingTypeExtensions
    {
        /// <summary>
        /// Extension method that converts an IZnodeShippingType list to an IZnodeProviderType list.
        /// </summary>
        /// <param name="list">The list of IZnodeShippingType items.</param>
        /// <returns>A list of IZnodeProviderType items.</returns>
        public static List<IZnodeProviderType> ToProviderTypeList(this List<IZnodeShippingsType> list)
        {
            return list.Cast<IZnodeProviderType>().ToList();
        }
    }
}
