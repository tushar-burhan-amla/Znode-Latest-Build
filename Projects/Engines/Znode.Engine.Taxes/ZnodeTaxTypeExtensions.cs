using System.Collections.Generic;
using System.Linq;
using Znode.Libraries.ECommerce.Entities;
using Znode.Engine.Taxes.Interfaces;
namespace Znode.Engine.Taxes
{
    public static class ZnodeTaxTypeExtensions
    {
        // Extension method that converts an IZnodeTaxType list to an IZnodeProviderType list.
        public static List<IZnodeProviderType> ToProviderTypeList(this List<IZnodeTaxesType> list) => list.Cast<IZnodeProviderType>().ToList();
    }
}