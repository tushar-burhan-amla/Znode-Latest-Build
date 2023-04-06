CREATE  PROCEDURE [dbo].[Znode_SetPublishWidgetTitleEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@PreviewVersionId INT = 0 
  ,@IsPreviewEnable int= 0 
  ,@ProductionVersionId INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@CMSContentPagesId int = 0
  ,@UserId int = 0 
  ,@Status int = 0 Output
)
AS
/*
    This Procedure is used to publish the blog news against the store 
  
	EXEC ZnodeSetPublishWidgetTitleEntity 1 2,3
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
	
	Exec [ZnodeSetPublishWidgetTitleEntity]
	   @PortalId  = 1 
	  ,@LocaleId  = 0 
	  ,@PreviewVersionId = 0 
	  ,@ProductionVersionId = 0 
	  ,@RevisionState = 'Preview/Production' 
	  ,@CMSSEOTypeId = 0
	  ,@CMSSEOCode = ''
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
		DECLARE @TBL_LocaleId TABLE ( RowId INT IDENTITY(1,1) PRIMARY KEY , LocaleId INT )
		 		
		DECLARE @TBL_GetDATATable TABLE (WidgetTitleConfigurationId INT ,MappingId INT ,  PortalId INT 
			, LocaleId INT , MediaPath   VARCHAR(300) , Title nvarchar (600) ,TitleCode nvarchar(600) ,TypeOFMapping NVARCHAR(100) 
			,Url nvarchar(600)  , WidgetsKey  NVARCHAR(256),IsNewTab bit, DisplayOrder INT)

		
		INSERT INTO @TBL_LocaleId (LocaleId )  SELECT LocaleId FROm ZnodeLocale WHERE IsActive = 1 AND (LocaleId = @LocaleId OR   ISNULL(@LocaleId,0) = 0)
		DECLARE @CounterId INT = 1 , @MaxRowId INT = (SELECT Max(RowId ) FROM @TBL_LocaleId) 
		--SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		
		WHILE @CounterId <= @MaxRowId
		BEGIN 
			SET @SetLocaleId = (SELECT TOP 1 LocaleID FROM @TBL_LocaleId WHERe RowId =@CounterId )  
			If @CMSContentPagesId > 0 
				Begin
				;With Cte_GetContentPagesId AS 
				(
					SELECT ZCWTC.CMSWidgetTitleConfigurationId WidgetTitleConfigurationId,ZCWTC.CMSMappingId MappingId,
					@PortalId
					AS PortalId,LocaleId LocaleId,ZM.[path]MediaPath,ZCWTCL.Title,ZCWTC.TitleCode,ZCWTC.TypeOFMapping,ZCWTCl.Url,ZCWTC.WidgetsKey,IsNewTab
					,ZCWTCL.DisplayOrder
					FROM ZnodeCMSWidgetTitleConfiguration ZCWTC 
					INNER JOIN ZnodeCMSWidgetTitleConfigurationLocale  ZCWTCL ON (ZCWTCL.CMSWidgetTitleConfigurationId = ZCWTC.CMSWidgetTitleConfigurationId )
					INNER JOIN ZnodeCMSContentPages CCP on CMSContentPagesId= ZCWTC.CMSMappingId   
					LEFT JOIN ZnodeMedia ZM ON (Zm.MediaId = ZCWTCL.MediaId)
					WHERE ZCWTCL.LocaleId   IN (@DefaultLocaleId,@SetLocaleId) 
					and (CCP.PortalId  =  @PortalId OR @PortalId = 0  )
					AND  (ZCWTC.TypeOfMapping = 'ContentPageMapping'	AND ZCWTC.CMSMappingId  = @CMSContentPagesId OR @CMSContentPagesId  = 0)
				) 
				, Cte_LocaleIdFiletr AS 
				(
					SELECT WidgetTitleConfigurationId WidgetTitleConfigurationId,MappingId MappingId,
					PortalId,LocaleId LocaleId, MediaPath,Title,TitleCode,TypeOFMapping,Url,WidgetsKey,IsNewTab,DisplayOrder
					FROM Cte_GetContentPagesId CET1 WHERE LocaleId = @SetLocaleId
				)
				,Cte_CompleteRecords AS 
				(
					SELECT WidgetTitleConfigurationId WidgetTitleConfigurationId,MappingId MappingId,
					PortalId,LocaleId LocaleId, MediaPath,Title,TitleCode,TypeOFMapping,Url,WidgetsKey,IsNewTab,DisplayOrder
					FROM Cte_LocaleIdFiletr
					UNION ALL 
					SELECT WidgetTitleConfigurationId WidgetTitleConfigurationId,MappingId MappingId,
					PortalId,LocaleId LocaleId, MediaPath,Title,TitleCode,TypeOFMapping,Url,WidgetsKey,IsNewTab,DisplayOrder
					FROM Cte_GetContentPagesId  CTE2
					WHERE Cte2.localeId = @DefaultLocaleId
					AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_LocaleIdFiletr CTE1 WHERE Cte1.WidgetTitleConfigurationId = CTE2.WidgetTitleConfigurationId )
				)

				INSERT INTO @TBL_GetDATATable (WidgetTitleConfigurationId  ,MappingId  ,  PortalId  , LocaleId  , MediaPath   , Title  ,TitleCode  ,TypeOFMapping  ,Url   , WidgetsKey,IsNewTab ,DisplayOrder)
				SELECT WidgetTitleConfigurationId WidgetTitleConfigurationId,MappingId MappingId,
					PortalId,@SetLocaleId LocaleId, MediaPath,Title,TitleCode,TypeOFMapping,Url,WidgetsKey,IsNewTab,DisplayOrder
				FROM Cte_CompleteRecords 
			End
			ELSE If @CMSContentPagesId = 0  and @PortalId > 0 
				Begin
				;With Cte_GetContentPagesId AS 
				(
					SELECT ZCWTC.CMSWidgetTitleConfigurationId WidgetTitleConfigurationId,ZCWTC.CMSMappingId MappingId,
					@PortalId
					AS PortalId,LocaleId LocaleId,ZM.[path]MediaPath,ZCWTCL.Title,ZCWTC.TitleCode,ZCWTC.TypeOFMapping,ZCWTCl.Url,ZCWTC.WidgetsKey,IsNewTab
					,ZCWTCL.DisplayOrder
					FROM ZnodeCMSWidgetTitleConfiguration ZCWTC 
					INNER JOIN ZnodeCMSWidgetTitleConfigurationLocale  ZCWTCL ON (ZCWTCL.CMSWidgetTitleConfigurationId = ZCWTC.CMSWidgetTitleConfigurationId )
					LEFT JOIN ZnodeMedia ZM ON (Zm.MediaId = ZCWTCL.MediaId)
					WHERE ZCWTCL.LocaleId   IN (@DefaultLocaleId,@SetLocaleId) 
					AND  ((ZCWTC.TypeOfMapping = 'PortalMapping'	AND ZCWTC.CMSMappingId  = @PortalId)
					OR    (ZCWTC.TypeOfMapping = 'ContentPageMapping'	AND Exists 
					(Select TOP 1 1 from ZnodeCMSContentPages CP  where CP.PortalId = @PortalId 
					and CP.IsActive =1 and CP.CMSContentPagesId = ZCWTC.CMSMappingId   )))
				)	 
				, Cte_LocaleIdFiletr AS 
				(
					SELECT WidgetTitleConfigurationId WidgetTitleConfigurationId,MappingId MappingId,
					PortalId,LocaleId LocaleId, MediaPath,Title,TitleCode,TypeOFMapping,Url,WidgetsKey,IsNewTab,DisplayOrder
					FROM Cte_GetContentPagesId CET1 WHERE LocaleId = @SetLocaleId
				)
				,Cte_CompleteRecords AS 
				(
					SELECT WidgetTitleConfigurationId WidgetTitleConfigurationId,MappingId MappingId,
					PortalId,LocaleId LocaleId, MediaPath,Title,TitleCode,TypeOFMapping,Url,WidgetsKey,IsNewTab,DisplayOrder
					FROM Cte_LocaleIdFiletr
					UNION ALL 
					SELECT WidgetTitleConfigurationId WidgetTitleConfigurationId,MappingId MappingId,
					PortalId,LocaleId LocaleId, MediaPath,Title,TitleCode,TypeOFMapping,Url,WidgetsKey,IsNewTab,DisplayOrder
					FROM Cte_GetContentPagesId  CTE2
					WHERE Cte2.localeId = @DefaultLocaleId
					AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_LocaleIdFiletr CTE1 WHERE Cte1.WidgetTitleConfigurationId = CTE2.WidgetTitleConfigurationId )
				)

				INSERT INTO @TBL_GetDATATable (WidgetTitleConfigurationId  ,MappingId  ,  PortalId  , LocaleId  , MediaPath   , Title  ,TitleCode  ,TypeOFMapping  ,Url   , WidgetsKey,IsNewTab ,DisplayOrder)
				SELECT WidgetTitleConfigurationId WidgetTitleConfigurationId,MappingId MappingId,
					PortalId,@SetLocaleId LocaleId, MediaPath,Title,TitleCode,TypeOFMapping,Url,WidgetsKey,IsNewTab,DisplayOrder
				FROM Cte_CompleteRecords 
			End

			SET @CounterId = @CounterId+1 
		END 

	END 

	If @IsPreviewEnable = 1 AND ( @RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) 
	Begin
		--Data inserted into flat table ZnodePublishWidgetTitleEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishWidgetTitleEntity where PortalId = @PortalId  and VersionId in (Select PreviewVersionId from  @Tbl_PreviewVersionId) and
		(MappingId = @CMSContentPagesId OR @CMSContentPagesId = 0 )

		Insert Into ZnodePublishWidgetTitleEntity
		(
			VersionId,PublishStartTime,WidgetTitleConfigurationId,PortalId,MappingId,MediaPath,Title,
		    Url,WidgetsKey,TypeOFMapping,ActivationDate,ExpirationDate,IsActive,LocaleId,TitleCode,DisplayOrder,IsNewTab)

		SELECT B.PreviewVersionId , @GetDate, WidgetTitleConfigurationId  ,A.PortalId  ,MappingId  , MediaPath   , Title  ,
			Url   , WidgetsKey,TypeOFMapping  , NULL, NULL , 0 , A.LocaleId  ,TitleCode ,DisplayOrder,IsNewTab
	    FROM @TBL_GetDATATable  A Inner join @Tbl_PreviewVersionId B on 
		 a.PortalId = B.PortalId AND A.LocaleId = B.LocaleId 		
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin

	 -- Only production version id will process 
		Delete from ZnodePublishWidgetTitleEntity where PortalId = @PortalId  and VersionId in ( select ProductionVersionId from @TBL_ProductionVersionId)  and
		(MappingId = @CMSContentPagesId OR @CMSContentPagesId = 0 )
		
		Insert Into ZnodePublishWidgetTitleEntity 
		(
			VersionId,PublishStartTime,WidgetTitleConfigurationId,PortalId,MappingId,MediaPath,Title,
		    Url,WidgetsKey,TypeOFMapping,ActivationDate,ExpirationDate,IsActive,LocaleId,TitleCode,DisplayOrder,IsNewTab		)
	
		SELECT B.ProductionVersionId , @GetDate, WidgetTitleConfigurationId  ,A.PortalId  ,MappingId  , MediaPath   , Title  ,
			Url   , WidgetsKey,TypeOFMapping  , NULL, NULL , 0, a.LocaleId  ,TitleCode ,DisplayOrder,IsNewTab
	    FROM @TBL_GetDATATable A Inner join @Tbl_ProductionVersionId B on 
		 a.PortalId = B.PortalId AND A.LocaleId = B.LocaleId 		
	End
	SET @Status =1 
END TRY 
BEGIN CATCH 
	SET @Status =0  
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishWidgetTitleEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+''',@CMSContentPagesId= ' + CAST(@CMSContentPagesId  AS varchar(20))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		                			 
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishWidgetTitleEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END