using System;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ImportProcessLogsViewModel : BaseViewModel
    {
        public int ImportProcessLogId { get; set; }
        public int ImportTemplateId { get; set; }
        public string Status { get; set; }
        public string TemplateName { get; set; }
        public Nullable<DateTime> ProcessStartedDate { get; set; }
        public Nullable<DateTime> ProcessCompletedDate { get; set; }
        public string ImportName { get; set; }
    }
}