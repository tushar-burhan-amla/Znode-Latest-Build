using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Xml.Linq;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Helper;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.ElasticSearch;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class CatalogService : BaseService, ICatalogService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePimCatalog> _pimCatalogRepository;
        private readonly IZnodeRepository<ZnodePimCategoryHierarchy> _pimCategoryHierarchyRepository;
        private readonly IZnodeRepository<ZnodePublishCatalogLog> _publishCatalogLogRepository;
        private readonly IZnodeRepository<ZnodePublishedXml> _publishedXmlRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetail> _seoDetailRepository;
        private readonly ProductAssociationHelper productAssociationHelper = null;
        private readonly IZnodeRepository<ZnodeSearchIndexServerStatu> _searchIndexServerStatusRepository;
        private readonly IZnodeRepository<ZnodePimCategoryProduct> _pimCategoryProductRepository;
        private readonly ISearchService _searchService;
        protected readonly IDefaultDataService defaultDataService;
        #endregion Private Variables

        #region Constructor

        public CatalogService()
        {
            _pimCatalogRepository = new ZnodeRepository<ZnodePimCatalog>();
            _pimCategoryHierarchyRepository = new ZnodeRepository<ZnodePimCategoryHierarchy>();
            _publishedXmlRepository = new ZnodeRepository<ZnodePublishedXml>();
            _publishCatalogLogRepository = new ZnodeRepository<ZnodePublishCatalogLog>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _seoDetailRepository = new ZnodeRepository<ZnodeCMSSEODetail>();
            productAssociationHelper = new ProductAssociationHelper();
            _searchIndexServerStatusRepository = new ZnodeRepository<ZnodeSearchIndexServerStatu>();
            _pimCategoryProductRepository = new ZnodeRepository<ZnodePimCategoryProduct>();
            _searchService = GetService<ISearchService>();
            defaultDataService = GetService<IDefaultDataService>();
        }

        #endregion Constructor

        #region Public Methods

        //Get a list of Catalogs.
        public virtual CatalogListModel GetCatalogs(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set parameters of SP Znode_GetCatalogList:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<CatalogModel> objStoredProc = new ZnodeViewRepository<CatalogModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            List<CatalogModel> publishCatalogLogs = objStoredProc.ExecuteStoredProcedureList("Znode_GetCatalogList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("publishCatalogLogs list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { publishCatalogLogsListCount = publishCatalogLogs?.Count });

            CatalogListModel publishCatalogLogList = new CatalogListModel { Catalogs = publishCatalogLogs };
            publishCatalogLogList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishCatalogLogList;
        }

        //Get a Catalog using catalogId.
        public virtual CatalogModel GetCatalog(int pimCatalogId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { pimCatalogId = pimCatalogId });
            if (pimCatalogId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, PIM_Resources.ErrorCatalogIdLessThanOne);
            CatalogModel catalogModel = _pimCatalogRepository.GetById(pimCatalogId).ToModel<CatalogModel>();

            BindDefaultPortal(catalogModel);
            return catalogModel;
        }

        //Bind Default Portal to Catalog
        private void BindDefaultPortal(CatalogModel catalogModel)
        {
            ZnodeRepository<ZnodePortal> portalRepository = new ZnodeRepository<ZnodePortal>();
            catalogModel.DefaultStore = portalRepository.Table.FirstOrDefault(m => m.PortalId == catalogModel.PortalId)?.StoreName;
        }

        //Creates a new Catalog
        public virtual CatalogModel CreateCatalog(CatalogModel catalogModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNull(catalogModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodePimCatalog catalog = _pimCatalogRepository.Insert(catalogModel.ToEntity<ZnodePimCatalog>());
            ZnodeLogging.LogMessage(catalog?.PimCatalogId > 0 ? string.Format(PIM_Resources.SuccessCreateCatalog, catalogModel.CatalogName) : string.Format(PIM_Resources.ErrorCreateCatalog, catalogModel.CatalogName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return catalog?.PimCatalogId > 0 ? catalog.ToModel<CatalogModel>() : null;
        }

        //Updates an already existing Catalog.
        public virtual bool UpdateCatalog(CatalogModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, PIM_Resources.ErrorCatalogModelNull);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { CatalogModel = model });
            bool status = _pimCatalogRepository.Update(model.ToEntity<ZnodePimCatalog>());
            ZnodeLogging.LogMessage(status ? string.Format(PIM_Resources.SuccessUpdateCatalog, model.CatalogName) : string.Format(PIM_Resources.ErrorUpdateCatalog, model.CatalogName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return status;
        }

        //Copy the existing Catalog.
        public virtual bool CopyCatalog(CatalogModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("CatalogId", model.PimCatalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("AccountId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("CatalogName", model.CatalogName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("CatalogCode", model.CatalogCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("CopyAllData", model.CopyAllData, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_CopyPimCatalog @CatalogId, @AccountId, @CatalogCode, @CatalogName, @CopyAllData, @Status OUT", 4, out status);
            if (deleteResult.FirstOrDefault()?.Status.Value ?? false)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessCopyCatalog, model.CatalogName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorCopyCatalog, model.CatalogName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.AlreadyExistCatalogName);
            }

        }

        //Deletes a Catalog using catalogId.
        public virtual bool DeleteCatalog(CatalogDeleteModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(model?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorCatalogIdNull);
            ZnodeLogging.LogMessage("Catalog Ids to delete catalog:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.Ids);
            View_ReturnBooleanWithMessage deleteResult = null;
            IZnodeViewRepository<View_ReturnBooleanWithMessage> objStoredProc = new ZnodeViewRepository<View_ReturnBooleanWithMessage>();
            objStoredProc.SetParameter("PimCatalogIds", model.Ids, ParameterDirection.Input, DbType.String);

            // Delete Publish and PIM Catalog
            if (model.IsDeletePublishCatalog)
            {
                objStoredProc.SetParameter("IsDeleteFromPublish", true, ParameterDirection.Input, DbType.Boolean);
                deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimCatalog  @PimCatalogIds, @IsDeleteFromPublish")?.FirstOrDefault();
                if (IsNotNull(deleteResult?.MessageDetails))
                {
                    string publishCatalogId = deleteResult.MessageDetails;
                    View_ReturnBoolean result = null;
                    IZnodeViewRepository<View_ReturnBoolean> storedProcedure = new ZnodeViewRepository<View_ReturnBoolean>();
                    storedProcedure.SetParameter("@PublishCatalogId", publishCatalogId, ParameterDirection.Input, DbType.String);
                    result = storedProcedure.ExecuteStoredProcedureList("Znode_DeletePublishCatalogData  @PublishCatalogId")?.FirstOrDefault();
                }
            }
            else
                //Delete Pim catalog
                deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimCatalog @PimCatalogIds")?.FirstOrDefault();

            ZnodeLogging.LogMessage(deleteResult?.Status.GetValueOrDefault() ?? false ? PIM_Resources.SuccessDeleteCatalog : PIM_Resources.ErrorDeleteCatalog, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return deleteResult?.Status.GetValueOrDefault() ?? false;
        }

        //Get tree structure of categories.
        public virtual ContentPageTreeModel GetCatgoryTreeNode(CatalogAssociationModel catalogAssociationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter catalogAssociationModel properties:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { CatalogId = catalogAssociationModel?.CatalogId, ProfileCatalogId = catalogAssociationModel?.ProfileCatalogId, LocaleId = catalogAssociationModel?.LocaleId });
            if (catalogAssociationModel?.CatalogId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorCatalogIdLessThanOne);

            IZnodeViewRepository<TreeModel> objStoredProc = new ZnodeViewRepository<TreeModel>();
            if (IsNotNull(catalogAssociationModel))
            {
                objStoredProc.SetParameter(ZnodePimCatalogEnum.PimCatalogId.ToString(), catalogAssociationModel.CatalogId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), catalogAssociationModel.LocaleId, ParameterDirection.Input, DbType.Int32);         
            }

            //Get all path from database.
            List<ContentPageTreeModel> list = objStoredProc.ExecuteStoredProcedureList("ZNode_GetCategoryHierarchy @PimCatalogId,@LocaleId")?.Select(x => new ContentPageTreeModel
            {
                Text = x.CategoryValue,
                Id = x.PimCategoryHierarchyId,
                ParentId = x.ParentPimCategoryHierarchyId.GetValueOrDefault(),
                DisplayOrder = x.DisplayOrder,
                PimCategoryId = x.PimCategoryId.GetValueOrDefault()
            }).ToList();
            ZnodeLogging.LogMessage("ContentPageTreeModel list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { ContentPageTreeModelListCount = list?.Count });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Convert path to parent child pattern.
            return GetAllNode(list).FirstOrDefault();
        }

        //Get Associated Catalog Hierarchy list on the basis of pim product Id.
        public virtual List<CatalogTreeModel> GetAssociatedCatalogHierarchy(int pimProductId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { pimProductId = pimProductId });

            if (pimProductId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorCatalogIdGreaterThanOne);

            IZnodeViewRepository<TreeModel> objStoredProc = new ZnodeViewRepository<TreeModel>();
            objStoredProc.SetParameter(ZnodePimProductEnum.PimProductId.ToString(), pimProductId, ParameterDirection.Input, DbType.Int32);

            //Get Catalog from database.
            List<ContentPageTreeModel> catalogList = objStoredProc.ExecuteStoredProcedureList("Znode_GetCatalogCategoryHierarchy @PimProductId")?.Select(x => new ContentPageTreeModel
            {
                Text = x.CatalogName,
                PimCatalogId = x.PimCatalogId,
                CategoryValue = x.CategoryValue
            }).ToList();
            ZnodeLogging.LogMessage("catalogList list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogListCount = catalogList?.Count });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Convert path to parent child pattern.
            return GetCatalogTreeNodes(catalogList);
        }

        //Get Catalog Tree Nodes
        protected virtual List<CatalogTreeModel> GetCatalogTreeNodes(List<ContentPageTreeModel> catalogList)
        {
            List<CatalogTreeModel> CatalogHierarchyList = new List<CatalogTreeModel>();
            IZnodeRepository<ZnodePublishCatalogEntity> _publishCatalogEntity = new ZnodeRepository<ZnodePublishCatalogEntity>(HelperMethods.Context);
            List<int> pageTreeCatalog = catalogList?.Select(x => x.PimCatalogId.Value)?.ToList();
            List<int> publishCatalogs = null;
            if( pageTreeCatalog.Count > 0)
            publishCatalogs = _publishCatalogEntity.Table.Where(x => pageTreeCatalog.Contains(x.ZnodeCatalogId))?.Select(m=> m.ZnodeCatalogId)?.Distinct().ToList();

            if (catalogList?.Count > 0)
            {
                foreach (var item in catalogList)
                {
                    if (!CatalogHierarchyList.Any(m => m.CatalogName == item.Text))
                    {
                        CatalogTreeModel catalogtreeModel = GetCatalogNode(catalogList, item, publishCatalogs);               
                        CatalogHierarchyList.Add(catalogtreeModel);
                    }
                }
            }
            return CatalogHierarchyList;
        }

        //Get Single Catalog Nodes
        protected virtual CatalogTreeModel GetCatalogNode(List<ContentPageTreeModel> catalogList,ContentPageTreeModel item, List<int> publishCatalogs)
        {
            CatalogTreeModel catalogtreeModel = new CatalogTreeModel();
            catalogtreeModel.CatalogName = item.Text;
            List<ContentPageTreeModel> categories = catalogList.Where(m => m.PimCatalogId == item.PimCatalogId).Select(m => new ContentPageTreeModel { Text = m.CategoryValue }).ToList();
            ZnodeLogging.LogMessage("categories list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { categoriesListCount = categories?.Count });
            catalogtreeModel.Children = categories;
            catalogtreeModel.IsCatalogPublished = publishCatalogs.Any(m => m == item.PimCatalogId.GetValueOrDefault());
            return catalogtreeModel;
        }

        // Get associated categories.
        public virtual CatalogAssociateCategoryListModel GetAssociatedCategories(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int localeId = 0;
            if (IsNotNull(filters))
            {
                localeId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower())?.FirstOrDefault()?.Item3);
                ZnodeLogging.LogMessage("localeId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { localeId = localeId });
                filters.Remove(filters.Where(filterTuple => filterTuple.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower())?.FirstOrDefault());
            }
            CatalogAssociateCategoryListModel categoryListModel = GetXmlCategory(filters, sorts, page);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return categoryListModel;
        }

        //Get locale id from filter otherwise set default
        public static int GetLocaleId(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int localeId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale);
            ZnodeLogging.LogMessage("localeId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { localeId = localeId });
            //Checking For LocaleId exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()))
            {
                localeId = Convert.ToInt32(filters.Where(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower());
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return localeId;
        }

        //Unassociate categories and product from catalog.
        [Obsolete("This method is not in use now, as new method has been introduced to unassociate category")]
        public virtual bool UnAssociateCategoryFromCatalog(CatalogAssociationModel catalogAssociationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNull(catalogAssociationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogAssociationModel = catalogAssociationModel });

            bool isUnAssociate = false;

            //Remove the category from profile catalog
            if (catalogAssociationModel.ProfileCatalogId > 0)
                isUnAssociate = UnAssociateCategoryFromProfile(catalogAssociationModel);

            //Remove the categories.
            else if (IsNotNull(catalogAssociationModel.PimCategoryHierarchyId) && catalogAssociationModel.ProfileCatalogId <= 0)
                isUnAssociate = UnAssociateCategories(catalogAssociationModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return isUnAssociate;
        }


        //Unassociate categories and product from catalog.
        [Obsolete("This method is not in use now, As product Association/UnAssociation has been removed from catalog category")]
        public virtual bool UnAssociateProductFromCatalog(CatalogAssociationModel catalogAssociationModel)
        {
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogAssociationModel = catalogAssociationModel });

            return UnassociateProductOnCategoryMerchandise(catalogAssociationModel);
        }


        [Obsolete("This method is not in use now, As product Association/UnAssociation has been removed from catalog category also store procedure")]
        protected virtual bool UnassociateProductOnCategoryMerchandise(CatalogAssociationModel catalogAssociationModel)
        {
            if (Equals(catalogAssociationModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogAssociationModel = catalogAssociationModel });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeProfileCatalogEnum.ProfileCatalogId.ToString(), catalogAssociationModel.ProfileCatalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("PimCatalogId", catalogAssociationModel.CatalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("PimCategoryId", catalogAssociationModel.CategoryId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("PimProductId", catalogAssociationModel.ProductIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), catalogAssociationModel.PimCategoryHierarchyId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Boolean);
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimCatalogProducts @ProfileCatalogId, @PimCatalogId, @PimCategoryId, @PimProductId, @PimCategoryHierarchyId, @Status OUT", 4, out status);
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessRemoveProduct, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorRemoveProduct, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        public virtual ProductDetailsListModel GetCategoryAssociatedProducts(CatalogAssociationModel catalogAssociationModel, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogAssociationModel = catalogAssociationModel });

            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            int portalId = GetPortalId(filters);
            SetProductStatusFilter(filters);

            string whereClause = ProductService.GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("pageListModel and whereClause to get ProductDetailsModel list:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { pageListModel = pageListModel?.ToDebugString(), whereClause = whereClause });
            string attributeCode = GetAttributeCodes(filters);
            IZnodeViewRepository<ProductDetailsModel> objStoredProc = new ZnodeViewRepository<ProductDetailsModel>();
            objStoredProc.SetParameter("@WhereClause", whereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), catalogAssociationModel.LocaleId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodePimCategoryEnum.PimCategoryId.ToString(), catalogAssociationModel.CategoryId.GetValueOrDefault(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodePimCatalogEnum.PimCatalogId.ToString(), catalogAssociationModel.CatalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", catalogAssociationModel.IsAssociated, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("@AttributeCode", attributeCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), catalogAssociationModel.PimCategoryHierarchyId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId.ToString(), ParameterDirection.Input, DbType.Int32);
            //List of all products associated to category.
            List<ProductDetailsModel> list = objStoredProc.ExecuteStoredProcedureList("ZNode_GetCatalogCategoryProducts @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PimCategoryId,@PimCatalogId,@IsAssociated,@AttributeCode,@PimCategoryHierarchyId,@PortalId", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("ProductDetailsModel list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { ProductDetailsModelListCount = list?.Count });

            ProductDetailsListModel listModel = new ProductDetailsListModel { ProductDetailList = list };
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get details(Display order, active status, etc.)of category associated to catalog.
        public virtual CatalogAssociateCategoryModel GetAssociateCategoryDetails(CatalogAssociateCategoryModel catalogAssociateCategoryModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter catalogAssociateCategoryModel properties:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { PimCatalogId = catalogAssociateCategoryModel?.PimCatalogId, PimCategoryHierarchyId = catalogAssociateCategoryModel?.PimCategoryHierarchyId });

            //check for pimCatalogId, pimCategoryId greater than 1.
            if (HelperUtility.IsNotNull(catalogAssociateCategoryModel) && catalogAssociateCategoryModel.PimCatalogId < 1 && catalogAssociateCategoryModel.PimCategoryHierarchyId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, PIM_Resources.ErrorCatalogorCategoryIdLessThanOne);

            ZnodeRepository<ZnodePimAttribute> _pimAttributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            ZnodeRepository<ZnodePimCategoryAttributeValue> _pimCategoryAttributeValueRepository = new ZnodeRepository<ZnodePimCategoryAttributeValue>();
            ZnodeRepository<ZnodePimCategoryAttributeValueLocale> _pimCategoryAttributeValueLocaleRepository = new ZnodeRepository<ZnodePimCategoryAttributeValueLocale>();

            return (from category in _pimCategoryHierarchyRepository.Table
                    where category.PimCatalogId == catalogAssociateCategoryModel.PimCatalogId && category.PimCategoryHierarchyId == catalogAssociateCategoryModel.PimCategoryHierarchyId
                    from categoryAttribute in _pimAttributeRepository.Table
                    join categoryAttributeValue in _pimCategoryAttributeValueRepository.Table
                    on categoryAttribute.PimAttributeId equals categoryAttributeValue.PimAttributeId
                    join categoryAttributeValueLocale in _pimCategoryAttributeValueLocaleRepository.Table
                    on categoryAttributeValue.PimCategoryAttributeValueId equals categoryAttributeValueLocale.PimCategoryAttributeValueId
                    where categoryAttribute.AttributeCode == "CategoryName" && categoryAttributeValue.PimCategoryId == category.PimCategoryId && categoryAttributeValueLocale.LocaleId == catalogAssociateCategoryModel.LocaleId
                    select new CatalogAssociateCategoryModel()
                    {
                        PimCatalogId = category.PimCatalogId,
                        ParentPimCategoryHierarchyId = category.ParentPimCategoryHierarchyId,
                        PimCategoryId = category.PimCategoryId,
                        CategoryValue = categoryAttributeValueLocale.CategoryValue,
                        DisplayOrder = category.DisplayOrder,
                        IsActive = category.IsActive,
                        ActivationDate = category.ActivationDate,
                        ExpirationDate = category.ExpirationDate,
                        PimCategoryHierarchyId = category.PimCategoryHierarchyId,
                    })?.FirstOrDefault();
        }

        //Update details(Display order, active status, etc.)of category associated to catalog.
        public virtual bool UpdateAssociateCategoryDetails(CatalogAssociateCategoryModel catalogAssociateCategoryModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Check if model is null.
            if (HelperUtility.IsNull(catalogAssociateCategoryModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);
            ZnodeLogging.LogMessage("CatalogAssociateCategoryModel details:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, catalogAssociateCategoryModel);
            if (catalogAssociateCategoryModel.UpdateDisplayOrder)
                return UpdateDisplayOrder(catalogAssociateCategoryModel);

            catalogAssociateCategoryModel.ParentPimCategoryHierarchyId = catalogAssociateCategoryModel.ParentPimCategoryHierarchyId == 0 ? null : catalogAssociateCategoryModel.ParentPimCategoryHierarchyId;

            if (catalogAssociateCategoryModel.PimCategoryHierarchyId < 1)
            {
                //set filters to get category hierarchy.
                FilterCollection filters = SetFiltersToSetDisplayOrder(catalogAssociateCategoryModel);

                //Generate whereclause.
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("whereClauseModel to get catalogAssociateCategoryEntityList:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);
                //Get the list of selected and its previous category.
                IList<ZnodePimCategoryHierarchy> catalogAssociateCategoryEntityList = _pimCategoryHierarchyRepository.GetEntityList(whereClauseModel.WhereClause);

                //Get data for selected category.
                ZnodePimCategoryHierarchy catalogAssociateCategoryEntity = catalogAssociateCategoryEntityList.Where(x => x.PimCategoryId == catalogAssociateCategoryModel.PimCategoryId)?.FirstOrDefault();

                if (catalogAssociateCategoryModel.CategoryId > 0)
                {
                    //Get data for parent category.
                    ZnodePimCategoryHierarchy catalogAssociateCategory = catalogAssociateCategoryEntityList.Where(x => x.PimCategoryId == catalogAssociateCategoryModel.CategoryId)?.FirstOrDefault();

                    if (IsNotNull(catalogAssociateCategory))
                    {
                        //set display order.
                        if (catalogAssociateCategoryModel.IsMoveUp)
                        {
                            catalogAssociateCategoryModel.DisplayOrder = catalogAssociateCategory.DisplayOrder > 1 ? catalogAssociateCategory.DisplayOrder - 1 : catalogAssociateCategoryEntity?.DisplayOrder;
                            catalogAssociateCategoryModel.ParentPimCategoryHierarchyId = catalogAssociateCategory.ParentPimCategoryHierarchyId;
                        }
                        else if (catalogAssociateCategoryModel.IsMoveDown)
                        {
                            catalogAssociateCategoryModel.DisplayOrder = catalogAssociateCategory.DisplayOrder == 999 ? catalogAssociateCategory.DisplayOrder : catalogAssociateCategory.DisplayOrder + 1;
                            catalogAssociateCategoryModel.ParentPimCategoryHierarchyId = catalogAssociateCategory.ParentPimCategoryHierarchyId;
                        }
                    }
                }

                if (IsNotNull(catalogAssociateCategoryEntity))
                {
                    catalogAssociateCategoryModel.PimCategoryHierarchyId = catalogAssociateCategoryEntity.PimCategoryHierarchyId;
                    catalogAssociateCategoryModel.IsActive = catalogAssociateCategoryEntity.IsActive;
                    catalogAssociateCategoryModel.ExpirationDate = catalogAssociateCategoryEntity.ExpirationDate;
                    catalogAssociateCategoryModel.ActivationDate = catalogAssociateCategoryEntity.ActivationDate;
                }
            }

            bool isUpdated = _pimCategoryHierarchyRepository.Update(catalogAssociateCategoryModel.ToEntity<ZnodePimCategoryHierarchy>());

            ZnodeLogging.LogMessage(isUpdated ? string.Format(PIM_Resources.SuccessUpdateAssociateCategoryDetails, catalogAssociateCategoryModel.PimCategoryId) : PIM_Resources.ErrorUpdateAssociateCategoryDetails, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return isUpdated;
        }

        //Move category.
        public virtual bool MoveCategory(CatalogAssociateCategoryModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, PIM_Resources.ErrorCatalogAssociateCategoryModelNull);

            if (IsCategorySame(model.PimCategoryHierarchyId, model.ParentPimCategoryHierarchyId.GetValueOrDefault()))
                throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.ErrorCategoryExists);

            ZnodeLogging.LogMessage("Input parameter catalogAssociateCategoryModel properties:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { PimCatalogId = model?.PimCatalogId, PimCategoryHierarchyId = model?.PimCategoryHierarchyId, ParentPimCategoryHierarchyId = model?.ParentPimCategoryHierarchyId });

            ZnodePimCategoryHierarchy znodePimCategoryHierarchy = _pimCategoryHierarchyRepository.Table.FirstOrDefault(x => x.PimCatalogId == model.PimCatalogId && x.PimCategoryHierarchyId == model.PimCategoryHierarchyId);

            if (IsNotNull(znodePimCategoryHierarchy))
            {
                model.ParentPimCategoryHierarchyId = model.ParentPimCategoryHierarchyId == 1 ? null : model.ParentPimCategoryHierarchyId;

                //Assign value to parent media path id.
                if (IsNotNull(_pimCategoryHierarchyRepository.Table.FirstOrDefault(x => x.PimCatalogId == model.PimCatalogId & x.ParentPimCategoryHierarchyId == model.ParentPimCategoryHierarchyId & x.PimCategoryId == znodePimCategoryHierarchy.PimCategoryId)))
                    throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.ErrorCategoryExists);

                znodePimCategoryHierarchy.ParentPimCategoryHierarchyId = model.ParentPimCategoryHierarchyId != 1 ? model.ParentPimCategoryHierarchyId : null;
                znodePimCategoryHierarchy.DisplayOrder = GetMaxDisplayOrder(model.PimCatalogId.GetValueOrDefault(), znodePimCategoryHierarchy.ParentPimCategoryHierarchyId) + 75000;

                bool result = _pimCategoryHierarchyRepository.Update(znodePimCategoryHierarchy);
                ZnodeLogging.LogMessage(result ? PIM_Resources.SuccessMoveCategory : PIM_Resources.ErrorMoveCategory, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return result;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return false;
        }

        //Gets the attribute code form filters.
        protected static string GetAttributeCodes(FilterCollection filters)
        {
            if (filters?.Count > 0)
            {
                string attributeCode = string.Join(",", filters.Select(x => x.FilterName).ToArray());
                if (!string.IsNullOrEmpty(attributeCode) && attributeCode.Contains("|"))
                    attributeCode = attributeCode.Replace('|', ',');
                return attributeCode;
            }
            return string.Empty;
        }

        protected virtual void PublishSuccessCallBack(IAsyncResult ar)
        {
            AsyncResult result = ar as AsyncResult;
        }

        public virtual void UpdatePublishedProductAssociatedData()
        {
            try
            {
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_PublishAssociatedProduct");
                if (!deleteResult.FirstOrDefault().Status.Value)
                {
                    ZnodeLogging.LogMessage("Failed to update associated products of publish products", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                }
                ZnodeLogging.LogMessage("Successfully update associated products of publish products", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.GenericExceptionDuringPublish, ex.Message);
            }

        }

        //Publish catalog category associated products.
        public virtual PublishedModel PublishCategoryProducts(int pimCatalogId, int pimCategoryHierarchyId, string revisionType)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { pimCatalogId = pimCatalogId, pimCategoryHierarchyId = pimCategoryHierarchyId, revisionType = revisionType });
            int categoryId = _pimCategoryHierarchyRepository.Table.FirstOrDefault(n => n.PimCategoryHierarchyId == pimCategoryHierarchyId)?.PimCategoryId ?? 0;

            // Update : Znode has removed support of product publish from category publish, hence directly made call to category Publish method
            return GetService<ICategoryService>()?.Publish(new ParameterModel() { Ids = categoryId.ToString(), RevisionType = revisionType, publishCataLogId = pimCatalogId });
        }

        //Get unique job id. On the basis of job id we show the publish notification / progress bar.
        protected virtual Guid GetUniqueJobId()
            => Guid.NewGuid();

        //Get category publish progress message.
        private string GetcategoryPublishProgressMessage(int pimCatalogId)
        {
            string catalogName = _pimCatalogRepository.GetById(pimCatalogId)?.CatalogName;
            ZnodeLogging.LogMessage("CatalogName:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, catalogName);
            return IsNotNull(catalogName) ? "Category from " + catalogName + " catalog" : "Category";
        }


        [Obsolete]
        //Reference in unused method
        //Update SEO product publish status
        private void UpdateSeoPublishStatusBySEOCode(List<string> seoCodes)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("SeoCodes:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, seoCodes);
            List<ZnodeCMSSEODetail> cMSSEODetailList = _seoDetailRepository.Table.Where(x => seoCodes.Contains(x.SEOCode)).ToList();
            foreach (var cMSSEODetail in cMSSEODetailList)
            {
                cMSSEODetail.IsPublish = true;
                _seoDetailRepository.Update(cMSSEODetail);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
        }

        //Delete publish Catalog along with associated category and products
        public virtual bool DeletePublishCatalog(int publishCatalogId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { publishCatalogId = publishCatalogId });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("publishCatalogId", publishCatalogId.ToString(), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("IsDeleteCatalogId", true, ParameterDirection.Input, DbType.Boolean);

            //Delete Publish Catalog and its associated items
            View_ReturnBoolean deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePublishCatalog @publishCatalogId,@IsDeleteCatalogId")?.FirstOrDefault();

            if (deleteResult?.Status.GetValueOrDefault() ?? false)
            {
                //Delete Publish Catalog and its associated items 
                string _publishCatalogId = Convert.ToString(publishCatalogId);
                View_ReturnBoolean result = null;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProcedure = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter("@PublishCatalogId", _publishCatalogId, ParameterDirection.Input, DbType.String);
                result = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePublishCatalogData  @PublishCatalogId")?.FirstOrDefault();
            }
            ZnodeLogging.LogMessage(deleteResult?.Status.GetValueOrDefault() ?? false ? PIM_Resources.SuccessDeletePublishCatalog : PIM_Resources.ErrorDeletePublishCatalog, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return deleteResult?.Status.GetValueOrDefault() ?? false;
        }

        //Get a Catalog Publish Status.
        public virtual PublishCatalogLogListModel GetCatalogPublishStatus(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set parameters of SP Znode_GetPublishStatus:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<PublishCatalogLogModel> objStoredProc = new ZnodeViewRepository<PublishCatalogLogModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            List<PublishCatalogLogModel> publishCatalogLogs = objStoredProc.ExecuteStoredProcedureList("Znode_GetPublishStatus @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("publishCatalogLogs list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { publishCatalogLogsListCount = publishCatalogLogs?.Count });

            PublishCatalogLogListModel publishCatalogLogList = new PublishCatalogLogListModel { PublishCatalogLogList = publishCatalogLogs };
            publishCatalogLogList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishCatalogLogList;
        }


        //Update display order product associated to catalog category.
        public virtual bool UpdateCatalogCategoryProduct(CatalogAssociationModel catalogAssociationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            bool isUpdated = false;

            if (IsNull(catalogAssociationModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogAssociationModel = catalogAssociationModel });

            int? categoryId = _pimCategoryHierarchyRepository.Table.FirstOrDefault(x => x.PimCatalogId == catalogAssociationModel.CatalogId & x.PimCategoryHierarchyId == catalogAssociationModel.PimCategoryHierarchyId)?.PimCategoryId.GetValueOrDefault();

            if (categoryId != null)
            {
                ZnodePimCategoryProduct pimCategoryProduct = _pimCategoryProductRepository.Table.FirstOrDefault(x => x.PimCategoryId == categoryId & x.PimProductId == catalogAssociationModel.ProductId);

                if (pimCategoryProduct != null)
                {
                    pimCategoryProduct.DisplayOrder = catalogAssociationModel.DisplayOrder;
                    pimCategoryProduct.ModifiedDate = catalogAssociationModel.ModifiedDate;
                    isUpdated = _pimCategoryProductRepository.Update(pimCategoryProduct);
                }
            }
            ZnodeLogging.LogMessage(isUpdated ? string.Format(PIM_Resources.SuccessUpdateAssociateProductDetails, catalogAssociationModel.ProductId) : PIM_Resources.ErrorUpdateAssociateProductDetails, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isUpdated;
        }

        //If catalog publish is true then create index for its all associated stores.
        public virtual PortalIndexModel AssociatedPortalCreateIndex(int publishCatalogId, PortalIndexModel portalIndex, bool isPreviewProductionEnabled, string revisionType = null)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { publishCatalogId = publishCatalogId, revisionType = revisionType });

                ISearchService searchService = GetService<ISearchService>();

                if (portalIndex?.CatalogIndexId > 0)
                {
                    portalIndex.CreatedBy = GetLoginUserId();
                    portalIndex.ModifiedBy = GetLoginUserId();
                    portalIndex.RevisionType = revisionType;
                    portalIndex.PublishCatalogId = publishCatalogId;
                    portalIndex.IsPreviewProductionEnabled = isPreviewProductionEnabled;
                    searchService.InsertCreateIndexData(portalIndex);

                    if (!CheckIndexCreationSucceed(Convert.ToInt32(portalIndex?.SearchCreateIndexMonitorId)))
                        throw new ZnodeException(ErrorCodes.CreationFailed, "Create Index failed.");
                    ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessCreateIndexForPortal, publishCatalogId), ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                }
                else
                {
                    IPublishedCatalogDataService _publishedCatalogDataService = GetService<IPublishedCatalogDataService>();
                    portalIndex = searchService.InsertCreateIndexData(new PortalIndexModel() { IndexName = PublishHelper.GetIndexName(_publishedCatalogDataService.GetPublishCatalogById(publishCatalogId)?.CatalogName), PublishCatalogId = publishCatalogId, CreatedBy = GetLoginUserId(), ModifiedBy = GetLoginUserId(), RevisionType = revisionType, IsPreviewProductionEnabled = isPreviewProductionEnabled });
                    if (!CheckIndexCreationSucceed(Convert.ToInt32(portalIndex?.SearchCreateIndexMonitorId)))
                        throw new ZnodeException(ErrorCodes.CreationFailed, "Create Index failed.");
                    ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessCreateIndexForPortal, publishCatalogId), ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                throw;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorCreateIndexForPortal, publishCatalogId), ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return portalIndex;
        }



        //Get Portal id from filter otherwise set default
        protected virtual int GetPortalId(FilterCollection filters)
        {
            int PortalId = 0;
            if (filters.Exists(x => x.Item1 == ZnodePortalEnum.PortalId.ToString().ToLower()))
            {
                PortalId = Convert.ToInt32(filters.Where(x => x.Item1 == ZnodePortalEnum.PortalId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
                filters.RemoveAll(x => x.Item1 == ZnodePortalEnum.PortalId.ToString().ToLower());
            }
            ZnodeLogging.LogMessage("PortalId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, PortalId);
            return PortalId;
        }

        //Check whether or not index creation completed or not. 
        protected virtual bool CheckIndexCreationSucceed(int searchCreateIndexMonitorId)
        {
            var searchIndexServerStatus = _searchIndexServerStatusRepository.Table.FirstOrDefault(x => x.SearchIndexMonitorId == searchCreateIndexMonitorId);
            if (!string.IsNullOrEmpty(searchIndexServerStatus?.Status) && !searchIndexServerStatus.Status.Equals("Complete", StringComparison.InvariantCultureIgnoreCase))
                return false;
            return true;
        }

        //Set Product status filter
        protected virtual void SetProductStatusFilter(FilterCollection filters)
        {
            bool isActive = false;
            if (Equals(filters?.Exists(x => x.Item1.ToLower() == ZnodeConstant.IsActive.ToLower()), true))
            {
                isActive = Equals(filters?.FirstOrDefault(x => x.Item1.ToLower() == ZnodeConstant.IsActive.ToLower()).FilterValue?.Contains("true"), true);
                filters?.RemoveAll(x => x.Item1.ToLower() == ZnodeConstant.IsActive.ToLower());
            }
            if (isActive)
                filters?.Add(new FilterTuple(ZnodeConstant.IsActive.ToLower(), FilterOperators.Equals, $"'{isActive}'".ToLower()));
        }

        //Publish catalog with associated product which has draft status.
        public virtual PublishedModel Publish(int pimCatalogId, string revisionType)
        {
            return Publish(pimCatalogId, revisionType, isDraftProductsOnly: true);
        }


        //Publish a complete catalog according to pimCatalogId, revisionType and isDraftProductsOnly flag
        public virtual PublishedModel Publish(int pimCatalogId, string revisionType,bool isDraftProductsOnly)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { pimCatalogId = pimCatalogId, revisionType = revisionType, isDraftProductsOnly = isDraftProductsOnly });

            PublishCatalogDataService publishCatalogDataService = (PublishCatalogDataService)GetService<IPublishCatalogDataService>();

            bool isPublished = false;
            int publishCatalogId = 0;
            string jobId = Convert.ToString(Guid.NewGuid());

            //Check whether any other catalog is in publish state or not. #Step 1
            if (publishCatalogDataService.IsCatalogPublishInProgress() || publishCatalogDataService.IsImportProcessInProgress()|| publishCatalogDataService.IsExportPublishInProgress())
                throw new ZnodeException(ErrorCodes.NotPermitted, PIM_Resources.ErrorPublishCatalog);

            try
            {
                //Create a thread to start publish process of catalog. #Step 2
                HttpContext httpContext = HttpContext.Current;
                Action threadWorker = delegate ()
                {
                    HttpContext.Current = httpContext;
                    try
                    {
                        //Call master sp to perform all the catalog publish with draft/all associated product as per isDraftProductsOnly flag is passed  operation . #Step 3
                        bool status = publishCatalogDataService.ProcessCatalogPublish(pimCatalogId, revisionType, jobId, isDraftProductsOnly, out publishCatalogId);

                        //Only perform if status is true, false means master sp task failed
                        if (status)
                        {

                            FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeCatalogIndexEnum.PublishCatalogId.ToString(), FilterOperators.Equals, publishCatalogId.ToString()) };
                            PortalIndexModel portalIndex = _searchService.GetCatalogIndexData(null, filter);
                            if (IsNotNull(portalIndex) && defaultDataService.IsIndexExists(portalIndex.IndexName))
                                portalIndex.NewIndexName = _searchService.UpdateIndexNameWithTimestamp(portalIndex.IndexName);
                            
                            if(IsNotNull(portalIndex))
                                portalIndex.IsPublishDraftProductsOnly = isDraftProductsOnly;

                            List<string> revisionTypeList = publishCatalogDataService.GetRevisionTypesForElasticIndex(revisionType);

                            bool isPreviewProductionEnabled = revisionTypeList?.Count() > 1 ? true : false;

                            //Perform elastic index creation operation of products. #Step 4
                            foreach (string revisiontype in revisionTypeList)
                                portalIndex =  AssociatedPortalCreateIndex(publishCatalogId, portalIndex, isPreviewProductionEnabled, revisiontype);


                            //Perform deletion of old version data & update necessary flags. #Step 5 
                            publishCatalogDataService.PurgePreviouslyPublishedCatalogDetails(publishCatalogId, jobId);

                            //Update flag value true if all operation of catalog publish execute successfully
                            isPublished = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Revert all inserted data of processing catalog in case of any failure
                        publishCatalogDataService.RevertInProgressCatalogData(publishCatalogId, jobId);

                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                        throw ex;
                    }

                    //Only perform if status is true, false means master sp task failed
                    if (isPublished)
                    {
                        //Call update sp to update associate & linked products data. #Step 6 
                        publishCatalogDataService.UpdatePublishedProductAssociatedData();

                        //Clear Cache of portal after catalog publish. #Step 7
                        ClearCacheAfterPublish(publishCatalogId);

                    }
                };
                AsyncCallback callBack = new AsyncCallback(PublishSuccessCallBack);
                threadWorker.BeginInvoke(callBack, null);

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                //Return model with proper details. #Step 8
                return new PublishedModel { IsPublished = true, ErrorMessage = Admin_Resources.SuccessPublish };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw ex;
            }
        }


        #region Product(s) & Category(s) operation to catalog category  

        //Associate the product(s) to catalog categories
        public virtual bool AssociateProductsToCatalogCategory(CatalogAssociationModel catalogAssociationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(catalogAssociationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            int categoryId = Convert.ToInt32(catalogAssociationModel.CategoryId);
            string[] productIds = !string.IsNullOrEmpty(catalogAssociationModel.ProductIds) ? catalogAssociationModel.ProductIds.Split(',') : null;

            List<ZnodePimCategoryProduct> pimCategoryProduct = new List<ZnodePimCategoryProduct>();

            if (productIds?.Count() > 0)
            {
                foreach (string productId in productIds)
                    pimCategoryProduct.Add(new ZnodePimCategoryProduct { PimCategoryId = categoryId, PimProductId = Convert.ToInt32(productId), DisplayOrder = ZnodeConstant.DisplayOrder, Status = true });
            }

            ZnodeLogging.LogMessage("Product Ids:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info, productIds);

            IEnumerable<ZnodePimCategoryProduct> pimCategoryAssociatedProducts = _pimCategoryProductRepository.Insert(pimCategoryProduct);

            return pimCategoryAssociatedProducts?.FirstOrDefault()?.PimCategoryProductId > 0 ? true : false;
        }

        //Unassociate categories and product from catalog.
        public virtual bool UnAssociateProductsFromCatalogCategory(CatalogAssociationModel catalogAssociationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (Equals(catalogAssociationModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            int categoryId = Convert.ToInt32(catalogAssociationModel.CategoryId);
            string[] productIds = !string.IsNullOrEmpty(catalogAssociationModel.ProductIds) ? catalogAssociationModel.ProductIds.Split(',') : null;

            if (productIds?.Count() < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorPimCategoryProductIdLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimCategoryProductEnum.PimProductId.ToString(), ProcedureFilterOperators.In, string.Join(",", productIds)));
            filters.Add(new FilterTuple(ZnodePimCategoryProductEnum.PimCategoryId.ToString(), ProcedureFilterOperators.In, categoryId.ToString()));

            return _pimCategoryProductRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        //Associate the category(s) to catalog
        public virtual bool AssociateCategoryToCatalog(CatalogAssociationModel catalogAssociationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(catalogAssociationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogAssociationModel = catalogAssociationModel });

            int? pimCategoryHierarchyId = catalogAssociationModel.PimCategoryHierarchyId;
            int catalogId = catalogAssociationModel.CatalogId;
            string[] categoryIds = !string.IsNullOrEmpty(catalogAssociationModel.CategoryIds) ? catalogAssociationModel.CategoryIds.Split(',') : null;
            List<ZnodePimCategoryHierarchy> pimCategoryHierarchy = new List<ZnodePimCategoryHierarchy>();

            if (categoryIds?.Length > 0)
            {
                int maxDisplayOrder = GetMaxDisplayOrder(catalogId, pimCategoryHierarchyId);
                foreach (string categoryId in categoryIds)
                {
                    maxDisplayOrder += 75000;
                    pimCategoryHierarchy.Add(new ZnodePimCategoryHierarchy { PimCatalogId = catalogId, ParentPimCategoryHierarchyId = pimCategoryHierarchyId > 0 ? pimCategoryHierarchyId : null, PimCategoryId = Convert.ToInt32(categoryId), IsActive = true, DisplayOrder = maxDisplayOrder });
                }

                pimCategoryHierarchy = _pimCategoryHierarchyRepository.Insert(pimCategoryHierarchy)?.ToList();
                ZnodeLogging.LogMessage("CategoryHierarchy list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { categoryHierarchyListCount = pimCategoryHierarchy?.Count });
            }

            return pimCategoryHierarchy?.Count > 0 ? true : false;
        }

        //UnAssociate the category(s) from catalog.
        public virtual bool UnAssociateCategoryFromCatalogCategory(CatalogAssociationModel catalogAssociationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(catalogAssociationModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogAssociationModel = catalogAssociationModel });

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

            objStoredProc.SetParameter(ZnodePimCatalogEnum.PimCatalogId.ToString(), catalogAssociationModel.CatalogId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), string.Join(",", catalogAssociationModel.PimCategoryHierarchyIds), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimCategoryHierarchy @PimCategoryHierarchyId, @PimCatalogId, @Status OUT", 2, out status);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessUnassociateCategories, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorUnassociateCategories, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        #endregion Product(s) & Category(s) operation to catalog category


        public virtual bool IsCodeExists(HelperParameterModel parameterModel)
        {
            if (!string.IsNullOrEmpty(parameterModel.CodeField))
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return _pimCatalogRepository.Table.Any(a => a.CatalogCode == parameterModel.CodeField);
            }
            return true;
        }

        //Get catalog details by catalog code.
        public virtual CatalogModel GetCatalogByCatalogCode(string catalogcode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(catalogcode))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorCatalogCodeRequired);

            int? catalogId = _pimCatalogRepository?.Table?.FirstOrDefault(x => x.CatalogCode.ToLower().Equals(catalogcode.ToLower()))?.PimCatalogId;

            if (IsNull(catalogId) || catalogId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.CatalogCodeIsInValidMessages);

            return GetCatalog(catalogId.GetValueOrDefault());
        }

        //Delete catalog by code.
        public virtual bool DeleteCatalogByCode(CatalogDeleteModel catalogCodes)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(catalogCodes.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.VoucherCodeCanNotBeEmpty);

            CatalogDeleteModel catalogId = new CatalogDeleteModel();
            catalogId.IsDeletePublishCatalog = catalogCodes.IsDeletePublishCatalog;
            var catalogIds = _pimCatalogRepository?.Table?.Where(x => catalogCodes.Ids.ToLower().Contains(x.CatalogCode.ToLower()))?.Select(x => x.PimCatalogId.ToString())?.ToList();

            if (catalogIds?.Count > 0)
                catalogId.Ids = String.Join(",", catalogIds);

            if (IsNull(catalogId.Ids) || catalogId.Ids?.Count() < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Api_Resources.CatalogCodeIsInValidMessages);

            return DeleteCatalog(catalogId);
        }
        #endregion Public Methods

        #region Private Methods

        //Update display order
        private bool UpdateDisplayOrder(CatalogAssociateCategoryModel catalogAssociateCategoryModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodePimCategoryHierarchy categoryHierarchy = _pimCategoryHierarchyRepository.Table.FirstOrDefault(x => x.PimCatalogId == catalogAssociateCategoryModel.PimCatalogId && x.PimCategoryHierarchyId == catalogAssociateCategoryModel.PimCategoryHierarchyId);
            if (IsNotNull(categoryHierarchy))
            {
                categoryHierarchy.DisplayOrder = catalogAssociateCategoryModel.DisplayOrder;
                ZnodeLogging.LogMessage("CatalogAssociateCategoryModel with: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { DisplayOrder = catalogAssociateCategoryModel?.DisplayOrder, PimCatalogId = catalogAssociateCategoryModel?.PimCatalogId, PimCategoryHierarchyId = catalogAssociateCategoryModel?.PimCategoryHierarchyId });
                bool isUpdated = _pimCategoryHierarchyRepository.Update(categoryHierarchy);

                ZnodeLogging.LogMessage(isUpdated ? string.Format(PIM_Resources.SuccessUpdateAssociateCategoryDetails, catalogAssociateCategoryModel.PimCategoryId) : PIM_Resources.ErrorUpdateAssociateCategoryDetails, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return isUpdated;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return false;
        }


        //Return List of categories on basis of where clause
        private CatalogAssociateCategoryListModel GetXmlCategory(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            //Get locale Id.
            int localeId = GetLocaleId(filters);
            int pimCatalogId, pimCategoryId, pimCategoryHierarchyId;
            string isAssociated;
            //Set filters for associated categories for profile.
            SetFiltersForAssociatedCategoriesForProfile(filters, out isAssociated, out pimCatalogId, out pimCategoryId, out pimCategoryHierarchyId);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string whereClause = ProductService.GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            ZnodeLogging.LogMessage("Whereclause:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause);
            objStoredProc.GetParameter("@WhereClause", whereClause, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, SqlDbType.Int);
            objStoredProc.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter(ZnodeProfileCatalogEnum.PimCatalogId.ToString(), pimCatalogId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@IsAssociated", isAssociated, ParameterDirection.Input, SqlDbType.NVarChar);          
            objStoredProc.GetParameter(ZnodePimCategoryEnum.PimCategoryId.ToString(), pimCategoryId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), pimCategoryHierarchyId, ParameterDirection.Input, SqlDbType.Int);

            ZnodeLogging.LogMessage("Znode_GetPimCatalogAssociatedCategory SP parameters: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new
            {
                whereClause = whereClause,
                pageListModel = pageListModel?.ToDebugString(),
                localeId = localeId,
                pimCatalogId = pimCatalogId,
                isAssociated = isAssociated,
                pimCategoryId = pimCategoryId,
                pimCategoryHierarchyId = pimCategoryHierarchyId
            });
            var ds = objStoredProc.GetSPResultInDataSet("Znode_GetPimCatalogAssociatedCategory");

            CatalogAssociateCategoryListModel categoryListModel = Xmldataset(ds, out pageListModel.TotalRowCount);
            categoryListModel.BindPageListModel(pageListModel);

            return categoryListModel;
        }

        //Convert dataset to dynamic list
        private CatalogAssociateCategoryListModel Xmldataset(DataSet ds, out int recordCount)
        {
            // out pageListModel.TotalRowCount
            recordCount = 0;
            if (IsNotNull(ds) && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var xml = Convert.ToString(ds.Tables[0]?.Rows[0]["CategoryXML"]);

                if (!string.IsNullOrEmpty(xml))
                {
                    var columns = ds.Tables[1];

                    var _columnlist = GetDictionary(columns);

                    if (!string.IsNullOrEmpty(ds.Tables[2].Rows[0]["RowsCount"].ToString()))
                        recordCount = Convert.ToInt32(ds.Tables[2].Rows[0]["RowsCount"].ToString());

                    ZnodeLogging.LogMessage("recordCount: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { recordCount = recordCount });
                    XDocument xmlDoc = XDocument.Parse(xml);

                    dynamic root = new ExpandoObject();

                    XmlToDynamic.Parse(root, xmlDoc.Elements().First());

                    var _list = new List<dynamic>();
                    if (!(root.MainCategory.Category is List<dynamic>))
                        _list.Add(root.MainCategory.Category);
                    else
                        _list = (List<dynamic>)root.MainCategory.Category;

                    return new CatalogAssociateCategoryListModel { AttributeColumnName = _columnlist, XmlDataList = _list };
                }
            }
            return new CatalogAssociateCategoryListModel();
        }

        //Get dictionary of datatable
        private Dictionary<string, object> GetDictionary(DataTable dt)
        {
            return dt.AsEnumerable()
              .ToDictionary<DataRow, string, object>(row => row.Field<string>(0),
                                        row => row.Field<object>(1));
        }

        //Get the tree with its child.
        protected virtual List<ContentPageTreeModel> GetAllNode(List<ContentPageTreeModel> childTreeNodes)
        {
            if (IsNotNull(childTreeNodes))
            {
                foreach (ContentPageTreeModel node in childTreeNodes)
                {
                    //find all child folder and add to list
                    List<ContentPageTreeModel> child = childTreeNodes.Where(x => x.ParentId == node.Id && x.Id != node.Id)?.ToList();
                    node.Children = IsNotNull(node.Children) ? node.Children : new List<ContentPageTreeModel>();
                    node.Children.AddRange(GetAllNode(child));
                }
                ZnodeLogging.LogMessage("ChildTreeNodes count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { ChildTreeNodesCount = childTreeNodes?.Count });
                return childTreeNodes;
            }
            return new List<ContentPageTreeModel>();
        }

        //Get the maximum display order of category.
        private int GetMaxDisplayOrder(int catalogId, int? parentCategory)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogId = catalogId, parentCategory = parentCategory });
            parentCategory = parentCategory > 0 ? parentCategory : null;
            return _pimCategoryHierarchyRepository.Table.Where(x => x.PimCatalogId == catalogId && x.ParentPimCategoryHierarchyId == parentCategory)?.Max(x => x.DisplayOrder) ?? 0;
        }

        //Set filters for associated categories for profile.
        private void SetFiltersForAssociatedCategoriesForProfile(FilterCollection filters, out string isAssociated, out int pimCatalogId, out int pimCategoryId, out int pimCategoryHierarchyId)
        {
            isAssociated = (filters.Where(filterTuple => filterTuple.Item1 == FilterKeys.IsAssociated.ToLower().ToString())?.FirstOrDefault()?.Item3);
            pimCatalogId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodeProfileCatalogEnum.PimCatalogId.ToString().ToLower())?.FirstOrDefault()?.Item3);
            pimCategoryId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodePimCategoryEnum.PimCategoryId.ToString().ToLower())?.FirstOrDefault()?.Item3);
            pimCategoryHierarchyId = Convert.ToInt32(filters.FirstOrDefault(filterTuple => filterTuple.Item1 == ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString().ToLower())?.Item3);

            ZnodeLogging.LogMessage("Out parameters: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { isAssociated = isAssociated, pimCatalogId = pimCatalogId, pimCategoryId = pimCategoryId, pimCategoryHierarchyId = pimCategoryHierarchyId });
            filters.RemoveAll(x => x.Item1.Equals(FilterKeys.IsAssociated.ToLower(), StringComparison.InvariantCultureIgnoreCase));
            filters.Remove(filters.Where(filterTuple => filterTuple.Item1 == ZnodeProfileCatalogEnum.PimCatalogId.ToString().ToLower())?.FirstOrDefault());
            filters.Remove(filters.Where(filterTuple => filterTuple.Item1 == ZnodePimCategoryEnum.PimCategoryId.ToString().ToLower())?.FirstOrDefault());
            filters.Remove(filters.FirstOrDefault(filterTuple => filterTuple.Item1 == ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString().ToLower()));
        }

        //unassociate category hierarchy from catalog.
        [Obsolete("This method is not in use now, as new method has been introduced to unassociate category")]
        private bool UnAssociateCategories(CatalogAssociationModel catalogAssociationModel)
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePimCatalogEnum.PimCatalogId.ToString(), catalogAssociationModel.CatalogId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), string.Join(",", catalogAssociationModel.PimCategoryHierarchyIds), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

            ZnodeLogging.LogMessage("CatalogId and PimCategoryHierarchyIds to unassociate categories: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { CatalogId = catalogAssociationModel?.CatalogId, PimCategoryHierarchyIds = catalogAssociationModel?.PimCategoryHierarchyIds });
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimCategoryHierarchy @PimCategoryHierarchyId, @PimCatalogId, @Status OUT", 2, out status);
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessUnassociateCategories, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorUnassociateCategories, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Set filters to get unassociated categories of catalog.
        protected virtual FilterCollection SetFilterToUnAssociateCategoryProducts(CatalogAssociationModel catalogAssociationModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimCategoryHierarchyEnum.PimCatalogId.ToString(), FilterOperators.Equals, catalogAssociationModel.CatalogId.ToString()));

            if (!string.IsNullOrEmpty(catalogAssociationModel.CategoryIds))
                filters.Add(new FilterTuple(ZnodePimCategoryEnum.PimCategoryId.ToString(), FilterOperators.In, catalogAssociationModel.CategoryIds));
            else if (!string.IsNullOrEmpty(catalogAssociationModel.ProductIds))
            {
                filters.Add(new FilterTuple(ZnodePimCategoryEnum.PimCategoryId.ToString(), FilterOperators.Equals, Convert.ToString(catalogAssociationModel.CategoryId)));
                filters.Add(new FilterTuple(ZnodePimProductEnum.PimProductId.ToString(), FilterOperators.In, catalogAssociationModel.ProductIds));
            }
            return filters;
        }

        //Set filters to get unassociated categories of catalog.
        private FilterCollection SetFilterToUnAssociateCategoryProductsFromProfileCatalog(CatalogAssociationModel catalogAssociationModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeProfileCatalogCategoryEnum.ProfileCatalogCategoryId.ToString(), FilterOperators.In, catalogAssociationModel.ProfileCatalogCategoryIds.ToString()));
            return filters;
        }

        //Set filters to get display order of category.
        private FilterCollection SetFiltersToSetDisplayOrder(CatalogAssociateCategoryModel catalogAssociateCategoryModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimCategoryHierarchyEnum.PimCatalogId.ToString(), FilterOperators.Equals, catalogAssociateCategoryModel.PimCatalogId.ToString()));

            if (catalogAssociateCategoryModel.CategoryId > 0)
                filters.Add(new FilterTuple(ZnodePimCategoryHierarchyEnum.PimCategoryId.ToString(), FilterOperators.In, $"{catalogAssociateCategoryModel.PimCategoryId},{catalogAssociateCategoryModel.CategoryId}"));
            else
                filters.Add(new FilterTuple(ZnodePimCategoryHierarchyEnum.PimCategoryId.ToString(), FilterOperators.Equals, catalogAssociateCategoryModel.PimCategoryId.ToString()));

            ZnodeLogging.LogMessage("FiltersToSetDisplayOrder ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { FiltersToSetDisplayOrder = filters });
            return filters;
        }

        //Remove the categories from Catalog.
        [Obsolete("This method is not in use now, as removed both profile table so this sp has been removed")]
        private bool UnAssociateCategoryFromProfile(CatalogAssociationModel catalogAssociationModel)
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeProfileCatalogEnum.ProfileCatalogId.ToString(), catalogAssociationModel.ProfileCatalogId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePimCatalogEnum.PimCatalogId.ToString(), catalogAssociationModel.CatalogId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), string.Join(",", catalogAssociationModel.PimCategoryHierarchyIds), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("SP parameters to unassociate category from profile: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { ProfileCatalogId = catalogAssociationModel?.ProfileCatalogId, CatalogId = catalogAssociationModel?.CatalogId, PimCategoryHierarchyIds = catalogAssociationModel?.PimCategoryHierarchyIds });
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteProfileCatalogCategory @ProfileCatalogId, @PimCatalogId,@PimCategoryHierarchyId, @Status OUT", 3, out status);
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessUnassociateCategoriesFromProfile, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorUnassociateCategoriesFromProfile, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        private Dictionary<int, int> GetCategoryHierarchyIdDictionary(string[] categoryIds, List<ZnodePimCategoryHierarchy> categoryHierarchy)
        {
            Dictionary<int, int> categoryHierarchyIds = new Dictionary<int, int>();
            for (int index = 0; index < categoryIds.Length; index++)
                categoryHierarchyIds.Add(Convert.ToInt32(categoryIds[index]), categoryHierarchy[index].PimCategoryHierarchyId);

            return categoryHierarchyIds;
        }

        protected virtual bool IsAllowIndexing(int pimCatalogId)
        {
            if (pimCatalogId > 0)
                return _pimCatalogRepository.Table.FirstOrDefault(n => n.PimCatalogId == pimCatalogId)?.IsAllowIndexing ?? false;

            return false;
        }

        //Clear Cache of portal after catalog publish.
        protected virtual void ClearCacheAfterPublish(int publishCatalogId)
        {
            if (publishCatalogId == 0) return;

            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { publishCatalogId = publishCatalogId });
            List<ZnodePortalCatalog> associatedPortalId = _portalCatalogRepository.Table.Where(portalCatalog => portalCatalog.PublishCatalogId == publishCatalogId)?.ToList();
            List<int> ids = associatedPortalId.Select(x => x.PortalId).ToList();
            ClearCacheHelper.EnqueueEviction(new PortalPublishEvent()
            {
                Comment = "From catalog service publish operation.",
                PortalIds = ids.ToArray()
            });
            //Clear all cloudflare cache
            var znodeEventNotifierCloudflare = new ZnodeEventNotifier<CloudflarePurgeModel>(new CloudflarePurgeModel() { PortalId = ids });

        }

        //Associate categories to profile.
        private bool AssociateCategoriesToCatalogForProfile(CatalogAssociationModel catalogAssociationModel)
        {
            IZnodeViewRepository<CatalogAssociationModel> objStoredProc = new ZnodeViewRepository<CatalogAssociationModel>();
            objStoredProc.SetParameter(ZnodeProfileCatalogEnum.ProfileCatalogId.ToString(), catalogAssociationModel.ProfileCatalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeProfileCatalogEnum.PimCatalogId.ToString(), catalogAssociationModel.CatalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("PimCategoryId", catalogAssociationModel.CategoryIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("PimCategoryHierarchyId", string.Join(",", catalogAssociationModel.PimCategoryHierarchyId), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;

            ZnodeLogging.LogMessage("SP parameters: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { ProfileCatalogId = catalogAssociationModel?.ProfileCatalogId, CatalogId = catalogAssociationModel?.CatalogId, CategoryIds = catalogAssociationModel.CategoryIds, PimCategoryHierarchyId = catalogAssociationModel?.PimCategoryHierarchyId });
            objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateProfileCatalog null,@PimCatalogId,@UserId, @Status OUT,@PimCategoryId,@ProfileCatalogId,null,@PimCategoryHierarchyId", 5, out status);
            if (status == 1)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessAssociateCategoriesToCatalog, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorAssociateCategoriesToCatalog, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        private bool IsCategorySame(int pimHierarchyId, int pimParentHierarchyId)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { pimHierarchyId = pimHierarchyId, pimParentHierarchyId = pimParentHierarchyId });
            return (from categoryHierarchy in _pimCategoryHierarchyRepository.Table
                    join parentCategoryHierarchy in _pimCategoryHierarchyRepository.Table on categoryHierarchy.PimCategoryId equals parentCategoryHierarchy.PimCategoryId
                    where categoryHierarchy.PimCategoryHierarchyId == pimHierarchyId && parentCategoryHierarchy.PimCategoryHierarchyId == pimParentHierarchyId
                    select categoryHierarchy)?.Count() > 0;
        }

        //Converts Searchable Attributes List to Data Table
        private static DataTable ConvertKeywordListToDataTable(List<int> localeIds)
        {
            ZnodeLogging.LogMessage("LocaleIds: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, localeIds);
            DataTable table = new DataTable("TransferId");
            table.Columns.Add("Id");

            foreach (int model in localeIds)
                table.Rows.Add(model);

            return table;
        }
        #endregion Private Methods
    }
}