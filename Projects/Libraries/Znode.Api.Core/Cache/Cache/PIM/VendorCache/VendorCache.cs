using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class VendorCache : BaseCache, IVendorCache
    {
        #region Private Variables
        private readonly IVendorService _service;
        #endregion

        #region Constructor
        public VendorCache(IVendorService vendorService)
        {
            _service = vendorService;
        }
        #endregion

        #region Public Methods       
        //Get a list of vendors.
        public string GetVendors(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get vendors list
                VendorListModel list = _service.GetVendorList(Expands, Filters, Sorts, Page);
                if (list?.Vendors?.Count > 0)
                {
                    //Create response.
                    VendorListResponse response = new VendorListResponse { Vendors = list.Vendors };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get vendor using PimVendorId.
        public string GetVendor(int PimVendorId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                VendorModel vendorModel = _service.GetVendor(PimVendorId);
                if (IsNotNull(vendorModel))
                {
                    VendorResponse response = new VendorResponse { Vendor = vendorModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get vendor code list.
        public string GetVendorCodeList(string attributeCode, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //vendor code list
                VendorListModel list = _service.GetAvailableVendorCodes(attributeCode);
                if (list?.VendorCodes?.Count > 0)
                {
                    //Get response and insert it into cache.
                    VendorListResponse response = new VendorListResponse { VendorCodes = list.VendorCodes };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}