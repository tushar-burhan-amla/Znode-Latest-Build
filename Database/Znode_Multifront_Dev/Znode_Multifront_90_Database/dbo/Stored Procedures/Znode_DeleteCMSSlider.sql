CREATE PROCEDURE [dbo].[Znode_DeleteCMSSlider]
( @CMSSliderId VARCHAR(MAX),
  @Status      BIT OUT)
AS 
 /*  
     Summary:Remove CMS Slider with details
			 Before delete check is not associated with ZnodeCMSWidgetSliderBanner 
			 output dataset contain the status if passed @CMSSliderId all ids are deleted then this will true other wise false 
			 Delete table sequence 
			 1.[ZnodeCMSSliderBanner]
			 2.[ZnodeCMSSlider]
     Unit Testing  
			 begin tran 
			 EXEC [Znode_DeletePimCatalog] 7,0
			 rollback tran
   */
     BEGIN
         BEGIN TRAN DeleteCMSSlider;
         BEGIN TRY
             SET NOCOUNT ON;
			 -- table hold the CMSSliderId 
             DECLARE @TBL_CMSSliderIds TABLE(CMSSliderId INT);
             INSERT INTO @TBL_CMSSliderIds
                    SELECT item
                    FROM dbo.Split(@CMSSliderId, ',');

			DELETE FROM ZnodeCMSWidgetSliderBanner
			WHERE EXISTS
			(
				 SELECT TOP 1 1
                 FROM @TBL_CMSSliderIds AS TBDCS
                 WHERE TBDCS.CMSSliderId = ZnodeCMSWidgetSliderBanner.CMSSliderId
			);

             DECLARE @TBL_DeleteCMSSliderId TABLE(CMSSliderId INT);
             INSERT INTO @TBL_DeleteCMSSliderId
                    SELECT ZCS.CMSSliderId
                    FROM [dbo].ZnodeCMSSlider AS ZCS
                         INNER JOIN @TBL_CMSSliderIds AS TBCS ON(ZCS.CMSSliderId = TBCS.CMSSliderId)
                    WHERE NOT EXISTS
                    (   -- check CMSSliderId is not exists in ZnodeCMSWidgetSliderBanner
                        SELECT TOP 1 1
                        FROM ZnodeCMSWidgetSliderBanner AS ZCWSB
                        WHERE ZCWSB.CMSSliderId = ZCS.CMSSliderId
                    ); 

             DELETE FROM ZnodeCMSSliderBannerLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeCMSSliderBanner
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @TBL_DeleteCMSSliderId AS TBDCS
                     WHERE TBDCS.CMSSliderId = ZnodeCMSSliderBanner.CMSSliderId
                 )
                       AND ZnodeCMSSliderBanner.CMSSliderBannerId = ZnodeCMSSliderBannerLocale.CMSSliderBannerId
             );
             DELETE FROM ZnodeCMSSliderBanner
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteCMSSliderId AS TBDCS
                 WHERE TBDCS.CMSSliderId = ZnodeCMSSliderBanner.CMSSliderId
             );
             DELETE FROM ZnodeCMSSlider
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteCMSSliderId AS TBDCS
                 WHERE TBDCS.CMSSliderId = ZnodeCMSSlider.CMSSliderId
             );
             IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeleteCMSSliderId
             ) =
             (   -- if count of both query are equal then dataset status true other wise false
                 SELECT COUNT(1)
                 FROM @TBL_CMSSliderIds
             )  
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS [Status];  
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS [Status];
                     SET @Status = 0;
                 END;
             COMMIT TRAN DeleteCMSSlider;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteCMSSlider @CMSSliderId = '+CAST(@CMSSliderId AS VARCHAR(100))+',@Status='+CAST(@Status AS VARCHAR(50));
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS [Status];
             SET @Status = 0;
			 ROLLBACK TRAN DeleteCMSSlider;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteCMSSlider',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
             ROLLBACK TRAN DeleteCMSSlider;
         END CATCH;
     END;
