using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class GiftCardCache : BaseCache, IGiftCardCache
    {
        #region Private Variable
        private readonly IGiftCardService _service;
        #endregion

        #region Constructor
        public GiftCardCache(IGiftCardService giftCardService)
        {
            _service = giftCardService;
        }
        #endregion

        #region Public Methods

        //Get list of GiftCard.
        public virtual string GetGiftCardList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get GiftCard list.
                GiftCardListModel list = _service.GetGiftCardList(Expands, Filters, Sorts, Page);
                if (IsNotNull(list))
                {
                    //Create response.
                    GiftCardListResponse response = new GiftCardListResponse { GiftCardList = list.GiftCardList, ReferralCommissionCount = list.ReferralCommissionCount };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get GiftCard by gift card id.
        public virtual string GetGiftCard(int giftCardId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                GiftCardModel giftCardModel = _service.GetGiftCard(giftCardId);
                if (IsNotNull(giftCardModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new GiftCardResponse { GiftCard = giftCardModel });
            }
            return data;
        }

        //Get voucher by voucherCode.
        public virtual string GetVoucher(string voucherCode, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                GiftCardModel giftCardModel = _service.GetVoucher(voucherCode);
                if (IsNotNull(giftCardModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new GiftCardResponse { GiftCard = giftCardModel });
            }
            return data;
        }

        //Get list of Gift Card history for a user.
        public virtual string GetGiftCardHistoryList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Gift Card history list.
                GiftCardHistoryListModel list = _service.GetGiftCardHistoryList(Expands, Filters, Sorts, Page);
                if (IsNotNull(list?.GiftCard))
                {
                    GiftCardHistoryListResponse response = new GiftCardHistoryListResponse { GiftCardHistoryList = list.GiftCardHistoryList, GiftCard = list.GiftCard };

                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}