using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Znode.Engine.Admin.ViewModels;

namespace Znode.Admin.Core.ViewModels
{
    public class ExportProcessLogsViewModel : BaseViewModel
    {
        public int ExportProcessLogId { get; set; }
        public string ExportType { get; set; }
        public string Status { get; set; }
        public DateTime? ProcessStartedDate { get; set; }
        public DateTime? ProcessCompletedDate { get; set; }
        public string TableName { get; set; }
        public string FileType { get; set; }
    }
}
