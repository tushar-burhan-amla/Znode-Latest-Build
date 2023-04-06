using MongoDB.Driver;
using System;

namespace Znode.Libraries.MongoDB.Data
{
    public class MongoContextHelper
    {

        #region Public Methods
        //Get MongoDatabase Context.
        public static MongoDatabase GetMongoContext(string connectionString)
        {
            MongoClient mongoClient = MongoDBInstance.GetMongoClient(connectionString);
            MongoServer mongoServer = mongoClient.GetServer();

            // Get a reference to the "znode" database object 
            // from the Mongo server object
            string databaseName = MongoUrl.Create(connectionString).DatabaseName;
            return mongoServer.GetDatabase(databaseName);
        }
        #endregion

        public static class MongoDBInstance
        {
            //volatile: ensure that assignment to the instance variable
            //is completed before the instance variable can be accessed
            private static volatile MongoClient mongoClient = null;
            private static readonly object syncLock = new Object();

            public static MongoClient GetMongoClient(string connectionString)
            {
                if (Equals(mongoClient, null))
                {
                    lock (syncLock)
                    {
                        if (Equals(mongoClient, null))
                            mongoClient = new MongoClient(connectionString);
                    }
                }
                return mongoClient;
            }
        }

    }
}
