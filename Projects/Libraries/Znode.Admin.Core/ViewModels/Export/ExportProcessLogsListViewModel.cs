using System.Collections.Generic;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Admin.Core.ViewModels
{
    public class ExportProcessLogsListViewModel : BaseViewModel
    { 
        public ExportProcessLogsListViewModel()
        {
            ProcessLogs = new List<ExportProcessLogsViewModel>();
        }
        public List<ExportProcessLogsViewModel> ProcessLogs { get; set; }
        public GridModel GridModel { get; set; }
    }
}
