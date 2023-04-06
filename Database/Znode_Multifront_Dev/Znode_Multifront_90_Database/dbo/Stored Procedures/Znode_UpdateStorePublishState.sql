CREATE PROC [dbo].[Znode_UpdateStorePublishState] 
(
  @PortalId INT = 0
 ,@LocaleId INT = 0 
 ,@CurrentPublishStateId VARCHAR(2000) = '0' 
 ,@PublishStateId TINYINT = 0
 ,@Status BIT OUT

)
AS 
BEGIN 
   SET NOCOUNT ON 
   BEGIN TRY 
   BEGIN TRAN 	UpdateStorePublishState

   DECLARE @CurrentPublishStateIds TABLE (PublishStateId INT ) 
  
   INSERT INTO @CurrentPublishStateIds (PublishStateId)
   SELECT Item
   FROM dbo.Split(@CurrentPublishStateId,',')SP

   UPDATE ZnodeCMSContentPages
   SET PublishStateId = @PublishStateId
   WHERE EXISTS ( SELECT TOP 1 1 FROM @CurrentPublishStateIds TY WHERE  TY.PublishStateId = ZnodeCMSContentPages.PublishStateId) 
   AND PortalId = @PortalId

   UPDATE a  
   SET PublishStateId = @PublishStateId
   FROM ZnodeCMSMessage a 
   INNER JOIN ZnodeCMSPortalMessage b ON (b.CmsMessageId =a.CmsMessageId )
   WHERE EXISTS ( SELECT TOP 1 1 FROM @CurrentPublishStateIds TY WHERE  TY.PublishStateId = a.PublishStateId) 
   AND b.PortalId = @PortalId

   UPDATE ZnodeCMSSEODetail 
   SET PublishStateId = @PublishStateId
   WHERE  EXISTS ( SELECT TOP 1 1 FROM @CurrentPublishStateIds TY WHERE  TY.PublishStateId = ZnodeCMSSEODetail.PublishStateId) 
   AND CMSSEOTypeId IN (SELECT CMSSEOTypeId FROM ZnodeCMSSEOType WHERE name IN ('Content Page','BlogNews'))
   AND PortalId = @PortalId
   
   UPDATE a
   SET PublishStateId = @PublishStateId
   FROM ZnodeCMSSlider a 
   INNER JOIN ZnodeCMSWidgetSliderBanner b ON (b.CMSSliderId =a.CMSSliderId )
   WHERE  EXISTS ( SELECT TOP 1 1 FROM @CurrentPublishStateIds TY WHERE  TY.PublishStateId = a.PublishStateId) 
   AND b.CMSMappingId = @PortalId
   AND b.TypeOfMapping = 'PortalMapping'
   	 
   --UPDATE ZnodePublishPortalLog
   --SET PublishStateId = @PublishStateId
   --WHERE  PublishStateId = @CurrentPublishStateId 
   --AND PortalId = @PortalId

   SET @Status = 1 
   SELECT 1 Id , CAST(1 AS BIT ) Status
   COMMIT  TRAN 	UpdateStorePublishState 
   END TRY 
   BEGIN CATCH 
      SELECT 1 Id , CAST(0 AS BIT ) Status
	   SET @Status = 1 
	  SELECT ERROR_MESSAGE()
	  ROLLBACK TRAN UpdateStorePublishState 
   END CATCH 

END