CREATE PROCEDURE [dbo].[Znode_PublishContentContainerVariantEntity]
(
	 @ContainerKey VARCHAR(100) = ''
	,@RevisionState VARCHAR(50) = 'Production'
	,@UserId INT = 0
	,@OldPreviewId INT = 0
	,@OldProductionId INT = 0
	,@ContainerProfileVariantId INT = 0
	,@Status BIT = 0 OUTPUT
)
AS
/*
	if profileid and portalid will null then it must be publish.
	This Procedure is used to publish the Content Container Variant against the store 

	EXEC [Znode_PublishContentContainerVariantEntity] 1 2,3

	EXEC Znode_PublishContentContainerVariantEntity @@ContainerKey='',@RevisionState='Production',@UserId=2,
	@OldPreviewId=0,@OldProductionId=

	SELECT * FROM ZnodePublishContentContainerVariantEntity

*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	DECLARE @VersionId Int
	SET @VersionId = ( SELECT TOP 1 VersionId FROM ZnodePublishContetContainerVersionEntity WHERE PublishState = @RevisionState )

	IF ISNULL(@VersionId,0) = 0 AND @RevisionState = 'PREVIEW'
	BEGIN
		INSERT INTO ZnodePublishContetContainerVersionEntity(PublishStartTime,PublishState)
		SELECT @GetDate,'PREVIEW'
		SET @VersionId = @@IDENTITY
	END
	
	IF ISNULL(@VersionId,0) = 0 AND @RevisionState = 'PRODUCTION'
 	BEGIN
		INSERT INTO ZnodePublishContetContainerVersionEntity(PublishStartTime,PublishState)
		SELECT @GetDate,'PRODUCTION'
		SET @VersionId = @@IDENTITY
	END 


		DECLARE @SetLocaleId INT, @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1;
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1));
		DECLARE @TBL_ContentContainerVariantEntity TABLE
			(CMSContainerProfileVariantId INT,PortalId INT, LocaleId INT, Name NVARCHAR(100), ContainerKey NVARCHAR(100),	CMSContentContainerId INT, ProfileId INT,
				CMSContainerTemplateId INT, CreatedBy INT, CreatedDate DATETIME, ModifiedBy INT, ModifiedDate DATETIME,IsActive BIT);

		INSERT INTO @TBL_Locale (LocaleId)
		SELECT LocaleId
		FROM ZnodeLocale
		WHERE IsActive =1 --AND (LocaleId = @LocaleId OR @LocaleId = 0);

		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0);

		WHILE @IncrementalId <= @MaxCount
		BEGIN 
			SET @SetLocaleId = (SELECT TOP 1 LocaleId FROM @TBL_locale WHERE  RowId = @IncrementalId);

			;WITH Cte_CMSContainerProfileVariant AS
			(
				SELECT DISTINCT WPV.CMSContainerProfileVariantId, ZCW.ContainerKey, WPV.PortalId, --ZCW.Tags,
					CASE WHEN WPV.PortalId IS NULL THEN  'Any Store' ELSE ZP.StoreName END StoreName  ,
					CASE WHEN WPV.ProfileId IS NULL THEN  'Any User Profile' ELSE ZPR.ProfileName END ProfileName, 
					@GetDate As CreatedDate, @GetDate As ModifiedDate,
					CAST(CASE WHEN WPV.ProfileId IS NULL AND WPV.PortalId IS NULL THEN 1 ELSE 0 END AS BIT) AS IsDefaultVarient,
					CASE WHEN WPV.PortalId IS NULL THEN  'AnyStore' ELSE ZP.StoreCode END StoreCode ,
					CASE WHEN WPV.ProfileId IS NULL THEN  'AnyUserProfile' ELSE ZPR.DefaultExternalAccountNo END ProfileCode,
					CPVL.LocaleId,ZCW.Name,ZCW.CMSContentContainerId,WPV.ProfileId,CPVL.CMSContainerTemplateId,CPVL.IsActive
				FROM ZnodeCMSContentContainer ZCW
				INNER JOIN ZnodeCMSContainerProfileVariant WPV ON WPV.CMSContentContainerId = ZCW.CMSContentContainerId
				INNER JOIN ZnodeCMSContainerProfileVariantLocale CPVL ON WPV.CMSContainerProfileVariantId=CPVL.CMSContainerProfileVariantId
				LEFT JOIN ZnodePortal ZP ON (WPV.PortalId = ZP.PortalId ) 
				LEFT JOIN ZnodeProfile ZPR ON WPV.ProfileId = ZPR.ProfileId
				WHERE (CPVL.LocaleId = @SetLocaleId) -- OR CPVL.LocaleId = @DefaultLocaleId
				--AND CPVL.IsActive = 1 
				AND (ZCW.ContainerKey = @ContainerKey OR @ContainerKey = '' )
				AND ( WPV.CMSContainerProfileVariantId = @ContainerProfileVariantId OR @ContainerProfileVariantId = 0)
			)
			, Cte_GetFirstFilterData AS
			(
				SELECT CMSContainerProfileVariantId,ContainerKey,PortalId,StoreName,ProfileName,CreatedDate,ModifiedDate,IsDefaultVarient,
					StoreCode,ProfileCode,LocaleId,Name,CMSContentContainerId,ProfileId,CMSContainerTemplateId,IsActive
				FROM Cte_CMSContainerProfileVariant
				WHERE LocaleId = @SetLocaleId
			)
			, Cte_GetDefaultFilterData AS
			(
				SELECT CMSContainerProfileVariantId,ContainerKey,PortalId,StoreName,ProfileName,CreatedDate,ModifiedDate,IsDefaultVarient,
					StoreCode,ProfileCode,LocaleId,Name,CMSContentContainerId,ProfileId,CMSContainerTemplateId,IsActive
				FROM Cte_GetFirstFilterData
				UNION ALL
				SELECT CMSContainerProfileVariantId,ContainerKey,PortalId,StoreName,ProfileName,CreatedDate,ModifiedDate,IsDefaultVarient,
					StoreCode,ProfileCode,LocaleId,Name,CMSContentContainerId,ProfileId,CMSContainerTemplateId,IsActive
				FROM Cte_CMSContainerProfileVariant CTEC
				WHERE  LocaleId = @DefaultLocaleId
					AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GetFirstFilterData CTEFD WHERE CTEFD.CMSContainerProfileVariantId = CTEC.CMSContainerProfileVariantId)
			)
			INSERT INTO @TBL_ContentContainerVariantEntity
				(CMSContainerProfileVariantId,PortalId, LocaleId, Name, ContainerKey,	CMSContentContainerId, ProfileId, CMSContainerTemplateId,
					CreatedBy, CreatedDate, ModifiedBy,	ModifiedDate,IsActive)
			SELECT CMSContainerProfileVariantId,PortalId, LocaleId, Name, ContainerKey,	CMSContentContainerId, ProfileId, CMSContainerTemplateId,
					@UserId, CreatedDate, @UserId,	ModifiedDate,IsActive
			FROM Cte_GetDefaultFilterData

			SET @IncrementalId = @IncrementalId +1;
		END 

		CREATE TABLE #ContentContainerGlobalAttribute(CMSContainerProfileVariantId INT,LocaleId INT, GlobalAttributes NVARCHAR(MAX))

		INSERT INTO #ContentContainerGlobalAttribute (CMSContainerProfileVariantId ,LocaleId , GlobalAttributes )
		EXEC [Znode_SetPublishContentContainerGlobalAttributeEntity] @ContainerKey = @ContainerKey, @ContainerProfileVariantId = @ContainerProfileVariantId
		-- Data inserted INTO flat table ZnodeBlogNewsEntity (Replica of MongoDB Collection )
		IF (@RevisionState like '%Preview%' ) 
		BEGIN
			--Data inserted INTO flat table ZnodePublishContentContainerVariantEntity (Replica of MongoDB Collection )
			DELETE FROM ZnodePublishContentContainerVariantEntity WHERE VersionId = @VersionId
			AND EXISTS(SELECT * FROM @TBL_ContentContainerVariantEntity cc WHERE ZnodePublishContentContainerVariantEntity.ContainerKey = CC.ContainerKey
						AND ZnodePublishContentContainerVariantEntity.CMSContainerProfileVariantId = CC.CMSContainerProfileVariantId)
			OR NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariant cc WHERE ZnodePublishContentContainerVariantEntity.CMSContainerProfileVariantId = CC.CMSContainerProfileVariantId)

			INSERT INTO ZnodePublishContentContainerVariantEntity
			(
				VersionId,PortalId,LocaleId,Name,ContainerKey,CMSContentContainerId,ProfileId,CMSContainerTemplateId,
				CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CMSContainerProfileVariantId, GlobalAttributes,IsActive
			)
			SELECT @VersionId,A.PortalId,A.LocaleId,Name,ContainerKey,CMSContentContainerId,ProfileId,CMSContainerTemplateId,
				CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,A.CMSContainerProfileVariantId, B.GlobalAttributes,A.IsActive
			FROM @TBL_ContentContainerVariantEntity A
			LEFT JOIN #ContentContainerGlobalAttribute B ON A.CMSContainerProfileVariantId = B.CMSContainerProfileVariantId AND A.LocaleId = B.LocaleId

			DELETE FROM ZnodePublishContentContainerVariantEntity WHERE VersionId = @OldPreviewId

		END
		---------------------------- END Preview 
		IF (@RevisionState LIKE '%Production%' OR @RevisionState = 'None')
		BEGIN
			-- Only production version id will process 
			DELETE FROM ZnodePublishContentContainerVariantEntity WHERE VersionId = @VersionId
			AND EXISTS(SELECT * FROM @TBL_ContentContainerVariantEntity cc WHERE ZnodePublishContentContainerVariantEntity.ContainerKey = CC.ContainerKey
					AND ZnodePublishContentContainerVariantEntity.CMSContainerProfileVariantId = CC.CMSContainerProfileVariantId)
			OR NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariant cc WHERE ZnodePublishContentContainerVariantEntity.CMSContainerProfileVariantId = CC.CMSContainerProfileVariantId)

			INSERT INTO ZnodePublishContentContainerVariantEntity
			(
				VersionId,PortalId,LocaleId,Name,ContainerKey,CMSContentContainerId,ProfileId,CMSContainerTemplateId,
				CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CMSContainerProfileVariantId,GlobalAttributes,IsActive
			)
			SELECT DISTINCT @VersionId,A.PortalId,A.LocaleId,Name,ContainerKey,CMSContentContainerId,ProfileId,CMSContainerTemplateId,
				CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,A.CMSContainerProfileVariantId,B.GlobalAttributes,A.IsActive
			FROM @TBL_ContentContainerVariantEntity A
			LEFT JOIN #ContentContainerGlobalAttribute B ON A.CMSContainerProfileVariantId = B.CMSContainerProfileVariantId AND A.LocaleId = B.LocaleId

			DELETE FROM ZnodePublishContentContainerVariantEntity WHERE VersionId = @OldProductionId
		
		END

		SET @Status =1 ;
		IF (@RevisionState = 'Preview')
			UPDATE B SET PublishStateId = (SELECT dbo.Fn_GetPublishStateIdForPreview()) --, ISPublish = 1 
			FROM @TBL_ContentContainerVariantEntity A
			INNER JOIN ZnodeCMSContainerProfileVariant B ON A.CMSContainerProfileVariantId = B.CMSContainerProfileVariantId
				
		ELSE IF (@RevisionState = 'Production' Or @RevisionState = 'None' )
			UPDATE B SET PublishStateId = (SELECT dbo.Fn_GetPublishStateIdForPublish()) --, ISPublish = 1 
			FROM @TBL_ContentContainerVariantEntity A
			INNER JOIN ZnodeCMSContainerProfileVariant B ON A.CMSContainerProfileVariantId = B.CMSContainerProfileVariantId
				

		SELECT 1 ID, @Status Status;
END TRY 
BEGIN CATCH
	SET @Status =0

	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishContentContainerVariantEntity
		@ContainerKey = '+CAST(@ContainerKey AS VARCHAR	(max))
		+',@ContainerProfileVariantId = ''' + CAST(@ContainerProfileVariantId AS VARCHAR(50))
		+',@RevisionState = ''' + CAST(@RevisionState AS VARCHAR(50))
		+',@UserId = ' + CAST(@UserId AS VARCHAR(20))
		+',@Status='+CAST(@Status AS VARCHAR(10));
		SELECT 0 AS ID,@Status AS Status;
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_PublishContentContainerVariantEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END