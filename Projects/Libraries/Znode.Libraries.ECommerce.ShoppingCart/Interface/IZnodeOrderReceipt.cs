using System.Collections.Generic;
using System.Data;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    public interface IZnodeOrderReceipt
    {
        bool FromApi { get; set; }

        OrderModel OrderModel { get; set; }

        //Create datatable for shipping
        DataTable CreateShippingTable(DataTable shippingTable);

        //Generates the HTML used in email receipts.
        string GenerateVendorProductOrderReceipt(string templateContent, string vendorCode);

        string GenerateHtmlResendReceiptWithParser(string receiptHtml, string templateCode = "");

        // Builds the order line item table.
        void BuildOrderLineItem(DataTable multipleAddressTable, DataTable orderLineItemTable, DataTable multipleTaxAddressTable, OrderModel Order, DataTable returnedOrderlineItemTable, string templateCode = "");

        //Set Order Line Item Table.
        DataRow SetOrderLineItemTable(DataTable orderLineItemTable, OrderLineItemModel lineitem, decimal extendedPrice, string templateCode = "");

        // Set Returned Order Line Item Table.
        DataRow SetReturnedOrderLineItemTable(DataTable returnedOrderLineItemTable, OrderLineItemModel lineitem, decimal extendedPrice);

        //to set order amount data
        DataTable SetOrderAmountData(OrderModel Order, string templateCode = "");

        // To set returned order amount data
        DataTable SetReturnedOrderAmountData(OrderModel order);

        //to set order details
        DataTable SetOrderData(OrderModel Order, string templateCode = "");

        string GetOrderShippingAddress(OrderModel orderShipping);

        string CreateOrderReceipt(string template, string templateCode = "");

        //to create order table
        DataTable CreateOrderTable();

        //to create order amount table
        DataTable CreateOrderAmountTable();

        // To create returned order amount table
        DataTable CreateReturnedOrderAmountTable();

        //to create order order line item table
        DataTable CreateOrderLineItemTable();

        // To create returned order line item table
        DataTable CreateReturnedOrderLineItemTable();

        //to create order address table
        DataTable CreateOrderAddressTable();

        //to create order address table
        DataTable CreateOrderTaxAddressTable();

        //to set order details
        DataTable SetOrderData();

        //to set order amount data
        DataTable SetOrderAmountData();

        // Builds the order amount table.
        void BuildOrderAmountTable(string title, decimal amount, DataTable orderAmountTable);

        // Builds the returned order amount table.
        void BuildReturnedOrderAmountTable(string title, decimal amount, DataTable orderAmountTable, string cultureCode = "");

        // Builds the order line item table.
        void BuildOrderLineItem(DataTable multipleAddressTable, DataTable orderLineItemTable, DataTable multipleTaxAddressTable, string templateCode = "");

        // Builds the Shipment order line item table.
        void BuildOrderShipmentTotalLineItem(string title, decimal amount, int OmsOrderShipmentId, DataTable taxTable);

        //to get order shipment address
        string GetOrderShipmentAddress(OrderShipmentModel orderShipment);

        //to get shipping address
        string GetOrderBillingAddress(AddressModel orderBilling);

        //For Getting personalise attribute.
        string GetPersonaliseAttributes(Dictionary<string, object> personaliseValueList);

        string GetPersonaliseAttributesDetail(List<PersonaliseValueModel> PersonaliseValuesDetail);

        //to add space after comma
        string FormatStringComma(string input);

        //Create Vendor Product Order Receipt.
        string CreateVendorProductOrderReceipt(string template, string vendorCode);

        //Build Order LineI tem For Vendor Products.
        void BuildOrderLineItemForVendorProduct(DataTable multipleAddressTable, DataTable orderLineItemTable, string vendorCode);


        void setGroupProductDetails(OrderLineItemModel lineitem, OrderLineItemModel orderLineItem);


        //to get amount with currency symbol
        string GetFormatPriceWithCurrency(decimal priceValue, string uom = "", string cultureCode = "");

        //Set Tracking Url.
        string SetTrackingUrl(string trackingNo, string trackingUrl);


        // Gets the HTML used when showing receipts in the UI.
        string GetOrderReceiptHtml(string templatePath, string templateCode = "");

        // Gets the HTML used when showing receipts in the UI.
        string GetVendorProductOrderReceiptHtml(string templatePath, string vendorCode);

        string GetOrderResendReceiptHtml(string templatePath, string templateCode = "");

        //Generates the HTML used in email receipts.
        string GenerateOrderReceipt(string templateContent, string templateCode = "");

        //Set tracking number with link to the selected shipping type
        void SetOrderTrackingNumber(OrderModel order, DataRow orderRow);

        string SetTrackingURL(OrderModel ordermodel);

        //Get shipping type name based on provided shipping id
        string GetShippingType(int shippingId);

        //Generates the HTML used in email receipts.
        string GenerateProductKeysOrderReceipt(string templateContent, DownloadableProductKeyListModel key);


        // Gets the HTML used when showing receipts in the UI.
        string GetProductKeysOrderReceiptHtml(string templatePath, DownloadableProductKeyListModel key);


        // Builds the order line item table.
        void BuildOrderLineItemOfProductKeys(DataTable orderLineItemTable, DownloadableProductKeyListModel key);

        //to create order order line item table
        DataTable CreateOrderLineItemTableForProductKeys();


        //Method to create keys for order receipt.
        string CreateProductKeysOrderReceipt(string template, DownloadableProductKeyListModel key);

        // Get the HTML for low inventory order notification
        string CreateLowInventoryNotification(string template);
    }
}