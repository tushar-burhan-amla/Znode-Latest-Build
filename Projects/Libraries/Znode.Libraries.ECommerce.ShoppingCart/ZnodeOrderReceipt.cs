using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.Admin;
using Znode.Libraries.ECommerce.Fulfillment;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    public class ZnodeOrderReceipt : ZnodeBusinessBase, IZnodeOrderReceipt
    {
        #region Constant Variables
        public const string PlusSign = "+";
        public const string MinusSign = "-";
        public readonly Dictionary<string, string> TitleSignDictionary = new Dictionary<string, string>
            {
                {Admin_Resources.LabelShipping, PlusSign },
                {Admin_Resources.LabelShippingHandlingCharges, PlusSign },
                {Admin_Resources.LabelTax, PlusSign },
                {Admin_Resources.LabelSalesTax, PlusSign },
                {Admin_Resources.LabelVAT, PlusSign },
                {Admin_Resources.LabelHST, PlusSign },
                {Admin_Resources.LabelPST, PlusSign },
                {Admin_Resources.LabelGST, PlusSign },
                {Admin_Resources.LabelDiscountAmount, MinusSign },
                {Admin_Resources.LabelCSRDiscount, MinusSign },
                {Admin_Resources.LabelShippingDiscount, MinusSign },
                {Admin_Resources.LabelReturnCharges, PlusSign }
            };
        #endregion

        #region Private Variables

        private string _cultureCode = string.Empty;
        private string _currencyCode = string.Empty;

        #endregion Private Variables

        #region Public Properties

        public ZnodeOrderFulfillment Order { get; set; }
        public ZnodeShoppingCart ShoppingCart { get; set; }
        public string FeedbackUrl { get; set; }
        public bool FromApi { get; set; }
        public ZnodeShoppingCart ApiShoppingCart { get; set; }
        public OrderModel OrderModel { get; set; }

        #endregion Public Properties

        #region Constructors

        public ZnodeOrderReceipt()
        {
        }

        public ZnodeOrderReceipt(ZnodeOrderFulfillment order)
        {
            Order = order;
        }

        public ZnodeOrderReceipt(OrderModel orderOrderModel)
        {
            OrderModel = orderOrderModel;
        }

        public ZnodeOrderReceipt(ZnodeOrderFulfillment order, ZnodeShoppingCart shoppingCart)
        {
            Order = order;
            ShoppingCart = shoppingCart;
        }

        public ZnodeOrderReceipt(ZnodeOrderFulfillment order, string feedbackUrl)
        {
            Order = order;
            FeedbackUrl = feedbackUrl;
        }

        public ZnodeOrderReceipt(string cultureCode)
        {
            _cultureCode = cultureCode;
        }

        #endregion Constructors

        #region Private Methods

        //Create datatable for shipping
        public virtual DataTable CreateShippingTable(DataTable shippingTable)
        {
            // Additional info
            shippingTable.Columns.Add("BillingFirstName");
            shippingTable.Columns.Add("BillingLastName");
            shippingTable.Columns.Add("TrackingMessage");
            shippingTable.Columns.Add("Message");

            return shippingTable;
        }

        //Generates the HTML used in email receipts.
        public virtual string GenerateVendorProductOrderReceipt(string templateContent, string vendorCode)
        {
            return CreateVendorProductOrderReceipt(templateContent, vendorCode);
        }

        public virtual string GenerateHtmlResendReceiptWithParser(string receiptHtml, string templateCode = "")
        {
            if (string.IsNullOrEmpty(receiptHtml))
                return receiptHtml;

            //order to bind order details in data table
            DataTable orderTable = SetOrderData(OrderModel, templateCode);

            //create order line Item
            DataTable orderlineItemTable = CreateOrderLineItemTable();

            // create returned order line Item
            DataTable returnedOrderlineItemTable = CreateReturnedOrderLineItemTable();

            //order to bind order amount details in data table
            DataTable orderAmountTable = SetOrderAmountData(OrderModel, templateCode);

            // order to bind returned order amount details in data table
            DataTable returnedOrderAmountTable = SetReturnedOrderAmountData(OrderModel);

            //create multiple Address
            DataTable multipleAddressTable = CreateOrderAddressTable();

            //create multiple tax address
            DataTable multipleTaxAddressTable = CreateOrderTaxAddressTable();

            //bind line item data
            BuildOrderLineItem(multipleAddressTable, orderlineItemTable, multipleTaxAddressTable, OrderModel, returnedOrderlineItemTable, templateCode);

            ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(receiptHtml);

            // Parse order table
            receiptHelper.Parse(orderTable.CreateDataReader());

            // Parse order line items table
            receiptHelper.Parse("AddressItems", multipleAddressTable.CreateDataReader());
            foreach (DataRow address in multipleAddressTable.Rows)
            {
                // Parse OrderLineItem
                var filterData = orderlineItemTable.DefaultView;
                filterData.RowFilter = $"OmsOrderShipmentID={address["OmsOrderShipmentID"]}";
                List<DataTable> group = filterData.ToTable().AsEnumerable()
                  .GroupBy(r => new { Col1 = r["GroupId"] })
                  .Select(g => g.CopyToDataTable()).ToList();

                receiptHelper.ParseWithGroup("LineItems" + address["OmsOrderShipmentID"], group);

                //Parse Tax based on order shipment
                var amountFilterData = multipleTaxAddressTable.DefaultView;
                amountFilterData.RowFilter = $"OmsOrderShipmentID={address["OmsOrderShipmentID"]}";
                receiptHelper.Parse($"AmountLineItems{address["OmsOrderShipmentID"]}", amountFilterData.ToTable().CreateDataReader());
            }

            // Parse returned OrderLineItem
            var returnFilterData = returnedOrderlineItemTable.DefaultView;
            if (returnFilterData.Count > 0 && IsNotNull(returnFilterData))
                receiptHelper.Parse("ReturnLineItems", returnFilterData.ToTable().CreateDataReader());

            // Parse order amount table
            receiptHelper.Parse("GrandAmountLineItems", orderAmountTable.CreateDataReader());

            // Parse returned order amount table
            if (returnedOrderAmountTable.Rows.Count > 0 && IsNotNull(returnedOrderAmountTable))
                receiptHelper.Parse("ReturnedGrandAmountLineItems", returnedOrderAmountTable.CreateDataReader());
            //Replace the Email Template Keys, based on the passed email template parameters.

            if (IsTaxSummaryAvailable() && !OrderModel.IsFromReturnedReceipt)
            {
                DataTable taxSummaryHeadTable = SetTaxSummaryHeadData();
                DataTable taxSummaryTable = SetTaxSummaryData();

                receiptHelper.Parse("TaxSummaryHead", taxSummaryHeadTable.CreateDataReader());
                receiptHelper.Parse("TaxSummary", taxSummaryTable.CreateDataReader());
            }
            else
            {
                receiptHelper.RemoveHTML("TaxSummaryHead");
                receiptHelper.RemoveHTML("TaxSummary");
            }
            // Return the HTML output
            return receiptHelper.Output;
        }

        // Builds the order line item table.
        public virtual void BuildOrderLineItem(DataTable multipleAddressTable, DataTable orderLineItemTable, DataTable multipleTaxAddressTable, OrderModel Order, DataTable returnedOrderlineItemTable, string templateCode = "")
        {
            List<OrderLineItemModel> OrderLineItemList = Order?.OrderLineItems.GroupBy(p => new { p.OmsOrderShipmentId }).Select(g => g.First()).ToList();
            IEnumerable<OrderShipmentModel> orderShipments = OrderLineItemList.Select(s => s.ZnodeOmsOrderShipment);

            int shipmentCounter = 1;

            foreach (OrderShipmentModel orderShipment in orderShipments)
            {
                DataRow addressRow = multipleAddressTable.NewRow();

                // If multiple shipping addresses then display the address for each group
                if (orderShipments.Count() > 1)
                {
                    addressRow["ShipmentNo"] = $"Shipment #{shipmentCounter++}{orderShipment.ShipName}";
                    addressRow["ShipTo"] = GetOrderShipmentAddress(orderShipment);
                }

                addressRow["OmsOrderShipmentID"] = orderShipment?.OmsOrderShipmentId;

                foreach (OrderLineItemModel lineitem in Order?.OrderLineItems?.Where(x => x.OmsOrderShipmentId == orderShipment.OmsOrderShipmentId)?.Reverse())
                {
                    if (orderLineItemTable != null)
                        orderLineItemTable.Rows.Add(SetOrderLineItemTable(orderLineItemTable, lineitem, lineitem.Price * lineitem.Quantity, templateCode));
                }

                var globalResourceObject = HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShipmentSubTotal");
                if (IsNotNull(globalResourceObject) && IsNotNull(Order))
                    BuildOrderShipmentTotalLineItem(globalResourceObject.ToString(), Order.SubTotal, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                if (orderShipments.Count() > 1 && IsNotNull(Order))
                {
                    BuildOrderShipmentTotalLineItem($"Shipping Cost({Order.ShippingTypeName})", Order.ShippingCost, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnSalesTax")) ? Admin_Resources.LabelSalesTax : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnSalesTax").ToString(), Order.SalesTax, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVAT")) ? Admin_Resources.LabelVAT : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVAT").ToString(), Order.VAT, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnGST")) ? Admin_Resources.LabelGST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnGST").ToString(), Order.GST, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnHST")) ? Admin_Resources.LabelHST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnHST").ToString(), Order.HST, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnPST")) ? Admin_Resources.LabelPST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnPST").ToString(), Order.PST, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);
                }
                multipleAddressTable.Rows.Add(addressRow);
            }

            // Returned line item
            foreach (OrderLineItemModel lineitem in Order?.ReturnedOrderLineItems ?? new List<OrderLineItemModel>())
            {
                if (returnedOrderlineItemTable != null)
                    returnedOrderlineItemTable.Rows.Add(SetReturnedOrderLineItemTable(returnedOrderlineItemTable, lineitem, lineitem.Price * lineitem.Quantity));
            }
        }

        //Set Order Line Item Table.
        public virtual DataRow SetOrderLineItemTable(DataTable orderLineItemTable, OrderLineItemModel lineitem, decimal extendedPrice, string templateCode = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(lineitem.ProductName + "<br />");

            //For binding personalise attribute to Name
            sb.Append(GetPersonaliseAttributesDetail(lineitem.PersonaliseValuesDetail));
            DataRow orderlineItemDbRow = orderLineItemTable.NewRow();
            orderlineItemDbRow["ProductImage"] = lineitem?.ProductImagePath;
            orderlineItemDbRow["Name"] = sb.ToString();
            orderlineItemDbRow["SKU"] = string.IsNullOrEmpty(lineitem?.OrderLineItemCollection?.FirstOrDefault()?.Sku) ? lineitem.Sku : lineitem?.OrderLineItemCollection.FirstOrDefault().Sku;
            orderlineItemDbRow["Description"] = lineitem.Description;
            orderlineItemDbRow["UOMDescription"] = string.Empty;
            double quantity;
            double.TryParse(Convert.ToString(lineitem.Quantity > 0 ? lineitem.Quantity : lineitem?.OrderLineItemCollection?.FirstOrDefault(x => x.Quantity > 0)?.Quantity), out quantity);
            orderlineItemDbRow["Quantity"] = Convert.ToString(quantity);
            orderlineItemDbRow["Price"] = GetFormatPriceWithCurrency(lineitem.Price);
            orderlineItemDbRow["ExtendedPrice"] = GetFormatPriceWithCurrency(extendedPrice);
            orderlineItemDbRow["OmsOrderShipmentID"] = lineitem.OmsOrderShipmentId;
            orderlineItemDbRow["ShortDescription"] = string.Empty;
            orderlineItemDbRow["OrderLineItemState"] = lineitem.OrderLineItemState;
            orderlineItemDbRow["TrackingNumber"] = lineitem.TrackingNumber;
            orderlineItemDbRow["Custom1"] = string.Empty;
            orderlineItemDbRow["Custom2"] = string.Empty;
            orderlineItemDbRow["Custom3"] = string.Empty;
            orderlineItemDbRow["Custom4"] = string.Empty;
            orderlineItemDbRow["Custom5"] = string.Empty;
            orderlineItemDbRow["GroupId"] = string.IsNullOrEmpty(lineitem.GroupId) ? Guid.NewGuid().ToString() : lineitem.GroupId;
            return orderlineItemDbRow;
        }

        // Set Returned Order Line Item Table.
        public virtual DataRow SetReturnedOrderLineItemTable(DataTable returnedOrderLineItemTable, OrderLineItemModel lineitem, decimal extendedPrice)
        {
            DataRow orderlineItemDbRow = returnedOrderLineItemTable.NewRow();
            if (IsNotNull(lineitem))
            {
                orderlineItemDbRow["ReturnedProductImage"] = lineitem.ProductImagePath;
                orderlineItemDbRow["ReturnedName"] = lineitem.ProductName;
                orderlineItemDbRow["ReturnedSKU"] = lineitem.Sku;
                orderlineItemDbRow["ReturnedDescription"] = lineitem.Description;
                orderlineItemDbRow["ReturnedQuantity"] = Convert.ToString(double.Parse(Convert.ToString(lineitem.Quantity)));
                orderlineItemDbRow["ReturnedPrice"] = GetFormatPriceWithCurrency(lineitem.Price);
                orderlineItemDbRow["ReturnedOmsOrderShipmentID"] = lineitem.OmsOrderShipmentId;
            }
            orderlineItemDbRow["ReturnedExtendedPrice"] = GetFormatPriceWithCurrency(extendedPrice);
            orderlineItemDbRow["ReturnedShortDescription"] = string.Empty;
            orderlineItemDbRow["ReturnedUOMDescription"] = string.Empty;
            return orderlineItemDbRow;
        }

        //to set order amount data
        public virtual DataTable SetOrderAmountData(OrderModel Order, string templateCode = "")
        {
            // Create order amount table
            DataTable orderAmountTable = CreateOrderAmountTable();

            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleSubTotal")) ? Admin_Resources.LabelSubTotal : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleSubTotal").ToString(), Order.SubTotal, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalShippingCost")) ? $"{Admin_Resources.LabelShipping}" : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalShippingCost").ToString(), (Order.ShippingCost), orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleHandlingCharges")) ? $"{Admin_Resources.LabelShippingHandlingCharges}" : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleHandlingCharges").ToString(), (Order.ShippingHandlingCharges).GetValueOrDefault(), orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleImportDutyTaxCost")) ? "Import Duty" : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleImportDutyTaxCost").ToString(), Order.ImportDuty, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalTaxCost")) ? Admin_Resources.LabelTax : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalTaxCost").ToString(), Order.TaxCost, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVAT")) ? Admin_Resources.LabelVAT : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVAT").ToString(), 0, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnHST")) ? Admin_Resources.LabelHST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnHST").ToString(), 0, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnPST")) ? Admin_Resources.LabelPST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnPST").ToString(), 0, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnGST")) ? Admin_Resources.LabelGST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnGST").ToString(), 0, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleDiscountAmount")) ? Admin_Resources.LabelDiscountAmount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleDiscountAmount").ToString(), Order.DiscountAmount, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleCSRDiscountAmount")) ? Admin_Resources.LabelCSRDiscount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleCSRDiscountAmount").ToString(), Order.CSRDiscountAmount, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShippingDiscountAmount")) ? Admin_Resources.LabelShippingDiscount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShippingDiscountAmount").ToString(), Order.ShippingDiscount.GetValueOrDefault(), orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnReturnCharges")) ? Admin_Resources.LabelReturnCharges : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnReturnCharges").ToString(), Order.ReturnCharges.GetValueOrDefault(), orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVoucherAmount")) ? Admin_Resources.LabelVoucherAmount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVoucherAmount").ToString(), Order.GiftCardAmount, orderAmountTable);

            return orderAmountTable;
        }

        // To set returned order amount data
        public virtual DataTable SetReturnedOrderAmountData(OrderModel order)
        {
            // Create order amount table
            DataTable orderAmountTable = CreateReturnedOrderAmountTable();
            BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleSubTotal")) ? Admin_Resources.LabelSubTotal : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleSubTotal").ToString(), order.ReturnSubTotal, orderAmountTable);
            BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalShippingCost")) ? $"{Admin_Resources.LabelShipping}" : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalShippingCost").ToString(), order?.ReturnItemList?.ShippingCost ?? 0, orderAmountTable);
            BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalTaxCost")) ? Admin_Resources.LabelTax : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalTaxCost").ToString(), order?.ReturnItemList?.TaxCost ?? 0, orderAmountTable);
            BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleImportDuty")) ? Admin_Resources.LabelImportDuty : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleImportDuty").ToString(), order.ReturnImportDuty, orderAmountTable);
            BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleDiscountAmount")) ? Admin_Resources.LabelDiscountAmount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleDiscountAmount").ToString(), order?.ReturnItemList?.DiscountAmount ?? 0, orderAmountTable);
            BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleCSRDiscountAmount")) ? Admin_Resources.LabelCSRDiscount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleCSRDiscountAmount").ToString(), order?.ReturnItemList?.CSRDiscount ?? 0, orderAmountTable);
            BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShippingDiscountAmount")) ? Admin_Resources.LabelShippingDiscount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShippingDiscountAmount").ToString(), order?.ReturnItemList?.ShippingDiscount ?? 0, orderAmountTable);
            BuildReturnedOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnReturnCharges")) ? Admin_Resources.LabelReturnCharges : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnReturnCharges").ToString(), order?.ReturnItemList?.ReturnCharges ?? 0, orderAmountTable);
            return orderAmountTable;
        }

        //to set order details
        public virtual DataTable SetOrderData(OrderModel Order, string templateCode = "")
        {
            // Create new row
            DataTable orderTable = CreateOrderTable();
            // Create new  for Shipping
            orderTable = CreateShippingTable(orderTable);
            DataRow orderRow = orderTable.NewRow();
            IZnodeOrderHelper orderHelper = GetService<IZnodeOrderHelper>();
            PortalModel portal = orderHelper.GetPortalDetailsByPortalId(Order.PortalId);
            _currencyCode = Order.CurrencyCode;
            _cultureCode = Order.CultureCode;
            // Additional info
            orderRow["SiteName"] = portal?.StoreName ?? ZnodeConfigManager.SiteConfig.StoreName;
            orderRow["StoreLogo"] = orderHelper.SetPortalLogo(Order.PortalId);
            orderRow["ReceiptText"] = string.Empty;
            orderRow["CustomerServiceEmail"] = FormatStringComma(portal?.CustomerServiceEmail) ?? FormatStringComma(ZnodeConfigManager.SiteConfig.CustomerServiceEmail);
            orderRow["CustomerServicePhoneNumber"] = portal?.CustomerServicePhoneNumber.Trim() ?? ZnodeConfigManager.SiteConfig.CustomerServicePhoneNumber.Trim();
            orderRow["FeedBack"] = FeedbackUrl;
            orderRow["ShippingName"] = Order?.ShippingTypeName;

            //Payment info
            if (!String.IsNullOrEmpty(Order.PaymentTransactionToken))
            {
                orderRow["CardTransactionID"] = Order.PaymentTransactionToken;
                orderRow["CardTransactionLabel"] = Admin_Resources.LabelTransactionId;
            }

            orderRow["PaymentName"] = Order.PaymentDisplayName;

            if (!String.IsNullOrEmpty(Order.PurchaseOrderNumber))
            {
                orderRow["PONumber"] = Order.PurchaseOrderNumber;
                orderRow["PurchaseNumberLabel"] = Admin_Resources.LabelPurchaseOrderNumber;
            }

            //Customer info
            orderRow["OrderId"] = Order.OrderNumber;
            orderRow["OrderDate"] = Order.OrderDate.ToShortDateString();

            orderRow["BillingAddress"] = Order.BillingAddressHtml;
            orderRow["PromotionCode"] = Order.CouponCode;
            orderRow["JobName"] = Order.JobName;

            orderRow["ShippingAddress"] = GetOrderShippingAddress(Order);

            orderRow["TotalCost"] = GetFormatPriceWithCurrency(Order.Total);

            // Returned total amount
            orderRow["ReturnedTotalCost"] = GetFormatPriceWithCurrency((decimal)Order.ReturnItemList?.Total);
            if (Order.AdditionalInstructions != null)
            {
                orderRow["AdditionalInstructions"] = Order.AdditionalInstructions;
                orderRow["AdditionalInstructLabel"] = Admin_Resources.LabelAdditionalNotes;
            }

            // Additional info for shipping
            orderRow["TrackingNumber"] = !string.IsNullOrEmpty(Order?.TrackingNumber) ? SetTrackingURL(Order) : string.Empty;
            orderRow["BillingFirstName"] = Order?.BillingAddress?.FirstName;
            orderRow["BillingLastName"] = Order?.BillingAddress?.LastName;
            orderRow["LabelInHandDate"] = Api_Resources.LabelInHandDate;
            orderRow["InHandDate"] = Order.InHandDate.HasValue ? Order.InHandDate.Value.ToString(GetStringDateFormat()) : string.Empty;
            orderRow["LabelShippingConstraintsCode"] = Api_Resources.LabelShippingConstraintsCode;
            orderRow["ShippingConstraintCode"] = string.IsNullOrWhiteSpace(Order.ShippingConstraintCode) ? string.Empty : GetEnumDescriptionValue((ShippingConstraintsEnum)Enum.Parse(typeof(ShippingConstraintsEnum), Order.ShippingConstraintCode));

            var shippedLineItemId = Order?.OrderLineItems?.Where(y => y?.OrderLineItemState?.ToLower() == ZnodeOrderStatusEnum.SHIPPED.ToString().ToLower()).Select(x => x.OmsOrderLineItemsId).ToArray();

            if (!string.IsNullOrEmpty(Order?.TrackingNumber))
            {
                //Set tracking number with link to the selected shipping type url
                SetOrderTrackingNumber(Order, orderRow);
                string url = !string.IsNullOrEmpty(SetTrackingURL(Order)) ? SetTrackingURL(Order) : string.Empty;
                orderRow["TrackingMessage"] = Equals(Order.OrderState, ZnodeOrderStatusEnum.SHIPPED.ToString()) ? Admin_Resources.ShippingTrackingNoMessage + url : string.Empty;
                orderRow["Message"] = string.Format(Admin_Resources.ShippingStatusMessage, Order?.OrderState?.ToLower()) + (Equals(Order?.OrderState, ZnodeOrderStatusEnum.SHIPPED.ToString()) ? Admin_Resources.TrackingPackageMessage : string.Empty);
            }
            else if (shippedLineItemId.Any())
            {
                orderRow["TrackingMessage"] = string.Empty;
                orderRow["Message"] = string.Format(Admin_Resources.ShippingStatusMessage, ZnodeOrderStatusEnum.SHIPPED.ToString().ToLower()) + Admin_Resources.TrackingPackageMessage;
            }
            else
            {
                orderRow["TrackingMessage"] = string.Empty;
                orderRow["Message"] = string.Format(Admin_Resources.ShippingStatusMessage, Order?.OrderState?.ToLower());
            }

            // Add rows to order table
            orderTable.Rows.Add(orderRow);
            return orderTable;
        }

        public virtual string GetOrderShippingAddress(OrderModel orderShipping)
        {
            if (IsNotNull(orderShipping))
            {
                string street1 = string.IsNullOrEmpty(orderShipping.ShippingAddress.Address2) ? string.Empty : "<br />" + orderShipping.ShippingAddress.Address2;
                return $"{orderShipping?.ShippingAddress.FirstName}{" "}{orderShipping?.ShippingAddress.LastName}{"<br />"}{orderShipping?.ShippingAddress.CompanyName}{"<br />"}{orderShipping.ShippingAddress.Address1}{street1}{"<br />"}{ orderShipping.ShippingAddress.CityName}{", "}{orderShipping.ShippingAddress.StateName}{", "}{orderShipping.ShippingAddress.CountryName}{", "}{orderShipping.ShippingAddress.PostalCode}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{orderShipping.ShippingAddress.PhoneNumber}";
            }
            return string.Empty;
        }

        public virtual string CreateOrderReceipt(string template, string templateCode = "")
        {
            if (string.IsNullOrEmpty(template))
                return template;

            //order to bind order details in data table
            System.Data.DataTable orderTable = SetOrderData();

            //create order line Item
            DataTable orderlineItemTable = CreateOrderLineItemTable();

            //order to bind order amount details in data table
            DataTable orderAmountTable = SetOrderAmountData();

            //create multiple Address
            DataTable multipleAddressTable = CreateOrderAddressTable();

            //create multiple tax address
            DataTable multipleTaxAddressTable = CreateOrderTaxAddressTable();

            //bind line item data
            BuildOrderLineItem(multipleAddressTable, orderlineItemTable, multipleTaxAddressTable, templateCode );

            ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(template);

            // Parse order table
            receiptHelper.Parse(orderTable.CreateDataReader());

            // Parse order line items table
            receiptHelper.Parse("AddressItems", multipleAddressTable.CreateDataReader());
            foreach (DataRow address in multipleAddressTable.Rows)
            {
                // Parse OrderLineItem
                var filterData = orderlineItemTable.DefaultView;

                List<DataTable> group = filterData.ToTable().AsEnumerable()
                .GroupBy(r => new { Col1 = r["GroupId"] })
                .Select(g => g.CopyToDataTable()).ToList();

                filterData.RowFilter = $"OmsOrderShipmentID={address["OmsOrderShipmentID"]}";
                receiptHelper.ParseWithGroup("LineItems" + address["OmsOrderShipmentID"], group);

                //Parse Tax based on order shipment
                var amountFilterData = multipleTaxAddressTable.DefaultView;
                amountFilterData.RowFilter = $"OmsOrderShipmentID={address["OmsOrderShipmentID"]}";
                receiptHelper.Parse($"AmountLineItems{address["OmsOrderShipmentID"]}", amountFilterData.ToTable().CreateDataReader());
            }
            // Parse order amount table
            receiptHelper.Parse("GrandAmountLineItems", orderAmountTable.CreateDataReader());

            if (IsTaxSummaryAvailable())
            {
                DataTable taxSummaryHeadTable = SetTaxSummaryHeadData();
                DataTable taxSummaryTable = SetTaxSummaryData();

                receiptHelper.Parse("TaxSummaryHead", taxSummaryHeadTable.CreateDataReader());
                receiptHelper.Parse("TaxSummary", taxSummaryTable.CreateDataReader());
            }
            else
            {
                receiptHelper.RemoveHTML("TaxSummaryHead");
                receiptHelper.RemoveHTML("TaxSummary");
            }            
            //Replace the Email Template Keys, based on the passed email template parameters.

            // Return the HTML output
            return receiptHelper.Output;
        }

        //to create order table
        public virtual DataTable CreateOrderTable()
        {
            DataTable orderTable = new DataTable();
            // Additional info
            orderTable.Columns.Add("SiteName");
            orderTable.Columns.Add("StoreLogo");
            orderTable.Columns.Add("ReceiptText");
            orderTable.Columns.Add("CustomerServiceEmail");
            orderTable.Columns.Add("CustomerServicePhoneNumber");
            orderTable.Columns.Add("FeedBack");
            orderTable.Columns.Add("AdditionalInstructions");
            orderTable.Columns.Add("AdditionalInstructLabel");

            // Payment info
            orderTable.Columns.Add("CardTransactionID");
            orderTable.Columns.Add("CardTransactionLabel");
            orderTable.Columns.Add("ShippingMethod");
            orderTable.Columns.Add("AccountNumber");
            orderTable.Columns.Add("PaymentName");

            orderTable.Columns.Add("PONumber");
            orderTable.Columns.Add("PurchaseNumberLabel");

            // Customer info
            orderTable.Columns.Add("OrderId");
            orderTable.Columns.Add("OrderDate");
            orderTable.Columns.Add("UserId");
            orderTable.Columns.Add("BillingAddress");
            orderTable.Columns.Add("ShippingAddress");
            orderTable.Columns.Add("PromotionCode");
            orderTable.Columns.Add("TotalCost");
            orderTable.Columns.Add("JobName");
            orderTable.Columns.Add("LabelInHandDate");
            orderTable.Columns.Add("InHandDate");
            orderTable.Columns.Add("LabelShippingConstraintsCode");
            orderTable.Columns.Add("ShippingConstraintCode");
            // Returned total cost
            orderTable.Columns.Add("ReturnedTotalCost");
            orderTable.Columns.Add("StyleSheetPath");

            orderTable.Columns.Add("ShippingName");
            orderTable.Columns.Add("TrackingNumber");
            return orderTable;
        }

        //to create order amount table
        public virtual DataTable CreateOrderAmountTable()
        {
            DataTable orderAmountTable = new DataTable();
            orderAmountTable.Columns.Add("Title");
            orderAmountTable.Columns.Add("Amount");
            return orderAmountTable;
        }

        // To create returned order amount table
        public virtual DataTable CreateReturnedOrderAmountTable()
        {
            DataTable orderAmountTable = new DataTable();
            orderAmountTable.Columns.Add("ReturnedTitle");
            orderAmountTable.Columns.Add("ReturnedAmount");
            return orderAmountTable;
        }

        //to create order order line item table
        public virtual DataTable CreateOrderLineItemTable()
        {
            DataTable orderlineItemTable = new DataTable();
            orderlineItemTable.Columns.Add("ProductImage");
            orderlineItemTable.Columns.Add("Name");
            orderlineItemTable.Columns.Add("SKU");
            orderlineItemTable.Columns.Add("Quantity");
            orderlineItemTable.Columns.Add("Description");
            orderlineItemTable.Columns.Add("UOMDescription");
            orderlineItemTable.Columns.Add("Price");
            orderlineItemTable.Columns.Add("ExtendedPrice");
            orderlineItemTable.Columns.Add("OmsOrderShipmentID");
            orderlineItemTable.Columns.Add("ShortDescription");
            orderlineItemTable.Columns.Add("ShippingId");
            orderlineItemTable.Columns.Add("OrderLineItemState");
            orderlineItemTable.Columns.Add("TrackingNumber");
            orderlineItemTable.Columns.Add("Custom1");
            orderlineItemTable.Columns.Add("Custom2");
            orderlineItemTable.Columns.Add("Custom3");
            orderlineItemTable.Columns.Add("Custom4");
            orderlineItemTable.Columns.Add("Custom5");
            orderlineItemTable.Columns.Add("GroupId");
            orderlineItemTable.Columns.Add("GroupingRowspan");
            orderlineItemTable.Columns.Add("GroupingDisplay");
            return orderlineItemTable;
        }

        // To create returned order line item table
        public virtual DataTable CreateReturnedOrderLineItemTable()
        {
            DataTable returnedOrderlineItemTable = new DataTable();
            returnedOrderlineItemTable.Columns.Add("ReturnedProductImage");
            returnedOrderlineItemTable.Columns.Add("ReturnedName");
            returnedOrderlineItemTable.Columns.Add("ReturnedSKU");
            returnedOrderlineItemTable.Columns.Add("ReturnedQuantity");
            returnedOrderlineItemTable.Columns.Add("ReturnedDescription");
            returnedOrderlineItemTable.Columns.Add("ReturnedUOMDescription");
            returnedOrderlineItemTable.Columns.Add("ReturnedPrice");
            returnedOrderlineItemTable.Columns.Add("ReturnedExtendedPrice");
            returnedOrderlineItemTable.Columns.Add("ReturnedOmsOrderShipmentID");
            returnedOrderlineItemTable.Columns.Add("ReturnedShortDescription");
            returnedOrderlineItemTable.Columns.Add("ReturnedShippingId");
            return returnedOrderlineItemTable;
        }

        //to create order address table
        public virtual DataTable CreateOrderAddressTable()
        {
            DataTable multipleAddressTable = new DataTable();
            multipleAddressTable.Columns.Add("ShipTo");
            multipleAddressTable.Columns.Add("OmsOrderShipmentID");
            multipleAddressTable.Columns.Add("ShipmentNo");
            return multipleAddressTable;
        }

        //to create order address table
        public virtual DataTable CreateOrderTaxAddressTable()
        {
            DataTable multipleTaxAddressTable = new DataTable();
            multipleTaxAddressTable.Columns.Add("OmsOrderShipmentID");
            multipleTaxAddressTable.Columns.Add("Title");
            multipleTaxAddressTable.Columns.Add("Amount");
            return multipleTaxAddressTable;
        }

        //to set order details
        public virtual DataTable SetOrderData()
        {
            // Create new row
            DataTable orderTable = CreateOrderTable();
            DataRow orderRow = orderTable.NewRow();
            IZnodeOrderHelper helper = GetService<IZnodeOrderHelper>();
            PortalModel portal = helper.GetPortalDetailsByPortalId(Order.PortalId);
            _currencyCode = Order.CurrencyCode;
            _cultureCode = Order.CultureCode;
            // Additional info
            orderRow["SiteName"] = portal?.StoreName ?? ZnodeConfigManager.SiteConfig.StoreName;
            orderRow["StoreLogo"] = helper.SetPortalLogo(Order.PortalId);
            orderRow["ReceiptText"] = string.Empty;
            orderRow["CustomerServiceEmail"] = FormatStringComma(portal?.CustomerServiceEmail) ?? FormatStringComma(ZnodeConfigManager.SiteConfig.CustomerServiceEmail);
            orderRow["CustomerServicePhoneNumber"] = portal?.CustomerServicePhoneNumber.Trim() ?? ZnodeConfigManager.SiteConfig.CustomerServicePhoneNumber.Trim();
            orderRow["FeedBack"] = FeedbackUrl;
            orderRow["ShippingName"] = Order?.ShippingName;

            //Payment info
            if (!String.IsNullOrEmpty(Order.PaymentTransactionToken))
            {
                orderRow["CardTransactionID"] = Order.PaymentTransactionToken;
                orderRow["CardTransactionLabel"] = Admin_Resources.LabelTransactionId;
            }

            orderRow["AccountNumber"] = Order.AccountNumber;
            orderRow["ShippingMethod"] = Order.ShippingMethod;
            orderRow["PaymentName"] = Order.PaymentDisplayName;

            if (!String.IsNullOrEmpty(Order.PurchaseOrderNumber))
            {
                orderRow["PONumber"] = Order.PurchaseOrderNumber;
                orderRow["PurchaseNumberLabel"] = Admin_Resources.LabelPurchaseOrderNumber;
            }

            //Customer info
            orderRow["OrderId"] = Order?.Order?.OrderNumber;
            orderRow["OrderDate"] = Order.OrderDateWithTime;

            orderRow["BillingAddress"] = GetOrderBillingAddress(Order.BillingAddress);
            orderRow["PromotionCode"] = Order.CouponCode;
            orderRow["JobName"] = Order.JobName;

            var addresses = ((IZnodePortalCart)ShoppingCart).AddressCarts;

            orderRow["ShippingAddress"] = addresses.Count > 1 ? Admin_Resources.MessageKeyShippingMultipleAddress :
                GetOrderShipmentAddress(Order.OrderLineItems.FirstOrDefault().ZnodeOmsOrderShipment);

            orderRow["LabelInHandDate"] = Api_Resources.LabelInHandDate;
            orderRow["InHandDate"] = Order.InHandDate.HasValue ? Order.InHandDate.Value.ToString(GetStringDateFormat()) : string.Empty;
            orderRow["LabelShippingConstraintsCode"] = Api_Resources.LabelShippingConstraintsCode;
            orderRow["ShippingConstraintCode"] = string.IsNullOrWhiteSpace(Order.ShippingConstraintCode) ? string.Empty : GetEnumDescriptionValue((ShippingConstraintsEnum)Enum.Parse(typeof(ShippingConstraintsEnum), Order.ShippingConstraintCode));

            orderRow["TotalCost"] = GetFormatPriceWithCurrency(Order.OrderTotalWithoutVoucher);
            if (Order.AdditionalInstructions != null)
            {
                orderRow["AdditionalInstructions"] = Order.AdditionalInstructions;
                orderRow["AdditionalInstructLabel"] = Admin_Resources.LabelAdditionalNotes;
            }
            // Add rows to order table
            orderTable.Rows.Add(orderRow);
            return orderTable;
        }

        //to set order amount data
        public virtual DataTable SetOrderAmountData()
        {
            // Create order amount table
            DataTable orderAmountTable = CreateOrderAmountTable();

            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleSubTotal")) ? Admin_Resources.LabelSubTotal : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleSubTotal").ToString(), Order.SubTotal, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalShippingCost")) ? $"{Admin_Resources.LabelShipping}" : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalShippingCost").ToString(), (Order.ShippingCost), orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleHandlingCharges")) ? $"{Admin_Resources.LabelShippingHandlingCharges}" : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleHandlingCharges").ToString(), (Order.ShippingHandlingCharges), orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleImportDutyTaxCost")) ? "Import Duty" : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleImportDutyTaxCost").ToString(), Order.ImportDuty, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalTaxCost")) ? Admin_Resources.LabelTax : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleTotalTaxCost").ToString(), Order.TaxCost, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVAT")) ? Admin_Resources.LabelVAT : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVAT").ToString(), Order.VAT, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnHST")) ? Admin_Resources.LabelHST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnHST").ToString(), Order.HST, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnPST")) ? Admin_Resources.LabelPST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnPST").ToString(), Order.PST, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnGST")) ? Admin_Resources.LabelGST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnGST").ToString(), Order.GST, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleDiscountAmount")) ? Admin_Resources.LabelDiscountAmount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleDiscountAmount").ToString(), Order.DiscountAmount, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleCSRDiscountAmount")) ? Admin_Resources.LabelCSRDiscount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleCSRDiscountAmount").ToString(), Order.CSRDiscountAmount, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShippingDiscountAmount")) ? Admin_Resources.LabelShippingDiscount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShippingDiscountAmount").ToString(), Order.ShippingDiscount, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnReturnCharges")) ? Admin_Resources.LabelReturnCharges : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnReturnCharges").ToString(), Order.ReturnCharges, orderAmountTable);
            BuildOrderAmountTable(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVoucherAmount")) ? Admin_Resources.LabelVoucherAmount : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVoucherAmount").ToString(), Order.GiftCardAmount, orderAmountTable);

            return orderAmountTable;
        }

        // Builds the order amount table.
        public virtual void BuildOrderAmountTable(string title, decimal amount, DataTable orderAmountTable)
        {
            //gets +/- signs based on the order summary field label. 
            string sign = GetSignBasedOnSummaryTitle(title);
            if(string.Equals(title, Admin_Resources.LabelReturnCharges, StringComparison.InvariantCultureIgnoreCase) && string.Equals(OrderModel?.OrderLineItems?.FirstOrDefault()?.OrderLineItemState, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                sign = MinusSign;

            if (amount != 0)
            {
                var row = orderAmountTable.NewRow();
                row["Title"] = title;
                row["Amount"] = string.Concat(sign, GetFormatPriceWithCurrency(amount));

                orderAmountTable.Rows.Add(row);
            }
            else if (string.Equals(title, Admin_Resources.LabelSubTotal, StringComparison.InvariantCultureIgnoreCase) || string.Equals(title, Admin_Resources.LabelShipping, StringComparison.InvariantCultureIgnoreCase) || string.Equals(title, Admin_Resources.LabelTax, StringComparison.InvariantCultureIgnoreCase))
            {
                var row = orderAmountTable.NewRow();
                row["Title"] = title;
                row["Amount"] = string.Concat(sign, GetFormatPriceWithCurrency(amount));

                orderAmountTable.Rows.Add(row);
            }
        }

        // Builds the returned order amount table.
        public virtual void BuildReturnedOrderAmountTable(string title, decimal amount, DataTable orderAmountTable, string cultureCode = "")
        {
            //gets +/- signs based on the order summary field label.
            string sign = GetSignBasedOnSummaryTitle(title);
            if (string.Equals(title, Admin_Resources.LabelReturnCharges, StringComparison.InvariantCultureIgnoreCase))
                sign = MinusSign;
            if (amount != 0)
            {
                var row = orderAmountTable.NewRow();
                row["ReturnedTitle"] = title;
                row["ReturnedAmount"] = string.Concat(sign, GetFormatPriceWithCurrency(amount, cultureCode: cultureCode));

                orderAmountTable.Rows.Add(row);
            }
            else if (string.Equals(title, Admin_Resources.LabelSubTotal, StringComparison.InvariantCultureIgnoreCase) || string.Equals(title, Admin_Resources.LabelShipping, StringComparison.InvariantCultureIgnoreCase) || string.Equals(title, Admin_Resources.LabelTax, StringComparison.InvariantCultureIgnoreCase))
            {
                var row = orderAmountTable.NewRow();
                row["ReturnedTitle"] = title;
                row["ReturnedAmount"] = string.Concat(sign, GetFormatPriceWithCurrency(amount, cultureCode: cultureCode));

                orderAmountTable.Rows.Add(row);
            }
        }

        // Builds the order line item table.
        public virtual void BuildOrderLineItem(DataTable multipleAddressTable, DataTable orderLineItemTable, DataTable multipleTaxAddressTable, string templateCode = "")
        {
            List<OrderLineItemModel> OrderLineItemList = Order?.OrderLineItems.GroupBy(p => new { p.OmsOrderShipmentId }).Select(g => g.First()).ToList();
            IEnumerable<OrderShipmentModel> orderShipments = OrderLineItemList.Select(s => s.ZnodeOmsOrderShipment);

            int shipmentCounter = 1;

            foreach (var orderShipment in orderShipments)
            {
                var addressRow = multipleAddressTable.NewRow();

                // If multiple shipping addresses then display the address for each group
                if (orderShipments.Count() > 1)
                {
                    addressRow["ShipmentNo"] = $"Shipment #{shipmentCounter++}{orderShipment.ShipName}";
                    addressRow["ShipTo"] = GetOrderShipmentAddress(orderShipment);
                }

                addressRow["OmsOrderShipmentID"] = orderShipment.OmsOrderShipmentId;
                var counter = 0;

                var readedShoppingCartItems = new List<ZnodeShoppingCartItem>();

                foreach (OrderLineItemModel lineitem in Order.OrderLineItems.Where(x => x.OmsOrderShipmentId == orderShipment.OmsOrderShipmentId))
                {
                    var shoppingCartItems = ((IZnodePortalCart)ShoppingCart).AddressCarts.Where(x => x.OrderShipmentID == orderShipment.OmsOrderShipmentId).SelectMany(x => x.ShoppingCartItems.Cast<ZnodeShoppingCartItem>());

                    lineitem.OrderLineItemCollection.RemoveAll(x => x.OrderLineItemRelationshipTypeId == Convert.ToInt16(ZnodeCartItemRelationshipTypeEnum.AddOns));
                    if (lineitem.OrderLineItemCollection?.Any(x => x.OrderLineItemRelationshipTypeId != Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.Bundles)) ?? false)
                    {
                        foreach (OrderLineItemModel childLineItem in lineitem.OrderLineItemCollection)
                        {
                            ZnodeShoppingCartItem shoppingCartItem = null;
                            if (Order.Order.IsQuoteOrder)
                            {
                                //TODO : Right now we are adding these conditions for handling same products with different addons. We will refactor this later.
                                if (readedShoppingCartItems.Where(s => s.SKU.Equals(childLineItem.Sku)).Count() > 0)
                                {
                                    shoppingCartItem = shoppingCartItems.FirstOrDefault(s => s.GroupId == lineitem.GroupId && (!string.IsNullOrEmpty(s.SKU) ? childLineItem.Sku.Contains(s.SKU) : false) && s.OrderLineItemRelationshipTypeId.HasValue && !readedShoppingCartItems.Any(n => n.SKU == s.SKU && n.Sequence == s.Sequence && n.Description == s.Description));
                                }
                                else
                                {
                                    shoppingCartItem = shoppingCartItems.FirstOrDefault(s => s.GroupId == lineitem.GroupId && (!string.IsNullOrEmpty(s.SKU) ? childLineItem.Sku.Contains(s.SKU) : false) && s.OrderLineItemRelationshipTypeId.HasValue);
                                }
                                if (IsNull(shoppingCartItem))
                                {
                                    if (readedShoppingCartItems.Where(s => s.SKU.Equals(childLineItem.Sku)).Count() > 0)
                                    {
                                        shoppingCartItem = shoppingCartItems.FirstOrDefault(s => s.GroupId == lineitem.GroupId && childLineItem.Sku == s.ParentProductSKU && s.OrderLineItemRelationshipTypeId.HasValue && !readedShoppingCartItems.Any(n => n.SKU == s.SKU && n.Sequence == s.Sequence && n.Description == s.Description));
                                    }
                                    else
                                    {
                                        shoppingCartItem = shoppingCartItems.FirstOrDefault(s => s.GroupId == lineitem.GroupId && childLineItem.Sku == s.ParentProductSKU && s.OrderLineItemRelationshipTypeId.HasValue);
                                    }

                                }
                            }
                            else
                            {
                                if (readedShoppingCartItems.Where(s => s.SKU.Equals(childLineItem.Sku)).Count() > 0)
                                {
                                    shoppingCartItem = shoppingCartItems.FirstOrDefault(s => s.GroupId == lineitem.GroupId && s.SKU == childLineItem.Sku && s.OrderLineItemRelationshipTypeId.HasValue && !readedShoppingCartItems.Any(n => n.SKU == s.SKU && n.Sequence == s.Sequence && n.Description == s.Description && childLineItem?.PersonaliseValuesDetail?.Count <= 0));
                                }
                                else
                                {
                                    shoppingCartItem = shoppingCartItems.FirstOrDefault(s => s.GroupId == lineitem.GroupId && s.SKU == childLineItem.Sku && s.OrderLineItemRelationshipTypeId.HasValue);
                                }
                            }

                            //Get ShoppingCartItem when GroupId is Null
                            if (IsNull(shoppingCartItem))
                            {
                                if (readedShoppingCartItems.Where(s => s.SKU.Equals(childLineItem.Sku)).Count() > 0)
                                {
                                    shoppingCartItem = shoppingCartItems.FirstOrDefault(m => m.ParentProductSKU.Equals(childLineItem.Sku) && !readedShoppingCartItems.Any(n => n.SKU == m.SKU && n.Sequence == m.Sequence && n.Description == m.Description));
                                }
                                else
                                {
                                    shoppingCartItem = shoppingCartItems.FirstOrDefault(m => m.ParentProductSKU.Equals(childLineItem.Sku));
                                }
                            }
                            setGroupProductDetails(lineitem, childLineItem);

                            StringBuilder sb = new StringBuilder();
                            sb.Append(lineitem.ProductName + "<br />");

                            //For binding personalise attribute to Name
                            if (IsNotNull(lineitem.PersonaliseValueList))
                            {
                                sb.Append(GetPersonaliseAttributes(lineitem.PersonaliseValueList));
                            }
                            else
                            {
                                sb.Append(GetPersonaliseAttributesDetail(lineitem.PersonaliseValuesDetail));
                            }

                            if (!String.IsNullOrEmpty(shoppingCartItem?.Product?.DownloadLink?.Trim()))
                            {
                                sb.Append("<a href='" + shoppingCartItem.Product.DownloadLink + "' target='_blank'>Download</a><br />");
                            }

                            if (orderLineItemTable != null)
                            {
                                DataRow orderlineItemDbRow = orderLineItemTable.NewRow();
                                orderlineItemDbRow["ProductImage"] = lineitem.ProductImagePath;
                                orderlineItemDbRow["Name"] = sb.ToString();
                                orderlineItemDbRow["SKU"] = childLineItem.Sku;
                                orderlineItemDbRow["Description"] = lineitem.Description;
                                orderlineItemDbRow["UOMDescription"] = string.Empty;
                                orderlineItemDbRow["Quantity"] = childLineItem.Quantity;
                                if (shoppingCartItem != null)
                                {
                                    orderlineItemDbRow["Price"] = GetFormatPriceWithCurrency(shoppingCartItem.UnitPrice, shoppingCartItem.UOM);
                                    orderlineItemDbRow["ExtendedPrice"] = GetFormatPriceWithCurrency(shoppingCartItem.ExtendedPrice);
                                    orderlineItemDbRow["ShortDescription"] = shoppingCartItem.Product.ShortDescription;
                                }

                                orderlineItemDbRow["OmsOrderShipmentID"] = childLineItem.OmsOrderShipmentId;
                                orderlineItemDbRow["GroupId"] = string.IsNullOrEmpty(lineitem.GroupId) ? Guid.NewGuid().ToString() : lineitem.GroupId;

                                orderLineItemTable.Rows.Add(orderlineItemDbRow);
                            }
                            counter++;
                            if (shoppingCartItem != null)
                                readedShoppingCartItems.Add(shoppingCartItem);
                        }
                    }
                    else
                    {
                        var shoppingCartItem = shoppingCartItems.ElementAt(counter++);

                        foreach (OrderLineItemModel orderLineItem in lineitem.OrderLineItemCollection)
                            setGroupProductDetails(lineitem, orderLineItem);

                        StringBuilder sb = new StringBuilder();
                        sb.Append(lineitem.ProductName + "<br />");

                        //For binding personalise attribute Value to Name
                        if (IsNotNull(lineitem.PersonaliseValueList))
                        {
                            sb.Append(GetPersonaliseAttributes(lineitem.PersonaliseValueList));
                        }
                        else
                        {
                            sb.Append(GetPersonaliseAttributesDetail(lineitem.PersonaliseValuesDetail));
                        }
                        if (!String.IsNullOrEmpty(shoppingCartItem.Product.DownloadLink.Trim()))
                        {
                            sb.Append("<a href='" + shoppingCartItem.Product.DownloadLink + "' target='_blank'>Download</a><br />");
                        }

                        if (orderLineItemTable != null)
                        {
                            DataRow orderlineItemDbRow = orderLineItemTable.NewRow();
                            orderlineItemDbRow["ProductImage"] = lineitem.ProductImagePath;
                            orderlineItemDbRow["Name"] = sb.ToString();
                            orderlineItemDbRow["SKU"] = lineitem.Sku;
                            orderlineItemDbRow["Description"] = lineitem.Description;
                            orderlineItemDbRow["UOMDescription"] = string.Empty;
                            orderlineItemDbRow["Quantity"] = lineitem.OrderLineItemRelationshipTypeId.Equals((int)ZnodeCartItemRelationshipTypeEnum.Group) ? lineitem.GroupProductQuantity : Convert.ToString(double.Parse(Convert.ToString(lineitem.Quantity)));
                            orderlineItemDbRow["Price"] = GetFormatPriceWithCurrency(shoppingCartItem.UnitPrice, shoppingCartItem.UOM);
                            orderlineItemDbRow["ExtendedPrice"] = GetFormatPriceWithCurrency(shoppingCartItem.ExtendedPrice);
                            orderlineItemDbRow["OmsOrderShipmentID"] = lineitem.OmsOrderShipmentId;
                            orderlineItemDbRow["ShortDescription"] = shoppingCartItem.Product.ShortDescription;
                            orderlineItemDbRow["GroupId"] = string.IsNullOrEmpty(lineitem.GroupId) ? Guid.NewGuid().ToString() : lineitem.GroupId;
                            orderLineItemTable.Rows.Add(orderlineItemDbRow);
                        }
                    }
                }

                var addressCart = ((IZnodePortalCart)ShoppingCart).AddressCarts.FirstOrDefault(y => y.OrderShipmentID == orderShipment.OmsOrderShipmentId);

                if (addressCart != null && orderShipments.Count() > 1)
                {
                    var globalResourceObject = HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnTitleShipmentSubTotal");
                    if (globalResourceObject != null)
                        BuildOrderShipmentTotalLineItem(globalResourceObject.ToString(), addressCart.SubTotal, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem($"Shipping Cost({addressCart.Shipping.ShippingName})", addressCart.ShippingCost, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnSalesTax")) ? Admin_Resources.LabelSalesTax : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnSalesTax").ToString(), addressCart.SalesTax, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVAT")) ? Admin_Resources.LabelVAT : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnVAT").ToString(), addressCart.VAT, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnGST")) ? Admin_Resources.LabelGST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnGST").ToString(), addressCart.GST, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnHST")) ? Admin_Resources.LabelHST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnHST").ToString(), addressCart.HST, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);

                    BuildOrderShipmentTotalLineItem(IsNull(HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnPST")) ? Admin_Resources.LabelPST : HttpContext.GetGlobalResourceObject("CommonCaption", "ColumnPST").ToString(), addressCart.PST, orderShipment.OmsOrderShipmentId, multipleTaxAddressTable);
                }
                multipleAddressTable.Rows.Add(addressRow);
            }
        }

        // Builds the Shipment order line item table.
        public virtual void BuildOrderShipmentTotalLineItem(string title, decimal amount, int OmsOrderShipmentId, DataTable taxTable)
        {
            if (amount > 0)
            {
                var taxAddressRow = taxTable.NewRow();
                taxAddressRow["Title"] = title;
                taxAddressRow["Amount"] = GetFormatPriceWithCurrency(amount);
                taxAddressRow["OmsOrderShipmentID"] = OmsOrderShipmentId;
                taxTable.Rows.Add(taxAddressRow);
            }
        }

        //to get order shipment address
        public virtual string GetOrderShipmentAddress(OrderShipmentModel orderShipment)
        {
            if (IsNotNull(orderShipment))
            {
                string street1 = string.IsNullOrEmpty(orderShipment.ShipToStreet2) ? string.Empty : "<br />" + orderShipment.ShipToStreet2;
                orderShipment.ShipToCompanyName = IsNotNull(orderShipment?.ShipToCompanyName) ? $"{orderShipment?.ShipToCompanyName}" : Order.ShippingAddress.CompanyName;
                return $"{orderShipment?.ShipToFirstName}{" "}{ orderShipment?.ShipToLastName}{"<br />"}{ orderShipment.ShipToCompanyName}{"<br />"}{orderShipment.ShipToStreet1}{street1}{"<br />"}{ orderShipment.ShipToCity}{", "}{orderShipment.ShipToStateCode}{", "}{orderShipment.ShipToCountry}{" "}{orderShipment.ShipToPostalCode}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{orderShipment.ShipToPhoneNumber}";
            }
            return string.Empty;
        }

        //to get shipping address
        public virtual string GetOrderBillingAddress(AddressModel orderBilling)
        {
            if (IsNotNull(orderBilling))
            {
                string street2 = string.IsNullOrEmpty(orderBilling.Address2) ? string.Empty : "<br />" + orderBilling.Address2;
                return $"{orderBilling?.FirstName}{" "}{orderBilling?.LastName}{"<br />"}{orderBilling?.CompanyName}{"<br />"}{orderBilling.Address1}{street2}{"<br />"}{ orderBilling.CityName}{", "}{orderBilling.StateName}{", "}{orderBilling.CountryName}{" "}{orderBilling.PostalCode}{"<br />"}{Admin_Resources.LabelPhoneNumber}{" : "}{orderBilling.PhoneNumber}";
            }
            return string.Empty;
        }

        //For Getting personalise attribute.
        public virtual string GetPersonaliseAttributes(Dictionary<string, object> personaliseValueList)
        {
            string personaliseAttributeHtml = string.Empty;
            if (IsNotNull(personaliseValueList))
            {
                foreach (var personaliseAttribute in personaliseValueList)
                {
                    personaliseAttributeHtml += $"{"<p>"} { personaliseAttribute.Key}{":"}{personaliseAttribute.Value}{"</p>"}";
                }
                return personaliseAttributeHtml;
            }
            return string.Empty;
        }

        public virtual string GetPersonaliseAttributesDetail(List<PersonaliseValueModel> PersonaliseValuesDetail)
        {
            string personaliseAttributeHtml = string.Empty;
            if (PersonaliseValuesDetail?.Count > 0)
            {
                foreach (var personaliseAttribute in PersonaliseValuesDetail)
                {
                    if (!string.IsNullOrEmpty(personaliseAttribute.PersonalizeValue))
                        personaliseAttributeHtml += $"{"<p>"} { personaliseAttribute.PersonalizeName}{":"}{personaliseAttribute.PersonalizeValue}{"</p>"}";
                }

                return personaliseAttributeHtml;
            }
            return string.Empty;
        }


        //to add space after comma
        public virtual string FormatStringComma(string input)
        {
            return input.Replace(",", ", ");
        }

        //Create Vendor Product Order Receipt.
        public virtual string CreateVendorProductOrderReceipt(string template, string vendorCode)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            //order to bind order details in data table
            DataTable orderTable = SetOrderData();

            //create order line Item
            DataTable orderlineItemTable = CreateOrderLineItemTable();

            //create multiple Address
            DataTable multipleAddressTable = CreateOrderAddressTable();

            //bind line item data
            BuildOrderLineItemForVendorProduct(multipleAddressTable, orderlineItemTable, vendorCode);

            ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(template);

            // Parse order table
            receiptHelper.Parse(orderTable.CreateDataReader());

            // Parse order line items table
            receiptHelper.Parse("AddressItems", multipleAddressTable.CreateDataReader());
            foreach (DataRow address in multipleAddressTable.Rows)
            {
                // Parse OrderLineItem
                var filterData = orderlineItemTable.DefaultView;
                filterData.RowFilter = $"OmsOrderShipmentID={address["OmsOrderShipmentID"]}";
                receiptHelper.Parse("LineItems" + address["OmsOrderShipmentID"], filterData.ToTable().CreateDataReader());
            }

            // Return the HTML output
            return receiptHelper.Output;
        }

        //Build Order LineI tem For Vendor Products.
        public virtual void BuildOrderLineItemForVendorProduct(DataTable multipleAddressTable, DataTable orderLineItemTable, string vendorCode)
        {
            List<OrderLineItemModel> OrderLineItemList = Order.OrderLineItems.Where(x => x.Vendor == vendorCode).GroupBy(p => new { p.OmsOrderShipmentId }).Select(g => g.First()).ToList();
            IEnumerable<OrderShipmentModel> orderShipments = OrderLineItemList.Select(s => s.ZnodeOmsOrderShipment);

            int shipmentCounter = 1;
            // get order line item as per order shipment.
            foreach (var orderShipment in orderShipments)
            {
                var addressRow = multipleAddressTable.NewRow();

                // If multiple shipping addresses then display the address for each group
                if (orderShipments.Count() > 1)
                {
                    addressRow["ShipmentNo"] = $"Shipment #{shipmentCounter++}{orderShipment.ShipName}";
                    addressRow["ShipTo"] = GetOrderShipmentAddress(orderShipment);
                }

                addressRow["OmsOrderShipmentID"] = orderShipment.OmsOrderShipmentId;
                var counter = 0;

                // get order line item of same shipment.
                foreach (OrderLineItemModel lineitem in Order.OrderLineItems.Where(x => x.OmsOrderShipmentId == orderShipment.OmsOrderShipmentId && x.Vendor == vendorCode))
                {
                    var shoppingCartItem = ((IZnodePortalCart)ShoppingCart).AddressCarts.Where(x => x.OrderShipmentID == orderShipment.OmsOrderShipmentId).SelectMany(x => x.ShoppingCartItems.Cast<ZnodeShoppingCartItem>().AsEnumerable().Reverse()).ElementAt(counter++);

                    StringBuilder sb = new StringBuilder();
                    sb.Append(lineitem.ProductName + "<br />");

                    //For binding personalise attribute to Name
                    if (IsNotNull(lineitem.PersonaliseValueList))
                    {
                        sb.Append(GetPersonaliseAttributes(lineitem.PersonaliseValueList));
                    }
                    else
                    {
                        sb.Append(GetPersonaliseAttributesDetail(lineitem.PersonaliseValuesDetail));
                    }
                    decimal extendedPrice = lineitem.OrderLineItemRelationshipTypeId.Equals((int)ZnodeCartItemRelationshipTypeEnum.Group) ? lineitem.Price : lineitem.Price * lineitem.Quantity;
                    if (IsNotNull(orderLineItemTable))
                    {
                        DataRow orderlineItemDbRow = orderLineItemTable.NewRow();
                        orderlineItemDbRow["Name"] = sb.ToString();
                        orderlineItemDbRow["SKU"] = lineitem.Sku;
                        orderlineItemDbRow["Description"] = lineitem.Description;
                        orderlineItemDbRow["UOMDescription"] = string.Empty;
                        orderlineItemDbRow["Quantity"] = lineitem.OrderLineItemRelationshipTypeId.Equals((int)ZnodeCartItemRelationshipTypeEnum.Group) ? lineitem.GroupProductQuantity : Convert.ToString(double.Parse(Convert.ToString(lineitem.Quantity)));
                        orderlineItemDbRow["Price"] = GetFormatPriceWithCurrency(shoppingCartItem.UnitPrice, shoppingCartItem.UOM);
                        orderlineItemDbRow["ExtendedPrice"] = GetFormatPriceWithCurrency(shoppingCartItem.ExtendedPrice);
                        orderlineItemDbRow["OmsOrderShipmentID"] = lineitem.OmsOrderShipmentId;
                        orderlineItemDbRow["ShortDescription"] = shoppingCartItem.Product.ShortDescription;
                        orderLineItemTable.Rows.Add(orderlineItemDbRow);
                    }
                }

                multipleAddressTable.Rows.Add(addressRow);
            }
        }

        public virtual void setGroupProductDetails(OrderLineItemModel lineitem, OrderLineItemModel orderLineItem)
        {
            if (orderLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)
            {
                lineitem.OrderLineItemRelationshipTypeId = (int)ZnodeCartItemRelationshipTypeEnum.Group;
                lineitem.GroupProductQuantity = lineitem.GroupProductQuantity + "<br/>" + double.Parse(orderLineItem.Quantity.ToString());
                lineitem.Description = orderLineItem.Description + "<br/>" + orderLineItem.ProductName;
                lineitem.GroupProductPrice = lineitem.GroupProductPrice + "<br/>" + GetFormatPriceWithCurrency(orderLineItem.Price);
                lineitem.Sku = orderLineItem.Sku;
            }
            if (orderLineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Configurable)
            {
                lineitem.OrderLineItemRelationshipTypeId = (int)ZnodeCartItemRelationshipTypeEnum.Configurable;
                lineitem.Sku = orderLineItem.Sku;
                lineitem.Price = orderLineItem.Price;
                lineitem.Quantity = orderLineItem.Quantity;
            }
        }

        //to get amount with currency symbol
        public virtual string GetFormatPriceWithCurrency(decimal priceValue, string uom = "", string cultureCode = "")
        {
            if (_cultureCode == null || _cultureCode == "")
                _cultureCode = cultureCode;
            return ZnodeCurrencyManager.FormatPriceWithCurrency(priceValue, _cultureCode, uom);
        }

        //Set Tracking Url.
        public virtual string SetTrackingUrl(string trackingNo, string trackingUrl)
        {
            return IsNotNull(trackingUrl) ? $"<a target=_blank href={ trackingUrl + trackingNo }>{trackingNo} </ a >" : trackingNo;
        }

        #endregion Private Methods

        #region Public Methods

        // Gets the HTML used when showing receipts in the UI.
        public virtual string GetOrderReceiptHtml(string templatePath, string templateCode = "")
        => GenerateOrderReceipt(templatePath, templateCode);

        // Gets the HTML used when showing receipts in the UI.
        public virtual string GetVendorProductOrderReceiptHtml(string templatePath, string vendorCode)
            => GenerateVendorProductOrderReceipt(templatePath, vendorCode);

        public virtual string GetOrderResendReceiptHtml(string templatePath, string templateCode = "")
        => GenerateHtmlResendReceiptWithParser(templatePath, templateCode);

        //Generates the HTML used in email receipts.
        public virtual string GenerateOrderReceipt(string templateContent, string templateCode = "")
        {
            //TO set initial template for Order Receipt
            string receiptTemplate = templateContent;

            return CreateOrderReceipt(receiptTemplate, templateCode);
        }

        //Set tracking number with link to the selected shipping type
        public virtual void SetOrderTrackingNumber(OrderModel order, DataRow orderRow) => orderRow["TrackingNumber"] = SetTrackingURL(order);


        public virtual string SetTrackingURL(OrderModel ordermodel)
        {
            //Get shipping type name based on provided shipping id
            string shippingType = GetShippingType(ordermodel.ShippingId);
            switch (shippingType?.ToLower())
            {
                case ZnodeConstant.UPS:
                    return $"<a target=_blank href={ ZnodeApiSettings.UPSTrackingURL }>{ordermodel.TrackingNumber} </a >";
                case ZnodeConstant.FedEx:
                    return $"<a target=_blank href={ ZnodeApiSettings.FedExTrackingURL }>{ordermodel.TrackingNumber} </a >";
                case ZnodeConstant.USPS:
                    return $"<a target=_blank href={ ZnodeApiSettings.USPSTrackingURL }>{ordermodel.TrackingNumber} </a >";
                default:
                    return ordermodel.TrackingNumber;
            }
        }

        //Get shipping type name based on provided shipping id
        public virtual string GetShippingType(int shippingId)
        {
            IZnodeRepository<ZnodeShipping> _shippingRepository = new ZnodeRepository<ZnodeShipping>();
            IZnodeRepository<ZnodeShippingType> _shippingTypeRepository = new ZnodeRepository<ZnodeShippingType>();
            return (from shipping in _shippingRepository.Table
                    join shippingType in _shippingTypeRepository.Table
                        on shipping.ShippingTypeId equals shippingType.ShippingTypeId
                    where shipping.ShippingId == shippingId
                    select shippingType.Name).FirstOrDefault();
        }

        public virtual string CreateLowInventoryNotification(string template)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            //order to bind order details in data table
            DataTable orderTable = SetLowInventoryOrderData();

            //create order line Item
            DataTable orderlineItemTable = SetOrderLineItemData();

            ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(template);

            // Parse order table
            receiptHelper.Parse(orderTable.CreateDataReader());

            // Parse OrderLineItem
            var filterData = orderlineItemTable.DefaultView;

            List<DataTable> group = filterData.ToTable().AsEnumerable()
            .GroupBy(r => new { Col1 = r["SKU"] })
            .Select(g => g.CopyToDataTable()).ToList();

            receiptHelper.ParseWithGroup("LineItems", group);

            // Return the HTML output
            return receiptHelper.Output;
        }

        //to set order line item details in data table
        protected DataTable SetOrderLineItemData()
        {
            //create order line Item
            DataTable orderlineItemTable = new DataTable();
            orderlineItemTable.Columns.Add("SKU");
            orderlineItemTable.Columns.Add("Quantity");
            orderlineItemTable.Columns.Add("GroupingRowspan");
            orderlineItemTable.Columns.Add("GroupingDisplay");
            IZnodeOrderHelper helper = GetService<IZnodeOrderHelper>();
            //bind line item data
            foreach (var item in ShoppingCart.LowInventoryProducts)
            {
                if (orderlineItemTable != null)
                {
                    DataRow orderlineItemDbRow = orderlineItemTable.NewRow();
                    orderlineItemDbRow["SKU"] = Convert.ToString(item.SKU);
                    orderlineItemDbRow["Quantity"] = helper.GetInventoryRoundOff(Convert.ToString(item.Quantity));
                    orderlineItemTable.Rows.Add(orderlineItemDbRow);
                }
            }

            return orderlineItemTable;
        }

        //to set order details in data table
        protected DataTable SetLowInventoryOrderData()
        {
            // Create new row
            DataTable orderTable = CreateLowInventoryOrderTable();
            DataRow orderRow = orderTable.NewRow();
            IZnodeOrderHelper helper = GetService<IZnodeOrderHelper>();
            PortalModel portal = helper.GetPortalDetailsByPortalId(Order.PortalId);
            // Additional info
            orderRow["SiteName"] = portal?.StoreName ?? ZnodeConfigManager.SiteConfig.StoreName;
            orderRow["StoreLogo"] = helper.SetPortalLogo(Order.PortalId);

            //Customer info
            orderRow["OrderId"] = Order?.Order?.OrderNumber;
            orderRow["OrderDate"] = Order.OrderDateWithTime;

            // Add rows to order table
            orderTable.Rows.Add(orderRow);
            return orderTable;
        }



        //to create order table
        protected  DataTable CreateLowInventoryOrderTable()
        {
            DataTable orderTable = new DataTable();
            // store info
            orderTable.Columns.Add("SiteName");
            orderTable.Columns.Add("StoreLogo");

            // order info
            orderTable.Columns.Add("OrderId");
            orderTable.Columns.Add("OrderDate");

            return orderTable;
        }

        //gets +/- signs based on the order summary field label.
        protected virtual string GetSignBasedOnSummaryTitle(string title)
            => TitleSignDictionary.Where(x => title.Equals(x.Key)).Select(x => x.Value).FirstOrDefault() ?? string.Empty;

        #region Downloadable product keys receipt.
        //Generates the HTML used in email receipts.
        public virtual string GenerateProductKeysOrderReceipt(string templateContent, DownloadableProductKeyListModel key)
        {
            //To set initial template for Order Receipt.
            string receiptTemplate = templateContent;

            return CreateProductKeysOrderReceipt(receiptTemplate, key);
        }

        // Gets the HTML used when showing receipts in the UI.
        public virtual string GetProductKeysOrderReceiptHtml(string templatePath, DownloadableProductKeyListModel key)
        => GenerateProductKeysOrderReceipt(templatePath, key);


        // Builds the order line item table.
        public virtual void BuildOrderLineItemOfProductKeys(DataTable orderLineItemTable, DownloadableProductKeyListModel key)
        {
            //Added one column as keys.
            orderLineItemTable.Columns.Add("Keys");

            foreach (DownloadableProductKeyModel lineitem in key?.DownloadableProductKeys.AsEnumerable().Reverse())
            {
                if (orderLineItemTable != null)
                {
                    // Create new row
                    DataRow orderRow = orderLineItemTable.NewRow();
                    IZnodeOrderHelper helper = GetService<IZnodeOrderHelper>();
                    PortalModel portal = helper.GetPortalDetailsByPortalId(Order.PortalId);
                    _currencyCode = Order.CurrencyCode;
                    _cultureCode = Order.CultureCode;
                    // Additional info
                    orderRow["SiteName"] = portal?.StoreName ?? ZnodeConfigManager.SiteConfig.StoreName;
                    orderRow["StoreLogo"] = helper.SetPortalLogo(Order.PortalId);
                    orderRow["ReceiptText"] = string.Empty;
                    orderRow["CustomerServiceEmail"] = FormatStringComma(portal?.CustomerServiceEmail) ?? FormatStringComma(ZnodeConfigManager.SiteConfig.CustomerServiceEmail);
                    orderRow["CustomerServicePhoneNumber"] = portal?.CustomerServicePhoneNumber.Trim() ?? ZnodeConfigManager.SiteConfig.CustomerServicePhoneNumber.Trim();
                    orderRow["FeedBack"] = FeedbackUrl;
                    orderRow["ShippingName"] = Order?.ShippingName;

                    //Payment info
                    if (!String.IsNullOrEmpty(Order.PaymentTransactionToken))
                    {
                        orderRow["CardTransactionID"] = Order.PaymentTransactionToken;
                        orderRow["CardTransactionLabel"] = Admin_Resources.LabelTransactionId;
                    }

                    orderRow["PaymentName"] = Order.PaymentDisplayName;

                    if (!String.IsNullOrEmpty(Order.PurchaseOrderNumber))
                    {
                        orderRow["PONumber"] = Order.PurchaseOrderNumber;
                        orderRow["PurchaseNumberLabel"] = Admin_Resources.LabelPurchaseOrderNumber;
                    }

                    orderRow["Name"] = Order?.Order?.OrderNumber;

                    //Customer info
                    orderRow["OrderId"] = Order?.Order?.OrderNumber;
                    orderRow["OrderDate"] = Order.OrderDateWithTime;

                    orderRow["BillingAddress"] = GetOrderBillingAddress(Order.BillingAddress);
                    orderRow["PromotionCode"] = Order.CouponCode;

                    var addresses = ((IZnodePortalCart)ShoppingCart).AddressCarts;
                    orderRow["ShippingAddress"] = addresses.Count > 1 ? Admin_Resources.MessageKeyShippingMultipleAddress : GetOrderShipmentAddress(Order.OrderLineItems.FirstOrDefault().ZnodeOmsOrderShipment);

                    orderRow["TotalCost"] = GetFormatPriceWithCurrency(Order.OrderTotalWithoutVoucher);
                    if (Order.AdditionalInstructions != null)
                    {
                        orderRow["AdditionalInstructions"] = Order.AdditionalInstructions;
                        orderRow["AdditionalInstructLabel"] = Admin_Resources.LabelAdditionalNotes;
                    }

                    orderRow["SKU"] = lineitem.SKU;
                    orderRow["Name"] = lineitem.ProductName;
                    orderRow["Keys"] = lineitem.DownloadableProductKey;
                    orderLineItemTable.Rows.Add(orderRow);
                }
            }
        }

        //to create order order line item table
        public virtual DataTable CreateOrderLineItemTableForProductKeys()
        {
            DataTable orderTable = new DataTable();
            orderTable.Columns.Add("Name");
            orderTable.Columns.Add("SKU");
            orderTable.Columns.Add("Quantity");
            // Additional info
            orderTable.Columns.Add("SiteName");
            orderTable.Columns.Add("StoreLogo");
            orderTable.Columns.Add("ReceiptText");
            orderTable.Columns.Add("CustomerServiceEmail");
            orderTable.Columns.Add("CustomerServicePhoneNumber");
            orderTable.Columns.Add("FeedBack");
            orderTable.Columns.Add("AdditionalInstructions");
            orderTable.Columns.Add("AdditionalInstructLabel");

            // Payment info
            orderTable.Columns.Add("CardTransactionID");
            orderTable.Columns.Add("CardTransactionLabel");
            orderTable.Columns.Add("PaymentName");

            orderTable.Columns.Add("PONumber");
            orderTable.Columns.Add("PurchaseNumberLabel");

            // Customer info
            orderTable.Columns.Add("OrderId");
            orderTable.Columns.Add("OrderDate");
            orderTable.Columns.Add("UserId");
            orderTable.Columns.Add("BillingAddress");
            orderTable.Columns.Add("ShippingAddress");
            orderTable.Columns.Add("PromotionCode");
            orderTable.Columns.Add("TotalCost");
            // Returned total cost
            orderTable.Columns.Add("ReturnedTotalCost");
            orderTable.Columns.Add("StyleSheetPath");

            orderTable.Columns.Add("ShippingName");
            orderTable.Columns.Add("TrackingNumber");

            return orderTable;
        }

        //Method to create keys for order receipt.
        public virtual string CreateProductKeysOrderReceipt(string template, DownloadableProductKeyListModel key)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            //create order line Item
            DataTable orderTable = CreateOrderLineItemTableForProductKeys();

            BuildOrderLineItemOfProductKeys(orderTable, key);

            ZnodeReceiptHelper receiptHelper = new ZnodeReceiptHelper(template);

            // Parse order table
            receiptHelper.Parse("DownloadableProductKey", orderTable.CreateDataReader());

            // Return the HTML output
            return receiptHelper.Output;
        }

        // Builds the order amount table.
        protected virtual void BuildTaxSummaryOrderTable(DataTable orderAmountTable)
        {
            var row = orderAmountTable.NewRow();
            row["TaxName"] = "Tax Name";
            row["Rates"] = "Rates";
            row["Amount"] = "Taxes/Fees";

            orderAmountTable.Rows.Add(row);
        }

        //to create order amount table
        protected virtual DataTable CreateSummaryTaxTable()
        {
            DataTable orderAmountTable = new DataTable();
            orderAmountTable.Columns.Add("TaxName");
            orderAmountTable.Columns.Add("Rates");
            orderAmountTable.Columns.Add("Amount");
            return orderAmountTable;
        }

        //to create order amount table
        protected virtual DataTable SetTaxSummaryHeadData()
        {
            DataTable orderAmountTable = new DataTable();
            orderAmountTable.Columns.Add("TaxName");
            orderAmountTable.Columns.Add("Rates");
            orderAmountTable.Columns.Add("Amount");
            BuildTaxSummaryOrderTable(orderAmountTable);

            return orderAmountTable;
        }

        // Builds the order amount table.
        protected virtual void BuildTaxAmountTable(DataTable orderTaxAmountTable)
        {
            if (IsNotNull(Order?.TaxSummaryList) && Order?.TaxSummaryList?.Count() > 0)
            {
                foreach (TaxSummaryModel taxSummary in Order.TaxSummaryList)
                {
                    BindSummaryLineDetails(taxSummary, orderTaxAmountTable);
                }
            }
            else if (IsNotNull(OrderModel?.TaxSummaryList) && OrderModel?.TaxSummaryList?.Count() > 0)
            {
                foreach (TaxSummaryModel taxSummary in OrderModel.TaxSummaryList)
                {
                    BindSummaryLineDetails(taxSummary, orderTaxAmountTable);
                }
            }
        }

        //to set order amount data
        protected virtual DataTable SetTaxSummaryData()
        {
            // Create order amount table
            DataTable orderAmountTable = CreateSummaryTaxTable();

            BuildTaxAmountTable(orderAmountTable);

            return orderAmountTable;
        }

        protected virtual void BindSummaryLineDetails(TaxSummaryModel taxSummary, DataTable orderTaxAmountTable)
        {
            if (taxSummary?.Tax.GetValueOrDefault() > 0)
            {
                var row = orderTaxAmountTable.NewRow();

                row["TaxName"] = taxSummary.TaxName;
                row["Rates"] = taxSummary.Rate;
                row["Amount"] = GetFormatPriceWithCurrency(taxSummary.Tax.GetValueOrDefault());

                orderTaxAmountTable.Rows.Add(row);
            }
        }

        protected virtual void BindTaxSummaryDetails(DataTable orderAmountTable)
        {
            if (IsNotNull(Order?.TaxSummaryList) && Order?.TaxSummaryList?.Count() > 0)
            {
                foreach (TaxSummaryModel taxSummary in Order.TaxSummaryList)
                {
                    BuildOrderAmountTable(taxSummary.TaxName, taxSummary.Tax.GetValueOrDefault(), orderAmountTable);
                }
            }
        }

        protected virtual bool IsTaxSummaryAvailable()
        {
            return (IsNotNull(Order?.TaxSummaryList) && Order?.TaxSummaryList?.Count() > 0 || IsNotNull(OrderModel?.TaxSummaryList) && OrderModel?.TaxSummaryList?.Count() > 0);
        }

        #endregion
        #endregion Public Methods
    }
}