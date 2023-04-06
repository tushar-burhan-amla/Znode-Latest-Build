using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class WarehouseService : BaseService, IWarehouseService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeWarehouse> _warehouseRepository;
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        private readonly IZnodeRepository<ZnodeWarehouseAddress> _warehouseAddressRepository;
        #endregion

        #region Constructor
        public WarehouseService()
        {
            _warehouseRepository = new ZnodeRepository<ZnodeWarehouse>();
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _warehouseAddressRepository = new ZnodeRepository<ZnodeWarehouseAddress>();
        }
        #endregion

        #region Public Methods
        //Gets the list of Warehouse.
        public virtual WarehouseListModel GetWarehouseList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters :", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<WarehouseModel> objStoredProc = new ZnodeViewRepository<WarehouseModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            List<WarehouseModel> warehouseList = objStoredProc.ExecuteStoredProcedureList("Znode_GetWarehouseList @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT", 4, out pageListModel.TotalRowCount).ToList();
            ZnodeLogging.LogMessage("warehouseList count :", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, warehouseList?.Count);

            WarehouseListModel listModel = new WarehouseListModel();

            listModel.WarehouseList = warehouseList?.Count > 0 ? warehouseList : new List<WarehouseModel>();
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            return listModel;
        }

        //Create warehouse.
        public virtual WarehouseModel CreateWarehouse(WarehouseModel warehouseModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("warehouse with Id  : ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, warehouseModel?.WarehouseId);

            if (IsNull(warehouseModel))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ModelNotNull);

            if (IsCodeAlreadyExist(warehouseModel.WarehouseCode))
            {
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorWarehouseCodeExists);
            }

            //Create new warehouse and return it.
            ZnodeWarehouse warehouse = _warehouseRepository.Insert(warehouseModel.ToEntity<ZnodeWarehouse>());
            ZnodeLogging.LogMessage("warehouse model inserted with id ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, warehouse?.WarehouseId);

            if (warehouse?.WarehouseId > 0)
            {
                warehouseModel.Address.WarehouseId = warehouse.WarehouseId;
                //Create the Address for the Warehouse.
                ZnodeAddress address = _addressRepository.Insert(warehouseModel.Address.ToEntity<ZnodeAddress>());
                ZnodeLogging.LogMessage("Inserted address with id ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, address?.AddressId);

                if (address?.AddressId > 0)
                    _warehouseAddressRepository.Insert(new ZnodeWarehouseAddress() { WarehouseId = warehouse.WarehouseId, AddressId = address.AddressId });

                warehouseModel = warehouse.ToModel<WarehouseModel>();
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            return warehouseModel;
        }

        //Get warehouse by warehouse id.
        public virtual WarehouseModel GetWarehouse(int warehouseId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("warehouseId  : ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, warehouseId);

            if (warehouseId <= 0)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorWarehouseIdLessThanOne);
            //Get the warehouse Details based on id.
            ZnodeWarehouse warehouse = _warehouseRepository.Table.FirstOrDefault(x => x.WarehouseId == warehouseId);
            WarehouseModel warehouseModel = warehouse.ToModel<WarehouseModel>();
            if (warehouse?.WarehouseId > 0)
            {
                //Get the Default Address for the warehouse.
                ZnodeAddress defaultAddress = (from accountAddress in _warehouseAddressRepository.Table
                                               join address in _addressRepository.Table on accountAddress.AddressId equals address.AddressId
                                               where accountAddress.WarehouseId == warehouse.WarehouseId
                                               select address).FirstOrDefault();
                warehouseModel.Address = defaultAddress.ToModel<AddressModel>();
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            return warehouseModel;
        }

        //Update warehouse.
        public virtual bool UpdateWarehouse(WarehouseModel warehouseModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("warehouse with code : ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, warehouseModel?.WarehouseCode);

            bool isWarehouseUpdated = false;
            if (IsNull(warehouseModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            if (warehouseModel.WarehouseId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            if (warehouseModel?.WarehouseId > 0)
            {
                //Update warehouse
                isWarehouseUpdated = _warehouseRepository.Update(warehouseModel.ToEntity<ZnodeWarehouse>());

                if (isWarehouseUpdated)
                {
                    warehouseModel.Address.WarehouseId = warehouseModel.WarehouseId;

                    //If Address present then Update, Else Insert the Address.
                    if (warehouseModel?.Address?.AddressId > 0)
                        isWarehouseUpdated = _addressRepository.Update(warehouseModel?.Address.ToEntity<ZnodeAddress>());
                    else
                    {
                        //Create the Address for the Warehouse.
                        ZnodeAddress address = _addressRepository.Insert(warehouseModel.Address.ToEntity<ZnodeAddress>());
                        ZnodeLogging.LogMessage("Inserted address with id ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, address?.AddressId);
                        if (address?.AddressId > 0)
                            _warehouseAddressRepository.Insert(new ZnodeWarehouseAddress() { WarehouseId = warehouseModel.Address.WarehouseId, AddressId = address.AddressId });
                    }
                }
            }
            ZnodeLogging.LogMessage(isWarehouseUpdated ? string.Format(Admin_Resources.SuccessUpdateWarehouse, warehouseModel.WarehouseCode) : string.Format(Admin_Resources.ErrorUpdateWarehouse, warehouseModel?.WarehouseCode), ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
            return isWarehouseUpdated;
        }

        //Delete warehouse.
        public virtual bool DeleteWarehouse(ParameterModel warehouseIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("warehouseIds to be deleted :", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, warehouseIds?.Ids);

            if (IsNull(warehouseIds) || string.IsNullOrEmpty(warehouseIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorWarehouseIdLessThanOne);

                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter(ZnodeWarehouseEnum.WarehouseId.ToString(), warehouseIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                objStoredProc.ExecuteStoredProcedureList("Znode_DeleteWarehouse @WarehouseId,  @Status OUT", 1, out status);
                if (status == 1)
                {
                    ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessWarehouseDelete, warehouseIds.Ids), ZnodeLogging.Components.Setup.ToString(), TraceLevel.Info);
                    return true;
                }
                else
                {
                   throw new ZnodeException(ErrorCodes.AssociationDeleteError, string.Format(Admin_Resources.ErrorDeleteWarehouseDueToAssociation, warehouseIds.Ids));
                }


        }

        #region Warehouse inventory association.
        //Get list of associated inventory list.
        public virtual InventoryWarehouseMapperListModel GetAssociatedInventoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters :", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            int localeId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.Locale);

            //Checking For LocaleId exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()))
            {
                localeId = Convert.ToInt32(filters.Where(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower()).Select(x => x.FilterValue).FirstOrDefault());
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower());
            }
            IZnodeViewRepository<InventoryWarehouseMapperModel> objStoredProc = new ZnodeViewRepository<InventoryWarehouseMapperModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            IList<InventoryWarehouseMapperModel> skuInventoryEntityList = objStoredProc.ExecuteStoredProcedureList("Znode_GetSKUInventoryList @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("skuInventoryEntityList with count ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, skuInventoryEntityList?.Count);

            InventoryWarehouseMapperListModel inventorySKUListModel = new InventoryWarehouseMapperListModel { InventoryWarehouseMapperList = skuInventoryEntityList?.ToList() };
            inventorySKUListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Info);

            return inventorySKUListModel;
        }
        #endregion
        #endregion

        #region Private Method

        //Check if warehouse code is already present or not.
        private bool IsCodeAlreadyExist(string warehouseCode)
         => _warehouseRepository.Table.Any(x => x.WarehouseCode == warehouseCode);
        #endregion
    }
}
