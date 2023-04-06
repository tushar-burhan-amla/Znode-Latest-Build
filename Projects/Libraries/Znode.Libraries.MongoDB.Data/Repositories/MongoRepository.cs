using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Exceptions;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.MongoDB.Data
{
    public class MongoRepository<T> : IMongoRepository<T>, IDisposable where T : IMongoEntity, IDisposable, new()
    {
        private readonly MongoConnectionHandler<T> MongoConnectionHandler;
        private readonly MongoEntity LogEntity;
        private bool isDisposed;
        private readonly string logComponentName = "MongoDB";
        private string ErrorMessage;


        public MongoRepository(bool IsLog)
        {
            MongoConnectionHandler = new MongoConnectionHandler<T>(IsLog);
        }


        ~MongoRepository()
        {
            if (!isDisposed)
                Dispose();
        }

        public void Dispose()
        {

            isDisposed = true;
        }

        public virtual MongoConnectionHandler<T> Table
        {
            get
            {
                return MongoConnectionHandler;
            }
        }

        public virtual void Create(T entity)
        {
            try
            {
                // Save the entity with safe mode (WriteConcern.Acknowledged)
                var result = this.MongoConnectionHandler.MongoCollection.Save(
                    entity,
                    new MongoInsertOptions
                    {
                        WriteConcern = WriteConcern.Acknowledged
                    });

                if (result.HasLastErrorMessage)
                    throw new Exception(result.LastErrorMessage);
            }
            catch (Exception ex)
            {
                ErrorMessage = LogErrorBasedOnQueryOrData(ex, "Create", null, Convert.ToString(typeof(T)), Convert.ToString(entity?.Id));
                throw new ZnodeException(ErrorCodes.MongoAuthentication, ErrorMessage);
            }
        }

        public virtual void Create(IEnumerable<T> entities)
        {
            try
            {
                // Save the entities 
                this.MongoConnectionHandler.MongoCollection.InsertBatch(entities);
            }
            catch (Exception ex)
            {

                string ids = entities != null ? String.Join(",", entities?.Select(o => o.Id)) : null;
                ErrorMessage = LogErrorBasedOnQueryOrData(ex, "Create bulk", null, Convert.ToString(typeof(T)), ids);
                throw new ZnodeException(ErrorCodes.MongoAuthentication, ErrorMessage);
            }
        }

        public virtual bool DeleteByQuery(IMongoQuery query, bool ignoreEmbeddedVersionIdCheck = false)
        {
            var result = this.MongoConnectionHandler.MongoCollection.Remove(query, WriteConcern.Acknowledged);

            if (result.HasLastErrorMessage)
                throw new Exception(result.LastErrorMessage);

            return result.DocumentsAffected > 0;
        }

        public virtual List<T> GetPagedList(IMongoQuery query, IMongoSortBy orderBy, int pageIndex, int pageSize, out int totalCount, bool ignoreEmbeddedVersionIdCheck = false)
        {
            try
            {
                long longTotalCount = this.MongoConnectionHandler.MongoCollection.Count(query);
                Int32.TryParse(Convert.ToString(longTotalCount), out totalCount);

                if (!Equals(orderBy, null))
                    return this.MongoConnectionHandler.MongoCollection.Find(query).SetSortOrder(orderBy).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList();
                else
                    return this.MongoConnectionHandler.MongoCollection.Find(query).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).ToList();
            }

            catch (Exception ex)
            {
                totalCount = 0;
                ErrorMessage = LogErrorBasedOnQueryOrData(ex, "GetPagedList", query, Convert.ToString(typeof(T)));
                throw new ZnodeException(ErrorCodes.MongoAuthentication, ErrorMessage);
            }
        }


        private string LogErrorBasedOnQueryOrData(Exception ex, string methodName, IMongoQuery query, string entityType = "", string data = "")
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(entityType))
                    ErrorMessage = $"Error occurred during {methodName} for {query} of type {entityType}. Exception details - {ex?.Message}";
                else
                    ErrorMessage = $"Error occurred during {methodName} for {query}. Exception details - {ex?.Message}";
            }
            else if (!string.IsNullOrEmpty(data))
            {
                if (!string.IsNullOrEmpty(entityType))
                    ErrorMessage = $"Error occurred during {methodName} entities of type : {entityType}. Exception details - {ex?.Message}";
                else
                    ErrorMessage = $"Error occurred during {methodName} with data : {data}. Exception details - {ex?.Message}";
            }
            else
            {
                ErrorMessage = $"Error occurred during {methodName} entities of type :{entityType}. Exception details - {ex?.Message}";
            }
            ZnodeLogging.LogMessage(ErrorMessage, logComponentName, TraceLevel.Error);
            return ErrorMessage;
        }       
    }
}
