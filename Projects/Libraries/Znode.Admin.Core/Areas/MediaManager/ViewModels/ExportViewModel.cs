using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    //View Model for Export File type.
    public class ExportViewModel : BaseViewModel
    {
        public string ExportFileType { get; set; }
        public List<SelectListItem> ExportFileTypes { get; set; }
    }
}