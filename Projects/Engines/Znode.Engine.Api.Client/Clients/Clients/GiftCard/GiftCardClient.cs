using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class GiftCardClient : BaseClient, IGiftCardClient
    {
        // Gets the list of GiftCard.
        public virtual GiftCardListModel GetGiftCardList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetGiftCardList(expands, filters, sorts, null, null);

        // Gets the list of GiftCard.
        public virtual GiftCardListModel GetGiftCardList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = GiftCardEndpoint.GetGiftCardList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            GiftCardListResponse response = GetResourceFromEndpoint<GiftCardListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GiftCardListModel list = null;
            if (HelperUtility.IsNotNull(response))
            {
                list = new GiftCardListModel { GiftCardList = response.GiftCardList, ReferralCommissionCount = response.ReferralCommissionCount };
                list.MapPagingDataFromResponse(response);
            }
            return list;
        }

        //Create gift card.
        public virtual GiftCardModel CreateGiftCard(GiftCardModel giftCardModel)
        {
            //Get Endpoint.
            string endpoint = GiftCardEndpoint.CreateGiftCard();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GiftCardResponse response = PostResourceToEndpoint<GiftCardResponse>(endpoint, JsonConvert.SerializeObject(giftCardModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.GiftCard;
        }

        //Get gift card by gift card id.
        public virtual GiftCardModel GetGiftCard(int giftCardId)
        {
            string endpoint = GiftCardEndpoint.GetGiftCard(giftCardId);

            ApiStatus status = new ApiStatus();
            GiftCardResponse response = GetResourceFromEndpoint<GiftCardResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.GiftCard;
        }

        //Update gift card.
        public virtual GiftCardModel UpdateGiftCard(GiftCardModel giftCardModel)
        {
            string endpoint = GiftCardEndpoint.UpdateGiftCard();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GiftCardResponse response = PutResourceToEndpoint<GiftCardResponse>(endpoint, JsonConvert.SerializeObject(giftCardModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.GiftCard;
        }

        //Delete gift card by gift card id.
        public virtual bool DeleteGiftCard(ParameterModel giftCardId)
        {
            string endpoint = GiftCardEndpoint.DeleteGiftCard();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(giftCardId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get randomly generated gift card number.
        public virtual string GetRandomGiftCardNumber()
        {
            string endpoint = GiftCardEndpoint.GetRandomGiftCardNumber();

            ApiStatus status = new ApiStatus();
            StringResponse response = GetResourceFromEndpoint<StringResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Response;
        }

        //Get gift card history for a user.
        public virtual GiftCardHistoryListModel GetGiftCardHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = GiftCardEndpoint.GetGiftCardHistoryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            GiftCardHistoryListResponse response = GetResourceFromEndpoint<GiftCardHistoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GiftCardHistoryListModel list = new GiftCardHistoryListModel { GiftCardHistoryList = response?.GiftCardHistoryList, GiftCard = response?.GiftCard };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get vouchers list
        public virtual GiftCardListModel GetVouchers(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = GiftCardEndpoint.GetGiftCardList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            GiftCardListResponse response = GetResourceFromEndpoint<GiftCardListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GiftCardListModel list = new GiftCardListModel { GiftCardList = response?.GiftCardList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Activate Deactivate vouchers.
        public virtual bool ActivateDeactivateVouchers(ParameterModel voucherIds, bool isActive)
        {
            string endpoint = GiftCardEndpoint.ActivateDeactivateVouchers(isActive);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(voucherIds), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

    }
}
