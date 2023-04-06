using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class TaxRuleTypeCache : BaseCache, ITaxRuleTypeCache
    {
        #region Private Variables
        private readonly ITaxRuleTypeService _taxRuleTypeService;
        #endregion

        #region Constructor
        public TaxRuleTypeCache(ITaxRuleTypeService taxRuleTypeService)
        {
            _taxRuleTypeService = taxRuleTypeService;
        }
        #endregion

        #region Public Methods
        public virtual string GetTaxRuleTypeList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                TaxRuleTypeListModel list = _taxRuleTypeService.GetTaxRuleTypeList(Filters, Sorts, Page);

                //If list count is greater than 0 then Create a list response for TaxRuleType and insert into cache.
                if (list?.TaxRuleTypes?.Count > 0)
                {                    
                    TaxRuleTypeListResponse response = new TaxRuleTypeListResponse { TaxRuleTypes = list.TaxRuleTypes };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetTaxRuleType(int taxRuleTypeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                TaxRuleTypeModel taxRuleType = _taxRuleTypeService.GetTaxRuleType(taxRuleTypeId);

                //If taxRuleType has data then Create a response for TaxRuleType and insert into cache.
                if (!Equals(taxRuleType, null))
                {
                    TaxRuleTypeResponse response = new TaxRuleTypeResponse { TaxRuleType = taxRuleType };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAllTaxRuleTypesNotInDatabase(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                TaxRuleTypeListModel list = _taxRuleTypeService.GetAllTaxRuleTypesNotInDatabase();

                //If list count is greater than 0 then Create a list response for TaxRuleType and insert into cache.
                if (list?.TaxRuleTypes?.Count > 0)
                {
                    TaxRuleTypeListResponse response = new TaxRuleTypeListResponse { TaxRuleTypes = list.TaxRuleTypes };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

    }
}