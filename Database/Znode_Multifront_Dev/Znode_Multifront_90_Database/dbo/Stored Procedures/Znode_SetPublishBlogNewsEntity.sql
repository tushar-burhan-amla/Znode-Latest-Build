CREATE PROCEDURE [dbo].[Znode_SetPublishBlogNewsEntity]
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
  ,@BlogNewsCode Varchar(300) = ''
  ,@BlogNewsType Varchar(300) = ''
)
AS
/*
    This Procedure is used to publish the blog news against the store 
  
	EXEC ZnodePublishBlogNews 1 2,3

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
		DECLARE @TBL_BlogData TABLE (BlogNewsId INT,PortalId INT ,MediaId INT ,BlogNewsType NVARCHAR(max),IsBlogNewsActive BIT ,IsAllowGuestComment BIT,LocaleId INT ,BlogNewsTitle NVARCHAR(max),CMSContentPagesId INT
		,BodyOverview NVARCHAR(max),Tags NVARCHAR(max),BlogNewsContent NVARCHAR(max),CreatedDate DATETIME,ActivationDate DATETIME ,ExpirationDate DATETIME, MediaPath varchar(max),BlogNewsCode NVARCHAR(4000) )
		DECLARE @BlogNewsEntityId int 
		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )
		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN 
			SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)

			;With Cte_GetCmsBlogNewsData AS 
			(
				SELECT ZBN.BlogNewsId,PortalId,ZBN.MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,ZBNL.LocaleId
				,BlogNewsTitle,ZBN.CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,ZBN.CreatedDate,ActivationDate,ExpirationDate,
				zm.Path MediaPath,ZBN.BlogNewsCode
				FROM ZnodeBlogNews ZBN 
				INNER JOIN ZnodeBlogNewsLocale ZBNL ON (ZBNL.BlogNewsId = ZBN.BlogNewsId)
				LEFT JOIN ZnodeBlogNewsContent ZBNC ON ( ZBNC.BlogNewsId = ZBN.BlogNewsId AND ZBNC.LocaleId = ZBNL.LocaleId) 
				left join ZnodeMedia ZM on(ZM.MediaId = ZBN.MediaId)
				WHERE (ZBNL.LocaleId = @SetLocaleId OR ZBNL.LocaleId = @DefaultLocaleId)  
				AND (ZBN.PortalId = @PortalId  OR @PortalId = 0 ) 
				AND (ZBN.BlogNewsCode =  @BlogNewsCode OR @BlogNewsCode = '')
				AND (ZBN.BlogNewsType =  @BlogNewsType OR @BlogNewsType = '')
				AND ZBN.IsBlogNewsActive = 1 
			)
			, Cte_GetFirstFilterData AS
			(
				SELECT BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,LocaleId
				,BlogNewsTitle,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode
				FROM Cte_GetCmsBlogNewsData 
				WHERE LocaleId = @SetLocaleId
			)
			, Cte_GetDefaultFilterData AS
			(
				SELECT BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,LocaleId
				,BlogNewsTitle,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode
				FROM  Cte_GetFirstFilterData 
				UNION ALL 
				SELECT BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,LocaleId
				,BlogNewsTitle,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode
				FROM Cte_GetCmsBlogNewsData CTEC 
				WHERE LocaleId = @DefaultLocaleId 
				AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GetFirstFilterData CTEFD WHERE CTEFD.BlogNewsId = CTEC.BlogNewsId )
			)
	
			INSERT INTO @TBL_BlogData (BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,LocaleId,BlogNewsTitle
			,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode)
			SELECT BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,@SetLocaleId,BlogNewsTitle
			,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode
			FROM Cte_GetDefaultFilterData

			SET @IncrementalId = @IncrementalId +1 
		END 
		-- Data inserted into flat table ZnodeBlogNewsEntity (Replica of MongoDB Collection )  
		
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
	Begin
	    --Data inserted into flat table ZnodePublishWidgetSliderBannerEntity (Replica of MongoDB Collection )  

		Delete from ZnodePublishBlogNewsEntity where  VersionId in (Select PreviewVersionId from @Tbl_PreviewVersionId )
		AND (PortalId = @PortalId OR @PortalId = 0 ) 
	 	AND (BlogNewsCode =  @BlogNewsCode OR @BlogNewsCode = '')
		AND (BlogNewsType =  @BlogNewsType OR @BlogNewsType = '')	 
		

		Insert Into ZnodePublishBlogNewsEntity
		(
			VersionId,PublishStartTime,BlogNewsId,PortalId,MediaId,CMSContentPagesId,LocaleId,BlogNewsType,BlogNewsTitle,
			BodyOverview,Tags,BlogNewsContent,MediaPath,IsBlogNewsActive,IsAllowGuestComment,ActivationDate,ExpirationDate,
			CreatedDate,BlogNewsCode
		)
		SELECT PreviewVersionId, @GetDate, BlogNewsId,A.PortalId,MediaId,CMSContentPagesId,B.LocaleId,BlogNewsType,BlogNewsTitle,
			BodyOverview, Tags,BlogNewsContent,MediaPath,IsBlogNewsActive,IsAllowGuestComment,ActivationDate,ExpirationDate,
			CreatedDate,BlogNewsCode FROM @TBL_BlogData A Inner join  @Tbl_PreviewVersionId B ON A.LocaleId = B.LocaleId 
			AND A.PortalId = b.PortalId
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishBlogNewsEntity where VersionId in 
		(Select ProductionVersionId from @Tbl_ProductionVersionId )
		AND (PortalId = @PortalId OR @PortalId = 0 ) 
  	 	AND (BlogNewsCode =  @BlogNewsCode OR @BlogNewsCode = '')	 
		AND (BlogNewsType =  @BlogNewsType OR @BlogNewsType = '')
		
		Insert Into ZnodePublishBlogNewsEntity
		(
			VersionId,PublishStartTime,BlogNewsId,PortalId,MediaId,CMSContentPagesId,LocaleId,BlogNewsType,BlogNewsTitle,
			BodyOverview,Tags,BlogNewsContent,MediaPath,IsBlogNewsActive,IsAllowGuestComment,ActivationDate,ExpirationDate,
			CreatedDate,BlogNewsCode
		)
		SELECT ProductionVersionId  , @GetDate,  BlogNewsId,A.PortalId,MediaId,CMSContentPagesId,B.LocaleId,BlogNewsType,BlogNewsTitle,
			BodyOverview, Tags,BlogNewsContent,MediaPath,IsBlogNewsActive,IsAllowGuestComment,ActivationDate,ExpirationDate,
			CreatedDate,BlogNewsCode 
			FROM @TBL_BlogData A Inner join  @Tbl_ProductionVersionId B ON A.LocaleId = B.LocaleId 
			AND A.PortalId = b.PortalId

	End
	SET @Status =1 ;
	If (@RevisionState = 'Preview'  )
		Update B SET PublishStateId = (select dbo.Fn_GetPublishStateIdForPreview()) --, ISPublish = 1 
		from @TBL_BlogData A inner join ZnodeBlogNews B  ON A.BlogNewsCode  = B.BlogNewsCode
		and (B.BlogNewsType = @BlogNewsType OR @BlogNewsType = ''  )
	else If (@RevisionState = 'Production'  Or @RevisionState = 'None' )
		Update B SET PublishStateId = (select dbo.Fn_GetPublishStateIdForPublish()) --, ISPublish = 1 
		from @TBL_BlogData A inner join ZnodeBlogNews B  ON A.BlogNewsCode  = B.BlogNewsCode
		and (B.BlogNewsType = @BlogNewsType OR @BlogNewsType = '' )
	


	End
END TRY 
BEGIN CATCH 
	SET @Status =0  

	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishBlogNewsEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  	
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishBlogNewsEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END