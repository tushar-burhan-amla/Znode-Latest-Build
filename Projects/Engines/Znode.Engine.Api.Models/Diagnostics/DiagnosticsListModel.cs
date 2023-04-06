using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class DiagnosticsListModel : BaseListModel
    {
        public DiagnosticsListModel()
        {
            DiagnosticsList = new List<DiagnosticsModel>();
        }
        public List<DiagnosticsModel> DiagnosticsList { get; set; }
    }
}
