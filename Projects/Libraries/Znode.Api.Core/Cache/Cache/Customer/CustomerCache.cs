using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class CustomerCache : BaseCache, ICustomerCache
    {
        #region Private Variable
        private readonly ICustomerService _service;
        #endregion

        #region Constructor
        public CustomerCache(ICustomerService customerService)
        {
            _service = customerService;
        }
        #endregion

        #region Public Methods
        #region Profile Association
        //Get a list of unassociated profile list.
        public virtual string GetUnAssociatedProfileList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get profile list
                ProfileListModel list = _service.GetUnAssociatedProfileList(Expands, Filters, Sorts, Page);
                if (list?.Profiles?.Count > 0)
                {
                    //Create response.
                    ProfileListResponse response = new ProfileListResponse { Profiles = list.Profiles };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of associated profiles based on customers.
        public virtual string GetAssociatedProfileList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get profile list
                ProfileListModel list = _service.GetAssociatedProfileList(Expands, Filters, Sorts, Page);
                if (HelperUtility.IsNotNull(list?.Profiles))
                {
                    //Create response.
                    ProfileListResponse response = new ProfileListResponse { Profiles = list.Profiles, CustomerName = list.CustomerName, AccountId = list.AccountId };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of associated profiles based on profile.
        public virtual string GetCustomerPortalProfilelist(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get profile list
                ProfileListModel list = _service.GetCustomerPortalProfilelist(Expands, Filters, Sorts, Page);
                if (HelperUtility.IsNotNull(list?.Profiles))
                {
                    //Create response.
                    ProfileListResponse response = new ProfileListResponse { Profiles = list.Profiles, CustomerName = list.CustomerName, AccountId = list.AccountId };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Affiliate
        //Get list of referral commission type.
        public virtual string GetReferralCommissionTypeList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get list of referral commission type.
                ReferralCommissionTypeListModel list = _service.GetReferralCommissionTypeList(Expands, Filters, Sorts, Page);
                if (list?.ReferralCommissionTypes?.Count > 0)
                {
                    //Create response.
                    ReferralCommissionListResponse response = new ReferralCommissionListResponse { ReferralCommissionTypes = list.ReferralCommissionTypes };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get customer affiliate data.
        public virtual string GetCustomerAffiliate(int userId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get customer affiliate data.
                ReferralCommissionModel list = _service.GetCustomerAffiliate(userId, Expands);
                if (HelperUtility.IsNotNull(list))
                {
                    //Create response.
                    ReferralCommissionResponse response = new ReferralCommissionResponse { ReferralCommission = list };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of referral commission for user.
        public virtual string GetReferralCommissionList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get list of referral commission for user.
                ReferralCommissionListModel list = _service.GetReferralCommissionList(Expands, Filters, Sorts, Page);
                if (list?.ReferralCommissions?.Count > 0)
                {
                    //Create response.
                    ReferralCommissionListResponse response = new ReferralCommissionListResponse { ReferralCommissions = list.ReferralCommissions };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Address
        public virtual string GetAddressList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Account list
                AddressListModel customers = _service.GetAddressList(Expands, Filters, Sorts, Page);
                if (customers?.AddressList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AddressListResponse response = new AddressListResponse { AddressList = customers.AddressList };
                    response.MapPagingDataFromModel(customers);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetCustomerAddress(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                AddressModel address = _service.GetCustomerAddress(Filters, Expands);
                if (HelperUtility.IsNotNull(address))
                {
                    //Get response and insert it into cache.
                    AddressResponse response = new AddressResponse { Address = address };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }


        //Get list of search locations.
        public string GetSearchLocation(int portalId, string searchTerm, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get search locations
                AddressListModel customers = _service.GetSearchLocation(portalId, searchTerm);
                if (customers?.AddressList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    AddressListResponse response = new AddressListResponse { AddressList = customers.AddressList };
                    response.MapPagingDataFromModel(customers);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        
        #endregion
    }
}