using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
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