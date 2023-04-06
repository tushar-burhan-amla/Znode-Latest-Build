namespace Znode.Engine.Api.Client.Endpoints
{
    public class SMSEndpoint : BaseEndpoint
    {
        //Get SMS Endpoint
        public static string GetSMSSetting(int portalId,bool isSMSSettingEnabled = false) => $"{ApiRoot}/sms/Get/{portalId}/{isSMSSettingEnabled}";

        //Get SMS Provider List Endpoint
        public static string GetSMSProviderList() => $"{ApiRoot}/sms/GetSmsProviderList/";

        //Update SMS Setting Endpoint
        public static string InsertUpdateSMSSetting() => $"{ApiRoot}/sms/insertupdatesmssetting";
    }
}
