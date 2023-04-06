using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ImportManageTemplateMappingListViewModel: BaseViewModel
    {
        public List<ImportManageTemplateMappingViewModel> ImportTemplate { get; set; }

        public GridModel GridModel { get; set; }
    }
}
