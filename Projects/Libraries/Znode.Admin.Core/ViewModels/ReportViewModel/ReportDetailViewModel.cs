using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReportDetailViewModel : BaseViewModel
    {
        public int ReportCategoryId { get; set; }
        public int ReportDetailId { get; set; }     
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        public string Description { get; set; }
    }
}
