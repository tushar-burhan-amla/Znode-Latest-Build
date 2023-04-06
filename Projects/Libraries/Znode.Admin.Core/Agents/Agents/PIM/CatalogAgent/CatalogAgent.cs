using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Enum;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents

{
    public class CatalogAgent : BaseAgent, ICatalogAgent
    {
        #region Private Variables

        private readonly ICatalogClient _catalogClient;
        private readonly ICategoryClient _categoryClient;
        private readonly IProfileClient _profilesClient;

        #endregion Private Variables

        #region Constructor

        public CatalogAgent(ICatalogClient catalogClient, ICategoryClient categoryClient, IProfileClient profileClient)
        {
            _catalogClient = GetClient<ICatalogClient>(catalogClient);
            _categoryClient = GetClient<ICategoryClient>(categoryClient);
            _profilesClient = GetClient<IProfileClient>(profileClient);
        }

        #endregion Constructor

        #region Public Methods

        public virtual CatalogListViewModel GetCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetCatalogList(expands, filters, sorts, null, null);

        public virtual CatalogListViewModel GetCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });

            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.PimCatalogId, DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            CatalogListModel catalogList = _catalogClient.GetCatalogList(expands, filters, sorts, pageIndex, pageSize);

            CatalogListViewModel listViewModel = new CatalogListViewModel { Catalogs = catalogList?.Catalogs?.ToViewModel<CatalogViewModel>().ToList() };

            listViewModel.Catalogs?.ForEach(item =>
            {
                item.IsActiveList = GetBooleanList();
                item.UrlEncodedCatalogName = HttpUtility.HtmlEncode(item.CatalogName);
                item.ConnectorTouchPoints = "PublishCatalog_" + item.PimCatalogId + "_" + HttpUtility.UrlEncode(item.CatalogName);
                item.SchedulerCallFor = ZnodeConstant.PublishCatalog;
            });

            SetListPagingData(listViewModel, catalogList);

            //Set tool menu for catalog list grid view.
            SetCatalogListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return catalogList?.Catalogs?.Count > 0 ? listViewModel : new CatalogListViewModel();
        }

        public virtual CatalogViewModel CreateCatalog(CatalogViewModel catalogModel)
            => _catalogClient.CreateCatalog(catalogModel.ToModel<CatalogModel>())?.ToViewModel<CatalogViewModel>();

        public virtual CatalogViewModel GetCatalog(int pimCatalogId)
            => pimCatalogId > 0 ? _catalogClient.GetCatalog(pimCatalogId)?.ToViewModel<CatalogViewModel>() : new CatalogViewModel();

        public virtual CatalogViewModel UpdateCatalog(CatalogViewModel viewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                CatalogModel model = _catalogClient.UpdateCatalog(viewModel.ToModel<CatalogModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return !Equals(model, null) ? model.ToViewModel<CatalogViewModel>() : new CatalogViewModel() { HasError = true, ErrorMessage = PIM_Resources.UpdateErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                viewModel.HasError = true;
                viewModel.ErrorMessage = PIM_Resources.UpdateErrorMessage;
                return viewModel;
            }
        }

        public virtual bool CopyCatalog(CatalogViewModel catalogModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return _catalogClient.CopyCatalog(catalogModel.ToModel<CatalogModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                catalogModel.ErrorMessage = PIM_Resources.AlreadyExistCatalogName;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual bool DeleteCatalog(string pimCatalogIds, bool isDeletePublishCatalog)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                ZnodeLogging.LogMessage("Delete Catlog with", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info, new { PimCatalogId = pimCatalogIds });

                return _catalogClient.DeleteCatalog(new CatalogDeleteModel { Ids = pimCatalogIds, IsDeletePublishCatalog = isDeletePublishCatalog });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual bool PublishCatalog(int pimCatalogId, string revisionType, out string errorMessage, string publishContent = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            errorMessage = PIM_Resources.ErrorPublished;
            try
            {
                ZnodeLogging.LogMessage("Publish Catlog with", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info, new { PimCatalogId = pimCatalogId });
                //Set isDraft to true if want to publish draft status product only else set false to publish all products
                bool isDraftProductsOnly = !string.IsNullOrEmpty(publishContent) && publishContent.Contains(ZnodePublishStatesEnum.DRAFT.ToString());
                return Convert.ToBoolean(_catalogClient.PublishCatalog(pimCatalogId, revisionType, isDraftProductsOnly)?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotPermitted:
                        errorMessage = PIM_Resources.ErrorPublishCatalog;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return false;
        }

        public virtual bool PreviewCatalog(int pimCatalogId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            errorMessage = PIM_Resources.ErrorPublished;
            try
            {
                return Convert.ToBoolean(_catalogClient.PublishCatalog(pimCatalogId, ZnodePublishStatesEnum.PREVIEW.ToString())?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotPermitted:
                        errorMessage = PIM_Resources.ErrorPublishCatalog;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return false;
        }

        //Publish catalog category associated products.
        public virtual bool PublishCategoryProducts(int pimCatalogId, int pimCategoryHierarchyId, string revisionType, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            errorMessage = PIM_Resources.ErrorPublished;
            try
            {
                return Convert.ToBoolean(_catalogClient.PublishCategoryProducts(pimCatalogId, pimCategoryHierarchyId, revisionType)?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = PIM_Resources.ErrorPublishCatalogProducts;
                        break;
                    case ErrorCodes.NotPermitted:
                        errorMessage = PIM_Resources.ErrorPublishCatalogInProcess;
                        break;
                    case ErrorCodes.CategoryPublishError:
                        errorMessage = PIM_Resources.ErrorPublishCatalogCategory;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return false;
        }

        #region Category Catalog Association

        //Associate Categories and Products to catalog.
        [Obsolete("This method is not in use now, as new method has been introduced to associate category")]
        public virtual bool AssociateCategory(CatalogAssociationViewModel catalogAssociationViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNotNull(catalogAssociationViewModel))
            {
                try
                {
                    ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    catalogAssociationViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                    //Associate the category.
                    return _catalogClient.AssociateCategory(catalogAssociationViewModel.ToModel<CatalogAssociationModel>());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        //Gets the associated products belongs to catalog and category.
        public virtual CatalogAssociationViewModel GetAssociatedProducts(int catalogId, int categoryHierarchyId, int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogId = catalogId, filters = filters, sorts = sorts });

            if (catalogId > 0)
            {
                CatalogModel catalog = _catalogClient.GetCatalog(catalogId);
                if (IsNotNull(catalog))
                {
                    SetStoreFilter(portalId, ref filters);
                    ProductDetailsListModel catalogList = categoryHierarchyId > 0
                        ? _catalogClient.GetCategoryAssociatedProducts(new CatalogAssociationModel() { CatalogId = catalogId, PimCategoryHierarchyId = categoryHierarchyId, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale) }, filters, sorts, pageIndex, pageSize)
                        : new ProductDetailsListModel();
                    CatalogAssociationViewModel listViewModel = new CatalogAssociationViewModel { PimCategoryHierarchyId = categoryHierarchyId, CatalogId = catalogId, AssociatedProducts = catalogList?.ProductDetailList?.ToViewModel<ProductDetailsViewModel>().ToList() };

                    listViewModel.CatalogName = catalog.CatalogName;

                    SetListPagingData(listViewModel, catalogList);

                    SetAssociatedProductsToolMenu(listViewModel);

                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                    return listViewModel?.AssociatedProducts?.Count > 0 ? listViewModel : new CatalogAssociationViewModel() { CatalogName = catalog.CatalogName, PimCategoryHierarchyId = categoryHierarchyId, CatalogId = catalogId, AssociatedProducts = new List<ProductDetailsViewModel>() };
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return new CatalogAssociationViewModel() { HasError = true, AssociatedProducts = new List<ProductDetailsViewModel>() };
        }

        //Get the list of unassociated category list.
        public virtual CatalogAssociateCategoryListViewModel GetUnAssociatedCategoryList(int catalogId, int categoryId, int categoryHierarchyId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            SetLocaleFilterIfNotPresent(ref filters);

            //Set filter for catelogId and isAssociated.
            GetFiltersForCategoryList(catalogId, categoryId, categoryHierarchyId, "True", filters);

            CatalogAssociateCategoryListModel categoryList = _catalogClient.GetAssociatedCategoryList(filters, sorts, pageIndex, pageSize);
            CatalogAssociateCategoryListViewModel listViewModel = new CatalogAssociateCategoryListViewModel
            {
                CatalogAssociateCategories = categoryList?.catalogAssociatedCategoryList?.ToViewModel<CatalogAssociateCategoryViewModel>().ToList(),
                XmlDataList = categoryList.XmlDataList,
                AttributeColumnName = categoryList.AttributeColumnName
            };

            listViewModel.PimCatalogId = catalogId;
            listViewModel.PimCategoryId = categoryId;
            listViewModel.PimCategoryHierarchyId = categoryHierarchyId;
            SetListPagingData(listViewModel, categoryList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return categoryList?.XmlDataList?.Count > 0 ? listViewModel : new CatalogAssociateCategoryListViewModel() { PimCatalogId = catalogId, PimCategoryId = categoryId, PimCategoryHierarchyId = categoryHierarchyId, CatalogAssociateCategories = new List<CatalogAssociateCategoryViewModel>(), XmlDataList = new List<dynamic>() };
        }

        //Get the list of unassociated products list.
        public virtual ProductDetailsListViewModel GetUnAssociatedProductsList(int catalogId, int categoryId, int categoryHierarchyId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ProductDetailsListModel productList = _catalogClient.GetCategoryAssociatedProducts(new CatalogAssociationModel() { CatalogId = catalogId, CategoryId = categoryId, PimCategoryHierarchyId = categoryHierarchyId, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale), IsAssociated = true }, filters, sorts, pageIndex, pageSize);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            ProductDetailsListViewModel listViewModel = new ProductDetailsListViewModel
            {
                ProductDetailList = productList?.ProductDetailList?.ToViewModel<ProductDetailsViewModel>().ToList()
            };
            SetListPagingData(listViewModel, productList);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return productList?.ProductDetailList?.Count > 0 ? listViewModel : new ProductDetailsListViewModel()
            {
                ProductDetailList = new List<ProductDetailsViewModel>()
            };
        }

        //UnAssociate Categories and Products to catalog.
        [Obsolete("This method is not in use now, as new method has been introduced to unassociate category")]
        public virtual bool UnAssociateCategory(CatalogAssociationViewModel catalogAssociationViewModel)
             => _catalogClient.UnAssociateCategory(catalogAssociationViewModel.ToModel<CatalogAssociationModel>());

        //UnAssociate products to catalog.
        [Obsolete("This method is not in use now, As product association/unAssociation has been removed from catalog category")]
        public virtual bool UnAssociateProduct(CatalogAssociationViewModel catalogAssociationViewModel)
             => _catalogClient.UnAssociateProduct(catalogAssociationViewModel.ToModel<CatalogAssociationModel>());

        //get tree structure for categories.
        public virtual string GetTree(int catalogId)
        => $"[{JsonConvert.SerializeObject(GetTreeViewModel(_catalogClient.GetCategoryTree(new CatalogAssociationModel { CatalogId = catalogId, CategoryId = -1, LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale) })))}]";

        //Move folder.
        public virtual bool MoveCategory(int folderId, int addtoFolderId, int catalogId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                return _catalogClient.MoveCategory(new CatalogAssociateCategoryModel() { PimCategoryHierarchyId = folderId, ParentPimCategoryHierarchyId = addtoFolderId, PimCatalogId = catalogId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        errorMessage = PIM_Resources.ErrorAlreadyExistCategory;
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual bool MoveCategory(int pimCatalogId, int pimCategoryHierarchyId, int displayOrder, bool isDown)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return !Equals(_catalogClient.UpdateAssociateCategoryDetails(new CatalogAssociateCategoryModel() { PimCatalogId = pimCatalogId, PimCategoryHierarchyId = pimCategoryHierarchyId, DisplayOrder = displayOrder, UpdateDisplayOrder = true }), null);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return false;
            }
        }
        protected virtual FilterCollection SetStoreFilter(int portalId, ref FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(filters))
                filters = new FilterCollection();
            if (filters.Exists(x => x.FilterName.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower()))
                filters.RemoveAll(x => x.FilterName.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower());
            if (portalId > 0)
                filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters });
            return filters;
        }
        #endregion Category Catalog Association

        public virtual CatalogAssociateCategoryViewModel GetAssociatedCategoryDetails(CatalogAssociateCategoryViewModel catalogAssociateCategoryViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNotNull(catalogAssociateCategoryViewModel))
                catalogAssociateCategoryViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return _catalogClient.GetAssociateCategoryDetails(catalogAssociateCategoryViewModel?.ToModel<CatalogAssociateCategoryModel>())?.ToViewModel<CatalogAssociateCategoryViewModel>();
        }

        public virtual CatalogAssociateCategoryViewModel UpdateAssociatedCategoryDetails(CatalogAssociateCategoryViewModel catalogAssociateCategoryViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return _catalogClient.UpdateAssociateCategoryDetails(catalogAssociateCategoryViewModel?.ToModel<CatalogAssociateCategoryModel>())?.ToViewModel<CatalogAssociateCategoryViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return (CatalogAssociateCategoryViewModel)GetViewModelWithErrorMessage(catalogAssociateCategoryViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Check whether the catalog name already exists.
        public virtual bool CheckCatalogNameExist(string catalogName, int pimCatalogId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogName = catalogName, pimCatalogId = pimCatalogId });

            if (!string.IsNullOrEmpty(catalogName))
            {
                catalogName = catalogName.Trim();
                if (catalogName.IndexOf("'") >= 0)
                    catalogName = catalogName.Replace("'", "''");

                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePimCatalogEnum.CatalogName.ToString(), FilterOperators.Contains, catalogName));
                ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters });
                //Get the catalog list based on the catalog name filter.
                CatalogListModel catalogList = _catalogClient.GetCatalogList(null, filters, null);

                if (catalogList?.Catalogs?.Count > 0)
                {
                    if (pimCatalogId > 0)
                        //Set the status in case the catalog is open in edit mode.
                        catalogList.Catalogs.RemoveAll(x => x.PimCatalogId == pimCatalogId);
                    ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                    return catalogList.Catalogs.FindIndex(x => x.CatalogName == catalogName) != -1;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return false;
        }

        public virtual PublishCatalogLogListViewModel GetCatalogPublishStatus(int pimCatalogId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { pimCatalogId = pimCatalogId, filters = filters, sorts = sorts });

            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(ZnodePublishCatalogLogEnum.PublishCatalogLogId.ToString(), DynamicGridConstants.DESCKey);
            }
            filters = IsNull(filters) ? new FilterCollection() : filters;

            filters.RemoveAll(x => string.Equals(x.FilterName, ZnodePublishCatalogLogEnum.PimCatalogId.ToString(), StringComparison.OrdinalIgnoreCase));
            filters.Add(ZnodePublishCatalogLogEnum.PimCatalogId.ToString(), FilterOperators.Equals, pimCatalogId.ToString());
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            PublishCatalogLogListModel publishCatalogLogList = _catalogClient.GetCatalogPublishStatus(filters, sorts, pageIndex, pageSize);
            PublishCatalogLogListViewModel publishCatalogLogListViewModel = new PublishCatalogLogListViewModel { PublishCatalogLog = publishCatalogLogList?.PublishCatalogLogList?.ToViewModel<PublishCatalogLogViewModel>().ToList() };

            //Set the start time and end time of publish with its last publish date for each record in the log.
            publishCatalogLogListViewModel.PublishCatalogLog?.ForEach(item =>
            {
                item.LastPublishedDate = item.CreatedDate;

                DateTime createdDate = DateTime.Parse(item.CreatedDate);
                item.CreatedDate = createdDate.ToString("HH:mm:ss tt");

                if (item.PublishStatus == ZnodeConstant.Processing)
                {
                    item.ModifiedDate = String.Empty;
                }
                else
                {
                    DateTime modifiedDate = DateTime.Parse(item.ModifiedDate);
                    item.ModifiedDate = modifiedDate.ToString("HH:mm:ss tt");
                }
            });

            SetListPagingData(publishCatalogLogListViewModel, publishCatalogLogList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return publishCatalogLogList?.PublishCatalogLogList?.Count > 0 ? publishCatalogLogListViewModel : new PublishCatalogLogListViewModel { PublishCatalogLog = new List<PublishCatalogLogViewModel>() };
        }

        //Update catalog associated category products.
        public virtual bool UpdateCatalogCategoryProduct(CatalogAssociationViewModel catalogAssociationViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool status = false;
            try
            {
                status = _catalogClient.UpdateCatalogCategoryProduct(catalogAssociationViewModel.ToModel<CatalogAssociationModel>());
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return status;
        }
     

        //Get Associate Catalog Tree by pimProductId
        public virtual string GetAssociatedCatalogTree(int pimProductId) => $"[{JsonConvert.SerializeObject(_catalogClient.GetCatalogCategoryHierarchy(pimProductId))}]";

        //Get Product Status filter
        public virtual bool GetActiveProductFilter(Dictionary<string, string> queryStringValues, FilterCollection filters)
        {
            bool result = true;
            if (Equals(queryStringValues?.ContainsKey(ZnodeConstant.ViewMode), true) && Equals(filters?.Exists(x => x.Item1 == ZnodeConstant.IsActive), true))
                result = Equals(filters?.FirstOrDefault(x => x.Item1 == ZnodeConstant.IsActive).FilterValue?.ToLower().Contains("true"), true);
            else if (Equals(queryStringValues?.ContainsKey(ZnodeConstant.IsActive), true))
                result = Equals(queryStringValues[ZnodeConstant.IsActive].ToLower().Contains("true"), true);
            else if (Equals(filters?.Exists(x => x.Item1 == ZnodeConstant.IsActive), true))
                result = Equals(filters?.FirstOrDefault(x => x.Item1 == ZnodeConstant.IsActive).FilterValue?.ToLower().Contains("true"), true);
            return result;
        }

        //Set product Status filter
        public virtual void SetActiveProductFilter(FilterCollection filters, bool isActiveProducts)
        {
            if (Equals(filters?.Exists(x => x.Item1 == ZnodeConstant.IsActive), true))
            {
                filters?.RemoveAll(x => x.Item1 == ZnodeConstant.IsActive);
            }
            filters?.Add(new FilterTuple(ZnodeConstant.IsActive, FilterOperators.Equals, isActiveProducts.ToString().ToLower()));
        }


        #region Product(s) operation to catalog category

        //Associate the product(s) to catalog categories
        public virtual bool AssociateProductsToCatalogCategory(CatalogAssociationViewModel catalogAssociationViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNotNull(catalogAssociationViewModel))
            {
                try
                {
                    ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    catalogAssociationViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                    status = _catalogClient.AssociateProductsToCatalogCategory(catalogAssociationViewModel.ToModel<CatalogAssociationModel>());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return status;
        }

        //UnAssociate the product(s) from catalog category
        public virtual bool UnAssociateProductsFromCatalogCategory(CatalogAssociationViewModel catalogAssociationViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNotNull(catalogAssociationViewModel))
            {
                try
                {
                    catalogAssociationViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                    status = _catalogClient.UnAssociateProductsFromCatalogCategory(catalogAssociationViewModel.ToModel<CatalogAssociationModel>());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return status;
        }

        //Associate Category(s) to catalog.
        public virtual bool AssociateCategoryToCatalog(CatalogAssociationViewModel catalogAssociationViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNotNull(catalogAssociationViewModel))
            {
                try
                {
                    catalogAssociationViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                    status = _catalogClient.AssociateCategoryToCatalog(catalogAssociationViewModel.ToModel<CatalogAssociationModel>());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return status;
        }

        //UnAssociate Category(s) from catalog.
        public virtual bool UnAssociateCategoryFromCatalog(CatalogAssociationViewModel catalogAssociationViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNotNull(catalogAssociationViewModel))
            {
                try
                {
                    catalogAssociationViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                    status = _catalogClient.UnAssociateCategoryFromCatalog(catalogAssociationViewModel.ToModel<CatalogAssociationModel>());
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return status;
        }

        #endregion Product(s) operation to catalog category

        #endregion Public Methods

        #region Private Methods

        //To Do
        private ContentPageTreeViewModel GetTreeViewModel(ContentPageTreeModel treeModel)
        {
            if (IsNotNull(treeModel))
            {
                ContentPageTreeViewModel data = new ContentPageTreeViewModel
                {
                    icon = treeModel.Icon,
                    id = treeModel.Id,
                    text = treeModel.Text,
                    children = treeModel.Children.Select(x => GetTreeViewModel(x)).ToList(),
                };
                data.data = new Dictionary<string, string>();
                data.data.Add("displayorder", treeModel.DisplayOrder.ToString());
                data.data.Add("PimCategoryId", treeModel.PimCategoryId.ToString());
                return data;
            }
            else
                return new ContentPageTreeViewModel();
        }

        //Set tool menu for catalog list grid view.
        private void SetCatalogListToolMenu(CatalogListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeleteCatalogPopup')", ControllerName = "Catalog", ActionName = "Delete" });
            }
        }

        //Set tool menu for catalog list grid view.
        private void SetCatalogListToolMenu(CatalogAssociationViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = PIM_Resources.TextRemoveProducts, JSFunctionName = "Catalog.prototype.ConfirmRemoveAssociatedProducts()", ControllerName = "Catalog", ActionName = "Delete" });
            }
        }

        //Gets the filters for category list.
        private FilterCollection GetFiltersForCategoryList(int catalogId, int categoryId, int categoryHierarchyId, string isAssociated, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogId = catalogId, categoryId = categoryId, categoryHierarchyId = categoryHierarchyId });

            //If null set it to new.
            if (IsNull(filters))
                filters = new FilterCollection();

            //Remove IsAssociatedProducts and PimCatalogId filter.
            filters.RemoveAll(x => x.Item1.Equals(FilterKeys.IsAssociated, StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.Item1.Equals(ZnodePimCatalogEnum.PimCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.Item1.Equals(ZnodePimCategoryEnum.PimCategoryId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.Item1.Equals(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), StringComparison.InvariantCultureIgnoreCase));

            //Add IsAssociatedProducts and PimCatalogId with new values.
            filters.Add(new FilterTuple(FilterKeys.IsAssociated.ToLower(), FilterOperators.Equals, isAssociated));
            filters.Add(new FilterTuple(ZnodePimCatalogEnum.PimCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString()));
            filters.Add(new FilterTuple(ZnodePimCategoryEnum.PimCategoryId.ToString(), FilterOperators.Equals, categoryId.ToString()));
            filters.Add(new FilterTuple(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), FilterOperators.Equals, categoryHierarchyId.ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters });

            return filters;
        }

        //Set tool menu for manage catalog list grid view.
        private void SetAssociatedProductsToolMenu(CatalogAssociationViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = PIM_Resources.TextAddNewCategory, JSFunctionName = "Catalog.prototype.CreateNewCategory()", ControllerName = "Category", ActionName = "Create" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = PIM_Resources.TextRemoveCategory, JSFunctionName = "Catalog.prototype.remove()", ControllerName = "Catalog", ActionName = "DeleteAssociateCategory" });
            }
        }

        //Gets the filters for category list.
        private FilterCollection SetFiltersForCategoryListForProfile(int catalogId, int? profileCatalogId, int pimCategoryId, int pimCategoryHierarchyId, FilterCollection filters)
        {
            //If null set it to new.
            if (IsNull(filters))
                filters = new FilterCollection();

            //Remove ProfileCatalogId and PimCatalogId filter.
            filters.RemoveAll(x => x.Item1.Equals(ZnodeProfileCatalogEnum.PimCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.Item1.Equals(ZnodePimCategoryEnum.PimCategoryId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.Item1.Equals(ZnodeProfileCatalogEnum.ProfileCatalogId.ToString(), StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.Item1.Equals(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), StringComparison.InvariantCultureIgnoreCase));

            //Add ProfileCatalogId and PimCatalogId with new values.
            filters.Add(new FilterTuple(ZnodePimCatalogEnum.PimCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString()));
            filters.Add(new FilterTuple(ZnodeProfileCatalogEnum.ProfileCatalogId.ToString(), FilterOperators.Equals, profileCatalogId.ToString()));
            filters.Add(new FilterTuple(ZnodePimCategoryEnum.PimCategoryId.ToString(), FilterOperators.Equals, pimCategoryId.ToString()));
            filters.Add(new FilterTuple(ZnodePimCategoryHierarchyEnum.PimCategoryHierarchyId.ToString(), FilterOperators.Equals, pimCategoryHierarchyId.ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters = filters });
            return filters;
        }

        #endregion Private Methods
    }
}