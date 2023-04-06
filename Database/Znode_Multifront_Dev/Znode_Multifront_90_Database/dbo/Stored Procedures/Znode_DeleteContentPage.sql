CREATE PROCEDURE [dbo].[Znode_DeleteContentPage]
( 
	@CMSContentPageId VARCHAR(2000),
	@Status           BIT OUT
)
AS 
  /*  
     Summary : Remove content page details with their referance data 
			   Here complete delete the ContentPages and their references without any check  
			   If passed @CMSContentPageIds are matched with deleted count then data set return true other wise false 
			   dbo.Split function use to make comma separeted data in table rows 
			   1 ZnodeCMSContentPagesProfile
			   2 ZnodeCMSContentPagesLocale
			   3 ZnodeCMSContentPageGroupMapping
			   4 ZnodeCMSSEODetail
			   5 ZnodeCMSContentPages
     Unit Testing 
	 begin tran
     DEclare @Status bit 
     EXEC Znode_DeleteContentPage  29 ,@Status =@Status OUT 
	 rollback tran
       
    */
	 BEGIN
         BEGIN TRAN DeleteContentPage;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TBL_DeleteContentPage TABLE(CMSContentPagesId INT,PageName NVARCHAR(200),PortalId INT );  -- table holds the CMSContentPagesId id 
             INSERT INTO @TBL_DeleteContentPage
                    SELECT a.CMSContentPagesId,a.PageName, a.PortalId
                    FROM [dbo].[ZnodeCMSContentPages] AS a
                         INNER JOIN dbo.Split(@CMSContentPageId, ',') AS b ON(a.CMSContentPagesId = b.Item); -- dbo.Split function use to make ',' separeted data in table rows 
             DELETE FROM ZnodeCMSContentPagesProfile
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.CMSContentPagesId = ZnodeCMSContentPagesProfile.CMSContentPagesId
             );
             DELETE FROM ZnodeCMSContentPagesLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.CMSContentPagesId = ZnodeCMSContentPagesLocale.CMSContentPagesId
             );
             DELETE FROM ZnodeCMSContentPageGroupMapping
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.CMSContentPagesId = ZnodeCMSContentPageGroupMapping.CMSContentPagesId
             );
             DELETE FROM ZnodeCMSWidgetCategory
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.CMSContentPagesId = ZnodeCMSWidgetCategory.CMSMappingId
                       AND ZnodeCMSWidgetCategory.TypeOFMapping = 'ContentPageMapping'
             );
             DELETE FROM ZnodeCMSWidgetProduct
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.CMSContentPagesId = ZnodeCMSWidgetProduct.CMSMappingId
                       AND ZnodeCMSWidgetProduct.TypeOFMapping = 'ContentPageMapping'
             );
             DELETE FROM ZnodeCMSWidgetSliderBanner
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.CMSContentPagesId = ZnodeCMSWidgetSliderBanner.CMSMappingId
                       AND ZnodeCMSWidgetSliderBanner.TypeOFMapping = 'ContentPageMapping'
             );

			 DELETE FROM ZnodeCMSWidgetTitleConfigurationLocale
			 WHERE EXISTS
			 ( 
				select * FROM ZnodeCMSWidgetTitleConfiguration
				WHERE EXISTS
				(
					SELECT TOP 1 1
					FROM @TBL_DeleteContentPage AS TBDCP
					WHERE TBDCP.CMSContentPagesId = ZnodeCMSWidgetTitleConfiguration.CMSMappingId
						AND ZnodeCMSWidgetTitleConfiguration.TypeOFMapping = 'ContentPageMapping'
				) 
				AND ZnodeCMSWidgetTitleConfigurationLocale.CMSWidgetTitleConfigurationId = ZnodeCMSWidgetTitleConfiguration.CMSWidgetTitleConfigurationId
			 )

             DELETE FROM ZnodeCMSWidgetTitleConfiguration
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.CMSContentPagesId = ZnodeCMSWidgetTitleConfiguration.CMSMappingId
                       AND ZnodeCMSWidgetTitleConfiguration.TypeOFMapping = 'ContentPageMapping'
             );
             DELETE FROM ZnodeCMSSEODetailLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeCMSSEODetail
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @TBL_DeleteContentPage AS TBDCP
                     WHERE TBDCP.PageName = ZnodeCMSSEODetail.SEOCode
					 AND TBDCP.PortalId = ZnodeCMSSEODetail.PortalId
                 )
                       AND ZnodeCMSSEODetail.CMSSEOTypeId IN
                 (
                     SELECT CMSSEOTypeId
                     FROM ZnodeCMSSEOType
                     WHERE NAME = 'Content Page'
                 )
                       AND ZnodeCMSSEODetail.CMSSEODetailId = ZnodeCMSSEODetailLocale.CMSSEODetailId
             );
             DELETE FROM ZnodeCMSSEODetail
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.PageName = ZnodeCMSSEODetail.SEOCode
				 AND TBDCP.PortalId = ZnodeCMSSEODetail.PortalId
             )
                   AND ZnodeCMSSEODetail.CMSSEOTypeId IN
             (
                 SELECT CMSSEOTypeId
                 FROM ZnodeCMSSEOType
                 WHERE NAME = 'Content Page'
             );
             DELETE FROM ZnodeCMSContentPagesLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.CMSContentPagesId = ZnodeCMSContentPagesLocale.CMSContentPagesId
             );
             DELETE FROM ZnodeFormWidgetEmailConfiguration
            WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM @TBL_DeleteContentPage AS TBDCP
                WHERE TBDCP.CMSContentPagesId = ZnodeFormWidgetEmailConfiguration.CMSContentPagesId
            );
             DELETE FROM ZnodeCMSContentPages
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS TBDCP
                 WHERE TBDCP.CMSContentPagesId = ZnodeCMSContentPages.CMSContentPagesId
             );
             IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeleteContentPage
             ) =
             (   -- if count are equal then  dataset status are return true other wise false 
                 SELECT COUNT(1)
                 FROM dbo.Split(@CMSContentPageId, ',')
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
             COMMIT TRAN DeleteContentPage;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteContentPage @CMSContentPageId = '+@CMSContentPageId+',@Status='+CAST(@Status AS VARCHAR(50));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS [Status];
             ROLLBACK TRAN DeleteContentPage;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteContentPage',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;