
CREATE  PROCEDURE [dbo].[Znode_SetPublishMediaWidgetEntity]
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
/*
    This Procedure is used to publish the blog news against the store 
  
	EXEC ZnodeSetPublishMediaWidgetEntity 1 2,3
	A. 
		1. Preview - Preview
		2. None    - Production   --- 
		3. Production - Preview/Production
	B.
		select * from ZnodePublishStateApplicationTypeMapping
		select * from ZnodePublishState where PublishStateId in (3,4) 
		select * from ZnodePublishPortalLog 
	C.
		Select * from ZnodePublishState where IsDefaultContentState = 1  and IsContentState = 1  --Production 
    
	Unit testing 
	
		 Exec [ZnodeSetPublishMediaWidgetEntity]
   @PortalId  = 1 
  ,@PreviewVersionId = 0 
  ,@ProductionVersionId = 0 
  ,@RevisionState = 'Preview&Production' 
  ,@CMSMappingId  =0 
  ,@UserId = 0 


	
	*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
   Begin 
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
	    DECLARE @Tbl_PreviewVersionId    TABLE    (PreviewVersionId int , PortalId int , LocaleId int)
		DECLARE @Tbl_ProductionVersionId TABLE    (ProductionVersionId int  , PortalId int , LocaleId int)

		If @PreviewVersionId = 0 
			Begin
   				Insert into @Tbl_PreviewVersionId 
				SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and PublishState ='PREVIEW'
			end
		Else 
				Insert into @Tbl_PreviewVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
				where VersionId = @PreviewVersionId
		If @ProductionVersionId = 0 
   			Begin
				Insert into @Tbl_ProductionVersionId 
				SELECT distinct VersionId , PortalId , LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and PublishState ='PRODUCTION'
			End 
		Else 
			Insert into @Tbl_ProductionVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
			where VersionId = @ProductionVersionId
 

		DECLARE @CMSWidgetDataFinal TABLE (CMSMediaConfigurationId INT ,CMSWidgetsId INT ,WidgetsKey  NVARCHAR(256) ,CMSMappingId  INT ,TypeOFMapping NVARCHAR(100) ,[MediaPath]  NVARCHAR(1000));
		if @CMSMappingId > 0 
				INSERT INTO @CMSWidgetDataFinal
					( CMSMediaConfigurationId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping ,MediaPath)
                     SELECT CMSMediaConfigurationId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , ZM.Path as MediaPath
                     FROM ZnodeCMSMediaConfiguration AS a
					 inner join ZnodeMedia ZM on a.MediaId = ZM.MediaId
                     WHERE (a.TypeOFMapping = 'ContentPageMapping'
                     AND ( EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSContentPages  WHERE IsActive =1 and  a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  )))
					 AND (a.CMSMappingId = @CMSMappingId OR @CMSMappingId = 0  )

		Else if @CMSMappingId = 0 
				 INSERT INTO @CMSWidgetDataFinal
					( CMSMediaConfigurationId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping ,MediaPath)
                     SELECT CMSMediaConfigurationId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , ZM.Path as MediaPath
                     FROM ZnodeCMSMediaConfiguration AS a
					 inner join ZnodeMedia ZM on a.MediaId = ZM.MediaId
                     WHERE (a.TypeOFMapping = 'ContentPageMapping'
                     AND ( EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSContentPages  WHERE IsActive =1 and  a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  ))
                     OR (a.TypeOFMapping = 'PortalMapping' AND a.CMSMappingId = @PortalId ))
		End 
    --select * from @TBL_ContentPage 
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%' )
	Begin
	    --Data inserted into flat table ZnodePublishMediaWidgetEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishMediaWidgetEntity where PortalId = @PortalId  and VersionId in (Select PreviewVersionId from  @TBL_PreviewVersionId ) and
		(MappingId = @CMSMappingId OR @CMSMappingId = 0 )

		Insert Into ZnodePublishMediaWidgetEntity 
		(VersionId,PublishStartTime,MediaWidgetConfigurationId,MappingId,PortalId,TypeOFMapping,MediaPath,WidgetsKey)
		SELECT PreviewVersionId , @GetDate ,CMSMediaConfigurationId,CMSMappingId,@PortalId,TypeOFMapping,MediaPath,WidgetsKey
		FROM @CMSWidgetDataFinal A Inner join @TBL_PreviewVersionId B On B.PortalId = @PortalId
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishMediaWidgetEntity where PortalId = @PortalId  and VersionId in (Select ProductionVersionId from  @TBL_ProductionVersionId) and 
		(MappingId = @CMSMappingId OR @CMSMappingId = 0 )
		
		Insert Into ZnodePublishMediaWidgetEntity 
		(
			VersionId,PublishStartTime,MediaWidgetConfigurationId,MappingId,PortalId,TypeOFMapping,MediaPath,WidgetsKey
		)
		SELECT ProductionVersionId , @GetDate ,CMSMediaConfigurationId,CMSMappingId,@PortalId,TypeOFMapping,MediaPath,WidgetsKey
		FROM @CMSWidgetDataFinal Inner join @TBL_ProductionVersionId B On B.PortalId = @PortalId
	End
	SET @Status = 1;

END TRY 
BEGIN CATCH 
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishMediaWidgetEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+''',@CMSMappingId= ' + CAST(@CMSMappingId  AS varchar(20))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		                			 
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishMediaWidgetEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END