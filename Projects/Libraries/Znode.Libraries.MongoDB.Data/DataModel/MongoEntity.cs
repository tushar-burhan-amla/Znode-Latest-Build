using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Znode.Libraries.MongoDB.Data
{
    public class MongoEntity : IMongoEntity
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public int VersionId { get; set; }

        // We have used this to perform temporary operation on category publish.
        public long? PublishStartTime { get; set; }

        public bool isDisposed { get; set; }
    }

}
