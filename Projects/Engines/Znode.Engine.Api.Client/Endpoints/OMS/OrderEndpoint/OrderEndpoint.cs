namespace Znode.Engine.Api.Client.Endpoints
{
    public class OrderEndpoint : BaseEndpoint
    {
        // Get order list endpoint.
        public static string List() => $"{ApiRoot}/orders/list";

        // Get order list endpoint.
        public static string GetGroupOrderList() => $"{ApiRoot}/orders/getgrouporderlist";

        //Create new order endpoint.
        public static string Create() => $"{ApiRoot}/orders/create";

        //update existing order endpoint.
        public static string Update() => $"{ApiRoot}/orders/update";

        //To get the order details for payment.
        public static string GetOrderDetailsForPayment() => $"{ApiRoot}/orders/getorderdetailsforpayment";

        // Get order details by order id.
        public static string Get(int orderId) => $"{ApiRoot}/orders/{orderId}";

        //Create new customer endpoint.
        public static string CreateNewCustomer() => $"{ApiRoot}/orders/createnewcustomer";

        //Get Order invoice details
        public static string GetOrderDetailsForInvoice() => $"{ApiRoot}/orders/GetOrderDetailsForInvoice";

        //Update Order Payment Status
        public static string UpdateOrderPaymentStatus(int orderId, string paymentStatus) => $"{ApiRoot}/orders/UpdateOrderPaymentStatus/{orderId}/{paymentStatus}";

        //update existing order Shipping Billing address endpoint.
        public static string UpdateOrderAddress() => $"{ApiRoot}/orders/UpdateOrderAddress";

        //Update Order Tracking Number
        public static string UpdateTrackingNumber(int orderId, string trackingNumber) => $"{ApiRoot}/orders/updatetrackingnumber/{orderId}/{trackingNumber}";

        //Get OrderLine Items With Refund payment left 
        public static string GetOrderLineItemsWithRefund(int orderDetailsId) => $"{ApiRoot}/orders/getorderlineitemswithrefund/{orderDetailsId}";

        // Add Refund payment details
        public static string AddRefundPaymentDetails() => $"{ApiRoot}/orders/addrefundpaymentdetails";

        // Resend order confirmation email
        public static string ResendOrderConfirmationEmail(int orderId) => $"{ApiRoot}/orders/resendorderconfirmationemail/{orderId}";

        //Resend order confirmation mail for single cart.
        public static string ResendOrderEmailForCartLineItem(int orderId, int cartItemId) => $"{ApiRoot}/orders/ResendOrderEmailForCartLineItem/{orderId}/{cartItemId}";

        //Update order status
        public static string UpdateOrderStatus() => $"{ApiRoot}/orders/updateorderstatus";

        //Update Order status, external id and order notes by order number.
        public static string UpdateOrderDetailsByOrderNumber() => $"{ApiRoot}/orders/updateorderdetailsbyordernumber";
        
        // Get order by order line item id.
        public static string GetOrderByOrderLineItemId(int orderLineItemId) => $"{ApiRoot}/orders/getorderbyorderlineitemid/{orderLineItemId}";

        //Add Order Note
        public static string AddOrderNote() => $"{ApiRoot}/orders/addordernote";

        //Get Order invoice details
        public static string GetOrderNotesList(int omsOrderId, int omsQuoteId) => $"{ApiRoot}/orders/GetOrderNotesList/{omsOrderId}/{omsQuoteId}";

        // Check inventory and min/max quantity endpoint.
        public static string CheckInventoryAndMinMaxQuantity() => $"{ApiRoot}/orders/checkinventoryandminmaxquantity";

        //For getting the order state list
        public static string GetPaymentStateList() => $"{ApiRoot}/orders/getpaymentstatelist";

        //Create new order History endpoint.
        public static string createOrderHistory() => $"{ApiRoot}/orders/createorderhistory";

        //Get Order State by Id.
        public static string GetOrderStateValueById(int omsOrderStateId) => $"{ApiRoot}/orders/getorderstatevaluebyid/{omsOrderStateId}";

        // Send returned order email.
        public static string SendReturnedOrderEmail(int orderId) => $"{ApiRoot}/orders/sendreturnedorderemail/{orderId}";

        //Send purchase order email.
        public static string SendPOEmail() => $"{ApiRoot}/Order/SendPOEmail";

        //Update Order TransactionId
        public static string UpdateOrderTransactionId(int orderId, string transactionId) => $"{ApiRoot}/orders/updateorderoransactionid/{orderId}/{transactionId}";

        //Convert to Order Endpoint.
        public static string ConvertToOrder() => $"{ApiRoot}/orders/converttoorder";

        //Single reorder
        public static string ReorderSinglelineItemOrder(int OmsOrderLineItemsId, int portalId, int userId) => $"{ApiRoot}/orders/ReorderSinglelineItemOrder/{OmsOrderLineItemsId}/{portalId}/{userId}";

        //Reorder Complete Order
        public static string ReorderCompleteOrder(int orderId, int portalId, int userId) => $"{ApiRoot}/orders/reordercompleteorder/{orderId}/{portalId}/{userId}";

        //Get customer address list.
        public static string GetAddressListWithShipment(int orderId, int userId) => $"{ApiRoot}/orders/GetAddressListWithShipment/{orderId}/{userId}";

        // Order Receipt Details by order Id
        public static string GetOrderReceiptByOrderId(int orderId) => $"{ApiRoot}/orders/getorderreceiptbyorderid/{orderId}";

        // Get order list endpoint.
        public static string FailedOrderTransactionList() => $"{ApiRoot}/orders/FailedOrderTransactionList";

        //Save Order Payment Detail.
        public static string SaveOrderPaymentDetail() => $"{ApiRoot}/orders/saveorderpaymentdetail";

    }
}
