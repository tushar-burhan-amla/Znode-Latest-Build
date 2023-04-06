using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Api.Models;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Maps
{
    public class OrderViewModelMap
    {
        // Bind portal list.
        public static List<SelectListItem> ToListItems(IEnumerable<PortalModel> model)
        {
            List<SelectListItem> portals = new List<SelectListItem>();
            if (IsNotNull(model))
            {
                portals = (from item in model
                           select new SelectListItem
                           {
                               Text = item.StoreName,
                               Value = item.PortalId.ToString(),
                           }).ToList();
            }
            return portals;
        }

        // Bind catalog list.
        public static List<SelectListItem> ToListItems(IEnumerable<PortalCatalogModel> model)
        {
            List<SelectListItem> catalog = new List<SelectListItem>();
            if (IsNotNull(model))
            {
                catalog = (from item in model
                           select new SelectListItem
                           {
                               Text = item.CatalogName,
                               Value = item.PortalId.ToString(),
                           }).ToList();
            }
            return catalog;
        }

        // Bind account list.
        public static List<SelectListItem> ToListItems(IEnumerable<AccountModel> model)
        {
            List<SelectListItem> accounts = new List<SelectListItem>();
            if (IsNotNull(model))
            {
                accounts = (from item in model
                            select new SelectListItem
                            {
                                Text = item.Name,
                                Value = item.AccountId.ToString(),
                            }).ToList();
            }
            accounts.Insert(0, new SelectListItem { Text = AdminConstants.SelectAccount, Value = AdminConstants.Zero.ToString() });

            return accounts;
        }

        //Bind address list.
        public static List<SelectListItem> ToListItems(IEnumerable<AddressModel> model)
        {
            if (IsNotNull(model))
            {
                List<SelectListItem> addressNames = new List<SelectListItem>();
                if (IsNotNull(model))
                {
                    addressNames = (from item in model
                                    select new SelectListItem
                                    {
                                        Text = item.DisplayName,
                                        Value = item.AddressId.ToString(),
                                    }).ToList();
                }

                return addressNames;
            }
            return new List<SelectListItem>();
        }
    }
}