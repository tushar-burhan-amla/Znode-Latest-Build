﻿CREATE PROCEDURE [dbo].[Znode_SetPublishTextWidgetEntity]
(
   @PortalId  INT = 0 
  ,@LocaleId  INT = 0 
  ,@PreviewVersionId INT = 0 
  ,@IsPreviewEnable int = 0 
  ,@ProductionVersionId INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@CMSMappingId INT =0
  ,@UserId int = 0 
  ,@Status int = 0 Output 
)
AS
/*
    This Procedure is used to publish the blog news against the store 
  
	EXEC ZnodeSetPublishTextWidgetEntity 1 2,3
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
	
	 Exec [ZnodeSetPublishTextWidgetEntity]
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
		
		DECLARE @CMSWidgetData TABLE (CMSTextWidgetConfigurationId INT ,LocaleId  INT ,CMSWidgetsId INT ,WidgetsKey NVARCHAR(256) ,CMSMappingId  INT ,TypeOFMapping   NVARCHAR(100) ,[Text]  NVARCHAR(MAX));
		
		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )

		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN
			    SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
				
				if (@CMSMappingId > 0 )
				Begin  
					;With Cte_GetCMSSEODetails AS 
					(
						SELECT CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
						FROM ZnodeCMSTextWidgetConfiguration AS a
						WHERE 
						(a.TypeOFMapping = 'ContentPageMapping'
						AND (EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSContentPages  WHERE IsActive =1 AND a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  ))
						AND (a.LocaleId = @SetLocaleId OR a.LocaleId = @DefaultLocaleId) )
						AND (a.CMSMappingId = @CMSMappingId OR @CMSMappingId = 0 )

					)
					, Cte_GetFirstCMSSEODetails  AS
					(
						SELECT 
							 CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
						FROM Cte_GetCMSSEODetails 
						WHERE LocaleId = @SetLocaleId
					)
					, Cte_GetDefaultFilterData AS
					(
						SELECT 
							CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
						FROM  Cte_GetFirstCMSSEODetails 
						UNION ALL 
						SELECT 
							 CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
						FROM Cte_GetCMSSEODetails p
						WHERE LocaleId = @DefaultLocaleId 
						AND NOT EXISTS ( SELECT TOP 1 1 FROM Cte_GetFirstCMSSEODetails AS q WHERE q.CMSWidgetsId = p.CMSWidgetsId
						AND q.WidgetsKey = p.WidgetsKey AND q.TypeOFMapping = p.TypeOFMapping AND q.CMSMappingId = p.CMSMappingId  )
					)
					insert into @CMSWidgetData(CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text])
					SELECT CMSTextWidgetConfigurationId , @SetLocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]	FROM Cte_GetDefaultFilterData  A 
				End 
				Else if (@CMSMappingId = 0 )
				Begin
					;With Cte_GetCMSSEODetails AS 
					(
						SELECT CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
						FROM ZnodeCMSTextWidgetConfiguration AS a
						WHERE 
						(a.TypeOFMapping = 'ContentPageMapping'
						AND (EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSContentPages  WHERE  IsActive =1 AND  a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  ))
						OR (a.TypeOFMapping = 'PortalMapping' AND a.CMSMappingId = @PortalId )
						AND (a.LocaleId = @SetLocaleId OR a.LocaleId = @DefaultLocaleId) )
					)
					, Cte_GetFirstCMSSEODetails  AS
					(
						SELECT 
							 CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
						FROM Cte_GetCMSSEODetails 
						WHERE LocaleId = @SetLocaleId
					)
					, Cte_GetDefaultFilterData AS
					(
						SELECT 
							CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
						FROM  Cte_GetFirstCMSSEODetails 
						UNION ALL 
						SELECT 
							 CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
						FROM Cte_GetCMSSEODetails p
						WHERE LocaleId = @DefaultLocaleId 
						AND NOT EXISTS ( SELECT TOP 1 1 FROM Cte_GetFirstCMSSEODetails AS q WHERE q.CMSWidgetsId = p.CMSWidgetsId
						AND q.WidgetsKey = p.WidgetsKey AND q.TypeOFMapping = p.TypeOFMapping AND q.CMSMappingId = p.CMSMappingId  )
					)
					insert into @CMSWidgetData(CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text])
					SELECT CMSTextWidgetConfigurationId , @SetLocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]	FROM Cte_GetDefaultFilterData  A 
				End

				SET @IncrementalId = @IncrementalId + 1;
				--DELETE FROM @CMSWidgetData;
				--DELETE FROM @CMSWidgetDataFinal;
        END;
	
	End

    --select * from @TBL_ContentPage 
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%' ) 
	Begin
	    --Data inserted into flat table ZnodePublishTextWidgetEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishTextWidgetEntity where PortalId = @PortalId  and VersionId in (Select PreviewVersionId from  @TBL_PreviewVersionId) and
		(MappingId = @CMSMappingId OR @CMSMappingId = 0 )

		Insert Into ZnodePublishTextWidgetEntity 
		(VersionId,PublishStartTime,TextWidgetConfigurationId,MappingId,PortalId,TypeOFMapping,LocaleId,WidgetsKey,Text)
		SELECT B.PreviewVersionId , @GetDate, CMSTextWidgetConfigurationId ,CMSMappingId,@PortalId, TypeOFMapping,A.LocaleId,WidgetsKey,[Text]
		FROM @CMSWidgetData A inner join @Tbl_PreviewVersionId B on @PortalId = B.PortalId and  A.LocaleId = B.LocaleId
		
		
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishTextWidgetEntity where PortalId = @PortalId  and VersionId in (Select ProductionVersionId from @TBL_ProductionVersionId ) and 
		(MappingId = @CMSMappingId OR @CMSMappingId = 0 )
		
		Insert Into ZnodePublishTextWidgetEntity 
		(
			VersionId,PublishStartTime,TextWidgetConfigurationId,MappingId,PortalId,TypeOFMapping,LocaleId,WidgetsKey,Text
		)
		SELECT B.ProductionVersionId, @GetDate, CMSTextWidgetConfigurationId ,CMSMappingId,@PortalId, TypeOFMapping,A.LocaleId,WidgetsKey,[Text]
		FROM @CMSWidgetData A inner join @Tbl_ProductionVersionId B on @PortalId = B.PortalId and  A.LocaleId = B.LocaleId
		
	End
	SET @Status = 1 
END TRY 
BEGIN CATCH 
	SET @Status = 0  
 
	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishTextWidgetEntity 
			@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
			+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
			+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
			+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
			+''',@CMSMappingId= ' + CAST(@CMSMappingId  AS varchar(20))
			+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		                			 

         			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishTextWidgetEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END