using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Helpers;

namespace Znode.Engine.Admin.Maps
{
    public static class AccountViewModelMap
    {
        //Convert Enumerable list of Order state model to select lit items.
        public static List<SelectListItem> ToOrderStateSelectListItemList(IEnumerable<OrderStateModel> orderStateList, int omsQuoteId)
        {
            List<SelectListItem> orderStateItems = new List<SelectListItem>();
            if (orderStateList?.Count() > 0)
            {
                orderStateItems = (from item in orderStateList
                                   orderby item.Description ascending
                                   select new SelectListItem
                                   {
                                       Text = item.Description,
                                       Value = item.OrderStateId.ToString()
                                   }).ToList();


                //Remove Ordered status from list.
                orderStateItems.RemoveAll(x => string.Equals(x.Text, AdminConstants.Ordered, System.StringComparison.CurrentCultureIgnoreCase));

                //Remove Draft status from list.
                orderStateItems.RemoveAll(x => string.Equals(x.Text, AdminConstants.Draft, System.StringComparison.CurrentCultureIgnoreCase));
            }
            return orderStateItems;
        }
    }
}