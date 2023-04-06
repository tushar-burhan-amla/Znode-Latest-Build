using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class ShippingCache : BaseCache, IShippingCache
    {
        #region Global Variable
        private readonly IShippingService _service;
        #endregion

        #region Default Constructor
        public ShippingCache(IShippingService shippingService)
        {
            _service = shippingService;
        }
        #endregion

        #region Public Methods

        //Get shipping by shipping Id.
        public virtual string GetShipping(int shippingId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get shipping by shipping id.
                ShippingModel shipping = _service.GetShipping(shippingId);
                if (!Equals(shipping, null))
                {
                    ShippingResponse response = new ShippingResponse { Shipping = shipping };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get shipping list.
        public virtual string GetShippingList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //shipping list
                ShippingListModel shippingList = _service.GetShippingList(Expands, Filters, Sorts, Page);
                if (shippingList?.ShippingList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    ShippingListResponse response = new ShippingListResponse { ShippingList = shippingList.ShippingList };
                    response.MapPagingDataFromModel(shippingList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #region Shipping SKU


        public virtual string GetShippingSKUList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ShippingSKUListModel list = _service.GetShippingSKUList(Expands, Filters, Sorts, Page);
                if (list?.ShippingSKUList?.Count > 0)
                {
                    ShippingSKUListResponse response = new ShippingSKUListResponse { ShippingSKUList = list.ShippingSKUList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion

        #region Shipping Service Code

        //Get shipping service code by Id.
        public virtual string GetShippingServiceCode(int shippingServiceCodeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get shipping service code by id.
                ShippingServiceCodeModel shippingServiceCode = _service.GetShippingServiceCode(shippingServiceCodeId);
                if (!Equals(shippingServiceCode, null))
                {
                    ShippingServiceCodeResponse response = new ShippingServiceCodeResponse { ShippingServiceCode = shippingServiceCode };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get shipping service code list.
        public virtual string GetShippingServiceCodes(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //shipping service code list.
                ShippingServiceCodeListModel shippingServiceCodeList = _service.GetShippingServiceCodeList(Expands, Filters, Sorts, Page);
                if (shippingServiceCodeList?.ShippingServiceCodes?.Count > 0)
                {
                    //Get response and insert it into cache.
                    ShippingServiceCodeListResponse response = new ShippingServiceCodeListResponse { ShippingServiceCodes = shippingServiceCodeList.ShippingServiceCodes };
                    response.MapPagingDataFromModel(shippingServiceCodeList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Shipping Rule

        public virtual string GetShippingRule(int shippingRuleId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ShippingRuleModel shippingRule = _service.GetShippingRule(shippingRuleId);
                if (IsNotNull(shippingRule))
                {
                    ShippingRuleResponse response = new ShippingRuleResponse { ShippingRule = shippingRule };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetShippingRuleList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ShippingRuleListModel list = _service.GetShippingRuleList(Expands, Filters, Sorts, Page);
                if (list?.ShippingRuleList?.Count > 0)
                {
                    ShippingRuleListResponse response = new ShippingRuleListResponse { ShippingRuleList = list.ShippingRuleList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion

        #region Shipping Rule Type

        //Get shipping rule type list.
        public virtual string GetShippingRuleTypeList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //shipping rule type list.
                ShippingRuleTypeListModel shippingRuleTypeList = _service.GetShippingRuleTypeList(Filters, Sorts);
                if (shippingRuleTypeList?.ShippingRuleTypeList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    ShippingRuleTypeListResponse response = new ShippingRuleTypeListResponse { ShippingRuleTypeList = shippingRuleTypeList.ShippingRuleTypeList };
                    response.MapPagingDataFromModel(shippingRuleTypeList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Portal/Profile Shipping
        //Get associated shipping list for Portal/Profile.
        public virtual string GetAssociatedShippingList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ShippingListModel list = _service.GetAssociatedShippingList(Expands, Filters, Sorts, Page);
                if (IsNotNull(list))
                {
                    ShippingListResponse response = new ShippingListResponse { ShippingList = list.ShippingList, ProfileName = list.ProfileName, PortalName = list.PortalName };

                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of unassociated shipping list for Portal/Profile.
        public virtual string GetUnGetAssociatedShippingList(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ShippingListModel list = _service.GetUnAssociatedShippingList(null, Filters, Sorts, Page);
                if (list?.ShippingList?.Count > 0)
                {
                    //Create Response
                    ShippingListResponse response = new ShippingListResponse { ShippingList = list.ShippingList };

                    //Apply Pagination Parameters
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
        #endregion
    }
}