using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ImportTypeListViewModel : BaseViewModel
    {
        public ImportTypeListViewModel()
        {
            ImportTypeList = new List<ImportViewModel>();
            GridModel = new GridModel();
        }
        public List<ImportViewModel> ImportTypeList { get; set; }
        public GridModel GridModel { get; set; }
    }
}