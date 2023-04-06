using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
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
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public partial class CategoryService : BaseService, ICategoryService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePimCategory> _pimCategory;
        private readonly IZnodeRepository<ZnodePimCategoryProduct> _pimCategoryProductRepository;
        private readonly IZnodeRepository<ZnodePimCategoryHierarchy> _pimCategoryHierarchyRepository;
        private readonly IZnodeRepository<ZnodeLocale> _localeRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetail> _seoDetailRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IWebSiteService webSiteService;
        private ProductAssociationHelper productAssociationHelper;
        private readonly IZnodeRepository<ZnodeCMSPortalSEOSetting> _portalSEOSettingRepository;

        #endregion

        #region Constructor
        public CategoryService()
        {
            _pimCategory = new ZnodeRepository<ZnodePimCategory>();
            _pimCategoryProductRepository = new ZnodeRepository<ZnodePimCategoryProduct>();
            _pimCategoryHierarchyRepository = new ZnodeRepository<ZnodePimCategoryHierarchy>();
            _localeRepository = new ZnodeRepository<ZnodeLocale>();
            _seoDetailRepository = new ZnodeRepository<ZnodeCMSSEODetail>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            webSiteService = ZnodeDependencyResolver.GetService<IWebSiteService>();
            productAssociationHelper = new ProductAssociationHelper();
            _portalSEOSettingRepository = new ZnodeRepository<ZnodeCMSPortalSEOSetting>();
        }
        #endregion

        #region Public Methods

        //Get a list of Categories
        public virtual CategoryListModel GetCategories(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {       
            

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            ZnodeLogging.LogMessage("pageListModel for GetXmlCategory:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //gets the order by clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);
            
            //This condition execute only when Category list is required for Catalog category association.
            if (filters.Any(filterTuple => filterTuple.FilterName.Equals(FilterKeys.IsAssociatedProducts)))
                {
                    int catalogId = Convert.ToInt32(filters.Where(filterTuple => filterTuple.Item1 == ZnodePimCatalogEnum.PimCatalogId.ToString().ToLower())?.FirstOrDefault()?.Item3);
                    ZnodeLogging.LogMessage("catalogId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, catalogId );

                    List<int?> categoryIds = (from catalogHierarchy in _pimCategoryHierarchyRepository.Table
                                              where catalogHierarchy.PimCatalogId == catalogId
                                              select catalogHierarchy.PimCategoryId).ToList();
                    ZnodeLogging.LogMessage("categoryIds list :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose,  categoryIds?.Count() );

                    filters.Remove(filters.Where(x => x.FilterName == ZnodePimCatalogEnum.PimCatalogId.ToString().ToLower())?.FirstOrDefault());
                    filters.Remove(filters.Where(x => x.FilterName == FilterKeys.IsAssociatedProducts.ToLower())?.FirstOrDefault());

                    if (categoryIds?.Count > 0) { filters.Add(new FilterTuple(ZnodePimCategoryEnum.PimCategoryId.ToString(), FilterOperators.NotIn, string.Join(",", categoryIds))); }
                }

            var xmlList = GetXmlCategory(filters, pageListModel, orderBy);

            CategoryListModel categoryList = new CategoryListModel
                {
                    Categories = new List<CategoryModel>(),
                    Locale = GetActiveLocaleList(),
                    AttrubuteColumnName = xmlList.AttrubuteColumnName,
                    XmlDataList = xmlList.XmlDataList
                };

            categoryList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return categoryList;
        }
    
        // Get category details for edit
        public virtual PIMFamilyDetailsModel GetCategory(int categoryId, int familyId, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters categoryId, familyId, localeId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { categoryId, familyId, localeId });

            //Check category id exist in database or not.
            int categoryExist = _pimCategory.Table.Where(x => x.PimCategoryId == categoryId).Select(x => x.PimCategoryId).FirstOrDefault();

            if (categoryId != 0 && categoryExist == 0)
                return null;

            //Get all attributes associated with CategoryId and with familyId associated with categoryId
            IZnodeViewRepository<View_PimCategoryAttributeValues> pimAttributeValues = new ZnodeViewRepository<View_PimCategoryAttributeValues>();
            pimAttributeValues.SetParameter("ChangeFamilyId", familyId, ParameterDirection.Input, DbType.Int32);
            pimAttributeValues.SetParameter("PimCategoryId", categoryId, ParameterDirection.Input, DbType.Int32);
            pimAttributeValues.SetParameter(ZnodePimAttributeLocaleEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
            var pimAttributes = pimAttributeValues.ExecuteStoredProcedureList("Znode_GetPimCategoryAttributeValues @ChangeFamilyId, @PimCategoryId, @LocaleId");
            ZnodeLogging.LogMessage("All attributes associated with familyId and categoryId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributes?.Count());
    
            int associatedFamilyId = pimAttributes.FirstOrDefault(e => e.FamilyCode != "DefaultCategory")?.PimAttributeFamilyId ?? 0;
            ZnodeLogging.LogMessage("Associated Family with Id :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, associatedFamilyId);

            //Get all groups associated with default family and familyId

            IList<View_PimAttributeGroupbyFamily> pimAttributeGroups = GetGroupsAssociatedWithFamily(associatedFamilyId, true, localeId);
            ZnodeLogging.LogMessage("Groups associated with family :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeGroups?.Count);

            //Get All Pim category Families except the default family
            IList<PIMAttributeFamilyModel> pimAttributeFamilies = GetCategoryFamilies(localeId);
            ZnodeLogging.LogMessage("Category Families :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pimAttributeFamilies?.Count);

            return IsNotNull(pimAttributeGroups) && IsNotNull(pimAttributes) && IsNotNull(pimAttributeFamilies)
                ? MapToPIMFamilyDetailsModel(associatedFamilyId, categoryId, pimAttributes, pimAttributeGroups, pimAttributeFamilies) : null;
        }

        //Creates a new Category
        public virtual CategoryValuesListModel CreateCategory(CategoryValuesListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(model?.AttributeValues))
                    throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorCategoryModelNull);

            ZnodeLogging.LogMessage("SaveUpdateAttributeValues method with parameters.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.AttributeValues);
            IList<View_ReturnBoolean> SaveStatus = SaveUpdateAttributeValues(model?.AttributeValues);
            
            if (SaveStatus.FirstOrDefault().Status.Value)
                {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessSaveCategory, model.AttributeValues.FirstOrDefault().AttributeValue), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                if (IsNotNull(model))
                        model.PimCategoryId = SaveStatus.FirstOrDefault().Id;

                ZnodeLogging.LogMessage("InsertCatalogData with parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model);

                //Insert category data in catalog hierarchy table.
                InsertCategoryDataForCatalog(model);

                ZnodeLogging.LogMessage("Catalog data inserted.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, null);

                return model;
                }
                else
                {
                    ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorSaveCategory, model.AttributeValues.FirstOrDefault().AttributeValue), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return model;
                }
        }


        //Updates an already existing Category 
        public virtual bool UpdateCategory(CategoryValuesListModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(model?.AttributeValues))
                    throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorCategoryModelNull);

                ZnodeLogging.LogMessage("SaveUpdateAttributeValues with parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, model?.AttributeValues);
                IList<View_ReturnBoolean> SaveStatus = SaveUpdateAttributeValues(model?.AttributeValues);

            if (SaveStatus.FirstOrDefault()?.Status.GetValueOrDefault() ?? false)
                {
                    ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessUpdateCategory, model.AttributeValues.FirstOrDefault().AttributeValue), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return true;
                }
                else
                {
                    ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorUpdateCategory, model.AttributeValues.FirstOrDefault().AttributeValue), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return false;
                }

        }

        //Deletes a Category using catergoryId
        public virtual bool DeleteCategory(ParameterModel categoryIds)
        {
                ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("categoryIds to be deleted :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, categoryIds?.Ids);

                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter("PimCategoryIds", categoryIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimCategory @PimCategoryIds, @Status OUT", 1, out status);
                 ZnodeLogging.LogMessage("Deleted result count :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, deleteResult?.Count);

                if (deleteResult.FirstOrDefault().Status.Value)
                {
                    ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteCategory, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return true;
                }
                else
                {
                    ZnodeLogging.LogMessage(PIM_Resources.ErrorFailToDeleteCategory, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return false;
                }
        }

        //Deletes a products associated to category.
        public virtual bool DeleteCategoryAssociatedProducts(ParameterModel PimCategoryProductId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(PimCategoryProductId))
                throw new ZnodeException(ErrorCodes.NullModel, PIM_Resources.ErrorModelNull);

            if (PimCategoryProductId.Ids.Count() < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorPimCategoryProductIdLessThanOne);

            ZnodeLogging.LogMessage("PimCategoryProductId to be deleted :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, PimCategoryProductId.Ids);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimCategoryProductEnum.PimCategoryProductId.ToString(), ProcedureFilterOperators.In, PimCategoryProductId.Ids.ToString()));
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return _pimCategoryProductRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        // Delete associated categories to product.
        public virtual bool DeleteAssociatedCategoriesToProduct(ParameterModel PimCategoryProductId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (PimCategoryProductId.Ids.Count() < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorCategoryProductIdLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePimCategoryProductEnum.PimCategoryProductId.ToString(), ProcedureFilterOperators.In, PimCategoryProductId.Ids.ToString()));

            return _pimCategoryProductRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        //Get associate products to Category
        public virtual bool AssociateCategoryProduct(List<CategoryProductModel> categoryProductModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(categoryProductModel))
                throw new ZnodeException(ErrorCodes.NullModel, PIM_Resources.ErrorModelNull);

            if (categoryProductModel.Count < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorCategoryProductModelCount);

            int categoryId = Convert.ToInt32(categoryProductModel[0].PimCategoryId.ToString());
            ZnodeLogging.LogMessage("categoryId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, categoryId);

            var associatedProductList = _pimCategoryProductRepository.Table?.Where(x => x.PimCategoryId == categoryId).Select(y => y.PimProductId);

            categoryProductModel = associatedProductList.Any() ? categoryProductModel?.Where(i => !associatedProductList.Contains(i.PimProductId)).ToList() : categoryProductModel;

            if (categoryProductModel.Count == 0)
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }

            IEnumerable<ZnodePimCategoryProduct> associateCategoryProducts = _pimCategoryProductRepository.Insert(categoryProductModel.ToEntity<ZnodePimCategoryProduct>().ToList());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return associateCategoryProducts?.Count() > 0;
        }

        public bool AssociateCategoriesToProduct(List<CategoryProductModel> categoryProductModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(categoryProductModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (categoryProductModel.Count < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorCategoryProductModelCount);

            int categoryId = Convert.ToInt32(categoryProductModel[0].PimCategoryId.ToString());
            ZnodeLogging.LogMessage("Category Id :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, categoryId);

            var associatedProductList = _pimCategoryProductRepository.Table?.Where(x => x.PimCategoryId == categoryId).Select(y => y.PimProductId);
            ZnodeLogging.LogMessage("associatedProductList :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, associatedProductList.Count());

            categoryProductModel = associatedProductList.Any() ? categoryProductModel?.Where(i => !associatedProductList.Contains(i.PimProductId)).ToList() : categoryProductModel;

            if (categoryProductModel.Count == 0)
                return true;

            IEnumerable<ZnodePimCategoryProduct> associateCategoryProducts = _pimCategoryProductRepository.Insert(categoryProductModel.ToEntity<ZnodePimCategoryProduct>().ToList());
            ZnodeLogging.LogMessage("associateCategoryProducts :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, associateCategoryProducts.Count());

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return associateCategoryProducts?.Count() > 0;
        }

        //Get Product By Category 
        public virtual CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            int catalogId = 0;
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for ProductList:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            var list = ProductList(filters, pageListModel, categoryId, associatedProduts, catalogId);
            ZnodeLogging.LogMessage("product list count :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, list.Count);

            CategoryProductListModel categoryProductListModel = new CategoryProductListModel { CategoryProducts = list?.Count > 0 ? list.ToList() : null };
            
            //Get PimCategoryProductId 
            if (categoryProductListModel.CategoryProducts?.Count > 0)
                foreach (var item in categoryProductListModel.CategoryProducts)
                    item.PimCategoryProductId = _pimCategoryProductRepository.Table.Where(x => x.PimCategoryId == categoryId && x.PimProductId == item.PimProductId).Select(g => g.PimCategoryProductId).FirstOrDefault();
            
            categoryProductListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return categoryProductListModel;
        }

        //Get associated Product By Category 
        public virtual CategoryProductListModel GetAssociatedCategoryProducts(int categoryId, bool associatedProducts, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //Get locale id from filter otherwise set default
            int localeId = GetLocaleId(filters);
            string whereClause = ProductService.GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated  :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause);

            string attributeCode = GetAttributeCodes(filters);
            
            IZnodeViewRepository<CategoryProductModel> objStoredProc = new ZnodeViewRepository<CategoryProductModel>();
            objStoredProc.SetParameter("@WhereClause", whereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", associatedProducts, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodePimCategoryEnum.PimCategoryId.ToString(), categoryId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@AttributeCode", attributeCode, ParameterDirection.Input, DbType.String);
            //List of all products associated to category.
            IList<CategoryProductModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPimCategoryProductList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PimCategoryId,@IsAssociated,@AttributeCode", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("List of all products associated to category count :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, list.Count);
            CategoryProductListModel categoryProductListModel = new CategoryProductListModel { CategoryProducts = list?.Count > 0 ? list.ToList() : null };
           
            //Get PimCategoryProductId 
            if (categoryProductListModel.CategoryProducts?.Count > 0)
                foreach (var item in categoryProductListModel.CategoryProducts)
                    item.PimCategoryProductId = _pimCategoryProductRepository.Table.Where(x => x.PimCategoryId == categoryId && x.PimProductId == item.PimProductId).Select(g => g.PimCategoryProductId).FirstOrDefault();
            
            categoryProductListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return categoryProductListModel;
        }

        //Get the list of associated categories to product. 
        public virtual CategoryProductListModel GetAssociatedCategoriesToProducts(int productId, bool associatedProducts, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters productId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, productId);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //Get locale id from filter otherwise set default
            int localeId = GetLocaleId(filters);
            string whereClause = ProductService.GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated  :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause);

            string attributeCode = GetAttributeCodes(filters);
            IZnodeViewRepository<CategoryProductModel> objStoredProc = new ZnodeViewRepository<CategoryProductModel>();
            objStoredProc.SetParameter("@WhereClause", whereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PimProductIdInput", productId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", associatedProducts, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@AttributeCode", attributeCode, ParameterDirection.Input, DbType.String);

            //List of all categories associated to products.
            IList<CategoryProductModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPimProductCategoryList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PimProductIdInput,@IsAssociated,@AttributeCode", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("List of all categories associated to products count :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, list.Count);

            CategoryProductListModel categoryProductListModel = new CategoryProductListModel { CategoryProducts = list?.Count > 0 ? list.ToList() : null };
           
            categoryProductListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return categoryProductListModel;
        }

        //Get locale id from filter otherwise set default
        public static int GetLocaleId(FilterCollection filters)
        {
            int localeId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale);
            //Checking For LocaleId exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()))
            {
                localeId = Convert.ToInt32(filters.Where(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower());
            }

            return localeId;
        }
        //Get all groups associated with default family and familyId
        public static IList<View_PimAttributeGroupbyFamily> GetGroupsAssociatedWithFamily(int familyId, bool IsCategory, int localeId = 0)
        {
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { familyId, localeId });

            localeId = localeId == 0 ? Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale) : localeId;
            IZnodeViewRepository<View_PimAttributeGroupbyFamily> pimAttributeGroupByFamily = new ZnodeViewRepository<View_PimAttributeGroupbyFamily>();
            pimAttributeGroupByFamily.SetParameter(ZnodePimAttributeFamilyEnum.PimAttributeFamilyId.ToString(), familyId, ParameterDirection.Input, DbType.Int32);
            pimAttributeGroupByFamily.SetParameter(ZnodePimAttributeGroupLocaleEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
            pimAttributeGroupByFamily.SetParameter(ZnodePimAttributeGroupEnum.IsCategory.ToString(), IsCategory, ParameterDirection.Input, DbType.Boolean);
            return pimAttributeGroupByFamily.ExecuteStoredProcedureList("Znode_GetPimAttributeGroupbyFamily @PimAttributeFamilyId,@IsCategory,@LocaleId");
        }

        #region Category Publish

        //Publish single category using pim category id
        public virtual PublishedModel Publish(ParameterModel parameterModel)
        {
            ZnodeLogging.LogMessage("Execution started", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            PublishCategoryDataService _publishCategoryDataService = (PublishCategoryDataService)GetService<IPublishCategoryDataService>();

            //Check whether any other catalog is in publish state or not. #Step 1
            if (_publishCategoryDataService.IsCatalogPublishInProgress())
                throw new ZnodeException(ErrorCodes.CategoryPublishError, PIM_Resources.ErrorPublishCatalogCategory);

            if(_publishCategoryDataService.IsExportPublishInProgress())
            {
                throw new ZnodeException(ErrorCodes.NotPermitted, PIM_Resources.ErrorPublishCatalog);
            }

            int pimCategoryId, pimCatalogId = 0;
            int isAssociate = 0;
            bool isPublished = false;
            bool status = false;

            Int32.TryParse(parameterModel.Ids, out pimCategoryId);

            //Bind revision type if revision type is empty, means category is been published in only Production mode.
            string revisionType = string.IsNullOrEmpty(parameterModel?.RevisionType) ? "NONE" : parameterModel?.RevisionType;

            if (IsNotNull(parameterModel.publishCataLogId) && parameterModel.publishCataLogId > 0)
            {
                pimCatalogId = parameterModel.publishCataLogId;
            }

            try
            {
                //Call master sp to perform all the category publish operation. #Step 2
                DataSet dataSet = _publishCategoryDataService.ProcessSingleCategoryPublish(pimCategoryId, pimCatalogId, revisionType, out status, out isAssociate);

                //Check whether the data set contains data or not #Step 3
                if (status && dataSet?.Tables[0]?.Rows?.Count > 0)
                {
                    List<int> catalogIds = dataSet.Tables[0].AsEnumerable().Select(dataRow => dataRow.Field<int>("PublishCatalogId")).Distinct().ToList();

                    //Clear webstore and cloudflare cache #Step 4
                    ClearCloudflareCacheAfterPublish(dataSet);

                    //Clear portal cache data based on catalogId #Step 5
                    foreach (int catalogId in catalogIds)
                    {
                        ClearCacheAfterPublish(catalogId);
                    }

                    //Update flag value true if all operation of catalog publish execute successfully #Step 6
                    isPublished = true;
                }
                else
                {
                    //If status is 0 means category is not assigned to any published catalog
                    if (isAssociate == 0)
                        throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.CategoryPublishError);
                    else
                        return new PublishedModel { IsPublished = false, ErrorMessage = Admin_Resources.ErrorPublished };
                }

                ZnodeLogging.LogMessage("Category publish execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                //Return model with proper details. #Step 7
                return isPublished ? new PublishedModel { IsPublished = true, ErrorMessage = Admin_Resources.SuccessPublish }
                                     : new PublishedModel { IsPublished = false, ErrorMessage = Admin_Resources.ErrorPublished };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        //Gets the attribute code form filters.
        protected virtual string GetAttributeCodes(FilterCollection filters)
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

        //Return List of categories on basis of where clause
        private CategoryListModel GetXmlCategory(FilterCollection filters, PageListModel pageListModel, string orderBy)
        {
            //Get locale Id.
            int localeId = GetLocaleId(filters);

            bool isCatalogFilter= false;
            //Get PIM Catalog Id from filters.
            int pimCatalogId = ServiceHelper.GetCatalogFilterValues(filters, ref isCatalogFilter);

            //gets the where clause with filter Values.              
            string whereClause = ProductService.GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause);

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@WhereClause", whereClause, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@Order_By", orderBy, ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@PimCatalogId", pimCatalogId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@IsCatalogFilter", isCatalogFilter, ParameterDirection.Input, SqlDbType.Bit);

            var ds = objStoredProc.GetSPResultInDataSet("Znode_ManageCategoryList_XML");
            
            return Xmldataset(ds, out pageListModel.TotalRowCount);
        }

        //Convert dataset to dynamic list
        private CategoryListModel Xmldataset(DataSet ds, out int recordCount)
        {
            // out pageListModel.TotalRowCount
            recordCount = 0;
            if (!Equals(ds, null) && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var xml = Convert.ToString(ds.Tables[0]?.Rows[0]["CategoryXML"]);
               
                if (!string.IsNullOrEmpty(xml))
                {
                    var columns = ds.Tables[1];

                    var _columnlist = GetDictionary(columns);

                    if (!string.IsNullOrEmpty(ds.Tables[2].Rows[0]["RowsCount"].ToString()))
                        recordCount = Convert.ToInt32(ds.Tables[2].Rows[0]["RowsCount"].ToString());

                    XDocument xmlDoc = XDocument.Parse(xml);

                    dynamic root = new ExpandoObject();

                    XmlToDynamic.Parse(root, xmlDoc.Elements().First());

                    var _list = new List<dynamic>();
                    if (!(root.MainCategory.Category is List<dynamic>))
                    {
                        _list.Add(root.MainCategory.Category);
                    }
                    else
                    {
                        _list = (List<dynamic>)root.MainCategory.Category;
                    }

                    return new CategoryListModel { AttrubuteColumnName = _columnlist, XmlDataList = _list };
                }
            }
            return new CategoryListModel();
        }

        private Dictionary<string, object> GetDictionary(DataTable dt)
        {
            return dt.AsEnumerable()
              .ToDictionary<DataRow, string, object>(row => row.Field<string>(0),
                                        row => row.Field<object>(1));
        }

        //return List of product on basis of where clause
        protected virtual IList<CategoryProductModel> ProductList(FilterCollection filters, PageListModel pageListModel, int categoryId, bool associatedProduts, int catalogId)
        {
            int localeId = GetLocaleId(filters);

            string whereClause = ProductService.GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClause);

            string attributeCode = GetAttributeCodes(filters);
            int portalId = GetPortalId(filters);
            ZnodeLogging.LogMessage("portalId :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, portalId);

            IZnodeViewRepository<CategoryProductModel> objStoredProc = new ZnodeViewRepository<CategoryProductModel>();
            objStoredProc.SetParameter("@WhereClause", whereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodePimCategoryEnum.PimCategoryId.ToString(), categoryId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodePimCatalogEnum.PimCatalogId.ToString(), catalogId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", associatedProduts, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@AttributeCode", attributeCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PortalId", portalId.ToString(), ParameterDirection.Input, DbType.Int32);
            //List of all products associated to category.
            return objStoredProc.ExecuteStoredProcedureList("ZNode_GetCatalogCategoryProducts @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PimCategoryId,@PimCatalogId,@IsAssociated,@AttributeCode,@PortalId", 4, out pageListModel.TotalRowCount)?.ToList();

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
            return PortalId;
        }

        //Insert/Update category.
        private IList<View_ReturnBoolean> SaveUpdateAttributeValues(List<PIMCategoryValuesListModel> attributeValues)
        {
            string xmlData = ToXML<List<PIMCategoryValuesListModel>>(attributeValues);
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("CategoryXML", xmlData, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            return objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdatePimCategory @CategoryXML,@Status OUT, @UserId ", 1, out status);
        }

        //Get All Pim category Families except the default family
        private IList<PIMAttributeFamilyModel> GetCategoryFamilies(int localeId = 0)
        {
            //Get all family with familyId
            IZnodeViewRepository<PIMAttributeFamilyModel> pimAttributeValues = new ZnodeViewRepository<PIMAttributeFamilyModel>();
            pimAttributeValues.SetParameter(ZnodePimAttributeEnum.IsCategory.ToString(), true, ParameterDirection.Input, DbType.Boolean);
            pimAttributeValues.SetParameter(ZnodePimAttributeLocaleEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
            var pimFamilies = pimAttributeValues.ExecuteStoredProcedureList("Znode_GetPimAttributeFamilyList @IsCategory, @LocaleId");
            return pimFamilies;
        }

        private PIMFamilyDetailsModel MapToPIMFamilyDetailsModel(int familyId, int categoryId, IList<View_PimCategoryAttributeValues> pimAttributes, IList<View_PimAttributeGroupbyFamily> pimAttributeGroups, IList<PIMAttributeFamilyModel> pimAttributeFamilies)
        => new PIMFamilyDetailsModel
        { 

            PimAttributeFamilyId = familyId,
            Attributes = pimAttributes.ToModel<PIMProductAttributeValuesModel>().ToList(),
            Groups = pimAttributeGroups.ToModel<PIMAttributeGroupModel>().ToList(),
            Family = pimAttributeFamilies.ToList(),
            Name = pimAttributes.FirstOrDefault(x => x.AttributeCode == "CategoryName")?.AttributeValue,
            Id = categoryId,
            Locale = _localeRepository.Table.Where(x => x.IsActive).ToModel<LocaleModel>().ToList(),
        };

        //Insert category data in catalog hierarchy table.
        private void InsertCategoryDataForCatalog(CategoryValuesListModel model)
        {
            if (model?.PimCatalogId > 0)
            {
                IZnodeRepository<ZnodePimCategoryHierarchy> _categoryHierarchyRepository = new ZnodeRepository<ZnodePimCategoryHierarchy>();

                model.PimParentCategoryId = model.PimParentCategoryId > 0 ? model.PimParentCategoryId : null;
                int maxDisplayOrder = GetMaxDisplayOrder(model.PimCatalogId.GetValueOrDefault(), model.PimParentCategoryId) + 75000;

                if (_categoryHierarchyRepository.Insert(new ZnodePimCategoryHierarchy { IsActive = true, PimCatalogId = model.PimCatalogId, PimCategoryId = model.PimCategoryId, ParentPimCategoryHierarchyId = model.PimParentCategoryId, DisplayOrder = maxDisplayOrder })?.PimCategoryHierarchyId > 0)
                    ZnodeLogging.LogMessage(PIM_Resources.SuccessInsertInCategoryHierarchy, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                else
                    ZnodeLogging.LogMessage(PIM_Resources.ErrorInsertInCategoryHierarchy, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            }
        }

        //Get the maximum display order of category.
        private int GetMaxDisplayOrder(int catalogId, int? parentCategory)
            => _pimCategoryHierarchyRepository.Table.Where(x => x.PimCatalogId == catalogId && x.ParentPimCategoryHierarchyId == parentCategory)?.Max(x => x.DisplayOrder) ?? 0;

        

        //Clear webstore and cloudflare cache after category publish
        private void ClearCloudflareCacheAfterPublish(DataSet resultDataSet)
        {
            if (HelperUtility.IsNull(resultDataSet) || resultDataSet.Tables.Count < 3)
            {
                return;
            }

            if (resultDataSet?.Tables[1]?.Rows?.Count > 0)
            {
                List<CategoryPublishEventModel> categoryPublishEventModel = resultDataSet.Tables[1].AsEnumerable().DistinctBy(dataRow =>
               new
               {
                   PimPortalId = dataRow.Field<int>("PortalId"),
                   PublishCatalogId = dataRow.Field<int>("PublishCatalogId")
               })
                .Select(dataRow => new CategoryPublishEventModel
                {
                    PortalId = dataRow.Field<int>("PortalId"),
                    CatalogId = dataRow.Field<int>("PublishCatalogId"),
                    CategoryId = dataRow.Field<int>("PublishCategoryId"),
                    SeoUrl = dataRow.Field<string>("SEOUrl")
                }).ToList();

                //Clear cache call               
                var clearCacheInitializer = new ZnodeEventNotifier<List<CategoryPublishEventModel>>(categoryPublishEventModel);
            }
        }

        //Update product details associated to category.
        public virtual bool UpdateCategoryProductDetail(CategoryProductModel categoryProductModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { catalogAssociationModel = categoryProductModel });
            bool isUpdated = false;
            if (IsNotNull(categoryProductModel))
            {
                ZnodePimCategoryProduct znodePimCategoryProduct = _pimCategoryProductRepository.Table
                                                            .FirstOrDefault(m => m.PimCategoryProductId == categoryProductModel.PimCategoryProductId);
                if (IsNotNull(znodePimCategoryProduct))
                {
                    znodePimCategoryProduct.DisplayOrder = categoryProductModel.DisplayOrder;
                    znodePimCategoryProduct.ModifiedDate = categoryProductModel.ModifiedDate;
                    isUpdated = _pimCategoryProductRepository.Update(znodePimCategoryProduct);
                    ZnodeLogging.LogMessage(isUpdated ? string.Format(PIM_Resources.SuccessAssociatedProductUpdate, categoryProductModel.PimCategoryProductId) : PIM_Resources.ErrorUpdateAssociateProductDetails, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return isUpdated;
        }
        //Clear Cache of portal after catalog publish.
        private void ClearCacheAfterPublish(int publishCatalogId)
        {
            List<ZnodePortalCatalog> associatedPortalId = _portalCatalogRepository.Table.Where(portalCatalog => portalCatalog.PublishCatalogId == publishCatalogId)?.ToList();
            int[] ids = associatedPortalId.Select(x => x.PortalId).ToArray();
            ClearCacheHelper.EnqueueEviction(new PortalPublishEvent()
            {
                Comment = $"From category service clearing cache after publish of category within catalog with id '{publishCatalogId}'.",
                PortalIds = ids
            });
        }
        #endregion
    }
}
