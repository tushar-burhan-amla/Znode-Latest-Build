using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Znode.Engine.ABSConnector
{
    public class ABSResponseHelper
    {
        //Get response models.
        public T GetResponseModel<T>(T responseModel, string requestType, string xmlResponse)
        {
            switch (requestType)
            {
                case ABSRequestTypes.ARList:
                    return (T)Convert.ChangeType(GetARListModel(responseModel as List<ABSARListResponseModel>, xmlResponse), typeof(T));
                case ABSRequestTypes.ARPayment:
                    return (T)Convert.ChangeType(GetARPaymentModel(responseModel as List<ABSARPaymentResponseModel>, xmlResponse), typeof(T));
                case ABSRequestTypes.Inventory:
                    return (T)Convert.ChangeType(GetInventoryModel(responseModel as List<ABSInventoryResponseModel>, xmlResponse), typeof(T));
                case ABSRequestTypes.OrderNumber:
                    return (T)Convert.ChangeType(GetOrderNumberModel(responseModel as List<ABSOrderNumberResponseModel>, xmlResponse), typeof(T));
                case ABSRequestTypes.OrderHistory:
                    return (T)Convert.ChangeType(GetOrderHistoryModel(responseModel as List<ABSOrderResponseDetailsModel>, xmlResponse), typeof(T));
                case ABSRequestTypes.PDFPrint:
                    return (T)Convert.ChangeType(GetPDFPrintModel(responseModel as List<ABSPrintInfoResponseModel>, xmlResponse), typeof(T));
            }

            return responseModel;
        }

        //Map the ABS AR List Response Model.
        private List<ABSARListResponseModel> GetARListModel(List<ABSARListResponseModel> model, string xmlResponse)
        {
            if (!string.IsNullOrEmpty(xmlResponse))
            {
                var doc = XDocument.Parse(xmlResponse);

                return (from r in doc.Root.Elements("ResponseInfo")
                        select new ABSARListResponseModel()
                        {
                            ItemStatus = (string)r.Element("ItemStatus"),
                            ItemReferenceNumber = (string)r.Element("ItemReferenceNumber"),
                            ItemReferenceDate = (string)r.Element("ItemReferenceDate"),
                            ItemReferenceType = (string)r.Element("ItemReferenceType"),
                            ItemShipDate = (string)r.Element("ItemShipDate"),
                            ItemAmount = (string)r.Element("ItemAmount"),
                            ItemPoNumber = (string)r.Element("ItemPoNumber"),
                            ItemFreightAmount = (string)r.Element("ItemFreightAmount"),
                            ItemAdjustmentAmount = (string)r.Element("ItemAdjustmentAmount"),
                            ItemShipToNumber = (string)r.Element("ItemShipToNumber"),

                        }).ToList();
            }

            return new List<ABSARListResponseModel>();
        }

        //Map the ABS AR Payment Response Model.
        private List<ABSARPaymentResponseModel> GetARPaymentModel(List<ABSARPaymentResponseModel> model, string xmlResponse)
        {
            if (!string.IsNullOrEmpty(xmlResponse))
            {
                var doc = XDocument.Parse(xmlResponse);

                return (from r in doc.Root.Elements("ArPaymentResponse")
                        select new ABSARPaymentResponseModel()
                        {
                            ProcessSuccessfully = (string)r.Element("ProcessSuccessfully"),
                        }).ToList();
            }

            return new List<ABSARPaymentResponseModel>();
        }

        //Map the ABS Inventory Response Model.
        private List<ABSInventoryResponseModel> GetInventoryModel(List<ABSInventoryResponseModel> model, string xmlResponse)
        {
            if (!string.IsNullOrEmpty(xmlResponse))
            {
                var doc = XDocument.Parse(xmlResponse);

                return (from r in doc.Root.Elements("InventoryRecord")
                        select new ABSInventoryResponseModel()
                        {
                            UpcNumber = (string)r.Element("UpcNumber"),
                            InventoryCode = (string)r.Element("InventoryCode"),
                            InventoryQty = Convert.ToInt32((string)r.Element("InventoryQty")),
                            InventoryCutNumber = (string)r.Element("InventoryCutNumber"),
                            InventoryExpectedDate = (!Equals((string)r.Element("InventoryExpectedDate"), null)) ? DateTime.ParseExact((string)r.Element("InventoryExpectedDate"), "yyyyMMdd", CultureInfo.InvariantCulture) : (DateTime?)null,
                            InventroyCloseOut = (string)r.Element("InventroyCloseOut"),
                            SoldOutFlag = (string)r.Element("SoldOutFlag"),
                            SoldOutCode = (string)r.Element("SoldOutCode"),
                            SoldOutDescription = (string)r.Element("SoldOutDescription"),
                            SoldOutDate = (!Equals((string)r.Element("SoldOutDate"), null)) ? DateTime.ParseExact((string)r.Element("SoldOutDate"), "yyyyMMdd", CultureInfo.InvariantCulture) : (DateTime?)null,
                        }).ToList();
            }

            return new List<ABSInventoryResponseModel>();
        }

        //Map the ABS Order Number Response Model.
        private List<ABSOrderNumberResponseModel> GetOrderNumberModel(List<ABSOrderNumberResponseModel> model, string xmlResponse)
        {
            if (!string.IsNullOrEmpty(xmlResponse))
            {
                var doc = XDocument.Parse(xmlResponse);

                return (from r in doc.Root.Elements("ResponseInfo")
                        select new ABSOrderNumberResponseModel()
                        {
                            NextOrderNumber = (string)r.Element("NextOrderNumber"),
                        }).ToList();
            }

            return new List<ABSOrderNumberResponseModel>();
        }

        //Map the ABS Order History Response Model.
        private List<ABSOrderResponseDetailsModel> GetOrderHistoryModel(List<ABSOrderResponseDetailsModel> model, string xmlResponse)
        {
            if (!string.IsNullOrEmpty(xmlResponse))
            {
                var test = xmlResponse.Replace("&", "&amp;");
                var doc = XDocument.Parse(test);

                var demo = (from r in doc.Root.Elements("OrderInformation").Elements("SoldToInformation")
                            select new ABSSoldToInformationModel()
                            {
                                SoldToAddress1 = (string)r.Element("SoldToAddress1"),
                                SoldToAddress2 =(string)r.Element("SoldToAddress2"),
                                SoldToCity = (string)r.Element("SoldToCity"),
                                SoldToName = (string)r.Element("SoldToName"),
                                SoldToState= (string)r.Element("SoldToState"),
                                SoldToZip= (string)r.Element("SoldToZip"),
                            }).ToList();

            }

            return new List<ABSOrderResponseDetailsModel>();
        }

        //Map the ABS Print Info Response Model.
        private List<ABSPrintInfoResponseModel> GetPDFPrintModel(List<ABSPrintInfoResponseModel> model, string xmlResponse)
        {
            if (!string.IsNullOrEmpty(xmlResponse))
            {
                var doc = XDocument.Parse(xmlResponse);

                return (from r in doc.Root.Elements("ResponseInfo")
                        select new ABSPrintInfoResponseModel()
                        {
                            DocumentURL = (string)r.Element("DocumentURL"),
                        }).ToList();
            }

            return new List<ABSPrintInfoResponseModel>();
        }
    }
}
