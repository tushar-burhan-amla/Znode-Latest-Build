using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class PaymentSettingCache : BaseCache, IPaymentSettingCache
    {
        #region Private Variable
        private readonly IPaymentSettingService _service;
        #endregion

        #region Constructor
        public PaymentSettingCache(IPaymentSettingService paymentSettingService)
        {
            _service = paymentSettingService;
        }
        #endregion

        #region Public Methods

        //Get list of Payment Settings.
        public virtual string GetPaymentSettings(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get GiftCard list.
                PaymentSettingListModel list = _service.GetPaymentSettingList(Expands, Filters, Sorts, Page);
                if (list?.PaymentSettings?.Count > 0)
                {
                    //Create response.
                    PaymentSettingListResponse response = new PaymentSettingListResponse { PaymentSettings = list.PaymentSettings };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Payment Setting by Payment Setting id.
        public virtual string GetPaymentSetting(int paymentSettingId, string routeUri, string routeTemplate, int portalId = 0)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PaymentSettingModel paymentSettingModel = _service.GetPaymentSetting(paymentSettingId, Expands, portalId);
                if (IsNotNull(paymentSettingModel))
                {
                    PaymentSettingResponse response = new PaymentSettingResponse { PaymentSetting = paymentSettingModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of Payment Settings.
        public virtual string GetPaymentSettingByUserDetails(string routeUri, string routeTemplate, UserPaymentSettingModel userPaymentSettingModel)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get PaymentSetting list.
                PaymentSettingListModel paymentSettingList = _service.GetPaymentSettingByUserDetails(userPaymentSettingModel);
                if (paymentSettingList?.PaymentSettings?.Count > 0)
                {
                    //Create response.
                    PaymentSettingListResponse response = new PaymentSettingListResponse { PaymentSettings = paymentSettingList.PaymentSettings };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(paymentSettingList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
