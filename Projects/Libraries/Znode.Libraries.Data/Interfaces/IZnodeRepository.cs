using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Znode.Libraries.Data
{
    /// <summary>
    /// Znode Generic Repository
    /// </summary>
    public interface IZnodeRepository<T>
    {
        bool EnableDisableLazyLoading { set; }

        /// <summary>
        ///Returns an IQueryable instance for access to entities of the given type in the context
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Get Entity by its Primary Key.
        /// </summary>
        /// <typeparam name="TId">DataType for Primary Key</typeparam>
        /// <param name="id">Primary Key value</param>
        /// <returns>returns T</returns>
        T GetById<TId>(TId id) where TId : struct;

        /// <summary>
        /// Get Entity by its Primary Key Asynchronously.
        /// </summary>
        /// <typeparam name="TId">DataType for Primary Key</typeparam>
        /// <param name="id">Primary Key value</param>
        /// <returns>returns Task<T></returns>
        Task<T> GetByIdAsync<TId>(TId id) where TId : struct;

        /// <summary>
        /// Get Entity by its Primary Key Asynchronously.
        /// </summary>
        /// <param name="id">Primary Key value</param>
        /// <returns>returns Task<T></returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Insert entity Asynchronously.
        /// </summary>
        /// <param name="entity">Entity</param>
        T Insert(T entity);

        /// <summary>
        /// Insert entity 
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<T> InsertAsync(T entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        IEnumerable<T> Insert(IEnumerable<T> entities);

        /// <summary>
        /// Insert entities Asynchronously.
        /// </summary>
        /// <param name="entities">Entities</param>
        Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        bool Update(T entity);

        /// <summary>
        /// Update multiple entities in one batch.
        /// </summary>
        /// <param name="entities">Entities to update</param>
        bool BatchUpdate(IEnumerable<T> entities);

        /// <summary>
        /// Update entity Asynchronously.
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        bool Delete(T entity);

        /// <summary>
        /// Delete entity Asynchronously.
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<bool> DeleteAsync(T entity);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        bool Delete(IEnumerable<T> entities);

        /// <summary>
        /// Delete entities Asynchronously.
        /// </summary>
        /// <param name="entities">Entities</param>
        Task<bool> DeleteAsync(IEnumerable<T> entities);

        /// <summary>
        /// Delete entities based on Where Clause
        /// Use Where clause having multiple Ids
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns>return bool</returns>
        bool Delete(string whereClause);

        /// <summary>
        /// Delete entities based on Where Clause
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="values">Filter values to pass.</param>
        /// <returns>return bool</returns>
        bool Delete(string whereClause, object[] values);

        /// <summary>
        /// Returns entity using filter condition
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <returns>Entity<T></returns>
        T GetEntity(string where);

        /// <summary>
        /// Returns entity using filter condition
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="values">Filter values to pass.</param>
        /// <returns>Entity<T></returns>
        T GetEntity(string where, object[] values);

        /// <summary>
        /// Returns entity with its specified navigation
        /// </summary>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// <returns>Entity<T></returns>
        T GetEntity(List<string> navigationProperties);

        /// <summary>
        /// Returns entity with its specified navigation as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// <returns>Entity<T></returns>
        T GetEntity(string where, List<string> navigationProperties);

        /// <summary>
        /// Returns entity with its specified navigation as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// /// <param name="values">Filter values to pass.</param>
        /// <returns>Entity<T></returns>
        T GetEntity(string where, List<string> navigationProperties, object[] values);

        /// <summary>
        /// Returns list of entities using filter condition
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityList(string where);

        /// <summary>
        /// Returns sorted list of entities using order by condition
        /// </summary>
        /// <param name="orderBy">Order by condition to sort.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityListByOrder(string orderBy);

        /// <summary>
        /// Returns list of entities using filter condition
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="values">Filter values to pass.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityList(string where, object[] values);

        /// <summary>
        /// Returns list of entities with its specified navigation
        /// </summary>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityList(List<string> navigationProperties);

        /// <summary>
        /// Returns sorted list of entities using order by condition as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="orderBy">Order by condition to sort.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityList(string where, string orderBy);

        /// <summary>
        /// Returns list of entities with its specified navigation as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityList(string where, List<string> navigationProperties);

        /// <summary>
        /// Returns sorted list of entities using order by condition as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="orderBy">Order by condition to sort.</param>
        /// <param name="values">Filter values to pass.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityList(string where, string orderBy, object[] values);

        /// <summary>
        /// Returns list of entities with its specified navigation as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// <param name="values">Filter values to pass.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityList(string where, List<string> navigationProperties, object[] values);

        /// <summary>
        /// Returns sorted list of entities along with its specified navigation
        /// </summary>
        /// <param name="orderBy">Order by condition to sort.</param>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityListByOrder(string orderBy, List<string> navigationProperties);

        /// <summary>
        /// Returns sorted list of entities along with its specified navigation as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="orderBy">Order by condition to sort.</param>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// <param name="values">Filter values to pass.</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetEntityList(string where, string orderBy, List<string> navigationProperties, object[] values);

        /// <summary>
        /// Returns sorted paged list of entities
        /// </summary>
        /// <param name="orderBy">Order by condition to sort</param>
        /// <param name="pageIndex">Paging starts from.</param>
        /// <param name="pageSize">No of records to be displayed in single page.</param>
        /// <param name="totalCount">No of records returned in the result (out).</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetPagedListByOrder(string orderBy, int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// Returns paged list of entities as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="pageIndex">Paging starts from.</param>
        /// <param name="pageSize">No of records to be displayed in single page.</param>
        /// <param name="totalCount">No of records returned in the result (out).</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetPagedList(string where, string orderBy, int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// Returns paged list of entities as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="values">Filter values to pass.</param>
        /// <param name="pageIndex">Paging starts from.</param>
        /// <param name="pageSize">No of records to be displayed in single page.</param>
        /// <param name="totalCount">No of records returned in the result (out).</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetPagedList(string where, string orderBy, object[] values, int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// Returns paged list of entities along with its specified navigation
        /// </summary>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// <param name="pageIndex">Paging starts from.</param>
        /// <param name="pageSize">No of records to be displayed in single page.</param>
        /// <param name="totalCount">No of records returned in the result (out).</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetPagedList(string orderBy, List<string> navigationProperties, int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// Returns sorted paged list of entities along with its specified navigation as per filter passed
        /// </summary>
        /// <param name="where">Filter condition to pass.</param>
        /// <param name="orderBy">Order by condition to sort.</param>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result.</param>
        /// <param name="values">Filter values to pass.</param>
        /// <param name="pageIndex">Paging starts from.</param>
        /// <param name="pageSize">No of records to be displayed in single page.</param>
        /// <param name="totalCount">No of records returned in the result (out).</param>
        /// <returns>IListEntity<T></returns>
        IList<T> GetPagedList(string where, string orderBy, List<string> navigationProperties, object[] values, int pageIndex, int pageSize, out int totalCount);

        /// <summary>
        /// Get unordered list of entity along with its specified navigation as per filter passed & will not have default order by clause in the query execution.
        /// </summary>
        /// <param name="where">Filter condition to pass</param>
        /// <param name="navigationProperties">Comma-separated list of related objects to return in the result</param>
        /// <param name="values">Filter values to pass</param>
        /// <returns>IListEntity<T>Return unordered list</returns>
        IList<T> GetEntityListWithoutOrderBy(string where, List<string> navigationProperties, object[] values);

        /// <summary>
        /// Get unordered list of entity using filter condition & will not have default order by clause in the query execution.
        /// </summary>
        /// <param name="where">Filter condition to pass</param>
        /// <returns>Return unordered list</returns>
        IList<T> GetEntityListWithoutOrderBy(string where);

        /// <summary>
        /// Get unordered list of entity using filter condition & will not have default order by clause in the query execution.
        /// </summary>
        /// <param name="where">Filter condition to pass</param>
        /// <param name="values">Filter values to pass</param>
        /// <returns>Return unordered list</returns>
        IList<T> GetEntityListWithoutOrderBy(string where, object[] values);

        /// <summary>
        /// Get unordered list of entity and specified navigation entities & will not have default order by clause in the query execution.
        /// </summary>
        /// <param name="where">Filter condition to pass</param>
        /// <param name="navigationProperties">Filter values to pass</param>
        /// <returns>Return unordered list</returns>
        IList<T> GetEntityListWithoutOrderBy(string where, List<string> navigationProperties);
    }
}
