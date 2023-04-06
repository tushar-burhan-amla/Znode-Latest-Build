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

namespace Znode.Engine.Services
{
    public partial class VendorService : BaseService, IVendorService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePimVendor> _vendorRepository;
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        private readonly IZnodeRepository<ZnodePimAttribute> _pimAttributeRepository;
        private readonly IZnodeRepository<ZnodePimAttributeDefaultValue> _pimAttributeDefaultValueRepository;
        private readonly IZnodeRepository<ZnodePimAttributeDefaultValueLocale> _pimAttributeDefaultValueLocaleRepository;

        #endregion

        #region Constructor
        public VendorService()
        {
            _vendorRepository = new ZnodeRepository<ZnodePimVendor>();
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _pimAttributeDefaultValueRepository = new ZnodeRepository<ZnodePimAttributeDefaultValue>();
            _pimAttributeRepository = new ZnodeRepository<ZnodePimAttribute>();
            _pimAttributeDefaultValueLocaleRepository = new ZnodeRepository<ZnodePimAttributeDefaultValueLocale>();
        }
        #endregion

        #region Public Methods

        //Get list of Vendors.
        public virtual VendorListModel GetVendorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("GetVendorList method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<VendorModel> objStoredProc = new ZnodeViewRepository<VendorModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);
            List<VendorModel> vendorList = objStoredProc.ExecuteStoredProcedureList("Znode_GetVendorList  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("vendorList list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, vendorList?.Count());
            VendorListModel listModel = new VendorListModel();

            listModel.Vendors = vendorList?.Count > 0 ? vendorList.ToList() : new List<VendorModel>();
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Create Vendor.
        public virtual VendorModel CreateVendor(VendorModel vendorModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (IsNull(vendorModel))
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ModelNotNull);

            //Save address of vendor.
            ZnodeAddress address = _addressRepository.Insert(vendorModel.Address.ToEntity<ZnodeAddress>());
            ZnodeLogging.LogMessage("Inserted address with id ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, address?.AddressId);
            if (address?.AddressId > 0)
            {        
                vendorModel.AddressId = address.AddressId;
                ZnodePimVendor vendor = _vendorRepository.Insert(vendorModel.ToEntity<ZnodePimVendor>());
                ZnodeLogging.LogMessage("Inserted vendor with code ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, vendor?.VendorCode);
                ZnodeLogging.LogMessage((IsNotNull(vendor) ? string.Format(PIM_Resources.SuccessCreateVendor, vendorModel.VendorCode) : string.Format(PIM_Resources.ErrorCreateVendor, vendorModel.VendorCode)), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                vendorModel = vendor.ToModel<VendorModel>();
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return vendorModel;
        }

        //Get vendor details by PimVendorId.
        public virtual VendorModel GetVendor(int pimVendorId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (pimVendorId > 0)
            {
                VendorModel vendorModel = _vendorRepository.GetById(pimVendorId).ToModel<VendorModel>();
                int localeId = GetDefaultLocaleId();
                vendorModel.VendorName = (from a in _pimAttributeDefaultValueRepository.Table
                                          join b in _pimAttributeDefaultValueLocaleRepository.Table
                                          on a.PimAttributeDefaultValueId equals b.PimAttributeDefaultValueId
                                          where a.AttributeDefaultValueCode == vendorModel.VendorCode && b.LocaleId == localeId
                                          select b.AttributeDefaultValue).FirstOrDefault().ToString();
                ZnodeLogging.LogMessage("Vendor Name:", ZnodeLogging.Components.OMS.ToString(),TraceLevel.Verbose, vendorModel.VendorName);
                if (vendorModel?.PimVendorId > 0)
                {
                    //Get the Address for the vendor.
                    ZnodeAddress defaultAddress = _addressRepository.Table.Where(address => address.AddressId == vendorModel.AddressId).Select(address => address).FirstOrDefault();
                    ZnodeLogging.LogMessage("Method GetVendor-DefaultAddress:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, defaultAddress);
                    vendorModel.Address = defaultAddress.ToModel<AddressModel>();
                }

                return vendorModel;
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return null;
        }

        //Update vendor.
        public virtual bool UpdateVendor(VendorModel vendorModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool isVendorUpdated = false;
            if (IsNull(vendorModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            if (vendorModel.PimVendorId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorIDLessThanOne);

            //Update Vendor
            isVendorUpdated = _vendorRepository.Update(vendorModel.ToEntity<ZnodePimVendor>());
            if (isVendorUpdated && vendorModel?.Address.AddressId > 0)
                isVendorUpdated = _addressRepository.Update(vendorModel?.Address.ToEntity<ZnodeAddress>());

            ZnodeLogging.LogMessage(isVendorUpdated ? string.Format(PIM_Resources.SuccessUpdateVendor, vendorModel.VendorCode) : string.Format(PIM_Resources.ErrorUpdateVendor, vendorModel.VendorCode), ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isVendorUpdated;
        }

        //Delete Vendor.
        public virtual bool DeleteVendor(ParameterModel pimVendorIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(pimVendorIds?.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, PIM_Resources.ErrorVendorIdLessThanOne);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodePimVendorEnum.PimVendorId.ToString(), pimVendorIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_DeleteVendor @PimVendorId,  @Status OUT", 1, out status);

            if (status == 1)
            {
                ZnodeLogging.LogMessage(PIM_Resources.SuccessDeleteVendor, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(PIM_Resources.ErrorDeleteVendor, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, PIM_Resources.ErrorDeleteVendor);
            }
        }

        //Get vendor codes.
        public virtual VendorListModel GetVendorCodes(string attributeCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            //Filter for attribute code.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePimAttributeEnum.AttributeCode.ToString(), FilterOperators.Is, attributeCode));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Where clause in GetVendorCodes method get data", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause);
            //Filter for PimAttributeId.
            filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePimAttributeDefaultValueEnum.PimAttributeId.ToString(), FilterOperators.Equals, _pimAttributeRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.PimAttributeId.ToString()));
            whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Where clause in GetVendorCodes method get data", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause);
            _pimAttributeDefaultValueRepository.EnableDisableLazyLoading = true;

            PIMAttributeDefaultValueListModel pimAttributeDefaultValueListModel = PIMAttributesMap.ToDefaultValueListModel(_pimAttributeDefaultValueRepository.GetEntityList(whereClauseModel.WhereClause).ToList());
            if (pimAttributeDefaultValueListModel.DefaultValues?.Count > 0)
                return new VendorListModel() { VendorCodes = pimAttributeDefaultValueListModel.DefaultValues };
            else
                return null;

        }

        //Active/Inactive Vendor
        public virtual bool ActiveInactiveVendor(ActiveInactiveVendorModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            bool isVendorUpdated = false;

            if (string.IsNullOrEmpty(model.VendorId))
                throw new ZnodeException(ErrorCodes.InvalidData, PIM_Resources.ErrorVendorIdNull);

            var vendorIds = model.VendorId.Split(',').ToList();
            foreach (var item in vendorIds)
            {
                int vendorId = Convert.ToInt32(item);
                var vendorModel = _vendorRepository.Table.FirstOrDefault(x => x.PimVendorId == vendorId);

                if (vendorModel.IsActive != model.IsActive)
                {
                    vendorModel.IsActive = model.IsActive;
                    isVendorUpdated = _vendorRepository.Update(vendorModel);
                }
                else
                    isVendorUpdated = true;
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return isVendorUpdated;
        }

        //Get available vendor codes.
        public virtual VendorListModel GetAvailableVendorCodes(string attributeCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            VendorListModel vendorListModel = new VendorListModel();

            vendorListModel.VendorCodes = GetVendorCodes(attributeCode)?.VendorCodes;
            ZnodeLogging.LogMessage("Vendor Codes: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, vendorListModel.VendorCodes);
            List<string> vendorCodes = GetVendorList(null, null, null, null)?.Vendors?.Select(x => x.VendorCode)?.ToList();
            ZnodeLogging.LogMessage("vendorCodes list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, vendorCodes?.Count());
            if (vendorCodes?.Count() > 0)
                vendorListModel.VendorCodes = vendorListModel?.VendorCodes?.Where(code => !vendorCodes.Contains(code.AttributeDefaultValueCode))?.ToList();
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return vendorListModel;
        }

        #region Product Association
        //Get associate products to vendor.
        public virtual bool AssociateAndUnAssociateProduct(VendorProductModel vendorProductModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("SP parameter values: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new object[] { vendorProductModel.ProductIds, vendorProductModel.AttributeCode, Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), vendorProductModel.AttributeValue, "Method: GetLoginUserId()",null, });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter("ProductId", vendorProductModel.ProductIds, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("PimAttributeCode", vendorProductModel.AttributeCode, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("LocaleId", Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("AttributeValue", vendorProductModel.AttributeValue, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                objStoredProc.SetParameter("IsUnAssociated", vendorProductModel.IsUnAssociated, ParameterDirection.Input, DbType.Int32);
                int status = 0;
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_UpdateAttributeValue @ProductId,@PimAttributeCode,@LocaleId,@AttributeValue,@UserId, @Status OUT,@IsUnAssociated", 5, out status);
                ZnodeLogging.LogMessage("DeleteResult list count:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, deleteResult?.Count());
            if (deleteResult.FirstOrDefault().Status.Value)
                {
                if (vendorProductModel.IsUnAssociated)
                    ZnodeLogging.LogMessage(PIM_Resources.SuccessUnassociateProductInVendor, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                else
                    ZnodeLogging.LogMessage(PIM_Resources.SuccessAssociateProductInVendor, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
                }
                else
                {
                    ZnodeLogging.LogMessage(PIM_Resources.ErrorUnassociateProductInVendor, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                    return false;
                }
        }
        #endregion

        #endregion
    }
}
