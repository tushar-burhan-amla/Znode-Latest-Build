using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.Admin
{
    public class ZnodeQuoteHelper :IZnodeQuoteHelper
    {

        #region Private Variables

        private readonly IZnodeRepository<ZnodeOmsQuote> _quoteRepository;
        private readonly IZnodeRepository<ZnodeOmsQuoteLineItem> _quoteLineItemRepository;
        #endregion Private Variables

        #region Public Constructor

        public ZnodeQuoteHelper()
        {
            _quoteRepository = new ZnodeRepository<ZnodeOmsQuote>();
            _quoteLineItemRepository = new ZnodeRepository<ZnodeOmsQuoteLineItem>();
        }

        #endregion Public Constructor
        //to get quote by quoteId
        public virtual ZnodeOmsQuote GetQuoteById(int omsQuoteId)
        {
            if (omsQuoteId <= 0)
                return null;
            else
                return _quoteRepository.Table.FirstOrDefault(x => x.OmsQuoteId == omsQuoteId);
        }

        //to get quote line items by omsQuoteId
        public virtual List<ZnodeOmsQuoteLineItem> GetQuoteLineItemByQuoteId(int omsQuoteId)
        {

            FilterDataCollection filter = new FilterDataCollection();
            filter.Add(new FilterDataTuple(ZnodeOmsQuoteLineItemEnum.OmsQuoteId.ToString(), FilterOperators.Equals, omsQuoteId.ToString()));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter)?.WhereClause;
            return _quoteLineItemRepository.GetEntityList(whereClause)?.ToList();
        }

        public List<PersonaliseValueModel> GetPersonalisedValueCartLineItem(List<int?> quoteLineItemIds)
        {
            IZnodeRepository<ZnodeOmsQuotePersonalizeItem> _savedOmsPersonalizeCartItemRepository = new ZnodeRepository<ZnodeOmsQuotePersonalizeItem>();         
            IZnodeRepository<ZnodePimAttribute> _pimAttribute = new  ZnodeRepository<ZnodePimAttribute>();
            IZnodeRepository<ZnodePimAttributeLocale> _pimAttributeLocale = new ZnodeRepository<ZnodePimAttributeLocale>();

            List<PersonaliseValueModel> personalizeItemList = new List<PersonaliseValueModel>();
            if (quoteLineItemIds.Count > 0)
            {

                List<PersonaliseValueModel> personalizeItem = (from item in _savedOmsPersonalizeCartItemRepository.Table
                                                               join itemAttributeDetails in _pimAttribute.Table on item.PersonalizeCode equals itemAttributeDetails.AttributeCode
                                                               join itemAttributeLocaleDetails in _pimAttributeLocale.Table on itemAttributeDetails.PimAttributeId equals itemAttributeLocaleDetails.PimAttributeId
                                                               where quoteLineItemIds.Contains(item.OmsQuoteLineItemId)
                                                               select new PersonaliseValueModel
                                                               {
                                                                   PersonalizeCode = item.PersonalizeCode,
                                                                   PersonalizeValue = item.PersonalizeValue,
                                                                   DesignId = item.DesignId,
                                                                   ThumbnailURL = item.ThumbnailURL,
                                                                   PersonalizeName = itemAttributeLocaleDetails.AttributeName,
                                                                   OmsSavedCartLineItemId = item.OmsQuoteLineItemId
                                                               }).ToList();
                if (personalizeItem.Count > 0)
                    personalizeItemList.AddRange(personalizeItem);
                return personalizeItemList.Distinct().ToList();
            }
            return personalizeItemList;
        }
    }
}
