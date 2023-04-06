using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Znode.Libraries.Data.Helpers;

namespace Znode.Libraries.Data.DataModel
{
    public partial class ZnodeKlaviyoEntities: IDBContext
    {
        #region Variables
        private string CreatedDate = "CreatedDate";
        private string CreatedBy = "CreatedBy";
        private string ModifiedDate = "ModifiedDate";
        private string ModifiedBy = "ModifiedBy";
        #endregion      


        #region Public Methods      

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

        public new int SaveChanges() => SaveChanges(0);

        //Override Method to Insert/Update the Created/Modified Date for the Entity.
        public int SaveChanges(int loginUserAccountId, int createdBy = 0, int modifiedBy = 0)
        {
            try
            {
                foreach (var ent in this.ChangeTracker.Entries().Where(p => Equals(p.State, EntityState.Added) || Equals(p.State, EntityState.Deleted) || Equals(p.State, EntityState.Modified)))
                {
                    SetDataIntoEntity(ent, loginUserAccountId, createdBy, modifiedBy);
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
                EntityLogging.LogMessage(exceptionMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception ex)
            {
                ReplaceCorruptEntity();
                EntityLogging.LogMessage(ex.Message);
                throw;
            }
        }

        public new Task<int> SaveChangesAsync() => SaveChangesAsync(0);

        public Task<int> SaveChangesAsync(int loginUserAccountId)
        {
            try
            {
                foreach (var ent in this.ChangeTracker.Entries().Where(p => Equals(p.State, EntityState.Added) || Equals(p.State, EntityState.Deleted) || Equals(p.State, EntityState.Modified)))
                {
                    SetDataIntoEntity(ent, loginUserAccountId);
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
                EntityLogging.LogMessage(exceptionMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception ex)
            {
                ReplaceCorruptEntity();
                EntityLogging.LogMessage(ex.Message);
                throw;
            }
        }


        #endregion

        #region Private Method
        //Set the Created/Modified Date for the existing Entity.
        public void SetDataIntoEntity(DbEntityEntry dbEntry, int loginUserAccountId, int createdBy = 0, int modifiedBy = 0)
        {
            if (Equals(dbEntry.State, EntityState.Added))
            {
                dbEntry.Entity.SetPropertyValue(CreatedBy, (createdBy > 0) ? createdBy : loginUserAccountId);
                dbEntry.Entity.SetPropertyValue(ModifiedBy, (modifiedBy > 0) ? modifiedBy : loginUserAccountId);

                dbEntry.Entity.SetPropertyValue(CreatedDate, HelperMethods.GetEntityDateTime());
                dbEntry.Entity.SetPropertyValue(ModifiedDate, HelperMethods.GetEntityDateTime());
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
                        if (loginUserAccountId > 0 && Equals(propertyName, CreatedBy))
                        {
                            dbEntry.Entity.SetPropertyValue(CreatedBy, OriginalValue);
                        }
                    }
                }
                dbEntry.Entity.SetPropertyValue(ModifiedBy, modifiedBy > 0 ? modifiedBy : loginUserAccountId);
                dbEntry.Entity.SetPropertyValue(ModifiedDate, HelperMethods.GetEntityDateTime());
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
