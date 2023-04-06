CREATE PROCEDURE [dbo].[Znode_SetPublishPortalCustomCssEntity]
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
  
	EXEC [ZnodeSetPortalCustomCssEntity] 1 2,3

*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
   Begin 
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
		
		DECLARE @TBL_CustomeCSSEntity TABLE (PortalId int,DynamicStyle	nvarchar(max),PublishState	varchar(50), LocaleId	int	)
		
		DECLARE @WebStoreEntityId int 
		
		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )
		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN 
			SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
			Insert into @TBL_CustomeCSSEntity 
			(
				PortalId,DynamicStyle,LocaleId
			)
			Select  PortalId,DynamicCssStyle,@SetLocaleId FROM ZnodePortalCustomCss
			Where PortalId  = @PortalId and IsActive =1 
			
		SET @IncrementalId = @IncrementalId +1 
		END 
		-- Data inserted into flat table ZnodeWebStoreEntity (Replica of MongoDB Collection )  
		
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
	Begin
	    --Data inserted into flat table ZnodePublishWidgetSliderBannerEntity (Replica of MongoDB Collection )  

		Delete from ZnodePublishPortalCustomCssEntity where VersionId in (Select PreviewVersionId  from  @TBL_PreviewVersionId ) AND PortalId = @PortalId
	 		 
		Insert Into ZnodePublishPortalCustomCssEntity 
		(
			VersionId,PublishStartTime,PortalId,DynamicStyle,PublishState,LocaleId
		)
		SELECT B.PreviewVersionId  , @GetDate,A.PortalId,DynamicStyle,'PREVIEW',A.LocaleId  FROM @TBL_CustomeCSSEntity A Inner Join
		@TBL_PreviewVersionId B on A.PortalId = B.PortalId  and A.LocaleId = B.LocaleId 
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishPortalCustomCssEntity where VersionId = @ProductionVersionId  AND PortalId = @PortalId
		
		Insert Into ZnodePublishPortalCustomCssEntity 
		(
			VersionId,PublishStartTime,PortalId,DynamicStyle,PublishState,LocaleId
		)
		SELECT B.ProductionVersionId  , @GetDate,A.PortalId,DynamicStyle,'PRODUCTION',A.LocaleId   FROM @TBL_CustomeCSSEntity 
		A Inner join @Tbl_ProductionVersionId   B On A.PortalId = B.PortalId and A.LocaleId  = B.LocaleId 


	End
	SET @Status =1 ;


	End
END TRY 
BEGIN CATCH 
	SET @Status =0  

	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishPortalCustomCssEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  	
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishPortalCustomCssEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END