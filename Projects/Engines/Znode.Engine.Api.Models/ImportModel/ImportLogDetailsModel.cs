using System;

namespace Znode.Engine.Api.Models
{
    public class ImportLogDetailsModel : BaseModel
    {
        public int ImportLogId { get; set; }
        public int ImportProcessLogId { get; set; }
        public Int64 RowNumber { get; set; }
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }
        public string ErrorDescription { get; set; }
    }
}
