namespace Znode.Engine.Admin.ViewModels
{
    public class ImportLogsViewModel : BaseViewModel
    {
        public int ImportLogId { get; set; }
        public int ImportProcessLogId { get; set; }
        public int? RowNumber { get; set; }
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }
        public string ErrorDescription { get; set; }
    }
}