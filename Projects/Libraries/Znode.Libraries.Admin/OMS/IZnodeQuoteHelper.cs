using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Libraries.Admin
{
    public interface IZnodeQuoteHelper
    {
        /// <summary>
        /// to get quote by omsQuoteId
        /// </summary>
        /// <param name="omsQuoteId"></param>
        /// <returns>ZnodeOmsQuote</returns>
        ZnodeOmsQuote GetQuoteById(int omsQuoteId);

        /// <summary>
        /// To get quote line items by omsQuoteId
        /// </summary>
        /// <param name="omsQuoteId"></param>
        /// <returns>List of ZnodeOmsQuoteLineItem</returns>
        List<ZnodeOmsQuoteLineItem> GetQuoteLineItemByQuoteId(int omsQuoteId);

        /// <summary>
        /// Get Personalized Value for QuoteLineItem
        /// </summary>
        /// <param name="quoteLineItemIds"></param>
        /// <returns></returns>
        List<PersonaliseValueModel> GetPersonalisedValueCartLineItem(List<int?> quoteLineItemIds);
    }
}
