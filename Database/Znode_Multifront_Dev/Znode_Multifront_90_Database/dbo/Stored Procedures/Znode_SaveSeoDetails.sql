CREATE PROCEDURE [dbo].[Znode_SaveSeoDetails]
(
	@CMSSEODetailId INT = 0,
	@CMSSEOTypeId INT = 0,
	@IsAllStore BIT = 0,
	@LocaleId INT = 0,
	@PimProductId INT = 0,
	@PortalId INT = 0,
	@SEODescription NVARCHAR(max) = null,
	@SEOId INT = 0,
	@SEOKeywords NVARCHAR(MAX) = NULL,
	@SEOTitle NVARCHAR(MAX) = NULL,
	@SEOUrl NVARCHAR(MAX)= NULL,
	@IsRedirect BIT = NULL,
	@IsPublish BIT = NULL,
	@CreatedBy NVARCHAR(200)=null,
	@ModifiedBy NVARCHAR(200) = null,
	@CanonicalURL VARCHAR(200) = null,
	@RobotTag VARCHAR(50) = null
)
AS 
BEGIN
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
IF(@IsAllStore = 'false')
BEGIN
IF NOT EXISTS(SELECT 1 FROM ZnodeCMSSEODetail WHERE CMSSEODetailId =@CMSSEODetailId)
BEGIN
	INSERT INTO ZnodeCMSSEODetail(
									CMSSEOTypeId,
									SEOId,
									IsRedirect,
									PortalId,
									SEOUrl,
									CreatedBy,
									CreatedDate,
									IsPublish
								)
					VALUES
								(
									@CMSSEOTypeId,
									@SEOId,
									@IsRedirect,
									@PortalId,
									@SEOUrl,
									@CreatedBy,
									@GetDate,
									@IsPublish
								)
							SET @CMSSEODetailId	=(SELECT SCOPE_IDENTITY())
END
ELSE
BEGIN
			UPDATE ZnodeCMSSEODetail SET
									CMSSEOTypeId=@CMSSEOTypeId,
									SEOId=@SEOId,
									IsRedirect=@IsRedirect,
									PortalId=@PortalId,
									SEOUrl=@SEOUrl,
									ModifiedBy=@ModifiedBy,
									ModifiedDate=@GetDate,
									IsPublish= @IsPublish
				WHERE CMSSEODetailId= @CMSSEODetailId
									
END
IF NOT EXISTS(SELECT 1 FROM ZnodeCMSSEODetailLocale WHERE CMSSEODetailId =@CMSSEODetailId AND LocaleId =@LocaleId)
BEGIN
	INSERT INTO [dbo].[ZnodeCMSSEODetailLocale]
           ([CMSSEODetailId]
           ,[LocaleId]
           ,[SEOTitle]
           ,[SEODescription]
           ,[SEOKeywords]
           ,[CreatedBy]
           ,[CreatedDate]
		   ,CanonicalURL
		   ,RobotTag)
     VALUES
           (
			@CMSSEODetailId
           ,@LocaleId
           ,@SEOTitle
           ,@SEODescription
           ,@SEOKeywords
           ,@CreatedBy
           ,@GetDate
		   ,@CanonicalURL
		   ,@RobotTag)
END
ELSE
begin
	UPDATE [dbo].[ZnodeCMSSEODetailLocale]
   SET [CMSSEODetailId] = @CMSSEODetailId
      ,[LocaleId] = @LocaleId
      ,[SEOTitle] = @SEOTitle
      ,[SEODescription] = @SEODescription
      ,[SEOKeywords] = @SEOKeywords
      ,[ModifiedBy] = @ModifiedBy
      ,[ModifiedDate] = @GetDate
	  ,CanonicalURL = @CanonicalURL
	  ,RobotTag = @RobotTag
 WHERE CMSSEODetailId =@CMSSEODetailId AND LocaleId=@LocaleId
END
END
ELSE
BEGIN
DECLARE @PortalIds table
(
    RowId				INT IDENTITY(1,1), 
    PortalId			INT
)

