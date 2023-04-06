CREATE PROCEDURE  [dbo].[Znode_UpdateShowOnGridColumn]   
(  
   @PimAttributeId INT = 0   
 , @UserId int = 0   
 , @IsShowOngrid bit = 0   
 , @Status bit Out   
 ,@IsCreateJob bit = 0 
)  
AS   
BEGIN   
SET NOCOUNT ON   
 BEGIN TRY   
    
  
   DECLARE @ImportHeadName NVARCHAR(100), @SPScript NVARCHAR(MAX),@Tockem nvarchar(2000), @DatabaseName NVARCHAR(100), @ServerName NVARCHAR(100), @ImportProcessLogId INT= 0,@sql_job NVARCHAR(max),@UserName nvarchar(100);  
      SET @DatabaseName = DB_NAME();  
         SET @ServerName = @@serverName;  
   SET @UserName = SYSTEM_USER;  
   SET @Tockem = NEWID()  
  
  DECLARE @sql NVARCHAr(max) = '', @AttributeCode VARCHAR(2000) = (SELECT TOP 1 AttributeCode FROM ZnodePimAttribute WHERE PimAttributeId = @PimAttributeId )  
  IF @IsShowOngrid = 1  AND NOT EXISTS (SELECT TOP 1 1 FROM  INFORMATION_SCHEMA.COLUMNS a WHERE a.TABLE_NAME = 'ZnodePimProduct'   
  AND a.COLUMN_NAME = @AttributeCode  
   )  
  BEGIN   
  SET @sql = ' ALTER TABLE ZnodePimProduct ADD '+@AttributeCode + ' NVARCHAR(max)'  
  END   
  --ELSE  IF @IsShowOngrid = 0  
  --BEGIN   
  --  SET @sql = ' ALTER TABLE ZnodePimProduct DROP COLUMN  '+@AttributeCode   
  --END   
  
  EXEC (@sql)  
  
  SET @sql = ' EXEC Znode_UpdateShowOnGridColumInternal '+CAST(@PimAttributeId AS varchar (2000) )  
  
   SELECT @PimAttributeId AS ID, CAST(1 AS BIT)  AS [Status];   
  
  IF @IsCreateJob = 1 
  BEGIN 

   SET @sql_job = '  
         DECLARE @jobId BINARY(16);  
         -- SET @NewGUID = REPLACE(@Tockem, '''', '');  
         EXEC msdb.dbo.sp_add_job   
              @job_name = @Tockem,@enabled = 1,@notify_level_eventlog = 0,@notify_level_email = 2,@notify_level_netsend = 2,@notify_level_page = 2,@delete_level = 3,  
              @category_name = N''[Uncategorized (Local)]'',@owner_login_name = N'''+@UserName+''',@job_id = @jobId OUTPUT;  
  
         SELECT @jobId;  
  
         EXEC msdb.dbo.sp_add_jobserver  
              @job_name = @Tockem,@server_name = @ServerName;  
  
         EXEC msdb.dbo.sp_add_jobstep  
              @job_name = @Tockem,@step_name = N''ShowOngridUpdate'',@step_id = 1,@cmdexec_success_code = 0,@on_success_action = 1,@on_fail_action = 2,@retry_attempts = 3,  
              @retry_interval = 0,@os_run_priority = 0,@subsystem = N''TSQL'',@command = @SQL,@database_name = @DatabaseName,@flags = 0;  
    
   --IF @ReturnCode= 0 (success) or 1 (failure)  
         DECLARE @ReturnCode TINYINT;   
         EXEC @ReturnCode = msdb.dbo.sp_start_job  
              @job_name = @Tockem,@server_name = @ServerName,@step_name = N''ShowOngridUpdate'';  
      SELECT @Tockem  '  
  
    
  
   IF @@VERSION LIKE '%Azure%' OR @@VERSION LIKE '%Express Edition%'  AND  @IsShowOngrid = 1   
   BEGIN   
        EXEC sys.sp_executesql  @SQL;      
  
   END   
   ELSE IF  @IsShowOngrid = 1   
   BEGIN   
     
  EXEC sys.sp_executesql  @sql_job,N' @SQL NVARCHAR(max) ,@DatabaseName NVARCHAR(100), @ServerName NVARCHAR(100),@Tockem uniqueidentifier,@LocaleId TransferId READONLY',@SQL  =@SQL,@DatabaseName=@DatabaseName,@ServerName=@ServerName,@Tockem =@Tockem   ;  
  
        
   END  
   END 
   ELSE 
   BEGIN 

   EXEC (@sql) 

   END 

    
   SET @Status = 1;  
 END TRY   
 BEGIN CATCH   
 SET @Status = 0;  
 SELECT ERROR_MESSAGE()  
 END CATCH    
END