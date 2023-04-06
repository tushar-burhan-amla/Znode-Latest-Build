CREATE PROCEDURE [dbo].[Znode_PublishContentPageEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@CMSContentPagesId int = 0
  ,@UserId int = 0
  ,@Status Bit =0 OUTPUT 

)
AS

/*
  To publish all Contenet pages and their mapping into their respective entities 
	ZnodePublishContentPageConfigEntity
	ZnodePublishSEOEntity
	ZnodePublishWidgetProductEntity
	ZnodePublishMediaWidgetEntity
	ZnodePublishSearchWidgetEntity
	ZnodePublishTextWidgetEntity
	ZnodePublishWidgetSliderBannerEntity
	ZnodePublishWidgetTitleEntity

	Unit Testing : 
	Exec [dbo].[Znode_PublishContentPageEntity]
     @PortalId  = 1 
	,@LocaleId  = 0 
	,@RevisionState = 'Preview&Production' 
	,@CMSContentPagesId = 0
	,@UserId = 2
  
	declare @p8 int
	set @p8=NULL
		exec sp_executesql N'ZnodePublishContentPageEntity @PortalId,@LocaleId,@RevisionState,
		@CMSContentPagesId,@UserId,@Status OUT',N'@UserId int,@PortalId int,@LocaleId int,
		@RevisionState nvarchar(10),@CMSContentPagesId int,@Status int output',
		@UserId=2,@PortalId=1,@LocaleId=0,@RevisionState=N'PRODUCTION',@CMSContentPagesId=72,@Status=@p8 output
	select @p8


*/
BEGIN
BEGIN TRY 
SET NOCOUNT ON
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	Declare @Type varchar(50) = '',	@CMSSEOCode varchar(300);
	SET @Status = 1 
	Declare @IsPreviewEnable int,
	@PreviewVersionId INT = 0  ,@ProductionVersionId INT = 0

 	If Exists (SELECT  * from ZnodePublishStateApplicationTypeMapping PSA where PSA.IsEnabled =1 and  
	Exists (select TOP 1 1  from ZnodePublishState PS where PS.PublishStateId = PSA.PublishStateId ) and ApplicationType =  'WebstorePreview')
		SET @IsPreviewEnable = 1 
	else 
		SET @IsPreviewEnable = 0 

   If @PreviewVersionId = 0 
		-- select TOP 1 @PreviewVersionId =   PublishPortalLogId from ZnodePublishPortalLog where 
		--PortalId = @PortalId AND  PublishStateId  = [dbo].[Fn_GetPublishStateIdForPreview]() Order by 1 DESC   
		SELECT TOP 1 @PreviewVersionId =  VersionId  from  ZnodePublishWebStoreEntity where PortalId =  @PortalId and  (LocaleId = 	@LocaleId OR @LocaleId = 0  ) and PublishState ='PREVIEW'
   If @ProductionVersionId = 0 
		--select TOP 1  @ProductionVersionId =   PublishPortalLogId  from ZnodePublishPortalLog where 
		--PortalId = @PortalId AND PublishStateId  = [dbo].[Fn_GetPublishStateIdForPublish] () Order by 1 DESC  
		SELECT TOP 1 @ProductionVersionId =  VersionId  from  ZnodePublishWebStoreEntity where PortalId =  @PortalId and  (LocaleId = 	@LocaleId OR @LocaleId = 0  ) and PublishState ='PRODUCTION'
	
	Truncate table ZnodePublishErrorLogEntity 
	


	IF (@IsPreviewEnable = 1 AND @PreviewVersionId = 0 ) OR @ProductionVersionId =0 
	Begin
		SET @Status =0 
		SELECT 1 AS ID,@Status AS Status;  
		INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishWebStoreEntity', @RevisionState, 'Faild : WebStore version not found' , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
			Return 0 
	End
	
	Begin Transaction 
	if @Type = 'ZnodePublishContentPageConfigEntity' OR @Type = ''
	Begin
		 EXEC [dbo].[Znode_SetPublishContentPageConfigEntity]
			 @PortalId  = @PortalId 
			,@LocaleId   = @LocaleId 
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@CMSContentPagesId = @CMSContentPagesId
			,@UserId = @UserId	
			,@Status = @Status Output 

			If @Status = 0 
				Rollback Transaction 
				 
			INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishContentPageConfigEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
			
			If @Status = 0 
				Begin
					update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
					Return  0  
				End

	End
	
	if @Type = 'ZnodePublishSEOEntity' OR @Type = ''
	Begin
		
			SELECT @CMSSEOCode  = PageName  from ZnodeCMSContentPages where @CMSContentPagesId = CMSContentPagesId and  
			(PortalId = @PortalId OR @PortalId  =0 ) 
			
			SET @CMSSEOCode  = Isnull( @CMSSEOCode  , '') 
			
			Exec [Znode_SetPublishSEOEntity]
			 @PortalId  = @PortalId
			,@LocaleId  = @LocaleId 
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@CMSSEOTypeId = '3'
			,@CMSSEOCode = @CMSSEOCode  
			,@UserId = @UserId  
			,@Status = @Status Output 
			
			If @Status = 0 
				Rollback Transaction 
				
			INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishSEOEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status = 0 
				Begin
					update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
					Return  0  
				End


	End 

	if @Type = 'ZnodePublishWidgetProductEntity' OR @Type = ''
	Begin
			EXEC Znode_SetPublishWidgetProductEntity
			 @PortalId  = @PortalId
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@CMSMappingId = @CMSContentPagesId
			,@UserId = @UserId 
			,@Status = @Status  Output
			
			If @Status  = 0 
				Rollback Transaction 
			INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishWidgetProductEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
			
			If @Status = 0 
				Begin
					update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
					Return  0  
				End
	END 
	
	if @Type = 'ZnodeSetPublishMediaWidgetEntity' OR @Type = ''
	Begin
			EXEC Znode_SetPublishMediaWidgetEntity
			 @PortalId  = @PortalId
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@CMSMappingId = @CMSContentPagesId
			,@UserId = @UserId 
			,@Status = @Status  Output
			
			If @Status = 0 
				Rollback Transaction 
			INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishMediaWidgetEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status = 0 
				Begin
					update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
					Return  0  
				End

	END 

	if @Type = 'ZnodePublishSearchWidgetEntity' OR @Type = ''
	Begin
			EXEC Znode_SetPublishSearchWidgetEntity
			 @PortalId  = @PortalId
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@CMSMappingId = @CMSContentPagesId
			,@UserId = @UserId 
			,@Status = @Status  Output
			
			If @Status = 0 
				Rollback Transaction 
			INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishSearchWidgetEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status = 0 
				Begin
					update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
					Return  0  
				End
	END 
	
	if @Type = 'ZnodePublishTextWidgetEntity' OR @Type = ''
	Begin
			EXEC Znode_SetPublishTextWidgetEntity
			 @PortalId  = @PortalId
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@CMSMappingId = @CMSContentPagesId
			,@UserId = @UserId 
			,@Status = @Status  Output
			
			If @Status = 0 
				Rollback Transaction 
			INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishTextWidgetEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status = 0 
				Begin
					update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
					Return  0  
				End

	END 
	if @Type = 'ZnodeSetPublishWidgetSliderBannerEntity' OR @Type = ''
	Begin
			EXEC Znode_SetPublishWidgetSliderBannerEntity
			 @PortalId  = @PortalId
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@CMSContentPagesId = @CMSContentPagesId
			,@UserId = @UserId 
			,@Status = @Status  Output
			
			If @Status = 0 
				Rollback Transaction 
			INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishWidgetSliderBannerEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status = 0 
				Begin
					update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
					Return  0  
				End
	END 
	if @Type = 'ZnodeSetPublishWidgetTitleEntity' OR @Type = ''
	Begin
			EXEC Znode_SetPublishWidgetTitleEntity
			 @PortalId  = @PortalId
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@CMSContentPagesId = @CMSContentPagesId
			,@UserId = @UserId 
			,@Status = @Status  Output
			
			If @Status =0 
				Rollback Transaction
			INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishWidgetTitleEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status = 0 
				Begin
					update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
					Return  0  
				End

	END 

	if (@Type = 'ZnodePublishContainerWidgetEntity' OR @Type = '' ) AND @Status = 1  
			Begin
					EXEC Znode_SetPublishContainerWidgetEntity
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = 0 
					,@ProductionVersionId = 0 
					,@RevisionState = @RevisionState 
					,@CMSMappingId = @CMSContentPagesId
					,@UserId = @UserId 
					,@Status = @Status  Output
			
					If @Status =0 
				        Rollback Transaction
				INSERT INTO ZnodePublishErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishWidgetTitleEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status = 0 
				Begin
					update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
					Return  0  
				End	

			END

	IF Exists (select TOP 1 1  from ZnodePublishErrorLogEntity where  ProcessStatus <> 'Fail') 
		Begin
			If @RevisionState = 'PREVIEW'
			Begin
				update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPreview() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
				update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPreview() where 
				SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
			End 
			Else 
			Begin
				update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublish() where @CMSContentPagesId = CMSContentPagesId and  (PortalId = @PortalId OR @PortalId  =0 ) 
				update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublish() where 
				SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
			End
	    	SET @Status = 1
		End
		SELECT 1 AS ID,@Status AS Status;   
		Commit Transaction 
END TRY 
BEGIN CATCH 
	SET @Status =0  
	 SELECT 1 AS ID,@Status AS Status;   
	
	Rollback Transaction 

	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishContentPageEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+''',@CMSContentPagesId= ' + CAST(@CMSContentPagesId  AS varchar(20))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	
			
	INSERT INTO ZnodePublishErrorLogEntity
	(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
	SELECT 'ZnodePublishContentPageEntity', @RevisionState + isnull(@ErrorMessage,'') , 'Fail' , @GetDate, 
	@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

		                			 
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_PublishContentPageEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END