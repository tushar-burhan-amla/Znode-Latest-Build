using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReportListViewModel : BaseViewModel
    {
        public ReportListViewModel()
        {
            ReportList = new List<SelectListItem>();
            GridModel = new GridModel();
            Reports = new List<ReportViewModel>();
        }

        public List<SelectListItem> ReportList { get; set; }
        public List<ReportViewModel> Reports { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorReportRequired)]
        public string Path { get; set; }
        public GridModel GridModel { get; set; }
    }
}