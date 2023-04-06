namespace Znode.Engine.Api.Models.Responses
{
    public class DiagnosticsResponse:BaseResponse
    {
        public string ProductVersion { get; set; }
        public string ServiceStatus { get; set; }
        public DiagnosticsListModel Diagnostics { get; set; }
    }
}
