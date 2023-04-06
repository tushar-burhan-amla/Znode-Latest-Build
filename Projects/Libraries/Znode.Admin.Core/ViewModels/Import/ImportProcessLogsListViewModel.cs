using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ImportProcessLogsListViewModel : BaseViewModel
    {
        public ImportProcessLogsListViewModel()
        {
            ProcessLogs = new List<ImportProcessLogsViewModel>();
        }

        public List<ImportProcessLogsViewModel> ProcessLogs { get; set; }

        public GridModel GridModel { get; set; }
    }
}