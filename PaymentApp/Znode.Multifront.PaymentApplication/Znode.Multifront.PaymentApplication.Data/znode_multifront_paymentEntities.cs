using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Znode.Multifront.PaymentApplication.Helpers;

namespace Znode.Multifront.PaymentApplication.Data
{
    public partial class znode_multifront_paymentEntities : IDBContext
    {
        #region Variables
        private string CreatedDate = "CreatedDate";
        private string ModifiedDate = "ModifiedDate";
        #endregion

        #region Properties

        //Gets current context object
        public static znode_multifront_paymentEntities Current { get { return GetObjectContext(); } }

        #endregion

        #region Public Methods

        //Returns current context object
        public static znode_multifront_paymentEntities GetObjectContext()
        {
            string objectContextKey = "ocm_" + HttpContext.Current.GetHashCode().ToString("x");
            if (!HttpContext.Current.Items.Contains(objectContextKey))
                HttpContext.Current.Items.Add(objectContextKey, new znode_multifront_paymentEntities());
            return HttpContext.Current.Items[objectContextKey] as znode_multifront_paymentEntities;
        }


        //Returns a System.Data.Entity.DbSet`1 instance for access to entities of the given
        //     type in the context and the underlying store.
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();

        //Gets a System.Data.Entity.Infrastructure.DbEntityEntry`1 object for the given
        //     entity providing access to information about the entity and the ability to perform
        //     actions on the entity.
        public new DbEntityEntry<T> Entry<T>(T entity) where T : class => base.Entry(entity);

        //Provides access to configuration options for the context.
        public DbContextConfiguration GetConfiguration() => base.Configuration;

        //Provides access to the Database configuration of  the Context.
        public Database GetDatabase() => base.Database;

        public new int SaveChanges() => SaveChanges(DateTime.UtcNow);

        //Override Method to Insert/Update the Created/Modified Date for the Entity.
        public int SaveChanges(DateTime datetime)
        {
            try
            {
                foreach (var ent in this.ChangeTracker.Entries().Where(p => Equals(p.State, EntityState.Added) || Equals(p.State, EntityState.Deleted) || Equals(p.State, EntityState.Modified)))
                {
                    SetDataIntoEntity(ent, datetime);
                }
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                IEnumerable<string> errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                string fullErrorMessage = string.Join("; ", errorMessages);
                string exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                ReplaceCorruptEntity();
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception ex)
            {
                ReplaceCorruptEntity();
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
        }

        public new Task<int> SaveChangesAsync() => SaveChangesAsync(DateTime.UtcNow);

        public Task<int> SaveChangesAsync(DateTime dateTime)
        {
            try
            {
                foreach (var ent in this.ChangeTracker.Entries().Where(p => Equals(p.State, EntityState.Added) || Equals(p.State, EntityState.Deleted) || Equals(p.State, EntityState.Modified)))
                {
                    SetDataIntoEntity(ent, dateTime);
                }
                return base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                IEnumerable<string> errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                string fullErrorMessage = string.Join("; ", errorMessages);

                string exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                ReplaceCorruptEntity();
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception ex)
            {
                ReplaceCorruptEntity();
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw ex;
            }
        }
        #endregion

        #region Private Method
        //Set the Created/Modified Date for the existing Entity.
        private void SetDataIntoEntity(DbEntityEntry dbEntry, DateTime datetime)
        {
            if (Equals(dbEntry.State, EntityState.Added))
            {

                dbEntry.Entity.SetPropertyValue(CreatedDate, datetime);
                dbEntry.Entity.SetPropertyValue(ModifiedDate, datetime);
            }
            else if (Equals(dbEntry.State, EntityState.Modified))
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually changed
                    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    {
                        var OriginalValue = Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), null) ? null : dbEntry.OriginalValues.GetValue<object>(propertyName);
                        if (Equals(propertyName, CreatedDate))
                        {
                            dbEntry.Entity.SetPropertyValue(CreatedDate, OriginalValue);
                        }
                    }
                }

                dbEntry.Entity.SetPropertyValue(ModifiedDate, datetime);
            }
        }

        //Set the actual values for Corrupted entities in edmx module.
        private void ReplaceCorruptEntity()
        {
            //Get list of entities that are marked as modified
            List<DbEntityEntry> modifiedEntityList =
                this.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList();

            //Replace corrupt entities with the correct existing values, causing the EDMX module error.
            foreach (DbEntityEntry entity in modifiedEntityList)
            {
                DbPropertyValues propertyValues = entity.OriginalValues;
                foreach (String propertyName in propertyValues.PropertyNames)
                {
                    //Replace current values with original values
                    PropertyInfo property = entity.Entity.GetType().GetProperty(propertyName);
                    property.SetValue(entity.Entity, propertyValues[propertyName]);
                }
            }
        }
        #endregion
    }
}
