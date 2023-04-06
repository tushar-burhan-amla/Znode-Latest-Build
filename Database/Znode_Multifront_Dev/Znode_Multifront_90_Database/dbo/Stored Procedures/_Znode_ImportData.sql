
CREATE PROCEDURE [dbo].[_Znode_ImportData]
    (
      @TableName  VARCHAR(200) ,
      @NewGUID    NVARCHAR(200) ,
      @TemplateId NVARCHAR(200) ,
      @UserId     INT ,
      @LocaleId   INT,
	  @DefaultFamilyId INT = 0 
    )
AS
/*
    Summary :  Import Process call respective import method from @TemplateId 
    Process :  
*/
     BEGIN

	  DECLARE @ImportHeadName NVARCHAR(100), @SPScript nvarchar(max),@DatabaseName nvarchar(100) 

	  SELECT TOP 1 @ImportHeadName = Name FROM ZnodeImportTemplate zit inner join ZnodeImportHead zih on  zit.ImportHeadId =  zih.ImportHeadId
	  where zit.ImportTemplateId = @TemplateId
	  
	  SET @DatabaseName = DB_Name()

	  IF @ImportHeadName in ( 'Pricing', 'Product','Inventory')
	  BEGIN 
		SET @SPScript = N' EXEC Znode_ImportValidatePimProductData @TableName = ' + @TableName + 
		',@NewGUID = ' + @NewGUID + ' ,@TemplateId = ' + CONVERT(varchar(100), @TemplateId)  + 
		',@UserId = ' + CONVERT(varchar(100), @UserId ) + ',@LocaleId = ' + CONVERT(varchar(100), @LocaleId ) + ',@IsCategory= 0 ,@DefaultFamilyId = ' + CONVERT(varchar(100), @DefaultFamilyId)  + '  ,@ImportHeadName = ''' + @ImportHeadName + ''''
			  
		--Add a job
		DECLARE @jobId BINARY(16)
		SET @NewGUID = Replace (@NewGUID, '''','')
		EXEC  msdb.dbo.sp_add_job @job_name=@NewGUID, 
				@enabled=1, 
				@notify_level_eventlog=0, 
				@notify_level_email=2, 
				@notify_level_netsend=2, 
				@notify_level_page=2, 
				@delete_level=0, 
				@category_name=N'[Uncategorized (Local)]', 
				@owner_login_name=N'sa', @job_id = @jobId OUTPUT
		select @jobId

		EXEC msdb.dbo.sp_add_jobserver @job_name=@NewGUID, @server_name = N'MSSVR039'

		EXEC msdb.dbo.sp_add_jobstep @job_name=@NewGUID, @step_name=N'Import', 
				@step_id=1, 
				@cmdexec_success_code=0, 
				@on_success_action=1, 
				@on_fail_action=2, 
				@retry_attempts=0, 
				@retry_interval=0, 
				@os_run_priority=0, @subsystem=N'TSQL', 
				@command = @SPScript, 
				@database_name=N'Znode_Multifront_90_Dev', 
				@flags=0

		
		DECLARE @ReturnCode tinyint -- 0 (success) or 1 (failure)
		EXEC @ReturnCode=msdb.dbo.sp_start_job @job_name=@NewGUID , @server_name = N'mssvr039',@step_name = N'Import';
		--RETURN (@ReturnCode)
		select @ReturnCode
      END
	  Else IF @ImportHeadName = 'Category'
	  BEGIN 
		  EXEC Znode_ImportValidatePimProductData @TableName =@TableName ,@NewGUID = @NewGUID,@TemplateId =@TemplateId,@UserId =@UserId,
		  @LocaleId =@LocaleId   ,@IsCategory= 1,@DefaultFamilyId = @DefaultFamilyId ,@ImportHeadName = @ImportHeadName
       END
    END