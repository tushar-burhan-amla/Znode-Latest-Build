CREATE PROCEDURE [dbo].[AspNet_SqlCacheRegisterTableStoredProcedure]
       @tableName NVARCHAR(450)
AS
     BEGIN
         DECLARE @triggerName AS NVARCHAR(3000);
         DECLARE @fullTriggerName AS NVARCHAR(3000);
         DECLARE @canonTableName NVARCHAR(3000);
         DECLARE @quotedTableName NVARCHAR(3000);
		 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

         /* Create the trigger name */

         SET @triggerName = REPLACE(@tableName , '[' , '__o__');
         SET @triggerName = REPLACE(@triggerName , ']' , '__c__');
         SET @triggerName = @triggerName+'_AspNet_SqlCacheNotification_Trigger';
         SET @fullTriggerName = 'dbo.['+@triggerName+']'; 

         /* Create the cannonicalized table name for trigger creation */

         /* Do not touch it if the name contains other delimiters */

         IF ( CHARINDEX('.' , @tableName) <> 0
              OR
              CHARINDEX('[' , @tableName) <> 0
              OR
              CHARINDEX(']' , @tableName) <> 0 )
             BEGIN
                 SET @canonTableName = @tableName;
             END
         ELSE
             BEGIN
                 SET @canonTableName = '['+@tableName+']';
             END; 

         /* First make sure the table exists */

         IF ( SELECT OBJECT_ID(@tableName , 'U') ) IS NULL
             BEGIN
                 RAISERROR('00000001' , 16 , 1);
                 RETURN;
             END;
         BEGIN TRAN;
         
         /* Insert the value into the notification table */

         IF NOT EXISTS ( SELECT tableName
                         FROM dbo.AspNet_SqlCacheTablesForChangeNotification WITH (NOLOCK)
                         WHERE tableName = @tableName
                       )
             BEGIN
                 IF NOT EXISTS ( SELECT tableName
                                 FROM dbo.AspNet_SqlCacheTablesForChangeNotification WITH (TABLOCKX)
                                 WHERE tableName = @tableName
                               )
                     BEGIN
                         INSERT INTO dbo.AspNet_SqlCacheTablesForChangeNotification
                         VALUES ( @tableName , @GetDate , 0
                                )
                     END
             END;

         /* Create the trigger */

         SET @quotedTableName = QUOTENAME(@tableName , '''');
         IF NOT EXISTS ( SELECT name
                         FROM sysobjects WITH (NOLOCK)
                         WHERE name = @triggerName
                               AND
                               type = 'TR'
                       )
             BEGIN
                 IF NOT EXISTS ( SELECT name
                                 FROM sysobjects WITH (TABLOCKX)
                                 WHERE name = @triggerName
                                       AND
                                       type = 'TR'
                               )
                     BEGIN
                         EXEC ('CREATE TRIGGER '+@fullTriggerName+' ON '+@canonTableName+'
                       FOR INSERT, UPDATE, DELETE AS BEGIN
                       SET NOCOUNT ON
                       EXEC dbo.AspNet_SqlCacheUpdateChangeIdStoredProcedure N'+@quotedTableName+'
                       END
                       ')
                     END
             END;
         COMMIT TRAN;
     END;