INSERT INTO @PortalIds (PortalId) SELECT PortalId FROM ZnodePortal
--*/

DECLARE @maxRowPortalId INT= (SELECT MAX(RowId) FROM @PortalIds)
DECLARE @RowPortalId int = 1

WHILE @RowPortalId <= @maxRowPortalId
BEGIN

    SET @PortalId = (SELECT PortalId FROM @PortalIds WHERE RowId = @RowPortalId)

	DECLARE @CMSSEODetailIds table
	(
		RowId				INT IDENTITY(1,1), 
		CMSSEODetailId			INT
	)
	INSERT INTO @CMSSEODetailIds (CMSSEODetailId) SELECT CMSSEODetailId FROM ZnodeCMSSEODetail where PortalId=@PortalId and SEOId =@SEOId
	DECLARE @maxRowSEODetailId INT= (SELECT MAX(RowId) FROM @CMSSEODetailIds)
	DECLARE @RowSEODetailId int = 1
	WHILE @RowSEODetailId <= @maxRowSEODetailId
	BEGIN
		SET @CMSSEODetailId  =(SELECT CMSSEODetailId FROM @CMSSEODetailIds WHERE RowId = @RowSEODetailId)
		IF NOT EXISTS(SELECT 1 FROM ZnodeCMSSEODetail WHERE CMSSEODetailId =@CMSSEODetailId)
BEGIN
	INSERT INTO ZnodeCMSSEODetail(
									CMSSEOTypeId,
									SEOId,
									IsRedirect,
									PortalId,
									SEOUrl,
									CreatedBy,
									CreatedDate,
									IsPublish
								)
					VALUES
								(
									@CMSSEOTypeId,
									@SEOId,
									@IsRedirect,
									@PortalId,
									@SEOUrl,
									@CreatedBy,
									@GetDate,
									@IsPublish
								)
							SET @CMSSEODetailId	=(SELECT SCOPE_IDENTITY())
END
ELSE
BEGIN
			UPDATE ZnodeCMSSEODetail SET
									CMSSEOTypeId=@CMSSEOTypeId,
									SEOId=@SEOId,
									IsRedirect=@IsRedirect,
									PortalId=@PortalId,
									SEOUrl=@SEOUrl,
									ModifiedBy=@ModifiedBy,
									ModifiedDate=@GetDate,
									IsPublish= @IsPublish
				WHERE CMSSEODetailId= @CMSSEODetailId
									
END
		IF NOT EXISTS(SELECT 1 FROM ZnodeCMSSEODetailLocale WHERE CMSSEODetailId =@CMSSEODetailId AND LocaleId =@LocaleId)
BEGIN
	INSERT INTO [dbo].[ZnodeCMSSEODetailLocale]
           ([CMSSEODetailId]
           ,[LocaleId]
           ,[SEOTitle]
           ,[SEODescription]
           ,[SEOKeywords]
           ,[CreatedBy]
           ,[CreatedDate]
		   ,CanonicalURL
		   ,RobotTag)
     VALUES
           (
			@CMSSEODetailId
           ,@LocaleId
           ,@SEOTitle
           ,@SEODescription
           ,@SEOKeywords
           ,@CreatedBy
           ,@GetDate
		   ,@CanonicalURL
		   ,@RobotTag)
END
ELSE
begin
	UPDATE [dbo].[ZnodeCMSSEODetailLocale]
   SET [CMSSEODetailId] = @CMSSEODetailId
      ,[LocaleId] = @LocaleId
      ,[SEOTitle] = @SEOTitle
      ,[SEODescription] = @SEODescription
      ,[SEOKeywords] = @SEOKeywords
      ,[ModifiedBy] = @ModifiedBy
      ,[ModifiedDate] = @GetDate
	  ,CanonicalURL = @CanonicalURL
	  ,RobotTag= @RobotTag
 WHERE CMSSEODetailId =@CMSSEODetailId AND LocaleId=@LocaleId
END
		SET @RowSEODetailId =@RowSEODetailId +1
	END

    SELECT @RowPortalId = @RowPortalId + 1
End
END
END