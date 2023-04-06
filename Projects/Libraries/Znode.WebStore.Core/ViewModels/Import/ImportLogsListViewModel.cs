using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ImportLogsListViewModel : BaseViewModel
    {
        public ImportLogsListViewModel()
        {
            LogsList = new List<ImportLogsViewModel>();
        }

        public List<ImportLogsViewModel> LogsList { get; set; }
        public GridModel GridModel { get; set; }
    }
}