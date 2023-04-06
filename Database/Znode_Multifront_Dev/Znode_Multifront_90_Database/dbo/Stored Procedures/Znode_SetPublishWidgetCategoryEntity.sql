CREATE  PROCEDURE [dbo].[Znode_SetPublishWidgetCategoryEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@VersionId INT = 0 
  ,@PreviewVersionId INT = 0 
  ,@IsPreviewEnable int = 0 
  ,@ProductionVersionId INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@UserId int = 0 
  ,@Status int = 0 OUTPUT 
)
AS
/*
    This Procedure is used to publish the blog news against the store 
  
	EXEC [ZnodeSetWidgetCategoryEntity] 1 2,3

*/
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
				SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and  (LocaleId = 	@LocaleId OR @LocaleId = 0  ) and PublishState ='PREVIEW'
			end
		Else 
				Insert into @Tbl_PreviewVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
				where VersionId = @PreviewVersionId
		If @ProductionVersionId = 0 
   			Begin
				Insert into @Tbl_ProductionVersionId 
				SELECT distinct VersionId , PortalId , LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and  (LocaleId = 	@LocaleId OR @LocaleId = 0  ) and PublishState ='PRODUCTION'
			End 
		Else 
			Insert into @Tbl_ProductionVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
			where VersionId = @ProductionVersionId
 
		DECLARE @TBL_WidgetCategoryEntity TABLE (WidgetCategoryId	int,ZnodeCategoryId	int,MappingId	int,PortalId	int
			   ,WidgetsKey	nvarchar(500),TypeOFMapping	varchar(100),DisplayOrder	int,CategoryCode	varchar(600))
		
		Declare @TBL_CMSContentPagesPortalWise TABLE (CMSContentPagesId int )
		INSERT INTO @TBL_CMSContentPagesPortalWise(CMSContentPagesId)	SELECT CMSContentPagesId FROM ZnodeCMSContentPages
			WHERE PortalId = @PortalId	AND IsActive = 1 

			Insert into @TBL_WidgetCategoryEntity 
			(
				WidgetCategoryId,ZnodeCategoryId,MappingId,PortalId,WidgetsKey,TypeOFMapping,DisplayOrder,CategoryCode
			)
			 Select  CMSWidgetCategoryId,Isnull(PublishCategoryId,0),CMSMappingId,@PortalId,WidgetsKey,TypeOFMapping,DisplayOrder,CategoryCode
			 FROM ZnodeCMSWidgetCategory
			 WHERE
			((TypeOfMapping = 'PortalMapping'	AND (CMSMappingId = @PortalId OR @PortalId = 0 ))	OR 
			(TypeOfMapping = 'ContentPageMapping' AND CMSMappingId IN  (SELECT CMSContentPagesId	FROM @TBL_CMSContentPagesPortalWise)))
			
		-- Data inserted into flat table ZnodeWebStoreEntity (Replica of MongoDB Collection )  
		
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
	Begin
	    --Data inserted into flat table ZnodePublishWidgetSliderBannerEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishWidgetCategoryEntity where VersionId in (Select  PreviewVersionId from @Tbl_PreviewVersionId)  AND PortalId = @PortalId
	 		 
		Insert Into ZnodePublishWidgetCategoryEntity 
		(
			VersionId,PublishStartTime,WidgetCategoryId,ZnodeCategoryId,MappingId,PortalId,WidgetsKey,
			TypeOFMapping,DisplayOrder,CategoryCode
		)
		SELECT B.PreviewVersionId  , @GetDate,WidgetCategoryId,ZnodeCategoryId,MappingId,A.PortalId,WidgetsKey,TypeOFMapping,DisplayOrder,CategoryCode   FROM @TBL_WidgetCategoryEntity A
		Inner join @Tbl_PreviewVersionId B on A.PortalId = B.PortalId

	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishWidgetCategoryEntity where VersionId in( Select ProductionVersionId from @TBL_ProductionVersionId ) AND PortalId = @PortalId
		
		Insert Into ZnodePublishWidgetCategoryEntity 
		(
			VersionId,PublishStartTime,WidgetCategoryId,ZnodeCategoryId,MappingId,PortalId,WidgetsKey,
			TypeOFMapping,DisplayOrder,CategoryCode
		)
		SELECT B.ProductionVersionId  , @GetDate,WidgetCategoryId,ZnodeCategoryId,MappingId,A.PortalId,WidgetsKey,TypeOFMapping,DisplayOrder,CategoryCode  FROM @TBL_WidgetCategoryEntity A
		Inner join @Tbl_ProductionVersionId B on A.PortalId = B.PortalId

	End
	SET @Status =1 ;


	End
END TRY 
BEGIN CATCH 
	SET @Status =0  

	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishWidgetCategoryEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  	
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishWidgetCategoryEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END