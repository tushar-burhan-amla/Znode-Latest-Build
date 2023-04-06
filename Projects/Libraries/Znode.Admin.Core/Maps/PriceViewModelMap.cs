using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Maps
{
    public class PriceViewModelMap
    {
        // This method maps IEnumerable type list of PIMAttributeFamilyModel to SelectListItem type list of PIMAttributeFamilyViewModel.
        public static List<SelectListItem> ToListItems(IEnumerable<CurrencyModel> model, int currencyId)
        {
            List<SelectListItem> currency = new List<SelectListItem>();

            if (HelperUtility.IsNotNull(model))
            {
                //Set default currency on top in dropdown as per in Global setting.
                List<CurrencyModel> currencyList = SetDefaultCurrency(model);
                currency = (from item in currencyList
                            select new SelectListItem
                            {
                                Text = item.CurrencyName,
                                Value = item.CurrencyId.ToString(),
                                Selected = item.CultureId == currencyId ? true : false
                            }).ToList();
            }
            return currency;
        }

        public static List<SelectListItem> ToListItems(IEnumerable<CultureModel> model, int cultureId)
        {
            List<SelectListItem> culture = new List<SelectListItem>();

            if (HelperUtility.IsNotNull(model))
            {
                //Set default currency on top in dropdown as per in Global setting.
                List<CultureModel> cultureList = SetDefaultCulture(model, cultureId);
                culture = (from item in cultureList
                           select new SelectListItem
                           {
                               Text = $"{item.CultureName} - {item.Symbol}",
                               Value = item.CultureId.ToString(),
                               Selected = item.CultureId == cultureId ? true : false
                           }).ToList();
            }
            return culture;
        }

        //Method to set default currency on top in dropdown as per in Global setting.
        private static List<CurrencyModel> SetDefaultCurrency(IEnumerable<CurrencyModel> model)
        {
            List<CurrencyModel> currencyList = model.ToList();
            //Get default currency id.
            int defaultCurrencyId = currencyList.FindIndex(x => x.CurrencyId == currencyList.FirstOrDefault(i => i.IsDefault == true)?.CurrencyId);
            CurrencyModel defaultCurrency = currencyList[defaultCurrencyId];
            currencyList[defaultCurrencyId] = currencyList[0];
            currencyList[0] = defaultCurrency;
            return currencyList;
        }

        private static List<CultureModel> SetDefaultCulture(IEnumerable<CultureModel> model, int cultureId)
        {
            List<CultureModel> cultureList = model.ToList();
            //Get default currency id.
            int defaultCultureId = cultureId > 0 ? cultureList.FindIndex(x => x.CultureId == cultureList.FirstOrDefault(y => y.CultureId == cultureId)?.CultureId)
                : cultureList.FindIndex(x => x.CultureId == cultureList.FirstOrDefault(i => i.IsDefault == true)?.CultureId);
            if (defaultCultureId > 0)
            {
                CultureModel defaultCulture = cultureList[defaultCultureId];
                cultureList[defaultCultureId] = cultureList[0];
                cultureList[0] = defaultCulture;
            }
            return cultureList;
        }

        //Mapping ID of PriceList which is going to share and IDs of store with whom price list is going to share with list of PricePortalModel.
        public static PricePortalListModel ToAssociateStoreListModel(int priceListId, string storeIds)
        {
            if (!string.IsNullOrEmpty(storeIds))
            {
                PricePortalListModel listModel = new PricePortalListModel { PricePortalList = new List<PricePortalModel>() };
                foreach (string storeId in storeIds.Split(','))
                    listModel.PricePortalList.Add(new PricePortalModel { PriceListId = priceListId, PortalId = Convert.ToInt32(storeId), Precedence = 999 });
                return listModel;
            }
            return new PricePortalListModel();
        }

        //Mapping ID of PriceList which is going to share and IDs of store with whom price list is going to share with list of PricePortalModel.
        public static PricePortalListModel ToAssociatePriceListModelToStore(int storeId, string priceListIds)
        {
            if (!string.IsNullOrEmpty(priceListIds))
            {
                PricePortalListModel listModel = new PricePortalListModel { PricePortalList = new List<PricePortalModel>() };
                foreach (string priceListId in priceListIds.Split(','))
                    listModel.PricePortalList.Add(new PricePortalModel { PriceListId = Convert.ToInt32(priceListId), PortalId = storeId });
                return listModel;
            }
            return new PricePortalListModel();
        }

        //Mapping ID of PriceList which is going to share and IDs of store with whom price list is going to share with list of PricePortalModel.
        public static PriceProfileListModel ToAssociatePriceListModelToProfile(int profileId, string priceListIds, int portalId)
        {
            if (!string.IsNullOrEmpty(priceListIds))
            {
                PriceProfileListModel listModel = new PriceProfileListModel { PriceProfileList = new List<PriceProfileModel>() };
                foreach (string priceListId in priceListIds.Split(','))
                    listModel.PriceProfileList.Add(new PriceProfileModel { PortalId = portalId, PriceListId = Convert.ToInt32(priceListId), PortalProfileId = profileId });
                return listModel;
            }
            return new PriceProfileListModel();
        }

        //Mapping ID of PriceList which is going to share and IDs of profile with whom price list is going to share with list of PriceProfileModel.
        public static PriceProfileListModel ToAssociateProfileListModel(int priceListId, string profileIds)
        {
            if (!string.IsNullOrEmpty(profileIds))
            {
                PriceProfileListModel listModel = new PriceProfileListModel { PriceProfileList = new List<PriceProfileModel>() };
                foreach (string profileId in profileIds.Split(','))
                    listModel.PriceProfileList.Add(new PriceProfileModel { PriceListId = priceListId, ProfileId = Convert.ToInt32(profileId), Precedence = 999 });
                return listModel;
            }
            return new PriceProfileListModel();
        }

        //Mapping ID of PriceList which is going to share and IDs of customer with whom price list is going to share with list of PriceAccountModel.
        public static PriceUserListModel ToAssociateCustomerListModel(int priceListId, string customerIds)
        {
            if (!string.IsNullOrEmpty(customerIds))
            {
                PriceUserListModel listModel = new PriceUserListModel { PriceUserList = new List<PriceUserModel>() };
                //
                foreach (string customerId in customerIds.Split(','))
                    listModel.PriceUserList.Add(new PriceUserModel { PriceListId = priceListId, UserId = Convert.ToInt32(customerId), Precedence = 999 });
                return listModel;
            }
            return new PriceUserListModel();
        }

        //Mapping ID of PriceList which is going to share and IDs of account with whom price list is going to share with list of PriceAccountModel.
        public static PriceAccountListModel ToAssociateAccountListModel(int priceListId, string accountIds)
        {
            if (!string.IsNullOrEmpty(accountIds))
            {
                PriceAccountListModel listModel = new PriceAccountListModel { PriceAccountList = new List<PriceAccountModel>() };
                foreach (string accountId in accountIds.Split(','))
                    listModel.PriceAccountList.Add(new PriceAccountModel { PriceListId = priceListId, AccountId = Convert.ToInt32(accountId), Precedence = 999 });
                return listModel;
            }
            return new PriceAccountListModel();
        }

        public static List<SelectListItem> PriceProfileModelToListItems(IEnumerable<PortalProfileModel> portalProfileModel)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (!Equals(portalProfileModel, null))
            {
                listItems = (from item in portalProfileModel
                             select new SelectListItem
                             {
                                 Text = item.ProfileName,
                                 Value = item.PortalProfileID.ToString()
                             }).ToList();
            }
            listItems.Insert(0, new SelectListItem() { Value = "0", Text = "Select Profile" });
            return listItems;
        }

        public static List<SelectListItem> UOMToListItems(IEnumerable<UomModel> uomModel)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (!Equals(uomModel, null))
            {
                listItems = (from item in uomModel
                             select new SelectListItem
                             {
                                 Text = item.Uom,
                                 Value = item.UomId.ToString()
                             }).ToList();
            }
            return listItems;
        }

        public static List<SelectListItem> PriceListToListItems(IEnumerable<PriceModel> priceModel)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (!Equals(priceModel, null))
            {
                listItems = (from item in priceModel
                             select new SelectListItem
                             {
                                 Text = item.ListCode,
                                 Value = item.PriceListId.ToString()
                             }).ToList();
            }
            return listItems;
        }

        //Map PriceSKUDataViewModel to PriceTierListModel.
        public static PriceTierListModel ToPriceTierListModel(PriceSKUDataViewModel model)
        {
            if (HelperUtility.IsNotNull(model?.PriceTierList))
            {
                PriceTierListModel listModel = new PriceTierListModel { TierPriceList = new List<PriceTierModel>() };
                string quantity = model?.PriceTierList.Select(x => x.Quantity).FirstOrDefault().ToString();
                foreach (var item in model.PriceTierList)
                {
                    if (string.IsNullOrWhiteSpace(quantity))
                        listModel.TierPriceList.Add(new PriceTierModel { PriceListId = model.PriceListId.Value, SKU = model.PriceSKU.SKU });
                    else
                        listModel.TierPriceList.Add(new PriceTierModel { Quantity = item.Quantity.Value, Price = item.Price,Custom1=item.Custom1,Custom2=item.Custom2,Custom3=item.Custom3, PriceTierId = HelperUtility.IsNull(item.PriceTierId) ? 0 : item.PriceTierId.Value, PriceListId = model.PriceListId.Value, SKU = model.PriceSKU.SKU });
                }
                return listModel;
            }
            return new PriceTierListModel();
        }
    }
}