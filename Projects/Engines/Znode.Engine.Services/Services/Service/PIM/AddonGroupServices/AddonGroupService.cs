using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class AddonGroupService : BaseService, IAddonGroupService
    {
        private readonly IZnodeRepository<ZnodePimAddonGroup> _addonGroupRepository;
        private readonly IZnodeRepository<ZnodePimAddonGroupLocale> _addonGroupLocaleRepository;
        private readonly IZnodeRepository<ZnodePimAddonGroupProduct> _addonGroupProductRepository;
        private readonly IProductService _productService;

        public AddonGroupService(IProductService productService)
        {
            _addonGroupLocaleRepository = new ZnodeRepository<ZnodePimAddonGroupLocale>();
            _addonGroupRepository = new ZnodeRepository<ZnodePimAddonGroup>();
            _addonGroupProductRepository = new ZnodeRepository<ZnodePimAddonGroupProduct>();
            _productService = productService;
        }

        #region Public methods

        #region Addon Group

        //Get addonGroup using filters.
        public virtual AddonGroupModel GetAddonGroup(FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            var navigationProperties = GetExpands(expands);

            int localeId = Convert.ToInt32(filters.Where(x => x.FilterName == ZnodePimAddonGroupLocaleEnum.LocaleId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
            filters.RemoveAll(x => x.FilterName == ZnodePimAddonGroupLocaleEnum.LocaleId.ToString().ToLower());

            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodePimAddonGroup addonGroupEntity = _addonGroupRepository.GetEntity(whereClause.WhereClause, navigationProperties);
            
            AddonGroupModel addonGroupModel = addonGroupEntity.ToModel<AddonGroupModel>();

            addonGroupModel.PimAddonGroupLocales = addonGroupEntity.ZnodePimAddonGroupLocales.ToModel<AddonGroupLocaleModel, ZnodePimAddonGroupLocale>().AsEnumerable().Where(x => x.LocaleId == localeId).Select(x => x).ToList();
            addonGroupModel.LocaleId = localeId;
            if (IsNotNull(addonGroupModel) && addonGroupModel.PimAddonGroupLocales.Count == 0)
            {
                addonGroupModel.PimAddonGroupLocales = addonGroupEntity.ZnodePimAddonGroupLocales.ToModel<AddonGroupLocaleModel, ZnodePimAddonGroupLocale>().AsEnumerable().Where(x => x.LocaleId == Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale)).Select(x => x).ToList();
                addonGroupModel.AddonGroupName = addonGroupModel.PimAddonGroupLocales.Where(x => x.LocaleId == Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale)).Select(x => x.AddonGroupName).FirstOrDefault();
            }
            ZnodeLogging.LogMessage("AddonGroupName, PimAddonGroupId and PimAddonGroupLocales list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { addonGroupModel?.AddonGroupName,
            addonGroupModel?.PimAddonGroupId, addonGroupModel?.PimAddonGroupLocales?.Count});
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return addonGroupModel;
        }

        //Create addonGroup.
        public virtual AddonGroupModel CreateAddonGroup(AddonGroupModel addonGroup)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (IsNull(addonGroup))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAddonGroupNull);

            //Remove blank spaces included into Add-On group name.
            foreach (var item in addonGroup.PimAddonGroupLocales)
                item.AddonGroupName = item.AddonGroupName.Trim();

            if (IsAddonGroupExists(addonGroup.PimAddonGroupLocales[0].AddonGroupName, addonGroup.PimAddonGroupLocales[0].LocaleId.GetValueOrDefault()))
                throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.ErrorAddonGroupAlreadyExists);

            ZnodePimAddonGroup addonGroupEntity = _addonGroupRepository.Insert(addonGroup.ToEntity<ZnodePimAddonGroup>());
            ZnodeLogging.LogMessage("PimAddonGroup with Id: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { addonGroupEntity?.PimAddonGroupId });
            
            if (addonGroupEntity?.PimAddonGroupId > 0)
                {
                    addonGroup.PimAddonGroupId = addonGroupEntity.PimAddonGroupId;

                    //At a time only one locale will be inserted; hence index 0 is used.
                    addonGroup.PimAddonGroupLocales[0].PimAddonGroupId = addonGroupEntity.PimAddonGroupId;
                    addonGroup.PimAddonGroupLocales = InsertAddonGroupLocale(addonGroup);
                    ZnodeLogging.LogMessage("PimAddonGroupLocales list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { addonGroup?.PimAddonGroupLocales?.Count });
            
                    if (addonGroup.PimAddonGroupLocales[0].PimAddonGroupLocaleId > 0)
                    {
                        ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessCreateAddOnGroup, addonGroup.PimAddonGroupLocales.FirstOrDefault().AddonGroupName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                        return addonGroupEntity.ToModel<AddonGroupModel>();
                    }
                }
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorCreateAddOnGroup, addonGroup.PimAddonGroupLocales.FirstOrDefault().AddonGroupName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return null;
        }

        //Update addon group
        public virtual bool UpdateAddonGroup(AddonGroupModel addonGroup)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodePimAddonGroup addonGroupEntity = new ZnodePimAddonGroup();
            bool isUpdated = false;
            if (IsNull(addonGroup))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAddonGroupNull);

            // Remove blank spaces included into Add-On group name.
            foreach (var item in addonGroup.PimAddonGroupLocales)
                item.AddonGroupName = item.AddonGroupName.Trim();

            if (IsAddonGroupExistsOnUpdate(addonGroup.PimAddonGroupLocales[0].AddonGroupName, Convert.ToInt32(addonGroup?.PimAddonGroupLocales[0]?.LocaleId), addonGroup.PimAddonGroupId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, PIM_Resources.ErrorAddonGroupAlreadyExists);

            if (IsAddonGroupInsertLocalewise(Convert.ToInt32(addonGroup?.PimAddonGroupLocales[0]?.LocaleId), addonGroup.PimAddonGroupId))
            {
                ZnodeLogging.LogMessage("Addon group with Id to be updated: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, addonGroup?.PimAddonGroupId);
                isUpdated = _addonGroupRepository.Update(addonGroup.ToEntity<ZnodePimAddonGroup>());
            }

            if (isUpdated)
            {
                isUpdated &= UpdateAddonGroupLocale(addonGroup);
                ZnodeLogging.LogMessage(isUpdated ? string.Format(PIM_Resources.SuccessUpdateAddOnGroup, addonGroup?.PimAddonGroupLocales?.FirstOrDefault()?.AddonGroupName) : string.Format(PIM_Resources.ErrorUpdateAddOnGroup, addonGroup.AddonGroupName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return isUpdated;
            }
            else
            {
                addonGroup.PimAddonGroupLocales = InsertAddonGroupLocale(addonGroup);
                ZnodeLogging.LogMessage("PimAddonGroupLocales list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { addonGroup?.PimAddonGroupLocales?.Count });
            }

            if (addonGroup.PimAddonGroupLocales[0].PimAddonGroupLocaleId > 0)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessCreateAddOnGroup, addonGroup.PimAddonGroupLocales.FirstOrDefault().AddonGroupName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                addonGroupEntity.ToModel<AddonGroupModel>();
                return true;
            }
            ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorUpdateAddOnGroup, addonGroup?.PimAddonGroupLocales?.FirstOrDefault()?.AddonGroupName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return false;
        }

        //Get list of addonGroup.
        public virtual AddonGroupListModel GetAddonGroupList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Set default locale Id.
            int localeId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale);
            //Checking For LocaleId exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()))
            {
                //Set locale from filter if present.
                localeId = Convert.ToInt32(filters.Where(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower());
            }

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<View_GetPimAddonGroups> objStoredProc = new ZnodeViewRepository<View_GetPimAddonGroups>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);

            ZnodeLogging.LogMessage("pageListModel and localeId to get addon groups list: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), localeId });
            IList<View_GetPimAddonGroups> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPimAddonGroups @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("PimAddonGroups list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { list?.Count });

            AddonGroupListModel addonGroup = new AddonGroupListModel { AddonGroups = list.ToModel<AddonGroupModel>().ToList() };
            addonGroup.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return addonGroup;
        }

        //Delete addonGroup by addonGroupIds.
        public virtual bool DeleteAddonGroup(ParameterModel addonGroupIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PimAddOnGroupId", addonGroupIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            ZnodeLogging.LogMessage("addonGroupIds to be deleted: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, addonGroupIds?.Ids);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimAddOnGroup @PimAddOnGroupId,@Status OUT", 1, out status);
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteAddOnGroup, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteAddonGroup, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorDeleteAddonGroup);
            }
        }

        #endregion Addon Group

        #region Addon group product association

        //Creates association of addon groups and its associated products.
        public virtual bool AssociateAddonGroupProduct(AddonGroupProductListModel addonGroupProducts)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (addonGroupProducts?.AddonGroupProducts?.Count() <= 0)
                throw new ZnodeException(ErrorCodes.NullModel, PIM_Resources.ErrorAddonGroupProductModelNull);

            List<ZnodePimAddonGroupProduct> addonGroupProductEntities = new List<ZnodePimAddonGroupProduct>();
            //Prepare a list of entities to insert.
            foreach (AddonGroupProductModel addonGroupProduct in addonGroupProducts?.AddonGroupProducts)
                addonGroupProductEntities.Add(new ZnodePimAddonGroupProduct { PimAddonGroupId = addonGroupProduct.PimAddonGroupId, PimChildProductId = addonGroupProduct.PimChildProductId });

            ZnodeLogging.LogMessage("addonGroupProductEntities list count: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, addonGroupProductEntities?.Count);
            return (_addonGroupProductRepository.Insert(addonGroupProductEntities)).ToModel<AddonGroupProductModel>()?.Count() > 0;
        }

        //Get unassociated addon product by addonProductId.
        public virtual ProductDetailsListModel GetUnassociatedAddonGroupProducts(int addonGroupId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            int localeId = Convert.ToInt32(filters.Where(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
            ProductDetailsListModel unassociatedAddonGroupProducts = null;
            string associatedProductIds = string.Empty;

            ZnodeLogging.LogMessage("addonGroupId to get associatedProductIds: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, addonGroupId);
            if (addonGroupId > 0)
                associatedProductIds = string.Join(",", _addonGroupProductRepository.Table?.Where(x => x.PimAddonGroupId == addonGroupId)?.Select(y => Equals(y.PimChildProductId, null) ? 0 : y.PimChildProductId)?.ToArray());

            //gets the where clause with filter Values.
            filters.Add(View_ManageProductListEnum.ProductType.ToString(), ProcedureFilterOperators.Is, GetProductTypeByLocale(localeId));

            filters.Add(ZnodeLocaleEnum.LocaleId.ToString().ToLower(), FilterOperators.Equals, localeId.ToString());
            ZnodeLogging.LogMessage("GetXmlProduct method call with parameters: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), associatedProductIds, true });
            unassociatedAddonGroupProducts = _productService.GetXmlProduct(filters, pageListModel, associatedProductIds, true);
            unassociatedAddonGroupProducts.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return unassociatedAddonGroupProducts;
        }

        //Get unassociated addon product by addonProductId.
        public virtual ProductDetailsListModel GetAssociatedProducts(int addonGroupId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string localeId = filters.Where(x => x.Item1 == FilterKeys.LocaleId.ToLower()).Select(x => x.FilterValue).FirstOrDefault();
            ProductDetailsListModel unassociatedAddonGroupProducts = new ProductDetailsListModel();
            string associatedProductIds = string.Empty;
            List<ZnodePimAddonGroupProduct> associatedProducts = null;
            if (addonGroupId > 0)
            {
                ZnodeLogging.LogMessage("addonGroupId to get associatedProductIds: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, addonGroupId);
                associatedProducts = _addonGroupProductRepository.Table?.Where(x => x.PimAddonGroupId == addonGroupId)?.Select(x => x).ToList();
                associatedProductIds = string.Join(",", associatedProducts.Select(x => x.PimChildProductId.ToString()));
            }

            if (!string.IsNullOrEmpty(associatedProductIds))
            {
                //gets the where clause with filter Values.
                filters.Add(View_ManageProductListEnum.ProductType.ToString(), ProcedureFilterOperators.Is, GetProductTypeByLocale(Convert.ToInt32(localeId)));

                filters.Add(ZnodeLocaleEnum.LocaleId.ToString().ToLower(), FilterOperators.Equals, localeId);
                ZnodeLogging.LogMessage("GetXmlProduct method call with parameters", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), associatedProductIds, false });
                unassociatedAddonGroupProducts = _productService.GetXmlProduct(filters, pageListModel, associatedProductIds, false);
                unassociatedAddonGroupProducts?.XmlDataList
                   ?.ForEach(
                       x => x.AddonGroupProductId = (associatedProducts?.Where(y => y.PimChildProductId == Convert.ToInt32(x.PimProductId))?.Select(y => y.PimAddonGroupProductId).FirstOrDefault()).GetValueOrDefault()
                   );
            }
            unassociatedAddonGroupProducts.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return unassociatedAddonGroupProducts;
        }

        //Delete Addon Group product association.
        public virtual bool DeleteAddonGroupProductAssociation(ParameterModel addonGroupProductIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(addonGroupProductIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorAssociatedProductIdNull);

            FilterCollection filters = new FilterCollection();

            //Create tuple to generate where clause.
            filters.Add(new FilterTuple(ZnodePimAddonGroupProductEnum.PimAddonGroupProductId.ToString(), ProcedureFilterOperators.In, addonGroupProductIds.Ids));

            //Generating where clause to get account details.
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("WhereClause to delete addon group product association: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);
            return _addonGroupProductRepository.Delete(whereClauseModel?.WhereClause);
        }

        #endregion Addon group product association

        #endregion Public methods

        #region Private methods

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (!Equals(expands, null) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodePimAddonGroupEnum.ZnodePimAddonGroupLocales.ToString().ToLower()))
                        SetExpands(ZnodePimAddonGroupEnum.ZnodePimAddonGroupLocales.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Insert addon group locale.
        private List<AddonGroupLocaleModel> InsertAddonGroupLocale(AddonGroupModel addonGroup)
        {
            return _addonGroupLocaleRepository.Insert(addonGroup.PimAddonGroupLocales.ToEntity<ZnodePimAddonGroupLocale, AddonGroupLocaleModel>().ToList()).ToModel<AddonGroupLocaleModel, ZnodePimAddonGroupLocale>().ToList();
        }

        //Method to check addonGroup exists on create.
        private bool IsAddonGroupExists(string addonGroupName, int localeId)
            => _addonGroupLocaleRepository.Table.Any(x => x.LocaleId == localeId && x.AddonGroupName == addonGroupName);

        //Method to check addon groups exists on update.
        private bool IsAddonGroupExistsOnUpdate(string addonGroupName, int localeId, int addonGroupId)
        {
            ZnodeLogging.LogMessage("addonGroupName, localeId and addonGroupId to check addon group exists on update: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { addonGroupName, localeId, addonGroupId });
            var entityList = _addonGroupLocaleRepository.Table.Where(x => x.AddonGroupName == addonGroupName && x.LocaleId == localeId).Select(x => x);

            if (entityList.Count() == 0) return false;

            if (entityList.Count() == 1 && entityList.ToList()[0].PimAddonGroupId == addonGroupId) return false;

            return true;
        }

        //Method to check weather addon group is present with same localeId and pimAddonGroupId
        private bool IsAddonGroupInsertLocalewise(int localeId, int pimAddonGroupId)
        {
            ZnodeLogging.LogMessage("localeId and pimAddonGroupId: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { localeId, pimAddonGroupId });
            var entityList = _addonGroupLocaleRepository.Table.Where(x => x.PimAddonGroupId == pimAddonGroupId && x.LocaleId == localeId).Select(x => x);

            if (entityList.Count() == 1) return true;

            return false;
        }

        //Updates addon group locale.
        private bool UpdateAddonGroupLocale(AddonGroupModel addonGroup) =>
            _addonGroupLocaleRepository.Update(addonGroup.PimAddonGroupLocales[0].ToEntity<ZnodePimAddonGroupLocale>());

        //Get value of Product type from LocaleId
        private string GetProductTypeByLocale(int localeId)
        {
            ZnodeLogging.LogMessage("localeId to get product type: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, localeId);
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
            return Convert.ToString(executeSpHelper.GetSPResultInObject("Znode_GetDefaultSimpleProductFilter"));
        }
        #endregion Private methods
    }
}