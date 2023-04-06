namespace Znode.Engine.Api.Client.Endpoints
{
    public class RMARequestEndpoint : BaseEndpoint
    {
        //Get Order RMA Flag
        public static string GetOrderRMAFlag(int omsOrderDetailsId) => $"{ApiRoot}/getorderrmaflag/{omsOrderDetailsId}";

        //Get RMARequestList
        public static string GetRMARequestList() => $"{ApiRoot}/rmarequest/getrmarequestlist";

        //Get RMAGiftCardDetails
        public static string GetRMAGiftCardDetails(int rmaRequestId) => $"{ApiRoot}/rmarequest/getrmagiftcarddetails/{rmaRequestId}";

        //Get RMARequest by rmaRequestId
        public static string Get(int rmaRequestId) => $"{ApiRoot}/rmarequest/getrmarequest/{rmaRequestId}";

        //Update RMARequest
        public static string UpdateRMARequest(int rmaRequestId) => $"{ApiRoot}/rmarequest/updatermarequest/{rmaRequestId}";

        //Create RMA request
        public static string Create() => $"{ApiRoot}/creatermarequest";

        //Check Send RMA Request Status Mail
        public static string SendRMAStatusMail(int rmaRequestId) => $"{ApiRoot}/rmarequest/sendrmastatusmail/{rmaRequestId}";

        //Check Send Gift Card Mail
        public static string SendGiftCardMail() => $"{ApiRoot}/rmarequest/sendgiftcardmail";

    }
}
