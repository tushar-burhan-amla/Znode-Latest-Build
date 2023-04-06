
CREATE PROCEDURE [dbo].[AspNet_SqlCacheQueryRegisteredTablesStoredProcedure]
AS
     BEGIN
         SELECT tableName
         FROM dbo.AspNet_SqlCacheTablesForChangeNotification;
     END;

