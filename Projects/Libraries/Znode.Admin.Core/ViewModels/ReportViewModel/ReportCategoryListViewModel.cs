using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReportCategoryListViewModel: BaseViewModel
    {
        public ReportCategoryListViewModel()
        {
            ReportCategoryList = new List<ReportCategoryViewModel>();
            ReportDetailList = new List<ReportDetailViewModel>();
        }
        public List<ReportCategoryViewModel> ReportCategoryList { get; set; }
        public List<ReportDetailViewModel> ReportDetailList { get; set; }
    }
}
