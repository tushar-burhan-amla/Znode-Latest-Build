using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore;
using Znode.Engine.WebStore.Agents;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.WebStore.Core.ViewModels;

namespace Znode.WebStore.Core.Agents
{
    public class SiteMapAgent : BaseAgent, ISiteMapAgent
    {
        #region Private Variables
        private readonly ISiteMapClient _siteMapClient;

        #endregion

        #region Constructor
        public SiteMapAgent(ISiteMapClient siteMapClient)
        {
            _siteMapClient = GetClient<ISiteMapClient>(siteMapClient);
        }
        #endregion

        #region Public Methods.

        //This is used to fetch the categories for sitemap.
        public virtual SiteMapCategoryListViewModel GetSiteMapCategoryList(int? pageSize = 0, int? pageLength = 0)
        {
            // Data is sorted as per the Display Order.
            SortCollection sorts = new SortCollection { { SortKeys.DisplayOrder, SortDirections.Ascending } };

            // includeAssociatedCategories flag is used to fetch the child categories.
            // Data will be fetch as per the parent categories.            
            // If value is set true then child categories will be fetch from the database.
            // If value is false then only parent categories will be fetch from the database.
            // Default value is true and if child categories count is too high then only value can be set as false.  
            bool includeAssociatedCategories = true;

            //Get the categories list.
            SiteMapCategoryListModel listModel = _siteMapClient.GetSitemapCategoryList(includeAssociatedCategories, null, CreateCategoriesFilter(true, 0), sorts, pageSize, pageLength);

            SiteMapCategoryListViewModel listViewModel = new SiteMapCategoryListViewModel { CategoryList = listModel?.CategoryList?.ToViewModel<SiteMapCategoryViewModel>().ToList(), TotalResults = listModel?.TotalResults.GetValueOrDefault() ?? 0 };

            listViewModel.BrandList = new List<SiteMapBrandViewModel>();
            // if the categories list doesnot contains data then brand data will be fetch.
            if (listViewModel == null || listViewModel.CategoryList == null || listViewModel?.CategoryList?.Count == 0)
            {
                int TotalBrandCount = string.IsNullOrEmpty(ZnodeWebstoreSettings.TotalBrandCount) ? Convert.ToInt32(ZnodeWebstoreSettings.TotalBrandCount) : 25;

                //Get Brand list for header.
                SiteMapBrandListModel brandList = _siteMapClient.GetSitemapBrandList(null, GetBrandFilter(), sorts, 1, TotalBrandCount);

                listViewModel.BrandList = brandList?.BrandList?.Count > 0 ? brandList?.BrandList?.ToViewModel<SiteMapBrandViewModel>().ToList() : new List<SiteMapBrandViewModel>();
            }
            return listViewModel;
        }

        //This is used to fetch the publish products for sitemap.
        public virtual SiteMapProductListViewModel GetPublishProductList(int? pageIndex, int? pageSize)
        {
            FilterCollection filters = new FilterCollection();
            filters.RemoveAll(x => x.FilterName == FilterKeys.ZnodeCatalogId);
            filters.Add(FilterKeys.ZnodeCatalogId, FilterOperators.Equals, PortalAgent.CurrentPortal.PublishCatalogId.ToString());
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());
            filters.Add(FilterKeys.LocaleId, FilterOperators.Equals, PortalAgent.CurrentPortal.Locales?.FirstOrDefault(x => x.IsDefault == true)?.LocaleId.ToString());

            ExpandCollection expands = new ExpandCollection();
            SiteMapProductListModel listModel = _siteMapClient.GetSitemapProductList(expands, filters, null, pageIndex, pageSize);
            SiteMapProductListViewModel siteMapProductListViewModel = new SiteMapProductListViewModel { ProductList = listModel?.ProductList?.ToViewModel<SiteMapProductViewModel>().ToList(), TotalResults = listModel?.TotalResults.GetValueOrDefault() ?? 0 };
            return siteMapProductListViewModel;
        }

