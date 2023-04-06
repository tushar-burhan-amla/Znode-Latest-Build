CREATE PROCEDURE [dbo].[Znode_SetPublishContentPageConfigEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@PreviewVersionId INT = 0 
  ,@IsPreviewEnable int = 0 
  ,@ProductionVersionId INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@CMSContentPagesId int = 0
  ,@UserId int = 0 
  ,@Status int  Output
)
AS
/*
    This Procedure is used to publish the blog news against the store 
  
	EXEC ZnodeSetPublishContentPageConfigEntity 1 2,3
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
		DECLARE @TBL_ContentPage TABLE 
		(ContentPageId	int,PortalId	int,FileName	varchar(300), ProfileId	varchar(300),
		LocaleId	int,PageTitle	nvarchar(200),PageName	nvarchar(max),ActivationDate	datetime,
		ExpirationDate	datetime,IsActive	varchar)
 
		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )

		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN 
			SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
			
			;With Cte_GetCMSContentPages AS 
			(
				SELECT CCP.CMSContentPagesId,CCP.PortaLId, CT.FileName,CCP.PageName ,CCPL.PageTitle, 
				CCP.ActivationDate ,CCP.ExpirationDate ,CCP.IsActive   ,
				CCPL.LocaleId  
				From ZnodeCMSContentPages CCP inner join ZnodeCMSTemplate CT on CCP.CMSTemplateId = CT.CMSTemplateId
				INNER JOIN ZnodeCMSContentPagesLocale CCPL ON CCP.CMSContentPagesId = CCPL.CMSContentPagesId 
				WHERE (CCPL.LocaleId = @SetLocaleId OR CCPL.LocaleId = @DefaultLocaleId)  
				AND CCP.PortalId = @PortalId  
				AND (CCP.CMSContentPagesId = @CMSContentPagesId OR @CMSContentPagesId = 0 )
			)
			, Cte_GetFirstCMSContentPages  AS
			(
				SELECT CMSContentPagesId,PortaLId,FileName,PageName,PageTitle, 
				ActivationDate,ExpirationDate,IsActive, LocaleId
				FROM Cte_GetCMSContentPages 
				WHERE LocaleId = @SetLocaleId
			)
			, Cte_GetDefaultFilterData AS
			(
				SELECT CMSContentPagesId,PortaLId,FileName,PageName,PageTitle,ActivationDate,ExpirationDate,IsActive, LocaleId
				FROM  Cte_GetFirstCMSContentPages 
				UNION ALL 
				SELECT CMSContentPagesId,PortaLId,FileName,PageName,PageTitle,ActivationDate,ExpirationDate,IsActive, LocaleId
				FROM Cte_GetCMSContentPages CTEC 
				WHERE LocaleId = @DefaultLocaleId 
				AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GetFirstCMSContentPages CTEFD WHERE CTEFD.CMSContentPagesId = CTEC.CMSContentPagesId )
			)
	
			INSERT INTO @TBL_ContentPage ( ContentPageId,PortaLId,FileName,PageName,PageTitle,ActivationDate,ExpirationDate,IsActive, LocaleId,ProfileId)
			SELECT  CMSContentPagesId,PortaLId,FileName,PageName,PageTitle,ActivationDate,ExpirationDate,IsActive, @SetLocaleId,
			'[' + (
			--select ProfileId ProfileId from ZnodeCMSContentPagesProfile  C Where C.CMSContentPagesId = A.CMSContentPagesId  and ProfileId  is not null FOR JSON PATH, Include_Null_Values
			(Select stuff((SELECT ','+ Cast(ProfileId as Varchar(10)) FROM ZnodeCMSContentPagesProfile  C Where C.CMSContentPagesId = A.CMSContentPagesId  and ProfileId  is not null 
			FOR XML PATH('')), 1, 1, ''))

			) + ']' AS ProfileId
			FROM Cte_GetDefaultFilterData  A 
     		SET @IncrementalId = @IncrementalId +1 
		END 
	End

	--select * from @TBL_ContentPage 
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%' OR @RevisionState like '%Production%' )
	Begin
	    --Data inserted into flat table ZnodePublishContentPageConfigEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishContentPageConfigEntity where PortalId = @PortalId  and VersionId in (Select PreviewVersionId from  @TBL_PreviewVersionId)
		AND (ContentPageId = @CMSContentPagesId OR @CMSContentPagesId = 0 )

		Insert Into ZnodePublishContentPageConfigEntity 
		(
			VersionId,PublishStartTime,ContentPageId,PortalId,FileName,ProfileId,LocaleId,PageTitle,PageName,
			ActivationDate,ExpirationDate,IsActive
		)
		SELECT B.PreviewVersionId , @GetDate,  ContentPageId,A.PortalId,FileName,ProfileId,B.LocaleId,PageTitle,PageName,
			ActivationDate,ExpirationDate,IsActive FROM @TBL_ContentPage A 
			Inner join @TBL_PreviewVersionId B  ON A.PortalId = B.PortalId AND A.LocaleId = B.LocaleId 
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishContentPageConfigEntity where PortalId = @PortalId  and VersionId in (Select ProductionVersionId from  @TBL_ProductionVersionId)
		AND (ContentPageId = @CMSContentPagesId OR @CMSContentPagesId = 0 )
		
		Insert Into ZnodePublishContentPageConfigEntity 
		(
			VersionId,PublishStartTime,ContentPageId,PortalId,FileName,ProfileId,LocaleId,PageTitle,PageName,
			ActivationDate,ExpirationDate,IsActive
		)
		SELECT B.ProductionVersionId , @GetDate,  ContentPageId,A.PortalId,FileName,ProfileId,B.LocaleId,PageTitle,PageName,
		ActivationDate,ExpirationDate,IsActive FROM @TBL_ContentPage A
		Inner join @TBL_ProductionVersionId B  ON A.PortalId = B.PortalId AND A.LocaleId = B.LocaleId 
	End
	SET @Status = 1
END TRY 
BEGIN CATCH 
	SET @Status =0  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
	@ErrorLine VARCHAR(100)= ERROR_LINE(), 
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishContentPageConfigEntity 
	@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
	+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
	+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
	+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
	+''',@CMSContentPagesId= ' + CAST(@CMSContentPagesId  AS varchar(20))
	+',@UserId = ' + CAST(@UserId AS varchar(20));
              			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishContentPageConfigEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END