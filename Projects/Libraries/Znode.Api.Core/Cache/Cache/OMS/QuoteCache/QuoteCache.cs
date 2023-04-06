using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class QuoteCache : BaseCache, IQuoteCache
    {
        #region Private Variables
        private readonly IQuoteService _quoteService;
        #endregion

        #region Constructor
        public QuoteCache(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }
        #endregion

        #region Public Methods
        public virtual string GetQuoteList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                QuoteListModel quoteList = _quoteService.GetQuoteList(Filters, Sorts, Page);

                QuoteListResponse response = new QuoteListResponse { QuoteList = quoteList, PortalName = quoteList?.PortalName };
                if (quoteList?.Quotes?.Count > 0)
                {
                    response.MapPagingDataFromModel(quoteList);
                }

                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }


        // Get Quote Receipt details.
        public virtual string GetQuoteReceipt(int quoteId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                QuoteResponseModel quote = _quoteService.GetQuoteReceipt(quoteId);
                if (IsNotNull(quote))
                {
                    QuoteResponse response = new QuoteResponse { QuoteDetails = quote };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetQuoteById(int omsQuoteId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                QuoteResponseModel quote = _quoteService.GetQuoteById(omsQuoteId);
                if (IsNotNull(quote))
                {
                    QuoteDetailResponse response = new QuoteDetailResponse { QuoteDetail = quote };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetQuoteByQuoteNumber(string quoteNumber, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                QuoteResponseModel quote = _quoteService.GetQuoteByQuoteNumber(quoteNumber);
                if (IsNotNull(quote))
                {
                    QuoteDetailResponse response = new QuoteDetailResponse { QuoteDetail = quote };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        
        public virtual string GetQuoteLineItems(int omsQuoteId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                List<QuoteLineItemModel> quoteItemList = _quoteService.GetQuoteLineItems(omsQuoteId);
                QuoteLineItemResponse response = new QuoteLineItemResponse { QuoteLineItems = quoteItemList };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }
        
        #endregion
    }
}
