using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.ERPConnector
{
    //Todo working on this
    public class ZnodeMockERPConnector : BaseERP
    {
        private readonly IUserClient _userClient;
        private readonly IAccountClient _accountClient;
        private readonly ICustomerClient _customerClient;
        private readonly string currentMethodName = string.Empty;
        private readonly string currentClassName = string.Empty;
        private readonly IOrderClient _orderClient;
        private readonly IZnodeRepository<ZnodeOmsOrderState> _orderStateRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailRepository;
        private readonly IZnodeRepository<ZnodeOmsOrder> _orderRepository;
        private readonly IZnodeRepository<ZnodeImportTemplate> _importTemplateRepository;
        private readonly IZnodeRepository<ZnodeImportTemplateMapping> _importTemplateMappingRepository;
        private readonly IImportClient _importClient;
        public ZnodeMockERPConnector()
        {
            _userClient = GetClient<UserClient>();
            _accountClient = GetClient<AccountClient>();
            _customerClient = GetClient<CustomerClient>();
            //Get current class name
            currentClassName = this.GetType().Name;
            _orderRepository = new ZnodeRepository<ZnodeOmsOrder>();
            _orderClient = GetClient<OrderClient>();
            _orderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            _orderDetailRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
            _importTemplateRepository = new ZnodeRepository<ZnodeImportTemplate>();
            _importTemplateMappingRepository = new ZnodeRepository<ZnodeImportTemplateMapping>();
            _importClient = GetClient<ImportClient>();
        }

        //Refresh Product Content from ERP to Znode
        public override bool ProductRefresh()
        {
            bool status = false;
            try
            {
                //Read the csv.
                string csvPath = HttpContext.Current.Server.MapPath(string.Format(ZnodeConstant.ERPCSV_XMLFilePath, currentClassName, "Products.csv"));
                if (File.Exists(csvPath))
                {
                    //Get the import template details from template name.
                    ZnodeImportTemplateMapping importTemplateMapping = new ZnodeImportTemplateMapping();
                    ZnodeImportTemplate importTemplate = _importTemplateRepository.Table.Where(y => y.TemplateName == "Test009")?.FirstOrDefault();
                    var importMappingList = _importTemplateMappingRepository.Table.Where(x => x.ImportTemplateId == importTemplate.ImportTemplateId).ToList();
                    //Get the template mapping list.
                    List<ImportMappingModel> mappingList = (from _item in importMappingList
                                                            select new ImportMappingModel
                                                            {
                                                                MapCsvColumn = _item.SourceColumnName,
                                                                MapTargetColumn = _item.TargetColumnName,
                                                                MappingId = _item.ImportTemplateMappingId,
                                                            }).ToList();
                    //Import API call.
                    _importClient.ImportData(new ImportModel { FamilyId = GetDefaultFamilyId(false), FileName = csvPath, ImportType = ImportHeadEnum.Product.ToString(), ImportTypeId = importTemplate.ImportHeadId, LocaleId = GetDefaultLocaleId(), TemplateName = "Test009", TemplateId = importTemplate.ImportTemplateId, PortalId = 1, Mappings = new ImportMappingListModel { DataMappings = mappingList } });
                    ZnodeLogging.LogMessage("Process for product refresh completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    status = true;
                }
                else
                {
                    throw new ZnodeException(ErrorCodes.FileNotFound, "File is not available on path");
                }
            }
            catch (ZnodeException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.FileNotFound:
                        throw ex;
                    default:
                        ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                        status = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                status = false;
            }
            return status;
        }

        //Refresh Category from ERP to Znode
        public override bool CategoryRefresh()
        {
            bool status = false;
            try
            {
                //Read the csv.
                string csvPath = HttpContext.Current.Server.MapPath(string.Format(ZnodeConstant.ERPCSV_XMLFilePath, currentClassName, "Category.csv"));
                if (File.Exists(csvPath))
                {
                    //Get the import template details from template name.
                    ZnodeImportTemplateMapping importTemplateMapping = new ZnodeImportTemplateMapping();
                    ZnodeImportTemplate importTemplate = _importTemplateRepository.Table.Where(y => y.TemplateName == "MockERPCategoryTemplate")?.FirstOrDefault();
                    var importMappingList = _importTemplateMappingRepository.Table.Where(x => x.ImportTemplateId == importTemplate.ImportTemplateId).ToList();

                    //Get the template mapping list.
                    List<ImportMappingModel> mappingList = (from _item in importMappingList
                                                            select new ImportMappingModel
                                                            {
                                                                MapCsvColumn = _item.SourceColumnName,
                                                                MapTargetColumn = _item.TargetColumnName,
                                                                MappingId = _item.ImportTemplateMappingId,

                                                            }).ToList();

                    //Import API call.
                    _importClient.ImportData(new ImportModel { FamilyId = GetDefaultFamilyId(true), FileName = csvPath, ImportType = ImportHeadEnum.Category.ToString(), ImportTypeId = importTemplate.ImportHeadId, LocaleId = GetDefaultLocaleId(), TemplateName = "MockERPCategoryTemplate", TemplateId = importTemplate.ImportTemplateId, PortalId = 1, Mappings = new ImportMappingListModel { DataMappings = mappingList } });
                    ZnodeLogging.LogMessage("Process for category import completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                status = false;
            }
            return status;
        }

        //Refresh inventory for products by warehouse/DC from ERP to Znode on a scheduled basis.
        public override bool InventoryRefresh()
        {
            bool status = false;
            try
            {
                //Read the csv.
                string csvPath = HttpContext.Current.Server.MapPath(string.Format(ZnodeConstant.ERPCSV_XMLFilePath, currentClassName, "Inventory.csv"));
                if (File.Exists(csvPath))
                {
                    //Get the import template details from template name.
                    ZnodeImportTemplateMapping importTemplateMapping = new ZnodeImportTemplateMapping();
                    ZnodeImportTemplate importTemplate = _importTemplateRepository.Table.Where(y => y.TemplateName == "MockERPInventoryTemplate")?.FirstOrDefault();
                    var importMappingList = _importTemplateMappingRepository.Table.Where(x => x.ImportTemplateId == importTemplate.ImportTemplateId).ToList();

                    //Get the template mapping list.
                    List<ImportMappingModel> mappingList = (from _item in importMappingList
                                                            select new ImportMappingModel
                                                            {
                                                                MapCsvColumn = _item.SourceColumnName,
                                                                MapTargetColumn = _item.TargetColumnName,
                                                                MappingId = _item.ImportTemplateMappingId,

                                                            }).ToList();

                    //Import API call.
                    _importClient.ImportData(new ImportModel { FileName = csvPath, ImportType = ImportHeadEnum.Inventory.ToString(), ImportTypeId = importTemplate.ImportHeadId, LocaleId = GetDefaultLocaleId(), TemplateName = "MockERPInventoryTemplate", TemplateId = importTemplate.ImportTemplateId, PortalId = 1, Mappings = new ImportMappingListModel { DataMappings = mappingList } });
                    ZnodeLogging.LogMessage("Process for inventory refresh completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                status = false;
            }
            return status;

        }

        //Refresh standard price list from ERP to Znode on a scheduled basis.
        public override bool PricingStandardPriceListRefresh()
        {
            bool status = false;
            try
            {
                //Read the csv.
                string csvPath = HttpContext.Current.Server.MapPath(string.Format(ZnodeConstant.ERPCSV_XMLFilePath, currentClassName, "Pricing.csv"));
                if (File.Exists(csvPath))
                {
                    //Get the import template details from template name.
                    ZnodeImportTemplateMapping importTemplateMapping = new ZnodeImportTemplateMapping();
                    ZnodeImportTemplate importTemplate = _importTemplateRepository.Table.Where(y => y.TemplateName == "MockERPPricingTemplate")?.FirstOrDefault();
                    var importMappingList = _importTemplateMappingRepository.Table.Where(x => x.ImportTemplateId == importTemplate.ImportTemplateId).ToList();

                    //Get the template mapping list.
                    List<ImportMappingModel> mappingList = (from _item in importMappingList
                                                            select new ImportMappingModel
                                                            {
                                                                MapCsvColumn = _item.SourceColumnName,
                                                                MapTargetColumn = _item.TargetColumnName,
                                                                MappingId = _item.ImportTemplateMappingId,

                                                            }).ToList();

                    //Import API call.
                    _importClient.ImportData(new ImportModel { FileName = csvPath, ImportType = ImportHeadEnum.Pricing.ToString(), ImportTypeId = importTemplate.ImportHeadId, LocaleId = GetDefaultLocaleId(), TemplateName = "MockERPPricingTemplate", TemplateId = importTemplate.ImportTemplateId, PortalId = 1, Mappings = new ImportMappingListModel { DataMappings = mappingList }, PriceListId = 7 });
                    ZnodeLogging.LogMessage("Process for pricing refresh completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                status = false;
            }
            return status;

        }

        //Get list or customer specific price on real-time from ERP to Znode     
        [RealTime("RealTime")]
        public List<PriceSKUModel> PricingRealTime(PriceSKUModel model)
        {
            try
            {
                List<PriceSKUModel> models = new List<PriceSKUModel>();
                //Read the csv.
                string csvPath = HttpContext.Current.Server.MapPath(string.Format(ZnodeConstant.ERPCSV_XMLFilePath, currentClassName, "RealTimePricing.csv"));
                if (File.Exists(csvPath))
                {
                    DataTable dataTable = new DataTable();
                    string Fulltext;
                    using (StreamReader sr = new StreamReader(csvPath))
                    {
                        while (!sr.EndOfStream)
                        {
                            Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                            string[] rows = Fulltext.Split('\n'); //split full file text into rows  
                            var lines = Fulltext.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                            //Add the data in model from csv(the output from erp).
                            foreach (var item in lines)
                            {
                                var values = item.Split(',');
                                var inventory = new PriceSKUModel
                                {
                                    RetailPrice = Convert.ToDecimal(values[0]),
                                    SalesPrice = Convert.ToDecimal(values[1]),
                                };

                                models.Add(inventory);
                            }
                        }
                    }
                    return models;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
            }

            return null;
        }

        //Get inventory on real-time from ERP to Znode              
        [RealTime("RealTime")]
        public List<InventorySKUModel> Inventory(InventorySKUModel model)
        {
            try
            {
                List<InventorySKUModel> models = new List<InventorySKUModel>();
                //Read the csv.
                string csvPath = HttpContext.Current.Server.MapPath(string.Format(ZnodeConstant.ERPCSV_XMLFilePath, currentClassName, "RealTimeInventory.csv"));
                if (File.Exists(csvPath))
                {
                    DataTable dataTable = new DataTable();
                    string Fulltext;
                    using (StreamReader sr = new StreamReader(csvPath))
                    {
                        while (!sr.EndOfStream)
                        {
                            Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                            string[] rows = Fulltext.Split('\n'); //split full file text into rows  
                            var lines = Fulltext.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                            //Add the data in model from csv(the output from erp).
                            foreach (var item in lines)
                            {
                                var values = item.Split(',');
                                var inventory = new InventorySKUModel
                                {
                                    Quantity = Convert.ToDecimal(values[0]),
                                    ReOrderLevel = Convert.ToDecimal(values[1]),
                                };
                                models.Add(inventory);
                            }
                        }
                    }
                    return models;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
            }

            return null;
        }

        //Create Order from Znode  to ERP 
        [RealTime("RealTime")]
        public bool CreateOrder(OrderModel model)
        {
            bool status = false;
            try
            {
                //Create a csv from the received model to provide to erp on submitting an order.
                StringBuilder orderCSV = new StringBuilder();
                orderCSV.AppendLine("OrderNumber,SKU,ProductName,Price,Quantity,SalesTax,ShippingCost,BillingFirstName,BillingLastName,BillingAddress1,BillingAddress2,BillingAddress3,BillingCity,BillingState,BillingCountry,BillingPostalCode,BillingCompanyName,BillingEmailAddress,BillingDisplayName,BillingFaxNumber,ShippingFirstName,ShippingLastName,ShippingAddress1,ShippingAddress2,ShippingAddress3,ShippingCity,ShippingState,ShippingCountry,ShippingPostalCode,ShippingCompanyName,ShippingEmailAddress,ShippingDisplayName,ShippingFaxNumber");
                foreach (var item in model.OrderLineItems)
                {
                    orderCSV.AppendLine($"{model?.OrderNumber},{item?.Sku},{item?.ProductName},{item?.Price},{item?.Quantity},{item?.SalesTax},{item?.ShippingCost},{model?.BillingAddress?.FirstName},{model?.BillingAddress?.LastName},{model?.BillingAddress?.Address1},{model?.BillingAddress?.Address2},{model?.BillingAddress?.Address3},{model?.BillingAddress?.CityName},{model?.BillingAddress?.StateCode},{model?.BillingAddress?.PostalCode},{model?.BillingAddress?.CompanyName},{model?.BillingAddress?.EmailAddress},{model?.BillingAddress?.DisplayName},{model?.BillingAddress?.FaxNumber},{model?.ShippingAddress?.FirstName},{model?.ShippingAddress?.LastName},{model?.ShippingAddress?.Address1},{model?.ShippingAddress?.Address2},{model?.ShippingAddress?.Address3},{model?.ShippingAddress?.CityName},{model?.ShippingAddress?.StateCode},{model?.ShippingAddress?.PostalCode},{model?.ShippingAddress?.CompanyName},{model?.ShippingAddress?.EmailAddress},{model?.ShippingAddress?.DisplayName},{model?.ShippingAddress?.FaxNumber}");
                    WriteTextStorage(orderCSV.ToString(), HttpContext.Current.Server.MapPath(string.Format(ZnodeConstant.ERPCSV_XMLFilePath, currentClassName, "Order")) + model.OrderNumber + ".csv", Mode.Write);
                }
                status = true;
                ZnodeLogging.LogMessage("Process for creating order files completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                status = false;
            }
            return status;
        }

        //Get Order Details Status from ERP to Znode       
        public override bool OrderDetailsStatus()
        {
            bool status = false;
            try
            {
                //Read the csv file path from erp.
                string csvPath = HttpContext.Current.Server.MapPath(string.Format(ZnodeConstant.ERPCSV_XMLFilePath, currentClassName, "OrderStatus.csv"));
                List<ZnodeOmsOrderDetail> models = new List<ZnodeOmsOrderDetail>();
                using (StreamReader sr = new StreamReader(csvPath))
                {
                    while (!sr.EndOfStream)
                    {
                        string Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                        string[] rows = Fulltext.Split('\n'); //split full file text into rows  
                        string[] lines = Fulltext.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var item in lines)
                        {
                            //Map csv data in order details model and create a list.
                            MapCSVDataInOrderDetailModel(models, item);
                        }
                    }
                }

                foreach (var item in models)
                {
                    //Create ZnodeOmsOrderDetail model to be submitted for update order status.
                    ZnodeOmsOrderDetail omsOrderDetails = CreateOrderDetail(item);

                    //API call to update order status.
                    _orderClient.UpdateOrderStatus(new OrderStateParameterModel { OmsOrderId = omsOrderDetails.OmsOrderId, OmsOrderDetailsId = omsOrderDetails.OmsOrderDetailsId, TrackingNumber = item.TrackingNumber, OmsOrderStateId = item.OmsOrderStateId });
                }

                status = true;
                ZnodeLogging.LogMessage("Process for updating the order status completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        #region ECertificate
        //Get available certificate balance
        [RealTime("RealTime")]
        public ECertTotalBalanceModel AvailableECertBalance(List<ECertificateModel> eCertificates)
        {
            return new ECertTotalBalanceModel
            {
                AvailableTotal = eCertificates?.Sum(o => o.Balance) ?? 0,
                TraceMessage = $"Returning data from the connector."
            };
        }
        #endregion

        #region Private
        //Set the filters for getting the address.
        private void SetFiltersForAddress(FilterCollection filters, UserModel userModel)
        {
            if (userModel.AccountId > 0)
            {
                //If Id Already present in filters Remove It
                filters.RemoveAll(x => x.Item1 == ZnodeNoteEnum.UserId.ToString());
                //Set filters for account id.
                SetAccountIdFilters(filters, Convert.ToInt32(userModel.AccountId));
            }
            else
            {
                //If AccountId Already present in filters Remove It
                filters.RemoveAll(x => x.Item1.ToLower() == ZnodeAccountPermissionEnum.AccountId.ToString().ToLower());
                //Set filters for user id.
                SetUserIdFilters(filters, userModel.UserId);
            }
        }

        /// <summary>
        /// Sets the filter for accountId property.
        /// </summary>
        /// <param name="filters">Filters to set for accountId.</param>
        /// <param name="accountId">Value to set for accountId.</param>
        private static void SetAccountIdFilters(FilterCollection filters, int accountId)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                //Checking For AccountId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1.ToLower() == ZnodeAccountPermissionEnum.AccountId.ToString().ToLower()))
                {
                    //If AccountId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1.ToLower() == ZnodeAccountPermissionEnum.AccountId.ToString().ToLower());
                    //Add New AccountId Into filters
                    filters.Add(new FilterTuple(ZnodeAccountPermissionEnum.AccountId.ToString().ToLower(), FilterOperators.Equals, accountId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeAccountPermissionEnum.AccountId.ToString().ToLower(), FilterOperators.Equals, accountId.ToString()));
            }
        }

        /// <summary>
        /// Sets the filter for userId property.
        /// </summary>
        /// <param name="filters">Filters to set for userId.</param>
        /// <param name="userId">Value to set for userId.</param>
        private static void SetUserIdFilters(FilterCollection filters, int userId)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                //Checking for Id already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == ZnodeNoteEnum.UserId.ToString()))
                {
                    //If Id Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == ZnodeNoteEnum.UserId.ToString());

                    //Add New Id Into filter.
                    filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeNoteEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            }
        }

        //Create ZnodeOmsOrderDetail model with order state and order details id to be submitted for update order status.
        private ZnodeOmsOrderDetail CreateOrderDetail(ZnodeOmsOrderDetail item)
        {
            return (from order in _orderRepository.Table
                    join orderDetails in _orderDetailRepository.Table on order.OmsOrderId equals orderDetails.OmsOrderId
                    where order.OmsOrderId == item.OmsOrderId
                    select new
                    {
                        OmsOrderId = order.OmsOrderId,
                        OmsOrderStateId = orderDetails.OmsOrderStateId,
                        OmsOrderDetailsId = orderDetails.OmsOrderDetailsId,
                    }).AsEnumerable().Select(x => new ZnodeOmsOrderDetail()
                    {
                        OmsOrderId = x.OmsOrderId,
                        OmsOrderStateId = x.OmsOrderStateId,
                        OmsOrderDetailsId = x.OmsOrderDetailsId,
                    }).FirstOrDefault();
        }

        //Map csv data in order details model and create a list.
        private void MapCSVDataInOrderDetailModel(List<ZnodeOmsOrderDetail> models, string item)
        {
            string[] values = item.Split(',');
            string orderState = Convert.ToString(values[0].Replace("\"", ""));
            string orderNumber = Convert.ToString(values[1]);
            ZnodeOmsOrderDetail model = new ZnodeOmsOrderDetail
            {

                OmsOrderId = Convert.ToInt32(_orderRepository.Table.Where(x => x.OrderNumber == orderNumber)?.Select(x => x.OmsOrderId).FirstOrDefault()),
                OmsOrderStateId = _orderStateRepository.Table.Where(x => x.OrderStateName == orderState).Select(y => y.OmsOrderStateId).FirstOrDefault(),
                TrackingNumber = Convert.ToString(values[15]),
            };

            models.Add(model);
        }

        //Get default family id.
        private int? GetDefaultFamilyId(bool isCategory)
        {
            ZnodeRepository<ZnodePimAttributeFamily> _family = new ZnodeRepository<ZnodePimAttributeFamily>();
            return _family.Table.Where(x => x.IsDefaultFamily && x.IsCategory == isCategory)?.Select(x => x.PimAttributeFamilyId)?.FirstOrDefault();
        }

        //Get default locale id.
        private int GetDefaultLocaleId()
        {
            ZnodeRepository<ZnodeLocale> _localeRepository = new ZnodeRepository<ZnodeLocale>();
            return _localeRepository.Table.Where(x => x.IsDefault).Select(x => x.LocaleId).FirstOrDefault();
        }

        private static void WriteTextStorage(string fileData, string filePath, Mode fileMode)
        {
            try
            {
                // Create directory if not exists.
                string logFilePath = filePath;
                FileInfo fileInfo = new FileInfo(logFilePath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();

                // Check file write mode and write content.
                if (Equals(fileMode, Mode.Append))
                    File.AppendAllText(logFilePath, fileData);
                else
                    File.WriteAllText(logFilePath, fileData);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                throw;
            }
        }
        private enum Mode
        {
            Read,  // Indicates text file read mode operation.
            Write, // Indicates text file write mode operation. It deletes the previous content.
            Append  // Indicates text file append mode operation. it preserves the previous content.
        }
        #endregion
    }
}

