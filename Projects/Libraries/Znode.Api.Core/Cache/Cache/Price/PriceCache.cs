using Newtonsoft.Json;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class PriceCache : BaseCache, IPriceCache
    {
        #region Private Variable
        private readonly IPriceService _service;
        #endregion

        #region Constructor
        public PriceCache(IPriceService priceService)
        {
            _service = priceService;
        }
        #endregion

        #region Price
        //Get Price by Price list id.
        public virtual string GetPrice(int priceListId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PriceModel price = _service.GetPrice(priceListId);
                if (IsNotNull(price))
                {
                    PriceResponse response = new PriceResponse { Price = price };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get a list of Price.
        public virtual string GetPriceList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Price list.
                PriceListModel list = _service.GetPriceList(Expands, Filters, Sorts, Page);
                if (IsNotNull(list?.PriceList))
                {
                    //Create response.
                    PriceListResponse response = new PriceListResponse { PriceList = list.PriceList, HasParentAccounts = list.HasParentAccounts, CustomerName = list.CustomerName };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region SKU Price
        public virtual string GetSKUPrice(int priceId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceSKUModel priceSKU = _service.GetSKUPrice(priceId);
                if (IsNotNull(priceSKU))
                {
                    PriceSKUResponse response = new PriceSKUResponse { PriceSKU = priceSKU };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetSKUPriceList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceSKUListModel list = _service.GetSKUPriceList(Expands, Filters, Sorts, Page);
                if (list?.PriceSKUList?.Count > 0)
                {
                    PriceSKUListResponse response = new PriceSKUListResponse { PriceSKUList = list?.PriceSKUList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get a list of uom.
        public virtual string GetUomList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                UomListModel list = _service.GetUomList(Expands, Filters, Sorts, Page);
                if (list?.UomList?.Count > 0)
                {
                    UomListResponse response = new UomListResponse { UomList = list?.UomList };

                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get paged list of SKU and its Prices
        public virtual string GetPagedPriceSKU(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceSKUListModel priceSKUList = _service.GetPagedPriceSKU(Expands, Filters, Sorts, Page, new System.Data.DataTable());
                if (priceSKUList?.PriceSKUList?.Count > 0)
                {
                    PriceSKUListResponse response = new PriceSKUListResponse { PriceSKUList = priceSKUList?.PriceSKUList };
                    response.MapPagingDataFromModel(priceSKUList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get price by sku.
        public virtual string GetPriceBySku(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceSKUModel priceSKU = _service.GetPriceBySku(Filters);
                if (IsNotNull(priceSKU))
                {
                    PriceSKUResponse response = new PriceSKUResponse { PriceSKU = priceSKU };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get pricing details by product price model.
        public virtual string GetProductPricingDetailsBySku(ProductPriceListSKUDataModel productPriceListSKUDataModel)
        {
            string data = string.Empty;
            if (!string.IsNullOrEmpty(productPriceListSKUDataModel.Sku))
            {
                PriceSKUModel priceSKU = _service.GetProductPricingDetailsBySku(productPriceListSKUDataModel);
                if (IsNotNull(priceSKU))
                {
                    PriceSKUResponse response = new PriceSKUResponse { PriceSKU = priceSKU };
                    data = JsonConvert.SerializeObject(response);
                }
            }
            return data;
        }
        #endregion

        #region Tier Price
        public virtual string GetTierPrice(int priceTierId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceTierModel tierPrice = _service.GetTierPrice(priceTierId);
                if (IsNotNull(tierPrice))
                {
                    TierPriceResponse response = new TierPriceResponse { TierPrice = tierPrice };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetTierPriceList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceTierListModel list = _service.GetTierPriceList(Expands, Filters, Sorts, Page);
                if (list?.TierPriceList?.Count > 0)
                {
                    TierPriceListResponse response = new TierPriceListResponse { TierPriceList = list?.TierPriceList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Associate Store
        public virtual string GetAssociatedStoreList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PricePortalListModel list = _service.GetAssociatedStoreList(Expands, Filters, Sorts, Page);
                if (list?.PricePortalList?.Count > 0)
                {
                    PricePortalListResponse response = new PricePortalListResponse { PricePortalList = list.PricePortalList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetUnAssociatedStoreList(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PortalListModel list = _service.GetUnAssociatedStoreList(Filters, Sorts, Page);
                if (list?.PortalList?.Count > 0)
                {
                    //Create Response
                    PortalListResponse response = new PortalListResponse { PortalList = list.PortalList };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAssociatedStorePrecedence(int priceListPortalId, string routeUri, string routeTemplate)
        {
            //Get data from Cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PricePortalModel pricePortal = _service.GetAssociatedStorePrecedence(priceListPortalId, Expands);
                if (IsNotNull(pricePortal))
                {
                    //Create Response.
                    PricePortalResponse response = new PricePortalResponse { PricePortal = pricePortal };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Associate Profile
        public virtual string GetAssociatedProfileList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceProfileListModel list = _service.GetAssociatedProfileList(Expands, Filters, Sorts, Page);
                if (list?.PriceProfileList?.Count > 0)
                {
                    PriceProfileListResponse response = new PriceProfileListResponse { PriceProfileList = list.PriceProfileList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetUnAssociatedProfileList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ProfileListModel list = _service.GetUnAssociatedProfileList(Filters, Sorts, Page);
                if (list?.Profiles?.Count > 0)
                {
                    ProfileListResponse response = new ProfileListResponse { Profiles = list.Profiles };

                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get associated profile precedence value.
        public virtual string GetAssociatedProfilePrecedence(int priceListProfileId, string routeUri, string routeTemplate)
        {
            //Get data from Cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceProfileModel priceProfile = _service.GetAssociatedProfilePrecedence(priceListProfileId, Expands);
                if (IsNotNull(priceProfile))
                {
                    //Create Response.
                    PriceProfileResponse response = new PriceProfileResponse { PriceProfile = priceProfile };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Associate Customer
        public virtual string GetAssociatedCustomerList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceUserListModel list = _service.GetAssociatedCustomerList(Expands, Filters, Sorts, Page);
                if (list?.PriceUserList?.Count > 0)
                {
                    PriceUserListResponse response = new PriceUserListResponse { PriceUserList = list?.PriceUserList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetUnAssociatedCustomerList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceUserListModel list = _service.GetUnAssociatedCustomerList(Expands, Filters, Sorts, Page);
                if (list?.PriceUserList?.Count > 0)
                {
                    PriceUserListResponse response = new PriceUserListResponse { PriceUserList = list?.PriceUserList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get associated customers precedence value.
        public virtual string GetAssociatedCustomerPrecedence(int priceListUserId, string routeUri, string routeTemplate)
        {
            //Get data from Cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceUserModel priceAccount = _service.GetAssociatedCustomerPrecedence(priceListUserId, Expands);
                if (IsNotNull(priceAccount))
                {
                    //Create Response.
                    PriceUserResponse response = new PriceUserResponse { PriceUser = priceAccount };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Associate Account
        public virtual string GetAssociatedAccountList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceAccountListModel list = _service.GetAssociatedAccountList(Expands, Filters, Sorts, Page);
                if (list?.PriceAccountList?.Count > 0)
                {
                    PriceAccountListResponse response = new PriceAccountListResponse { PriceAccountList = list?.PriceAccountList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetUnAssociatedAccountList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceAccountListModel list = _service.GetUnAssociatedAccountList(Expands, Filters, Sorts, Page);
                if (list?.PriceAccountList?.Count > 0)
                {
                    PriceAccountListResponse response = new PriceAccountListResponse { PriceAccountList = list?.PriceAccountList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get associated account precedence value.
        public virtual string GetAssociatedAccountPrecedence(int priceListUserId, string routeUri, string routeTemplate)
        {
            //Get data from Cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceAccountModel priceAccount = _service.GetAssociatedAccountPrecedence(priceListUserId, Expands);
                if (IsNotNull(priceAccount))
                {
                    //Create Response.
                    PriceAccountResponse response = new PriceAccountResponse { PriceAccount = priceAccount };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Price Management
        public virtual string GetUnAssociatedPriceList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PriceListModel list = _service.GetUnAssociatedPriceList(Expands, Filters, Sorts, Page);
                if (list?.PriceList?.Count > 0)
                {
                    PriceListResponse response = new PriceListResponse { PriceList = list.PriceList };

                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}