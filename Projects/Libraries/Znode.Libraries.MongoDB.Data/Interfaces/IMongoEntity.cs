using MongoDB.Bson;
namespace Znode.Libraries.MongoDB.Data
{
    public interface IMongoEntity
    {
        ObjectId Id { get; set; }
    }
}
