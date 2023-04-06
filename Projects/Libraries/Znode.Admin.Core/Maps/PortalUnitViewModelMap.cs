using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public class PortalUnitViewModelMap
    {
        // Creates a SelectListItem List for Currency Type
        public static List<SelectListItem> ToSelectListItemsforCurrency(IEnumerable<CurrencyModel> currencyModel)
        {
            List<SelectListItem> currencyItems = new List<SelectListItem>();
            if (!Equals(currencyModel, null))
            {
                currencyItems = (from item in currencyModel
                                     select new SelectListItem
                                     {
                                         Text = item.CurrencyName,
                                         Value = item.CurrencyId.ToString()
                                     }).ToList();
                var currencyId = currencyModel.FirstOrDefault(x => x.IsDefault == true).CurrencyId;
                currencyItems.FirstOrDefault(x => x.Value == currencyId.ToString()).Selected = true;

            }
            return currencyItems;
        }

        // Creates a SelectListItem List for Weight Unit.
        public static List<SelectListItem> ToSelectListItemsforWeight(IEnumerable<WeightUnitModel> weightUnitModel)
        {
            List<SelectListItem> weights = new List<SelectListItem>();
            if (!Equals(weightUnitModel, null))
            {
                weights = (from item in weightUnitModel
                                 select new SelectListItem
                                 {
                                     Text = item.WeightUnitCode,
                                     Value = item.WeightUnitCode
                                 }).ToList();
            }
            return weights;
        }

        // Creates a SelectListItem List for Currency Type
        public static List<SelectListItem> ToSelectListItemsforCurrencyCulture(IEnumerable<CultureModel> cultureModel)
        {
            List<SelectListItem> cultureItems = new List<SelectListItem>();
            if (!Equals(cultureModel, null))
            {
                cultureItems = (from item in cultureModel
                                 select new SelectListItem
                                 {
                                     Text = item.CultureName,
                                     Value = item.CultureId.ToString()
                                 }).ToList();
            }
            return cultureItems;
        }
    }
}