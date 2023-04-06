CREATE PROCEDURE [dbo].[Znode_SetPublishWidgetBrandEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
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
  
	EXEC ZnodeSetPublishWidgetBrandEntity 1 2,3
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
	
	 Exec [ZnodeSetPublishWidgetBrandEntity]
   @PortalId  = 1 
  ,@LocaleId  = 0 
  ,@PreviewVersionId = 0 
  ,@ProductionVersionId = 0 
  ,@RevisionState = 'Preview&Production' 
  ,@CMSMappingId  =88 
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
 
 		DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))
		DECLARE @TBL_CMSContentPagesPortalWise TABLE (CMSContentPagesId int , PortalId int )
		
		INSERT INTO @TBL_CMSContentPagesPortalWise(CMSContentPagesId, PortalId)	SELECT distinct CMSContentPagesId , PortalId FROM ZnodeCMSContentPages
		WHERE PortalId = @PortalId	AND IsActive = 1 

		DECLARE @CMSWidgetData TABLE (BrandId int ,MappingId int ,PortalId int ,WidgetsKey varchar(300),TypeOfMapping varchar(300),DisplayOrder int );
		
		Insert Into @CMSWidgetData (BrandId,WidgetsKey,MappingId,TypeOFMapping,DisplayOrder, PortalId)
						SELECT BrandId,WidgetsKey,CMSMappingId,TypeOFMapping,DisplayOrder,
						
						CASE WHEN a.TypeOFMapping = 'ContentPageMapping' THEN 
						(Select Portalid from @TBL_CMSContentPagesPortalWise X where  X.CMSContentPagesId = a.CMSMappingId)
						Else a.CMSMappingId   End PortalId 

						FROM ZnodeCMSWidgetBrand AS a WHERE 
						(
							(a.TypeOFMapping = 'ContentPageMapping'	AND (EXISTS ( SELECT TOP 1 1 FROM @TBL_CMSContentPagesPortalWise  WHERE a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  )))
							OR 
							(a.TypeOFMapping = 'PortalMapping' AND a.CMSMappingId  = @PortalId  )
						)

						AND (a.CMSMappingId = @CMSMappingId OR @CMSMappingId = 0 )
	End

    --select * from @TBL_ContentPage 
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) 
	Begin
	    --Data inserted into flat table ZnodePublishWidgetBrandEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishWidgetBrandEntity where PortalId = @PortalId  and VersionId in (Select PreviewVersionId from  @TBL_PreviewVersionId)
		
		Insert Into ZnodePublishWidgetBrandEntity 
		(VersionId,PublishStartDate,BrandId,MappingId,PortalId,WidgetsKey,TypeOfMapping,DisplayOrder)
		SELECT B.PreviewVersionId , @GetDate, A.BrandId,A.MappingId,A.PortalId, A.WidgetsKey,A.TypeOFMapping,A.DisplayOrder
		FROM @CMSWidgetData A inner join @Tbl_PreviewVersionId B on A.PortalId  = B.PortalId 
		
		
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishWidgetBrandEntity where PortalId = @PortalId  and VersionId in (Select ProductionVersionId from  @TBL_ProductionVersionId)
		
		Insert Into ZnodePublishWidgetBrandEntity 
		(VersionId,PublishStartDate,BrandId,MappingId,PortalId,WidgetsKey,TypeOfMapping,DisplayOrder)
		SELECT B.ProductionVersionId , @GetDate, A.BrandId,A.MappingId,A.PortalId, A.WidgetsKey,A.TypeOFMapping,A.DisplayOrder
		FROM @CMSWidgetData A inner join @Tbl_ProductionVersionId B on A.PortalId  = B.PortalId 
		
			
	End
	SET @Status = 1 
END TRY 
BEGIN CATCH 
	SET @Status = 0  
 
	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishWidgetBrandEntity 
			@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
			+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
			+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
			+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
			+''',@CMSMappingId= ' + CAST(@CMSMappingId  AS varchar(20))
			+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		                			 

         			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishWidgetBrandEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END