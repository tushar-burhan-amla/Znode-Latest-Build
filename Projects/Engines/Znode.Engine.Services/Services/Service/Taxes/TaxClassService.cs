using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Engine.Taxes;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class TaxClassService : BaseService, ITaxClassService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodeTaxClass> _taxClassRepository;
        private readonly IZnodeRepository<ZnodeTaxClassSKU> _taxClassSKURepository;
        private readonly IZnodeRepository<ZnodeTaxRule> _taxRuleRepository;
        private readonly IZnodeRepository<ZnodePortalTaxClass> _taxClassPortalRepository;
        private readonly IZnodeRepository<ZnodePimAttribute> _pimAttributeRepository;
        private readonly IZnodeRepository<ZnodePimAttributeValue> _pimAttributeValueRepository;
        private readonly IZnodeRepository<ZnodePimAttributeValueLocale> _pimAttributeValueLocaleRepository;
        private readonly IProductService _productService;

        #endregion Private Variables

        #region Constructor

        public TaxClassService(IProductService productService)
        {
            _taxClassRepository = new ZnodeRepository<ZnodeTaxClass>();
            _taxClassSKURepository = new ZnodeRepository<ZnodeTaxClassSKU>();
            _taxRuleRepository = new ZnodeRepository<ZnodeTaxRule>();
            _taxClassPortalRepository = new ZnodeRepository<ZnodePortalTaxClass>();
            _pimAttributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            _pimAttributeValueRepository = new ZnodeRepository<ZnodePimAttributeValue>();
            _pimAttributeValueLocaleRepository = new ZnodeRepository<ZnodePimAttributeValueLocale>();
            _productService = productService;
        }

        #endregion Constructor

        #region Public Methods

        //Returns tax class list.
        public virtual TaxClassListModel GetTaxClassList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            int portalId = GetPortalId(filters);
            bool isAssociated = GetIsAssociatedValue(filters);
            ResetFilters(filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("portalId and pageListModel to get taxClassList list ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { portalId = portalId, pageListModel = pageListModel?.ToDebugString() });
            IZnodeViewRepository<TaxClassModel> objStoredProc = new ZnodeViewRepository<TaxClassModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@IsAssociated", Convert.ToInt32(isAssociated), ParameterDirection.Input, DbType.Int32);

            //Gets the entity list according to where clause, order by clause and pagination
            IList<TaxClassModel> taxClassList = objStoredProc.ExecuteStoredProcedureList("Znode_GetTaxClassPortal  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@PortalId,@IsAssociated", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("taxClass list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, taxClassList?.Count());
            TaxClassListModel taxClassListModel = new TaxClassListModel { TaxClassList = taxClassList?.ToList() };

            taxClassListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxClassListModel;
        }

        //Returns a tax class on the basis of taxClassId.
        public virtual TaxClassModel GetTaxClass(int taxClassId) =>
            _taxClassRepository.GetById(taxClassId).ToModel<TaxClassModel, ZnodeTaxClass>();

        //Create tax class.
        public virtual TaxClassModel CreateTaxClass(TaxClassModel taxClassModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(taxClassModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (IsTaxClassExist(taxClassModel.Name, taxClassModel.TaxClassId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, string.Format(Admin_Resources.TaxClassAlreadyExist, taxClassModel.Name));
           
            ZnodeTaxClass taxClass = _taxClassRepository.Insert(taxClassModel.ToEntity<ZnodeTaxClass>());
            taxClassModel.TaxClassId = taxClass.TaxClassId;

            if (taxClass.TaxClassId > 0 && !string.IsNullOrEmpty(taxClassModel.ImportedSKUs))
                InsertTaxClassSku(taxClassModel);
           
            ZnodeLogging.LogMessage(IsNotNull(taxClass) ? string.Format(Admin_Resources.SuccessTaxClassCreate, taxClassModel.Name) : string.Format(Admin_Resources.ErrorTaxClassCreate, taxClassModel.Name), string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxClass.ToModel<TaxClassModel>();
        }

        //Update tax class.
        public virtual bool UpdateTaxClass(TaxClassModel taxClassModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(taxClassModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (taxClassModel.TaxClassId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.TaxClassIdValue);

            if (IsTaxClassExist(taxClassModel.Name, taxClassModel.TaxClassId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, string.Format(Admin_Resources.TaxClassAlreadyExist, taxClassModel.Name));

            if (IsTaxClassAssociated(taxClassModel.TaxClassId) && !taxClassModel.IsActive)
                throw new ZnodeException(ErrorCodes.AssociationUpdateError, string.Format(Admin_Resources.TaxClassCanNotExist, taxClassModel.Name));
            
            //Update tax class
            bool isTaxClassUpdated = _taxClassRepository.Update(taxClassModel.ToEntity<ZnodeTaxClass>());

            if (!string.IsNullOrEmpty(taxClassModel.ImportedSKUs))
                InsertTaxClassSku(taxClassModel);

            ZnodeLogging.LogMessage(isTaxClassUpdated ? Admin_Resources.SuccessTaxClassUpdate : Admin_Resources.ErrorTaxClassUpdate, string.Empty, TraceLevel.Info);
            return isTaxClassUpdated;
        }

        //Delete tax class.
        public virtual bool DeleteTaxClass(ParameterModel taxClassId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(taxClassId.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.TaxClassNotNull);

                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter(ZnodeTaxClassEnum.TaxClassId.ToString(), taxClassId.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                objStoredProc.ExecuteStoredProcedureList("Znode_DeleteTaxClass @TaxClassId,  @Status OUT", 1, out status);

                //SP will return status as 1 if tax class as well as all its associated items deleted successfully.
                if (Equals(status, 1))
                {
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessTaxClassDelete, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                    return true;
                }
                else
                {
                    throw new ZnodeException(ErrorCodes.AssociationDeleteError,Admin_Resources.ErrorTaxClassDeleteAsUsedByOther);
                }
        }
        
        #region Tax Class SKU

        //Get tax class SKU List.
        public virtual TaxClassSKUListModel GetTaxClassSKUList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            int taxClassId = GetTaxClassId(filters);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string whereClause = ProductService.GenerateXMLWhereClauseForSP(filters.ToFilterDataCollection());
            IZnodeViewRepository<TaxClassSKUModel> objStoredProc = new ZnodeViewRepository<TaxClassSKUModel>();
            ZnodeLogging.LogMessage("whereClause and pageListModel to get associatedSKUs", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { WhereClause = whereClause, PageListModel = pageListModel?.ToDebugString() });
            objStoredProc.SetParameter("@WhereClause", whereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@TaxClassId", taxClassId, ParameterDirection.Input, DbType.Int32);
            //Gets the entity list according to where clause, order by clause and pagination
            IList<TaxClassSKUModel> associatedSKUs = objStoredProc.ExecuteStoredProcedureList("Znode_GetTaxlClassDetail  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@TaxClassId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("associatedSKUs list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, associatedSKUs?.Count());
            TaxClassSKUListModel taxClassSKUListModel = new TaxClassSKUListModel { TaxClassSKUList = associatedSKUs?.ToList() };

            taxClassSKUListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxClassSKUListModel;
        }

        //Get paged Unassociated Products list
        public virtual ProductDetailsListModel GetUnassociatedProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            filters.Add(new FilterTuple(ZnodeTaxClassEnum.TaxClassId.ToString().ToLower(), ProcedureFilterOperators.Equals, "-1"));
            TaxClassSKUListModel taxClassSKUList = GetTaxClassSKUList(filters, null, null);
            string pimProductIdsNotIn = String.Join(",", taxClassSKUList.TaxClassSKUList.Select(x => x.PimProductId).Distinct());
            ZnodeLogging.LogMessage("pimProductIdsNotIn to get productList: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { PimProductIdsNotIn = pimProductIdsNotIn });
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            bool isNotInFilter = false;
            if (!string.IsNullOrEmpty(pimProductIdsNotIn))
            {
                isNotInFilter = true;
            }
            ProductDetailsListModel productList = _productService.GetXmlProduct(filters, pageListModel, pimProductIdsNotIn, isNotInFilter);
            productList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return productList;
        }

        //Add tax Class SKU
        public virtual TaxClassSKUModel AddTaxClassSKU(TaxClassSKUModel taxClassSKUModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(taxClassSKUModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            ZnodeLogging.LogMessage(_taxClassSKURepository.Insert(GetTaxClassSKUListModel(taxClassSKUModel))?.ToList()?.Count() > 0 ? Admin_Resources.SuccessTaxSkuAdd : Admin_Resources.ErrorTaxSkuAdd, string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxClassSKUModel;
        }

        //Delete Tax class SKU.
        public virtual bool DeleteTaxClassSKU(ParameterModel taxClassSKUId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            bool status = false;
            if (taxClassSKUId.Ids.Count() < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);
            
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeTaxClassSKUEnum.TaxClassSKUId.ToString(), ProcedureFilterOperators.In, taxClassSKUId.Ids.ToString()));

            status = _taxClassSKURepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(status ? string.Format(Admin_Resources.SuccessAssociatedSkuDelete, ZnodeTaxClassSKUEnum.TaxClassSKUId.ToString()) : Admin_Resources.ErrorAssociatedSkuDelete, string.Empty, TraceLevel.Info);

            return status;
        }

        #endregion Tax Class SKU

        #region Tax Rule

        //Get TaxRule list.
        public virtual TaxRuleListModel GetTaxRuleList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //get expands
            var navigationProperties = GetExpands(expands);

            //Gets the entity list according to where clause, order by clause and pagination
            IList<ZnodeTaxRule> associatedRules = _taxRuleRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, navigationProperties, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("associatedRules list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, associatedRules?.Count());
            TaxRuleListModel taxRuleListModel = new TaxRuleListModel { TaxRuleList = associatedRules?.ToModel<TaxRuleModel>().ToList() };

            taxRuleListModel.TaxRuleList.ForEach(item => item.TaxRuleTypeName = associatedRules.Where(x => x.TaxRuleId == item.TaxRuleId).Select(x => x.ZnodeTaxRuleType.Name).FirstOrDefault());
            taxRuleListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return taxRuleListModel;
        }

        //Get taxrule by taxrule Id.
        public virtual TaxRuleModel GetTaxRule(int taxRuleId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (taxRuleId > 0)
                return _taxRuleRepository.GetById(taxRuleId).ToModel<TaxRuleModel>();
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return null;
        }

        //Add tax rule.
        public virtual TaxRuleModel AddTaxRule(TaxRuleModel taxRuleModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(taxRuleModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            taxRuleModel = _taxRuleRepository.Insert(taxRuleModel?.ToEntity<ZnodeTaxRule>())?.ToModel<TaxRuleModel>();
            ZnodeLogging.LogMessage(IsNotNull(taxRuleModel) ? string.Format(Admin_Resources.SuccessTaxRulesAdd, taxRuleModel.TaxRuleTypeName) : string.Format(Admin_Resources.ErrorTaxRulesAdd, taxRuleModel.TaxRuleTypeName), string.Empty, TraceLevel.Info);
            
            return taxRuleModel;
        }

        //Update taxrule.
        public virtual bool UpdateTaxRule(TaxRuleModel taxRuleModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            bool status = false;
            if (IsNull(taxRuleModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            if (taxRuleModel?.TaxRuleId > 0)
                status = _taxRuleRepository.Update(taxRuleModel.ToEntity<ZnodeTaxRule>());

            ZnodeLogging.LogMessage(status ? string.Format(Admin_Resources.SuccessAssociateRulesUpdate, taxRuleModel.TaxRuleTypeName) : string.Format(Admin_Resources.ErrorAssociateRulesUpdate, taxRuleModel.TaxRuleTypeName), string.Empty, TraceLevel.Info);
            
            return status;
        }

        //Delete tax rule.
        public virtual bool DeleteTaxRule(ParameterModel taxRuleId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            bool status = false;
            if (taxRuleId.Ids.Count() < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeTaxRuleEnum.TaxRuleId.ToString(), ProcedureFilterOperators.In, taxRuleId.Ids.ToString()));

            ZnodeLogging.LogMessage("Filters to delete tax rule: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { Filters = filters });
            status = _taxRuleRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessAssociateRulesDelete : Admin_Resources.ErrorAssociateRulesDelete, string.Empty, TraceLevel.Info);

            return status;
        }

        #endregion Tax Rule

        #region Avalara Tax
        public string TestAvalaraConnection(TaxPortalModel taxportalModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNull(taxportalModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.TaxPortalNotNull);

            //Check if Account Number, License Key or Avatax Url is null
            if (IsNull(taxportalModel.AvalaraAccount) || IsNull(taxportalModel.AvalaraLicense) || IsNull(taxportalModel.AvataxUrl))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.TaxesRequiredParameter);

            IAvataxClient avalaraTaxSales = GetService<IAvataxClient>();
            return avalaraTaxSales.RESTTestConnection(taxportalModel);
        }
        #endregion

        #endregion Public Methods

        #region Private Method

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands != null && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                    if (Equals(key, ZnodeTaxRuleEnum.ZnodeTaxRuleType.ToString().ToLower())) SetExpands(ZnodeTaxRuleEnum.ZnodeTaxRuleType.ToString(), navigationProperties);
            }
            return navigationProperties;
        }

        //Checks if SKU already exists or not.
        private bool IsTaxClassExist(string name, int? taxclassId)
            => _taxClassRepository.Table.Any(x => x.Name == name && x.TaxClassId != taxclassId);

        //Checks IsTaxClassAssociated or not.
        private bool IsTaxClassAssociated(int taxclassId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePortalTaxClassEnum.TaxClassId.ToString(), ProcedureFilterOperators.Equals, taxclassId.ToString()));
            filters.Add(new FilterTuple(ZnodePortalTaxClassEnum.IsDefault.ToString(), ProcedureFilterOperators.Equals, ZnodeConstant.TrueValue.ToString()));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get ZnodePortalTaxClass", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { WhereClause = whereClauseModel?.WhereClause });
            //Update default tax.
            ZnodePortalTaxClass znodePortalTaxClass = _taxClassPortalRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.FirstOrDefault();
            return IsNotNull(znodePortalTaxClass);
        }

        //Insert Tax class SKU.
        private void InsertTaxClassSku(TaxClassModel taxClassModel)
        {
            ZnodeLogging.LogMessage("InsertTaxClassSku method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            string[] skus = taxClassModel.ImportedSKUs.Split(',');

            TaxClassSKUListModel taxClassSKUList = new TaxClassSKUListModel();
            taxClassSKUList.TaxClassSKUList = new List<TaxClassSKUModel>();
            ZnodeLogging.LogMessage("Parameters for GetValidImportedSKUs", ZnodeLogging.Components.Admin.ToString(),TraceLevel.Verbose, new object[] { taxClassModel, skus, taxClassSKUList });
            GetValidImportedSKUs(taxClassModel, skus, taxClassSKUList);

            IEnumerable<ZnodeTaxClassSKU> list = null;
            if (taxClassSKUList?.TaxClassSKUList?.Count > 0)
                list = _taxClassSKURepository.Insert(taxClassSKUList.TaxClassSKUList.ToEntity<ZnodeTaxClassSKU>().ToList());
        }

        //Get valid Importtax class SKUs.
        private void GetValidImportedSKUs(TaxClassModel taxClassModel, string[] skus, TaxClassSKUListModel taxClassSKUList)
        {
            //Get list of valid SKUs.
            string[] validSkus = _pimAttributeValueLocaleRepository.Table
           .Join(_pimAttributeValueRepository.Table, attributeValueLocale => attributeValueLocale.PimAttributeValueId, attributeValue => attributeValue.PimAttributeValueId, (attributeValueLocale, attributeValue) => new { attributeValueLocale, attributeValue })
           .Join(_pimAttributeRepository.Table, attributeValueInfo => attributeValueInfo.attributeValue.PimAttributeId, attribute => attribute.PimAttributeId, (attributeValueInfo, attribute) => new { attributeValueInfo, attribute })
           .Where(m => m.attribute.AttributeCode == ZnodeConstant.ProductSKU).Select(m => m.attributeValueInfo.attributeValueLocale.AttributeValue).ToArray();

            foreach (string sku in skus)
            {
                //check if the sku is valid and does not already exists.
                if (validSkus.Any(x => x == sku) && (!_taxClassSKURepository.Table.Any(x => x.SKU == sku && x.TaxClassId == taxClassModel.TaxClassId)))
                    taxClassSKUList.TaxClassSKUList.Add(new TaxClassSKUModel { SKU = sku, TaxClassId = taxClassModel.TaxClassId });
            }
        }

        //Get list of SKUs those are not associated with another taxclass.
        private List<ZnodeTaxClassSKU> GetTaxClassSKUListModel(TaxClassSKUModel shippingSKUModel)
        {
            List<ZnodeTaxClassSKU> shippingSKUModelList = new List<ZnodeTaxClassSKU>();
            List<View_PimProductAttributeValue> SKUList = GetSKUList(shippingSKUModel.SKUs);
            SKUList?.ForEach(item =>
            {
                ZnodeTaxClassSKU skuModel = new ZnodeTaxClassSKU();
                skuModel.SKU = item.AttributeValue;
                skuModel.TaxClassId = shippingSKUModel.TaxClassId;
                shippingSKUModelList.Add(skuModel);
            });
            ZnodeLogging.LogMessage("shippingSKUModelList list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, shippingSKUModelList?.Count());
            return shippingSKUModelList;
        }

        //This funcation use for reset filter value of IsActive.
        private void ResetFilters(FilterCollection filters)
        {
            filters = IsNull(filters) ? new FilterCollection() : filters;
            if (IsNotNull(filters.Where(x => x.FilterName.Equals(ZnodeTaxClassEnum.IsActive.ToString().ToLower()))?.FirstOrDefault()))
                SetFilterValue(filters, ZnodeTaxClassEnum.IsActive.ToString().ToLower());
        }

        //This funcation use for set filter value.
        private void SetFilterValue(FilterCollection filters, string filterName)
        {
            FilterTuple _filterAttributeType = filters.Where(x => x.FilterName.Equals(filterName))?.FirstOrDefault();
            filters.RemoveAll(x => x.FilterName.Equals(filterName));
            if(IsNotNull(_filterAttributeType))
            {
                if (_filterAttributeType.FilterValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                    filters.Add(filterName, ProcedureFilterOperators.Equals, "1");
                else if (_filterAttributeType.FilterValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                    filters.Add(filterName, ProcedureFilterOperators.Equals, "0");
            }
        }

        //Get portal Id from filter collection.
        private int GetPortalId(FilterCollection filters)
        {
            int portalId = 0;
            if (filters.Count > 0 && (filters.Find(x => x.Item1 == FilterKeys.PortalId.ToString().ToLower())?.Item1 == FilterKeys.PortalId.ToLower()))
            {
                portalId = Convert.ToInt32(filters.Find(x => x.Item1 == FilterKeys.PortalId.ToString().ToLower())?.Item3);
                filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId.ToLower());
            }
            ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, portalId);
            return portalId;
        }

        //Get taxclassId from filter collection.
        public int GetTaxClassId(FilterCollection filters)
        {
            int taxClassId = 0;
            if (filters.Count > 0 && (filters.Find(x => x.Item1 == FilterKeys.TaxClassId.ToString().ToLower())?.Item1 == FilterKeys.TaxClassId.ToLower()))
            {
                taxClassId = Convert.ToInt32(filters.Find(x => x.Item1 == FilterKeys.TaxClassId.ToString().ToLower())?.Item3);
                filters.RemoveAll(x => x.FilterName == FilterKeys.TaxClassId.ToLower());
            }
            ZnodeLogging.LogMessage("taxClassId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, taxClassId);
            return taxClassId;
        }

        //Get IsAssociated value from filter collection.
        private bool GetIsAssociatedValue(FilterCollection filters)
        {
            bool isAssociated = false;
            if (filters.Count > 0 && (filters.Find(x => x.Item1 == FilterKeys.IsAssociated.ToString().ToLower())?.Item1 == FilterKeys.IsAssociated.ToLower()))
            {
                isAssociated = Convert.ToBoolean(filters.Find(x => x.Item1 == FilterKeys.IsAssociated.ToString().ToLower())?.Item3);
                filters.RemoveAll(x => x.FilterName == FilterKeys.IsAssociated.ToLower());
            }
            return isAssociated;
        }

        #endregion Private Method
    }
}