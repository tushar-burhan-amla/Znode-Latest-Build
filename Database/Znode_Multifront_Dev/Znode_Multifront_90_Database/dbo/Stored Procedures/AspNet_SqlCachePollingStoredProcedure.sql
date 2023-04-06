
CREATE PROCEDURE [dbo].[AspNet_SqlCachePollingStoredProcedure]
AS
     BEGIN
         SELECT tableName , changeId
         FROM dbo.AspNet_SqlCacheTablesForChangeNotification;
         RETURN 0;
     END;

