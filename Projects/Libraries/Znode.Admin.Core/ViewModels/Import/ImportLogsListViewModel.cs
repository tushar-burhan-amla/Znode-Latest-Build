using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ImportLogsListViewModel : BaseViewModel
    {
        public ImportLogsListViewModel()
        {
            LogsList = new List<ImportLogsViewModel>();
        }

        public List<ImportLogsViewModel> LogsList { get; set; }
        public GridModel GridModel { get; set; }
        public ImportProcessLogsViewModel ImportLogs { get; set; }
    }
}