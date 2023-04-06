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
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using Znode.Libraries.Observer;

namespace Znode.Engine.Services
{
    public class PriceService : BaseService, IPriceService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePriceList> _priceListRepository;
        private readonly IZnodeRepository<ZnodePrice> _priceRepository;
        private readonly IZnodeRepository<ZnodePriceTier> _tierPriceRepository;
        private readonly IZnodeRepository<ZnodePriceListPortal> _priceListPortalRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodePriceListProfile> _priceListProfileRepository;
        private readonly IZnodeRepository<ZnodeProfile> _profileRepository;
        private readonly IZnodeRepository<ZnodePriceListUser> _priceListUserRepository;
        private readonly IZnodeRepository<ZnodePriceListAccount> _priceListAccountRepository;
        private readonly IZnodeRepository<ZnodePortalProfile> _portalProfileRepository;
        private readonly IZnodeRepository<ZnodeUom> _uomRepository;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;
        private readonly IZnodeRepository<ZnodePortalUnit> _portalUnitRepository;
        private readonly IZnodeRepository<ZnodeCurrency> _currencyRepository;
        private readonly IZnodeRepository<ZnodeCulture> _cultureRepository;
        private readonly ProductAssociationHelper productAssociationHelper;
        #endregion

        #region Constructor
        public PriceService()
        {
            _priceListRepository = new ZnodeRepository<ZnodePriceList>();
            _priceRepository = new ZnodeRepository<ZnodePrice>();
            _priceListPortalRepository = new ZnodeRepository<ZnodePriceListPortal>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _tierPriceRepository = new ZnodeRepository<ZnodePriceTier>();
            _priceListProfileRepository = new ZnodeRepository<ZnodePriceListProfile>();
            _profileRepository = new ZnodeRepository<ZnodeProfile>();
            _priceListUserRepository = new ZnodeRepository<ZnodePriceListUser>();
            _portalProfileRepository = new ZnodeRepository<ZnodePortalProfile>();
            _uomRepository = new ZnodeRepository<ZnodeUom>();
            _priceListAccountRepository = new ZnodeRepository<ZnodePriceListAccount>();
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _portalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();
            _currencyRepository = new ZnodeRepository<ZnodeCurrency>();
            _cultureRepository = new ZnodeRepository<ZnodeCulture>();
            productAssociationHelper = new ProductAssociationHelper();
        }
        #endregion

        #region Public Methods

