namespace Znode.Engine.Api.Client.Endpoints
{
    public class WebStoreCaseRequestEndpoint : BaseEndpoint
    {
        //Create CaseRequest Endpoint.
        public static string CreateContactUs() => $"{ApiRoot}/WebStoreCaseRequest/CreateContactUs";

        //Get the list of case request.
        public static string GetCaseRequests() => $"{ApiRoot}/webstorecaserequest/list";

        //Create Case Request.
        public static string CreateCaseRequest() => $"{ApiRoot}/webstorecaserequest/createcaserequest";

        //Get case request on the basis of caseRequestId.
        public static string GetCaseRequest(int caseRequestId) => $"{ApiRoot}/webstorecaserequest/getcaserequest/{caseRequestId}";

        //Update case request data.
        public static string UpdateCaseRequest() => $"{ApiRoot}/webstorecaserequest/updatecaserequest";

        //Reply to customer.
        public static string ReplyCustomer() => $"{ApiRoot}/webstorecaserequest/replycustomer";

        //Get the list of case priority.
        public static string GetCasePriorityList() => $"{ApiRoot}/webstorecaserequest/caseprioritylist";

        //Get the list of case status.
        public static string GetCaseStatusList() => $"{ApiRoot}/webstorecaserequest/casestatuslist";

        //Get the list of case type.
        public static string GetCaseTypeList() => $"{ApiRoot}/webstorecaserequest/casetypelist";
    }
}
