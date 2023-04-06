using MongoDB.Driver;
using System;
using System.Configuration;
using System.Web;
using ZNode.Libraries.MongoDB.Data.Constants;
namespace Znode.Libraries.MongoDB.Data
{
    public class MongoConnectionHandler<T> : IDisposable
    {
        public MongoCollection<T> MongoCollection { get; private set; }

        private bool isDisposed;

        public MongoConnectionHandler(bool IsLog)
        {
            IsLog = true;
            //Get mongo Connection string.
            string connectionString = ConfigurationManager.ConnectionStrings[MongoSettings.DBLogKey]?.ConnectionString;

            MongoConnectionString(connectionString, IsLog);
        }

        ~MongoConnectionHandler()
        {
            if (!isDisposed)
                Dispose();
        }

        public void Dispose()
            => isDisposed = true;

        private void MongoConnectionString(string connectionString, bool IsLogDB = false)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                MongoDatabase db = null;
                if (HttpContext.Current != null)
                {
                    string objectContextKey = IsLogDB ? "mongologdb_" : "mongo_" + HttpContext.Current.GetHashCode().ToString("x");
                    if (!HttpContext.Current.Items.Contains(objectContextKey))
                    {
                        db = MongoContextHelper.GetMongoContext(connectionString);
                        HttpContext.Current.Items.Add(objectContextKey, db);
                    }
                    else
                        db = HttpContext.Current.Items[objectContextKey] as MongoDatabase;

                }
                else
                    db = MongoContextHelper.GetMongoContext(connectionString);

                // Get a reference to the collection object from the Mongo database object
                MongoCollection = db.GetCollection<T>(typeof(T).Name.ToLower());

            }
        }

    }

}