        #region Price
        //Create Price.
        public virtual PriceModel CreatePrice(PriceModel priceModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(priceModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            ZnodeLogging.LogMessage("Existing price code:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, priceModel?.ListCode);

            if (IsCodeAlreadyExist(priceModel.ListCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.PriceListAlreadyExist);
            if (priceModel.ImportPriceList?.Count > 0)
            {
                string importPriceXML = ToXML(priceModel.ImportPriceList);
                IZnodeViewRepository<ImportPriceModel> objStoredProc = new ZnodeViewRepository<ImportPriceModel>();
                objStoredProc.SetParameter("PriceXML", importPriceXML, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                List<ImportPriceModel> importPriceErrorList = objStoredProc.ExecuteStoredProcedureList("Znode_ImportPriceList @PriceXML,@Status OUT, @UserId ", 1, out status).ToList();
                ZnodeLogging.LogMessage("Import price list count:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, importPriceErrorList?.Count());

                priceModel.ImportPriceList = importPriceErrorList?.FirstOrDefault()?.RowNumber > 0 ? importPriceErrorList : null;
                if (IsNull(priceModel.ImportPriceList))
                    //Get PriceListId by ListCode from ZnodePriceList Table.
                    priceModel.PriceListId = _priceListRepository.Table.FirstOrDefault(x => x.ListCode == priceModel.ListCode).PriceListId;
            }
            else
            {
                //Create new Price list and return it.
                ZnodePriceList price = _priceListRepository.Insert(priceModel.ToEntity<ZnodePriceList>());

                ZnodeLogging.LogMessage((price?.PriceListId > 0) ? Admin_Resources.ErrorPriceListInsert : Admin_Resources.SuccessPriceListInsert, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                if (IsNotNull(price))
                    return price.ToModel<PriceModel>();
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return priceModel;
        }

        //Get Price by Price list id.
        public virtual PriceModel GetPrice(int priceListId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters priceListId :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, priceListId);

            if (priceListId > 0)
            {
                ZnodePriceList pricing = _priceListRepository.Table.Where(x => x.PriceListId == priceListId)?.FirstOrDefault();
                return IsNotNull(pricing) ? pricing.ToModel<PriceModel>() : null;
            }
            return null;
        }

        //Update Price.
        public virtual bool UpdatePrice(PriceModel priceModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(priceModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            ZnodeLogging.LogMessage("Get price list while updating with id :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, priceModel?.PriceListId);

            if (priceModel.PriceListId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            bool isPriceUpdated = false;

            ZnodeLogging.LogMessage("Import Price List count :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, priceModel.ImportPriceList?.Count);

            if (priceModel.ImportPriceList?.Count > 0)
            {
                string importPriceXML = ToXML(priceModel.ImportPriceList);
                IZnodeViewRepository<ImportPriceModel> objStoredProc = new ZnodeViewRepository<ImportPriceModel>();
                objStoredProc.SetParameter("PriceXML", importPriceXML, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                List<ImportPriceModel> importPriceErrorList = objStoredProc.ExecuteStoredProcedureList("Znode_ImportPriceList @PriceXML,@Status OUT, @UserId ", 1, out status).ToList();
                priceModel.ImportPriceList = importPriceErrorList?.FirstOrDefault()?.RowNumber > 0 ? importPriceErrorList : null;
                isPriceUpdated = true;
                ZnodeLogging.LogMessage("importPriceErrorList count :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, importPriceErrorList?.Count);
            }

            else
                //Update Pricing
                isPriceUpdated = _priceListRepository.Update(priceModel.ToEntity<ZnodePriceList>());

            if (isPriceUpdated)
            {
                //If price list is updated with new currency delete its association from Portal/Profile having old currency. 
                RemoveAssociationOnCurrencyUpdate(priceModel);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessPriceUpdate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            }
            else
                ZnodeLogging.LogMessage(Admin_Resources.SuccessPriceUpdate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return isPriceUpdated;
        }

        //Get list of Price.
        public virtual PriceListModel GetPriceList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            string mode = string.Empty;
            PriceListModel priceListModel = new PriceListModel();
            if (filters.Count > 0)
            {
                if (filters.Any(x => x.FilterName == ZnodeConstant.Mode))
                    mode = filters.FirstOrDefault(x => x.FilterName == ZnodeConstant.Mode)?.FilterValue;

                //This method sets filter required to get price list.
                SetFilters(filters, priceListModel);
            }

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });
            IZnodeViewRepository<PriceModel> objStoredProc = new ZnodeViewRepository<PriceModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@Mode", mode, ParameterDirection.Input, DbType.String);

            IList<PriceModel> priceList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPriceList  @WhereClause,@Rows,@PageNo,@Order_By,@RowsCount OUT,@Mode", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("priceList count :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, priceList?.Count);

            priceListModel.PriceList = priceList?.ToList();

            //Check if account is parent account or not, if mode is account and list count is 0.
            if (string.Equals(mode, ZnodeConstant.Account, StringComparison.CurrentCultureIgnoreCase) && priceListModel?.PriceList?.Count <= 0)
                IsParentAccount(priceListModel, filters);

            priceListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceListModel;
        }

        //Delete Price.
        public virtual bool DeletePrice(ParameterModel priceListIds)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Price Id send for delete purpose:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new object[] { priceListIds?.Ids });
            if (IsNull(priceListIds) || string.IsNullOrEmpty(priceListIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceListIdNotLessThanOne);
            int status = 0;

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePriceListEnum.PriceListId.ToString(), priceListIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.ExecuteStoredProcedureList("Znode_DeletePriceList @PriceListId,  @Status OUT", 1, out status);
            if (status == 1)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessPriceListDelete, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError,Admin_Resources.PriceListAssociationDeleteErrorMessage);
            }
        }

        //Copy the existing price.
        public virtual bool CopyPrice(PriceModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsCodeAlreadyExist(model.ListCode))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.PriceListCodeAlreadyExist);

            ZnodeLogging.LogMessage("PriceModel with PriceListId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new object[] { model?.PriceListId });

            //Assign default date to activation date and expiration date.
            model.ActivationDate = !HelperUtility.IsNull(model.ActivationDate) ? model.ActivationDate : new DateTime(1753, 01, 01);
            model.ExpirationDate = !HelperUtility.IsNull(model.ExpirationDate) ? model.ExpirationDate : new DateTime(1753, 01, 01);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePriceEnum.PriceListId.ToString(), model.PriceListId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodePriceListEnum.ListCode.ToString(), model.ListCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePriceListEnum.ListName.ToString(), model.ListName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodePriceEnum.ActivationDate.ToString(), model.ActivationDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter(ZnodePriceEnum.ExpirationDate.ToString(), model.ExpirationDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter(ZnodePriceListEnum.CurrencyId.ToString(), model.CurrencyId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodePriceListEnum.CultureId.ToString(), model.CultureId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_CopyPricingList @PriceListId, @ListCode, @ListName, @ActivationDate, @ExpirationDate, @CurrencyId, @CultureId, @Status OUT", 7, out status);
            if (status == 1)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessPriceCopy, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorPriceCopy, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return false;
            }
        }
        #endregion

        #region SKU Price
        //Method to get sku price list.
        public virtual PriceSKUListModel GetSKUPriceList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IZnodeViewRepository<PriceSKUModel> objStoredProc = new ZnodeViewRepository<PriceSKUModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);

            //Gets the entity list according to where clause, order by clause and pagination
            IList<PriceSKUModel> associatedSKUs = objStoredProc.ExecuteStoredProcedureList("Znode_GetPriceSKUDetail  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("associatedSKUs count :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new object[] { associatedSKUs?.Count });

            PriceSKUListModel priceSKUListModel = new PriceSKUListModel { PriceSKUList = associatedSKUs?.ToList() };

            priceSKUListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceSKUListModel;
        }

        //Method to get sku price on the basis of price id.
        public virtual PriceSKUModel GetSKUPrice(int priceId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (priceId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorPriceIdNullOrEmpty);

            ZnodeLogging.LogMessage("Input parameter :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { priceId = priceId });

            PriceSKUModel priceSKUModel = _priceRepository.Table.Where(x => x.PriceId == priceId)?.FirstOrDefault()?.ToModel<PriceSKUModel>();

            //Get listName on the basis of priceListId.
            priceSKUModel.ListName = _priceListRepository.Table.Where(x => x.PriceListId == priceSKUModel.PriceListId)?.FirstOrDefault()?.ListName;

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return priceSKUModel;
        }

        //Method to create sku price.
        public virtual PriceSKUModel AddSKUPrice(PriceSKUModel priceSKUModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(priceSKUModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PriceSkuModelNotNull);

            if (IsSKUExist(priceSKUModel.SKU, priceSKUModel.PriceListId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.SkuPriceListAlreadyExist);

            string productSKU = GetSKUList(priceSKUModel.ProductId.ToString()).FirstOrDefault()?.AttributeValue;
            int pimProductId = Convert.ToInt32(priceSKUModel?.ProductId);
            priceSKUModel.SKU = IsNotNull(productSKU) ? productSKU : priceSKUModel.SKU;
            priceSKUModel = _priceRepository.Insert(priceSKUModel?.ToEntity<ZnodePrice>())?.ToModel<PriceSKUModel>();
            ZnodeLogging.LogMessage("SKU from priceSKUModel :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { SKU = priceSKUModel?.SKU });
            priceSKUModel.ProductId = pimProductId;
            ZnodeLogging.LogMessage(IsNotNull(priceSKUModel) ? Admin_Resources.SuccessSkuPriceListAdd : Admin_Resources.ErrorSkuPriceListAdd, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            productAssociationHelper.SaveProductAsDraft(pimProductId);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return priceSKUModel;
        }

        //Method to update sku price.
        public virtual bool UpdateSKUPrice(PriceSKUModel priceSKUModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            bool status = false;
            if (IsNull(priceSKUModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (priceSKUModel?.PriceId > 0)
                status = _priceRepository.Update(priceSKUModel.ToEntity<ZnodePrice>());

            //Clear webStore Cache on success update.
            if (status)
            {
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(priceSKUModel?.ProductId));
                priceSKUModel.PortalId = PortalId;
                var clearCacheInitializer = new ZnodeEventNotifier<PriceSKUModel>(priceSKUModel);
            }

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessSkuPriceUpdate : Admin_Resources.ErrorSkuPriceUpdate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return status;
        }

        //Delete multiple Price SKU along with all Tier Prices associated in it.
        public virtual bool DeleteSKUPrice(SKUPriceDeleteModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(model.PriceId) || string.IsNullOrEmpty(model.PriceListId.ToString()))
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.PriceListIdNotLessThanOne);

            ZnodeLogging.LogMessage("PriceId and PriceListId :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceId = model?.PriceId, PriceListId = model?.PriceListId.ToString() });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePriceEnum.PriceListId.ToString(), model.PriceListId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("PriceIds", model.PriceId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_DeletePriceSKU @PriceIds,@PriceListId, @Status OUT", 2, out status);
            if (status == 1)
            {
                productAssociationHelper.SaveProductAsDraft(model.PimProductId);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessPriceSkuDelete, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorPriceSkuDelete);
            }
        }

        //Method to get price on the basis of sku.
        public virtual PriceSKUModel GetPriceBySku(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            int pimProductId = Convert.ToInt32(filters?.Where(x => x.FilterName == FilterKeys.PimProductId)?.Select(x => x.FilterValue)?.FirstOrDefault());
            string sku = filters?.Where(x => x.FilterName == FilterKeys.Sku)?.Select(x => x.FilterValue)?.FirstOrDefault() ?? string.Empty;
            string productType = filters?.Where(x => x.FilterName == FilterKeys.ProductType)?.Select(x => x.FilterValue)?.FirstOrDefault() ?? string.Empty;

            IZnodeViewRepository<PriceSKUModel> objStoredProc = new ZnodeViewRepository<PriceSKUModel>();
            objStoredProc.SetParameter("@PortalId", PortalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@SKU", sku, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PimProductId", pimProductId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@ProductType", productType, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);

            PriceSKUModel priceSKUModel = objStoredProc.ExecuteStoredProcedureList("Znode_GetPriceFromAssociateProducts @PortalId,@SKU,@PimProductId,@UserId,@ProductType,@LocaleId")?.FirstOrDefault();

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return priceSKUModel;
        }

        //Method to get price with details the basis of sku.
        public virtual PriceSKUModel GetProductPricingDetailsBySku(ProductPriceListSKUDataModel productPriceListSKUDataModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            int priceListId = productPriceListSKUDataModel.ProductpriceListId;
            PriceSKUModel priceSKUModel = _priceRepository.Table.FirstOrDefault(x=> (x.PriceListId == priceListId || priceListId == 0) && x.SKU == productPriceListSKUDataModel.Sku)?.ToModel<PriceSKUModel>();

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceSKUModel;
        }

        //Get Unit of Measurement List.
        public virtual UomListModel GetUomList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { pageListModel = pageListModel?.ToDebugString()});

            //Get unit of measurement list.
            IList<ZnodeUom> entity = _uomRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);

            UomListModel uomListModel = new UomListModel { UomList = entity?.ToModel<UomModel>().ToList() };

            uomListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return uomListModel;
        }

        //Method to get paged sku and its prices.
        public virtual PriceSKUListModel GetPagedPriceSKU(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, DataTable productIdTable, string isInStock = "-1")
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            int portalId = Convert.ToInt32(filters?.Where(x => x.FilterName == FilterKeys.PortalId)?.Select(x => x.FilterValue)?.FirstOrDefault());
            filters?.RemoveAll(x => x.FilterName == FilterKeys.PortalId);

            string skus = filters?.Where(x => x.FilterName == FilterKeys.SKU)?.Select(x => x.FilterValue)?.FirstOrDefault() ?? string.Empty;
            filters?.RemoveAll(x => x.FilterName == FilterKeys.SKU);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<PriceSKUModel> objStoredProc = new ZnodeViewRepository<PriceSKUModel>();

            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Sku", skus, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@currentUtcDate", HelperUtility.GetDateTime().Date, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter("@IsInStock", isInStock, ParameterDirection.Input, DbType.String);
            IList<PriceSKUModel> associatedSKUs = null;
            if (DefaultGlobalConfigSettingHelper.IsColumnEncryptionSettingEnabled)
            {
                objStoredProc.SetParameter("@PublishProductId", productIdTable?.ToJson(), ParameterDirection.Input, DbType.String);
                //Gets the entity list according to where clause, order by clause and pagination
                associatedSKUs = objStoredProc.ExecuteStoredProcedureList("Znode_GetSkuPricebyCatalogWithJSON @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@Sku,@PortalId,@currentUtcDate,@PublishProductId,@IsInStock", 4, out pageListModel.TotalRowCount);
            }
            else
            {
                objStoredProc.SetTableValueParameter("@PublishProductId", productIdTable, ParameterDirection.Input, SqlDbType.Structured, "dbo.ProductForSortPrice");
                //Gets the entity list according to where clause, order by clause and pagination
                associatedSKUs = objStoredProc.ExecuteStoredProcedureList("Znode_GetSkuPricebyCatalog @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@Sku,@PortalId,@currentUtcDate,@PublishProductId,@IsInStock", 4, out pageListModel.TotalRowCount);
            }
            PriceSKUListModel priceSKUListModel = new PriceSKUListModel { PriceSKUList = associatedSKUs?.ToList() };

            priceSKUListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceSKUListModel;
        }
        #endregion

        #region Associate Store
        //Method to get associated store list.
        public virtual PricePortalListModel GetAssociatedStoreList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            filters.Add(new FilterTuple("IsAssociated ", ProcedureFilterOperators.Equals, "1"));

            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<View_GetAssociatedPortalToPriceList> objStoredProc = new ZnodeViewRepository<View_GetAssociatedPortalToPriceList>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<View_GetAssociatedPortalToPriceList> associatedStoreEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetAssociatedPortalToPriceList  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("associatedStoreEntity count:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, associatedStoreEntity?.Count);

            PricePortalListModel pricePortalListModel = new PricePortalListModel { PricePortalList = associatedStoreEntity?.ToModel<PricePortalModel>().ToList() };
            pricePortalListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return pricePortalListModel;
        }

        //Get list of unassociated store list.
        public virtual PortalListModel GetUnAssociatedStoreList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            filters.Add(new FilterTuple("IsAssociated ", ProcedureFilterOperators.Equals, "0"));

            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<View_GetAssociatedPortalToPriceList> objStoredProc = new ZnodeViewRepository<View_GetAssociatedPortalToPriceList>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<View_GetAssociatedPortalToPriceList> unAssociatedStoreEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetAssociatedPortalToPriceList  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("unAssociatedStoreEntity count:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, unAssociatedStoreEntity?.Count);

            PortalListModel listModel = new PortalListModel() { PortalList = unAssociatedStoreEntity?.ToModel<PortalModel>().ToList() };
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Method to associate store.
        public virtual bool AssociateStore(PricePortalListModel pricePortalModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PricePortalList count:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, pricePortalModel?.PricePortalList.Count);

            if (pricePortalModel?.PricePortalList.Count < 1)
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.PricePortalModelNotLessThanZero);

            IEnumerable<ZnodePriceListPortal> associateStore = _priceListPortalRepository.Insert(pricePortalModel?.PricePortalList.ToEntity<ZnodePriceListPortal>()?.ToList());

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return associateStore?.Count() > 0;
        }

        //Method to remove associate stores.
        public virtual bool RemoveAssociatedStores(RemoveAssociatedStoresModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(model) || string.IsNullOrEmpty(model.PriceListPortalIds) || model.PriceListId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ModelInvalid);

            //Generate where clause.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListEnum.PriceListId.ToString(), ProcedureFilterOperators.Equals, model.PriceListId.ToString()));
            filters.Add(new FilterTuple(ZnodePriceListPortalEnum.PriceListPortalId.ToString(), ProcedureFilterOperators.In, model.PriceListPortalIds));

            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            //Delete mapping of store against price.
            bool isDeleted = _priceListPortalRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessStoreUnassociate : Admin_Resources.ErrorStoreUnassociate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Get Associated Store with Precedence data.
        public virtual PricePortalModel GetAssociatedStorePrecedence(int priceListPortalId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (priceListPortalId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceListPortalIdNotLessThanOne);

            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose,new { priceListPortalId = priceListPortalId });

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListPortalEnum.PriceListPortalId.ToString(), ProcedureFilterOperators.Equals, priceListPortalId.ToString()));

