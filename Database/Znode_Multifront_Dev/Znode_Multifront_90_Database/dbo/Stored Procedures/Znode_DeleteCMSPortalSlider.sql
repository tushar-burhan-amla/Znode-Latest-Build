-- EXEC [Znode_DeletePimCatalog] 1
-- SELECT * FROM ZnodeMediaAttributeGroup
CREATE  Procedure [dbo].[Znode_DeleteCMSPortalSlider] 
(
@CMSPortalSliderId   VARCHAR(500)
,@Status                    BIT OUT 
)
AS 
BEGIN 
BEGIN TRAN 
BEGIN TRY 
SET NOCOUNT ON 
     

	 DECLARE @CMSPortalSliderIds TABLE (CMSPortalSliderId INT )
	 INSERT INTO @CMSPortalSliderIds
	 SELECT item FROM dbo.Split(@CMSPortalSliderId,',')  

	 DECLARE @DeleteCMSPortalSliderId TABLE (CMSPortalSliderId INT )
	 INSERT INTO @DeleteCMSPortalSliderId
	 SELECT a.CMSPortalSliderId 
	 FROM [dbo].ZnodeCMSPortalSlider a 
	 INNER JOIN @CMSPortalSliderIds b ON (a.CMSPortalSliderId = b.CMSPortalSliderId ) 
	 WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSWidgetConfiguration q WHERE q.CMSPortalSliderId = a.CMSPortalSliderId )

	 DELETE FROM ZnodeCMSSliderBanner		  WHERE EXISTS (SELECT TOP 1 1 FROM @DELETECMSPORTALSliderid w WHERE w.CMSPortalSliderId= ZnodeCMSSliderBanner.CMSPortalSliderId	   )
	-- DELETE FROM ZnodeCMSWidgetConfiguration  WHERE EXISTS (SELECT TOP 1 1 FROM @DELETECMSPORTALSliderid w WHERE w.CMSPortalSliderId= ZnodeCMSWidgetConfiguration.CMSPortalSliderId)
     DELETE FROM ZnodeCMSPortalSlider         WHERE EXISTS (SELECT TOP 1 1 FROM @DELETECMSPORTALSliderid w WHERE w.CMSPortalSliderId= ZnodeCMSPortalSlider.CMSPortalSliderId       )

  SET @Status  = 1 
  IF (SELECT COUNT (1) FROM @DeleteCMSPortalSliderId) = (SELECT COUNT (1) FROM @CMSPortalSliderIds)
  BEGIN 
  SELECT 1 ID , CAST(1 AS BIT ) Status
  END 
  ELSE 
  BEGIN 
  SELECT 0 ID , CAST(0 AS BIT ) Status
  END 
     
COMMIT TRAN 
END TRY 

BEGIN CATCH

    SET @Status  = 0 
    SELECT 0 ID , CAST(0 AS BIT ) Status 
	
	--SELECT ERROR_MESSAGE(),ERROR_LINE()
	ROLLBACK TRAN 
 
END CATCH 


END