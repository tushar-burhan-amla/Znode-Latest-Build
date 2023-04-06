using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using Znode.Libraries.MediaStorage;
using System.Text;
using Znode.Engine.Api.Models.Enum;
using System.IO;
using System.Xml.Serialization;
using System.Web;


namespace Znode.Engine.Services
{
    public class ProductFeedService : BaseService, IProductFeedService
    {
        #region Private Variables
        private readonly string strNamespace = ZnodeApiSettings.SiteMapNameSpace;
        private readonly string strGoogleFeedNamespace = ZnodeApiSettings.GoogleProductFeedNameSpace;
        private const string ItemCategory = "category";
        private const string ItemContentPages = "content";
        private const string ItemProduct = "product";
        private readonly string ItemUrlset = "urlset";
        private const string ItemAll = "all";
        private readonly IZnodeRepository<ZnodeProductFeed> _productFeedRepository;
        private readonly IZnodeRepository<ZnodeProductFeedSiteMapType> _productFeedSiteMapTypeRepository;
        private readonly IZnodeRepository<ZnodeProductFeedType> _productFeedTypeRepository;
        private readonly IZnodeProductFeedHelper productFeedHelper;
        private readonly IZnodeRepository<ZnodeRobotsTxt> _robotsTxtRepository;
        #endregion

