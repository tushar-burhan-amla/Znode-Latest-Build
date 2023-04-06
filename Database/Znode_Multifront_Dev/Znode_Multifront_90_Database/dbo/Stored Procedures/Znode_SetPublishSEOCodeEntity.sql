CREATE PROCEDURE [dbo].[Znode_SetPublishSEOCodeEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@CMSSEOTypeId int = 0  
  ,@CMSSEOCode varchar(300) = ''
  ,@UserId int = 0 
  ,@Status bit =0  OUTPUT 
  )
AS
/*
    This Procedure is used to publish the blog news against the store 
  
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
	
	declare @Status bit =0 
	Exec [dbo].[ZnodeSetPublishSEOCodeEntity]
	@PortalId  = 1 
	,@LocaleId  = 0
	,@RevisionState = 'Production' 
	,@CMSSEOTypeId = 1 
	,@CMSSEOCode = 'frt0987'
	,@UserId  = 2
	,@Status =@Status  OUTPUT 

GO
 

	declare @p8 bit
	set @p8=1
	exec sp_executesql N'ZnodeSetPublishSEOCodeEntity @PortalId,@LocaleId,@RevisionState,@CMSSEOTypeId,@CMSSEOCode,@UserId,@Status OUT',
	N'@PortalId int,@LocaleId int,@RevisionState varchar(50),@CMSSEOTypeId int,@CMSSEOCode varchar(50),@UserId int,@Status bit OUTput',
	@PortalId=1,@LocaleId=1,@RevisionState=N'Production',@CMSSEOTypeId=1,@CMSSEOCode= N'frt0987',@UserId=2,@Status =@p8 OUTPUT
	select @p8

*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
   Begin 
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
		DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))

		DECLARE @TBL_SEO TABLE 
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
								 AND (CD.CMSSEOTypeId =@CMSSEOTypeId ) )

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
						and seoCode = seoCode )
					)
	
					INSERT INTO @TBL_SEO (ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
					SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,
					PortalId,SEOCode,CanonicalURL,RobotTag)
					SELECT 
						ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
						SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,@SetLocaleId,OldSEOURL,CMSContentPagesId,
						PortalId,SEOCode,CanonicalURL,RobotTag	
					FROM Cte_GetDefaultFilterData  A 
				SET @IncrementalId = @IncrementalId +1 
			END 
		End 
		End			
		
		If (@CMSSEOTypeId =3 )
			Begin 
				Delete from ZnodePublishSeoEntity where (PortalId = @PortalId  OR @PortalId = 0 ) and VersionId  in 
				(select VersionId from ZnodePublishWebstoreentity
				where  (PortalId = @PortalId OR @PortalId = 0 ) 
				--AND (LocaleId  = @LocaleId OR @LocaleId  = 0 )
				AND 
				(
					(ZnodePublishWebstoreentity.PublishState =  Case when  (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) then 'Preview' End ) 
					OR 
					(ZnodePublishWebstoreentity.PublishState =  Case when (@RevisionState like '%Production%' OR @RevisionState = 'None') then  'Production'  end )
				)) 
				AND (SEOCode = @CMSSEOCode OR @CMSSEOCode = '' )
				AND (@CMSSEOTypeId = 3  )
	    		
				Insert Into ZnodePublishSeoEntity 
				(
					VersionId,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
					SEOTypeName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,
					PortalId,SEOCode,CanonicalURL,RobotTag	
				)
				SELECT B.VersionId , @GetDate, ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
					ItemName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,A.LocaleId,OldSEOURL,Isnull(CMSContentPagesId,0),
					A.PortalId,SEOCode,CanonicalURL,RobotTag
				FROM @TBL_SEO A Inner join ZnodePublishWebstoreentity B on A.PortalId = B.PortalId and A.LocaleId = B.LocaleId
				AND 
				(
					(B.PublishState =  Case when  (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) then 'Preview' End ) 
					OR 
					(B.PublishState =  Case when (@RevisionState like '%Production%' OR @RevisionState = 'None') then  'Production'  end )
				)
			end 
		If @CMSSEOTypeId in (2,1) 
		Begin
				
				Delete from ZnodePublishSeoEntity where 
				(PortalId = @PortalId  OR @PortalId = 0 ) and VersionId  in 
				(
					Select VersionId  from ZnodePublishVersionEntity  ZPVE inner join ZnodePortalCatalog  
					ZPC ON ZPVE.ZnodeCatalogId = ZPC.PublishCatalogId where  (ZPC.PortalId = @PortalId OR @PortalId = 0 )
					--AND (ZPVE.LocaleId  = @LocaleId OR @LocaleId  = 0 )  
					AND 
					(
						(ZPVE.RevisionType =  Case when  (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) then 'Preview' End ) 
						OR 
						(ZPVE.RevisionType =  Case when (@RevisionState like '%Production%' OR @RevisionState = 'None') then  'Production'  end )
					)
				) 
				AND (SEOCode = @CMSSEOCode OR @CMSSEOCode = '' )
				AND (@CMSSEOTypeId in (2,1)) 
	    		
				Insert Into ZnodePublishSeoEntity 
				(
					VersionId,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
					SEOTypeName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,
					PortalId,SEOCode,CanonicalURL,RobotTag	
				)
				SELECT ZPVE.VersionId , @GetDate, ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,
					ItemName,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,A.LocaleId,OldSEOURL,Isnull(CMSContentPagesId,0),
					A.PortalId,SEOCode,CanonicalURL,RobotTag
				from ZnodePublishVersionEntity  ZPVE inner join ZnodePortalCatalog  
					ZPC ON ZPVE.ZnodeCatalogId = ZPC.PublishCatalogId 
					Inner join @TBL_SEO A On A.PortalId = ZPC.PortalId  and ZPVE.LocaleId  = A.LocaleId  
					where  (ZPC.PortalId = @PortalId OR @PortalId = 0 )
					AND 
					(
						(ZPVE.RevisionType =  Case when  (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) then 'Preview' End ) 
						OR 
						(ZPVE.RevisionType =  Case when (@RevisionState like '%Production%' OR @RevisionState = 'None') then  'Production'  end )
					)		
		End
	End
	If (@RevisionState = 'Preview'  )
	Update B SET PublishStateId = (select dbo.Fn_GetPublishStateIdForPreview()) , ISPublish = 1 
		from @TBL_SEO  A inner join ZnodeCMSSEODetail B  ON A.CMSSEODetailId  = B.CMSSEODetailId
	else If (@RevisionState = 'Production'  Or @RevisionState = 'None' )
		Update B SET PublishStateId = (select dbo.Fn_GetPublishStateIdForPublish()) , ISPublish = 1 
		from @TBL_SEO  A inner join ZnodeCMSSEODetail B  ON A.CMSSEODetailId  = B.CMSSEODetailId

	SELECT 0 AS ID,CAST(1 AS BIT) AS Status;  
	SET @Status =1 
END TRY 
BEGIN CATCH 
	SET @Status =0  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishSEOEntity 
	@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
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