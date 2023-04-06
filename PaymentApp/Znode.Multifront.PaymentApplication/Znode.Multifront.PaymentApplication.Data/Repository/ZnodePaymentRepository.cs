using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Znode.Multifront.PaymentApplication.Helpers;

namespace Znode.Multifront.PaymentApplication.Data
{
    public class ZnodePaymentRepository<T> : IZnodePaymentRepository<T> where T : class
    {
        #region Declarations

        private IDBContext _context;

        public virtual bool IsUserWantToDebugSql { get; set; } = true;

        #endregion

        #region Constructor

        public ZnodePaymentRepository()
        {
            _context = znode_multifront_paymentEntities.Current;

            if (IsUserWantToDebugSql)
                _context.GetDatabase().Log = s => Debug.WriteLine("Component: {0} Message: {1} ", "Entity Repository", s);
        }

        #endregion

        #region Properties

        //Enable or disable Lazy Loading as per the value passed
        public bool EnableDisableLazyLoading { set { _context.GetConfiguration().LazyLoadingEnabled = value; } }

        //Returns an IQueryable instance for access to entities of the given type in the context
        public virtual IQueryable<T> Table
        {
            get
            {
                return Entities.AsNoTracking();
            }
        }

        //Returns a DbSet instance for access to entities of the given type in the context
        protected virtual DbSet<T> Entities
        {
            get
            {
                return !Equals(_context, null) ? _context.Set<T>() : null;
            }
        }

        #endregion

        #region Public Methods

        #region GetById Entity

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <typeparam name="TId">is a generic struct (eg: int, guid etc.) input</typeparam>
        /// <param name="id">is a primary key value to return entity.</param>
        /// <returns>return entity</returns>
        public virtual T GetById<TId>(TId id) where TId : struct => Entities.FindExtension(id);

        /// <summary>
        /// Get entity by identifier Asynchronously
        /// </summary>
        /// <typeparam name="TId">is a generic struct (eg: int, guid etc.) input</typeparam>
        /// <param name="id">is a primary key value to return entity.</param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync<TId>(TId id) where TId : struct => await Entities.FindExtensionAsync(id);

        /// <summary>
        /// Get entity by identifier Asynchronously
        /// </summary>
        /// <param name="id">Is a Primary key value.</param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        #endregion

