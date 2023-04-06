using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Admin.ViewModels
{
    public class ReportCategoryViewModel : BaseViewModel
    {
        public int ReportCategoryId { get; set; }
        public string CategoryName { get; set; }
        public Boolean IsActive { get; set; }
    }
}
