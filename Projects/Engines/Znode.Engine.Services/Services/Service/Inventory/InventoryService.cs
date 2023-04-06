using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using Znode.Libraries.Observer;

namespace Znode.Engine.Services
{
    public class InventoryService : BaseService, IInventoryService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeInventory> _inventoryRepository;
        private readonly IZnodeRepository<ZnodePimDownloadableProduct> _pimDownladableProductRepository;
        private readonly IZnodeRepository<ZnodePimDownloadableProductKey> _downloadableProductKeyRepository;

        #endregion

        #region Constructor
        public InventoryService()
        {
            _inventoryRepository = new ZnodeRepository<ZnodeInventory>();
            _pimDownladableProductRepository = new ZnodeRepository<ZnodePimDownloadableProduct>();
            _downloadableProductKeyRepository = new ZnodeRepository<ZnodePimDownloadableProductKey>();
        }
        #endregion

        #region Public Methods

        #region SKU Inventory
        //Get sku inventory list.
        public virtual InventorySKUListModel GetSKUInventoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            string sku = filters?.Find(x => string.Equals(x.FilterName, FilterKeys.SKU, StringComparison.CurrentCultureIgnoreCase))?.FilterValue;
            ZnodeLogging.LogMessage("SKU generated :", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { sku });

            if (!string.IsNullOrEmpty(sku))
            {
                //Encode the special characters contains in sku. 
                sku = HttpUtility.HtmlEncode(sku);
                string filterOperator = filters?.Find(x => string.Equals(x.FilterName, FilterKeys.SKU, StringComparison.CurrentCultureIgnoreCase))?.FilterOperator;
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.SKU, StringComparison.CurrentCultureIgnoreCase));
                filters.Add(new FilterTuple(FilterKeys.SKU, filterOperator, sku));
            }

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            //Get Default locale.
            int localeId = GetLocaleId(filters);
            ZnodeLogging.LogMessage("pageListModel and localeId  to set SP parameters", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), localeId });

            IZnodeViewRepository<InventorySKUModel> objStoredProc = new ZnodeViewRepository<InventorySKUModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            IList<InventorySKUModel> skuInventoryEntityList = objStoredProc.ExecuteStoredProcedureList("Znode_GetSKUInventoryList @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("skuInventoryEntityList count", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { skuInventoryEntityList?.Count});

            InventorySKUListModel inventorySKUListModel = new InventorySKUListModel { InventorySKUList = skuInventoryEntityList?.ToList() };
            inventorySKUListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            return inventorySKUListModel;
        }

        //Get SKU inventory by inventory id.
        public virtual InventorySKUModel GetSKUInventory(int inventoryId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter inventoryId ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { inventoryId });

            if (inventoryId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeInventoryEnum.InventoryId.ToString(), FilterOperators.Is, inventoryId.ToString()));

                //Bind the Filter, sorts & Paging details.
                PageListModel pageListModel = new PageListModel(filters, null, null);

                //Get Default locale.
                int localeId = GetLocaleId(filters);
                ZnodeLogging.LogMessage("pageListModel and localeId  to set SP parameters", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString(), localeId });

                IZnodeViewRepository<InventorySKUModel> objStoredProc = new ZnodeViewRepository<InventorySKUModel>();
                //SP parameters
                objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
                objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
                InventorySKUModel inventorySKUListModel = objStoredProc.ExecuteStoredProcedureList("Znode_GetSKUInventoryList @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount).FirstOrDefault();

                return inventorySKUListModel;
            }

            return new InventorySKUModel();
        }

        //Create SKU inventory.
        public virtual InventorySKUModel AddSKUInventory(InventorySKUModel inventorySKUModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            if (IsNull(inventorySKUModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            if (IsDownloadableProduct(inventorySKUModel.SKU))
                inventorySKUModel.IsDownloadable = true;

            //Check whether combination of SKU and Warehouse already exists.
            if (IsSKUWarehouseCombinationExists(inventorySKUModel.SKU, inventorySKUModel.WarehouseId, inventorySKUModel.InventoryId, inventorySKUModel.IsDownloadable))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorSkuWarehouseCombination);

            string productSKU = GetSKUList(inventorySKUModel.ProductId.ToString()).FirstOrDefault()?.AttributeValue;
            ZnodeLogging.LogMessage("productSKU returned from GetSKUList method", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { productSKU });

            inventorySKUModel.SKU = IsNotNull(productSKU) ? productSKU : inventorySKUModel.SKU;

            if (inventorySKUModel.IsDownloadable)
            {
                //Set quantity for downloadable product.
                SetQuantityForDownloadableProduct(inventorySKUModel);
                inventorySKUModel.WarehouseId = null;
                inventorySKUModel = _inventoryRepository.Insert(inventorySKUModel.ToEntity<ZnodeInventory>())?.ToModel<InventorySKUModel>();
                if(IsNotNull(inventorySKUModel))
                    inventorySKUModel.IsDownloadable = true;
            }
            else
                inventorySKUModel = _inventoryRepository.Insert(inventorySKUModel.ToEntity<ZnodeInventory>())?.ToModel<InventorySKUModel>();

            if (IsNotNull(inventorySKUModel))
            {
                inventorySKUModel.PortalId = PortalId;
                var clearCacheInitializer = new ZnodeEventNotifier<InventorySKUModel>(inventorySKUModel);
            }

            ZnodeLogging.LogMessage(IsNotNull(inventorySKUModel) ? Admin_Resources.SuccessSKUInventoryAdded : Admin_Resources.ErrorSKUInventoryNotAdded,ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            return inventorySKUModel;
        }

        //Update SKU inventory.
        public virtual bool UpdateSKUInventory(InventorySKUModel inventorySKUModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("InventorySKUModel with inventoryId ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { inventorySKUModel?.InventoryId });

            bool status = false;
            if (IsNull(inventorySKUModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            inventorySKUModel.IsDownloadable = IsDownloadableProduct(inventorySKUModel?.SKU);
            ZnodeLogging.LogMessage("IsDownloadable status returned from IsDownloadableProduct: ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { inventorySKUModel?.IsDownloadable });


            //Check whether combination of SKU and Warehouse already exists.          
            if (IsSKUWarehouseCombinationExists(inventorySKUModel?.SKU, inventorySKUModel?.WarehouseId, inventorySKUModel.InventoryId, inventorySKUModel.IsDownloadable))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorSkuWarehouseCombination);

            ZnodeInventory inventory = _inventoryRepository.Table?.Where(x => x.InventoryId == inventorySKUModel.InventoryId)?.FirstOrDefault();
            ZnodeLogging.LogMessage("ZnodeInventory model with details", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { inventory });

            if (inventorySKUModel.IsDownloadable)
            {
                inventorySKUModel.WarehouseId = null;
                if (IsNotNull(inventory) && !inventorySKUModel.IsFromWarehouse)
                    inventorySKUModel.Quantity = inventory.Quantity;
                else
                    throw new ZnodeException(ErrorCodes.DuplicateProductKey, Admin_Resources.ErrorUnableToUpdateRecordAsItIsDownloadableProduct);
            }

            if (IsNull(inventorySKUModel?.SKU))
                inventorySKUModel.SKU = inventory?.SKU;

            if (inventorySKUModel?.InventoryId > 0)
                status = _inventoryRepository.Update(inventorySKUModel.ToEntity<ZnodeInventory>());

            //Clear webStore Cache on success update.
            if (status)
            {
                inventorySKUModel.PortalId = PortalId;
                var clearCacheInitializer = new ZnodeEventNotifier<InventorySKUModel>(inventorySKUModel);
            }

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessSKUInventoryUpdated : Admin_Resources.ErrorUnableToUpdateSKUInventory,ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            return status;
        }

        //Delete SKU inventory.
        public virtual bool DeleteSKUInventory(ParameterModel inventoryId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("inventoryId to be deleted: ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose,inventoryId?.Ids);

            if (inventoryId.Ids.Count() <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeInventoryEnum.InventoryId.ToString(), inventoryId.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_DeleteInventory @InventoryId,  @Status OUT", 1, out status);
            if (status == 1)
            {
                InventorySKUModel inventorySKUModel = new InventorySKUModel();
                inventorySKUModel.PortalId = PortalId;
                var clearCacheInitializer = new ZnodeEventNotifier<InventorySKUModel>(inventorySKUModel);
                
                ZnodeLogging.LogMessage(Admin_Resources.SuccessInventoryListDeleted, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorDeleteInventoryList, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDeleteInventoryList);
            }
        }

        #endregion

        #region Digital Asset
        //Add downloadable product keys.
        public virtual DownloadableProductKeyModel AddDownloadableProductKeys(DownloadableProductKeyModel downloadableProductKeyModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            if (IsNull(downloadableProductKeyModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorDownloadableProductKeyModelNull);

            List<ZnodePimDownloadableProductKey> pimDownloadableProductKey = new List<ZnodePimDownloadableProductKey>();

            int? pimDownloadableProductId = _pimDownladableProductRepository.Table?.Where(x => x.SKU == downloadableProductKeyModel.SKU)?.FirstOrDefault()?.PimDownloadableProductId;
            ZnodeLogging.LogMessage("pimDownloadableProductId", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, pimDownloadableProductId);


            if (pimDownloadableProductId <= 0 || IsNull(pimDownloadableProductId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorDownloadableProductIdLessThanOrEqualZero);

            //Get Duplicate keys of downloadable product.
            List<ZnodePimDownloadableProductKey> duplicateKeys = GetDuplicateKeysOfDownloadableProduct(downloadableProductKeyModel);
            ZnodeLogging.LogMessage("Duplicate keys of downloadable product ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, duplicateKeys);
 
            if (duplicateKeys?.Count == 0)
            {
                if (downloadableProductKeyModel?.DownloadableProductKeyList?.Count() > 0)
                {
                    ZnodeInventory znodeInventory = _inventoryRepository.Table?.Where(x => x.SKU == downloadableProductKeyModel.SKU)?.FirstOrDefault();

                    int? warehouseId = znodeInventory?.WarehouseId;
                    ZnodeLogging.LogMessage("warehouseId from ZnodeInventory model ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, warehouseId);

                    //if warehouse id is null i.e. no inventory created for this downloadbale product.
                    //create a new inventory for PIM downloadbale edit page.
                    if (IsNull(znodeInventory))
                    {
                        warehouseId = (IsNull(warehouseId) ? (new ZnodeRepository<ZnodePortalWarehouse>()).Table.FirstOrDefault(a => a.PortalId == PortalId).WarehouseId : warehouseId);
                        _inventoryRepository.Insert(new ZnodeInventory()
                        {
                            WarehouseId = warehouseId,
                            SKU = downloadableProductKeyModel.SKU,
                            Quantity = 0,
                            ReOrderLevel = 0,
                            ExternalId = null
                        });
                    }


                    foreach (DownloadableProductKeyModel item in downloadableProductKeyModel.DownloadableProductKeyList)
                        pimDownloadableProductKey.Add(new ZnodePimDownloadableProductKey { DownloadableProductKey = item.DownloadableProductKey, DownloadableProductURL = item.DownloadableProductURL, PimDownloadableProductId = Convert.ToInt32(pimDownloadableProductId), WarehouseId = warehouseId });

                    try
                    {
                        IEnumerable<ZnodePimDownloadableProductKey> downloadableProductKeys = _downloadableProductKeyRepository.Insert(pimDownloadableProductKey)?.ToList();
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                        //Set duplicate keys for downloadable products.
                        if (ex.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                            return SetDuplicateKeyForDownloadableProduct(downloadableProductKeyModel);
                    }
                }
            }
            else
            {
                downloadableProductKeyModel.DownloadableProductKeyList?.ForEach(x =>
                {
                    var _item = duplicateKeys.Where(y => y.DownloadableProductKey == x.DownloadableProductKey)?.FirstOrDefault();
                    if (!Equals(_item, null))
                    {
                        x.IsDuplicate = true;
                    }
                });
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            return downloadableProductKeyModel;
        }

        //Get list of downloadable product keys
        public virtual DownloadableProductKeyListModel GetDownloadableProductKeyList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            IZnodeViewRepository<DownloadableProductKeyModel> objStoredProc = new ZnodeViewRepository<DownloadableProductKeyModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<DownloadableProductKeyModel> productKeyList = objStoredProc.ExecuteStoredProcedureList("Znode_GetPimDownloadableProductKeyList @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("productKeyList count", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { productKeyList.Count });

            DownloadableProductKeyListModel listModel = new DownloadableProductKeyListModel { DownloadableProductKeys = productKeyList?.ToList() };
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Delete Downloadable Product Keys.
        public virtual bool DeleteDownloadableProductKeys(ParameterModel pimDownloadableProductKeyId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("PimProductId", pimDownloadableProductKeyId.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeletePimDownloadableProductKey @PimProductId, @Status OUT", 1, out status);
            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessProductDeleted, ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorDeleteProduct, ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Update Downloadable Product Key
        public virtual bool UpdateDownloadableProductKey(DownloadableProductKeyModel downloadableProductKeyModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            

            //Check if model is null.
            if (IsNull(downloadableProductKeyModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            if (ProductKeyAlreadyExists(downloadableProductKeyModel.DownloadableProductKey, downloadableProductKeyModel.PimDownloadableProductKeyId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.KeyAlreadyExists);

            ZnodeLogging.LogMessage("DownloadableProductKey for DownloadableProductKeyModel :", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, downloadableProductKeyModel?.DownloadableProductKey);

            ZnodePimDownloadableProductKey pimDownloadableProductKey = _downloadableProductKeyRepository.Table.Where(x => x.PimDownloadableProductKeyId == downloadableProductKeyModel.PimDownloadableProductKeyId)?.FirstOrDefault();
            
            if (IsNotNull(pimDownloadableProductKey))
            {
                if (pimDownloadableProductKey.IsUsed)
                    throw new ZnodeException(ErrorCodes.IsUsed, ZnodeConstant.KeyIsInUsed);

                pimDownloadableProductKey.DownloadableProductKey = downloadableProductKeyModel.DownloadableProductKey;
                pimDownloadableProductKey.DownloadableProductURL = downloadableProductKeyModel.DownloadableProductURL;
                pimDownloadableProductKey.ModifiedDate = GetDateTime();
            }            

            bool isUpdated = _downloadableProductKeyRepository.Update(pimDownloadableProductKey);

            ZnodeLogging.LogMessage(isUpdated ? Admin_Resources.SuccessProductKeyUpdate : Admin_Resources.ErrorProductKeyUpdate, ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            return isUpdated;
        }


        //Checks if product key already exists.
        private bool ProductKeyAlreadyExists(string downloadableProductKey, int pimDownloadableProductKeyId)
            => _downloadableProductKeyRepository.Table.Any(x => x.DownloadableProductKey == downloadableProductKey && x.PimDownloadableProductKeyId != pimDownloadableProductKeyId);

        #endregion

        #endregion

        #region Private Method

        //Check whether combination of SKU and Warehouse already exists.
        private bool IsSKUWarehouseCombinationExists(string SKU, int? warehouseId, int inventoryId, bool isDownloadable)
        {
            ZnodeLogging.LogMessage("Check whether combination of SKU and Warehouse already exists. ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters SKU, warehouseId, inventoryId, isDownloadable: ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { SKU, warehouseId, inventoryId, isDownloadable });

            if (isDownloadable)
                warehouseId = null;
            return inventoryId > 0 ? _inventoryRepository.Table.Any(x => x.SKU == SKU && x.WarehouseId == warehouseId && x.InventoryId != inventoryId) :
                 _inventoryRepository.Table.Any(x => x.SKU == SKU && x.WarehouseId == warehouseId);
        }
        //Check whether is downloadable product.
        private bool IsDownloadableProduct(string sku)
            => _pimDownladableProductRepository.Table.Any(x => x.SKU == sku);

        //Get locale id.
        private int GetLocaleId(FilterCollection filters)
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

        //Set duplicate keys for downloadable products.
        private DownloadableProductKeyModel SetDuplicateKeyForDownloadableProduct(DownloadableProductKeyModel downloadableProductKeyModel)
        {
            List<string> duplicatesKeys = downloadableProductKeyModel?.DownloadableProductKeyList?.GroupBy(s => s.DownloadableProductKey)
                                                          .SelectMany(grp => grp.Skip(1))?.Select(x => x.DownloadableProductKey).ToList();
            ZnodeLogging.LogMessage("duplicatesKeys count: ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { duplicatesKeys?.Count });

            downloadableProductKeyModel?.DownloadableProductKeyList.ForEach(x =>
            {
                if (duplicatesKeys.Contains(x.DownloadableProductKey))
                    x.IsDuplicate = true;
            });
            return downloadableProductKeyModel;
        }

        //Get Duplicate keys of downloadable product.
        private List<ZnodePimDownloadableProductKey> GetDuplicateKeysOfDownloadableProduct(DownloadableProductKeyModel downloadableProductKeyModel)
        {
            List<string> keys = downloadableProductKeyModel.DownloadableProductKeyList.Select(y => y.DownloadableProductKey).ToList();
            //Get duplicate keys.
            List<ZnodePimDownloadableProductKey> duplicateKeys = _downloadableProductKeyRepository.Table.Where(x => keys.Contains(x.DownloadableProductKey))?.ToList();
            ZnodeLogging.LogMessage("duplicatesKeys count: ", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, new object[] { duplicateKeys?.Count });

            return duplicateKeys;
        }

        //Set quantity for downloadable product.
        private void SetQuantityForDownloadableProduct(InventorySKUModel inventorySKUModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

            InventorySKUModel inventory = _inventoryRepository.Table?.Where(x => x.SKU == inventorySKUModel.SKU)?.FirstOrDefault().ToModel<InventorySKUModel>();
            if (inventory?.InventoryId > 0)
            {
                inventorySKUModel.Quantity = inventory.Quantity;
            }
            else
            {
                int quantityCount = (from downladableProduct in _pimDownladableProductRepository.Table
                                     join downloadableProductKey in _downloadableProductKeyRepository.Table
                                     on downladableProduct.PimDownloadableProductId equals downloadableProductKey.PimDownloadableProductId
                                     where inventorySKUModel.SKU == downladableProduct.SKU && !downloadableProductKey.IsUsed
                                     select downloadableProductKey).Count();

                inventorySKUModel.Quantity = quantityCount;
            }

            ZnodeLogging.LogMessage("Quantity for downloadable product:", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Verbose, inventorySKUModel.Quantity);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Inventory.ToString(), TraceLevel.Info);

        }
        #endregion
    }
}
