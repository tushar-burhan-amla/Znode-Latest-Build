using Microsoft.PowerBI.Api.V2.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class PowerBIEmbedViewModel
    {
        public string ReportId { get; set; }

        public string EmbedUrl { get; set; }

        public EmbedToken EmbedToken { get; set; }

        public string ErrorMessage { get; set; }
    }
}