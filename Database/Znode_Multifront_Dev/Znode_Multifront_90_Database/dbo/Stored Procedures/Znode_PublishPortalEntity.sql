CREATE PROCEDURE [dbo].[Znode_PublishPortalEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@UserId int = 0
  ,@Status Bit =0 OUTPUT 
  ,@IsContentType Bit= 1
  ,@NewGUID nvarchar(500)  
)
AS
/*
  To publish all Content pages and their mapping into their respective entities 
	ZnodePublishContentPageConfigEntity
	ZnodePublishSEOEntity
	ZnodePublishWidgetProductEntity
	ZnodePublishMediaWidgetEntity
	ZnodePublishSearchWidgetEntity
	ZnodePublishTextWidgetEntity
	ZnodePublishWidgetSliderBannerEntity
	ZnodePublishWidgetTitleEntity

	Unit Testing : 
	Declare @Status bit 
	

	Declare @Status bit 
	Exec [dbo].[Znode_PublishPortalEntity]
     @PortalId  = 1 
	,@LocaleId  = 0 
	,@RevisionState = 'PRODUCTION' 
	,@UserId = 2
	,@Status = @Status 
	--Select @Status 


*/
BEGIN
BEGIN TRY 
SET NOCOUNT ON
	DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
	Declare @PortalCode Varchar(100)
	Declare @Type varchar(50) = '',	@CMSSEOCode varchar(300),@UserName Varchar(50);
	SET @Status = 1 
	Declare @IsPreviewEnable int,@PreviewVersionId INT = 0  ,@ProductionVersionId INT = 0
	
	Select TOP 1  @UserName = aspNetZnodeUser.UserName from ZnodeUser Inner Join aspNetUsers ON ZnodeUser.aspNetUserId = aspNetUsers.Id 
	Inner Join aspNetZnodeUser on aspNetUsers.UserName = aspNetZnodeUser.AspNetZnodeUserId
	where ZnodeUser.UserId = @userId
            


 		If Exists (SELECT  * from ZnodePublishStateApplicationTypeMapping PSA where PSA.IsEnabled =1 and  
		Exists (select TOP 1 1  from ZnodePublishState PS where PS.PublishStateId = PSA.PublishStateId ) and ApplicationType =  'WebstorePreview')
			SET @IsPreviewEnable = 1 
		else 
			SET @IsPreviewEnable = 0 

		--Generate preview entry 
		DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))
		
		DECLARE @TBL_StoreEntity TABLE 
		(
			 PortalThemeId	int,PortalId	int,ThemeId	int,ThemeName	varchar(200),CSSId	int,CSSName	nvarchar(2000),
			 WebsiteLogo	varchar(300),WebsiteTitle	nvarchar(400),FaviconImage	varchar(300),WebsiteDescription	nvarchar(MAX),
			 PublishState	varchar(100),LocaleId	int	
		)
		
		IF object_id('tempdb..[#Tbl_VersionEntity]') IS NOT NULL
			drop table tempdb..#Tbl_VersionEntity
		Create Table #Tbl_VersionEntity(PortalId int , VersionId int , LocaleId int , PublishType varchar(50) )

		IF object_id('tempdb..[#Tbl_OldVersionEntity]') IS NOT NULL
			drop table tempdb..#Tbl_OldVersionEntity
		Create Table #Tbl_OldVersionEntity(PortalId int , NewVersionId int ,OldVersionId int , LocaleId int , PublishType varchar(50) )

	
		DECLARE @WebStoreEntityId int 
		
		select @PortalCode  = StoreName  from ZnodePortal where PortalId = @PortalId 
		
		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )
		
		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN 
			SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
			if (@IsPreviewEnable = 1 AND ( @RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) ) 
			Begin
				Insert into ZnodePublishPortalLog
				(PortalId,IsPortalPublished,UserId,LogDateTime,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Token,PublishStateId)
				Select @PortalId ,1 , @UserId , @GetDate,@UserId ,@GetDate ,@UserId ,@GetDate, NULL, DBO.Fn_GetPublishStateIdForProcessing()
				
				insert into #Tbl_VersionEntity (PortalId,VersionId,LocaleId,PublishType)
				select @PortalId, @@Identity , @SetLocaleId ,'PREVIEW'
				
			End
			If (@RevisionState like '%Production%' OR @RevisionState = 'None')
			Begin
				--Genrate production entry 
				Insert into ZnodePublishPortalLog
				(PortalId,IsPortalPublished,UserId,LogDateTime,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Token,PublishStateId)
				Select @PortalId ,1 , @UserId , @GetDate,@UserId ,@GetDate ,@UserId ,@GetDate, NULL, DBO.Fn_GetPublishStateIdForProcessing()
			
				insert into #Tbl_VersionEntity (PortalId,VersionId,LocaleId,PublishType)
				select @PortalId, @@Identity , @SetLocaleId ,'PRODUCTION'
			End 
	   	SET @IncrementalId = @IncrementalId +1 
		END 

	Truncate table ZnodePublishPortalErrorLogEntity

	Declare @IsFirstTimeContentPublish bit 
	If Exists (Select TOP 1 1  from ZnodePublishWebStoreEntity where PortalId = @PortalId)
		SET @IsFirstTimeContentPublish =1 
	else 
		SET @IsFirstTimeContentPublish =0
    
	Declare @Tbl_PreviewVersionId  TABLE (VersionId int , PortalId int , LocaleId int )
	Declare @Tbl_ProductionVersionId  TABLE (VersionId int , PortalId int , LocaleId int )

	If @IsContentType = 0 AND @IsFirstTimeContentPublish =1 
	Begin 
		Insert into #Tbl_OldVersionEntity (PortalId,NewVersionId,OldVersionId, LocaleId, PublishType)
		Select A.PortalId , A.VersionId, B.VersionId, a.LocaleId,a.PublishType from #Tbl_VersionEntity A Inner join ZnodePublishWebStoreEntity B on 
		A.PortalId = B.PortalId and A.LocaleId = B.LocaleId AND A.PublishType= B.PublishState  
	End
	Delete from ZnodePublishProgressNotifierEntity where JobName  = @PortalCode 
	
	INSERT INTO ZnodePublishProgressNotifierEntity
	(VersionId,JobId,JobName,ProgressMark,IsCompleted,IsFailed,ExceptionMessage,StartedBy,StartedByFriendlyName)
	Values(0,@NewGUID , Isnull(@PortalCode,'') + ' Store' , 0 , 0 , 0 , '' , @UserId, @UserName)

	if @Type = 'ZnodePublishWebStoreEntity' OR @Type = ''
	Begin
		 Declare  @PreviewVersionIdString varchar(1000)= ''  ,@ProductionVersionIdString varchar(1000) = '' 
		 SELECT   @PreviewVersionIdString = STUFF((SELECT ',' + cast (VersionId as varchar(50))  FROM #Tbl_VersionEntity   where PublishType = 'PREVIEW'  FOR XML PATH ('')), 1, 1, '') 
		 SELECT   @ProductionVersionIdString = STUFF((SELECT ',' + cast (VersionId as varchar(50))  FROM #Tbl_VersionEntity   where PublishType = 'PRODUCTION'  FOR XML PATH ('')), 1, 1, '') 
		 
		 EXEC [dbo].[Znode_SetPublishWebStoreEntity]
			 @PortalId  = @PortalId 
			,@LocaleId   = @LocaleId 
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = @PreviewVersionIdString 
			,@ProductionVersionId = @ProductionVersionIdString 
			,@RevisionState = @RevisionState 
			,@UserId = @UserId	
			,@Status = @Status Output 

			INSERT INTO ZnodePublishPortalErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishWebStoreEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			Update ZnodePublishProgressNotifierEntity SET 
			ProgressMark =5 , 
			IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
			IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
			where  JobId = @NewGUID
	End
	
	if (@Type = 'ZnodePublishPortalBrandEntity' OR @Type = '' ) AND @Status = 1 
	Begin
			Exec [Znode_SetPublishPortalBrandEntity]
			 @PortalId  = @PortalId
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@UserId = @UserId  
			,@Status = @Status Output 

			INSERT INTO ZnodePublishPortalErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishPortalBrandEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
			
			Update ZnodePublishProgressNotifierEntity SET 
			ProgressMark = CASE When (@IsContentType = 1 OR (@IsFirstTimeContentPublish = 0 AND @IsContentType = 0 ) ) THEN 10 Else 100 End , 
			IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
			IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
			where  JobId = @NewGUID
	End 

	if (@Type = 'ZnodePublishSEOEntity' OR @Type = '') AND @Status = 1 and (@IsContentType = 0 or @PortalId > 0)
			Begin
					Exec [Znode_SetPublishSEOEntity]
					 @PortalId  = @PortalId
					,@LocaleId  = @LocaleId 
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = 0 
					,@ProductionVersionId = 0
					,@RevisionState = @RevisionState 
					,@CMSSEOTypeId = '4'
					,@CMSSEOCode = ''
					,@UserId = @UserId  
					,@Status = @Status Output 

			End 
	If (@IsContentType = 1 OR (@IsFirstTimeContentPublish = 0 AND @IsContentType = 0 ) ) AND @Status = 1  
	Begin
			if @Type = 'ZnodePublishBlogNewsEntity' OR @Type = '' 
			Begin
				 EXEC [dbo].[Znode_SetPublishBlogNewsEntity]
					 @PortalId  = @PortalId 
					,@LocaleId   = @LocaleId 
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@UserId = @UserId	
					,@Status = @Status Output 

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishBlogNewsEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 15 , 
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID

			End
			if (@Type = 'ZnodePublishPortalCustomCssEntity' OR @Type = '' ) AND @Status = 1  
			Begin
				 EXEC [dbo].[Znode_SetPublishPortalCustomCssEntity]
					 @PortalId  = @PortalId 
					,@LocaleId   = @LocaleId 
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@UserId = @UserId	
					,@Status = @Status Output 

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishPortalCustomCssEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 20  ,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID


			End
			if (@Type = 'ZnodePublishWidgetCategoryEntity' OR @Type = '' ) AND @Status = 1 
			Begin
				 EXEC [dbo].[Znode_SetPublishWidgetCategoryEntity]
					 @PortalId  = @PortalId 
					,@LocaleId   = @LocaleId 
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@UserId = @UserId	
					,@Status = @Status Output 

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishWidgetCategoryEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 25  ,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID
			End
	
			if (@Type = 'ZnodePublishWidgetProductEntity' OR @Type = '') AND @Status = 1  
			Begin
					EXEC Znode_SetPublishWidgetProductEntity
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@CMSMappingId = 0
					,@UserId = @UserId 
					,@Status = @Status  Output
			
					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishWidgetProductEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 30  ,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID
			END 

			if (@Type = 'ZnodePublishWidgetTitleEntity' OR @Type = '') AND @Status = 1  
			Begin
					EXEC Znode_SetPublishWidgetTitleEntity
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@CMSContentPagesId = 0
					,@UserId = @UserId 
					,@Status = @Status  Output

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishWidgetTitleEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 35  ,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID

			END 
			if (@Type = 'ZnodePublishWidgetSliderBannerEntity' OR @Type = '')AND @Status = 1  
			Begin
					EXEC Znode_SetPublishWidgetSliderBannerEntity
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@CMSContentPagesId = 0
					,@CMSSliderId = 0 
					,@UserId = @UserId 
					,@Status = @Status  Output
			
					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishWidgetSliderBannerEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
					
					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 40  ,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			
			END 
			if (@Type = 'ZnodePublishTextWidgetEntity' OR @Type = '' ) AND @Status = 1  
			Begin
					EXEC Znode_SetPublishTextWidgetEntity
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@CMSMappingId = 0
					,@UserId = @UserId 
					,@Status = @Status  Output
			
					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishTextWidgetEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 45  ,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			

			END 
			if (@Type = 'ZnodeSetPublishMediaWidgetEntity' OR @Type = '') AND @Status = 1
			Begin
					EXEC Znode_SetPublishMediaWidgetEntity
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@CMSMappingId = 0
					,@UserId = @UserId 
					,@Status = @Status  Output
			
					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishMediaWidgetEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
										
					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 50  ,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			
			END 
			if (@Type = 'ZnodePublishSearchWidgetEntity' OR @Type = '') AND @Status = 1
			Begin
					EXEC Znode_SetPublishSearchWidgetEntity
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@CMSMappingId = 0
					,@UserId = @UserId 
					,@Status = @Status  Output
			
					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishSearchWidgetEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
					
					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 55  ,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			
			END 

			if (@Type = 'ZnodePublishContentPageConfigEntity' OR @Type = '') AND @Status = 1
			Begin
				 EXEC [dbo].[Znode_SetPublishContentPageConfigEntity]
					 @PortalId  = @PortalId 
					,@LocaleId   = @LocaleId 
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@CMSContentPagesId = 0
					,@UserId = @UserId	
					,@Status = @Status Output 

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishContentPageConfigEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
					
					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 60,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			
			End

			if (@Type = 'ZnodePublishSEOEntity' OR @Type = '') AND @Status = 1
			Begin
					Exec [Znode_SetPublishSEOEntity]
					 @PortalId  = @PortalId
					,@LocaleId  = @LocaleId 
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = 0 
					,@ProductionVersionId = 0
					,@RevisionState = @RevisionState 
					,@CMSSEOTypeId = '3,5'
					,@CMSSEOCode = ''
					,@UserId = @UserId  
					,@Status = @Status Output 

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishSEOEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					
					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 60,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			

			End 
			if (@Type = 'ZnodePublishMessageEntity' OR @Type = '') AND @Status = 1
			Begin
					Exec [Znode_SetPublishMessageEntity]
					 @PortalId  = @PortalId
					,@LocaleId  = @LocaleId 
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = 0 
					,@ProductionVersionId = 0
					,@RevisionState = @RevisionState 
					,@UserId = @UserId  
					,@Status = @Status Output 

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishMessageEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 65,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			

			End 

		   if (@Type = 'ZnodePublishPortalGlobalAttributeEntity' OR @Type = '') AND @Status = 1
			Begin
					Exec [Znode_SetPublishPortalGlobalAttributeEntity]
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = 0 
					,@ProductionVersionId = 0
					,@RevisionState = @RevisionState 
					,@UserId = @UserId  
					,@Status = @Status Output 

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishPortalGlobalAttributeEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
					
					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 67,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			

			End 
 
		   if (@Type = 'ZnodePublishProductPageEntity' OR @Type = '') AND @Status = 1
			Begin
					Exec [Znode_SetPublishProductPageEntity]
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = 0 
					,@ProductionVersionId = 0
					,@RevisionState = @RevisionState 
					,@UserId = @UserId  
					,@Status = @Status Output 

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishProductPageEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 73,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			


			End 
			
			if (@Type = 'ZnodePublishWidgetBrandEntity' OR @Type = '') AND @Status = 1
			Begin
					Exec [Znode_SetPublishWidgetBrandEntity]
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = 0 
					,@ProductionVersionId = 0
					,@RevisionState = @RevisionState 
					,@UserId = @UserId  
					,@Status = @Status Output 

					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishWidgetBrandEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 80,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			

			End 

			if (@Type = 'ZnodePublishContainerWidgetEntity' OR @Type = '' ) AND @Status = 1  
			Begin
					EXEC Znode_SetPublishContainerWidgetEntity
					 @PortalId  = @PortalId
					,@IsPreviewEnable =@IsPreviewEnable 
					,@PreviewVersionId  = @PreviewVersionId 
					,@ProductionVersionId = @ProductionVersionId 
					,@RevisionState = @RevisionState 
					,@CMSMappingId = 0
					,@UserId = @UserId 
					,@Status = @Status  Output
			
					INSERT INTO ZnodePublishPortalErrorLogEntity
					(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
					SELECT 'ZnodePublishContainerWidgetEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
					@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

					Update ZnodePublishProgressNotifierEntity SET 
					ProgressMark = 90  ,
					IsCompleted  = Case when Isnull(@Status,0) = 0 then 1  Else 0 end,
					IsFailed =Case when Isnull(@Status,0) = 0 then 1  Else 0 end  
					where  JobId = @NewGUID			

			END 

	End
	SET @GetDate = dbo.Fn_GetDate();
		IF Exists (select TOP 1 1  from ZnodePublishPortalErrorLogEntity where  ProcessStatus = 'Fail') 
		Begin
			SET @Status  =0 
			SELECT 1 AS ID,@Status AS Status;
			INSERT INTO ZnodePublishPortalErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishPortalEntity', @RevisionState , 'Fail' , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
			
			Delete  From ZnodePublishWebStoreEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId
			Delete  From ZnodePublishBlogNewsEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishPortalCustomCssEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishWidgetCategoryEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishWidgetProductEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishWidgetTitleEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishWidgetSliderBannerEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishTextWidgetEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishMediaWidgetEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishSearchWidgetEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishContentPageConfigEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishSEOEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId and CMSSEOTypeId in (3,5)
			Delete  From ZnodePublishMessageEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishPortalGlobalAttributeEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishPortalBrandEntity Where  VersionId  in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishProductPageEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Delete  From ZnodePublishWidgetBrandEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
		    Delete  From ZnodePublishContainerWidgetEntity Where  VersionId in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
			Update ZnodePublishPortalLog SET PublishStateId = DBO.Fn_GetPublishStateIdForPublishFailed(),ModifiedDate=@GetDate  where  PublishPortalLogId in  (Select VersionId from #Tbl_VersionEntity Where PublishType = 'PREVIEW' )
			Update ZnodePublishPortalLog SET PublishStateId = DBO.Fn_GetPublishStateIdForPublishFailed(),ModifiedDate=@GetDate  where  PublishPortalLogId in (Select VersionId from #Tbl_VersionEntity Where PublishType = 'PRODUCTION' )

		End
	Else 
		Begin
			Update ZnodePublishPortalLog SET PublishStateId = DBO.Fn_GetPublishStateIdForPreview(),ModifiedDate=@GetDate  where  PublishPortalLogId in 
			(Select VersionId from #Tbl_VersionEntity Where PublishType = 'PREVIEW' )
			Update ZnodePublishPortalLog SET PublishStateId = DBO.Fn_GetPublishStateIdForPublish(),ModifiedDate=@GetDate  where  PublishPortalLogId in
			(Select VersionId from #Tbl_VersionEntity Where PublishType = 'PRODUCTION' )
			 
			Insert into ZnodePublishPreviewLogEntity
			(VersionId,PublishStartTime,IsDisposed,SourcePublishState,EntityId,EntityType,LogMessage,LogCreatedDate,PreviousVersionId,LocaleId,LocaleDisplayValue)
				Select A.VersionId,NULL,NULL,A.PublishType,@PortalId,'portal','portal has been published successfully' , @GetDate,  
				(select TOP 1 VersionId   from ZnodePublishWebStoreEntity where LocaleId = A.LocaleId AND PublishState = A.PublishType
				 and PortalId = @PortalId),A.LocaleId,B.Name
				from #Tbl_VersionEntity  A  Inner join ZnodeLocale B on A.LocaleId = B.LocaleId

			If @RevisionState = 'PREVIEW'
			Begin
				update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPreview() where (PortalId = @PortalId OR @PortalId  =0 ) 
				update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPreview() where 
				(PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
			End 
			Else 
			Begin
				update ZnodeCMSContentPages SET  IsPublished = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublish() where (PortalId = @PortalId OR @PortalId  =0 ) 
				update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublish() where 
				(PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 3 
			End
			
			if (@IsContentType =1  OR (@IsContentType = 0 AND @IsFirstTimeContentPublish =0))
			Begin
				If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
				Begin
					Delete  From ZnodePublishWebStoreEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishBlogNewsEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishPortalCustomCssEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetCategoryEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetProductEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetTitleEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetSliderBannerEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishTextWidgetEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishMediaWidgetEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishSearchWidgetEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishContentPageConfigEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishSEOEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId) And CMSSEOTypeId in (3,5) 
					Delete  From ZnodePublishMessageEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishPortalGlobalAttributeEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishPortalBrandEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishProductPageEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetBrandEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					Delete  From ZnodePublishContainerWidgetEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
				End
				If (@RevisionState like '%Production%' OR @RevisionState = 'None')
				Begin
					Delete  From ZnodePublishWebStoreEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishBlogNewsEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishPortalCustomCssEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetCategoryEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetProductEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetTitleEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetSliderBannerEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishTextWidgetEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishMediaWidgetEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishSearchWidgetEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishContentPageConfigEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishSEOEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId) And CMSSEOTypeId in (3,5) 
					Delete  From ZnodePublishMessageEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishPortalGlobalAttributeEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishPortalBrandEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishProductPageEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishWidgetBrandEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					Delete  From ZnodePublishContainerWidgetEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

				End
			End
			Else 
			Begin
				
				If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
				Begin
					Delete  From ZnodePublishWebStoreEntity           Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Delete  From ZnodePublishPortalBrandEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
				
					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishBlogNewsEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishPortalCustomCssEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					
					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetCategoryEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetProductEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetTitleEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetSliderBannerEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishTextWidgetEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishMediaWidgetEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishSearchWidgetEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId =6)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishContentPageConfigEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishSEOEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
					And CMSSEOTypeId in (3,5) 

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishMessageEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishPortalGlobalAttributeEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishProductPageEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetBrandEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishContainerWidgetEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PRODUCTION' AND PortalId = @PortalId)
	

				End
				If (@RevisionState like '%Production%' OR @RevisionState = 'None')
				Begin
					Delete  From ZnodePublishWebStoreEntity           Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Delete  From ZnodePublishPortalBrandEntity Where  VersionId not in (Select VersionId from #Tbl_VersionEntity) AND PortalId = @PortalId 
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
				
					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishBlogNewsEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishPortalCustomCssEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					
					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetCategoryEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetProductEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetTitleEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetSliderBannerEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishTextWidgetEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishMediaWidgetEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishSearchWidgetEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishContentPageConfigEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishSEOEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)
					And CMSSEOTypeId in (3,5)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishMessageEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishPortalGlobalAttributeEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishProductPageEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishWidgetBrandEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

					Update BA SET BA.VersionId = OV.NewVersionId from 
					ZnodePublishContainerWidgetEntity BA Inner join #Tbl_OldVersionEntity  OV on BA.VersionId = Ov.OldVersionId  AND BA.PortalId =Ov.PortalId  
					AND VersionId NOT IN (select VersionId  from ZnodePublishWebStoreEntity where PublishState = 'PREVIEW' AND PortalId = @PortalId)

				End 
			End
		 SET @Status = 1
		End
	SELECT 1 AS ID,@Status AS Status;   

END TRY 
BEGIN CATCH 
	SET @Status =0  
	 SELECT 1 AS ID,@Status AS Status;   

	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishPortalEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	
			
	INSERT INTO ZnodePublishPortalErrorLogEntity
	(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
	SELECT 'ZnodePublishPortalEntity', @RevisionState + isnull(@ErrorMessage,'') , 'Fail' , @GetDate, 
	@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

		                			 
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_PublishPortalEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END