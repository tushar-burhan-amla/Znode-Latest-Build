using System;

namespace Znode.Engine.Api.Models
{
    public class ProgressNotificationModel
    {
        public Guid JobId { get; set; }
        public string JobName { get; set; }
        public int ProgressMark { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
        public string ExceptionMessage { get; set; }
        public int StartedBy { get; set; }
        public string StartedByFriendlyName { get; set; }
    }
}
