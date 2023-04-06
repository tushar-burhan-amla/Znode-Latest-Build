using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Znode.Libraries.MongoDB.Data
{
    public class LogMessageEntity : MongoEntity, IDisposable
    {
        public string LogMessageId { get; set; }
        public string Component { get; set; }
        public string TraceLevel { get; set; }
        public string LogMessage { get; set; }
        public string StackTraceMessage { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreatedDate { get; set; }
        public string DomainName { get; set; }

        ~LogMessageEntity()
        {
            if (!isDisposed)
                Dispose();
        }
        public void Dispose()
        {
            isDisposed = true;
        }
    }
}
