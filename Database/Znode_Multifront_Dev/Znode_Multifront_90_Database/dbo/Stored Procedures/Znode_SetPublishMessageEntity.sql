CREATE PROCEDURE [dbo].[Znode_SetPublishMessageEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@PreviewVersionId INT = 0 
  ,@IsPreviewEnable int = 0 
  ,@ProductionVersionId INT = 0 
  ,@RevisionState varchar(50) = ''
  ,@CMSMessageKeyId INT = 0 
  ,@UserId int = 0 
  ,@Status int = 0 Output 
)
AS
/*
    This Procedure is used to publish the content block 
	1. For ZnodePublishGlobalMessageEntity pass parameter Value for @CMSMessageKeyId and @PortalId should be 0 
	declare @Status int 
	Exec [dbo].[ZnodeSetPublishMessageEntity]
	   @PortalId  = 0 
	  ,@LocaleId  = 0 
	  ,@PreviewVersionId = 0 
	  ,@IsPreviewEnable = 0 
	  ,@ProductionVersionId = 0 
	  ,@RevisionState = 'PRODUCTION'
	  ,@CMSMessageKeyId = 25 
	  ,@UserId = 2 
	  ,@Status = @Status  Output 

	 2. For ZnodePublishMessageEntity  pass parameter Value for @CMSMessageKeyId should be 0 
	 declare @Status int 
	Exec [dbo].[ZnodeSetPublishMessageEntity]
	   @PortalId  = 1 
	  ,@LocaleId  = 0 
	  ,@PreviewVersionId = 0 
	  ,@IsPreviewEnable = 0 
	  ,@ProductionVersionId = 0 
	  ,@RevisionState = 'PRODUCTION'
	  ,@CMSMessageKeyId = 0 
	  ,@UserId = 2 
	  ,@Status = @Status  Output 

  
	EXEC ZnodeSetPublishMessageEntity 1 2,3
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
	
	 Exec [ZnodeSetPublishMessageEntity]
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
		SET @Status =1 
		DECLARE @Tbl_PreviewVersionId    TABLE    (PreviewVersionId int , PortalId int , LocaleId int)
		DECLARE @Tbl_ProductionVersionId TABLE    (ProductionVersionId int  , PortalId int , LocaleId int)
		DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))
		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )

		
		---Following code is manditory because this sp get call from multiple places
		If Exists (SELECT  * from ZnodePublishStateApplicationTypeMapping PSA where PSA.IsEnabled =1 and  
		Exists (select TOP 1 1  from ZnodePublishState PS where PS.PublishStateId = PSA.PublishStateId ) and ApplicationType =  'WebstorePreview')
			SET @IsPreviewEnable = 1 
		else 
			SET @IsPreviewEnable = 0 

		If @PortalId > 0 
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
		Else 
		Begin
			Declare @GlobalVersion Table ( VersionId int Identity(1,1), LocaleId int , Publishstate varchar(300))
			if @IsPreviewEnable = 1
			Begin
				Insert into @GlobalVersion  
				Select LocaleId , 'PREVIEW' from @TBL_Locale 
			End
			Insert into @GlobalVersion  
				Select LocaleId , 'PRODUCTION' from @TBL_Locale 
		end
		
		If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
		Begin
			If not exists (Select TOP 1 1 from @Tbl_PreviewVersionId) AND @PortalId > 0 
				SET @Status =0 ;
			If @CMSMessageKeyId > 0 AND @Status =0     -- Content block
				SELECT 1 AS ID,cast(@Status as BIT) AS Status;  
			if @Status  = 0 
				Return 0 
		End
		
		If  (@RevisionState like '%Production%' OR @RevisionState = 'None')
		Begin
			If not exists (Select TOP 1 1 from @Tbl_ProductionVersionId) AND @PortalId > 0
				SET @Status =0 ;
			If @CMSMessageKeyId > 0 AND @Status =0  -- Content block
				SELECT 1 AS ID,cast(@Status as BIT) AS Status;  
			if @Status  = 0 
				Return 0 
		End
		
		 DECLARE @TBL_CMSMessageDataFinal TABLE
                     (CMSMessageId INT,
                      LocaleId     INT,
                      Message      NVARCHAR(MAX),
                      MessageKey   NVARCHAR(100),
                      AreaName     VARCHAR(100)
                     );
		
		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN
			    SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
				Begin  
					;With Cte_GetCMSSEODetails AS 
					(
						
						    SELECT DISTINCT ZCM.CMSMessageId,ZCM.LocaleId,ZCM.Message,ZCMK.MessageKey,'' AreaName
                            FROM ZnodeCMSmessage AS ZCM
                            INNER JOIN ZnodeCMSPortalMessage AS ZCPM ON ZCM.CMSMessageId = ZCPM.CMSMessageId
                            INNER JOIN znodecmsmessagekey AS ZCMK ON ZCPM.CMSMessageKeyId = ZCMK.CMSMessageKeyId
                            WHERE (ZCPM.PortalId = @PortalId OR  @PortalId = 0 )  
							AND (ZCM.LocaleId = @SetLocaleId OR ZCM.LocaleId = @DefaultLocaleId) 
							AND (ZCMK.CMSMessageKeyId = @CMSMessageKeyId OR @CMSMessageKeyId  = 0 ) 
					)
					, Cte_GetFirstCMSSEODetails  AS
					(
						SELECT 
							 CMSMessageId,LocaleId,Message,MessageKey,AreaName
							 FROM Cte_GetCMSSEODetails 
						WHERE LocaleId = @SetLocaleId
					)
					, Cte_GetDefaultFilterData AS
					(
						SELECT 
							CMSMessageId,LocaleId,Message,MessageKey,AreaName
						FROM  Cte_GetFirstCMSSEODetails 
						UNION ALL 
						SELECT 
							 CMSMessageId,LocaleId,Message,MessageKey,AreaName
						FROM Cte_GetCMSSEODetails p
						WHERE LocaleId = @DefaultLocaleId 
						AND NOT EXISTS ( SELECT TOP 1 1 FROM Cte_GetFirstCMSSEODetails AS q WHERE q.MessageKey = p.MessageKey)
					)
					
					insert into @TBL_CMSMessageDataFinal(CMSMessageId,LocaleId,Message,MessageKey,AreaName)
					SELECT CMSMessageId,@SetLocaleId ,Message,MessageKey,AreaName FROM Cte_GetDefaultFilterData  A 
				End 
				
				SET @IncrementalId = @IncrementalId + 1;
				--DELETE FROM @CMSWidgetData;
				--DELETE FROM @CMSWidgetDataFinal;
        END;
	
	End

	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) 
	Begin

		If @PortalId  >0 
		Begin
	
			--Data inserted into flat table ZnodePublishMessageEntity (Replica of MongoDB Collection )  
			Delete from ZnodePublishMessageEntity where PortalId = @PortalId  
			and MessageKey in (select MessageKey from @TBL_CMSMessageDataFinal) 
			and VersionId in (Select PreviewVersionId from  @TBL_PreviewVersionId) 
		
			Insert Into ZnodePublishMessageEntity 
			(VersionId,PublishStartTime,LocaleId,PortalId,MessageKey,Message,Area)
			SELECT B.PreviewVersionId , @GetDate, A.LocaleId,@PortalId,MessageKey,Message,AreaName
			FROM @TBL_CMSMessageDataFinal A inner join @Tbl_PreviewVersionId B on 
			@PortalId = B.PortalId and  A.LocaleId = B.LocaleId

			Update B Set B.IsPublished  =1 , PublishStateId = DBO.Fn_GetPublishStateIdForPreview()  
			from ZnodeCmsPortalMessage A 
			Inner join ZnodeCmsMessage B ON A.CMSMessageId = B.CMSMessageId 
			inner join ZnodeCmsMessagekey C   On A.CMSMessageKeyId = C.CMSMessageKeyId 
			where (C.CMSMessageKeyId = @CMSMessageKeyId OR @CMSMessageKeyId  = 0 ) 
			AND (A.PortalId = @PortalId OR @PortalId= 0 )
		End
		If @PortalId  = 0 
		Begin
		
			--ZnodePublishGlobalVersionEntity
			Insert into  ZnodePublishGlobalVersionEntity (VersionId,PublishStartTime,PublishState,LocaleId)
			select A.VersionId, @GetDate,A.Publishstate, A.LocaleId from @GlobalVersion A  where  A.PublishState = 'PREVIEW'
			and Not exists (Select TOP 1 1 from ZnodePublishGlobalVersionEntity B where B.LocaleId = A.LocaleId
			and B.PublishState = 'PREVIEW') 


			--Data inserted into flat table ZnodePublishMessageEntity (Replica of MongoDB Collection )  
			Delete from ZnodePublishGlobalMessageEntity where VersionId in (Select VersionId from  ZnodePublishGlobalVersionEntity
			where PublishState = 'PREVIEW' ) 
			AND (
			(Exists (Select TOP 1 1 from @TBL_CMSMessageDataFinal CMD where CMD.MessageKey = ZnodePublishGlobalMessageEntity.MessageKey))
			OR @CMSMessageKeyId = 0	)

			Insert Into ZnodePublishGlobalMessageEntity 
			(VersionId,PublishStartTime,LocaleId,MessageKey,Message,Area)
			SELECT B.VersionId , @GetDate, A.LocaleId,MessageKey,Message,AreaName
			FROM @TBL_CMSMessageDataFinal A inner join ZnodePublishGlobalVersionEntity B on 
			A.LocaleId = B.LocaleId AND B.PublishState = 'PREVIEW'

			Update B Set B.IsPublished  =1 , PublishStateId = DBO.Fn_GetPublishStateIdForPreview()  
			from ZnodeCmsPortalMessage A 
			Inner join ZnodeCmsMessage B ON A.CMSMessageId = B.CMSMessageId 
			inner join ZnodeCmsMessagekey C   On A.CMSMessageKeyId = C.CMSMessageKeyId 
			where (C.CMSMessageKeyId = @CMSMessageKeyId OR @CMSMessageKeyId  = 0 ) 
			AND (A.PortalId = @PortalId OR @PortalId= 0 )

		End
		
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
		
		if (@PortalId > 0 ) 
		Begin
			-- Only production version id will process 
			Delete from ZnodePublishMessageEntity where PortalId = @PortalId  
			and MessageKey in (select MessageKey from @TBL_CMSMessageDataFinal) 
			and VersionId in (Select ProductionVersionId from @TBL_ProductionVersionId ) 
		
			Insert Into ZnodePublishMessageEntity 
				(VersionId,PublishStartTime,LocaleId,PortalId,MessageKey,Message,Area)
				SELECT B.ProductionVersionId , @GetDate, A.LocaleId,@PortalId,MessageKey,Message,AreaName
				FROM @TBL_CMSMessageDataFinal A inner join @Tbl_ProductionVersionId B on 
				@PortalId = B.PortalId and  A.LocaleId = B.LocaleId		

			Update B Set B.IsPublished  =1 , PublishStateId = DBO.Fn_GetPublishStateIdForPublish()  
			from ZnodeCmsPortalMessage A 
			Inner join ZnodeCmsMessage B ON A.CMSMessageId = B.CMSMessageId 
			inner join ZnodeCmsMessagekey C   On A.CMSMessageKeyId = C.CMSMessageKeyId 
			where (C.CMSMessageKeyId = @CMSMessageKeyId OR @CMSMessageKeyId  = 0 ) 
			AND (A.PortalId = @PortalId OR @PortalId= 0 )
		End 
		else 
		If @PortalId  = 0 
		Begin
				--ZnodePublishGlobalVersionEntity
			Insert into  ZnodePublishGlobalVersionEntity (VersionId,PublishStartTime,PublishState,LocaleId)
			select A.VersionId, @GetDate,A.Publishstate, A.LocaleId from @GlobalVersion A  where  A.PublishState = 'PRODUCTION'
			and Not exists (Select TOP 1 1 from ZnodePublishGlobalVersionEntity B where B.LocaleId = A.LocaleId
			and B.PublishState = 'PRODUCTION') 

			--Data inserted into flat table ZnodePublishMessageEntity (Replica of MongoDB Collection )  
			Delete from ZnodePublishGlobalMessageEntity where VersionId in (Select VersionId from  ZnodePublishGlobalVersionEntity
			where PublishState = 'PRODUCTION' )  
			AND (
			(Exists (Select TOP 1 1 from @TBL_CMSMessageDataFinal CMD where CMD.MessageKey = ZnodePublishGlobalMessageEntity.MessageKey))
			OR @CMSMessageKeyId = 0	)

			Insert Into ZnodePublishGlobalMessageEntity 
			(VersionId,PublishStartTime,LocaleId,MessageKey,Message,Area)
			SELECT B.VersionId , @GetDate, A.LocaleId,MessageKey,Message,AreaName
			FROM @TBL_CMSMessageDataFinal A inner join ZnodePublishGlobalVersionEntity B on 
			A.LocaleId = B.LocaleId AND B.PublishState = 'PRODUCTION'

			Update B Set B.IsPublished  =1 , PublishStateId = DBO.Fn_GetPublishStateIdForPublish()  
			from ZnodeCmsPortalMessage A 
			Inner join ZnodeCmsMessage B ON A.CMSMessageId = B.CMSMessageId 
			inner join ZnodeCmsMessagekey C   On A.CMSMessageKeyId = C.CMSMessageKeyId 
			where (C.CMSMessageKeyId = @CMSMessageKeyId OR @CMSMessageKeyId  = 0 ) 
			AND (A.PortalId = @PortalId OR @PortalId= 0 )


		End
		
	End
		
	SET @Status = 1 
	If @CMSMessageKeyId > 0 
	SELECT 0 AS ID,CAST(1 AS BIT) AS Status;   
END TRY 
BEGIN CATCH 
	SET @Status = 0  
 
	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishMessageEntity 
			@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
			+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
			+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
			+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		    +',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	
	If @CMSMessageKeyId > 0 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishMessageEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END