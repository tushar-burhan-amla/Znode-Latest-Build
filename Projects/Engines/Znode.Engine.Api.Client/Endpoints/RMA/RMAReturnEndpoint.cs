namespace Znode.Engine.Api.Client.Endpoints
{
    public class RMAReturnEndpoint : BaseEndpoint
    {
        //Get order details by order number for return.
        public static string GetOrderDetailsForReturn(int userId, string orderNumber, bool isFromAdmin) => $"{ApiRoot}/rmareturn/getorderdetailsforreturn/{userId}/{orderNumber}/{isFromAdmin}";

        //Get Return List
        public static string GetReturnList() => $"{ApiRoot}/rmareturn/list";

        //Check if order is eligible for return
        public static string IsOrderEligibleForReturn(int userId, int portalId, string orderNumber) => $"{ApiRoot}/rmareturn/isordereligibleforreturn/{userId}/{portalId}/{orderNumber}";

        //Get order return details by return number
        public static string GetReturnDetails(string returnNumber) => $"{ApiRoot}/rmareturn/getreturndetails/{returnNumber}";

        //Insert or update order return details.
        public static string SaveOrderReturn() => $"{ApiRoot}/rmareturn/saveorderreturn";

        //Delete order return on the basis of return number.
        public static string DeleteOrderReturn(string returnNumber, int userId) => $"{ApiRoot}/rmareturn/deleteorderreturnbyreturnnumber/{returnNumber}/{userId}";

        //Submit order return.
        public static string SubmitOrderReturn() => $"{ApiRoot}/rmareturn/submitorderreturn";

        // Validate order lineitems for create return
        public static string IsValidReturnItem() => $"{ApiRoot}/rmareturn/isValidReturnItems";

        //Perform calculations for an order return line item.
        public static string CalculateOrderReturn() => $"{ApiRoot}/rmareturn/calculateorderreturn";

        //Get Return Status List
        public static string GetReturnStatusList() => $"{ApiRoot}/rmareturn/getreturnstatuslist";

        //Get order return details for admin by return number
        public static string GetReturnDetailsForAdmin(string returnNumber) => $"{ApiRoot}/rmareturn/getreturndetailsforadmin/{returnNumber}";

        //Create return history
        public static string CreateReturnHistory() => $"{ApiRoot}/rmareturn/createreturnhistory";
        
        //Save Return Notes
        public static string SaveReturnNotes() => $"{ApiRoot}/rmareturn/savereturnnotes";
    }
}
