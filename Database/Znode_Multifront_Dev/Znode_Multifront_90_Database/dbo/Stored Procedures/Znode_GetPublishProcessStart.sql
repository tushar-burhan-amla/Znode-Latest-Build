CREATE PROCEDURE [dbo].[Znode_GetPublishProcessStart]
(
   @PublishcatalogId INT 
   ,@VersionId INT 
   ,@UserId INT 
   ,@IsDebug INT = 0
   ,@LocaleId TransferId READONLY  ,
   @PublishStateId INT =0  
)
AS 
/*
  Summary : - This Procedure is used to get the  async process 
  Unit Testing
  BEGIN TRAN 
  DECLARE @R TRANSFERId 
  INSERT INTO @R
  VALUES(1),(24)
  EXEC Znode_GetPublishProcessStart 7,0,2,1,@R ,4
  ROLLBACK TRAN
 */
BEGIN 
   BEGIN TRY  
      DECLARE @ImportHeadName NVARCHAR(100), @SPScript NVARCHAR(MAX), @DatabaseName NVARCHAR(100), @ServerName NVARCHAR(100), @ImportProcessLogId INT= 0,@sql_job NVARCHAR(max),@UserName nvarchar(100);
	     SET @DatabaseName = DB_NAME();
         SET @ServerName = @@serverName;
		 SET @UserName = SYSTEM_USER;
		 DECLARE @DataString NVARCHAR(Max) 

		 SET @DataString = (SELECT ''+' INSERT INTO @localeId Values ('''+CAST(Id AS VARCHAR(max))+ ''')'
		FROM  @LocaleId FOR XML PATH (''))

		  DECLARE @Tockem uniqueidentifier = NEWID()
   	  DECLARE @SQL NVARCHAR(max) = 'DECLARE @localeId TransferId  '+@DataString+'
	     EXEC Znode_GetPublishProducts @PublishcatalogId='+CAST(@PublishcatalogId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@VersionId='+CAST(@VersionId AS VARCHAR(50))+',@PublishStateId='+CAST(@PublishStateId AS VARCHAR(50))+', @TokenId='''+CAST(@Tockem AS NVARCHAR(max))+''', @LocaleId=@LocaleId'
		
	  --IF @IsDebug = 1
   --      BEGIN
   --         EXEC sys.sp_sqlexec  @SPScript;               
   --         RETURN 0;
   --      END;
     PRINT @SQL

       SET @sql_job = '
         DECLARE @jobId BINARY(16);
         -- SET @NewGUID = REPLACE(@Tockem, '''', '');
         EXEC msdb.dbo.sp_add_job 
              @job_name = @Tockem,@enabled = 1,@notify_level_eventlog = 0,@notify_level_email = 2,@notify_level_netsend = 2,@notify_level_page = 2,@delete_level = 3,
              @category_name = N''[Uncategorized (Local)]'',@owner_login_name = N'''+@UserName+''',@job_id = @jobId OUTPUT;

         SELECT @jobId;

         EXEC msdb.dbo.sp_add_jobserver
              @job_name = @Tockem;

         EXEC msdb.dbo.sp_add_jobstep
              @job_name = @Tockem,@step_name = N''Publish'',@step_id = 1,@cmdexec_success_code = 0,@on_success_action = 1,@on_fail_action = 2,@retry_attempts = 3,
              @retry_interval = 0,@os_run_priority = 0,@subsystem = N''TSQL'',@command = @SQL,@database_name = @DatabaseName,@flags = 0;
		
		 --IF @ReturnCode= 0 (success) or 1 (failure)
         DECLARE @ReturnCode TINYINT; 
         EXEC @ReturnCode = msdb.dbo.sp_start_job
              @job_name = @Tockem,@step_name = N''Publish'';
   		 SELECT @Tockem	 '

		 PRINT @SQL 

		 IF @@VERSION LIKE '%Azure%' OR @@VERSION LIKE '%Express Edition%' 
		 BEGIN 
		      EXEC sys.sp_executesql  @SQL;    

		 END 
		 ELSE 
		 BEGIN 
		 
		EXEC sys.sp_executesql  @sql_job,N' @SQL NVARCHAR(max) ,@DatabaseName NVARCHAR(100), @ServerName NVARCHAR(100),@Tockem uniqueidentifier,@LocaleId TransferId READONLY',@SQL  =@SQL,@DatabaseName=@DatabaseName,@ServerName=@ServerName,@Tockem =@Tockem,@LocaleId= @LocaleId   ;  
		    
		 END 



	END TRY 
	BEGIN CATCH
	    DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishProcessStart @PublishcatalogId='+CAST(@PublishcatalogId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@VersionId='+CAST(@VersionId AS VARCHAR(50))+', @TokenId='''+CAST(@Tockem AS NVARCHAR(max))+',@Status='+CAST(@Status AS VARCHAR(10));
              	SELECT ERROR_MESSAGE()		 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetPublishProcessStart',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH 
END