CREATE PROCEDURE [dbo].[Znode_ImportData](
	  @TableName varchar(200), @NewGUID nvarchar(max), @TemplateId nvarchar(200), @UserId int, @LocaleId int= 1, @DefaultFamilyId int= 0, @IsDebug bit= 0, @PriceListId int= 0,@CountryCode Nvarchar(100) = '',@PortalId int = 0 ,
	  @IsDoNotCreateJob bit = 0 , @IsDoNotStartJob bit = 0, @StepName nvarchar(50) = 'Import' , @StartStepName nvarchar(50) = 'Import' ,
	  @step_id int  = 1 ,@Nextstep_id  int = 1 , @ERPTaskSchedulerId int = 0, @IsAccountAddress bit = 0,@IsAutoPublish Bit = 0  ,@ImportProcessLogId  int = 0,@PimCatalogId INT = 0, @PromotionTypeId	INT=0  )
AS
/*
    Summary :  Import Process call respective import method from @TemplateId 
    Process :  
	EXEC Znode_ImportValidatePimProductData @TableName = 'tempdb..[##SEODetails_61bbcb4c-5b83-49a0-8bb6-48eaf07f9ce0]',@NewGUID = '61bbcb4c-5b83-49a0-8bb6-48eaf07f9ce0' ,@TemplateId = 9,@UserId = 2,@PortalId = 0,@LocaleId = 1,@IsCategory= 1 ,@DefaultFamilyId = 0 ,@ImportHeadName = 'SEODetails', @ImportProcessLogId = 11, @PriceListId = 0, @CountryCode = ''
*/
BEGIN
BEGIN TRY 
	 DECLARE @ImportHeadName nvarchar(100), @SPScript nvarchar(max), @DatabaseName nvarchar(100), @ServerName nvarchar(100), @SPScript1 nvarchar(max),@UserName nvarchar(100);
	 DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
	 DECLARE @SPName nvarchar(100)
	
	 SELECT TOP 1 @ImportHeadName = Name
	 FROM ZnodeImportTemplate AS zit
		 INNER JOIN
		 ZnodeImportHead AS zih
		 ON zit.ImportHeadId = zih.ImportHeadId
	 WHERE zit.ImportTemplateId = @TemplateId;
	 SET @DatabaseName = DB_NAME();
	 SET @ServerName = @@serverName;/*We can use for the customization*/
	 SET @UserName = SYSTEM_USER;
	 
	 If (@ImportHeadName = 'ProductUpdate')
	 Begin
		SET @SPName = 'Znode_ImportPartialValidatePimProductData'
		SET @Nextstep_id = 1 
	 End
	 ELSE
	 Begin
		SET @SPName = 'Znode_ImportValidatePimProductData'
		SET @Nextstep_id = 3 
	 End

	--Generate new process for current import 
	If @ImportProcessLogId   = 0 
	Begin
		If @ERPTaskSchedulerId = 0 
			INSERT INTO ZnodeImportProcessLog( ImportTemplateId, Status, ProcessStartedDate, ProcessCompletedDate, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
			   SELECT @TemplateId, dbo.Fn_GetImportStatus( 0 ), @GetDate, NULL, @UserId, @GetDate, @UserId, @GetDate;
		else 
			INSERT INTO ZnodeImportProcessLog( ImportTemplateId, Status, ProcessStartedDate, ProcessCompletedDate, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate ,ERPTaskSchedulerId)
			   SELECT @TemplateId, dbo.Fn_GetImportStatus( 0 ), @GetDate, NULL, @UserId, @GetDate, @UserId, @GetDate , @ERPTaskSchedulerId;
		SET @ImportProcessLogId = @@IDENTITY;
	
	End
	
	SET @SPScript1 = N' EXEC ' + @SPName + ' @TableName = '''+@TableName+''',@NewGUID = '''+@NewGUID+''' ,@TemplateId = '
					+CONVERT(varchar(100), @TemplateId)+',@UserId = '+CONVERT(varchar(100), @UserId)
					+',@PortalId = '+CONVERT(varchar(100), @PortalId)+
					+',@IsAccountAddress = '+CONVERT(varchar(100), @IsAccountAddress)
					+',@LocaleId = '+CONVERT(varchar(100), @LocaleId)+',@IsCategory= '+CASE
					WHEN @ImportHeadName IN( 'Pricing', 'Product', 'Inventory' ) THEN '0'
					ELSE '1'
					END+' ,@DefaultFamilyId = '+CONVERT(varchar(100), @DefaultFamilyId)+' ,@ImportHeadName = '''+@ImportHeadName+''', @ImportProcessLogId = '
					+CONVERT(varchar(100), @ImportProcessLogId)+', @PriceListId = '+CONVERT(varchar(100), @PriceListId)
					+', @PimCatalogId = '+CONVERT(varchar(100), @PimCatalogId)+ ', @CountryCode = ''' + @CountryCode  + ''', @PromotionTypeId = '+CONVERT(varchar(100), @PromotionTypeId)+'';

	    
	  IF @IsAutoPublish = 1 
	  BEGIN 
			SET @SPScript1 = @SPScript1 + N' 
		   
			DECLARE @PimProductId Transferid 

			INSERT INTO  @PimProductId 
			SELECT DISTINCT  c.PimProductId 
			FROM ZnodeImportSuccessLog a 
			INNER JOIN ZnodePimAttributeValueLocale  b ON (b.AttributeValue = a.ImportedSku)
			INNER JOIN ZnodePimAttributeValue c ON (c.PimAttributeValueId = b.PimAttributeValueId)
			INNER JOIN ZnodePimAttribute d ON (d.PimAttributeId = c.PimAttributeId)
			WHERE d.AttributeCode = ''SKU''
			AND a.ImportedGuId = '''+@NewGUID+'''
			
			Exec [Znode_PublishSingleProductEntity] @PimProductId = @PimProductId  , @UserId = 2 ,  @RevisionType = ''PRODUCTION''  
			,@IsAutoPublish = 1,@ImportGUID = '''+@NewGUID+'''

			UPDATE ZnodeImportSuccessLog 
			SET    IsProductPublish =  1 
			WHERE ImportedGuId = '''+@NewGUID+'''

		'

		print @SPScript1
	  END 

	IF @@VERSION LIKE '%Azure%' OR @@VERSION LIKE '%Express Edition%'
	BEGIN
	      EXEC sys.sp_sqlexec @SPScript1;
	END;
	ELSE
	BEGIN 
		
		IF @IsDebug = 1
		          BEGIN
		              EXEC sys.sp_sqlexec
		                   @SPScript1;
		              RETURN 0;
		          END;
		--Add a job

		SET @SPScript1 = N' EXEC '+ @SPName + ' @TableName = '''''+@TableName+''''',@NewGUID = '''''+@NewGUID+''''' ,@TemplateId = '+CONVERT(varchar(100), @TemplateId)+',@UserId = '+CONVERT(varchar(100), @UserId)+',@PortalId = '+CONVERT(varchar(100), @PortalId)+',@LocaleId = '+CONVERT(varchar(100), @LocaleId)+',@IsCategory= '+CASE
																																																																										   WHEN @ImportHeadName IN( 'Pricing', 'Product', 'Inventory' ) THEN '0'
																																																																										   ELSE '1'
																																																																										   END
					+' ,@DefaultFamilyId = '+CONVERT(varchar(100), @DefaultFamilyId)+' ,@ImportHeadName = '''''+@ImportHeadName+''''', @ImportProcessLogId = '+CONVERT(varchar(100), @ImportProcessLogId)+', @PriceListId = '+CONVERT(varchar(100), @PriceListId)+',@IsAccountAddress = '+CONVERT(varchar(100), @IsAccountAddress)+', @PimCatalogId = '+CONVERT(varchar(100), @PimCatalogId)
																																																																										   + ', @CountryCode = ''''' + @CountryCode  +''''', @PromotionTypeId = '+CONVERT(varchar(100), @PromotionTypeId)+'';


		
		 IF @IsAutoPublish = 1 
	  BEGIN 
		SET @SPScript1 = @SPScript1 + N' 
		   
			DECLARE @PimProductId Transferid 

			INSERT INTO  @PimProductId 
			SELECT DISTINCT  b.PimProductId 
			FROM ZnodeImportSuccessLog a 
			INNER JOIN View_loadManageProductInternal b ON (b.AttributeValue = a.ImportedSku)
			WHERE b.AttributeCode = ''''SKU''''
			AND a.ImportedGuId = '''''+@NewGUID+'''''
			
			
			Exec [Znode_PublishSingleProductEntity] @PimProductId = @PimProductId  , @UserId = 2 ,  @RevisionType = ''''PRODUCTION''''  
			,@IsAutoPublish = 1,@ImportGUID = '''''+@NewGUID+'''''

						
			UPDATE ZnodeImportSuccessLog 
			SET    IsProductPublish =  1 
			WHERE ImportedGuId = '''''+@NewGUID+'''''
			 '
	  END 

		DECLARE @jobId binary(16);
		
		SET @NewGUID = 'Import_'+REPLACE(@NewGUID, '''', '');
		

		IF @IsDoNotCreateJob =0 
		Begin
			SET @SPScript = N'EXEC msdb.dbo.sp_add_job
				  @job_name = '''+@NewGUID+''' ,
				  @enabled = 1,
				  @notify_level_eventlog = 2,
				  @notify_level_email = 2,
				  @notify_level_netsend = 2,
				  @notify_level_page = 2,
				  @delete_level = 3,
				  @category_name = N''[Uncategorized (Local)]'',
				  @owner_login_name = N'''+ @UserName +'''';

			EXEC sys.sp_sqlexec @SPScript;

			SET @SPScript = N' EXEC msdb.dbo.sp_add_jobserver
				  @job_name = '''+@NewGUID+'''';

			EXEC sys.sp_sqlexec @SPScript;
		END

		SET @SPScript = N' EXEC msdb.dbo.sp_add_jobstep
              @job_name = '''+ @NewGUID +''',
              @step_name = N'''+ @StepName +''',
			  @step_id =  ' + Convert(nvarchar(10),@step_id ) +  ',
			  @cmdexec_success_code = 0,
              @on_success_action = ' + Convert(nvarchar(10),@Nextstep_id ) +  ',
              @on_fail_action = '    + Convert(nvarchar(10),@Nextstep_id ) +  ',
			  @retry_attempts = 0,
              @retry_interval = 0,
              @os_run_priority = 0,
              @subsystem = N''TSQL'',
              @command = N'''+ @SPScript1 +''',
              @database_name = '''+@DatabaseName+''',
              @flags = 0 ';
		PRINT  @SPScript
		EXEC sys.sp_sqlexec @SPScript;

		DECLARE @ReturnCode tinyint= 0; -- 0 (success) or 1 (failure)
		IF @IsDoNotStartJob = 0 
		Begin
			SET @SPScript = N'EXEC @ReturnCode = msdb.dbo.sp_start_job 
				  @job_name = '''+ @NewGUID +''',
				  @server_name = '''+ @ServerName +''',
				  @step_name = N''' + @StartStepName +'''';

			EXEC sys.SP_EXECUTESQL @SPScript, N'@ReturnCode TINYINT OUT', @ReturnCode = @ReturnCode OUT;
		END 
		
		SET @GetDate = dbo.Fn_GetDate();

		IF @ReturnCode = 1
		BEGIN
			UPDATE ZnodeImportProcessLog
			  SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
			WHERE ImportProcessLogId = @ImportProcessLogId
		END;
	END;
	END TRY 
	BEGIN CATCH 
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportData @TableName = '''+ISNULL(@TableName,'''''')+''',@NewGUID='''+ISNULL(CAST(@NewGUID AS
		VARCHAR(50)),'''''')+''',@TemplateId='''+ISNULL(CAST(@TemplateId AS VARCHAR(50)),'''''')+''',@CountryCode='''+ISNULL(@CountryCode,'''''')+''',
		@UserId='+ISNULL(CAST(@UserId AS VARCHAR(50)),'''')+',@DefaultFamilyId='+ISNULL(CAST(@DefaultFamilyId AS VARCHAR(50)),'''')+',@PriceListId='+ISNULL(CAST(@PriceListId AS VARCHAR(50)),'''')+',@PortalId='+ISNULL(CAST(@PortalId AS VARCHAR(50)),'''')
            
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    

		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ImportData',
		@ErrorInProcedure = 'Znode_ImportData',
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH 
END;