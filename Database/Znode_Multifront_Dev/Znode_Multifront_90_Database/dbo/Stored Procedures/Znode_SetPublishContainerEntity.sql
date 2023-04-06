CREATE PROCEDURE [dbo].[Znode_SetPublishContainerEntity]
(
	 @PreviewVersionId INT = 0
	,@IsPreviewEnable INT = 0
	,@ProductionVersionId INT = 0
	,@RevisionState VARCHAR(50) = 'PRODUCTION'
	,@ContainerKey NVARCHAR(100) = ''
	,@UserId INT = 0
	,@Status INT = 0 OUTPUT
)
AS
/*

	This Procedure is used to publish the Content Container and its varients publish

	EXEC  [Znode_SetPublishContainerEntity] @VersionId = 0,@PreviewVersionId=0,@IsPreviewEnable=1,@ProductionVersionId=0,
		@RevisionState='Preview',@ContainerKey='DiwaliOffer11',@UserId=2,@Status=0

				EXEC  [Znode_SetPublishContainerEntity] @PreviewVersionId=0,@IsPreviewEnable=1,@ProductionVersionId=0,
		@RevisionState='Preview',@ContainerKey='',@UserId=2,@Status=0

		EXEC  [Znode_SetPublishContainerEntity] @VersionId = 0,@PreviewVersionId=0,@IsPreviewEnable=0,@ProductionVersionId=0,
		@RevisionState='Production',@ContainerKey='',@UserId=2,@Status=0

*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
BEGIN TRAN ContentContainer
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	DECLARE @OldPreviewId INT, @OldProductionId INT
	IF @PreviewVersionId = 0 AND @ContainerKey = '' AND @RevisionState = 'PREVIEW'
	BEGIN
		SET @OldPreviewId = ( SELECT TOP 1 VersionId FROM ZnodePublishContetContainerVersionEntity WHERE PublishState ='PREVIEW')
			
		INSERT INTO ZnodePublishContetContainerVersionEntity(PublishStartTime,PublishState)
		SELECT @GetDate,'PREVIEW'
		SET @PreviewVersionId = @@IDENTITY

		DELETE FROM ZnodePublishContetContainerVersionEntity WHERE VersionId = @OldPreviewId 
	END
	ELSE IF @RevisionState = 'PREVIEW'
	BEGIN
		INSERT INTO ZnodePublishContetContainerVersionEntity(PublishStartTime,PublishState)
		SELECT @GetDate,'PREVIEW'
		WHERE NOT EXISTS (SELECT * FROM ZnodePublishContetContainerVersionEntity WHERE PublishState <> 'PREVIEW')
		
		SET @PreviewVersionId = ( SELECT TOP 1 VersionId FROM ZnodePublishContetContainerVersionEntity WHERE PublishState ='PREVIEW')
	END
			
	IF @ProductionVersionId = 0 AND @ContainerKey = '' AND @RevisionState = 'PRODUCTION'
 	BEGIN
		SET @OldProductionId = ( SELECT TOP 1 VersionId FROM ZnodePublishContetContainerVersionEntity WHERE PublishState ='PRODUCTION')
			
		INSERT INTO ZnodePublishContetContainerVersionEntity(PublishStartTime,PublishState)
		SELECT @GetDate,'PRODUCTION'
		SET @ProductionVersionId = @@IDENTITY

		DELETE FROM ZnodePublishContetContainerVersionEntity WHERE VersionId = @OldProductionId 
	END 
	ELSE IF @RevisionState = 'PRODUCTION'
	BEGIN
		INSERT INTO ZnodePublishContetContainerVersionEntity(PublishStartTime,PublishState)
		SELECT @GetDate,'PRODUCTION'
		WHERE NOT EXISTS (SELECT * FROM ZnodePublishContetContainerVersionEntity WHERE PublishState <> 'PRODUCTION')

		SET @ProductionVersionId = ( SELECT TOP 1 VersionId FROM ZnodePublishContetContainerVersionEntity WHERE PublishState ='PRODUCTION')
	END

	
	DECLARE @TBL_ContentContainerEntity TABLE
	(
		CMSContentContainerId INT, Name NVARCHAR(100), ContainerKey NVARCHAR(100),	FamilyId INT, Tags NVARCHAR(1000),
		CreatedBy INT, CreatedDate DATETIME, ModifiedBy INT,	ModifiedDate DATETIME
	);

	INSERT INTO @TBL_ContentContainerEntity(CMSContentContainerId, Name, ContainerKey, FamilyId, Tags, CreatedDate, ModifiedDate)
	SELECT DISTINCT ZCW.CMSContentContainerId, ZCW.Name, ZCW.ContainerKey, ZCW.FamilyId, ZCW.Tags,
		@GetDate, @GetDate
	FROM ZnodeCMSContentContainer ZCW WITH (NOLOCK)
	WHERE ( ZCW.ContainerKey = @ContainerKey OR ISNULL(@ContainerKey,'') = '' )

	IF (@RevisionState LIKE '%Preview%') 
	BEGIN
		--Data inserted INTO flat table ZnodePublishContentContainerEntity 
		DELETE FROM ZnodePublishContentContainerEntity WHERE VersionId = @PreviewVersionId
		AND EXISTS(SELECT * FROM @TBL_ContentContainerEntity cc WHERE ZnodePublishContentContainerEntity.ContainerKey = CC.ContainerKey)

		INSERT INTO ZnodePublishContentContainerEntity
		(
			VersionId,Name,ContainerKey,FamilyId,Tags,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
		)
		SELECT @PreviewVersionId,Name, ContainerKey, FamilyId, Tags, @UserId, CreatedDate, @UserId, ModifiedDate
		FROM @TBL_ContentContainerEntity A

		DELETE FROM ZnodePublishContentContainerEntity WHERE VersionId = @OldPreviewId

	END
	---------------------------- END Preview 
	IF (@RevisionState LIKE '%Production%' OR @RevisionState = 'None')
	BEGIN

		-- Only production version id will process 
		DELETE FROM ZnodePublishContentContainerEntity WHERE VersionId = @ProductionVersionId
		AND EXISTS(SELECT * FROM @TBL_ContentContainerEntity cc WHERE ZnodePublishContentContainerEntity.ContainerKey = CC.ContainerKey)
		
		INSERT INTO ZnodePublishContentContainerEntity
		(
			VersionId,Name,ContainerKey,FamilyId,Tags,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
		)
		SELECT @ProductionVersionId,Name, ContainerKey, FamilyId, Tags, @UserId, CreatedDate, @UserId, ModifiedDate
		FROM @TBL_ContentContainerEntity A
				
		DELETE FROM ZnodePublishContentContainerEntity WHERE VersionId = @OldProductionId
	END

	-- Content container varient publish
	EXEC [Znode_PublishContentContainerVariantEntity] @ContainerKey = @ContainerKey,@RevisionState=@RevisionState, @UserId = @UserId,@OldPreviewId=@OldPreviewId,@OldProductionId=@OldProductionId,@Status = 0
	
	SET @Status =1 ;
	IF (@RevisionState = 'Preview')
		UPDATE B SET PublishStateId = (SELECT dbo.Fn_GetPublishStateIdForPreview()) --, ISPublish = 1 
		FROM @TBL_ContentContainerEntity A
		INNER JOIN ZnodeCMSContentContainer B ON A.CMSContentContainerId = B.CMSContentContainerId
				
	ELSE IF (@RevisionState = 'Production' Or @RevisionState = 'None' )
		UPDATE B SET PublishStateId = (SELECT dbo.Fn_GetPublishStateIdForPublish()) --, ISPublish = 1 
		FROM @TBL_ContentContainerEntity A
		INNER JOIN ZnodeCMSContentContainer B ON A.CMSContentContainerId = B.CMSContentContainerId
	
COMMIT TRAN ContentContainer
SELECT 1 ID, @Status Status;
END TRY 
BEGIN CATCH
	SET @Status =0
	ROLLBACK TRAN ContentContainer
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
	@ErrorLine VARCHAR(100)= ERROR_LINE(),
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_SetPublishContainerEntity
	@PreviewVersionId = ' + CAST(@PreviewVersionId AS VARCHAR(20))
	+',@IsPreviewEnable='+CAST(@IsPreviewEnable AS VARCHAR(10))
	+',@ProductionVersionId = ' + CAST(@ProductionVersionId AS VARCHAR(20))
	+',@RevisionState = ''' + CAST(@RevisionState AS VARCHAR(50))
	+',@UserId = ' + CAST(@UserId AS VARCHAR(20))
	+',@Status='+CAST(@Status AS VARCHAR(10));
	SELECT 0 AS ID,CAST(@Status AS BIT) AS Status;

	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_SetPublishContainerEntity',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH
END