using Microsoft.PowerBI.Api.V2.Models;
using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class PowerBIEmbedViewModel
    {
        public string ReportId { get; set; }
        public string EmbedUrl { get; set; }
        public EmbedToken EmbedToken { get; set; }
        public string ErrorMessage { get; internal set; }
    }
}
