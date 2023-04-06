using MongoDB.Driver;
using System.Collections.Generic;

namespace Znode.Libraries.MongoDB.Data
{
    public interface IMongoRepository<T>  
    {

        /// <summary>
        /// Used to Query the Entity
        /// </summary>
        MongoConnectionHandler<T> Table { get; }

        /// <summary>
        /// Create/Insert document in mongo collection 
        /// If Mongo DB and collection is exists it will add document in existing db and collection.
        /// If DB and Collection is not exists then it will create first and add document in to it.
        /// </summary>
        /// <param name="entity">Mongo DB Entity (Document)</param>
        void Create(T entity);

        /// <summary>
        /// Create/Insert multiple documents in mongo collection 
        /// If Mongo DB and collection is exists it will add document in existing db and collection.
        /// If DB and Collection is not exists then it will create first and add document in to it.
        /// </summary>
        /// <param name="entities">List of entities</param>
        void Create(IEnumerable<T> entities);

        /// <summary>
        /// Delete documents matching the Query
        /// </summary>
        /// <param name="query">Query to delete documents</param>
        /// <returns>returns status after delete</returns>
        bool DeleteByQuery(IMongoQuery query, bool ignoreEmbeddedVersionIdCheck = false);

        /// <summary>
        /// Returns sorted paged list of mongo entities as per filter passed
        /// </summary>
        /// <param name="query">mongo Query</param>
        /// <param name="orderBy">Mongo Orderby Clause</param>
        /// <param name="pageIndex">pageIndex (number of records to skip)</param>
        /// <param name="pageSize">page size</param>
        /// <param name="totalCount">out total records</param>
        /// <returns>List of entities</returns>
        List<T> GetPagedList(IMongoQuery query, IMongoSortBy orderBy, int pageIndex, int pageSize, out int totalCount, bool ignoreEmbeddedVersionIdCheck = false);
    }
}
