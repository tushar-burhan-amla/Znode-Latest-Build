namespace Znode.Engine.Api.Client.Endpoints
{
    public class DiagnosticsEndpoint:BaseEndpoint
    {

        //for GetProductVersionDetails Endpoint.
        public static string GetProductVersionDetails() => $"{ApiRoot}/Diagnostics/GetProductVersionDetails";

        //for EmailDiagnostics Endpoint.
        public static string EmailDiagnostics() => $"{ApiRoot}/Diagnostics/EmailDiagnostics";

        //for GetDiagnosticsList Endpoint
        public static string GetDiagnosticsList() => $"{ApiRoot}/Diagnostics/GetDiagnosticsList";
    }
}
