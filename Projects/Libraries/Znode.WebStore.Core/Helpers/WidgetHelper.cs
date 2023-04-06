using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.ViewModels;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore
{
    public static class WidgetHelper
    {
        #region Private Variables
        private static readonly IWidgetDataAgent _widgetDataAgent;
        //TODO: This constant will be removed when actual recommendations are fetched from recommendation engine.
        //Home page specials widget key.
        private const string homePageSpecialsWidgetKey = "667";
        #endregion

        #region Constructor
        static WidgetHelper()
        {
            _widgetDataAgent = GetService<IWidgetDataAgent>();
        }

        #endregion

        //Async call for getting slider data.
        public static async Task<WidgetSliderBannerViewModel> GetSliderAsync(WidgetParameter parameter)
            => await _widgetDataAgent.GetSliderAsync(parameter);

        //Get slider data.
        public static WidgetSliderBannerViewModel GetSlider(WidgetParameter parameter)
           => _widgetDataAgent.GetSlider(parameter);

        //Get product list data.
        public static WidgetProductListViewModel GetProducts(WidgetParameter parameter)
        => _widgetDataAgent.GetProducts(parameter);

        //Get product list data for home page recommendations.
        public static WidgetProductListViewModel GetHomePageRecommendations(WidgetParameter parameter)
        => GetRecommendedProducts(parameter, PortalAgent.CurrentPortal.RecommendationSetting?.IsHomeRecommendation);

        //Get product list data for PDP page recommendations.
        public static WidgetProductListViewModel GetPDPPageRecommendations(WidgetParameter parameter)
        => GetRecommendedProducts(parameter, PortalAgent.CurrentPortal.RecommendationSetting?.IsPDPRecommendation);

        //Get product list data for cart page recommendations.
        public static WidgetProductListViewModel GetCartPageRecommendations(WidgetParameter parameter)
        => GetRecommendedProducts(parameter, PortalAgent.CurrentPortal.RecommendationSetting?.IsCartRecommendation);

        private static WidgetProductListViewModel GetRecommendedProducts(WidgetParameter parameter, bool? isRecommendationWidgetEnabled)
        {
            SetWidgetKeyAndMappingId(parameter);            
            //Return product list if recommendation setting is enabled from Znode Admin else return empty model.
            //TODO: Product list will be fetched from recommendation engine, so GetProducts method will not be used in future.
            if (isRecommendationWidgetEnabled.HasValue && isRecommendationWidgetEnabled.Value)
                return _widgetDataAgent.GetProducts(parameter);
            else
                return new WidgetProductListViewModel();
        }

        //To set the WidgetKey and CMSMappingId in WidgetParameter model.
        private static void SetWidgetKeyAndMappingId(WidgetParameter parameter)
        {
            parameter.WidgetKey = homePageSpecialsWidgetKey;
            parameter.CMSMappingId = PortalAgent.CurrentPortal.PortalId;
        }

        //Get link widget data.
        public static WidgetTitleListViewModel GetLinkWidget(WidgetParameter parameter)
           => _widgetDataAgent.GetLinkData(parameter);

        //Get Media Widget Details
        public static WidgetMediaViewModel GetMedia(WidgetParameter parameter)
          => _widgetDataAgent.GetMediaWidgetDetails(parameter);

        //Get category list data.
        public static WidgetCategoryListViewModel GetCategories(WidgetParameter parameter)
           => _widgetDataAgent.GetCategories(parameter);

        //Get brand list data.
        public static WidgetBrandListViewModel GetBrands(WidgetParameter parameter)
        => _widgetDataAgent.GetBrands(parameter);

        //Get Form Builder Attribute data.
        public static WidgetFormConfigurationViewModel GetFormConfiguration(WidgetParameter parameter)
        => _widgetDataAgent.GetFormConfiguration(parameter);

        //Get category list data.
        public static ContentPageListViewModel GetContentPages(WidgetParameter parameter)
           => _widgetDataAgent.GetContentPages(parameter);

        //Get content Page content.
        public static WidgetTextViewModel GetContent(WidgetParameter widgetparameter)
          => _widgetDataAgent.GetContent(widgetparameter);

        //Get sub categories.
        public static List<CategoryViewModel> GetSubCategories(WidgetParameter widgetparameter)
        => _widgetDataAgent.GetSubCategories(widgetparameter);

       
        //Get category products.
        public static ProductListViewModel GetCategoryProducts(WidgetParameter widgetparameter)
        {
            Dictionary<string, object> prop = widgetparameter.properties;
            if (!Equals(prop, null))
            {
                SetParameter(prop);
                return _widgetDataAgent.GetCategoryProducts(widgetparameter);
            }
            else
            {
                return _widgetDataAgent.GetCategoryProducts(widgetparameter);
            }


        }

        private static void SetParameter(Dictionary<string, object> prop)
        {
            if (string.IsNullOrEmpty(Convert.ToString(prop["pageSize"])))
            {
                //First we check wheather we set PageList value againest the portal or not
                //If not then we give default page size
                int? pageValue = PortalAgent.CurrentPortal?.PageList?.Select(x => x.PageValue).FirstOrDefault();
                prop["pageSize"] = (!Equals(pageValue, null) && pageValue > 0) ? pageValue : 16;
            }
            if (string.IsNullOrEmpty(Convert.ToString(prop["sort"])))
            {
                int? sortValue = PortalAgent.CurrentPortal?.SortList?.Select(x => x.SortValue).FirstOrDefault();
                prop["sort"] = (!Equals(sortValue, null) && sortValue > 0) ? sortValue : 0;
            }
            if (string.IsNullOrEmpty(Convert.ToString(prop["pagenumber"])))
                prop["pagenumber"] = 1;
        }

        //Get quick view data for a product.
        public static ProductViewModel GetProductQuickView(WidgetParameter widgetparameter)
          => _widgetDataAgent.GetProductQuickView(widgetparameter);

        //Get facet list.
        public static SearchResultViewModel GetFacetList(WidgetParameter widgetparameter)
             => _widgetDataAgent.GetFacetList(widgetparameter, 1, -1);

        //Get cart item count.
        public static decimal GetCartCount()
             => _widgetDataAgent.GetCartCount();

        //Get tag manager data.
        public static WidgetTextViewModel GetTagManager(WidgetParameter widgetparameter)
          => _widgetDataAgent.GetTagManager(widgetparameter);

        //get search widget products.
        public static WidgetSearchDataViewModel GetSearchWidgetData(WidgetParameter widgetparameter)
          => _widgetDataAgent.GetSearchWidgetData(widgetparameter);

        //Get total available balance for ECertificate.
        public static ECertTotalBalanceViewModel GetECertTotalBalance(WidgetParameter parameter, decimal availableBalance = 0)
           => _widgetDataAgent.GetECertTotalBalance(parameter, availableBalance);

        //Get Container data.
        public static string GetContainer(WidgetParameter widgetParameter)
            => _widgetDataAgent.GetContainer(widgetParameter);  
    }
}