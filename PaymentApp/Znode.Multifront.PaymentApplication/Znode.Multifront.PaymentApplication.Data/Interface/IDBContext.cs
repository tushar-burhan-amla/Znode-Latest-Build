using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace Znode.Multifront.PaymentApplication.Data
{
    public interface IDBContext
    {
        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="dateTime">Date Time</param>
        /// <returns></returns>
        int SaveChanges(DateTime dateTime);

        /// <summary>
        /// Save changes Asynchronously
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Save changes Asynchronously
        /// </summary>
        /// <param name="dateTime">Date Time</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(DateTime dateTime);

        /// <summary>
        /// Provides Informtation for the Entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        DbEntityEntry<T> Entry<T>(T entity) where T : class;

        /// <summary>
        /// Get the Configuration options for the Context.
        /// </summary>
        /// <returns></returns>
        DbContextConfiguration GetConfiguration();

        /// <summary>
        /// Get the Database Configuration OPtions for the Context.
        /// </summary>
        /// <returns></returns>
        Database GetDatabase();
    }
}
