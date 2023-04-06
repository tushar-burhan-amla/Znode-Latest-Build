using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Znode.Engine.ABSConnector;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.ERPConnector
{
    public class ZnodeABSConnector : BaseERP
    {
        #region Private Variables        
        private readonly IZnodeRepository<ZnodeImportHead> _importHeadRepository;
        private readonly IZnodeRepository<ZnodeOmsOrder> _orderRepository;      
        private readonly IZnodeRepository<ZnodeImportProcessLog> _importProcessLogs;
        private readonly IZnodeRepository<ZnodeImportLog> _importLogs;
        private readonly IZnodeRepository<ZnodeERPTaskScheduler> _erpTaskScheduler;
        private readonly IOrderClient _orderClient;
        private readonly IZnodeRepository<ZnodeOmsOrderState> _orderStateRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailRepository;
        #endregion

        #region Constructor
        public ZnodeABSConnector()
        {
            _importHeadRepository = new ZnodeRepository<ZnodeImportHead>();
            _importProcessLogs = new ZnodeRepository<ZnodeImportProcessLog>();
            _erpTaskScheduler = new ZnodeRepository<ZnodeERPTaskScheduler>();
            _importLogs = new ZnodeRepository<ZnodeImportLog>();
            _orderRepository = new ZnodeRepository<ZnodeOmsOrder>();
            _orderClient = GetClient<OrderClient>();
            _orderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            _orderDetailRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
        }
        #endregion

        #region Public Methods

        #region Batch Process
        //Product Refresh - Batch Process.
        public override bool ProductRefresh()
        {
            int? importHeadId = _importHeadRepository.Table.Where(y => y.Name == "Product")?.FirstOrDefault().ImportHeadId;

            bool status = false;
            Guid tableGuid = Guid.NewGuid();
            //Import PRDH.
            ImportABSData(ABSCSVNames.ProductParentLevelDetail, tableGuid, importHeadId);
            //Import PRDHA.
            ImportABSData(ABSCSVNames.ProductParenLevelAttributes, tableGuid, importHeadId);
            //Import PRDD.
            ImportABSData(ABSCSVNames.ProductSKULevelDetail, tableGuid, importHeadId);
            //Import PRDDA.
            ImportABSData(ABSCSVNames.ProductSKULevelAttributes, tableGuid, importHeadId);

            ZnodeLogging.LogMessage("Process for product refresh started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            SqlConnection conn = GetSqlConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("Znode_ImportProcessData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ImportHeadId", importHeadId);
                cmd.Parameters.AddWithValue("@TblGUID", tableGuid);
                cmd.Parameters.AddWithValue("@TouchPointName", GetCurrentMethod());
                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();
                cmd.ExecuteReader();

                status = true;
                ZnodeLogging.LogMessage("Process for product refresh completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        //Product Attribute Refresh - Batch Process.
        public bool AttributeRefresh()
        {
            bool status = false;
            Guid tableGuid = Guid.NewGuid();
            int? importHeadId = _importHeadRepository.Table.Where(y => y.Name == "ProductAttribute")?.FirstOrDefault().ImportHeadId;

            //Import MAGATTR attributes.
            ImportABSData(ABSCSVNames.Attributes, tableGuid, importHeadId);

            ZnodeLogging.LogMessage("Process for attribute refresh started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            SqlConnection conn = GetSqlConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("Znode_ImportProcessData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ImportHeadId", importHeadId);
                cmd.Parameters.AddWithValue("@TblGUID", tableGuid);
                cmd.Parameters.AddWithValue("@TouchPointName", GetCurrentMethod());
                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();
                cmd.ExecuteReader();

                status = true;
                ZnodeLogging.LogMessage("Process for attribute refresh completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        //Inventory Refresh - Batch Process.
        public override bool InventoryRefresh()
        {
            bool status = false;
            Guid tableGuid = Guid.NewGuid();
            int? importHeadId = _importHeadRepository.Table.Where(y => y.Name == "Inventory")?.FirstOrDefault().ImportHeadId;

            //Import MAGINV.
            ImportABSData(ABSCSVNames.InventoryBySKU, tableGuid, importHeadId);


            ZnodeLogging.LogMessage("Process for inventory refresh started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            SqlConnection conn = GetSqlConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("Znode_ImportProcessData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ImportHeadId", importHeadId);
                cmd.Parameters.AddWithValue("@TblGUID", tableGuid);
                cmd.Parameters.AddWithValue("@TouchPointName", GetCurrentMethod());
                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();
                cmd.ExecuteReader();

                status = true;

                ZnodeLogging.LogMessage("Process for inventory refresh completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        //Inventory Refresh - Batch Process.
        public override bool PricingStandardPriceListRefresh()
        {
            bool status = false;
            Guid tableGuid = Guid.NewGuid();
            int? importHeadId = _importHeadRepository.Table.Where(y => y.Name == "Pricing")?.FirstOrDefault().ImportHeadId;

            //Import Price.
            ImportABSData(ABSCSVNames.Pricing, tableGuid, importHeadId);


            ZnodeLogging.LogMessage("Process for price refresh started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            SqlConnection conn = GetSqlConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("Znode_ImportProcessData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ImportHeadId", importHeadId);
                cmd.Parameters.AddWithValue("@TblGUID", tableGuid);
                cmd.Parameters.AddWithValue("@TouchPointName", GetCurrentMethod());
                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();
                cmd.ExecuteReader();

                status = true;
                ZnodeLogging.LogMessage("Process for price refresh completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        //Category Refresh - Batch Process.
        public override bool CategoryRefresh()
        {
            int? importHeadId = _importHeadRepository.Table.Where(y => y.Name == "Category")?.FirstOrDefault().ImportHeadId;

            bool status = false;
            Guid tableGuid = Guid.NewGuid();

            //Import PRDHA/Category.
            ImportABSData(ABSCSVNames.ProductParenLevelAttributes, tableGuid, importHeadId);
            ImportABSData(ABSCSVNames.ProductSKULevelDetail, tableGuid, importHeadId);

            ZnodeLogging.LogMessage("Process for category refresh started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            SqlConnection conn = GetSqlConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("Znode_ImportProcessData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ImportHeadId", importHeadId);
                cmd.Parameters.AddWithValue("@TblGUID", tableGuid);
                cmd.Parameters.AddWithValue("@TouchPointName", GetCurrentMethod());
                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();
                cmd.ExecuteReader();

                status = true;
                ZnodeLogging.LogMessage("Process for category refresh completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        //Customer Refresh - Batch Process.
        public override bool CustomerDetailsRefresh()
        {
            bool status = false;
            Guid tableGuid = Guid.NewGuid();
            int? importHeadId = _importHeadRepository.Table.Where(y => y.Name == "CustomerAddress")?.FirstOrDefault().ImportHeadId;

            //Import MAGSOLD.
            ImportABSData(ABSCSVNames.CustomerSoldTo, tableGuid, importHeadId);
            //Import MAGSHIP.
            ImportABSData(ABSCSVNames.CustomerShipTo, tableGuid, importHeadId);

            ZnodeLogging.LogMessage("Process for customer refresh started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            SqlConnection conn = GetSqlConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("Znode_ImportProcessData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ImportHeadId", importHeadId);
                cmd.Parameters.AddWithValue("@TblGUID", tableGuid);
                cmd.Parameters.AddWithValue("@TouchPointName", GetCurrentMethod());
                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();
                cmd.ExecuteReader();

                status = true;
                ZnodeLogging.LogMessage("Process for customer refresh completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        //Order status update.
        public bool OrderStatusUpdate()
        {
            bool status = false;
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "OrderStatusUpdate")?.FirstOrDefault()?.ERPTaskSchedulerId });
            try
            {
                //Read the csv file path.
                string csvPath = GetCSVFilePath(ABSCSVNames.LineDetailStatusInformation);
                string Fulltext;
                List<ZnodeOmsOrderDetail> models = new List<ZnodeOmsOrderDetail>();
                using (StreamReader sr = new StreamReader(csvPath))
                {
                    while (!sr.EndOfStream)
                    {
                        Fulltext = sr.ReadToEnd().ToString(); //read full file text 
                        var lines = Fulltext.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var item in lines)
                        {
                            var values = item.Split(',');
                            string orderState = Enum.GetName(typeof(ABSOrderStateEnum), Convert.ToChar(values[0].Replace("\"", "")));
                            var model = new ZnodeOmsOrderDetail
                            {
                                OmsOrderId = Convert.ToInt32(values[1]),
                                OmsOrderStateId = _orderStateRepository.Table.Where(x => x.OrderStateName == orderState).Select(y => y.OmsOrderStateId).FirstOrDefault(),
                                TrackingNumber = Convert.ToString(values[15]),
                            };

                            models.Add(model);
                        }
                    }
                }

                foreach (var item in models)
                {
                    string orderId = Convert.ToString(item.OmsOrderId);
                    var details = (from order in _orderRepository.Table
                                   join orderDetails in _orderDetailRepository.Table on order.OmsOrderId equals orderDetails.OmsOrderId
                                   where order.OrderNumber == orderId
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

                    _orderClient.UpdateOrderStatus(new OrderStateParameterModel { OmsOrderId = details.OmsOrderId, OmsOrderDetailsId = details.OmsOrderDetailsId, TrackingNumber = item.TrackingNumber, OmsOrderStateId = item.OmsOrderStateId });
                }

                status = true;
                _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                ZnodeLogging.LogMessage("Process for updating the order status completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "53", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        //Create order files.
        public override bool OrderCreate()
        {
            bool status = false;
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "OrderCreate")?.FirstOrDefault()?.ERPTaskSchedulerId });
            try
            {
                ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
                DataSet dataSet = objStoredProc.GetSPResultInDataSet("Znode_GetOrderDetails");

                List<OrderModel> orderDetails = dataSet.Tables[0].AsEnumerable().Select(m => new OrderModel()
                {
                    OmsOrderId = m.Field<int>("OmsOrderID"),
                    OrderNumber = m.Field<string>("OrderNumber"),
                    PortalId = m.Field<int>("PortalId"),
                    UserId = m.Field<int>("UserId"),
                    OmsOrderStateId = m.Field<int>("OmsOrderStateId"),
                    ShippingId = m.Field<int>("ShippingID"),
                    PaymentTypeId = m.Field<int>("PaymentTypeId"),
                    TaxCost = Math.Round(m.Field<decimal>("TaxCost"), 2),
                    ShippingCost = Math.Round(m.Field<decimal>("ShippingCost"), 2),
                    SubTotal = Math.Round(m.Field<decimal>("SubTotal"), 2),
                    DiscountAmount = Math.Round(m.Field<decimal>("DiscountAmount"), 2),
                    Total = Math.Round(m.Field<decimal>("Total"), 2),
                    OrderDate = m.Field<DateTime>("OrderDate"),
                    ShippingNumber = m.Field<string>("ShippingNumber"),
                    TrackingNumber = m.Field<string>("TrackingNumber"),
                    CouponCode = m.Field<string>("CouponCode"),
                    PurchaseOrderNumber = m.Field<string>("PurchaseOrderNumber"),
                    ShipDate = m.Field<DateTime>("ShipDate"),
                    ReturnDate = m.Field<DateTime>("ReturnDate"),
                    OmsOrderDetailsId = m.Field<int>("OmsOrderDetailsId"),

                    BillingAddress = new AddressModel()
                    {
                        Address1 = m.Field<string>("BillingStreet1"),
                        Address2 = m.Field<string>("BillingStreet2"),
                        CityName = m.Field<string>("BillingCity"),
                        StateCode = m.Field<string>("BillingStateCode"),
                        PostalCode = m.Field<string>("BillingPostalCode"),
                        EmailAddress = m.Field<string>("BillingEmailId"),
                        PhoneNumber = m.Field<string>("BillingPhoneNumber"),
                    },
                    ShippingAddress = new AddressModel()
                    {
                        Address1 = m.Field<string>("ShipToStreet1"),
                        Address2 = m.Field<string>("ShipToStreet2"),
                        FirstName = m.Field<string>("ShipToFirstName"),
                        LastName = m.Field<string>("ShipToLastName"),
                        CityName = m.Field<string>("ShipToCity"),
                        StateCode = m.Field<string>("ShipToStateCode"),
                        PostalCode = m.Field<string>("ShipToPostalCode"),
                        PhoneNumber = m.Field<string>("ShipToPhoneNumber"),
                        EmailAddress = m.Field<string>("ShipToEmailId"),
                    }
                }).ToList();

                string lastOrderFilePath = HttpContext.Current.Server.MapPath($"{ConfigurationManager.AppSettings["LastOrderFilePath"]}/Temp.csv");
                string lastOrderId = string.Empty;
                using (StreamReader sr = new StreamReader(lastOrderFilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        lastOrderId = sr.ReadToEnd().ToString(); //read full file text  
                    }
                }
                if (!string.IsNullOrEmpty(lastOrderId))
                {
                    orderDetails = orderDetails.Where(x => x.OmsOrderId > Convert.ToInt32(lastOrderId)).ToList();
                }
                List<OrderLineItemModel> orderLines = dataSet.Tables[1].AsEnumerable().Select(m => new OrderLineItemModel()
                {
                    OmsOrderDetailsId = m.Field<int>("OmsOrderDetailsId"),
                    Quantity = Math.Round(m.Field<decimal>("Quantity"), 2),
                    Price = Math.Round(m.Field<decimal>("Price"), 2),
                    TrackingNumber = m.Field<string>("TrackingNumber"),
                    Sku = m.Field<string>("Sku"),
                    ProductName = m.Field<string>("ProductName"),
                    ShipDate = m.Field<DateTime>("ShipDate"),
                    ShippingCost = Math.Round(m.Field<decimal>("ShippingCost"), 2),
                }).ToList();

                orderDetails.ForEach(d =>
                {
                    var si = orderLines
                                .Where(s => s.OmsOrderDetailsId == d.OmsOrderDetailsId);

                    d.OrderLineItems = si != null ? si.ToList() : null;
                });

                if (orderDetails.Count > 0)
                {
                    int lastOmsOrderId = orderDetails.Last().OmsOrderId;
                    WriteTextStorage(lastOmsOrderId.ToString(), HttpContext.Current.Server.MapPath($"{ConfigurationManager.AppSettings["LastOrderFilePath"]}/Temp.csv"), Mode.Write);

                    CreateMAGOD(orderDetails);
                    CreateMAGOH(orderDetails);
                    CreateMAGOS(orderDetails);
                }
                status = true;
                _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                ZnodeLogging.LogMessage("Process for creating order files completed successfully.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return status;
            }
            catch (Exception ex)
            {
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "52", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }
        #endregion

        #region Real Time Process
        //AR List - Real time process.
        public bool ARList()
        {
            ABSConnector.ABSConnector _abs = new ABSConnector.ABSConnector();

            ABSARListRequestModel param = new ABSARListRequestModel();
            bool status = false;
            ABSMapper.MapABSARListRequestModel(param);

            //Log the started message.
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "ARList")?.FirstOrDefault()?.ERPTaskSchedulerId });
            ZnodeLogging.LogMessage("Real time process for AR List started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            try
            {
                List<ABSARListResponseModel> _model = _abs.GetResponse<List<ABSARListResponseModel>, ABSARListRequestModel>(GetDestinationUrl("ARListPort"), param, ABSRequestTypes.ARList);

                //Log the success message on completion of real time process.
                if (_model.Count > 0)
                {
                    _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                    ZnodeLogging.LogMessage("Real time process for AR List completed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                }
                status = true;
            }
            catch (Exception ex)
            {
                //Log the failure message on failing the real time process.
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });

                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "48", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });

                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning, ex);
                status = false;
            }

            return status;
        }

        //AR Payment - Real time process.
        [RealTime("RealTime")]
        public bool ARPayment(OrderModel model)
        {
            ABSConnector.ABSConnector _abs = new ABSConnector.ABSConnector();

            bool status = false;

            //Log the started message.
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "ARPayment")?.FirstOrDefault()?.ERPTaskSchedulerId });

            try
            {
                List<ABSARPaymentResponseModel> _model = _abs.GetResponse<List<ABSARPaymentResponseModel>, ABSARPaymentRequestModel>(GetDestinationUrl("ARPaymentPort"), ABSMapper.MapABSARPaymentRequestModel(model), ABSRequestTypes.ARPayment);
                //Log the success message on completion of real time process.
                if (_model.Count > 0)
                {
                    _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                    ZnodeLogging.LogMessage("Real time process for AR Payment completed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                }
                status = true;
            }
            catch (Exception ex)
            {
                //Log the failure message on failing the real time process.
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });

                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "48", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });

                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning, ex);
                status = false;
            }

            return status;
        }

        //Inventory - Real time process.
        [RealTime("RealTime")]
        public List<InventorySKUModel> Inventory(InventorySKUModel model)
        {
            ABSConnector.ABSConnector _abs = new ABSConnector.ABSConnector();

            //Log the started message.
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "Inventory")?.FirstOrDefault()?.ERPTaskSchedulerId });
            try
            {
                List<ABSInventoryResponseModel> _model = _abs.GetResponse<List<ABSInventoryResponseModel>, ABSInventoryRequestModel>(GetDestinationUrl("InventoryRequestPort"), ABSMapper.MapABSInventoryRequestModel(model), ABSRequestTypes.Inventory);

                List<InventorySKUModel> inventoryModel;

                inventoryModel = (from item in _model
                                  select new InventorySKUModel
                                  {
                                      Quantity = item.InventoryQty,
                                  }).ToList();

                //Log the success message on completion of real time process.
                if (_model.Count > 0)
                {
                    _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                    ZnodeLogging.LogMessage("Real time process for inventory completed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                }
                return inventoryModel;
            }
            catch (Exception ex)
            {
                //Log the failure message on failing the real time process.
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });

                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "48", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });

                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning, ex);
            }

            return null;
        }

        //PDF Print - Real time process.
        [RealTime("RealTime")]
        public bool PDFPrint()
        {
            ABSConnector.ABSConnector _abs = new ABSConnector.ABSConnector();

            ABSPrintInfoRequestModel param = new ABSPrintInfoRequestModel();
            bool status = false;
            ABSMapper.MapABSPrintInfoRequestModel(param);

            //Log the started message.
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "PDFPrint")?.FirstOrDefault()?.ERPTaskSchedulerId });

            try
            {
                List<ABSPrintInfoResponseModel> _model = _abs.GetResponse<List<ABSPrintInfoResponseModel>, ABSPrintInfoRequestModel>(GetDestinationUrl("PDFPrintPort"), param, ABSRequestTypes.PDFPrint);
                //Log the success message on completion of real time process.
                if (_model.Count > 0)
                {
                    _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                    ZnodeLogging.LogMessage("Real time process for PDF print completed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                }
                status = true;
            }
            catch (Exception ex)
            {
                //Log the failure message on failing the real time process.
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });

                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "48", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });

                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning, ex);
                status = false;
            }

            return status;
        }

        //Email Update - Real time process.
        [RealTime("RealTime")]
        public ABSEmptyResponseModel EmailUpdate(ZnodeUser model)
        {
            ABSConnector.ABSConnector absConnector = new ABSConnector.ABSConnector();

            //Log the started message.
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "EmailUpdate")?.FirstOrDefault()?.ERPTaskSchedulerId });

            try
            {
                ABSEmptyResponseModel _model = absConnector.GetResponse<ABSEmptyResponseModel, ABSEmailRequestModel>(GetDestinationUrl("EmailAddressPort"), ABSMapper.MapABSEmailRequestModel(model), ABSRequestTypes.Email);
                //Log the success message on completion of real time process.
                if (_model.Success)
                {
                    _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                    ZnodeLogging.LogMessage("Real time process for email update completed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    return _model;
                }
            }
            catch (Exception ex)
            {
                //Log the failure message on failing the real time process.
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });

                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "48", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });

                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning, ex);

            }
            return null;
        }

        //Bill To Address Update - Real time process.
        [RealTime("RealTime")]
        public ABSEmptyResponseModel BillToAddressUpdate(ZnodeAddress model)
        {
            ABSConnector.ABSConnector _abs = new ABSConnector.ABSConnector();

            //Log the started message.
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "BillToAddressUpdate")?.FirstOrDefault()?.ERPTaskSchedulerId });

            try
            {
                ABSEmptyResponseModel _model = _abs.GetResponse<ABSEmptyResponseModel, ABSBillToChangeRequestModel>(GetDestinationUrl("BillToAddressPort"), ABSMapper.MapABSBillToChangeRequestModel(model), ABSRequestTypes.BillToAddress);
                //Log the success message on completion of real time process.
                if (_model.Success)
                {
                    _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                    ZnodeLogging.LogMessage("Real time process for bill to change update completed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    return _model;
                }
            }
            catch (Exception ex)
            {
                //Log the failure message on failing the real time process.
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });

                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "48", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });

                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning, ex);

            }
            return null;
        }

        //Get Order Number - Real time process.
        [RealTime("RealTime")]
        public string GetOrderNumber(SubmitOrderModel model)
        {
            ABSConnector.ABSConnector _abs = new ABSConnector.ABSConnector();

            //Log the started message.
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "GetOrderNumber")?.FirstOrDefault()?.ERPTaskSchedulerId });
            try
            {
                List<ABSOrderNumberResponseModel> _model = _abs.GetResponse<List<ABSOrderNumberResponseModel>, ABSOrderNumberRequestModel>(GetDestinationUrl("OrderNumberPort"), ABSMapper.MapABSOrderNumberRequestModel(model), ABSRequestTypes.OrderNumber);
                //Log the success message on completion of real time process.
                if (_model.Count > 0)
                {
                    _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                    ZnodeLogging.LogMessage("Real time process for bill to change update completed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    return _model?.FirstOrDefault()?.NextOrderNumber;
                }
            }
            catch (Exception ex)
            {
                //Log the failure message on failing the real time process.
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });

                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "48", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });

                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning, ex);

            }
            return null;
        }

        //Tier Price change - Real time process.
        public override bool GetPricing()
        {
            ABSConnector.ABSConnector _abs = new ABSConnector.ABSConnector();

            ABSTierPriceRequestModel param = new ABSTierPriceRequestModel();
            bool status = false;
            ABSMapper.MapABSPriceRequestModel(param);

            //Log the started message.
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "PricingRealTime")?.FirstOrDefault()?.ERPTaskSchedulerId });
            try
            {
                ABSEmptyResponseModel _model = _abs.GetResponse<ABSEmptyResponseModel, ABSTierPriceRequestModel>(GetDestinationUrl("PriceTierPort"), param, ABSRequestTypes.PriceTier);
                //Log the success message on completion of real time process.
                if (_model.Success)
                {
                    _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                    ZnodeLogging.LogMessage("Real time process for pricing completed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                }
                status = true;
            }
            catch (Exception ex)
            {
                //Log the failure message on failing the real time process.
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });

                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "48", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });

                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning, ex);
                status = false;
            }

            return status;
        }

        //Get Order Number - Real time process.
        [RealTime("RealTime")]
        public List<ABSOrderResponseDetailsModel> OrderInformation()
        {
            ABSConnector.ABSConnector _abs = new ABSConnector.ABSConnector();
            ABSOrderInfoRequestModel param = new ABSOrderInfoRequestModel();
            ABSMapper.MapABSOrderInfoRequestModel(param);
            //Log the started message.
            ZnodeImportProcessLog importProcessLogEntity = _importProcessLogs.Insert(new ZnodeImportProcessLog { Status = "Started", ImportTemplateId = null, ProcessStartedDate = DateTime.Now, ProcessCompletedDate = null, ERPTaskSchedulerId = _erpTaskScheduler.Table.Where(x => x.TouchPointName == "OrderInformation")?.FirstOrDefault()?.ERPTaskSchedulerId });

            try
            {
                List<ABSOrderResponseDetailsModel> _model = _abs.GetResponse<List<ABSOrderResponseDetailsModel>, ABSOrderInfoRequestModel>(GetDestinationUrl("OrderHistoryPort"), param, ABSRequestTypes.OrderHistory);
                //Log the success message on completion of real time process.
                if (_model.Count > 0)
                {
                    _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Completed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });
                    ZnodeLogging.LogMessage("Real time process for order info completed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    return _model;
                }
            }
            catch (Exception ex)
            {
                //Log the failure message on failing the real time process.
                bool importProcessLogUpdated = _importProcessLogs.Update(new ZnodeImportProcessLog { Status = "Failed", ProcessStartedDate = importProcessLogEntity.ProcessStartedDate, ImportTemplateId = importProcessLogEntity.ImportTemplateId, ProcessCompletedDate = DateTime.Now, ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ERPTaskSchedulerId = importProcessLogEntity.ERPTaskSchedulerId });

                if (importProcessLogUpdated)
                    _importLogs.Insert(new ZnodeImportLog { ImportProcessLogId = importProcessLogEntity.ImportProcessLogId, ErrorDescription = "48", DefaultErrorValue = ex.Message, ColumnName = null, RowNumber = null, Data = null, Guid = null });

                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning, ex);

            }
            return null;
        }
        #endregion

        #endregion

        #region Private Methods
        
        private void ReadRowsWithoutHeader(DataTable dataTable, string[] rows, string headers, string fileLocation)
        {
            string[] headerArray = headers?.Split(',');

            for (int j = 0; j < headerArray?.Count(); j++)
                dataTable.Columns.Add(headerArray[j]); //add headers  

            for (int i = 0; i < rows.Count(); i++)
            {

                string[] rowValues = (fileLocation == "TPRICE") || (fileLocation == "MAGINV") ? rows[i].Split(',') : (fileLocation == "MAGSOLD") || (fileLocation == "MAGSHIP") ? rows[i].Split('|') : rows[i].Split('~'); //split each row with comma or tilde to get individual values  
                {
                    DataRow dr = dataTable.NewRow();
                    for (int k = 0; k < rowValues.Count(); k++)
                        dr[k] = Convert.ToString(rowValues[k].Replace('"', ' ').Trim());

                    dataTable.Rows.Add(dr); //add other rows  
                }
            }
        }

        //Get the port values.
        private string GetDestinationUrl(string portName)
        {
            NameValueCollection nameValueColl = new NameValueCollection();
            string Json = File.ReadAllText(HttpContext.Current.Server.MapPath(ZnodeConstant.ERPConnectionfilePath));
            JObject obj = JObject.Parse(Json);
            foreach (var item in obj[ZnodeConstant.ERPConnectorControlList])
                nameValueColl.Add(item[ZnodeConstant.Name].ToString(), DecryptData(item[ZnodeConstant.Value].ToString()));
            if (portName.Equals("ABSFolderPath"))
                return $"{ nameValueColl["ABSFolderPath"]}";
            if (portName.Equals("FTPPath") || portName.Equals("FTPUsername") || portName.Equals("FTPPassword"))
                return $"{ nameValueColl[portName]}";
            return $"http://63.254.157.29:{nameValueColl[portName]}";
        }

        private string GetCSVFilePath(string fileName)
        => $"{GetDestinationUrl("ABSFolderPath")}/{fileName}.csv";

        //Decrypt the data.
        private static string DecryptData(string data) => !string.IsNullOrEmpty(data) ? new ZnodeEncryption().DecryptData(data) : null;

        //Import ABS related files.
        private void ImportABSData(string fileLocation, Guid tableGuid, int? importHeadId)
        {
            //Read the csv file path.
            string csvPath = (GetCSVFilePath(fileLocation));

            DataTable dataTable = new DataTable();
            string Fulltext;
            using (StreamReader sr = new StreamReader(csvPath))
            {
                IZnodeViewRepository<string> objStoredProc = new ZnodeViewRepository<string>();
                //Get the headers of current csv.
                var headers = objStoredProc.ExecuteStoredProcedureList($"Znode_ImportGetTableDetails {fileLocation},{importHeadId}");
                while (!sr.EndOfStream)
                {
                    Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                    string[] rows = Fulltext.Split('\n'); //split full file text into rows  

                    ReadRowsWithoutHeader(dataTable, rows, headers.FirstOrDefault(), fileLocation);
                }
                string uniqueIdentifier = string.Empty;
                //Create hash table for the respective CSVs.
                ABSReadAndCreateTable(out uniqueIdentifier, headers.FirstOrDefault(), dataTable, tableGuid, fileLocation);
            }
        }

        //Create hash tables in database for ABS files import.
        private string ABSReadAndCreateTable(out string uniqueIdentifier, string headers, DataTable dt, Guid tableGuid, string csvName)
        {
            string tableName = string.Empty;
            uniqueIdentifier = string.Empty;
            string[] columnNames = headers?.Split(',')?.ToArray();

            if (columnNames?.Count() > 0)
            {
                //generate the double hash temp table name
                tableName = $"tempdb..[##{csvName}_{tableGuid}]";

                //add the same guid in datatable
                DataColumn guidCol = new DataColumn("guid", typeof(String));
                guidCol.DefaultValue = tableGuid;
                dt.Columns.Add(guidCol);

                //create the double hash temp table
                MakeTable(tableName, GetTableColumnsFromFirstLine(columnNames));

                //assign the guid to out parameter
                uniqueIdentifier = Convert.ToString(tableGuid);
            }

            //If table created and it has headers then dump the CSV data in double hash temp table
            if (!string.IsNullOrEmpty(tableName) && columnNames.Any())
                SaveDataInChunk(dt, tableName);

            return tableName;
        }

        //This method will create ##temp table in SQL
        private void MakeTable(string tableName, string columnList)
        {
            SqlConnection conn = GetSqlConnection();

            SqlCommand cmd = new SqlCommand("Znode_CreateTempTable", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tableName", tableName);
            cmd.Parameters.AddWithValue("@columnList", columnList);

            if (conn.State.Equals(ConnectionState.Closed))
                conn.Open();

            cmd.ExecuteNonQuery();
        }

        //This method will create the table columns and append the datatype to the column headers
        private string GetTableColumnsFromFirstLine(string[] firstLine)
        {
            StringBuilder sbColumnList = new StringBuilder();
            sbColumnList.Append("(");
            sbColumnList.Append(string.Join(" nvarchar(max) , ", firstLine));
            sbColumnList.Append(" nvarchar(max), guid nvarchar(max) )");
            return sbColumnList.ToString();
        }

        //This method will divide the data in chunk.  The chunk size is 5000.
        private void SaveDataInChunk(DataTable dt, string tableName)
        {
            if (HelperUtility.IsNotNull(dt) && dt.Rows?.Count > 0)
            {
                int chunkSize = int.Parse(ConfigurationManager.AppSettings["ZnodeImportChunkLimit"].ToString());
                int startIndex = 0;
                int totalRows = dt.Rows.Count;
                int totalRowsCount = totalRows / chunkSize;

                if (totalRows % chunkSize > 0)
                    totalRowsCount++;

                for (int iCount = 0; iCount < totalRowsCount; iCount++)
                {
                    DataTable fileData = dt.Rows.Cast<DataRow>().Skip(startIndex).Take(chunkSize).CopyToDataTable();
                    startIndex = startIndex + chunkSize;
                    InsertData(tableName, fileData);
                }
            }
        }

        //This method will save the chunk data in ##temp table using Bulk upload
        private void InsertData(string tableName, DataTable fileData)
        {
            SqlConnection conn = GetSqlConnection();
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.DestinationTableName = tableName;

                if (conn.State.Equals(ConnectionState.Closed))
                    conn.Open();

                bulkCopy.WriteToServer(fileData);
                conn.Close();
            }
        }

        //This method will provide the SQL connection
        private SqlConnection GetSqlConnection() => new SqlConnection(HelperMethods.ConnectionString);

        /// <summary>
        /// Writes text file to persistant storage.
        /// </summary>
        /// <param name="fileData">Specify the string that has the file content.</param>
        /// <param name="filePath">Specify the relative file path.</param>
        /// <param name="fileMode">Specify the file write mode operatation. </param>
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

        //Create order ship to file to  be submitted to ABS with the name as ORDSHP.
        private void CreateMAGOS(List<OrderModel> model)
        {
            StringBuilder MAGOS = new StringBuilder();
            foreach (var item in model)
                MAGOS.AppendLine($"{item.OrderNumber},0,0,{item.ShippingAddress?.FirstName},{item.ShippingAddress?.LastName},{item.ShippingAddress?.Address1},{item.ShippingAddress?.Address2},{item.ShippingAddress?.CityName},{item.ShippingAddress?.StateCode},{item.ShippingAddress?.PostalCode},{item.ShippingTypeName},0,{item.TaxCost},0,,,{item.ShippingAddress?.Address3}");
            string dateString = $"{DateTime.Now:yyyyMMdd-HH.mm.ss}";
            WriteTextStorage(MAGOS.ToString(), $"{ConfigurationManager.AppSettings["ABSOrderFilePath"]}/ToABS/ORDSHP-" + dateString + ".csv", Mode.Write);

        }

        //Create order header file to  be submitted to ABS with the name as ORDHDR.
        private void CreateMAGOH(List<OrderModel> model)
        {
            StringBuilder MAGOH = new StringBuilder();
            foreach (var item in model)
                MAGOH.AppendLine($"{item.OrderNumber},0,{item.OrderNumber},,,,,,,,,,,,{item.BillingAddress.EmailAddress},{item.BillingAddress.FirstName},{item.BillingAddress.AddressId},{item.BillingAddress.Address2},{item.BillingAddress.CityName},{item.BillingAddress.StateCode},{item.BillingAddress.PostalCode},{item.BillingAddress.CountryName},{item?.OrderDate.Date.ToString("yyyyMMdd")},{item?.OrderDate.TimeOfDay.ToString("hhmmss")},0,0,0,0,0,0,{item.Total},{item.TaxCost},,{item.PaymentType},{item.CreditCardNumber},,,,0,,,,,,,,,{item.BillingAddress.Address3},,,,,{item.PurchaseOrderNumber},{item.CouponCode}");
            string dateString = $"{DateTime.Now:yyyyMMdd-HH.mm.ss}";
            WriteTextStorage(MAGOH.ToString(), $"{ConfigurationManager.AppSettings["ABSOrderFilePath"]}/ToABS/ORDHDR-" + dateString + ".csv", Mode.Write);

        }

        //Create order per item sku file to  be submitted to ABS with the name as ORDDTL.
        private void CreateMAGOD(List<OrderModel> model)
        {
            StringBuilder MAGOD = new StringBuilder();

            foreach (var item in model)
            {
                if (!Equals(item, null))
                {
                    foreach (var lineItems in item.OrderLineItems)
                        if (!Equals(lineItems, null))
                            MAGOD.AppendLine($"{item.OrderNumber},0,{lineItems.Sku},{lineItems.Sku},{lineItems.Quantity},{lineItems.Price},,,,0,0,0,0,0,0,{lineItems.ShipDate.Value.Date.ToString("yyyyMMdd")},{lineItems.TrackingNumber},0,,,,");
                }
            }
            string dateString = $"{DateTime.Now:yyyyMMdd-HH.mm.ss}";
            WriteTextStorage(MAGOD.ToString(), $"{ConfigurationManager.AppSettings["ABSOrderFilePath"]}/ToABS/ORDDTL-" + dateString + ".csv", Mode.Write);

        }

        //Order status enum
        public enum ABSOrderStateEnum
        {
            OPEN = 'O',
            DELETED = 'D',
            PICKED = 'P',
            WIP = 'W',
            INVENTORY = 'A',
            CANCELLED = 'C',
            SHIPPED = 'S',
        }
        #endregion
    }
}
