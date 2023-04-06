CREATE PROCEDURE [dbo].[Znode_SetPublishWidgetSliderBannerEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@PreviewVersionId INT = 0 
  ,@IsPreviewEnable int = 0 
  ,@ProductionVersionId INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@CMSContentPagesId int = 0
  ,@CMSSliderId INT = 0 
  ,@UserId int = 0 
  ,@Status int = 0 OUTPUT 
)
AS
/*
    This Procedure is used to publish the slider banner widgets,
	This sp get call for catalog publish , store , and slider 
	  
	EXEC ZnodeSetPublishWidgetSliderBannerEntity 1 2,3
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
	

Exec [ZnodeSetPublishWidgetSliderBannerEntity]
@PortalId  = 1
,@LocaleId  = 0 
,@PreviewVersionId = 0 
,@ProductionVersionId = 0 
,@RevisionState = 'Preview@Production' 
,@CMSContentPagesId = 88
,@CMSSliderId = 0
,@UserId = 0 
	*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
	DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
   ---Following code is manditory because this sp get call from multiple places
   If Exists (SELECT  * from ZnodePublishStateApplicationTypeMapping PSA where PSA.IsEnabled =1 and  
	Exists (select TOP 1 1  from ZnodePublishState PS where PS.PublishStateId = PSA.PublishStateId ) and ApplicationType =  'WebstorePreview')
		SET @IsPreviewEnable = 1 
	else 
		SET @IsPreviewEnable = 0 

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
 
   If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
   Begin
		If not exists (Select TOP 1 1 from @Tbl_PreviewVersionId) 
			Begin
				SET @Status =0 ;
				If @CMSSliderId	> 0  AND @CMSContentPagesId  = 0 and @PortalId = 0  -- Slider Publish 
					SELECT 1 AS ID,cast(@Status as BIT) AS Status;  
					Return 0
			End
   End
   If  (@RevisionState like '%Production%' OR @RevisionState = 'None')
   Begin
		If not exists (Select TOP 1 1 from @Tbl_ProductionVersionId) 
		Begin
			SET @Status =0 ;
			If @CMSSliderId	> 0  AND @CMSContentPagesId  = 0 and @PortalId = 0  -- Slider Publish 
				SELECT 1 AS ID,cast(@Status as BIT) AS Status;  
			Return 0 
		End
   End


   Begin 
		DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))
	
		--CMSContentPage associated with portal 
		DECLARE @TBL_CMSContentPagesPortalWise TABLE(CMSContentPagesId INT, PortalId int );
		 
		 If @CMSSliderId	 = 0 AND @CMSContentPagesId  > 0 and @PortalId > 0 -- content page  
			INSERT INTO @TBL_CMSContentPagesPortalWise(CMSContentPagesId, PortalId)	SELECT CMSContentPagesId , PortalId FROM ZnodeCMSContentPages
			WHERE PortalId = @PortalId	AND IsActive = 1 AND (CMSContentPagesId = @CMSContentPagesId )
		 Else  If @CMSSliderId	> 0  AND @CMSContentPagesId  = 0 and @PortalId = 0  -- Slider Publish 
			INSERT INTO @TBL_CMSContentPagesPortalWise(CMSContentPagesId , PortalId)	SELECT ZCCP.CMSContentPagesId , ZCCP.PortalId FROM ZnodeCMSContentPages ZCCP
			WHERE ZCCP.IsActive = 1 AND 
			(Exists   (Select TOP 1 1 from @Tbl_ProductionVersionId TPC where TPC.PortalId = ZCCP.PortalId) 
			OR Exists (Select TOP 1 1 from @Tbl_PreviewVersionId TPC where TPC.PortalId = ZCCP.PortalId) )
		 Else If @CMSSliderId	= 0  AND @CMSContentPagesId  = 0 and @PortalId > 0 -- portal publish
			INSERT INTO @TBL_CMSContentPagesPortalWise(CMSContentPagesId,  PortalId )	SELECT CMSContentPagesId ,  PortalId FROM ZnodeCMSContentPages
			WHERE PortalId = @PortalId	AND IsActive = 1 
		

		Declare @CMSWidgetDataFinal TABLE 
		(
			WidgetSliderBannerId	int,MappingId	int,PortalId	int,LocaleId	int,Type	varchar(100),Navigation	varchar(100),
			AutoPlay	bit,AutoplayTimeOut	int,AutoplayHoverPause	bit,TransactionStyle	varchar(100),WidgetsKey	varchar(300),
			TypeOFMapping	varchar(100),SliderId	int,SliderBanners	nvarchar(max)
		)

		    DECLARE @Tlb_ZnodeCMSWidgetSliderBanner TABLE
                     (CMSWidgetSliderBannerId INT,
                      CMSMappingId            INT,
                      PortalId                INT,
                      Type                    NVARCHAR(100) NULL,
                      Navigation              NVARCHAR(100) NULL,
                      AutoPlay                BIT,
                      AutoplayTimeOut         INT,
                      AutoplayHoverPause      BIT,
                      TransactionStyle        NVARCHAR(100) NULL,
                      WidgetsKey              NVARCHAR(256),
                      TypeOFMapping           NVARCHAR(100),
                      CMSSliderId             INT
                     );

                     DECLARE @TBL_ZnodeCMSSliderDetail TABLE
                     (CMSSliderId        INT,
                      CMSSliderBannerId  INT,
                      MediaPath          VARCHAR(300),
                      Title              NVARCHAR(1000),
					  ButtonLabelName    NVARCHAR(1200),
                      ButtonLink         NVARCHAR(600),
                      TextAlignment      NVARCHAR(200),
                      BannerSequence     INT,
                      ActivationDate     DATETIME,
                      ExpirationDate     DATETIME,
                      ImageAlternateText NVARCHAR(1000),
                      DEscription        NVARCHAR(MAX)
                     );

                     DECLARE @TBL_ZnodeCMSSliderDetail_Locale TABLE
                     (CMSSliderId        INT,
                      CMSSliderBannerId  INT,
                      MediaPath          VARCHAR(300),
                      Title              NVARCHAR(1000),
                      ButtonLabelName    NVARCHAR(1200),
                      ButtonLink         NVARCHAR(600),
                      TextAlignment      NVARCHAR(200),
                      BannerSequence     INT,
                      ActivationDate     DATETIME,
                      ExpirationDate     DATETIME,
                      ImageAlternateText NVARCHAR(1000),
                      DEscription        NVARCHAR(MAX),
                      LocaleId           INT
                     );

		If @CMSSliderId	 = 0 AND @CMSContentPagesId  > 0 and @PortalId > 0 -- content page  
		   
			INSERT INTO @Tlb_ZnodeCMSWidgetSliderBanner(CMSWidgetSliderBannerId,CMSMappingId,PortalId,Type,Navigation,AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey,TypeOFMapping,CMSSliderId)
			SELECT ACWSB.CMSWidgetSliderBannerId,ACWSB.CMSMappingId,
			@PortalId PortalId,
			ACWSB.Type,ACWSB.Navigation,ACWSB.AutoPlay,ACWSB.AutoplayTimeOut
			,ACWSB.AutoplayHoverPause,ACWSB.TransactionStyle,ACWSB.WidgetsKey,ACWSB.TypeOfMapping
			OFMapping,ACWSB.CMSSliderId FROM ZnodeCMSWidgetSliderBanner AS ACWSB WHERE
			((TypeOfMapping = 'ContentPageMapping' AND CMSMappingId IN
			(SELECT CMSContentPagesId	FROM @TBL_CMSContentPagesPortalWise	) ))
         Else  If @CMSSliderId	> 0  AND @CMSContentPagesId  = 0 and @PortalId = 0  -- Slider Publish 
			INSERT INTO @Tlb_ZnodeCMSWidgetSliderBanner(CMSWidgetSliderBannerId,CMSMappingId,PortalId,Type,Navigation,AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey,TypeOFMapping,CMSSliderId)
			SELECT ACWSB.CMSWidgetSliderBannerId,ACWSB.CMSMappingId,
			CASE when TypeOfMapping = 'PortalMapping'  THEN ACWSB.CMSMappingId ELSE 
			(SELECT TOP 1 PortalId	FROM @TBL_CMSContentPagesPortalWise  where ACWSB.CMSMappingId =CMSMappingId )
			END 	AS PortalId,
			ACWSB.Type,ACWSB.Navigation,ACWSB.AutoPlay,ACWSB.AutoplayTimeOut
			,ACWSB.AutoplayHoverPause,ACWSB.TransactionStyle,ACWSB.WidgetsKey,ACWSB.TypeOfMapping
			OFMapping,ACWSB.CMSSliderId FROM ZnodeCMSWidgetSliderBanner AS ACWSB 
			WHERE
			((TypeOfMapping = 'PortalMapping'	AND ( Exists (Select TOP 1 1 from @Tbl_ProductionVersionId TPC where TPC.PortalId = CMSMappingId) 
			OR Exists (Select TOP 1 1 from @Tbl_PreviewVersionId TPC where TPC.PortalId = CMSMappingId) )	OR 
			 (TypeOfMapping = 'ContentPageMapping' AND CMSMappingId IN  (SELECT CMSContentPagesId	FROM @TBL_CMSContentPagesPortalWise)))
			AND (ACWSB.CMSSliderId = @CMSSliderId OR @CMSSliderId = 0 ))
			

		Else If @CMSSliderId	= 0  AND @CMSContentPagesId  = 0 and @PortalId > 0 -- portal publish
			INSERT INTO @Tlb_ZnodeCMSWidgetSliderBanner(CMSWidgetSliderBannerId,CMSMappingId,PortalId,Type,Navigation,AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey,TypeOFMapping,CMSSliderId)
			SELECT ACWSB.CMSWidgetSliderBannerId,ACWSB.CMSMappingId,
			CASE when TypeOfMapping = 'PortalMapping'  THEN ACWSB.CMSMappingId ELSE @PortalId END 	AS PortalId,
			ACWSB.Type,ACWSB.Navigation,ACWSB.AutoPlay,ACWSB.AutoplayTimeOut
			,ACWSB.AutoplayHoverPause,ACWSB.TransactionStyle,ACWSB.WidgetsKey,ACWSB.TypeOfMapping
			OFMapping,ACWSB.CMSSliderId FROM ZnodeCMSWidgetSliderBanner AS ACWSB WHERE
			((TypeOfMapping = 'PortalMapping'	AND CMSMappingId = @PortalId OR @PortalId = 0 )	OR 
				(TypeOfMapping = 'ContentPageMapping' AND CMSMappingId IN (SELECT CMSContentPagesId	FROM @TBL_CMSContentPagesPortalWise	) ))
							
		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )

		
		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN 
	
			SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
			;With Cte_GetCMSSEODetails AS 
			(
					SELECT ZCSB.CMSSliderId,ZCSB.CMSSliderBannerId,ZM.Path MediaPath ,ZCSBL.Title,ZCSBL.ButtonLabelName,ZCSBL.ButtonLink,ZCSB.TextAlignment,ZCSB.BannerSequence,ZCSB.ActivationDate,ZCSB.ExpirationDate,ZCSBL.ImageAlternateText,ZCSBL.DEscription,ISNULL(ZCSBL.LocaleId, @DefaultLocaleId) AS LocaleId
					FROM ZnodeCMSSliderBanner AS ZCSB LEFT JOIN ZnodeCMSSliderBannerLocale AS ZCSBL ON(ZCSB.CMSSliderBannerId = ZCSBL.CMSSliderBannerId
					AND	(ZCSBL.LocaleId  = @SetLocaleId OR ZCSBL.LocaleId  = @DefaultLocaleId))  
					LEFT OUTER JOIN ZnodeMEdia  AS ZM ON ZCSBL.MediaId = ZM.MediaId
					WHERE EXISTS
					(
						SELECT TOP 1 1
						FROM @Tlb_ZnodeCMSWidgetSliderBanner AS ACWSB
						WHERE ACWSB.CMSSliderId = ZCSB.CMSSliderId
					)

			)
			, Cte_GetFirstCMSSEODetails  AS
			(
				SELECT 
					CMSSliderId,CMSSliderBannerId,MediaPath,Title,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,ActivationDate,ExpirationDate,ImageAlternateText,DEscription
				FROM Cte_GetCMSSEODetails 
				WHERE LocaleId = @SetLocaleId
			)
			, Cte_GetDefaultFilterData AS
			(
				SELECT 
					CMSSliderId,CMSSliderBannerId,MediaPath,Title,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,ActivationDate,ExpirationDate,ImageAlternateText,DEscription
					 FROM  Cte_GetFirstCMSSEODetails 
				UNION ALL 
				SELECT 
					CMSSliderId,CMSSliderBannerId,MediaPath,Title,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,ActivationDate,ExpirationDate,ImageAlternateText,DEscription
				FROM Cte_GetCMSSEODetails CTEC 
				WHERE LocaleId = @DefaultLocaleId 
				AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GetFirstCMSSEODetails CTEFD WHERE   CTEC.CMSSliderBannerId = CTEFD.CMSSliderBannerId
                                      AND CTEC.CMSSliderId = CTEFD.CMSSliderId)
			)
	
			INSERT INTO @CMSWidgetDataFinal 
			(
				WidgetSliderBannerId,MappingId,PortalId,LocaleId,Type,Navigation	,
				AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey	,
				TypeOFMapping,SliderId,SliderBanners
			)
		   SELECT DISTINCT
		   CMSWidgetSliderBannerId AS WidgetSliderBannerId,CMSMappingId AS MappingId,PortalId,
		   @SetLocaleId AS LocaleId,Type,Navigation,AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey,TypeOFMapping
		   ,CMSSliderId AS SliderId,
			(  
				SELECT ISNULL(CMSSliderId,'') AS SliderId,ISNULL(CMSSliderBannerId,'') AS SliderBannerId,ISNULL(MediaPath,'')MediaPath
				,ISNULL(Title,'')Title,ISNULL(ButtonLabelName,'')ButtonLabelName,ISNULL(ButtonLink,'')ButtonLink
				,ISNULL(TextAlignment,'')TextAlignment,ISNULL(BannerSequence,'')BannerSequence,ActivationDate
				,ExpirationDate,ISNULL(ImageAlternateText,'')ImageAlternateText,ISNULL(Description,'')Description
				FROM Cte_GetDefaultFilterData AS wd
				WHERE wd.CMSSliderId = a.CMSSliderId 
				FOR JSON PATH  
			)SliderBanners
			FROM @Tlb_ZnodeCMSWidgetSliderBanner AS a

			SET @IncrementalId = @IncrementalId +1 
		END 
	End
		
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
	Begin
	    --Data inserted into flat table ZnodePublishWidgetSliderBannerEntity (Replica of MongoDB Collection )  

		Delete from ZnodePublishWidgetSliderBannerEntity where VersionId in (Select PreviewVersionId from @Tbl_PreviewVersionId )
	 		 AND (SliderId = @CMSSliderId OR @CMSSliderId = 0   )
			 AND (MappingId in (Select CMSContentPagesId from @TBL_CMSContentPagesPortalWise) OR @CMSContentPagesId  = 0);

		Insert Into ZnodePublishWidgetSliderBannerEntity 
		(VersionId,PublishStartTime,WidgetSliderBannerId,MappingId,PortalId,LocaleId,Type,Navigation	,
				AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey	,
				TypeOFMapping,SliderId,SliderBanners	)

		SELECT B.PreviewVersionId , @GetDate,WidgetSliderBannerId,MappingId,A.PortalId,A.LocaleId,Type,Navigation	,
				AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey	,
				TypeOFMapping,SliderId,SliderBanners
	    FROM @CMSWidgetDataFinal A INNER JOIN @Tbl_PreviewVersionId B
		On A.LocaleId = B.LocaleId AND A.PortalId = B.PortalId 
	
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishWidgetSliderBannerEntity where VersionId in (Select ProductionVersionId from @Tbl_ProductionVersionId )
	 		 AND (SliderId = @CMSSliderId OR @CMSSliderId = 0   )
			 AND (MappingId in (Select CMSContentPagesId from @TBL_CMSContentPagesPortalWise) OR @CMSContentPagesId  = 0);

		Insert Into ZnodePublishWidgetSliderBannerEntity 
		(
			VersionId,PublishStartTime,WidgetSliderBannerId,MappingId,PortalId,LocaleId,Type,Navigation,
			AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey	,
			TypeOFMapping,SliderId,SliderBanners
		)
		
		SELECT B.ProductionVersionId, @GetDate,WidgetSliderBannerId,MappingId,A.PortalId,A.LocaleId,Type,Navigation	,
				AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey	,
				TypeOFMapping,SliderId,SliderBanners
	    FROM @CMSWidgetDataFinal A INNER JOIN @Tbl_ProductionVersionId B
		On A.LocaleId = B.LocaleId AND A.PortalId = B.PortalId
		
	End

	If @RevisionState ='PREVIEW'
	Begin
			update A SET A.IsPublished = 1 , A.PublishStateId = [dbo].[Fn_GetPublishStateIdForPreview]() 
			from ZnodeCMSSlider A INNER JOIN @CMSWidgetDataFinal B on A.CMSSliderId = B.SliderId 
	End
	Else 
			update A SET A.IsPublished = 1 , A.PublishStateId = [dbo].[Fn_GetPublishStateIdForPublish]() 
			from ZnodeCMSSlider A INNER JOIN @CMSWidgetDataFinal B on A.CMSSliderId = B.SliderId 
		

	SET @Status =1 ;
	
	If Not Exists (select TOP 1 1 from @CMSWidgetDataFinal ) 
	Begin
		If @CMSSliderId	> 0  AND @CMSContentPagesId  = 0 and @PortalId = 0  -- Slider Publish 
		Begin
			SET @Status =0 ;
			SELECT 1 AS ID,cast(@Status as BIT) AS Status;  
		End
	End
	Else 
		If @CMSSliderId	> 0  AND @CMSContentPagesId  = 0 and @PortalId = 0  -- Slider Publish 
		SELECT 1 AS ID,cast(@Status as BIT) AS Status;

END TRY 
BEGIN CATCH 
	SET @Status =0 ;
	If @CMSSliderId	> 0  AND @CMSContentPagesId  = 0 and @PortalId = 0  -- Slider Publish 
		SELECT 1 AS ID,cast(@Status as BIT) AS Status;  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishWidgetSliderBannerEntity 
			@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
			+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
			+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
			+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
			+''',@CMSContentPagesId= ' + CAST(@CMSContentPagesId  AS varchar(20))
			+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishWidgetSliderBannerEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END
