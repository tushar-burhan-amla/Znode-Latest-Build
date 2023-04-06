namespace Znode.Engine.Api.Client.Endpoints
{
    public class AccountQuoteEndpoint : BaseEndpoint
    {
        //Get account quote list.
        public static string GetAccountQuoteList() => $"{ApiRoot}/accountquote/list";

        //Get account quote by omsQuoteId.
        public static string GetAccountQuote() => $"{ApiRoot}/accountquote/get";

        //Create account quote.
        public static string Create() => $"{ApiRoot}/accountquote/create";

        //Update account quote.
        public static string UpdateQuoteStatus() => $"{ApiRoot}/accountquote/updatequotestatus";

        //Update account quote line item quantity.
        public static string UpdateQuoteLineItemQuantity() => $"{ApiRoot}/accountquote/updatequotelineitemquantity";

        //Update account quote line item quantities.
        public static string UpdateQuoteLineItemQuantities() => $"{ApiRoot}/accountquote/updatequotelineitemquantities";

        //Delete account quote line item by omsQuoteLineItemId.
        public static string DeleteQuoteLineItem(int omsQuoteLineItemId, int omsQuoteId) => $"{ApiRoot}/accountquote/deletequotelineitem/{omsQuoteLineItemId}/{omsQuoteId}";

        //Get user approver list.
        public static string GetUserApproverList() => $"{ApiRoot}/accountquote/getuserapproverlist";

        //Method to check if the current user is an approver to any other user and has approvers itself.
        public static string UserApproverDetails(int userId) => $"{ApiRoot}/accountquote/userapproverdetails/{userId}";

        //Method to check if the current user is tha final approver for the quote.
        public static string IsLastApprover(int quoteId) => $"{ApiRoot}/accountquote/islastapprover/{quoteId}";

        //Get billing account number.
        public static string GetBillingAccountNumber(int userId) => $"{ApiRoot}/accountquote/getbillingaccountnumber/{userId}";

        //Method to check if the current user is an approver to any other user and has approvers itself.
        public static string UserDashboardPendingOrderDetailsCount() => $"{ApiRoot}/accountquote/userdashboardpendingorderdetailscount";

        #region Template
        //Create account template.
        public static string CreateTemplate() => $"{ApiRoot}/accountquote/createtemplate";

        //Get account template list.
        public static string GetTemplateList() => $"{ApiRoot}/accountquote/gettemplatelist";

        //Delete template Endpoint.
        public static string DeleteTemplate() => $"{ApiRoot}/accountquote/deletetemplate";

        //Delete cart item endpoint.
        public static string DeleteCartItem() => $"{ApiRoot}/accountquote/deletecartitem";

        //Gets an account template.
        public static string GetAccountTemplate(int omsTemplateId) => $"{ApiRoot}/accountquote/gettemplate/{omsTemplateId}";
        #endregion
    }
}