        #region Constructor
        public ProductFeedService()
        {
            _productFeedRepository = new ZnodeRepository<ZnodeProductFeed>();
            _productFeedSiteMapTypeRepository = new ZnodeRepository<ZnodeProductFeedSiteMapType>();
            _productFeedTypeRepository = new ZnodeRepository<ZnodeProductFeedType>();
            productFeedHelper = ZnodeDependencyResolver.GetService<IZnodeProductFeedHelper>();
            _robotsTxtRepository = new ZnodeRepository<ZnodeRobotsTxt>();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// This method will create the Product Feed or SiteMap
        /// </summary>
        /// <param name="model">Product Feed Model</param>
        /// <returns>Product Feed Model</returns>
        public virtual ProductFeedModel CreateGoogleSiteMap(ProductFeedModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
            ProductFeedModel generatedXml = new ProductFeedModel();
            string feedType = string.Empty;
            bool isFeed = false;
            string domainUrl = GetPortalDomain(model.PortalId);
            
            bool isProductFeedExist = _productFeedRepository.Table.Any(x => x.FileName.Equals(model.FileName, StringComparison.InvariantCultureIgnoreCase) && x.PortalId == model.PortalId && x.LocaleId == model.LocaleId);
            if (isProductFeedExist && !model.IsFromScheduler)
            {
                generatedXml.ErrorMessage = Admin_Resources.ProductFeedAlreadyExist;
                return generatedXml;
            }

            //check if Feed is for Site map or Product.
            CheckIsSiteMapOrProduct(model, out feedType, out isFeed);

            // Get XML files.
            List<string> xmlFiles = GetXMLfiles(model, isFeed, feedType);

            model.FileCount = xmlFiles?.Count ?? 0;
            if (model.FileCount == 0)
            {
                generatedXml.ErrorMessage = Admin_Resources.ErrorGeneratingFeed;
                return generatedXml;
            }

            string navigationUrl = GetNavigationUrl(model);
            model.SuccessXMLGenerationMessage = $"{HttpContext.Current.Request.Url.Scheme}://{domainUrl}/{navigationUrl}";

            //Set product feed master details.
            SetProductFeedMasterDetails(model);

            if (!model.IsFromScheduler)
                _productFeedRepository.Insert(model.ToEntity<ZnodeProductFeed>());

            // Upload sitemap to media.
            UploadSiteMapToMedia(model, xmlFiles);

            //Add sitemap url in robots.txt
            if (!string.IsNullOrEmpty(domainUrl))
            {
                AppendSitemapUrlInRobotsTxtContent(model);
            }

            xmlFiles = null;
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return model;
        }

        // Get XML files.
        public virtual List<string> GetXMLfiles(ProductFeedModel model, bool isFeed, string feedType)
        {
            ZnodeLogging.LogMessage("Fetch XML files.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            List<string> xmlFiles = new List<string>();

            if (isFeed)
            {
                if (feedType.Equals("Xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    xmlFiles = GetXmlProductFeedList(model.PortalId, model.FileName, feedType, model.LocaleId, model.Title, model.Link, model.Description);
                }
                else
                {
                    xmlFiles = productFeedHelper.GetProductList(model.PortalId, this.strGoogleFeedNamespace, model.FileName, model.Title, model.Link, model.Description, feedType, model.LocaleId);
                }
            }
            else
            {
                //Call if Site map to be generated for Category/Content Page
                ZnodeLogging.LogMessage("Parameter for getting fileNameCount", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { "ProductFeedModel model", xmlFiles?.Count, feedType });
                xmlFiles = GetXmlFiles(model, model.PortalId, feedType);
            }
            ZnodeLogging.LogMessage("Execution GetXMLfiles done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return xmlFiles;
        }


        private List<string> GetXmlProductFeedList(int portalId, string fileName, string feedType, int localeId, string title, string link, string description)
        {
            List<string> xmlFiles = new List<string>();
            ZnodeLogging.LogMessage("GetXmlProductFeedList execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                //Getting all Published product list for the selected stores and Locale from DB
                IZnodeViewRepository<XmlProductFeedModel> objStoredProc = new ZnodeViewRepository<XmlProductFeedModel>();
                objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@FeedType", feedType, ParameterDirection.Input, DbType.String);
                IList<XmlProductFeedModel> xmlProductFeedList = objStoredProc.ExecuteStoredProcedureList("Znode_GetXmlProductFeedList  @PortalId,@LocaleId,@FeedType");

                //Dividing products in to chunks as value set in WebConfig,ZnodeApiSettings.XmlProductFeedChunkSize
                List<List<XmlProductFeedModel>> chunkSize = xmlProductFeedList.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / Convert.ToInt32(ZnodeApiSettings.ProductFeedRecordCount)).Select(x => x.Select(v => v.Value).ToList()).ToList();
                XmlSerializer serializer = new XmlSerializer(typeof(XmlProductFeedXmlModel));
                //Loop count will be same as number of chunks , converting chunks to XML
                foreach (List<XmlProductFeedModel> chunk in chunkSize)
                {
                    XmlProductFeedXmlModel xmlProductFeedXmlModel = new XmlProductFeedXmlModel { Title = title, Description = description, Link = link, products = chunk };
                    using (StringWriter stream = new StringWriter())
                    {
                        using (StringWriter writer = new Utf8StringWriter())
                        {
                            serializer.Serialize(writer, xmlProductFeedXmlModel);
                            xmlFiles.Add(writer.ToString());
                        }
                    }
                }
                ZnodeLogging.LogMessage("GetXmlProductFeedList execution completed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return xmlFiles;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return new List<string>();
            }
        }

        //Get list of product feed.
        public virtual ProductFeedListModel GetProductFeedList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("pageListModel to generate ProductFeedModel list ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<ProductFeedModel> objStoredProc = new ZnodeViewRepository<ProductFeedModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<ProductFeedModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetProductFeedDetailsList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("ProductFeedModel list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, list?.Count());
            ProductFeedListModel listModel = new ProductFeedListModel { ProductFeeds = list?.ToList() };

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get ProductFeed by ProductFeed id.
        public virtual ProductFeedModel GetProductFeed(int productFeedId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (productFeedId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeProductFeedEnum.ProductFeedId.ToString(), FilterOperators.Equals, productFeedId.ToString()));

                return _productFeedRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection()), GetExpands(expands))?.ToModel<ProductFeedModel>();
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return null;
        }

        //Get product feed master table details.
        public virtual ProductFeedModel GetProductFeedMasterDetails()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return new ProductFeedModel()
            {
                ProductFeedSiteMapTypeList = _productFeedSiteMapTypeRepository.GetEntityList(string.Empty)?.ToModel<ProductFeedSiteMapTypeModel>().ToList(),
                ProductFeedTypeList = _productFeedTypeRepository.GetEntityList(string.Empty)?.ToModel<ProductFeedTypeModel>().ToList(),
            };
        }

