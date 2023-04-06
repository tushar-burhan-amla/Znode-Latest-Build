using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class CustomerReviewCache : BaseCache, ICustomerReviewCache
    {
        #region Private Variable
        private readonly ICustomerReviewService _service;
        #endregion

        #region Constructor
        public CustomerReviewCache(ICustomerReviewService customerReviewService)
        {
            _service = customerReviewService;
        }
        #endregion

        #region Public Methods

        //Get Customer Review List.
        public virtual string GetCustomerReviewList(string localeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CustomerReviewListModel list = _service.GetCustomerReviewList(localeId, Filters, Sorts, Page);
                if (list?.CustomerReviewList?.Count > 0)
                {
                    CustomerReviewListResponse response = new CustomerReviewListResponse { CustomerReviewList = list.CustomerReviewList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get Customer Review by Customer Review Id.
        public virtual string GetCustomerReview(int customerReviewId, string localeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CustomerReviewModel customerReview = _service.GetCustomerReview(customerReviewId, localeId);
                if (IsNotNull(customerReview))
                {
                    CustomerReviewResponse response = new CustomerReviewResponse { CustomerReview = customerReview };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}