using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class TaxClassCache : BaseCache, ITaxClassCache
    {
        #region Private Variable

        private readonly ITaxClassService _service;

        #endregion Private Variable

        #region Constructor

        public TaxClassCache(ITaxClassService taxClassService)
        {
            _service = taxClassService;
        }

        #endregion Constructor

        #region Public Methods

        #region Taxes

        // Get the list of all tax classes.
        public virtual string GetTaxClassList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                TaxClassListModel list = _service.GetTaxClassList(Filters, Sorts, Page);
                if (list?.TaxClassList?.Count > 0)
                {
                    TaxClassListResponse response = new TaxClassListResponse { TaxClasses = list.TaxClassList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        // Get a Tax Class.
        public virtual string GetTaxClass(int taxClassId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                TaxClassModel taxClass = _service.GetTaxClass(taxClassId);
                if (!Equals(taxClass, null))
                {
                    TaxClassResponse response = new TaxClassResponse { TaxClass = taxClass };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion Taxes

        #region Tax Class SKU

        /// Get Tax Class SKU list.
        public virtual string GetTaxClassSKUList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                TaxClassSKUListModel list = _service.GetTaxClassSKUList(Filters, Sorts, Page);
                if (list?.TaxClassSKUList?.Count > 0)
                {
                    TaxClassSKUListResponse response = new TaxClassSKUListResponse { TaxClassSKUList = list.TaxClassSKUList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Unassociated Products From Cache
        public virtual string GetUnassociatedProductList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                ProductDetailsListModel productList = _service.GetUnassociatedProductList(Expands, Filters, Sorts, Page);
                ProductListResponse response = new ProductListResponse
                {
                    ProductDetails = productList.ProductDetailList,
                    Locale = productList.Locale,
                    AttrubuteColumnName = productList.AttributeColumnName,
                    XmlDataList = productList.XmlDataList,
                    ProductDetailsDynamic = productList?.ProductDetailListDynamic,
                    NewAttributeList = productList?.NewAttributeList
                };
                response.MapPagingDataFromModel(productList);
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        #endregion Tax Class SKU

        #region Tax Rule

        // Get Tax Rule by TaxRuleId.
        public virtual string GetTaxRule(int taxRuleId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                TaxRuleModel taxRule = _service.GetTaxRule(taxRuleId);
                if (IsNotNull(taxRule))
                {
                    TaxRuleResponse response = new TaxRuleResponse { TaxRule = taxRule };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        // Get Tax Rule list.
        public virtual string GetTaxRuleList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                TaxRuleListModel list = _service.GetTaxRuleList(Expands, Filters, Sorts, Page);
                if (list?.TaxRuleList?.Count > 0)
                {
                    TaxRuleListResponse response = new TaxRuleListResponse { TaxRuleList = list.TaxRuleList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion Tax Rule

        #endregion Public Methods
    }
}