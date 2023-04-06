using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Admin.ViewModels
{
    public class ImportLogsDownloadListViewModel
    {
        [Display(Name = "ID")]
        public int ImportLogId { get; set; }
        public int ImportProcessLogId { get; set; }
        [Display(Name = "Row Number")]
        public int? RowNumber { get; set; }
        [Display(Name = "CSV Column Name")]
        public string ColumnName { get; set; }
        [Display(Name = "CSV Column Data")]
        public string ColumnValue { get; set; }
        [Display(Name = "Error Description")]
        public string ErrorDescription { get; set; }
        public List<ImportLogDetailsDownloadViewModel> LogsList { get; set; }
    }
}