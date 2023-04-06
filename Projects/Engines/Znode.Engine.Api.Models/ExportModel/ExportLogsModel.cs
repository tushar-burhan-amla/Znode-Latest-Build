﻿using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ExportLogsModel : BaseModel
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