        #region Insert Entity
        // Insert entity
        public virtual T Insert(T entity)
        {
            try
            {
                if (Equals(entity, null))
                    throw new ArgumentNullException(nameof(entity));

                Entities.Add(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //Remove added entity from objects for all the entities tracked by context.
                Entities.Remove(entity);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
            return entity;
        }

        /// <summary>
        /// Insert entity Asynchronously
        /// </summary>
        /// <param name="entity">Is a entity data model entity</param>
        /// <returns></returns>
        public virtual async Task<T> InsertAsync(T entity)
        {
            try
            {
                if (Equals(entity, null))
                    throw new ArgumentNullException(nameof(entity));

                Entities.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //Remove added entity from objects for all the entities tracked by context.
                Entities.Remove(entity);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw;
            }
            return entity;
        }

        // Insert entities       
        public virtual IEnumerable<T> Insert(IEnumerable<T> entities)
        {
            try
            {
                if (Equals(entities, null))
                    throw new ArgumentNullException(nameof(entities));

                _context.Set<T>().AddRange(entities);
                _context.SaveChanges();
                return entities;
            }
            catch (Exception ex)
            {
                //Remove added entities from objects for all the entities tracked by context.
                Entities.RemoveRange(entities);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        /// <summary>
        /// Insert entities Asynchronously
        /// </summary>
        /// <param name="entities">Is a list of entity data model entity</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities)
        {
            try
            {
                if (Equals(entities, null))
                    throw new ArgumentNullException(nameof(entities));
                _context.Set<T>().AddRange(entities);
                await _context.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                //Remove added entities from objects for all the entities tracked by context.
                Entities.RemoveRange(entities);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw;
            }
        }

        #endregion

        #region Update Entity

        /// <summary>
        /// Update entity 
        /// </summary>
        /// <param name="entity">Is a entity data model entity</param>
        /// <returns>Returns true or false</returns>
        public virtual bool Update(T entity)
        {
            try
            {
                if (Equals(entity, null))
                    throw new ArgumentNullException(nameof(entity));

                //Map the Modified property values to the original database entity. To update only those modified values.
                UpdateEntity(entity);
                return _context.SaveChanges() > 0;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                throw new Exception(SetEntityExceptionMessage(e));
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
        }
        #endregion

        #region Delete Entity
        // Delete entity
        public virtual bool Delete(T entity)
        {
            try
            {
                if (Equals(entity, null))
                    throw new ArgumentNullException(nameof(entity));

                Entities.Remove(Entities.Find(GetObjectKey(entity)));
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        /// <summary>
        /// Delete entity Asynchronously
        /// </summary>
        /// <param name="entity">Is a entity data model entity</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                if (Equals(entity, null))
                    throw new ArgumentNullException(nameof(entity));

                Entities.Remove(await _context.Set<T>().FindAsync(GetObjectKey(entity)));
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // Delete entities
        public virtual bool Delete(IEnumerable<T> entities)
        {
            try
            {
                if (Equals(entities, null))
                    throw new ArgumentNullException(nameof(entities));

                _context.Set<T>().RemoveRange(entities);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
        }


        /// <summary>
        /// Delete entities Asynchronously
        /// </summary>
        /// <param name="entities">Is a list of entity data model entity</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(IEnumerable<T> entities)
        {
            try
            {
                if (Equals(entities, null))
                    throw new ArgumentNullException(nameof(entities));

                _context.Set<T>().RemoveRange(entities);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // Delete entity using filter condition
        public virtual bool Delete(string where) => Delete(where, null);

        // Delete entity using filter condition
        public virtual bool Delete(string where, object[] values)
        {
            try
            {
                var dbQuery = Table;
                int count = (!Equals(values, null)) ? Table.Where(where, values).Delete() : Table.Where(where).Delete();
                return count > 0;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        #endregion

        #region Get Entity
        // Returns entity using filter condition
        public virtual T GetEntity(string where) => GetEntity(where, null, null);

        // Returns entity using filter condition
        public virtual T GetEntity(string where, object[] values) => GetEntity(where, null, values);

        // Returns entity with its specified navigation
        public virtual T GetEntity(List<string> navigationProperties) => GetEntity(string.Empty, navigationProperties, null);

        // Returns entity with its specified navigation as per filter passed
        public virtual T GetEntity(string where, List<string> navigationProperties) => GetEntity(where, navigationProperties, null);

        // Returns entity with its specified navigation as per filter passed
        public virtual T GetEntity(string where, List<string> navigationProperties, object[] values)
        {
            T item = null;
            try
            {
                IQueryable<T> dbQuery = Table;

                //Apply eager loading
                if (!Equals(navigationProperties, null))
                {
                    foreach (string navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include(navigationProperty);
                }

                if (!string.IsNullOrEmpty(where))
                {
                    dbQuery = (!Equals(values, null))
                             ? dbQuery.AsNoTracking().Where(where, values)
                             : dbQuery.AsNoTracking().Where(where);
                }

                item = dbQuery.AsNoTracking().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
            return item;
        }

        #endregion

        #region Get Entity List
        // Returns list of entities using filter condition
        public virtual IList<T> GetEntityList(string where) => GetEntityList(where, string.Empty, null, null);

        // Returns sorted list of entities using order by condition
        public virtual IList<T> GetEntityListByOrder(string orderBy) => GetEntityList(string.Empty, orderBy, null, null);

        // Returns list of entities using filter condition
        public virtual IList<T> GetEntityList(string where, object[] values) => GetEntityList(where, string.Empty, null, values);

        // Returns list of entities with its specified navigation
        public virtual IList<T> GetEntityList(List<string> navigationProperties) => GetEntityList(string.Empty, string.Empty, navigationProperties, null);

        // Returns sorted list of entities using order by condition as per filter passed
        public virtual IList<T> GetEntityList(string where, string orderBy) => GetEntityList(where, orderBy, null, null);

        // Returns list of entities with its specified navigation as per filter passed
        public virtual IList<T> GetEntityList(string where, List<string> navigationProperties) => GetEntityList(where, string.Empty, navigationProperties, null);

        // Returns sorted list of entities using order by condition as per filter passed
        public virtual IList<T> GetEntityList(string where, string orderBy, object[] values) => GetEntityList(where, orderBy, values);

        // Returns list of entities with its specified navigation as per filter passed
        public virtual IList<T> GetEntityList(string where, List<string> navigationProperties, object[] values) => GetEntityList(where, string.Empty, navigationProperties, values);

        // Returns sorted list of entities along with its specified navigation
        public virtual IList<T> GetEntityListByOrder(string orderBy, List<string> navigationProperties) => GetEntityList(string.Empty, orderBy, navigationProperties, null);

        // Returns sorted list of entities along with its specified navigation as per filter passed
        public virtual IList<T> GetEntityList(string where, string orderBy, List<string> navigationProperties, object[] values)
        {

            List<T> list = null;
            try
            {
                IQueryable<T> dbQuery = Table;

                if (string.IsNullOrEmpty(orderBy))
                {
                    orderBy = string.Format("{0} asc", GetEntityPrimaryKey<T>());
                }

                //Apply eager loading
                if (!Equals(navigationProperties, null))
                {
                    foreach (string navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include(navigationProperty);
                }
                if (!string.IsNullOrEmpty(where))
                {
                    dbQuery = (!Equals(values, null))
                        ? (string.IsNullOrEmpty(orderBy))
                        ? dbQuery.AsNoTracking().Where(where, values)
                        : dbQuery.AsNoTracking().Where(where, values).OrderBy(orderBy)
                    : (string.IsNullOrEmpty(orderBy))
                       ? dbQuery.AsNoTracking().Where(where)
                       : dbQuery.AsNoTracking().Where(where).OrderBy(orderBy);
                }
                else
                {
                    dbQuery = (string.IsNullOrEmpty(orderBy))
                        ? dbQuery.AsNoTracking()
                        : dbQuery.AsNoTracking().OrderBy(orderBy);
                }
                list = dbQuery.ToList<T>();
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
            return list;
        }

        #endregion

        #region Get Entity List with Paging
        // Returns sorted paged list of entities
        public virtual IList<T> GetPagedListByOrder(string orderBy, int pageIndex, int pageSize, out int totalCount) => GetPagedList(string.Empty, orderBy, null, null, pageIndex, pageSize, out totalCount);

        // Returns paged list of entities as per filter passed
        public virtual IList<T> GetPagedList(string where, string orderBy, int pageIndex, int pageSize, out int totalCount) => GetPagedList(where, orderBy, null, null, pageIndex, pageSize, out totalCount);

        // Returns paged list of entities as per filter passed
        public virtual IList<T> GetPagedList(string where, string orderBy, object[] values, int pageIndex, int pageSize, out int totalCount) => GetPagedList(where, orderBy, null, values, pageIndex, pageSize, out totalCount);

        // Returns paged list of entities along with its specified navigation
        public virtual IList<T> GetPagedList(string orderBy, List<string> navigationProperties, int pageIndex, int pageSize, out int totalCount) => GetPagedList(string.Empty, orderBy, navigationProperties, null, pageIndex, pageSize, out totalCount);

        // Returns sorted paged list of entities along with its specified navigation as per filter passed
        public virtual IList<T> GetPagedList(string where, string orderBy, List<string> navigationProperties, object[] values, int pageIndex, int pageSize, out int totalCount)
        {
            List<T> list = null;
            try
            {
                IQueryable<T> dbQuery = Table;

                if (string.IsNullOrEmpty(orderBy))
                {
                    orderBy = string.Format("{0} asc", GetEntityPrimaryKey<T>());
                }

                //Apply eager loading
                if (!Equals(navigationProperties, null))
                {
                    foreach (string navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include(navigationProperty);
                }

                if (!string.IsNullOrEmpty(where))
                {
                    dbQuery = (!Equals(values, null))
                        ? (string.IsNullOrEmpty(orderBy))
                        ? dbQuery.AsNoTracking().Where(where, values)
                        : dbQuery.AsNoTracking().Where(where, values).OrderBy(orderBy)
                    : (string.IsNullOrEmpty(orderBy))
                       ? dbQuery.AsNoTracking().Where(where)
                       : dbQuery.AsNoTracking().Where(where).OrderBy(orderBy);
                }
                else
                {
                    dbQuery = (!string.IsNullOrEmpty(orderBy))
                        ? dbQuery.AsNoTracking().OrderBy(orderBy)
                        : dbQuery.AsNoTracking();
                }

                list = new PagedList<T>(dbQuery, pageIndex, pageSize, out totalCount);
            }
            catch (Exception ex)
            {
                totalCount = 0;
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
            return list;
        }
        #endregion

        #endregion

        #region Private Methods

        //Set the Entity Exception message.
        private static string SetEntityExceptionMessage(System.Data.Entity.Validation.DbEntityValidationException e)
        {
            string rs = string.Empty;
            foreach (var eve in e.EntityValidationErrors)
            {
                rs = $"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:";

                foreach (var ve in eve.ValidationErrors)
                {
                    rs += "<br />" + $"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"";
                }
            }

            return rs;
        }

        //Set Modified Values for the Entity, to update only modified values based on the original database values.
        private void UpdateEntity(T entity)
        {
            //Get the Primary Key value for the Entity.
            var objKey = GetObjectKey(entity);

            //Get the Entity Data based on the primary key. If given entity already exists in the context, then data return from the context without making the db call.
            var originalEntity = Entities.Find(objKey);
            var originalEntityEntry = _context.Entry<T>(originalEntity);

            //Get the Database values for the Entity. In case the Entity return data from context cache, then updated values will not found in the entity.
            var databaseEntity = originalEntityEntry.GetDatabaseValues();

            foreach (var property in originalEntityEntry.OriginalValues.PropertyNames)
            {
                var original = databaseEntity.GetValue<object>(property);
                var current = entity.GetProperty(property);
                if (!Equals(original, current))
                {
                    originalEntityEntry.Property(property).CurrentValue = current;
                    originalEntityEntry.Property(property).IsModified = true;
                }
            }
        }

        //Get the Entity Name i.e. Table Name
        private string GetEntitySetName<TEntity>() where TEntity : class
        => GetEntitySet<TEntity>().Name;

        //Get the Primary Key Name.
        private string GetEntityPrimaryKey<TEntity>() where TEntity : class
        => GetEntitySet<TEntity>().ElementType.KeyMembers.Select(x => x.Name).FirstOrDefault();

        // Gets the Entity Set
        private EntitySet GetEntitySet<TEntity>() where TEntity : class
        {
            EntitySet entitySet = null;

            try
            {
                entitySet = (_context as IObjectContextAdapter).ObjectContext.CreateObjectSet<TEntity>().EntitySet;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
            return entitySet;
        }

        //Get the Primary Key Values.
        private object GetObjectKey(T entity)
        => typeof(T).GetProperty(GetEntityPrimaryKey<T>()).GetValue(entity, null);

        #endregion
    }
}
