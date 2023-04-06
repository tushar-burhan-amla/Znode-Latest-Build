CREATE PROCEDURE [dbo].[Znode_SetPublishPortalBrandEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
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
  
	EXEC ZnodeSetPublishPortalBrandEntity 1 2,3
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
	
	Exec [ZnodeSetPublishPortalBrandEntity]
	   @PortalId  = 7 
	  ,@LocaleId  = 0 
	  ,@PreviewVersionId = 0 
	  ,@ProductionVersionId = 0 
	  ,@RevisionState = 'Production' 
	  ,@CMSMappingId = 0
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
		
			DECLARE @TBL_BrandDetails TABLE  
        (
			Description         NVARCHAR(MAX),  
			BrandId             INT,  
			BrandCode           VARCHAR(600),  
			DisplayOrder        INT,  
			IsActive            BIT,  
			WebsiteLink         NVARCHAR(1000),  
			BrandDetailLocaleId INT,  
			SEOFriendlyPageName NVARCHAR(600),  
			MediaPath           NVARCHAR(MAX),  
			MediaId             INT,  
			ImageName           VARCHAR(300),
			BrandName			VARCHAR(100),	
			Custom1				NVARCHAR(MAX),	
			Custom2				NVARCHAR(MAX),
			Custom3				NVARCHAR(MAX),
			Custom4				NVARCHAR(MAX),
			Custom5				NVARCHAR(MAX),
			PortalId			Int,
			IsAssociated        Bit 
        );  
  
    DECLARE @AttributeId INT= [dbo].[Fn_GetProductBrandAttributeId]();  
             
	DECLARE @TBL_AttributeDefault TABLE  
    (
		PimAttributeId            INT,  
		AttributeDefaultValueCode VARCHAR(600),  
		IsEditable                BIT,  
		AttributeDefaultValue     NVARCHAR(MAX),
		DisplayOrder			  INT   
    );  

    DECLARE @TBL_SeoDetails TABLE  
    (
		CMSSEODetailId       INT,  
		SEOTitle             NVARCHAR(MAX),  
		SEOKeywords          NVARCHAR(MAX),  
		SEOURL               NVARCHAR(MAX),  
		ModifiedDate         DATETIME,  
		SEODescription       NVARCHAR(MAX),  
		MetaInformation      NVARCHAR(MAX),  
		IsRedirect           BIT,  
		CMSSEODetailLocaleId INT,  
		--SEOId                INT ,
		PublishStatus        NVARCHAR(20),
		SEOCode				 NVARCHAR(4000),
		CanonicalURL		 VARCHAR(200),
		RobotTag			 VARCHAR(50)			   
    );  

    DECLARE @TBL_BrandDetail TABLE  
    (
		Description          NVARCHAR(MAX),  
		BrandId              INT,  
		BrandCode            VARCHAR(600),  
		DisplayOrder         INT,  
		IsActive             BIT,  
		WebsiteLink          NVARCHAR(1000),  
		BrandDetailLocaleId  INT,  
		MediaPath            NVARCHAR(MAX),  
		MediaId              INT,  
		ImageName      VARCHAr(300) ,  
		CMSSEODetailId       INT,  
		SEOTitle             NVARCHAR(MAX),  
		SEOKeywords          NVARCHAR(MAX),  
		SEOURL               NVARCHAR(MAX),  
		ModifiedDate         DATETIME,  
		SEODescription       NVARCHAR(MAX),  
		MetaInformation      NVARCHAR(MAX),  
		IsRedirect           BIT,  
		CMSSEODetailLocaleId INT,  
		--SEOId                INT,  
		BrandName            NVARCHAR(MAX),  
		RowId                INT,  
		CountId              INT ,
		SEOCode              NVARCHAR(4000), 
		Custom1              NVARCHAR(MAX),
		Custom2              NVARCHAR(MAX),
		Custom3              NVARCHAR(MAX),
		Custom4              NVARCHAR(MAX),
		Custom5              NVARCHAR(MAX),
		PortalId			 INT
    );  

		iF object_id('tempdb..[#TBL_BrandDetail]') IS NOT NULL
			drop table tempdb..#TBL_BrandDetail
		Create Table #TBL_BrandDetail
		(
			Description          NVARCHAR(MAX),  
			BrandId              INT,  
			BrandCode            VARCHAR(600),  
			DisplayOrder         INT,  
			IsActive             BIT,  
			WebsiteLink          NVARCHAR(1000),  
			BrandDetailLocaleId  INT,  
			MediaPath            NVARCHAR(MAX),  
			MediaId              INT,  
			ImageName      VARCHAr(300) ,  
			CMSSEODetailId       INT,  
			SEOTitle             NVARCHAR(MAX),  
			SEOKeywords          NVARCHAR(MAX),  
			SEOFriendlyPageName  NVARCHAR(MAX),  
			ModifiedDate         DATETIME,  
			SEODescription       NVARCHAR(MAX),  
			MetaInformation      NVARCHAR(MAX),  
			IsRedirect           BIT,  
			CMSSEODetailLocaleId INT,  
			--SEOId                INT,  
			BrandName            NVARCHAR(MAX),  
			PromotionId		     INT,
			RowId                INT,  
			CountId              INT ,
			SEOCode              NVARCHAR(4000), 
			Custom1              NVARCHAR(MAX),
			Custom2              NVARCHAR(MAX),
			Custom3              NVARCHAR(MAX),
			Custom4              NVARCHAR(MAX),
			Custom5              NVARCHAR(MAX),
			PortalId			 INT,
			LocaleId             INT  
		);  
		
		
		;WITH Cte_GetBrandBothLocale AS 
	(
		SELECT ZBDL.Description,ZBD.BrandId,LocaleId,ZBD.BrandCode,isnull(ZPB.DisplayOrder,999) as DisplayOrder,ZBD.IsActive,ZBD.WebsiteLink,ZBDl.BrandDetailLocaleId,  
			SEOFriendlyPageName,[dbo].[Fn_GetMediaThumbnailMediaPath](Zm.path) MediaPath,ZBD.MediaId,Zm.path ImageName, ZBDL.BrandName, ZBD.Custom1, ZBD.Custom2, ZBD.Custom3, ZBD.Custom4, ZBD.Custom5, ZPB.PortalId,
			CASE WHEN ZPB.PortalBrandId IS NULL THEN 0 ELSE 1 END IsAssociated
		FROM ZnodeBrandDetails ZBD 
		LEFT JOIN ZnodePortalBrand ZPB ON ZBD.BrandId = ZPB.BrandId AND (ZPB.PortalId = @PortalId OR isnull(@PortalId,0) = 0 )
		LEFT JOIN ZnodeBrandDetailLocale ZBDL ON(ZBD.BrandId = ZBDL.BrandId)  
		LEFT JOIN ZnodeMedia ZM ON(ZM.MediaId = ZBD.MediaId)  
		WHERE LocaleId IN(@LocaleId, @DefaultLocaleId)  
		
              
    ),  
    Cte_BrandFirstLocale AS 
	(
		SELECT Description,BrandId,LocaleId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,SEOFriendlyPageName,MediaPath,MediaId,ImageName , BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId , IsAssociated
        FROM Cte_GetBrandBothLocale CTGBBL  
        WHERE LocaleId = @LocaleId
	),  
    Cte_BrandDefaultLocale AS 
	(
		SELECT Description,BrandId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,SEOFriendlyPageName,MediaPath,MediaId,ImageName, BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId, IsAssociated  
        FROM Cte_BrandFirstLocale  
        UNION ALL  
        SELECT Description,BrandId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,SEOFriendlyPageName,MediaPath,MediaId,ImageName , BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId, IsAssociated
		FROM Cte_GetBrandBothLocale CTBBL  
		WHERE LocaleId = @DefaultLocaleId  
		AND NOT EXISTS  
		(  
			SELECT TOP 1 1  
			FROM Cte_BrandFirstLocale CTBFL  
			WHERE CTBBL.BrandId = CTBFL.BrandId  
		)
	)    
	INSERT INTO @TBL_BrandDetails (Description,BrandId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,SEOFriendlyPageName,MediaPath,MediaId,ImageName, BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId, IsAssociated)  
    SELECT Description,BrandId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,SEOFriendlyPageName,MediaPath,MediaId,ImageName , BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId, IsAssociated
    FROM Cte_BrandDefaultLocale CTEBD;
       
	-----Update BrandName from attributedefault value
	;WITH Cte_GetBrandNameLocale AS 
	(
		select d.brandcode, a.AttributeDefaultValueCode, b.AttributeDefaultValue, b.LocaleId 
		from ZnodePimAttributeDefaultValue a
		inner join ZnodePimAttributeDefaultValueLocale b on a.PimAttributeDefaultValueId = b.PimAttributeDefaultValueId 
		inner join ZnodePimAttribute c on a.PimAttributeId = c.PimAttributeId
		inner join @TBL_BrandDetails d on a.AttributeDefaultValueCode = d.brandcode
		where c.attributecode = 'brand' and b.LocaleId IN(@LocaleId, @DefaultLocaleId)
              
    )
	,Cte_BrandNameFirstLocale AS 
	(
		SELECT brandcode, AttributeDefaultValueCode, AttributeDefaultValue, LocaleId  
        FROM Cte_GetBrandNameLocale CTGBBL  
        WHERE LocaleId = @LocaleId
	)
	,Cte_BrandDefaultLocale AS 
	(
		SELECT brandcode, AttributeDefaultValueCode, AttributeDefaultValue, LocaleId  
        FROM Cte_BrandNameFirstLocale  
        UNION ALL  
        SELECT brandcode, AttributeDefaultValueCode, AttributeDefaultValue, LocaleId  
		FROM Cte_GetBrandNameLocale CTBBL  
		WHERE LocaleId = @DefaultLocaleId  
		AND NOT EXISTS  
		(  
			SELECT TOP 1 1  
			FROM Cte_BrandNameFirstLocale CTBFL  
			WHERE CTBBL.brandcode = CTBFL.brandcode  
		)
	)  
	update b1 set b1.brandname = a1.AttributeDefaultValue
	from Cte_BrandDefaultLocale a1
	inner join @TBL_BrandDetails b1 on a1.brandcode = b1.brandcode

	DECLARE @SeoCode SelectColumnList
	INSERT INTO @SeoCode
	SELECT BrandCode FROM @TBL_BrandDetails
				

    INSERT INTO @TBL_SeoDetails 
	(
		CMSSEODetailId,SEOTitle,SEOKeywords,SEOURL,ModifiedDate,SEODescription,MetaInformation,IsRedirect,
		CMSSEODetailLocaleId,PublishStatus,SEOCode,CanonicalURL,RobotTag
	)  
    EXEC Znode_GetSeoDetails @SeoCode, 'Brand', @LocaleId;  
			              
    SELECT TBBD.*,TBSD.*
    INTO #TM_BrandLocale  
    FROM @TBL_BrandDetails TBBD  
    INNER JOIN @TBL_SeoDetails TBSD ON(TBSD.SEOCode = TBBD.BrandCode)  
	INNER JOIN ZnodeCmsSeoDetail CSD ON TBSD.CMSSEODetailId = CSD.CMSSEODetailId AND TBBD.PortalId = CSD.PortalId
	
	INSERT INTO #TM_BrandLocale
	SELECT DISTINCT TBBD.*,TBSD.*
    FROM @TBL_BrandDetails TBBD  
    LEFT JOIN @TBL_SeoDetails TBSD ON(TBSD.SEOCode = TBBD.BrandCode) 
	WHERE NOT EXISTS(SELECT * FROM #TM_BrandLocale BL WHERE TBBD.BrandId = BL.BrandId AND TBBD.BrandDetailLocaleId = BL.BrandDetailLocaleId)

		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )

		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN 
			SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
			
			Insert into #TBL_BrandDetail
			(BrandId,Description,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,MediaPath,MediaId,ImageName,CMSSEODetailId,SEOTitle,SEOKeywords,SEOFriendlyPageName,SEODescription,MetaInformation,IsRedirect,CMSSEODetailLocaleId,BrandName,
			PromotionId ,SEOCode,Custom1, Custom2, Custom3, Custom4, Custom5, PortalId,LocaleId)
			select BrandId,Description,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,MediaPath,MediaId,ImageName,CMSSEODetailId,SEOTitle,SEOKeywords,SEOFriendlyPageName,SEODescription,MetaInformation,IsRedirect,CMSSEODetailLocaleId,BrandName,
			0 PromotionId ,SEOCode,Custom1, Custom2, Custom3, Custom4, Custom5, PortalId ,@SetLocaleId from #TM_BrandLocale 
			where  PortalId = @PortalId
		SET @IncrementalId = @IncrementalId +1 
		END 
	End


	If @IsPreviewEnable = 1 AND ( @RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) 
	Begin
	    --Data inserted into flat table ZnodePublishPortalBrandEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishPortalBrandEntity where PortalId = @PortalId  and VersionId in 
		( Select PreviewVersionId from @Tbl_PreviewVersionId)
		
		Insert Into ZnodePublishPortalBrandEntity 
		(VersionId,PublishStartTime,PortalId,LocaleId, BrandId,BrandCode,BrandName,MediaId ,WebsiteLink,Description,PublishState,
		 SEOTitle,SEOKeywords,SEODescription,SEOFriendlyPageName ,DisplayOrder,IsActive,MediaPath,CMSSEODetailId,CMSSEODetailLocaleId,
		 BrandDetailLocaleId,ImageName,Custom1,Custom2,Custom3,Custom4,Custom5)
		SELECT B.PreviewVersionId , @GetDate,B.PortalId ,B.LocaleId ,a.BrandId,BrandCode,BrandName,MediaId ,WebsiteLink,
		       Description,'PREVIEW',SEOTitle,SEOKeywords,SEODescription,SEOFriendlyPageName ,a.DisplayOrder,IsActive,MediaPath,
			   CMSSEODetailId,CMSSEODetailLocaleId,BrandDetailLocaleId,ImageName,Custom1,Custom2,Custom3,Custom4,Custom5
		FROM #TBL_BrandDetail A inner join @TBL_PreviewVersionId B on 
		@PortalId= B.PortalId and A.LocaleId = b.LocaleId
		inner join ZnodePortalBrand ZPB on a.BrandId = ZPB.BrandId and b.PortalId = ZPB.PortalId

	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
		-- Only production version id will process 
		Delete from ZnodePublishPortalBrandEntity where PortalId = @PortalId  and VersionId in (select ProductionVersionId from @TBL_ProductionVersionId)
		
		Insert Into ZnodePublishPortalBrandEntity 
		(VersionId,PublishStartTime,PortalId,LocaleId,BrandId,BrandCode,BrandName,MediaId
		 ,WebsiteLink,Description,PublishState,SEOTitle,SEOKeywords,SEODescription,SEOFriendlyPageName
		 ,DisplayOrder,IsActive,MediaPath,CMSSEODetailId,CMSSEODetailLocaleId,BrandDetailLocaleId,ImageName)
		SELECT B.ProductionVersionId , @GetDate,B.PortalId ,B.LocaleId
		,a.BrandId,BrandCode,BrandName,MediaId
		,WebsiteLink,Description,'PREVIEW',SEOTitle,SEOKeywords,SEODescription,SEOFriendlyPageName
		,a.DisplayOrder,IsActive,MediaPath,CMSSEODetailId,CMSSEODetailLocaleId,BrandDetailLocaleId,ImageName
		FROM #TBL_BrandDetail A inner join @TBL_ProductionVersionId B on 
		@PortalId= B.PortalId and A.LocaleId = b.LocaleId
		inner join ZnodePortalBrand ZPB on a.BrandId = ZPB.BrandId and b.PortalId = ZPB.PortalId
	End
	SET @Status =1 
END TRY 
BEGIN CATCH 
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishPortalBrandEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		                			 
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishPortalBrandEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

	END CATCH
END