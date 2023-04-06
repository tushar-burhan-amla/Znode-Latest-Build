namespace Znode.Engine.Api.Client.Endpoints
{
    public class RMAConfigurationEndpoint : BaseEndpoint
    {
        #region RMA Configuration
        //Create RMA Configuration Endpoint.
        public static string CreateRMAConfiguration() => $"{ApiRoot}/rmaconfiguration/creatermaconfiguration";

        //Get RMA Configuration details Endpoint.
        public static string GetRMAConfiguration() => $"{ApiRoot}/rmaconfiguration/getrmaconfiguration";
        #endregion

        #region Reason For Return
        //Get Reason For Request list Endpoint.
        public static string GetReasonForReturnList() => $"{ApiRoot}/rmaconfiguration/getreasonforreturnlist";

        //Create Reason For Return Endpoint.
        public static string CreateReasonForReturn() => $"{ApiRoot}/rmaconfiguration/createreasonforreturn";

        //Get Reason For Return on the basis of Reason For Return Id Endpoint.
        public static string GetReasonForReturn(int rmaReasonForReturnId) => $"{ApiRoot}/rmaconfiguration/getreasonforreturn/{rmaReasonForReturnId}";

        //Update Reason For Return Endpoint.
        public static string UpdateReasonForReturn() => $"{ApiRoot}/rmaconfiguration/updatereasonforreturn";

        //Delete Reason For Return Endpoint.
        public static string DeleteReasonForReturn() => $"{ApiRoot}/rmaconfiguration/deletereasonforreturn";
        #endregion

        #region Request Status
        //Get Request Status list Endpoint.
        public static string GetRequestStatusList() => $"{ApiRoot}/rmaconfiguration/getrequeststatuslist";

        //Get Request Status on the basis of Request Status Id Endpoint.
        public static string GetRequestStatus(int rmaRequestStatusId) => $"{ApiRoot}/rmaconfiguration/getrequeststatus/{rmaRequestStatusId}";

        //Update Request Status Endpoint.
        public static string UpdateRequestStatus() => $"{ApiRoot}/rmaconfiguration/updaterequeststatus";

        //Delete Request Status Endpoint.
        public static string DeleteRequestStatus() => $"{ApiRoot}/rmaconfiguration/deleterequeststatus";
        #endregion
    }
}
