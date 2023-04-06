CREATE PROCEDURE [dbo].[Znode_SetPublishSEOEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@IsPreviewEnable int = 0 
  ,@PreviewVersionId INT = 0 
  ,@ProductionVersionId INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@CMSSEOTypeId varchar(500) = '' 
  ,@CMSSEOCode varchar(300) = ''
  ,@UserId int = 0 
  ,@Status int OUTPUT 
  ,@IsCatalogPublish bit = 0 
  ,@VersionIdString varchar(2000) = ''
  ,@IsSingleProductPublish bit = 0 
)
AS
/*
    This Procedure is used to publish the SEO details
  
	EXEC ZnodeSetPublishSEOEntity 1 2,3
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
	
	Exec [ZnodeSetPublishSEOEntity]
	   @PortalId  = 1 
	  ,@LocaleId  = 0 
	  ,@PreviewVersionId = 0 
	  ,@ProductionVersionId = 0 
	  ,@RevisionState = 'Preview/Production' 
	  ,@CMSSEOTypeId = 0
	  ,@CMSSEOCode = ''
	  ,@UserId = 0 

	 Exec [ZnodeSetPublishSEOEntity]
   @PortalId  = 1 
  ,@LocaleId  = 0 
  ,@PreviewVersionId = 0 
  ,@ProductionVersionId = 0 
  ,@RevisionState = 'Preview&Production' 
  ,@CMSSEOTypeId = 3
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
		If (@IsCatalogPublish = 0  AND @IsSingleProductPublish = 0 )
		Begin
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
 		End
		--Else if (@IsCatalogPublish= 1  AND @IsSingleProductPublish = 0 )
		--Begin
		--	 IF OBJECT_ID('tempdb..#VesionIds') is not null
		--		DROP TABLE #VesionIds
  				 
		--	 SELECT PV.* into #VesionIds FROM ZnodePublishVersionEntity PV Inner join Split(@VersionIdString,',') S ON PV.VersionId = S.Item
		--End

		IF OBJECT_ID('tempdb..#VesionIds') is not null
				DROP TABLE #VesionIds
  				 
			 SELECT PV.* into #VesionIds FROM ZnodePublishVersionEntity PV Inner join Split(@VersionIdString,',') S ON PV.VersionId = S.Item
		
		DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))
		CREATE TABLE #TBL_SEO  
		(
			ItemName varchar(50),CMSSEODetailId int ,CMSSEODetailLocaleId int ,CMSSEOTypeId int ,SEOId int ,SEOTypeName varchar(50),SEOTitle nvarchar(Max)
			,SEODescription nvarchar(Max),
			SEOKeywords nvarchar(Max),SEOUrl nvarchar(Max) ,IsRedirect bit ,MetaInformation nvarchar(Max) ,LocaleId int ,
			OldSEOURL nvarchar(Max),CMSContentPagesId int ,PortalId int ,SEOCode varchar(300) ,CanonicalURL varchar(200),RobotTag varchar(50)
		)
		
		BEGIN 
			INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )
			
			SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
			WHILE @IncrementalId <= @MaxCount
			BEGIN 
				SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
				IF @IsSingleProductPublish = 0
				Begin
					;With Cte_GetCMSSEODetails AS 
					(
							select CT.Name ItemName,CD.CMSSEODetailId, CDL.CMSSEODetailLocaleId ,  CD.CMSSEOTypeId ,
									 CD.SEOId ,CDL.SEOTitle,CDL.SEODescription,
									 CDL.SEOKeywords,Lower(CD.SEOUrl) SEOUrl,CD.IsRedirect,CD.MetaInformation,
									 CDL.LocaleId,
									 NULL OldSEOURL, 
									 NULL CMSContentPagesId,CD.PortalId, CD.seoCode,CDL.CanonicalURL,CDL.RobotTag
									 from ZnodeCMSSEODetail CD 
									 INNER JOIN ZnodeCMSSEOType CT ON CD.CMSSEOTypeId = CT.CMSSEOTypeId 
									 INNER JOIN ZnodeCMSSEODetailLocale CDL ON CD.CMSSEODetailId = CDL.CMSSEODetailId 
									 WHERE (CDL.LocaleId = @SetLocaleId OR CDL.LocaleId = @DefaultLocaleId)  
									 AND (CD.PortalId = @PortalId  OR @PortalId  = 0 ) 
									 AND (Isnull(CD.SEOCode ,'') = @CMSSEOCode OR @CMSSEOCode = '' )
									 AND (Exists  (SELECT TOP 1 1 FROM dbo.Split(@CMSSEOTypeId ,',') SP WHERE SP.Item = CD.CMSSEOTypeId ) )
									union all
									select CT.Name ItemName,CD.CMSSEODetailId, CDL.CMSSEODetailLocaleId ,  CD.CMSSEOTypeId ,
										 CD.SEOId ,CDL.SEOTitle,CDL.SEODescription,
										 CDL.SEOKeywords,Lower(CD.SEOUrl) SEOUrl,CD.IsRedirect,CD.MetaInformation,
										 CDL.LocaleId,
										 NULL OldSEOURL, 
										 NULL CMSContentPagesId,ZPB.PortalId, CD.seoCode,CDL.CanonicalURL,CDL.RobotTag
										 from ZnodeCMSSEODetail CD 
										 INNER JOIN ZnodeCMSSEOType CT ON CD.CMSSEOTypeId = CT.CMSSEOTypeId 
										 INNER JOIN ZnodeCMSSEODetailLocale CDL ON CD.CMSSEODetailId = CDL.CMSSEODetailId
										 INNER JOIN ZnodeBrandDetails ZBD ON CD.SeoCode = ZBD.BrandCode
										 INNER JOIN ZnodePortalBrand ZPB ON ZBD.BrandId = ZPB.BrandId
										 WHERE (CDL.LocaleId = @SetLocaleId OR CDL.LocaleId = @DefaultLocaleId)  
										 AND (ZPB.PortalId = @PortalId  OR @PortalId  = 0 ) 
										 AND (Isnull(CD.SEOCode ,'') = @CMSSEOCode OR @CMSSEOCode = '' )
										 AND (Exists  (SELECT TOP 1 1 FROM dbo.Split(@CMSSEOTypeId ,',') SP WHERE SP.Item = CD.CMSSEOTypeId ) )
										 AND CT.Name = 'Brand' 
										 AND @IsCatalogPublish = 0
									 Union All 
									 select CT.Name ItemName,CD.CMSSEODetailId, CDL.CMSSEODetailLocaleId ,  CD.CMSSEOTypeId ,
								 CD.SEOId ,CDL.SEOTitle,CDL.SEODescription,
								 CDL.SEOKeywords,Lower(CD.SEOUrl) SEOUrl,CD.IsRedirect,CD.MetaInformation,
								 CDL.LocaleId,
								 NULL OldSEOURL, 
								 NULL CMSContentPagesId,ZPB.PortalId, CD.seoCode,CDL.CanonicalURL,CDL.RobotTag
								 from ZnodeCMSSEODetail CD 
								 INNER JOIN ZnodeCMSSEOType CT ON CD.CMSSEOTypeId = CT.CMSSEOTypeId 
								 INNER JOIN ZnodeCMSSEODetailLocale CDL ON CD.CMSSEODetailId = CDL.CMSSEODetailId 
								 INNER JOIN ZnodeBrandDetails ZBD ON CD.SeoCode = ZBD.BrandCode
								 INNER JOIN ZnodePortalBrand ZPB ON ZBD.BrandId = ZPB.BrandId
								 WHERE (CDL.LocaleId = @SetLocaleId OR CDL.LocaleId = @DefaultLocaleId)  
								 AND (Isnull(CD.SEOCode ,'') = @CMSSEOCode OR @CMSSEOCode = '' )
								 AND (CT.Name = 'Brand' ) 
								 AND @IsCatalogPublish= 1 
					)
					, Cte_GetFirstCMSSEODetails  AS
					(
						SELECT 
							ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
							SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation, LocaleId ,OldSEOURL,CMSContentPagesId,
							PortalId,SEOCode,CanonicalURL,RobotTag	
						FROM Cte_GetCMSSEODetails 
						WHERE LocaleId = @SetLocaleId
					)
					, Cte_GetDefaultFilterData AS
					(
						SELECT 
							ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
							SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,
							PortalId,SEOCode,CanonicalURL,RobotTag	  FROM  Cte_GetFirstCMSSEODetails 
						UNION ALL 
						SELECT 
							ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
							SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,
							PortalId,SEOCode,CanonicalURL,RobotTag	 FROM Cte_GetCMSSEODetails CTEC 
						WHERE LocaleId = @DefaultLocaleId 
						AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GetFirstCMSSEODetails CTEFD WHERE CTEFD.CMSSEOTypeId = CTEC.CMSSEOTypeId 
						and CTEFD.seoCode = CTEC.seoCode )
					)
	
					INSERT INTO #TBL_SEO (ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
					SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,
					PortalId,SEOCode,CanonicalURL,RobotTag)
					SELECT 
						ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
						SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,@SetLocaleId,OldSEOURL,CMSContentPagesId,
						PortalId,SEOCode,CanonicalURL,RobotTag	
					FROM Cte_GetDefaultFilterData  A 

					End 
					Else If @IsSingleProductPublish = 1  
						INSERT INTO #TBL_SEO (ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
						SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,
						PortalId,SEOCode,CanonicalURL,RobotTag)
							SELECT CT.Name ItemName,CD.CMSSEODetailId, CDL.CMSSEODetailLocaleId ,  CD.CMSSEOTypeId ,
							CD.SEOId ,CDL.SEOTitle,CDL.SEODescription,
							CDL.SEOKeywords,Lower(CD.SEOUrl) SEOUrl,CD.IsRedirect,CD.MetaInformation,
							CDL.LocaleId,
							NULL OldSEOURL, 
							NULL CMSContentPagesId,CD.PortalId, CD.seoCode,CDL.CanonicalURL,CDL.RobotTag
							from ZnodeCMSSEODetail CD 
							INNER JOIN ZnodeCMSSEOType CT ON CD.CMSSEOTypeId = CT.CMSSEOTypeId 
							INNER JOIN ZnodeCMSSEODetailLocale CDL ON CD.CMSSEODetailId = CDL.CMSSEODetailId 
							WHERE (CDL.LocaleId = @LocaleId )  
							AND (CD.PortalId = @PortalId  ) 
							AND (Isnull(CD.SEOCode ,'') = @CMSSEOCode OR @CMSSEOCode = '' )
							AND (Exists  (SELECT TOP 1 1 FROM dbo.Split(@CMSSEOTypeId ,',') SP WHERE SP.Item = CD.CMSSEOTypeId ) )

				SET @IncrementalId = @IncrementalId +1 
			END 
		End 
		End			

	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%' OR  @RevisionState like '%Production%')  AND @IsSingleProductPublish = 0
	Begin
	    --Data inserted into flat table ZnodePublishSeoEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishSeoEntity where PortalId = @PortalId  and VersionId  in (Select PreviewVersionId  from @TBL_PreviewVersionId ) 
		AND (SEOCode = @CMSSEOCode OR @CMSSEOCode = '' )
		AND (Exists  (SELECT TOP 1 1 FROM dbo.Split(@CMSSEOTypeId ,',') SP WHERE SP.Item = CMSSEOTypeId ) )
		AND @IsCatalogPublish= 0   
		

		If @IsCatalogPublish= 0
		BEGIN
			UPDATE C SET C.ElasticSearchEvent = 2 
			FROM ZnodePublishSeoEntity C
			WHERE NOT EXISTS(SELECT * FROM #TBL_SEO A
				Inner join @TBL_PreviewVersionId B on A.LocaleId = B.LocaleId AND A.LocaleId = B.LocaleId
				WHERE B.PreviewVersionId = C.VersionId AND A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId 
							AND A.PortalId = C.PortalId AND A.SEOCode = C.SEOCode)
			--AND c.VersionId in (Select Item from Split(@VersionIdString,','))

			UPDATE C SET C.ItemName = A.ItemName ,
				C.SEOTypeName = A.ItemName,C.SEOTitle = A.SEOTitle,C.SEODescription = A.SEODescription,C.SEOKeywords=A.SEOKeywords,C.SEOUrl = A.SEOUrl,
				C.IsRedirect=A.IsRedirect,C.MetaInformation = A.MetaInformation,C.OldSEOURL = A.OldSEOURL,
				C.CanonicalURL = A.CanonicalURL,C.RobotTag = A.RobotTag, C.ElasticSearchEvent = 1
			FROM #TBL_SEO A 
			Inner join @TBL_PreviewVersionId B on A.LocaleId = B.LocaleId AND A.LocaleId = B.LocaleId 
			INNER JOIN ZnodePublishSeoEntity C ON B.PreviewVersionId = C.VersionId AND A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId 
						AND A.PortalId = C.PortalId AND A.SEOCode = C.SEOCode
			--AND c.VersionId in (Select Item from Split(@VersionIdString,','))

			Insert Into ZnodePublishSeoEntity 
			(
				VersionId,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
				SEOTypeName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL
				,CMSContentPagesId,	PortalId,SEOCode,CanonicalURL,RobotTag, ElasticSearchEvent
			)
			SELECT B.PreviewVersionId , @GetDate, ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
				ItemName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,A.LocaleId,OldSEOURL,Isnull(CMSContentPagesId,0),
				A.PortalId,SEOCode,CanonicalURL,RobotTag, 1 AS ElasticSearchEvent
			FROM #TBL_SEO A Inner join @TBL_PreviewVersionId B on A.LocaleId = B.LocaleId AND A.LocaleId = B.LocaleId
			WHERE NOT EXISTS(SELECT * FROM ZnodePublishSeoEntity C WHERE B.PreviewVersionId = C.VersionId AND A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId AND A.PortalId = C.PortalId)

		END

		If @IsCatalogPublish= 1
		BEGIN
			UPDATE C SET C.ElasticSearchEvent = 2 
			FROM ZnodePublishSeoEntity C
			WHERE NOT EXISTS(SELECT * FROM #TBL_SEO A
				--Inner join #VesionIds B on A.LocaleId = B.LocaleId AND B.RevisionType = 'PREVIEW'
				WHERE --B.VersionId = C.VersionId AND 
					A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId 
							AND A.PortalId = C.PortalId AND A.SEOCode = C.SEOCode)
			--AND c.VersionId in (Select Item from Split(@VersionIdString,','))

			UPDATE C SET C.ItemName = A.ItemName ,
				C.SEOTypeName = A.ItemName, C.SEOTitle = A.SEOTitle,C.SEODescription = A.SEODescription,C.SEOKeywords=A.SEOKeywords,C.SEOUrl = A.SEOUrl,
				C.IsRedirect=A.IsRedirect,C.MetaInformation = A.MetaInformation,C.OldSEOURL = A.OldSEOURL,
				C.CanonicalURL = A.CanonicalURL,C.RobotTag = A.RobotTag, C.ElasticSearchEvent = 1
			FROM #TBL_SEO A 
			--Inner join #VesionIds B on A.LocaleId = B.LocaleId AND B.RevisionType = 'PREVIEW' 
			INNER JOIN ZnodePublishSeoEntity C ON --B.VersionId = C.VersionId AND 
				A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId 
						AND A.PortalId = C.PortalId AND A.SEOCode = C.SEOCode
			--AND c.VersionId in (Select Item from Split(@VersionIdString,','))

			Insert Into ZnodePublishSeoEntity 
			(
				VersionId,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
				SEOTypeName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL
				,CMSContentPagesId,	PortalId,SEOCode,CanonicalURL,RobotTag, ElasticSearchEvent	
			)
			SELECT C.VersionId , @GetDate, A.ItemName,A.CMSSEODetailId,A.CMSSEODetailLocaleId,A.CMSSEOTypeId,A.SEOId,
				A.ItemName,A.SEOTitle,A.SEODescription,A.SEOKeywords,A.SEOUrl,A.IsRedirect,A.MetaInformation,A.LocaleId,
				A.OldSEOURL,Isnull(A.CMSContentPagesId,0),
				A.PortalId,A.SEOCode,A.CanonicalURL,A.RobotTag, 1 AS ElasticSearchEvent
			FROM #TBL_SEO A --Inner join #VesionIds B on A.LocaleId = B.LocaleId AND B.RevisionType = 'PREVIEW'
			INNER JOIN ZnodePublishSeoEntity C ON 
				A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId AND A.PortalId = C.PortalId

		END
	End

	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None') and @IsSingleProductPublish = 0
	Begin
		-- Only production version id will process 
		Delete from ZnodePublishSeoEntity where PortalId = @PortalId  and VersionId in (Select ProductionVersionId from  @TBL_ProductionVersionId ) 
		AND (SEOCode = @CMSSEOCode OR @CMSSEOCode = '' )
		AND (Exists  (SELECT TOP 1 1 FROM dbo.Split(@CMSSEOTypeId ,',') SP WHERE SP.Item = CMSSEOTypeId ) )
		AND @IsCatalogPublish= 0  
		--AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZnodePublishSeoEntity.VersionId)

		If @IsCatalogPublish= 0 		
		BEGIN
			UPDATE C SET C.ElasticSearchEvent = 2 
			FROM ZnodePublishSeoEntity C
			WHERE NOT EXISTS(SELECT * FROM #TBL_SEO A
				INNER JOIN @TBL_ProductionVersionId B on A.PortalId = B.PortalId and A.LocaleId = B.LocaleId 
				WHERE B.ProductionVersionId = C.VersionId AND A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId 
							AND A.PortalId = C.PortalId AND A.SEOCode = C.SEOCode)
			--AND c.VersionId in (Select Item from Split(@VersionIdString,','))

			UPDATE C SET ItemName = A.ItemName ,
				SEOTypeName = A.ItemName,SEOTitle = A.SEOTitle,SEODescription = A.SEODescription,SEOKeywords=A.SEOKeywords,SEOUrl = A.SEOUrl,
				IsRedirect=A.IsRedirect,MetaInformation = A.MetaInformation,OldSEOURL = A.OldSEOURL,
				CanonicalURL = A.CanonicalURL,RobotTag = A.RobotTag, C.ElasticSearchEvent = 1
			FROM #TBL_SEO A 
			INNER JOIN @TBL_ProductionVersionId B on A.PortalId = B.PortalId and A.LocaleId = B.LocaleId 
			INNER JOIN ZnodePublishSeoEntity C ON B.ProductionVersionId = C.VersionId AND A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId 
						AND A.PortalId = C.PortalId AND A.SEOCode = C.SEOCode
			--AND c.VersionId in (Select Item from Split(@VersionIdString,','))

			Insert Into ZnodePublishSeoEntity 
			(
				VersionId,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
				SEOTypeName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL
				,CMSContentPagesId,	PortalId,SEOCode,CanonicalURL,RobotTag, ElasticSearchEvent
			)
			SELECT B.ProductionVersionId , @GetDate, ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
				ItemName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,A.LocaleId,OldSEOURL,Isnull(CMSContentPagesId,0),
				A.PortalId,SEOCode,CanonicalURL,RobotTag, 1 AS ElasticSearchEvent
			FROM #TBL_SEO A Inner join @TBL_ProductionVersionId B on A.PortalId = B.PortalId and A.LocaleId = B.LocaleId
			WHERE NOT EXISTS(SELECT * FROM ZnodePublishSeoEntity C WHERE B.ProductionVersionId = C.VersionId AND A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId AND A.PortalId = C.PortalId)
		END
	   If @IsCatalogPublish= 1 		
	   BEGIN
			UPDATE C SET C.ElasticSearchEvent = 2 FROM ZnodePublishSeoEntity C
			WHERE NOT EXISTS(SELECT * FROM #TBL_SEO A
				INNER JOIN @TBL_ProductionVersionId B on A.PortalId = B.PortalId and A.LocaleId = B.LocaleId 
				WHERE B.ProductionVersionId = C.VersionId AND A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId 
							AND A.PortalId = C.PortalId AND A.SEOCode = C.SEOCode)
			--AND c.VersionId in (Select Item from Split(@VersionIdString,','))

			UPDATE C SET C.ItemName = A.ItemName ,
				C.SEOTypeName = A.ItemName,C.SEOTitle = A.SEOTitle,C.SEODescription = A.SEODescription,C.SEOKeywords=A.SEOKeywords,C.SEOUrl = A.SEOUrl,
				C.IsRedirect = A.IsRedirect,C.MetaInformation = A.MetaInformation, C.OldSEOURL = A.OldSEOURL,
				C.CanonicalURL = A.CanonicalURL, C.RobotTag = A.RobotTag, C.ElasticSearchEvent = 1
			FROM #TBL_SEO A 
			--INNER JOIN #VesionIds B on A.LocaleId = B.LocaleId AND B.RevisionType = 'PRODUCTION'
			INNER JOIN ZnodePublishSeoEntity C ON A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId 
						AND A.PortalId = C.PortalId AND A.SEOCode = C.SEOCode
			--AND c.VersionId in (Select Item from Split(@VersionIdString,','))

			Insert Into ZnodePublishSeoEntity 
			(
				VersionId,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
				SEOTypeName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL
				,CMSContentPagesId,	PortalId,SEOCode,CanonicalURL,RobotTag	,ElasticSearchEvent
			)
			SELECT C.VersionId , @GetDate, A.ItemName,A.CMSSEODetailId,A.CMSSEODetailLocaleId,A.CMSSEOTypeId,A.SEOId,
				A.ItemName,A.SEOTitle,A.SEODescription,A.SEOKeywords,A.SEOUrl,A.IsRedirect,A.MetaInformation,A.LocaleId,
				A.OldSEOURL,Isnull(A.CMSContentPagesId,0),
				A.PortalId,A.SEOCode,A.CanonicalURL,A.RobotTag, 1 AS ElasticSearchEvent
			FROM #TBL_SEO A 
			--Inner join #VesionIds B on A.LocaleId = B.LocaleId AND B.RevisionType = 'PRODUCTION'
			inner join ZnodePublishSeoEntity C ON A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId AND A.PortalId = C.PortalId
		END
	
	End

	--Single Product Publish 
	If @IsSingleProductPublish =1  
	Begin
			Delete from ZnodePublishSeoEntity where PortalId = @PortalId  --and VersionId in (Select Item from Split(@VersionIdString,',')) 
			AND (SEOCode = @CMSSEOCode OR @CMSSEOCode = '' )
			AND (Exists  (SELECT TOP 1 1 FROM dbo.Split(@CMSSEOTypeId ,',') SP WHERE SP.Item = CMSSEOTypeId ) )
			--AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZnodePublishSeoEntity.VersionId)

			UPDATE C SET ItemName = A.ItemName ,
				SEOTypeName = A.ItemName,SEOTitle = A.SEOTitle,SEODescription = A.SEODescription,SEOKeywords=A.SEOKeywords,SEOUrl = A.SEOUrl,
				IsRedirect=A.IsRedirect,MetaInformation = A.MetaInformation,OldSEOURL = A.OldSEOURL,
				CanonicalURL = A.CanonicalURL,RobotTag = A.RobotTag
			FROM #TBL_SEO A 
			INNER JOIN ZnodePublishSeoEntity C ON A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId 
						AND A.PortalId = C.PortalId AND A.SEOCode = C.SEOCode
			--AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = C.VersionId)
		
			Insert Into ZnodePublishSeoEntity 
			(
				VersionId,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
				SEOTypeName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL
				,CMSContentPagesId,	PortalId,SEOCode,CanonicalURL,RobotTag	
			)
			SELECT --(Select Item from Split(@VersionIdString,',')), 
				c.VersionId ,@GetDate,A.ItemName, A.CMSSEODetailId,A.CMSSEODetailLocaleId,A.CMSSEOTypeId,A.SEOId,
				A.ItemName,A.SEOTitle,A.SEODescription,A.SEOKeywords,A.SEOUrl,A.IsRedirect,A.MetaInformation,@LocaleId,
				A.OldSEOURL,Isnull(A.CMSContentPagesId,0),
				A.PortalId,A.SEOCode,A.CanonicalURL,A.RobotTag
			FROM #TBL_SEO A 
			inner join ZnodePublishSeoEntity C ON A.CMSSEODetailId = C.CMSSEODetailId AND A.LocaleId = C.LocaleId AND A.PortalId = C.PortalId
		
	end 

	If (@RevisionState = 'Preview'  )
		Update B SET PublishStateId = (select dbo.Fn_GetPublishStateIdForPreview()) , ISPublish = 1 
		from #TBL_SEO  A inner join ZnodeCMSSEODetail B  ON A.CMSSEODetailId  = B.CMSSEODetailId
	else If (@RevisionState = 'Production'  Or @RevisionState = 'None' )
		Update B SET PublishStateId = (select dbo.Fn_GetPublishStateIdForPublish()) , ISPublish = 1 
		from #TBL_SEO  A inner join ZnodeCMSSEODetail B  ON A.CMSSEODetailId  = B.CMSSEODetailId

	SET @Status =1 
END TRY 
BEGIN CATCH 
	SET @Status =0  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishSEOEntity 
	@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
	+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
	+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
	+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
	+''',@CMSSEOTypeId= ' + CAST(@CMSSEOTypeId  AS varchar(20))
	+',@UserId = ' + CAST(@UserId AS varchar(20))
	+',@CMSSEOCode  = ''' + CAST(@CMSSEOCode  AS varchar(20)) + '''';
	        			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishSEOEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END