            //Gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GenerateFiltersForAssociatedPriceList(filters).ToFilterDataCollection());

            //Get expands.
            List<string> navigationProperties = GetExpands(expands);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return _priceListPortalRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties)?.ToModel<PricePortalModel>();
        }

        //Update the precedence value for associated stores.
        public virtual bool UpdateAssociatedStorePrecedence(PricePortalModel pricePortalModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            bool status = false;
            if (IsNull(pricePortalModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PricePortalModelNotNull);

            if (pricePortalModel?.PriceListPortalId > 0)
                status = _priceListPortalRepository.Update(pricePortalModel.ToEntity<ZnodePriceListPortal>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessPrecedenceValueUpdate : Admin_Resources.ErrorPrecedenceValueUpdateToStore, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return status;
        }

        #endregion

        #region Associate Profile
        //Method to get associated profile list.
        public virtual PriceProfileListModel GetAssociatedProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            filters.Add(new FilterTuple("IsAssociated ", ProcedureFilterOperators.Equals, "1"));

            //Set Authorized Portal filter based on user portal access.
            BindUserPortalFilter(ref filters);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<PriceProfileModel> objStoredProc = new ZnodeViewRepository<PriceProfileModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<PriceProfileModel> associatedProfileEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetAssociatedProfileToPriceList  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("associatedProfileEntity count:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, associatedProfileEntity?.Count);

            PriceProfileListModel priceProfileListModel = new PriceProfileListModel { PriceProfileList = associatedProfileEntity?.ToList() };
            priceProfileListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceProfileListModel;
        }

        //Get list of unassociated Profile list.
        public virtual ProfileListModel GetUnAssociatedProfileList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            filters.Add(new FilterTuple("IsAssociated ", ProcedureFilterOperators.Equals, "0"));

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<View_GetAssociatedProfileToPriceList> objStoredProc = new ZnodeViewRepository<View_GetAssociatedProfileToPriceList>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<View_GetAssociatedProfileToPriceList> unAssociatedProfileEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetAssociatedProfileToPriceList  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);

            ProfileListModel listModel = new ProfileListModel() { Profiles = unAssociatedProfileEntity?.ToModel<ProfileModel>().ToList() };
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Method to associate profile.
        public virtual bool AssociateProfile(PriceProfileListModel priceProfileModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PriceProfileList count:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, priceProfileModel?.PriceProfileList?.Count);
            if (priceProfileModel?.PriceProfileList.Count < 1)
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PricePortalModelCountNotNull);

            IEnumerable<ZnodePriceListProfile> associateProfile = _priceListProfileRepository.Insert(priceProfileModel?.PriceProfileList.ToEntity<ZnodePriceListProfile>()?.ToList());

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return associateProfile?.Count() > 0;
        }

        //Method to remove associated profile.
        public virtual bool RemoveAssociatedProfiles(RemoveAssociatedProfilesModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(model) || string.IsNullOrEmpty(model.PriceListProfileIds) || model.PriceListId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelInvalid);

            ZnodeLogging.LogMessage("PriceListProfileIds to be removed:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, model?.PriceListProfileIds);

            //Generate where clause.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListEnum.PriceListId.ToString(), ProcedureFilterOperators.Equals, model.PriceListId.ToString()));
            filters.Add(new FilterTuple(ZnodePriceListProfileEnum.PriceListProfileId.ToString(), ProcedureFilterOperators.In, model.PriceListProfileIds));

            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            //Delete mapping of profile against price.
            bool isDeleted = _priceListProfileRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessProfileToPriceAssociate : Admin_Resources.ErrorProfileToPriceAssociate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Get Associated Profile with Precedence data.
        public virtual PriceProfileModel GetAssociatedProfilePrecedence(int priceListProfileId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { priceListProfileId = priceListProfileId });

            if (priceListProfileId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceListProfileIdNotLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListProfileEnum.PriceListProfileId.ToString(), ProcedureFilterOperators.Equals, priceListProfileId.ToString()));

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GenerateFiltersForAssociatedPriceList(filters).ToFilterDataCollection());

            //get expands for ZnodeProfile.
            List<string> navigationProperties = GetExpands(expands);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return _priceListProfileRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties)?.ToModel<PriceProfileModel>();
        }

        //Update the precedence value for associated profile.
        public virtual bool UpdateAssociatedProfilePrecedence(PriceProfileModel priceProfileModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            bool status = false;
            if (IsNull(priceProfileModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (priceProfileModel?.PriceListProfileId > 0)
                status = _priceListProfileRepository.Update(priceProfileModel.ToEntity<ZnodePriceListProfile>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessPrecedenceValueUpdateToProfile : Admin_Resources.ErrorPrecedenceValueUpdateToProfile, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return status;
        }

        #endregion

        #region Tier Price
        //Method to get tier price list.
        public virtual PriceTierListModel GetTierPriceList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //Get tier price list.
            IList<ZnodePriceTier> tierPriceList = _tierPriceRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);

            PriceTierListModel priceTierListModel = new PriceTierListModel { TierPriceList = tierPriceList?.ToModel<PriceTierModel>().ToList() };

            priceTierListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceTierListModel;
        }

        //Method to get tier price on the basis of price tier id.
        public virtual PriceTierModel GetTierPrice(int priceTierId)
          => priceTierId > 0 ? _tierPriceRepository.GetById(priceTierId)?.ToModel<PriceTierModel>() : new PriceTierModel();

        //Save tier price values.
        public virtual bool AddTierPrice(PriceTierListModel priceTierListModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(priceTierListModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@PriceListId", priceTierListModel.TierPriceList.FirstOrDefault().PriceListId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@SKU", priceTierListModel.TierPriceList.FirstOrDefault().SKU, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@PriceTierId", string.Join(",", priceTierListModel.TierPriceList.Select(x => x.PriceTierId).ToList()), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Price", string.Join(",", priceTierListModel.TierPriceList.Select(x => x.Price).ToList()), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Quantity", string.Join(",", priceTierListModel.TierPriceList.Select(x => x.Quantity).ToList()), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Custom1", string.Join(",", priceTierListModel.TierPriceList.Select(x => x.Custom1).ToList()), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Custom2", string.Join(",", priceTierListModel.TierPriceList.Select(x => x.Custom2).ToList()), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Custom3", string.Join(",", priceTierListModel.TierPriceList.Select(x => x.Custom3).ToList()), ParameterDirection.Input, DbType.String);


            View_ReturnBoolean output = objStoredProc.ExecuteStoredProcedureList("Znode_InsertProductTierPrice @SKU,@PriceListId,@PriceTierId,@Price,@Quantity,@UserId,Null,Null,@Custom1,@Custom2,@Custom3")?.FirstOrDefault();
            if (output?.Status == false)
                throw new ZnodeException(ErrorCodes.DuplicateQuantityError, Admin_Resources.DuplicateTierPriceErrorMessage);
            else
                return true;
        }

        //Method to update tier price.
        public virtual bool UpdateTierPrice(PriceTierModel priceTierModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            bool status = false;
            if (IsNull(priceTierModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PriceTierModelNotNull);

            ZnodeLogging.LogMessage("PriceTierId :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceTierId = priceTierModel?.PriceTierId });

            if (priceTierModel?.PriceTierId > 0)
                status = _tierPriceRepository.Update(priceTierModel.ToEntity<ZnodePriceTier>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessTierPriceUpdate : Admin_Resources.ErrorTierPriceUpdate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return status;
        }

        //Method to delete tier price.
        public virtual bool DeleteTierPrice(int priceTierId)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { priceTierId = priceTierId });

            if (priceTierId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PriceTierIdNotLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceTierEnum.PriceTierId.ToString(), ProcedureFilterOperators.Equals, priceTierId.ToString()));

            return _tierPriceRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }
        #endregion

        #region Associate Customer
        //Get Associated Customer List.
        public virtual PriceUserListModel GetAssociatedCustomerList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            //Filters for getting associated list.
            filters.Add(new FilterTuple(FilterKeys.IsAssociated, ProcedureFilterOperators.Equals, "1"));

            if (!DefaultGlobalConfigSettingHelper.AllowGlobalLevelUserCreation)
                //Set Authorized Portal filter based on user portal access.
                BindUserPortalFilter(ref filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            expands.Add(ZnodePriceListUserEnum.ZnodeUser.ToString(), ZnodePriceListUserEnum.ZnodeUser.ToString());

            IZnodeViewRepository<PriceUserModel> objStoredProc = new ZnodeViewRepository<PriceUserModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@AccountList", "0", ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<PriceUserModel> associatedCustomerEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetPriceUserDetails @WhereClause,@AccountList,@Rows,@PageNo,@Order_By,@RowCount OUT", 5, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("associatedCustomerEntity count :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { associatedCustomerEntityCount = associatedCustomerEntity?.Count });

            PriceUserListModel priceAccountListModel = new PriceUserListModel { PriceUserList = associatedCustomerEntity?.ToList() };
            priceAccountListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceAccountListModel;
        }

        //Get UnAssociated Customer List.
        public virtual PriceUserListModel GetUnAssociatedCustomerList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            //Filters for getting unassociated list.
            filters.Add(new FilterTuple(FilterKeys.IsAssociated, ProcedureFilterOperators.Equals, "0"));

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<PriceUserModel> objStoredProc = new ZnodeViewRepository<PriceUserModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@AccountList", "0", ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<PriceUserModel> unassociatedCustomerEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetPriceUserDetails @WhereClause,@AccountList,@Rows,@PageNo,@Order_By,@RowCount OUT", 5, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("unassociatedCustomerEntity count :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { associatedCustomerEntityCount = unassociatedCustomerEntity?.Count });

            PriceUserListModel listModel = new PriceUserListModel { PriceUserList = unassociatedCustomerEntity?.ToList() };
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Method to associate customer
        public virtual bool AssociateCustomer(PriceUserListModel priceCustomerModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(priceCustomerModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PriceCustomerModelNotNull);

            if (priceCustomerModel?.PriceUserList?.Count < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceCustomerModelNotLessThanZero);

            IEnumerable<ZnodePriceListUser> associateCustomer = _priceListUserRepository.Insert(priceCustomerModel?.PriceUserList.ToEntity<ZnodePriceListUser>().ToList());

            return associateCustomer?.Count() > 0;
        }

        //Delete Associated Customer.
        public virtual bool DeleteAssociatedCustomer(ParameterModel customerIds)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(customerIds) || string.IsNullOrEmpty(customerIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PriceCustomerIDNotLessThanOne);

            ZnodeLogging.LogMessage("customerIds to be deleted :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { customerIds = customerIds?.Ids });

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePriceListUserEnum.PriceListUserId.ToString(), ProcedureFilterOperators.In, customerIds.Ids));

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return _priceListUserRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
        }

        //Get Associated Customer with Precedence data.
        public virtual PriceUserModel GetAssociatedCustomerPrecedence(int priceListUserId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { priceListUserId = priceListUserId });

            if (priceListUserId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceListUserIdNotLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListUserEnum.PriceListUserId.ToString(), ProcedureFilterOperators.Equals, priceListUserId.ToString()));

            //Gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GenerateFiltersForAssociatedPriceList(filters).ToFilterDataCollection());

            //Get expands for ZnodeUser.
            List<string> navigationProperties = GetExpands(expands);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return _priceListUserRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties)?.ToModel<PriceUserModel>();
        }

        //Update the precedence value for associated customers.
        public virtual bool UpdateAssociatedCustomerPrecedence(PriceUserModel priceAccountModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            bool status = false;
            if (IsNull(priceAccountModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PriceUserModelNotNull);

            if (priceAccountModel?.PriceListUserId > 0)
                status = _priceListUserRepository.Update(priceAccountModel.ToEntity<ZnodePriceListUser>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessPrecedingValueForCustomerUpdate : Admin_Resources.ErrorPrecedingValueForCustomerUpdate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return status;
        }
        #endregion

        #region Associate Account
        //Get Associated Account List.
        public virtual PriceAccountListModel GetAssociatedAccountList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            //Filters for getting associated list.
            filters.Add(new FilterTuple(FilterKeys.IsAssociated, ProcedureFilterOperators.Equals, "1"));

            if (!DefaultGlobalConfigSettingHelper.AllowGlobalLevelUserCreation)
                //Set Authorized Portal filter based on user portal access.
                BindUserPortalFilter(ref filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<PriceAccountModel> objStoredProc = new ZnodeViewRepository<PriceAccountModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<PriceAccountModel> associatedCustomerEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetPriceListAssociatedAccounts @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("associatedCustomerEntity count :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { associatedCustomerEntity = associatedCustomerEntity?.Count });

            PriceAccountListModel priceAccountListModel = new PriceAccountListModel { PriceAccountList = associatedCustomerEntity?.ToList() };

            priceAccountListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceAccountListModel;
        }

        //Get UnAssociated Account List.
        public virtual PriceAccountListModel GetUnAssociatedAccountList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            //Filters for getting unassociated list.
            filters.Add(new FilterTuple(FilterKeys.IsAssociated, ProcedureFilterOperators.Equals, "0"));

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<PriceAccountModel> objStoredProc = new ZnodeViewRepository<PriceAccountModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<PriceAccountModel> unassociatedCustomerEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetPriceListAssociatedAccounts @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("unassociatedCustomerEntity count :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { unassociatedCustomerEntity = unassociatedCustomerEntity?.Count });

            PriceAccountListModel listModel = new PriceAccountListModel { PriceAccountList = unassociatedCustomerEntity?.ToList() };
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Method to associate account.
        public virtual bool AssociateAccount(PriceAccountListModel priceAccountModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(priceAccountModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (priceAccountModel?.PriceAccountList?.Count < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PriceCustomerModelCountNotLessThanZero);

            IEnumerable<ZnodePriceListAccount> associateAccount = _priceListAccountRepository.Insert(priceAccountModel?.PriceAccountList.ToEntity<ZnodePriceListAccount>().ToList());

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return associateAccount?.Count() > 0;
        }

        //Delete Associated Account.
        public virtual bool DeleteAssociatedAccount(ParameterModel accountIds)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(accountIds) || string.IsNullOrEmpty(accountIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceAccountIdNotLessThanOne);

            ZnodeLogging.LogMessage("accountIds to be deleted :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { accountIds = accountIds?.Ids });

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePriceListAccountEnum.PriceListAccountId.ToString(), ProcedureFilterOperators.In, accountIds.Ids));

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return _priceListAccountRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
        }

        //Method to remove associate accounts.
        public virtual bool RemoveAssociatedAccounts(RemoveAssociatedAccountsModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(model) || string.IsNullOrEmpty(model.PriceListAccountIds) || model.PriceListId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelInvalid);

            ZnodeLogging.LogMessage("PriceListAccountIds to be removed :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListAccountIds = model?.PriceListAccountIds });

            //Generate where clause.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListEnum.PriceListId.ToString(), ProcedureFilterOperators.Equals, model.PriceListId.ToString()));
            filters.Add(new FilterTuple(ZnodePriceListAccountEnum.PriceListAccountId.ToString(), ProcedureFilterOperators.In, model.PriceListAccountIds));

            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            //Delete mapping of account against price.
            bool isDeleted = _priceListAccountRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessAccountUnassociate : Admin_Resources.ErrorAccountUnassociate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Get Associated Account with Precedence data.
        public virtual PriceAccountModel GetAssociatedAccountPrecedence(int priceListUserId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { priceListUserId = priceListUserId });

            if (priceListUserId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceListUserIdNotLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListAccountEnum.PriceListAccountId.ToString(), ProcedureFilterOperators.Equals, priceListUserId.ToString()));

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GenerateFiltersForAssociatedPriceList(filters).ToFilterDataCollection());

            //get expands for ZnodeUser.
            List<string> navigationProperties = GetExpands(expands);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return _priceListAccountRepository.GetEntity(whereClauseModel.WhereClause, navigationProperties)?.ToModel<PriceAccountModel>();
        }

        //Update the precedence value for associated accounts.
        public virtual bool UpdateAssociatedAccountPrecedence(PriceAccountModel priceAccountModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            bool status = false;
            if (IsNull(priceAccountModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (priceAccountModel?.PriceListAccountId > 0)
                status = _priceListAccountRepository.Update(priceAccountModel.ToEntity<ZnodePriceListAccount>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessPrecedenceValueUpdate : Admin_Resources.ErrorPrecedenceValueUpdate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return status;
        }
        #endregion

        #region Price Management
        //Get list of unassociated Price list.
        public virtual PriceListModel GetUnAssociatedPriceList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);


            string mode = string.Empty;
            PriceListModel priceListModel = new PriceListModel();
            if (filters.Count > 0)
            {
                if (filters.Any(x => x.FilterName == ZnodeConstant.Mode))
                    mode = filters.FirstOrDefault(x => x.FilterName == ZnodeConstant.Mode)?.FilterValue;
            }

            // gets the where clause with filter Values.
            filters = GenerateFiltersForUnAssociatedList(filters);

            //This method sets filter required to get price list.
            SetFilters(filters, priceListModel);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel for GetPagedList :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //get expands
            List<string> navigationProperties = GetExpands(expands);

            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });
            IZnodeViewRepository<PriceModel> objStoredProc = new ZnodeViewRepository<PriceModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@Mode", mode, ParameterDirection.Input, DbType.String);
            IList<PriceModel> priceList = objStoredProc.ExecuteStoredProcedureList("Znode_GetUnAssociatedPriceList  @WhereClause,@Rows,@PageNo,@Order_By,@RowsCount OUT,@Mode", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("priceList count :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, priceList?.Count);

            priceListModel.PriceList = priceList?.ToList();

            priceListModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return priceListModel;

        }

        //Method to remove associated price list to store.
        public virtual bool RemoveAssociatedPriceListToStore(ParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(model) || string.IsNullOrEmpty(model.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelInvalid);

            ZnodeLogging.LogMessage("Ids to be removed :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { Ids = model?.Ids });

            //Generate where clause.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListPortalEnum.PriceListPortalId.ToString(), ProcedureFilterOperators.In, model.Ids.ToString()));

            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            //Delete mapping of profile against price.
            bool isDeleted = _priceListPortalRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessPriceListUnassociateFromStore : Admin_Resources.ErrorPriceListUnassociateFromStore, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        public virtual bool RemoveAssociatedPriceListToProfile(ParameterModel priceListProfileIds)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(priceListProfileIds?.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceListProfileIdNotNullOrEmpty);

            ZnodeLogging.LogMessage("priceListProfileIds to be removed :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { priceListProfileIds = priceListProfileIds?.Ids });

            //Generate where clause.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListProfileEnum.PriceListProfileId.ToString(), ProcedureFilterOperators.In, priceListProfileIds.Ids));

            //Delete mapping of profile against price.
            bool isDeleted = _priceListProfileRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection())?.WhereClause);
            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessPriceListUnassociateFromProfile : Admin_Resources.ErrorPriceListUnassociateFromProfile, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Get Associated Price List with Precedence data for Store/Profile.
        public virtual PricePortalModel GetAssociatedPriceListPrecedence(PricePortalModel pricePortalModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("PriceListProfileId :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { PriceListProfileId = pricePortalModel?.PriceListProfileId });

            FilterCollection filters = new FilterCollection();

            if (pricePortalModel?.PriceListProfileId > 0)
            {
                filters.Add(new FilterTuple(ZnodePriceListProfileEnum.PriceListProfileId.ToString(), ProcedureFilterOperators.Equals, pricePortalModel.PriceListProfileId.ToString()));

                //If profile id is greater assign Price List to Profile
                return _priceListProfileRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GenerateFiltersForAssociatedPriceList(filters).ToFilterDataCollection()).WhereClause)?.ToModel<PricePortalModel>();
            }

            //Assign Price List to Portal.
            return GetAssociatedPriceListPrecedenceForStore(filters, pricePortalModel);
        }

        //Update the precedence value for associated price list for Store/Profile.
        public virtual bool UpdateAssociatedPriceListPrecedence(PricePortalModel pricePortalModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            if (IsNull(pricePortalModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.PricePortalModelNotNull);

            bool status = false;

            //If profile and pricelistprofile id is greater update Price List for Profile else for Store.
            if (pricePortalModel?.PriceListProfileId > 0 && pricePortalModel.PortalProfileId > 0)
                status = _priceListProfileRepository.Update(pricePortalModel.ToEntity<ZnodePriceListProfile>());

            if (pricePortalModel?.PriceListPortalId > 0 && pricePortalModel.PortalId > 0)
                status = _priceListPortalRepository.Update(pricePortalModel.ToEntity<ZnodePriceListPortal>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessPrecedenceValueForPriceListUpdate : Admin_Resources.ErrorPrecedenceValueForPriceListUpdate, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);

            return status;
        }
        #endregion

        #region Export Price
        //Get export price data 
        public virtual List<ExportPriceModel> GetExportPriceData(string priceListIds)
        {
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { priceListIds = priceListIds });

            IZnodeViewRepository<ExportPriceModel> objStoredProc = new ZnodeViewRepository<ExportPriceModel>();
            objStoredProc.SetParameter(ZnodePriceListEnum.PriceListId.ToString(), priceListIds, ParameterDirection.Input, DbType.String);

            return objStoredProc.ExecuteStoredProcedureList("Znode_ExportPriceList  @PriceListId").ToList();
        }
        #endregion

        #endregion

        #region Private Method
        //Checks if SKU already exists or not.
        private bool IsSKUExist(string SKU, int priceListId)
            => _priceRepository.Table.Any(x => x.SKU == SKU && x.PriceListId == priceListId);

        //Check if pricing list code is already present or not.
        private bool IsCodeAlreadyExist(string priceListCode)
         => _priceListRepository.Table.Any(x => x.ListCode == priceListCode);

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    if (Equals(key, ZnodePriceListPortalEnum.ZnodePortal.ToString().ToLower())) SetExpands(ZnodePriceListPortalEnum.ZnodePortal.ToString(), navigationProperties);
                    if (Equals(key, ZnodePriceListEnum.ZnodePriceListProfiles.ToString().ToLower())) SetExpands(ZnodePriceListEnum.ZnodePriceListProfiles.ToString(), navigationProperties);
                    if (Equals(key, ZnodePriceListEnum.ZnodePriceListUsers.ToString().ToLower())) SetExpands(ZnodePriceListEnum.ZnodePriceListUsers.ToString(), navigationProperties);
                    if (Equals(key, ZnodePriceListAccountEnum.ZnodeAccount.ToString().ToLower())) SetExpands(ZnodePriceListAccountEnum.ZnodeAccount.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Generate filters for store associated price list.
        private FilterCollection StoreFilter(FilterCollection filters)
        {
            string priceListids = string.Empty;

            FilterCollection finalfilter = new FilterCollection();

            //Get filters for portal Id.
            FilterTuple _filterStore = filters.FirstOrDefault(x => x.FilterName == ZnodePriceListPortalEnum.PortalId.ToString().ToLower());
            if (IsNotNull(_filterStore))
            {
                //Get price list based on below portal id.
                int portalId = Convert.ToInt32(_filterStore.FilterValue);
                priceListids = string.Join(",", (_priceListPortalRepository.Table.Where(x => x.PortalId == portalId).Select(x => x.PriceListId.ToString())).ToArray());

                //If priceListids are empty, no list will appear in grid.
                if (!string.IsNullOrEmpty(priceListids))
                    finalfilter.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.In, string.Join(",", priceListids));
                else
                    finalfilter.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.Equals, "-1");
            }
            return finalfilter;
        }

        //Generate filters for Account associated price list.
        private FilterCollection AccountFilter(FilterCollection filters)
        {
            FilterCollection finalfilter = new FilterCollection();

            //Get filters for Account Id.
            int accountId = Convert.ToInt32(filters.Find(x => x.FilterName == ZnodePriceListAccountEnum.AccountId.ToString().ToLower())?.FilterValue);
            ZnodeLogging.LogMessage("accountId :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { accountId = accountId });

            if (accountId > 0)
            {
                //Get price list based on below account id.
                string priceListids = string.Join(",", (_priceListAccountRepository.Table.Where(x => x.AccountId == accountId).Select(x => x.PriceListId.ToString())).ToArray());

                //If priceListids are empty, no list will appear in grid.
                if (!string.IsNullOrEmpty(priceListids))
                    finalfilter.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.In, string.Join(",", priceListids));
                else
                    finalfilter.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.Equals, "-1");
            }
            if (filters.Exists(x => x.FilterName == ZnodePriceListEnum.ListCode.ToString().ToLower()))
                finalfilter.Add(filters.FirstOrDefault(x => x.FilterName == ZnodePriceListEnum.ListCode.ToString().ToLower()));
            return finalfilter;
        }

        //Generate filters for Customer associated price list.
        private FilterCollection CustomerFilter(FilterCollection filters)
        {
            FilterCollection finalfilter = new FilterCollection();

            //Get filters for Account Id.
            int userId = Convert.ToInt32(filters.Find(x => x.FilterName == ZnodePriceListUserEnum.UserId.ToString().ToLower())?.FilterValue);
            if (userId > 0)
            {
                string priceListids = string.Empty;

                int? accountId = IsB2BUser(userId);
                ZnodeLogging.LogMessage("accountId :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { accountId = accountId });

                //If accountId is greater than zero get price list ids from B2B customer else from B2C customer.
                priceListids = IsNotNull(accountId) ? string.Join(",", (_priceListAccountRepository.Table.Where(x => x.AccountId == accountId).Select(x => x.PriceListId.ToString())).ToArray())
                               : string.Join(",", (_priceListUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.PriceListId.ToString())).ToArray());

                //If priceListids are empty, no list will appear in grid.
                if (!string.IsNullOrEmpty(priceListids))
                    finalfilter.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.In, string.Join(",", priceListids));
                else
                    finalfilter.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.Equals, "-1");
            }
            if (filters.Exists(x => x.FilterName == ZnodePriceListEnum.ListCode.ToString().ToLower()))
                finalfilter.Add(filters.FirstOrDefault(x => x.FilterName == ZnodePriceListEnum.ListCode.ToString().ToLower()));
            return finalfilter;
        }

        //Generate filters for portal profile associated price list.
        private FilterCollection ProfileFilter(FilterCollection filters)
        {
            string priceListIds = string.Empty;
            FilterCollection finalfilter = new FilterCollection();

            //Get filters for profile Id.
            FilterTuple _filterProfile = filters.FirstOrDefault(x => x.FilterName == ZnodePriceListProfileEnum.PortalProfileId.ToString().ToLower());

            if (IsNotNull(_filterProfile))
            {
                //Get price list for portal profile id other than 0.
                int portalProfileId = Convert.ToInt32(_filterProfile.FilterValue);
                if (portalProfileId > 0)
                {
                    priceListIds = string.Join(",", (_priceListProfileRepository.Table.Where(x => x.PortalProfileId == portalProfileId).Select(x => x.PriceListId.ToString())).ToArray());

                    //If priceListids are empty, no list will appear in grid.
                    if (!string.IsNullOrEmpty(priceListIds))
                        finalfilter.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.In, string.Join(",", priceListIds));
                    else
                        finalfilter.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.Equals, "-1");
                }
                else
                {
                    int portalId = Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName == ZnodePriceListPortalEnum.PortalId.ToString().ToLower()).FilterValue);

                    IEnumerable<PriceProfileModel> priceListids = (_priceListProfileRepository.Table.Join(_portalProfileRepository.Table.Where(w => w.PortalId.Equals(portalId))
                        , plp => plp.PortalProfileId, pp => pp.PortalProfileID, (plp, pp) => new PriceProfileModel { PriceListId = plp.PriceListId })).ToList();

                    //If priceListids are empty, no list will appear in grid.
                    if (priceListids?.Count() > 0)
                        finalfilter.Add(ZnodePriceListProfileEnum.PriceListId.ToString(), FilterOperators.In, string.Join(",", priceListids.Select(x => x.PriceListId)));
                    else
                        finalfilter.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.Equals, "-1");
                }
            }
            return finalfilter;
        }

        //Generate filters for associated price list based on mode(store or profile).
        private FilterCollection GenerateFiltersForAssociatedPriceList(FilterCollection filters)
        {
            string mode = filters.FirstOrDefault(x => x.FilterName == ZnodeConstant.Mode)?.FilterValue;

            switch (mode)
            {
                case ZnodeConstant.Portal:
                    return StoreFilter(filters);
                case ZnodeConstant.Account:
                    return AccountFilter(filters);
                case ZnodeConstant.User:
                    return CustomerFilter(filters);
                case ZnodeConstant.Profile:
                    return ProfileFilter(filters);
                default:
                    return filters;
            }
        }

        //Generate filters for unassociated price list based on mode(store or profile).
        private FilterCollection GenerateFiltersForUnAssociatedList(FilterCollection filters)
        {
            string mode = filters.FirstOrDefault(x => x.FilterName == ZnodeConstant.Mode)?.FilterValue;

            switch (mode)
            {
                case ZnodeConstant.Portal:
                    return UnAssociatePriceListStoreFilter(filters);
                case ZnodeConstant.Account:
                    return UnAssociatePriceListAccountFilter(filters);
                case ZnodeConstant.User:
                    return UnAssociatePriceListCustomerFilter(filters);
                case ZnodeConstant.Profile:
                    return UnAssociatePriceListProfileFilter(filters);
                default:
                    return filters;
            }
        }

        //Generate filters for store unassociated price list.
        private FilterCollection UnAssociatePriceListStoreFilter(FilterCollection filters)
        {

            //Get filter for store.
            FilterTuple _filterStore = filters.FirstOrDefault(x => x.FilterName == ZnodePriceListPortalEnum.PortalId.ToString().ToLower());

            if (IsNotNull(_filterStore))
            {
                //Get associated price lists for below portal id.
                int portalId = Convert.ToInt32(_filterStore.FilterValue);
                if (portalId > 0)
                {
                    filters.Add(ZnodeConstant.PortalId, FilterOperators.Equals, portalId.ToString());
                    //Set currency filter.
                    SetCurrencyFilter(portalId, filters);
                }

                //Set Culture filter
                SetCultureFilter(portalId, filters);

                string priceListids = string.Join(",", (_priceListPortalRepository.Table.Where(x => x.PortalId == portalId).Select(x => x.PriceListId.ToString())).ToArray());

                //If priceListids are empty, all list will appear in grid.
                if (!string.IsNullOrEmpty(priceListids))
                    filters.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.NotIn, string.Join(",", priceListids));
            }
            return filters;
        }

        //If filter contains "|" set Global Search Filter.
        private void SetGlobalSearchFilter(FilterCollection filters, FilterCollection finalfilter)
        {
            FilterTuple globalFilter = filters.FirstOrDefault(x => x.FilterName.Contains("|"));
            filters.Add(globalFilter.FilterName.Replace("currencyname", string.Empty), globalFilter.FilterOperator, globalFilter.FilterValue);
        }

        //Generate filters for account unassociated price list.
        private FilterCollection UnAssociatePriceListAccountFilter(FilterCollection filters)
        {

            //Get account id from filter.
            int accountId = Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName == ZnodePriceListAccountEnum.AccountId.ToString().ToLower()).FilterValue);
            ZnodeLogging.LogMessage("accountId :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { accountId = accountId });

            if (accountId > 0)
            {
                filters.Add(ZnodeConstant.AccountId, FilterOperators.Equals, accountId.ToString());
                //Get associated price lists for below account id.
                string priceListids = string.Join(",", (_priceListAccountRepository.Table.Where(x => x.AccountId == accountId).Select(x => x.PriceListId.ToString())).ToArray());

                //If priceListids are empty, all list will appear in grid.
                if (!string.IsNullOrEmpty(priceListids))
                    filters.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.NotIn, string.Join(",", priceListids));

                IZnodeRepository<ZnodePortalAccount> _portalAccountRepository = new ZnodeRepository<ZnodePortalAccount>();
                int portalId = (_portalAccountRepository.Table.FirstOrDefault(x => x.AccountId == accountId)?.PortalId).GetValueOrDefault();

                if (portalId > 0)
                    //Set currency filter.
                    SetCurrencyFilter(portalId, filters);
            }
            return filters;
        }

        //Generate filters for customer unassociated price list.
        private FilterCollection UnAssociatePriceListCustomerFilter(FilterCollection filters)
        {

            //Get user id from filter.
            int userId = Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName == ZnodePriceListUserEnum.UserId.ToString().ToLower()).FilterValue);
            if (userId > 0)
            {
                int? accountId = IsB2BUser(userId);
                string priceListids = string.Empty;
                filters.Add(ZnodeConstant.UserId, FilterOperators.Equals, userId.ToString());
                if (IsNotNull(accountId))
                {
                    IQueryable<string> accountPriceListids = _priceListAccountRepository.Table.Where(x => x.AccountId == accountId)?.Select(x => x.PriceListId.ToString());
                    if (accountPriceListids?.Count() > 0)
                        priceListids = string.Join(",", accountPriceListids);
                }

                IQueryable<string> customerPriceListids = _priceListUserRepository.Table.Where(x => x.UserId == userId)?.Select(x => x.PriceListId.ToString());
                if (customerPriceListids?.Count() > 0)
                    priceListids = string.Join(",", customerPriceListids);

                if (!string.IsNullOrEmpty(priceListids))
                    filters.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.NotIn, priceListids);

                IZnodeRepository<ZnodeUserPortal> _userPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
                int portalId = (_userPortalRepository.Table.FirstOrDefault(x => x.UserId == userId)?.PortalId).GetValueOrDefault();

                if (portalId > 0)
                    //Set currency filter.
                    SetCurrencyFilter(portalId, filters);
            }
            return filters;
        }

        //Generate filters for profile unassociated price list.
        private FilterCollection UnAssociatePriceListProfileFilter(FilterCollection filters)
        {
            string priceListids = string.Empty;


            //Get filter for profile.
            FilterTuple _filterProfile = filters.FirstOrDefault(x => x.FilterName == ZnodePriceListProfileEnum.PortalProfileId.ToString().ToLower());

            if (IsNotNull(_filterProfile))
            {
                int portalProfileId = Convert.ToInt32(_filterProfile.FilterValue);

                //Get price list for portal profile id other than 0.
                if (portalProfileId > 0)
                {
                    filters.Add(ZnodeConstant.PortalProfileId, FilterOperators.Equals, portalProfileId.ToString());
                    priceListids = string.Join(",", (_priceListProfileRepository.Table.Where(x => x.PortalProfileId == portalProfileId).Select(x => x.PriceListId.ToString())).ToArray());

                    //If priceListids are empty, all list will appear in grid.
                    if (!string.IsNullOrEmpty(priceListids))
                        filters.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.NotIn, string.Join(",", priceListids));

                    int portalId = Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName == ZnodePriceListPortalEnum.PortalId.ToString().ToLower())?.FilterValue);
                    if (portalId > 0)
                        //Set currency filter.
                        SetCurrencyFilter(portalId, filters);
                }

                //Get price list for profile id 0 (means all profiles for the existing portal).
                else
                {
                    int portalId = Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName == ZnodePriceListPortalEnum.PortalId.ToString().ToLower()).FilterValue);

                    //Get price list for the above portal.
                    IEnumerable<PriceProfileModel> priceListIds = (_priceListProfileRepository.Table.Join(_portalProfileRepository.Table.Where(w => w.PortalId.Equals(portalId))
                        , plp => plp.PortalProfileId, pp => pp.PortalProfileID, (plp, pp) => new PriceProfileModel { PriceListId = plp.PriceListId })).AsEnumerable().Distinct();

                    //If priceListids are empty, all list will appear in grid.
                    if (priceListIds?.ToList().Count > 0)
                        filters.Add(ZnodePriceListProfileEnum.PriceListId.ToString(), FilterOperators.NotIn, string.Join(",", priceListIds.Select(x => x.PriceListId).Distinct()));
                    else
                    {
                        priceListids = string.Join(",", (_priceListRepository.Table.Select(x => x.PriceListId.ToString())).ToList());
                        filters.Add(ZnodePriceListEnum.PriceListId.ToString(), FilterOperators.In, string.Join(",", priceListids));
                    }
                }
            }
            return filters;
        }

        //Set currency filter.
        private void SetCurrencyFilter(int portalId, FilterCollection finalfilter)
        {
            ZnodeLogging.LogMessage("Input parameters  :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            //Get currency id to get unassociated price list according to currency assigned to portal.
            //fetching currency id using culture id from culture table.
            int currencyId = GetCurrencyIdBasedOnPortalId(portalId);
            ZnodeLogging.LogMessage("currencyId :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { currencyId = currencyId });

            //If currencyId is greater than 0, add currencyId filter else throw exception.
            if (currencyId > 0)
                finalfilter.Add(ZnodeCurrencyEnum.CurrencyId.ToString(), FilterOperators.Equals, currencyId.ToString());
            else
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceListNotDisplayedAsCurrencyNotSet);
        }

        //Get default currency associated to portal. 
        private int GetCurrencyIdBasedOnPortalId(int portalId) => (_portalUnitRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.CurrencyId).GetValueOrDefault();

        //Set Culture filter
        private void SetCultureFilter(int portalId, FilterCollection finalfilter)
        {
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new { portalId = portalId });

            //Get currency id to get unassociated price list according to currency assigned to portal.
            int cultureId = (_portalUnitRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.CultureId).GetValueOrDefault();

            //If currencyId is greater than 0, add currencyId filter else throw exception.
            if (cultureId > 0)
                finalfilter.Add(ZnodeCultureEnum.CultureId.ToString(), FilterOperators.Equals, cultureId.ToString());
            else
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceListNotDisplayedAsCultureNotSet);
        }

        //Set finalfilter for UnAssociatedPriceList.
        private void FilterForUnAssociatedPriceList(FilterCollection filters, FilterCollection finalfilter)
        {
            if (IsNotNull(filters) && IsNotNull(finalfilter))
            {
                //Set finalfilter for List Code if filters contains it.
                FilterTuple _filterListCode = filters.FirstOrDefault(x => x.FilterName == FilterKeys.ListCode);
                if (IsNotNull(_filterListCode))
                    finalfilter.Add(_filterListCode);

                //Set finalfilter for List Name if filters contains it.
                FilterTuple _listNamefilter = filters.FirstOrDefault(x => x.FilterName == ZnodePriceListEnum.ListName.ToString().ToLower());
                if (IsNotNull(_listNamefilter))
                    finalfilter.Add(_listNamefilter);

                //Set finalfilter for Currency if filters contains it.
                FilterTuple _filterCurrency = filters.FirstOrDefault(x => string.Equals(x.FilterName, ZnodeCurrencyEnum.CurrencyName.ToString(), StringComparison.CurrentCultureIgnoreCase));
                if (IsNotNull(_filterCurrency))
                {
                    EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(new FilterCollection() { _filterCurrency }.ToFilterDataCollection());
                    List<int> currencyId = _currencyRepository.GetEntityList(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues)?.Select(x => x.CurrencyId)?.ToList();
                    finalfilter.Add(ZnodeCurrencyEnum.CurrencyId.ToString(), FilterOperators.In, currencyId?.Count > 0 ? string.Join(",", currencyId) : "0");
                }

                //Set finalfilter for Activation Date if filters contains it.
                FilterTuple _activationfilter = filters.FirstOrDefault(x => x.FilterName == ZnodePriceEnum.ActivationDate.ToString().ToLower());
                if (IsNotNull(_activationfilter))
                    finalfilter.Add(_activationfilter);

                //Set finalfilter for Expiration Date if filters contains it.
                FilterTuple _expirationfilter = filters.FirstOrDefault(x => x.FilterName == ZnodePriceEnum.ExpirationDate.ToString().ToLower());
                if (IsNotNull(_expirationfilter))
                    finalfilter.Add(_expirationfilter);

                //If filter contains "|" set Global Search Filter.
                if (filters.Any(x => x.FilterName.Contains("|")))
                    SetGlobalSearchFilter(filters, finalfilter);
            }
        }

        //Assign Price List to Portal.
        private PricePortalModel GetAssociatedPriceListPrecedenceForStore(FilterCollection filters, PricePortalModel pricePortalModel)
        {
            filters.Add(new FilterTuple(ZnodePriceListPortalEnum.PriceListId.ToString(), ProcedureFilterOperators.Equals, pricePortalModel.PriceListId.ToString()));
            filters.Add(new FilterTuple(ZnodePriceListPortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, pricePortalModel.PortalId.ToString()));
            return _priceListPortalRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GenerateFiltersForAssociatedPriceList(filters).ToFilterDataCollection()).WhereClause)?.ToModel<PricePortalModel>();
        }

        private int? IsB2BUser(int userId)
        => _userRepository.Table.Where(x => x.UserId == userId).Select(x => x.AccountId).FirstOrDefault();

        //This method removes mode from filter and profileId if profileId equals zero.
        private void SetFilters(FilterCollection filters, PriceListModel priceListModel)
        {
            //Remove mode from filter.
            filters.RemoveAll(x => x.Item1.Equals(ZnodeConstant.Mode, StringComparison.InvariantCultureIgnoreCase));

            //Check if customer is B2B and add account id to filter.
            if (filters.Exists(x => x.FilterName.Equals(ZnodeUserEnum.UserId.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                SetFiltersForCustomer(filters, priceListModel);

            //Check if filter contains profile id.
            if (filters.Exists(x => x.FilterName.Equals(ZnodeProfileEnum.ProfileId.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                SetFiltersForProfile(filters);
        }

        //If customer is B2B, add account id to filter.
        private void SetFiltersForCustomer(FilterCollection filters, PriceListModel priceListModel)
        {
            int userId = Convert.ToInt32(filters.Find(x => x.FilterName == ZnodeUserEnum.UserId.ToString().ToLower())?.FilterValue);
            if (userId > 0)
            {
                int? accountId = IsB2BUser(userId);
                if (IsNotNull(accountId))
                    filters.Add(ZnodeAccountEnum.AccountId.ToString().ToLower(), FilterOperators.Equals, accountId.ToString());
                ZnodeLogging.LogMessage("Filter created for getting customer:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, filters);
                //Get customer name by user Id.
                GetCustomerName(userId, priceListModel);
            }
        }

        //If filter contains profile id equals 0, remove it.
        private void SetFiltersForProfile(FilterCollection filters)
        {
            int profileId = Convert.ToInt32(filters.Find(x => x.FilterName == ZnodeProfileEnum.ProfileId.ToString().ToLower())?.FilterValue);
            if (profileId == 0)
                filters.RemoveAll(x => x.Item1.Equals(ZnodeProfileEnum.ProfileId.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        //Check if account is parent account or not.
        private void IsParentAccount(PriceListModel priceListModel, FilterCollection filters)
        {
            int accountId = Convert.ToInt32(filters.FirstOrDefault(x => string.Equals(x.FilterName, ZnodeAccountEnum.AccountId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
            if (accountId > 0)
                priceListModel.HasParentAccounts = IsNull(new ZnodeRepository<ZnodeAccount>().Table.FirstOrDefault(x => x.AccountId == accountId)?.ParentAccountId);
        }

        //Get customer name by userId.
        private void GetCustomerName(int? userId, PriceListModel priceListModel)
        {
            if (IsNotNull(userId))
                priceListModel.CustomerName = _userRepository.Table.Where(x => x.UserId == userId).Select(x => x.FirstName + " " + x.LastName)?.FirstOrDefault();
            ZnodeLogging.LogMessage("Get customer name base on userId:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, new object[] { priceListModel?.CustomerName, userId });
        }

        //If price list is updated with new currency delete its association from Portal/Profile having old currency.
        private void RemoveAssociationOnCurrencyUpdate(PriceModel priceModel)
        {
            if (priceModel.OldCurrencyId > 0 && priceModel.CurrencyId > 0 && priceModel.CurrencyId != priceModel.OldCurrencyId)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodePriceListEnum.PriceListId.ToString(), ProcedureFilterOperators.Equals, priceModel.PriceListId.ToString()));
                EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("entity where clause model:", ZnodeLogging.Components.Setup.ToString(), TraceLevel.Verbose, entityWhereClauseModel);
                //Remove its association from Portal having old currency. 
                RemoveAssociationFromPortal(entityWhereClauseModel);

                //Remove its association from Profile having old currency.
                RemoveAssociationFromProfile(entityWhereClauseModel);

                //Remove its association from Customer having old currency.
                RemoveAssociationFromCustomer(entityWhereClauseModel);

                //Remove its association from Account having old currency.
                RemoveAssociationFromAccount(entityWhereClauseModel);
            }
        }

        //Remove its association from Portal having old currency. 
        private void RemoveAssociationFromPortal(EntityWhereClauseModel entityWhereClauseModel) => _priceListPortalRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);

        //Remove its association from Profile having old currency.
        private void RemoveAssociationFromProfile(EntityWhereClauseModel entityWhereClauseModel) => _priceListProfileRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);

        //Remove its association from Customer having old currency.
        private void RemoveAssociationFromCustomer(EntityWhereClauseModel entityWhereClauseModel) => _priceListUserRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);

        //Remove its association from Account having old currency.
        private void RemoveAssociationFromAccount(EntityWhereClauseModel entityWhereClauseModel) => _priceListAccountRepository.Delete(entityWhereClauseModel.WhereClause, entityWhereClauseModel.FilterValues);
        #endregion
    }
}

