namespace Znode.Engine.Api.Client.Endpoints
{
    public class CustomerReviewEndpoint : BaseEndpoint
    {
        //Get Customer Review list Endpoint.
        public static string GetCustomerReviewList(string localeId) => $"{ApiRoot}/customerreviewlist/{localeId}";

        //Get Customer Review on the basis of Customer Review id Endpoint.
        public static string GetCustomerReview(int customerReviewId, string localeId) => $"{ApiRoot}/customerreview/getcustomerreview/{customerReviewId}/{localeId}";

        //Update Customer Review Endpoint.
        public static string UpdateCustomerReview() => $"{ApiRoot}/customerreview/updatecustomerreview";

        //Delete Customer Review Endpoint.
        public static string DeleteCustomerReview() => $"{ApiRoot}/customerreview/deletecustomerreview";

        //Change status of customer review endpoint.
        public static string BulkStatusChange(string statusId) => $"{ApiRoot}/customerreview/bulkstatuschange/{statusId}";

        //Create customer review
        public static string CreateCustomerReview() => $"{ApiRoot}/customerreview/create";
    }
}
