namespace Znode.Engine.Api.Client.Endpoints
{
    public class ECertEndpoint : BaseEndpoint
    {
        //Get Available ECertificate balance
        public static string GetECertTotalBalance() => $"{ApiRoot}/ecert/GetAvailableECertBalance";

        //Get Available ECertificate list
        public static string GetAvailableECertList() => $"{ApiRoot}/ecert/list";

        //Add eCertificate in the wallet
        public static string AddECertToBalance() => $"{ApiRoot}/ecert/create";
    }
}
