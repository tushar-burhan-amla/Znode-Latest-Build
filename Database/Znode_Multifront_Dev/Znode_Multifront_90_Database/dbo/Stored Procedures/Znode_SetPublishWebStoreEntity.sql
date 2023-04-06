CREATE  PROCEDURE [dbo].[Znode_SetPublishWebStoreEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@IsPreviewEnable int = 0 
  ,@PreviewVersionId varchar(1000)= '' 
  ,@ProductionVersionId varchar(1000) = '' 
  ,@RevisionState varchar(50) = '' 
  ,@UserId int = 0 
  ,@Status int = 0 OUTPUT 

)
AS
/*
    This Procedure is used to publish the blog news against the store 
  
	EXEC [ZnodeSetPublishWebStoreEntity] 1 2,3

*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
   Begin 
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))
		
		DECLARE @TBL_StoreEntity TABLE 
		(
			 Id int Identity, PortalThemeId	int,PortalId	int,ThemeId	int,ThemeName	varchar(200),CSSId	int,CSSName	nvarchar(2000),
			 WebsiteLogo	varchar(300),WebsiteTitle	nvarchar(400),FaviconImage	varchar(300),WebsiteDescription	nvarchar(MAX),
			 PublishState	varchar(100),LocaleId	int	
		)
		
		DECLARE @WebStoreEntityId int 
		
		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )
		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN 
			SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)

		Insert into @TBL_StoreEntity 
		(
			 PortalThemeId,PortalId,ThemeId,ThemeName,CSSId,CSSName	,
			 WebsiteLogo,WebsiteTitle,FaviconImage,WebsiteDescription,
			 LocaleId
		)
				select CPT.CMSPortalThemeId,	CPT.PortalId , CPT.CMSThemeId  ThemeId , CT.Name ThemeName,ZCTC.CMSThemeCSSId CSSId,ZCTC.CSSName CSSName ,
				ZM.Path WebsiteLogo ,CPT.WebsiteTitle,FavZM.Path FaviconImage,CPT.WebsiteDescription,@SetLocaleId
				from ZnodeCMSPortalTheme CPT Inner join ZnodeCMSTheme CT ON CPT.CMSThemeId = CT.CMSThemeId 
				Inner join ZnodeCMSThemeCSS ZCTC On CPT.CMSThemeCSSId = ZCTC.CMSThemeCSSId 
				Left outer join ZnodeMedia ZM ON CPT.MediaId = ZM.MediaId
				Left outer join ZnodeMedia FavZM ON CPT.FavIconId = FavZM .MediaId
				Where CPT.PortalId  = @PortalId 
			
			SET @IncrementalId = @IncrementalId +1 
		END 
		-- Data inserted into flat table ZnodeWebStoreEntity (Replica of MongoDB Collection )  
		
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
	Begin
	    --Data inserted into flat table ZnodePublishWidgetSliderBannerEntity (Replica of MongoDB Collection )  

		Delete from ZnodePublishWebstoreEntity where VersionId in  (select Item from DBO.Split(@PreviewVersionId,',') )AND PortalId = @PortalId
	 		 
			 

		Insert Into ZnodePublishWebstoreEntity 
		(
			VersionId,PublishStartTime,PortalThemeId,PortalId,ThemeId,ThemeName,
			CSSId,CSSName,WebsiteLogo,WebsiteTitle,FaviconImage,WebsiteDescription,PublishState,LocaleId
		)
		SELECT B.Item  , @GetDate,PortalThemeId,PortalId,ThemeId,ThemeName,
			CSSId,CSSName,WebsiteLogo,WebsiteTitle,FaviconImage,WebsiteDescription,'PREVIEW',LocaleId FROM 
			@TBL_StoreEntity A Inner JOIN DBO.Split(@PreviewVersionId,',') B 
			On a.ID = b.Id 
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishWebstoreEntity where VersionId in (select Item from DBO.Split(@ProductionVersionId ,',') )   AND PortalId = @PortalId
		
		Insert Into ZnodePublishWebstoreEntity 
		(
			VersionId,PublishStartTime,PortalThemeId,PortalId,ThemeId,ThemeName,
			CSSId,CSSName,WebsiteLogo,WebsiteTitle,FaviconImage,WebsiteDescription,PublishState,LocaleId
		)
		SELECT B.Item  , @GetDate,PortalThemeId,PortalId,ThemeId,ThemeName,
			CSSId,CSSName,WebsiteLogo,WebsiteTitle,FaviconImage,WebsiteDescription,'PRODUCTION',LocaleId FROM 
			@TBL_StoreEntity A Inner JOIN DBO.Split(@ProductionVersionId,',') B 
			On a.ID = b.Id 


	End
	SET @Status =1 ;


	End
END TRY 
BEGIN CATCH 
	SET @Status =0  

	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishWebStoreEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  	
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishWebStoreEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END