CREATE PROCEDURE [dbo].[Znode_SetPublishProductPageEntity]
(
   @PortalId  INT = 0
  ,@IsPreviewEnable int = 0 
  ,@PreviewVersionId INT = 0 
  ,@ProductionVersionId INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@UserId int = 0 
  ,@Status int  Output
)
AS
/*
    This Procedure is used to publish the blog news against the store 
  
	EXEC ZnodeSetPublishProductPageEntity 1 2,3
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
	
	
 Exec [ZnodeSetPublishProductPageEntity]
   @PortalId  = 1 
  ,@PreviewVersionId = 0 
  ,@ProductionVersionId = 0 
  ,@RevisionState = 'Preview&Production' 
  ,@CMSMappingId  =89 
  ,@UserId = 0 

  
 Exec [ZnodeSetPublishProductPageEntity]
   @PortalId  = 1 
  ,@PreviewVersionId = 0 
  ,@ProductionVersionId = 0 
  ,@RevisionState = 'Preview&Production' 
  ,@CMSMappingId  =0 
  ,@UserId = 0 



	
	*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
   Begin 
		DECLARE @Tbl_PreviewVersionId    TABLE    (PreviewVersionId int , PortalId int , LocaleId int)
		DECLARE @Tbl_ProductionVersionId TABLE    (ProductionVersionId int  , PortalId int , LocaleId int)

		If @PreviewVersionId = 0 
			Begin
   				Insert into @Tbl_PreviewVersionId 
				SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and PublishState ='PREVIEW'
			end
		Else 
				Insert into @Tbl_PreviewVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
				where VersionId = @PreviewVersionId
		If @ProductionVersionId = 0 
   			Begin
				Insert into @Tbl_ProductionVersionId 
				SELECT distinct VersionId , PortalId , LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and PublishState ='PRODUCTION'
			End 
		Else 
			Insert into @Tbl_ProductionVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
			where VersionId = @ProductionVersionId

		Declare @ZnodeCMSPortalProductPage Table (ProductPageId	int,PortalId	int,ProductType	varchar(500) ,TemplateName	varchar(500))

		INSERT INTO @ZnodeCMSPortalProductPage
					(ProductPageId,PortalId,ProductType,TemplateName)
					 select CMSPortalProductPageId,PortalId,ProductType,TemplateName from ZnodeCMSPortalProductPage 
					 where PortalId = @PortalId OR @PortalId = 0 
					 

	End 
    --select * from @TBL_ContentPage 
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
	Begin
	    --Data inserted into flat table ZnodePublishProductPageEntity (Replica of MongoDB Collection )  
		Delete from ZnodePublishProductPageEntity where PortalId = @PortalId  and VersionId in (Select PreviewVersionId from @Tbl_PreviewVersionId  ) 
		
		Insert Into ZnodePublishProductPageEntity 
		(VersionId,PublishStartTime,ProductPageId,PortalId,ProductType,TemplateName)
		
		SELECT B.PreviewVersionId , @GetDate, ProductPageId,A.PortalId,ProductType,TemplateName
		FROM @ZnodeCMSPortalProductPage A inner join @Tbl_PreviewVersionId B on A.PortalId = B.PortalId
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
			-- Only production version id will process 
		Delete from ZnodePublishProductPageEntity where PortalId = @PortalId  and VersionId in (select ProductionVersionId from  @TBL_ProductionVersionId ) 
		Insert Into ZnodePublishProductPageEntity 
		(VersionId,PublishStartTime,ProductPageId,PortalId,ProductType,TemplateName)
		
		SELECT B.ProductionVersionId , @GetDate, ProductPageId,A.PortalId,ProductType,TemplateName
		FROM @ZnodeCMSPortalProductPage A inner join @Tbl_ProductionVersionId B on A.PortalId = B.PortalId
	End
	SET @Status = 1;
END TRY 
BEGIN CATCH 
	
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeSetPublishProductPageEntity 
		@PortalId = '+CAST(@PortalId AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
	+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
	+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
	+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
	+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeSetPublishProductPageEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

END CATCH
END