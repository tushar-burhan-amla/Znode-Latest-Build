CREATE  PROCEDURE [dbo].[Znode_SetPublishContainerWidgetEntity]
(
   @PortalId  INT = 0 
  ,@PreviewVersionId INT = 0 
  ,@IsPreviewEnable int = 0 
  ,@ProductionVersionId INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@CMSMappingId INT =0
  ,@UserId int = 0 
  ,@Status int = 0 Output
)
AS
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
   Begin 
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	    DECLARE @Tbl_PreviewVersionId    TABLE    (PreviewVersionId int , PortalId int , LocaleId int)
		DECLARE @Tbl_ProductionVersionId TABLE    (ProductionVersionId int  , PortalId int , LocaleId int)

		If @PreviewVersionId = 0 
			Begin
   				Insert into @Tbl_PreviewVersionId 
				SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and PublishState ='PREVIEW'
			end
		Else 
			Begin
				Insert into @Tbl_PreviewVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
				where VersionId = @PreviewVersionId
			End
		If @ProductionVersionId = 0 
   			Begin
				Insert into @Tbl_ProductionVersionId 
				SELECT distinct VersionId , PortalId , LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and PublishState ='PRODUCTION'
			End 
		Else 
			Begin
				Insert into @Tbl_ProductionVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
			    where VersionId = @ProductionVersionId
			End	
 

		DECLARE @CMSWidgetDataFinal TABLE (CMSContainerConfigurationId INT ,CMSWidgetsId INT ,WidgetsKey  NVARCHAR(256) ,CMSMappingId  INT ,TypeOFMapping NVARCHAR(100) , ContentContainerId INT, ContainerKey  NVARCHAR(100));
		if @CMSMappingId > 0 
			Begin
				INSERT INTO @CMSWidgetDataFinal
					(CMSContainerConfigurationId, CMSWidgetsId, WidgetsKey, CMSMappingId, TypeOFMapping, ContentContainerId, ContainerKey)
                     SELECT CMSContainerConfigurationId , CMSWidgetsId , WidgetKey , CMSMappingId , TypeOFMapping , ContentContainerId, a.ContainerKey
				     FROM ZnodeCMSContainerConfiguration AS a
					 inner join ZnodeCMSContentContainer ZCC on a.ContainerKey = ZCC.ContainerKey
                     WHERE (a.TypeOFMapping = 'ContentPageMapping'
                     AND ( EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSContentPages  WHERE IsActive =1 and  a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  )))
					 AND (a.CMSMappingId = @CMSMappingId OR @CMSMappingId = 0  )
			End
		Else if @CMSMappingId = 0 
			Begin
				 INSERT INTO @CMSWidgetDataFinal
					(CMSContainerConfigurationId, CMSWidgetsId, WidgetsKey, CMSMappingId, TypeOFMapping, ContentContainerId, ContainerKey)
                     SELECT CMSContainerConfigurationId , CMSWidgetsId , WidgetKey , CMSMappingId , TypeOFMapping , ContentContainerId, a.ContainerKey
				     FROM ZnodeCMSContainerConfiguration AS a
					 inner join ZnodeCMSContentContainer ZCC on a.ContainerKey = ZCC.ContainerKey
                     WHERE (a.TypeOFMapping = 'ContentPageMapping'
                     AND ( EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSContentPages  WHERE IsActive =1 and  a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  ))
                     OR (a.TypeOFMapping = 'PortalMapping' AND a.CMSMappingId = @PortalId ))
			End
		End 
    --select * from @TBL_ContentPage 
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%' )
	Begin
	    --Data inserted into flat table ZnodePublishMediaWidgetEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishContainerWidgetEntity where PortalId = @PortalId  and VersionId in (Select PreviewVersionId from  @TBL_PreviewVersionId ) and
		(MappingId = @CMSMappingId OR @CMSMappingId = 0 )

		Insert Into ZnodePublishContainerWidgetEntity 
		(VersionId,PublishStartTime,ContainerConfigurationId,MappingId,PortalId,TypeOFMapping,LocaleId,WidgetsKey,ContainerKey)
		SELECT PreviewVersionId , @GetDate ,CMSContainerConfigurationId,CMSMappingId,@PortalId,TypeOFMapping,LocaleId,WidgetsKey,ContainerKey
		FROM @CMSWidgetDataFinal A Inner join @TBL_PreviewVersionId B On B.PortalId = @PortalId
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishContainerWidgetEntity where PortalId = @PortalId  and VersionId in (Select ProductionVersionId from  @TBL_ProductionVersionId) and 
		(MappingId = @CMSMappingId OR @CMSMappingId = 0 )
		
		Insert Into ZnodePublishContainerWidgetEntity 
		(VersionId,PublishStartTime,ContainerConfigurationId,MappingId,PortalId,TypeOFMapping,LocaleId,WidgetsKey,ContainerKey)
		SELECT ProductionVersionId , @GetDate ,CMSContainerConfigurationId,CMSMappingId,@PortalId,TypeOFMapping,LocaleId,WidgetsKey,ContainerKey
		FROM @CMSWidgetDataFinal Inner join @TBL_ProductionVersionId B On B.PortalId = @PortalId
	End
	SET @Status = 1;

END TRY 
BEGIN CATCH 
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_SetPublishContainerWidgetEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+''',@CMSMappingId= ' + CAST(@CMSMappingId  AS varchar(20))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		                			 
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_SetPublishContainerWidgetEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END