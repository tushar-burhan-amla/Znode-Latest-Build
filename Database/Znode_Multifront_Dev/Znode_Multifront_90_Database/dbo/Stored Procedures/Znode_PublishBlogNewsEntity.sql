CREATE PROCEDURE [dbo].[Znode_PublishBlogNewsEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@BlogNewsCode Varchar(300) = ''
  ,@BlogNewsType Varchar(300) = ''
  ,@UserId int = 0
  ,@Status Bit =0 OUTPUT 

)
AS

/*
  To publish all Blob News for respective code with their seo details.  
	Insert data into following entitis for webstore version entity 
	ZnodePublishSEOEntity
	Znode_SetPublishBlogNewsEntity

	Unit Testing : 
  
--declare @Status bit 
--EXEC Znode_PublishBlogNewsEntity
--   @PortalId  = 0 
--  ,@LocaleId  = 0 
--  ,@RevisionState  = 'Production' 
--  ,@BlogNewsCode = ''
--  ,@BlogNewsType = ''
--  ,@UserId = 2
--  ,@Status =@Status  OUTPUT 

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
		INSERT INTO ZnodePublishBlogNewsErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishWebStoreEntity', @RevisionState, 'Faild : WebStore version not found' , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
			Return 0 
	End
	
	Begin Transaction 
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
				,@BlogNewsCode = @BlogNewsCode 
				,@BlogNewsType = @BlogNewsType
				,@Status = @Status Output 

				INSERT INTO ZnodePublishBlogNewsErrorLogEntity
				(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
				SELECT 'ZnodePublishBlogNewsEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
				@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status = 0 
				Rollback Transaction 
				 
			INSERT INTO ZnodePublishBlogNewsErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishContentPageConfigEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
			
			If @Status = 0 
				Begin
					update ZnodeBlogNews SET  PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() 
					where BlogNewsType = @BlogNewsType  and BlogNewsCode = @BlogNewsCode 
					Return  0  
				End
	End
	
	if @Type = 'ZnodePublishSEOEntity' OR @Type = ''
	Begin
		Exec [Znode_SetPublishSEOEntity]
			 @PortalId  = @PortalId
			,@LocaleId  = @LocaleId 
			,@IsPreviewEnable =@IsPreviewEnable 
			,@PreviewVersionId  = 0 
			,@ProductionVersionId = 0
			,@RevisionState = @RevisionState 
			,@CMSSEOTypeId = '5'
			,@CMSSEOCode = @BlogNewsCode
			,@UserId = @UserId  
			,@Status = @Status Output 
			
			If @Status = 0 
				Rollback Transaction 
				
			INSERT INTO ZnodePublishBlogNewsErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishSEOEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , @GetDate, 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status = 0 
				Begin
					update ZnodeBlogNews SET  PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() 
					where BlogNewsType = @BlogNewsType  and BlogNewsCode = @BlogNewsCode 
					update ZnodeCMSSEODEtail SET  IsPublish = 1 , PublishStateId  = DBO.Fn_GetPublishStateIdForPublishFailed() where 
					SEOCode = @CMSSEOCode and  (PortalId = @PortalId OR @PortalId  =0 )  AND CMSSEOTypeId = 5 
					Return  0  
				End
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
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishBlogNewsEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+''',@BlogNewsCode= ' + CAST(@BlogNewsCode  AS varchar(20))
		+''',@BlogNewsType= ' + CAST(@BlogNewsType  AS varchar(20))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	
			
	INSERT INTO ZnodePublishErrorLogEntity
	(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
	SELECT 'Znode_PublishBlogNewsEntity', @RevisionState + isnull(@ErrorMessage,'') , 'Fail' , @GetDate, 
	@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

		                			 
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_PublishBlogNewsEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END
