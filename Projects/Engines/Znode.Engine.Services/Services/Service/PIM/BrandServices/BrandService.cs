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
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public partial class BrandService : BaseService, IBrandService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeBrandDetail> _brandRepository;
        private readonly IZnodeRepository<ZnodeBrandDetailLocale> _brandLocaleRepository;
        private readonly IZnodeRepository<ZnodeBrandPortal> _brandPortalRepository;
        private readonly IZnodeRepository<ZnodeBrandProduct> _brandProductRepository;
        private readonly IZnodeRepository<ZnodePimAttribute> _pimAttributeRepository;
        private readonly IZnodeRepository<ZnodePimAttributeDefaultValue> _pimAttributeDefaultValueRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetail> _seoDetailRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetailLocale> _seoDetailLocaleRepository;
        private readonly IZnodeRepository<ZnodeCMSSEOType> _seoTypeRepository;
        private readonly IZnodeRepository<ZnodePortalBrand> _portalBrandRepository;
        #endregion

        #region Constructor
        public BrandService()
        {
            _brandRepository = new ZnodeRepository<ZnodeBrandDetail>();
            _pimAttributeDefaultValueRepository = new ZnodeRepository<ZnodePimAttributeDefaultValue>();
            _pimAttributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            _seoDetailRepository = new ZnodeRepository<ZnodeCMSSEODetail>();
            _seoTypeRepository = new ZnodeRepository<ZnodeCMSSEOType>();
            _seoDetailLocaleRepository = new ZnodeRepository<ZnodeCMSSEODetailLocale>();
            _brandLocaleRepository = new ZnodeRepository<ZnodeBrandDetailLocale>();
            _brandPortalRepository = new ZnodeRepository<ZnodeBrandPortal>();
            _brandProductRepository = new ZnodeRepository<ZnodeBrandProduct>();
            _portalBrandRepository = new ZnodeRepository<ZnodePortalBrand>();
        }
        #endregion

        #region Public Methods

        //Create Brand.
        public virtual BrandModel CreateBrand(BrandModel brandModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(brandModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            ZnodeLogging.LogMessage("BrandModel with BrandId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { brandModel?.BrandId });

            ZnodeBrandDetail brandDetails = _brandRepository.Insert(brandModel.ToEntity<ZnodeBrandDetail>());

            ZnodeLogging.LogMessage((IsNotNull(brandDetails) ? string.Format(PIM_Resources.SuccessCreateBrand, brandModel.BrandName) : string.Format(PIM_Resources.ErrorCreateBrand, brandModel.BrandName)), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNotNull(brandDetails))
            {
                brandModel.BrandId = brandDetails.BrandId;
                ZnodeBrandDetailLocale brandDetailLocale = InsertBrandDetailsLocale(brandModel);
                ZnodeCMSSEODetail cmsSEODetail = InsertBrandSEODetails(brandModel);
                if (IsNotNull(cmsSEODetail))
                {
                    brandModel.CMSSEODetailId = cmsSEODetail.CMSSEODetailId;
                    InsertSEODetailLocale(brandModel);
                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                brandModel.BrandDetailLocaleId = IsNotNull(brandDetails.ZnodeBrandDetailLocales.FirstOrDefault().BrandDetailLocaleId) ? brandDetails.ZnodeBrandDetailLocales.FirstOrDefault().BrandDetailLocaleId : 0;
                return brandModel;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return brandModel;
        }

        //Get brand details by brand  id.
        public virtual BrandModel GetBrand(int brandId, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters brandId, localeId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { brandId, localeId });

            if (brandId > 0)
            {
                //Generate where clause.
                FilterCollection filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodeBrandDetailEnum.BrandId.ToString(), FilterOperators.Is, Convert.ToString(brandId)));
                string whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseForSP(filter.ToFilterDataCollection());
                ZnodeLogging.LogMessage("WhereClause to set SP Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClauseModel });

                //SP call
                IZnodeViewRepository<BrandModel> objStoredProc = new ZnodeViewRepository<BrandModel>();
                objStoredProc.SetParameter("@WhereClause", whereClauseModel, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@Rows", 10, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", 1, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Order_By", "", ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@RowCount", 1, ParameterDirection.Output, DbType.Int32);
                objStoredProc.SetParameter(ZnodeCMSSEODetailLocaleEnum.LocaleId.ToString(), localeId, ParameterDirection.Input, DbType.Int32);
                BrandModel brandModel = objStoredProc.ExecuteStoredProcedureList("Znode_GetBrandDetailsLocale @WhereClause,@Rows,@PageNo,@Order_By,@RowCount,@LocaleId")?.FirstOrDefault();
                //Get brand image path
                if (IsNotNull(brandModel))
                    GetBrandImagePath(PortalId, brandModel);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return brandModel;
            }
            return null;
        }

        //Update Brand.
        public virtual bool UpdateBrand(BrandModel brandModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(brandModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            if (brandModel.BrandId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorIDLessThanOne);

            ZnodeLogging.LogMessage("brandModel with BrandId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { brandModel?.BrandId });

            //Update brand
            bool isBrandUpdated = _brandRepository.Update(brandModel.ToEntity<ZnodeBrandDetail>());
            ZnodeLogging.LogMessage(isBrandUpdated ? string.Format(PIM_Resources.SuccessUpdateBrand, brandModel.BrandName) : string.Format(PIM_Resources.ErrorUpdateBrand, brandModel.BrandName), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ZnodeCMSSEODetail cmsSEODetail = _seoDetailRepository.Table.FirstOrDefault(x => x.SEOCode == brandModel.BrandCode);

            bool isBrandSEODetailsUpdated=false;
            if (IsNull(cmsSEODetail))
            {
                ZnodeCMSSEODetail sEODetail =InsertBrandSEODetails(brandModel);
                isBrandSEODetailsUpdated = true;
                brandModel.CMSSEODetailId = sEODetail.CMSSEODetailId;
            }
            else
            {
                brandModel.CMSSEODetailId = cmsSEODetail.CMSSEODetailId;
                isBrandSEODetailsUpdated =UpdateBrandSEODetails(brandModel);
            }                      

            if (!isBrandUpdated)
                return false;
            else if (!UpdateBrandDetailLocale(brandModel))
                return false;
            else if (!isBrandSEODetailsUpdated)
                return false;
            else if (!UpdateSEODetailLocale(brandModel))
                return false;
            else
                return true;
        }

        //Get list of Brand.
        public virtual BrandListModel GetBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Set Locale If Not Present.
            SetLocaleFilterIfNotPresent(ref filters);

            //Get locale Id and check if brand associated to product or not.
            int localeId = Convert.ToInt32(filters.Find(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.Item3);
            bool isProductAssociated = Convert.ToBoolean(filters.Find(x => x.FilterName.Equals(ZnodeConstant.IsAssociated,StringComparison.InvariantCultureIgnoreCase))?.Item3);
            bool isWebstore = Convert.ToBoolean(filters.Find(x => x.FilterName.Equals(ZnodeConstant.IsWebstore,StringComparison.InvariantCultureIgnoreCase))?.Item3);
            filters.RemoveAll(x => x.FilterName.Equals(ZnodeConstant.IsAssociated,StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.FilterName.Equals(ZnodeConstant.IsWebstore,StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase));

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IZnodeViewRepository<BrandModel> objStoredProc = new ZnodeViewRepository<BrandModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", isProductAssociated, ParameterDirection.Input, DbType.Boolean);

            IList<BrandModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetBrandDetailsLocale @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@IsAssociated", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Brand list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { list?.Count});

            BrandListModel listModel = new BrandListModel { Brands = list?.ToList() };

            //Set seo Datails to brand list.
            SetSEODetailToBrands(listModel);

            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Delete Brand.
        public virtual bool DeleteBrand(ParameterModel brandIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            if (IsNull(brandIds) || string.IsNullOrEmpty(brandIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, PIM_Resources.ErrorBrandIdLessThanOne);

            ZnodeLogging.LogMessage("brandIds to be deleted:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { brandIds?.Ids });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeBrandDetailEnum.BrandId.ToString(), brandIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;

            objStoredProc.ExecuteStoredProcedureList("Znode_DeleteBrand @BrandId,  @Status OUT", 1, out status);
            if (status == 1)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteBrand, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteBrand, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorDeleteBrand);
            }
        }

        //Get brand codes.
        public virtual BrandListModel GetBrandCodes(string attributeCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter attributeCode :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { attributeCode });

            //Filter for attribute code.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePimAttributeEnum.AttributeCode.ToString(), FilterOperators.Is, attributeCode));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());

            //Filter for PimAttributeId.
            filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePimAttributeDefaultValueEnum.PimAttributeId.ToString(), FilterOperators.Equals, _pimAttributeRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.PimAttributeId.ToString()));

            whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());

            _pimAttributeDefaultValueRepository.EnableDisableLazyLoading = true;
            PIMAttributeDefaultValueListModel pimAttributeDefaultValueListModel = PIMAttributesMap.ToDefaultValueListModel(_pimAttributeDefaultValueRepository.GetEntityList(whereClauseModel.WhereClause).ToList());
            if (pimAttributeDefaultValueListModel.DefaultValues?.Count > 0)
                return new BrandListModel() { BrandCodes = pimAttributeDefaultValueListModel.DefaultValues };
            else
                return null;
        }

        //Get available brand codes.
        public virtual BrandListModel GetAvailableBrandCodes(string attributeCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            BrandListModel brandListModel = new BrandListModel();

            brandListModel.BrandCodes = GetBrandCodes(attributeCode)?.BrandCodes;

            List<string> brandCodes = GetBrandList(null, null, null, null)?.Brands?.Select(x => x.BrandCode)?.ToList();
            ZnodeLogging.LogMessage("brandCodes returned from GetBrandList:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { brandListModel?.BrandCodes });

            if (brandCodes?.Count() > 0)
                brandListModel.BrandCodes = brandListModel?.BrandCodes?.Where(code => !brandCodes.Contains(code.AttributeDefaultValueCode))?.ToList();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return brandListModel;
        }

        //Get associate/unassociate products to brand.
        public virtual bool AssociateAndUnAssociateProduct(BrandProductModel brandProductModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("ProductId", brandProductModel.ProductIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("PimAttributeCode", brandProductModel.AttributeCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("AttributeValue", brandProductModel.AttributeValue, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("IsUnAssociated", brandProductModel.IsUnAssociated, ParameterDirection.Input, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UpdateAttributeValue @ProductId,@PimAttributeCode,@LocaleId,@AttributeValue,@UserId, @Status OUT,@IsUnAssociated", 5, out status);
            ZnodeLogging.LogMessage("deleteResult count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { deleteResult.Count });

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessAssociateProductToBrand, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorAssociateProductToBrand, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Active/Inactive Brands
        public virtual bool ActiveInactiveBrand(ActiveInactiveBrandModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            bool isBrandUpdated = false;
            if (string.IsNullOrEmpty(model.BrandId))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.BrandIdNotNull);

            ZnodeLogging.LogMessage("ActiveInactiveBrandModel with BrandId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { model?.BrandId });

            var brandIds = model.BrandId.Split(',').ToList();
            foreach (var item in brandIds)
            {
                int brandId = Convert.ToInt32(item);
                var brandModel = _brandRepository.Table.FirstOrDefault(x => x.BrandId == brandId);

                if (brandModel.IsActive != model.IsActive)
                {
                    brandModel.IsActive = model.IsActive;
                    isBrandUpdated = _brandRepository.Update(brandModel);
                }
                else
                    isBrandUpdated = true;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isBrandUpdated;
        }

        public virtual PortalBrandListModel GetBrandPortalList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            //Set Authorized Portal filter based on user portal access.
            BindUserPortalFilter(ref filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IZnodeViewRepository<PortalBrandModel> objStoredProc = new ZnodeViewRepository<PortalBrandModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            List<PortalBrandModel> publishPortalLogs = objStoredProc.ExecuteStoredProcedureList("Znode_GetBrandStoreList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("publishPortalLogs count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { publishPortalLogs?.Count });

            PortalBrandListModel publishPortalLogList = new PortalBrandListModel { PortalBrandModel = publishPortalLogs };
            publishPortalLogList.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return publishPortalLogList;
        }

        //Get associate/unassociate products to brand.
        public virtual bool AssociateAndUnAssociatePortal(BrandPortalModel brandPortalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PortalId", brandPortalModel.PortalIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("brandId ", brandPortalModel.BrandId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("IsUnAssociated", brandPortalModel.IsUnAssociated ? 0 : 1, ParameterDirection.Input, DbType.Int32);

            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_AssociateBrandPortal @PortalId,@BrandId,@IsUnAssociated,@UserId, @Status OUT");
            ZnodeLogging.LogMessage("deleteResult count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { deleteResult?.Count });

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessUpdateBrand, brandPortalModel.BrandId), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorUpdateBrand, brandPortalModel.BrandId), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Check if brand already exists or not
        public virtual bool CheckBrandCode(string code)
         => _brandRepository.Table.Any(x => x.BrandCode == code);

        //Returns Associated /UnAssociated brands with portal.
        public virtual BrandListModel GetPortalBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Get and set Filter values in local variables.
            int portalId = GetFilterValue<int>(filters, FilterKeys.PortalId, true);
            bool isAssociated = GetFilterValue<bool>(filters, FilterKeys.IsAssociated, true);
            int localeId= GetFilterValue<int>(filters, FilterKeys.LocaleId, true);
            bool isWebstore= GetFilterValue<bool>(filters, FilterKeys.IsWebstore, true);
            SetFilterValue(filters, FilterKeys.IsActive.ToString().ToLower());         

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("Get Portal brand list ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId, pageListModel = pageListModel?.ToDebugString() });
            IZnodeViewRepository<BrandModel> objStoredProc = new ZnodeViewRepository<BrandModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", Convert.ToInt32(isAssociated), ParameterDirection.Input, DbType.Int32);

            IList<BrandModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetPortalBrandDetail @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PortalId,@IsAssociated", 4, out pageListModel.TotalRowCount);
           
            ZnodeLogging.LogMessage("Associated /UnAssociated Brand list count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { list?.Count });

            BrandListModel listModel = new BrandListModel { Brands = list?.ToList() };
            //Set seo Datails to brand list.
            SetSEODetailToBrands(listModel);

            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Get associate/unassociate products to brand.
        public virtual bool AssociateAndUnAssociatePortalBrands(PortalBrandAssociationModel portalBrandAssociationModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@PortalId", portalBrandAssociationModel.PortalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@BrandId", portalBrandAssociationModel.BrandIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", portalBrandAssociationModel.IsAssociated ? 1 : 0, ParameterDirection.Input, DbType.Int32);

            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_AssociatePortalBrand @PortalId,@BrandId,@IsAssociated,@UserId, @Status OUT");
            ZnodeLogging.LogMessage("Associated /UnAssociated Brand count:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { deleteResult?.Count });

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.SuccessUpdateBrand, portalBrandAssociationModel.BrandIds), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(string.Format(PIM_Resources.ErrorUpdateBrand, portalBrandAssociationModel.BrandIds), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Update the precedence value for associated price list for Store/Profile.
        public virtual bool UpdateAssociatedPortalBrandDetail(PortalBrandDetailModel portalBrandDetailModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(portalBrandDetailModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PricePortalModelNotNull);
            bool status = false;

            if (portalBrandDetailModel.BrandId > 0 && portalBrandDetailModel.PortalId > 0)
            {
                ZnodePortalBrand znodePortalBrand = _portalBrandRepository.Table.FirstOrDefault(x => x.BrandId == portalBrandDetailModel.BrandId && x.PortalId == portalBrandDetailModel.PortalId);
                
                if (znodePortalBrand?.DisplayOrder != portalBrandDetailModel.DisplayOrder)
                {
                    znodePortalBrand.DisplayOrder = portalBrandDetailModel?.DisplayOrder;
                    status = _portalBrandRepository.Update(znodePortalBrand);
                }
                else
                    status = true;                
            }
            ZnodeLogging.LogMessage(status ? "Associated Brand Details Updated Successfully" : "An error occurred while updating associated Brand", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return status;
        }

        #endregion

        #region private methods
        //Insert brand locale details.
        private ZnodeBrandDetailLocale InsertBrandDetailsLocale(BrandModel brandModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ZnodeBrandDetailLocale brandDetailsLocale = _brandLocaleRepository.Insert(new ZnodeBrandDetailLocale()
            {
                BrandId = brandModel.BrandId,
                Description = brandModel.Description,
                LocaleId = brandModel.LocaleId,
                BrandName = brandModel.BrandName
            });
            ZnodeLogging.LogMessage((IsNotNull(brandDetailsLocale) ? PIM_Resources.SuccessCreateBrandDetailLocale : PIM_Resources.ErrorCreateBrandDetailLocale), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return brandDetailsLocale;
        }

        //Insert SEO detail.
        private ZnodeCMSSEODetail InsertBrandSEODetails(BrandModel brandModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            ZnodeCMSSEOType cmsSEOType = GetCMSSEOType(ZnodeConstant.Brand);
            if (IsNotNull(cmsSEOType))
            {
                var seoDetail = new ZnodeCMSSEODetail
                {
                    CMSSEOTypeId = cmsSEOType.CMSSEOTypeId,
                    SEOId = brandModel.BrandId,
                    SEOUrl = brandModel.SEOFriendlyPageName,
                    SEOCode = brandModel.BrandCode,
                    PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT
                };

                ZnodeCMSSEODetail cmsSEODetail = _seoDetailRepository.Insert(seoDetail);
                ZnodeLogging.LogMessage((IsNotNull(cmsSEODetail) ? PIM_Resources.SuccessCreateBrandSEODetail : PIM_Resources.ErrorCreateBrandSEODetail), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

                return cmsSEODetail;
            }
            else
                return new ZnodeCMSSEODetail();
        }

        //Get CMSSEO Type details
        private ZnodeCMSSEOType GetCMSSEOType(string seoTypeName)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSSEOTypeEnum.Name.ToString(), FilterOperators.Is, seoTypeName));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            return _seoTypeRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
        }

        //Update SEO detail.
        private bool UpdateBrandSEODetails(BrandModel brandModel)
        {
            ZnodeLogging.LogMessage("BrandModel with BrandId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { brandModel?.BrandId });

            ZnodeCMSSEOType cmsSEOType = GetCMSSEOType(ZnodeConstant.Brand);

            if (IsNotNull(cmsSEOType))
                return _seoDetailRepository.Update(new ZnodeCMSSEODetail { CMSSEOTypeId = cmsSEOType.CMSSEOTypeId, SEOId = brandModel.BrandId, CMSSEODetailId = brandModel.CMSSEODetailId.GetValueOrDefault(), SEOUrl = brandModel.SEOFriendlyPageName, SEOCode = brandModel.BrandCode, PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT });
            else
                return false;
        }

        //Insert SEO detail locale.
        private ZnodeCMSSEODetailLocale InsertSEODetailLocale(BrandModel brandModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            ZnodeCMSSEODetailLocale cmsSEODetailLocale = _seoDetailLocaleRepository.Insert(
                 new ZnodeCMSSEODetailLocale
                 {
                     CMSSEODetailId = brandModel.CMSSEODetailId.GetValueOrDefault(),
                     SEODescription = brandModel.SEODescription,
                     SEOKeywords = brandModel.SEOKeywords,
                     SEOTitle = brandModel.SEOTitle,
                     LocaleId = brandModel.LocaleId
                 });
            brandModel.CMSSEODetailLocaleId = cmsSEODetailLocale?.CMSSEODetailLocaleId ?? 0;
            ZnodeLogging.LogMessage((IsNotNull(cmsSEODetailLocale) ? PIM_Resources.SuccessCreateBrandSEODetailLocale : PIM_Resources.ErrorCreateBrandSEODetailLocale), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return cmsSEODetailLocale;
        }

        //Update SEO detail locale.
        private bool UpdateSEODetailLocale(BrandModel brandModel)
        {
            ZnodeCMSSEODetailLocale cmsSEODetailslocale = GetSEODetailLocale(brandModel);
            if (IsNotNull(cmsSEODetailslocale))
            {

                bool isBrandSEOLocale = _seoDetailLocaleRepository.Update(
                     new ZnodeCMSSEODetailLocale
                     {
                         CMSSEODetailId = brandModel.CMSSEODetailId.GetValueOrDefault(),
                         CMSSEODetailLocaleId = cmsSEODetailslocale.CMSSEODetailLocaleId,
                         SEODescription = brandModel.SEODescription,
                         SEOKeywords = brandModel.SEOKeywords,
                         SEOTitle = brandModel.SEOTitle,
                         LocaleId = brandModel.LocaleId
                     });
                ZnodeLogging.LogMessage(isBrandSEOLocale ? PIM_Resources.SuccessCreateBrandSEODetailLocale : PIM_Resources.ErrorCreateBrandSEODetailLocale, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return isBrandSEOLocale;
            }
            else
                return IsNotNull(InsertSEODetailLocale(brandModel));
        }

        //Update Brand locale details.
        private bool UpdateBrandDetailLocale(BrandModel brandModel)
        {
            ZnodeBrandDetailLocale brandDetailslocale = GetBrandDetailLocale(brandModel);
            if (IsNotNull(brandDetailslocale))
            {
                bool isBrandUpdated = _brandLocaleRepository.Update(
                  new ZnodeBrandDetailLocale
                  {
                      BrandId = brandModel.BrandId,
                      BrandDetailLocaleId = brandDetailslocale.BrandDetailLocaleId,
                      Description = brandModel.Description,
                      BrandName = brandModel.BrandName,
                      LocaleId = brandModel.LocaleId,
                      SEOFriendlyPageName = brandModel.SEOFriendlyPageName
                  });
                ZnodeLogging.LogMessage((isBrandUpdated ? PIM_Resources.SuccessUpdateBrandLocaleDetail : PIM_Resources.ErrorUpdateBrandLocaleDetail), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return isBrandUpdated;
            }
            else
            {

                ZnodeBrandDetailLocale brandDetailLocale = _brandLocaleRepository.Insert(new ZnodeBrandDetailLocale()
                {
                    BrandId = brandModel.BrandId,
                    Description = brandModel.Description,
                    LocaleId = brandModel.LocaleId,
                    SEOFriendlyPageName = brandModel.SEOFriendlyPageName
                });
                ZnodeLogging.LogMessage((IsNotNull(brandDetailLocale) ? PIM_Resources.SuccessInsertBrandLocaleDetail : PIM_Resources.ErrorInsertBrandLocaleDetail), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return IsNotNull(brandDetailLocale);
            }
        }

        //Get seo detail locale. 
        private ZnodeCMSSEODetailLocale GetSEODetailLocale(BrandModel brandModel)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeCMSSEODetailLocaleEnum.CMSSEODetailId.ToString(), FilterOperators.Equals, brandModel.CMSSEODetailId.ToString()));
            filter.Add(new FilterTuple(ZnodeCMSSEODetailLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, brandModel.LocaleId.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause for GetEntity:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClauseModel?.WhereClause });
            ZnodeCMSSEODetailLocale cmsSEODetailslocale = _seoDetailLocaleRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
            return cmsSEODetailslocale;
        }

        //Get brand details locale.
        private ZnodeBrandDetailLocale GetBrandDetailLocale(BrandModel brandModel)
        {
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeBrandDetailLocaleEnum.BrandId.ToString(), FilterOperators.Equals, brandModel.BrandId.ToString()));
            filter.Add(new FilterTuple(ZnodeCMSSEODetailLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, brandModel.LocaleId.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause for GetEntity:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { whereClauseModel?.WhereClause });
            ZnodeBrandDetailLocale brandDetailslocale = _brandLocaleRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

            return brandDetailslocale;
        }

        //Set seo Datails to brand list.
        private void SetSEODetailToBrands(BrandListModel listModel)
        {
      if (PortalId > 0)
            {
                IImageHelper image = GetService<IImageHelper>();
                //Get image path forbrands.
                listModel?.Brands?.ForEach(
                    x =>
                    {
                        x.ImageSmallPath = image.GetImageHttpPathSmall(x.ImageName);
                        x.ImageMediumPath = image.GetImageHttpPathMedium(x.ImageName);
                        x.ImageThumbNailPath = image.GetImageHttpPathThumbnail(x.ImageName);
                        x.ImageSmallThumbnailPath = image.GetImageHttpPathSmallThumbnail(x.ImageName);
                    });
            }
        }

        //Gets Filters For SEO
        protected virtual FilterCollection GetFiltersForSEO( int localeId, string seoTypeName)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple("SEOTypeName", FilterOperators.Is, seoTypeName));
            filters.Add(new FilterTuple("LocaleId", FilterOperators.Equals, localeId.ToString()));
            filters.Add(new FilterTuple("VersionId", FilterOperators.Equals, GetCatalogVersionId().ToString()));
            return filters;
        }
        
        //Get brand image path
        private void GetBrandImagePath(int portalId, BrandModel brand)
        {
            ZnodeLogging.LogMessage("Input parameters portalId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { portalId });

            //Get brand image path
            if (portalId > 0)
            {
                IImageHelper image = GetService<IImageHelper>();
                brand.ImageLargePath = image.GetImageHttpPathLarge(brand.ImageName);
                brand.ImageMediumPath = image.GetImageHttpPathMedium(brand.ImageName);
                brand.ImageThumbNailPath = image.GetImageHttpPathThumbnail(brand.ImageName);
                brand.ImageSmallPath = image.GetImageHttpPathSmall(brand.ImageName);
                brand.OriginalImagepath = image.GetOriginalImagepath(brand.ImageName);
            }
        }

        //Get IsAssociated value from filter collection.
        private T GetFilterValue<T>(FilterCollection filters, string filerName, bool isRemove = false)
        {
            T result = default(T);
            if (filters.Count > 0 && (filters.Find(x => x.Item1.ToLower() == filerName.ToLower())?.Item1.ToLower() == filerName.ToString().ToLower()))
            {
                try
                {
                    result = (T)Convert.ChangeType(filters.Find(x => x.Item1.ToLower() == filerName.ToLower())?.Item3, typeof(T));
                }
                catch (NotSupportedException)
                {
                    ZnodeLogging.LogMessage("Type not supported :", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { filters, filerName = filerName });
                    return default(T);
                }
                if (isRemove)
                {
                    filters.RemoveAll(x => x.FilterName.ToLower() == filerName.ToLower());
                }
            }
            return result;
        }


        //This funcation use for set filter value.
        private void SetFilterValue(FilterCollection filters, string filterName)
        {
            filters = IsNull(filters) ? new FilterCollection() : filters;
            if (IsNotNull(filters.FirstOrDefault(x => x.FilterName.Equals(filterName.ToString().ToLower()))))
            {
                FilterTuple _filterAttributeType = filters.FirstOrDefault(x => x.FilterName.Equals(filterName));
                filters.RemoveAll(x => x.FilterName.Equals(filterName));
                if (IsNotNull(_filterAttributeType))
                {
                    if (_filterAttributeType.FilterValue.Equals(ZnodeConstant.TrueValue, StringComparison.OrdinalIgnoreCase))
                    { 
                        filters.Add(filterName, ProcedureFilterOperators.Equals, "1");
                    }
                    else if (_filterAttributeType.FilterValue.Equals(ZnodeConstant.FalseValue, StringComparison.OrdinalIgnoreCase))
                    { 
                        filters.Add(filterName, ProcedureFilterOperators.Equals, "0");
                    }
                }
            }

        }
        #endregion
    }
}