        //Get Product feed list by portal Id.
        public virtual List<ProductFeedModel> GetProductFeedByPortalId(string actionName, string requestUrl, out string xmlDocument)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            xmlDocument = string.Empty;
            try
            {
                //Check if application type is Webstore.
                string applicationType = PortalAgent.CurrentPortal.ApplicationType ?? ZnodeConstant.ApplicationType;
                if (!applicationType.Equals(ZnodeConstant.ApplicationType, StringComparison.InvariantCultureIgnoreCase)) 
                        return null;
                
                    List<ProductFeedModel> productFeedModelList = _siteMapClient.GetProductFeedByPortalId(PortalAgent.CurrentPortal.PortalId);
                    if (HelperUtility.IsNull(productFeedModelList) || !(productFeedModelList.Count > 0))
                        return null;

                    if (actionName.Equals(ZnodeConstant.SitemapXml, StringComparison.InvariantCultureIgnoreCase))
                    {
                        productFeedModelList = productFeedModelList.Where(x => x.ProductFeedTypeCode.Equals(ZnodeConstant.XmlSiteMap, StringComparison.InvariantCultureIgnoreCase))?.OrderByDescending(x => x.ModifiedDate).ToList();
                        //Check if there is any "ALL" type sitemap url available.
                        bool isExistsAllSitemap = productFeedModelList.FirstOrDefault().ProductFeedSiteMapTypeCode.Equals(ZnodeConstant.AllSitemapUrls, StringComparison.InvariantCultureIgnoreCase) && productFeedModelList.Any(x => x.ProductFeedSiteMapTypeCode.Equals(ZnodeConstant.AllSitemapUrls, StringComparison.InvariantCultureIgnoreCase));

                        if (isExistsAllSitemap)
                            //Get list of urls which were created using the "ALL" type sitemap option.
                            productFeedModelList = productFeedModelList.Where(x => x.ProductFeedSiteMapTypeCode.Equals(ZnodeConstant.AllSitemapUrls, StringComparison.InvariantCultureIgnoreCase))?.ToList();
                        else
                            //Get list of content, product, category feed type sitemap urls only.
                            productFeedModelList = productFeedModelList.Where(x => !x.ProductFeedSiteMapTypeCode.Equals(ZnodeConstant.AllSitemapUrls, StringComparison.InvariantCultureIgnoreCase))?.ToList();
               
                }
                else if (actionName.Equals(ZnodeConstant.GoogleProductFeedRoute, StringComparison.InvariantCultureIgnoreCase))
                    //Get list of only google product feed urls.
                    productFeedModelList = productFeedModelList.Where(x => x.ProductFeedTypeCode.Equals(ZnodeConstant.GoogleProductFeed, StringComparison.InvariantCultureIgnoreCase))?.ToList();

                else if (actionName.Equals(ZnodeConstant.BingProductFeedRoute, StringComparison.InvariantCultureIgnoreCase))
                    //Get list of only bing product feed urls.
                    productFeedModelList = productFeedModelList.Where(x => x.ProductFeedTypeCode.Equals(ZnodeConstant.BingProductFeedType, StringComparison.InvariantCultureIgnoreCase))?.ToList();

                else if (actionName.Equals(ZnodeConstant.XmlProductFeedRoute, StringComparison.InvariantCultureIgnoreCase))
                    //Get list of only shopping feed urls.
                    productFeedModelList = productFeedModelList.Where(x => x.ProductFeedTypeCode.Equals(ZnodeConstant.XmlProductFeedType, StringComparison.InvariantCultureIgnoreCase))?.ToList();

                xmlDocument = GetXmlUrlList(productFeedModelList, requestUrl);
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                return productFeedModelList;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.ErrorMessage, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                return null;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                return null;
            }
        }

        //Returns the xml child node urls with filename in format as per the sitemap type.
        protected virtual string GetXmlUrlList(List<ProductFeedModel> productFeedListModels, string requestUrl)
        {
            ZnodeLogging.LogMessage("GetXmlUrlList method to generate XML file names execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            string xmlDocument = string.Empty;
            foreach (ProductFeedModel productFeed in productFeedListModels)
            {
                string fileName = productFeed?.FileName?.Split('_')[0];
                if (productFeed.ProductFeedTypeCode.Equals(ZnodeConstant.XmlSiteMap, StringComparison.InvariantCultureIgnoreCase))
                {
                    xmlDocument = GetFileNameWithCount(fileName, productFeed.FileCount, xmlDocument, requestUrl);
                }
                else
                {
                    xmlDocument = GetFileNameWithLocaleIdAndCount(fileName, xmlDocument, productFeed, requestUrl);
                }
            }
            ZnodeLogging.LogMessage("GetXmlUrlList method execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return $"<sitemapindex>{xmlDocument}</sitemapindex>";
        }

        //Get file name for google and bing xml product feed with locale Id alongwith chunk id and portal Id.
        protected virtual string GetFileNameWithLocaleIdAndCount(string fileNameType, string xmlDocument, ProductFeedModel productFeed, string requestUrl)
        {
            for (int count = 0; count < productFeed.FileCount; count++)
            {
                xmlDocument += $"<sitemap><loc>{requestUrl}/{fileNameType}_{count}_{productFeed.LocaleId}.xml</loc></sitemap>";
            }
            return xmlDocument;
        }

        //Get file name for site map with chunk id and portal Id.
        protected virtual string GetFileNameWithCount(string fileNameType, int fileCount, string xmlDocument, string requestUrl)
        {
            for (int count = 0; count < fileCount; count++)
            {
                xmlDocument += $"<sitemap><loc>{requestUrl}/{fileNameType}_{count}.xml</loc></sitemap>";
            }
            return xmlDocument;
        }

        //Get sitemap xml fom Api
        public virtual string GetXmlFromAPIForWebstoreDomain(string requestUrl)
        {
            if (HelperUtility.IsNotNull(requestUrl))
            {
                string siteMapPath = string.Empty;
                MediaConfigurationModel mediaConfiguration = GetClient<MediaConfigurationClient>().GetDefaultMediaConfiguration();

                // Check whether the media setting's server is Azure or Local.
                if (mediaConfiguration.Server.Equals(ZnodeConstant.Azure, StringComparison.InvariantCultureIgnoreCase))
                    siteMapPath = string.Concat(mediaConfiguration.CDNUrl, ZnodeConstant.FolderSiteMap, requestUrl.ToLower().Split('.')[0], "_", PortalAgent.CurrentPortal.PortalId, ".xml");
                else if(mediaConfiguration.Server.Equals(ZnodeConstant.NetworkDrive, StringComparison.InvariantCultureIgnoreCase))
                    siteMapPath = string.Concat(mediaConfiguration.NetworkUrl,"/", mediaConfiguration.BucketName, "/", ZnodeConstant.FolderSiteMap, requestUrl.ToLower().Split('.')[0], "_", PortalAgent.CurrentPortal.PortalId, ".xml");
                else
                    siteMapPath = string.Concat(mediaConfiguration.URL, ZnodeConstant.FolderSiteMap, requestUrl.ToLower().Split('.')[0], "_", PortalAgent.CurrentPortal.PortalId, ".xml");
                
                return ReadXMl(siteMapPath);
            }
            return null;
        }

        //Read xml from API.
        protected virtual string ReadXMl(string url)
        {
            if (HelperUtility.IsNotNull(url))
            {
                try
                {
                    //Load sitemap XML
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(url);
                    //Create StringWriter object to get data from xml document.
                    StringWriter stringWriter = new StringWriter();
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                    xmlDocument.WriteTo(xmlTextWriter);
                    string XmlString = stringWriter.ToString();

                    return XmlString;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                    return null;
                }
            }
            return null;
        }

        #endregion

        #region Private Member

        //Create filters to get category list.
        private FilterCollection CreateCategoriesFilter(bool isTopLevelCategories, int? categoryId)
        {
            FilterCollection filters = GetRequiredFilters();
            filters.Add(WebStoreEnum.ZnodeParentCategoryIds.ToString(), FilterOperators.Equals, "0");
            return filters;
        }

        #endregion
    }
}