        //Update  Product Feed.
        public virtual bool UpdateProductFeed(ProductFeedModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
            if (model.ProductFeedId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);
            List<string> xmlFiles = new List<string>();
            string feedType = string.Empty;
            bool isFeed = false;
            string domainUrl = GetPortalDomain(model.PortalId);

            //check if Feed is for Site map or Product
            CheckIsSiteMapOrProduct(model, out feedType, out isFeed);

            ZnodeLogging.LogMessage("PortalId: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, model.PortalId);
            if (isFeed)
            {
                if (feedType.Equals("Xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    xmlFiles = GetXmlProductFeedList(model.PortalId, model.FileName, feedType, model.LocaleId, model.Title, model.Link, model.Description);
                }
                else
                    xmlFiles = productFeedHelper.GetProductList(model.PortalId, this.strGoogleFeedNamespace, model.FileName, model.Title, model.Link, model.Description, feedType, model.LocaleId);
            }
            else
            {
                //Call if Site map to be generated for Category/Content Page
                ZnodeLogging.LogMessage("Parameter for getting fileNameCount", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new object[] { " model", xmlFiles?.Count, productFeedHelper, model.PortalId });
                xmlFiles = GetXmlFiles(model, model.PortalId, feedType);

                //Show error if XML for feed or site map is not generated.
                if (xmlFiles.Count == 0)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorGeneratingXml, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    return false;
                }
            }

            //Set product feed master details.
            model.FileCount = xmlFiles.Count;
            SetProductFeedMasterDetails(model);

            //Upload files to media.
            if (xmlFiles?.Count > 0)
                UploadSiteMapToMedia(model, xmlFiles);

            //File is saved properly in folder location then send that file name along with domain name to be downloaded by Admin user.
            if (!string.IsNullOrEmpty(model.FileName))
            {
                string navigationUrl = GetNavigationUrl(model);
                model.SuccessXMLGenerationMessage = $"{HttpContext.Current.Request.Url.Scheme}://{domainUrl}/{navigationUrl}";
            }

            //Add sitemap url in robots.txt
            if (!string.IsNullOrEmpty(domainUrl))
            {
                AppendSitemapUrlInRobotsTxtContent(model);
            }

            bool isProductFeedUpdated = false;
            if (!model.IsFromScheduler)
                isProductFeedUpdated = _productFeedRepository.Update(model.ToEntity<ZnodeProductFeed>());
            xmlFiles = null;
            ZnodeLogging.LogMessage(isProductFeedUpdated ? Admin_Resources.SuccessProductFeedUpdation : Admin_Resources.ErrorProductFeedUpdation, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return isProductFeedUpdated;
        }

        //Get navigation url on the basis of product feed type.
        protected virtual string GetNavigationUrl(ProductFeedModel model)
        {
            if (IsNotNull(model))
            {
                switch (model.ProductFeedTypeCode.ToLower())
                {
                    case ZnodeConstant.XmlSiteMap:
                        return ZnodeConstant.SitemapRoute;

                    case ZnodeConstant.GoogleProductFeed:
                        return ZnodeConstant.GoogleProductFeedRoute;

                    case ZnodeConstant.BingProductFeedType:
                        return ZnodeConstant.BingProductFeedRoute;

                    case ZnodeConstant.XmlProductFeedType:
                        return ZnodeConstant.XmlProductFeedRoute;

                    default:
                        return String.Empty;
                }
            }
            else
            {
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);
            }
        }

        //Delete product feed  by product feed ids.
        public virtual bool DeleteProductFeed(ParameterModel productFeedIds)
        {
            ZnodeLogging.LogMessage("DeleteProductFeed Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(productFeedIds?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, "ID can not be less than 1 ");
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeProductFeedEnum.ProductFeedId.ToString(), ProcedureFilterOperators.In, productFeedIds?.Ids?.ToString()));
            List<ProductFeedModel> znodeProductFeedList = GetFeedDetails(productFeedIds.Ids);

            List<int> productFeedPortalIds = znodeProductFeedList?.Where(x => x.PortalId > 0 && x.ProductFeedTypeCode.Equals(ZnodeConstant.XmlSiteMap, StringComparison.InvariantCultureIgnoreCase))?.Select(x => x.PortalId)?.Distinct().ToList();
            if (productFeedPortalIds?.Count() > 0)
            {
                DeleteSitemapUrlFromRobotsTxtContent(productFeedIds.Ids, GetFeedDetailsByPortalId(productFeedIds.Ids, productFeedPortalIds), productFeedPortalIds);
            }

            bool isDeleted = _productFeedRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            if (isDeleted)
            {
                DeleteFilesFromMedia(znodeProductFeedList);
            }
            ZnodeLogging.LogMessage("DeleteProductFeed Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return isDeleted;
        }
        //Delete files from media storage
        public virtual void DeleteFilesFromMedia(List<ProductFeedModel> znodeProductFeedList)
        {
            ZnodeLogging.LogMessage("DeleteFilesFromMedia Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if ((IsNotNull(znodeProductFeedList) && znodeProductFeedList.Count > 0))
            {
                foreach (var znodeProductFeed in znodeProductFeedList)
                {
                    int? fileCnt = znodeProductFeed?.FileCount;

                    for (int chunkSize = 0; chunkSize < fileCnt; chunkSize++)
                    {

                        string fileName = znodeProductFeed.ProductFeedTypeCode.Equals(XMLSiteMap.XmlSiteMap.ToString(), StringComparison.InvariantCultureIgnoreCase) ?
                            string.Concat(znodeProductFeed?.ProductFeedSiteMapTypeCode, XMLSiteMap.XmlSiteMap, "_", chunkSize.ToString(), "_", znodeProductFeed?.PortalId.ToString(), ".xml") : string.Concat(znodeProductFeed?.ProductFeedTypeCode, ZnodeConstant.ProductFeedTypeNameForProduct, "_", chunkSize.ToString(), "_", znodeProductFeed?.LocaleId, "_", znodeProductFeed?.PortalId.ToString(), ".xml");
                        if (znodeProductFeed.ProductFeedSiteMapTypeCode.Equals(ZnodeConstant.ContentSitemap, StringComparison.InvariantCultureIgnoreCase))
                        {
                            fileName = string.Concat(ZnodeConstant.ContentPages, XMLSiteMap.XmlSiteMap, "_", chunkSize.ToString(), "_", znodeProductFeed?.PortalId.ToString(), ".xml");
                        }
                        DeleteSiteMapFromMedia(fileName.ToLower());

                    }
                }
            }
            ZnodeLogging.LogMessage("DeleteFilesFromMedia Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }
        //Get all product feed by portal Id.
        public virtual ProductFeedListModel GetProductFeedByPortalId(int portalId)
        {
            ZnodeLogging.LogMessage("GetProductFeedByPortalId Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            ProductFeedListModel productFeedListModel = new ProductFeedListModel();
            if (IsNotNull(portalId))
            {
                NameValueCollection expands = new NameValueCollection();
                SetExpands(expands);
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeProductFeedEnum.PortalId.ToString(), ProcedureFilterOperators.In, portalId.ToString()));

                List<ZnodeProductFeed> znodeProductFeedList = _productFeedRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection()), GetExpands(expands))?.ToList();
                if (IsNull(znodeProductFeedList))
                    throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

                List<ProductFeedModel> productFeedList = znodeProductFeedList.ToModel<ProductFeedModel>().ToList();

                productFeedListModel.ProductFeeds = productFeedList;
            }
            ZnodeLogging.LogMessage("GetProductFeedByPortalId Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return productFeedListModel;
        }

        //Create Sitemap files and return count of files created.
        public virtual List<string> GetXmlFiles(ProductFeedModel model, int portalId, string feedType = "")
        {
            //Assigned file default value null
            List<string> xmlFiles = new List<string>();
            switch (model.ProductFeedSiteMapTypeCode.ToLower())
            {

                case ItemCategory:
                    xmlFiles = productFeedHelper.GetCategoryList(portalId, model, ItemUrlset, this.strNamespace);
                    break;
                case ItemContentPages:
                    xmlFiles = productFeedHelper.GetContentPagesList(portalId, ItemUrlset, this.strNamespace, model);
                    break;
                case ItemProduct:
                    xmlFiles = productFeedHelper.GetProductXMLList(portalId, this.strNamespace, feedType, ItemUrlset, model);
                    break;
                case ItemAll:
                    xmlFiles = productFeedHelper.GetAllList(model, portalId, ItemUrlset, this.strNamespace, feedType);
                    break;
                default:
                    break;
            }
            ZnodeLogging.LogMessage("fileNameCount:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, xmlFiles?.Count);
            return xmlFiles;
        }

        //Method to check file name combination already exists.
        public virtual bool FileNameCombinationAlreadyExist(int localeId, string fileName = "")
        {
            ZnodeLogging.LogMessage("FileNameCombinationAlreadyExist Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                return _productFeedRepository.Table.Any(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase) && x.LocaleId == localeId);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion

        #region Private Methods
        //Get expands and add them to navigation properties
        protected virtual List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodeProductFeedEnum.ZnodeProductFeedType.ToString().ToLower())) SetExpands(ZnodeProductFeedEnum.ZnodeProductFeedType.ToString(), navigationProperties);
                    if (Equals(key, ZnodeProductFeedEnum.ZnodeProductFeedSiteMapType.ToString().ToLower())) SetExpands(ZnodeProductFeedEnum.ZnodeProductFeedSiteMapType.ToString(), navigationProperties);
                    if (Equals(key, ZnodeProductFeedEnum.ZnodeProductFeedType.ToString().ToLower())) SetExpands(ZnodeProductFeedEnum.ZnodeProductFeedType.ToString(), navigationProperties);
                    if (Equals(key, ZnodeProductFeedEnum.ZnodePortal.ToString().ToLower())) SetExpands(ZnodeProductFeedEnum.ZnodePortal.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Set Product Feed Type Code Details.
        protected virtual void GetProductFeedTypeCodeDetails(ProductFeedModel model)
        {
            ZnodeLogging.LogMessage("Input Parameter ProductFeedModel having ProductFeedTypeCode", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, model.ProductFeedTypeCode);
            if (IsNotNull(model))
            {
                ZnodeProductFeedType productFeedType = _productFeedTypeRepository.Table.Where(w => w.ProductFeedTypeCode == model.ProductFeedTypeCode)?.FirstOrDefault();
                model.ProductFeedTypeId = IsNotNull(productFeedType) ? productFeedType.ProductFeedTypeId : 0;
            }
        }


        //Set Product Feed Site Map Type Details.
        protected virtual void GetProductFeedSiteMapTypeDetails(ProductFeedModel model)
        {
            ZnodeLogging.LogMessage("Input Parameter ProductFeedModel having ProductFeedSiteMapTypeCode", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, model.ProductFeedSiteMapTypeCode);
            if (IsNotNull(model))
            {
                ZnodeProductFeedSiteMapType productFeedSiteMapType = _productFeedSiteMapTypeRepository.Table.Where(w => w.ProductFeedSiteMapTypeCode == model.ProductFeedSiteMapTypeCode)?.FirstOrDefault();
                model.ProductFeedSiteMapTypeId = IsNotNull(productFeedSiteMapType) ? productFeedSiteMapType.ProductFeedSiteMapTypeId : 0;
            }
        }


        //Set product feed master details.
        protected virtual void SetProductFeedMasterDetails(ProductFeedModel model)
        {
            ZnodeLogging.LogMessage("SetProductFeedMasterDetails method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            GetProductFeedSiteMapTypeDetails(model);
            GetProductFeedTypeCodeDetails(model);
        }

        // Create is Sitemap or product.
        protected virtual void CheckIsSiteMapOrProduct(ProductFeedModel model, out string feedType, out bool isFeed)
        {
            ZnodeLogging.LogMessage("Input Parameter ProductFeedModel having ProductFeedTypeCode", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, model.ProductFeedTypeCode);
            feedType = string.Empty;
            isFeed = false;
            if (!model.ProductFeedTypeCode.ToLower().Equals("xmlsitemap"))
            {
                feedType = model.ProductFeedTypeCode;
                isFeed = true;
            }
        }
        #endregion

        // Upload XML Files into the respective media storage.
        public virtual void UploadXMLFiles(string className, ServerConnector connectorobj, ProductFeedModel model, MediaConfigurationModel mediaConfiguration, int chunkSize)
        {
            ZnodeLogging.LogMessage("UploadFilesMedia Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            string fileName = string.Equals(model.ProductFeedTypeCode, XMLSiteMap.XmlSiteMap.ToString(), StringComparison.InvariantCultureIgnoreCase) ?
                 string.Concat(model?.ProductFeedSiteMapTypeCode, XMLSiteMap.XmlSiteMap, "_", chunkSize.ToString(), "_", model.PortalId.ToString(), ".xml") : string.Concat(model?.ProductFeedTypeCode, ZnodeConstant.ProductFeedTypeNameForProduct, "_", chunkSize.ToString(), "_", model?.LocaleId, "_", model?.PortalId.ToString(), ".xml");

            if (model.ProductFeedSiteMapTypeCode.Equals(ZnodeConstant.ContentSitemap, StringComparison.InvariantCultureIgnoreCase))
            {
                fileName = string.Concat(ZnodeConstant.ContentPages, XMLSiteMap.XmlSiteMap, "_", chunkSize.ToString(), "_", model?.PortalId.ToString(), ".xml");
            }
            if (IsNotNull(fileName))
            {
                string filePath = GetFolderPath();
                if (mediaConfiguration.Server == ZnodeConstant.NetworkDrive)
                {
                    if (!Directory.Exists(string.Concat(mediaConfiguration.NetworkUrl, '/', mediaConfiguration.BucketName, '/', filePath)))
                        Directory.CreateDirectory(string.Concat(mediaConfiguration.NetworkUrl, '/', mediaConfiguration.BucketName, '/', filePath));
                }
                else
                {
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(mediaConfiguration.BucketName + filePath)))
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(string.Concat(mediaConfiguration.BucketName, '/', filePath)));
                }

                byte[] fileByteData = Encoding.UTF8.GetBytes(model.XmlFileName);
                using (MemoryStream fileStream = new MemoryStream(fileByteData))
                {
                    connectorobj.CallConnector(className, MediaStorageAction.Upload, fileStream, fileName.ToLower(), filePath);
                }
            }
            ZnodeLogging.LogMessage("UploadFilesMedia Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }
        // For uploading the media.
        public virtual void UploadSiteMapToMedia(ProductFeedModel model, List<string> file)
        {
            string className;
            MediaConfigurationModel mediaConfiguration;
            ServerConnector _connectorobj = GetConnectorObject(out className, out mediaConfiguration);
            int loopCnt = 0;
            foreach (var xmlFile in file)
            {
                model.XmlFileName = xmlFile;
                UploadXMLFiles(className, _connectorobj, model, mediaConfiguration, loopCnt);
                loopCnt++;
            }
        }

        // Get server connection.
        protected virtual ServerConnector GetServerConnection(out string className, out int mediaConfigurationId, MediaConfigurationModel mediaConfiguration)
        {
            ServerConnector _connectorobj = null;

            if (HelperUtility.IsNotNull(mediaConfiguration))
            {
                // Sets the server connection.
                _connectorobj = new ServerConnector(new FileUploadPolicyModel(mediaConfiguration.AccessKey, mediaConfiguration.SecretKey, mediaConfiguration.BucketName, string.Empty, mediaConfiguration.URL, mediaConfiguration.NetworkUrl));
                className = mediaConfiguration.MediaServer.ClassName;
                mediaConfigurationId = mediaConfiguration.MediaConfigurationId;
                ZnodeLogging.LogMessage("GetServerConnection()", "File Upload Operation", TraceLevel.Info, mediaConfiguration);
            }
            else
            {
                if (mediaConfiguration.Server == ZnodeConstant.NetworkDrive)
                {
                    _connectorobj = new ServerConnector(new FileUploadPolicyModel(string.Empty, string.Empty, "Data/Media", "Thumbnail", mediaConfiguration.URL, mediaConfiguration.NetworkUrl));
                    className = mediaConfiguration.MediaServer.ClassName;
                    mediaConfigurationId = mediaConfiguration.MediaConfigurationId;
                }
                else
                {
                    _connectorobj = new ServerConnector(new FileUploadPolicyModel(string.Empty, string.Empty, "Data/Media", "Thumbnail", mediaConfiguration.URL, mediaConfiguration.NetworkUrl));
                    className = "LocalAgent";
                    mediaConfigurationId = mediaConfiguration.MediaConfigurationId;
                }
            }
            ZnodeLogging.LogMessage("GetServerConnection()", "File Upload Operation", TraceLevel.Info, _connectorobj);
            return _connectorobj;
        }

        // Get mediaconfiguration connector object.
        protected virtual ServerConnector GetConnectorObject(out string className, out MediaConfigurationModel mediaConfiguration)
        {
            int mediaConfigurationId = 0;
            className = string.Empty;
            mediaConfiguration = new MediaConfigurationService().GetDefaultMediaConfiguration();
            return GetServerConnection(out className, out mediaConfigurationId, mediaConfiguration);
        }

        // Delete SiteMap from media.
        public virtual bool DeleteSiteMapFromMedia(string mediaPaths)
        {
            ZnodeLogging.LogMessage("DeleteSiteMapFromMedia Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(mediaPaths))
            {
                string className;
                MediaConfigurationModel mediaConfiguration;
                ServerConnector _connectorobj = GetConnectorObject(out className, out mediaConfiguration);
                string folderName = GetFolderPath();
                // Delete the original file.
                object deletedObject = _connectorobj.CallConnector(className, MediaStorageAction.Delete, mediaPaths, folderName);

                // delete the thumbnail fil.
                return !Equals(deletedObject, null);
            }
            ZnodeLogging.LogMessage(" DeleteSiteMapFromMedia Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return false;
        }

        // Set Expands for ZnodeProductFeedType. 
        protected virtual void SetExpands(NameValueCollection expands)
        {
            expands.Add(ZnodeProductFeedEnum.ZnodeProductFeedSiteMapType.ToString().ToLower(), ZnodeProductFeedEnum.ZnodeProductFeedSiteMapType.ToString().ToLower());
            expands.Add(ZnodeProductFeedEnum.ZnodeProductFeedType.ToString().ToLower(), ZnodeProductFeedEnum.ZnodeProductFeedType.ToString().ToLower());
        }

        // Get webstore domain.
        protected virtual string GetPortalDomain(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            PortalModel model = new PortalModel();
            if (portalId > 0)
            {
                IZnodeRepository<ZnodeDomain> znodePortalDomain = new ZnodeRepository<ZnodeDomain>();

                model = (from domain in znodePortalDomain.Table.Where(x => x.PortalId == portalId && x.ApplicationType == ApplicationTypesEnum.WebStore.ToString() && x.IsActive && x.IsDefault)
                         where domain.PortalId == portalId
                         select new PortalModel
                         {
                             DomainUrl = domain.DomainName,
                         }).FirstOrDefault();
                if (IsNull(model))
                {
                    model = (from domain in znodePortalDomain.Table.Where(x => x.PortalId == portalId && x.ApplicationType == ApplicationTypesEnum.WebStore.ToString() && x.IsActive)
                             where domain.PortalId == portalId
                             select new PortalModel
                             {
                                 DomainUrl = domain.DomainName,
                             }).FirstOrDefault();
                }
            }
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return IsNotNull(model) ? model.DomainUrl : string.Empty;
        }

        // This method intends to provide a custom path for file upload.
        protected virtual string GetFolderPath()
        {
            string filePath = string.Empty;
            return string.IsNullOrEmpty(filePath) ? ZnodeConstant.FolderSiteMap : filePath;
        }

        // GetFeedDetails from database
        protected virtual List<ProductFeedModel> GetFeedDetails(string productFeedIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            IZnodeRepository<ZnodePortal> _znodePortal = new ZnodeRepository<ZnodePortal>();

            List<ProductFeedModel> productFeedModelList = (from productfeed in _productFeedRepository.Table
                                                           join sitemaptype in _productFeedSiteMapTypeRepository.Table on productfeed.ProductFeedSiteMapTypeId equals sitemaptype.ProductFeedSiteMapTypeId
                                                           join productFeedType in _productFeedTypeRepository.Table on productfeed.ProductFeedTypeId equals productFeedType.ProductFeedTypeId
                                                           join portal in _znodePortal.Table on productfeed.PortalId equals portal.PortalId
                                                           where productFeedIds.Contains(productfeed.ProductFeedId.ToString())
                                                           select new ProductFeedModel
                                                           {
                                                               ProductFeedTypeCode = productFeedType.ProductFeedTypeCode,
                                                               ProductFeedSiteMapTypeCode = sitemaptype.ProductFeedSiteMapTypeCode,
                                                               StoreName = portal.StoreName,
                                                               FileCount = productfeed.FileCount,
                                                               PortalId = portal.PortalId,
                                                               LocaleId = productfeed.LocaleId
                                                           }).ToList();

            ZnodeLogging.LogMessage("GetFeedDetails Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return productFeedModelList;
        }

        //Append sitemap url in Robots.txt file.
        protected virtual void AppendSitemapUrlInRobotsTxtContent(ProductFeedModel productFeedModel)
        {
            ZnodeLogging.LogMessage("AppendSitemapUrlInRobotsTxtContent Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(productFeedModel) && productFeedModel.ProductFeedTypeCode.Equals(ZnodeConstant.XmlSiteMap, StringComparison.InvariantCultureIgnoreCase))
                {
                    string sitemapUrl = $"\n{ZnodeConstant.FolderSiteMap}:{productFeedModel.SuccessXMLGenerationMessage}";

                    ZnodeRobotsTxt znodeRobotTxtContent = _robotsTxtRepository.Table.FirstOrDefault(x => x.PortalId == productFeedModel.PortalId);

                    if (IsNull(znodeRobotTxtContent))
                    {
                        CreateRobotTxtContentForSitemapUrl(productFeedModel, sitemapUrl);
                    }
                    else
                    {
                        bool isSitemapExistsInRobotTxtContent = Convert.ToBoolean(znodeRobotTxtContent.RobotsTxtContent?.Contains(sitemapUrl));
                        if (!isSitemapExistsInRobotTxtContent)
                        {
                            znodeRobotTxtContent.RobotsTxtContent += sitemapUrl;
                            //To update the RobotsTxtContent in ZnodeRobotsTxt table.
                            _robotsTxtRepository.Update(znodeRobotTxtContent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
        }

        //Create Robots.txt for respective portal.
        protected virtual void CreateRobotTxtContentForSitemapUrl(ProductFeedModel productFeedModel, string siteMapUrl)
        {
            ZnodeLogging.LogMessage($"Creating Robots.txt for portalId: {productFeedModel.PortalId}.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(productFeedModel))
                {
                    RobotsTxtModel robotTxtModel = new RobotsTxtModel();
                    robotTxtModel.PortalId = productFeedModel.PortalId;
                    robotTxtModel.StoreName = productFeedModel.StoreName;
                    robotTxtModel.RobotsTxtContent = siteMapUrl;
                    //Insert RobotsTxtContent for sitemap url in ZnodeRobotsTxt.
                    _robotsTxtRepository.Insert(robotTxtModel?.ToEntity<ZnodeRobotsTxt>());

                    ZnodeLogging.LogMessage($"Robots.txt successfully created for portalId: {productFeedModel.PortalId}.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
        }

        //Remove sitemap url from robots.txt.
        protected virtual void DeleteSitemapUrlFromRobotsTxtContent(string productFeedIds, List<ProductFeedModel> productFeedModelList, List<int> productFeedPortalIds)
        {
            ZnodeLogging.LogMessage("DeleteSitemapUrlFromRobotsTxtContent Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                List<ProductFeedModel> productFeedList = null;
                List<ZnodeRobotsTxt> znodeRobotTxtContent = _robotsTxtRepository.Table.Where(x => productFeedPortalIds.Contains(x.PortalId ?? 0))?.ToList();
                string content = string.Empty;
                string sitemapUrl = string.Empty;
                foreach (int portalId in productFeedPortalIds)
                {
                    productFeedList = productFeedModelList.Where(x => x.PortalId == portalId && x.ProductFeedTypeCode.Equals(ZnodeConstant.XmlSiteMap, StringComparison.InvariantCultureIgnoreCase) && !productFeedIds.Contains(x.ProductFeedId.ToString())).ToList();
                    if (!(productFeedList?.Count > 0))
                    {
                        sitemapUrl = $"\n{ZnodeConstant.FolderSiteMap}:{HttpContext.Current.Request.Url.Scheme}://{GetPortalDomain(portalId)}/{ZnodeConstant.SitemapRoute}";
                        content = znodeRobotTxtContent.FirstOrDefault(x => x.PortalId == portalId).RobotsTxtContent.Replace(sitemapUrl, "");
                        znodeRobotTxtContent.FirstOrDefault(x => x.PortalId == portalId).RobotsTxtContent = content;
                    }
                }
                //To update the RobotsTxtContent in ZnodeRobotsTxt table.
                _robotsTxtRepository.BatchUpdate(znodeRobotTxtContent);
                ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
        }

        //Get feed details by portal Ids.
        protected virtual List<ProductFeedModel> GetFeedDetailsByPortalId(string productFeedIds, List<int> portalIdList)
        {
            ZnodeLogging.LogMessage("GetFeedDetailsByPortalId Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            IZnodeRepository<ZnodePortal> _znodePortal = new ZnodeRepository<ZnodePortal>();

            List<ProductFeedModel> productFeedModelList = (from productfeed in _productFeedRepository.Table
                                                           join sitemaptype in _productFeedSiteMapTypeRepository.Table on productfeed.ProductFeedSiteMapTypeId equals sitemaptype.ProductFeedSiteMapTypeId
                                                           join productFeedType in _productFeedTypeRepository.Table on productfeed.ProductFeedTypeId equals productFeedType.ProductFeedTypeId
                                                           join portal in _znodePortal.Table on productfeed.PortalId equals portal.PortalId
                                                           where portalIdList.Contains(portal.PortalId)
                                                           select new ProductFeedModel
                                                           {
                                                               ProductFeedTypeCode = productFeedType.ProductFeedTypeCode,
                                                               ProductFeedSiteMapTypeCode = sitemaptype.ProductFeedSiteMapTypeCode,
                                                               StoreName = portal.StoreName,
                                                               FileCount = productfeed.FileCount,
                                                               PortalId = portal.PortalId,
                                                               LocaleId = productfeed.LocaleId,
                                                               ProductFeedId = productfeed.ProductFeedId
                                                           }).ToList();

            ZnodeLogging.LogMessage("GetFeedDetailsByPortalId Execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return productFeedModelList;
        }
    }
}
