
CREATE PROCEDURE [dbo].[Znode_DeleteAllPublishedData]
( @UserId INT,
  @PimCatalogId INT = 0
)
AS 
/*Delete published data of all catalog, portal and cms*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 BEGIN TRAN A
            
			--Truncate all catalog publish related entity
			TRUNCATE TABLE ZnodePublishCatalogEntity

			TRUNCATE TABLE ZnodePublishCategoryEntity
			 
			TRUNCATE TABLE ZnodePublishProductEntity

			TRUNCATE TABLE ZnodePublishAddonEntity

			TRUNCATE TABLE ZnodePublishBundleProductEntity

			TRUNCATE TABLE ZnodePublishConfigurableProductEntity

			TRUNCATE TABLE ZnodePublishGroupProductEntity

			TRUNCATE TABLE ZnodePublishCatalogAttributeEntity

			TRUNCATE TABLE ZnodePublishSeoEntity

			TRUNCATE TABLE ZnodePublishVersionEntity

			TRUNCATE TABLE ZnodePublishProgressNotifierEntity


			--Truncate all portal publish related entity
			TRUNCATE TABLE ZnodePublishWebStoreEntity

			TRUNCATE TABLE ZnodePublishBlogNewsEntity

			TRUNCATE TABLE ZnodePublishPortalCustomCssEntity

			TRUNCATE TABLE ZnodePublishWidgetCategoryEntity

			TRUNCATE TABLE ZnodePublishWidgetProductEntity

			TRUNCATE TABLE ZnodePublishWidgetTitleEntity

			TRUNCATE TABLE ZnodePublishWidgetSliderBannerEntity

			TRUNCATE TABLE ZnodePublishTextWidgetEntity

			TRUNCATE TABLE ZnodePublishMediaWidgetEntity

			TRUNCATE TABLE ZnodePublishSearchWidgetEntity

			TRUNCATE TABLE ZnodePublishContentPageConfigEntity

			TRUNCATE TABLE ZnodePublishSEOEntity

			TRUNCATE TABLE ZnodePublishMessageEntity

			TRUNCATE TABLE ZnodePublishPortalGlobalAttributeEntity

			TRUNCATE TABLE ZnodePublishPortalBrandEntity

			TRUNCATE TABLE ZnodePublishProductPageEntity

			TRUNCATE TABLE ZnodePublishWidgetBrandEntity

			TRUNCATE TABLE ZnodePublishPortalBrandEntity

			TRUNCATE TABLE ZnodePublishProgressNotifierEntity


			--Delete saved data of products from the tables to perform complete fresh publish from next time.
			TRUNCATE TABLE ZnodePublishCatalogProductDetail

            TRUNCATE TABLE ZnodePublishProductAttributeJson


			--Update status in ZnodePublishCatalogLog table
			DECLARE @FailedPublishStateId TINYINT;

			SET @FailedPublishStateId = (SELECT PublishStateId FROM ZnodePublishState 
			WHERE  PublishStateCode ='PUBLISH_FAILED')

			UPDATE ZnodePublishCatalogLog SET IsCatalogPublished = 0 
			WHERE IsCatalogPublished = NULL and PublishStateId = ISNULL(@FailedPublishStateId, 0)
			

		    --Update ZnodePublishCatalogLog set PublishStateId to new 'Aborted' state
			DECLARE @ProcessingPublishStateId TINYINT;
			DECLARE @AbortedPublishStateId TINYINT;

			SET  @ProcessingPublishStateId = (SELECT PublishStateId FROM ZnodePublishState
			WHERE  PublishStateCode ='PROCESSING')

			SET  @AbortedPublishStateId = (SELECT PublishStateId FROM ZnodePublishState
			WHERE  PublishStateCode ='ABORTED') 

			UPDATE ZnodePublishCatalogLog set IsCatalogPublished = 0, PublishStateId = @AbortedPublishStateId
			WHERE PublishStateId = ISNULL(@ProcessingPublishStateId, 0)
			

			--Set status of catalog, product, category & seo to its default 
			DECLARE @DraftPublishStateId TINYINT;
			DECLARE @NotPublishedPublishStateId TINYINT;

			SET @DraftPublishStateId = (SELECT PublishStateId FROM ZnodePublishState 
			WHERE PublishStateCode ='DRAFT');

			SET @NotPublishedPublishStateId = (SELECT PublishStateId FROM ZnodePublishState 
			WHERE PublishStateCode ='NOT_PUBLISHED');

			UPDATE ZnodePimCategory SET PublishStateId = @DraftPublishStateId
			WHERE PublishStateId != ISNULL(@NotPublishedPublishStateId, 0)

			UPDATE ZnodePimProduct SET PublishStateId = @DraftPublishStateId
			WHERE PublishStateId != ISNULL(@NotPublishedPublishStateId, 0)

			UPDATE ZnodeCMSSEODetail SET PublishStateId = @DraftPublishStateId
			WHERE PublishStateId != ISNULL(@NotPublishedPublishStateId, 0)

			UPDATE ZnodeCMSContentPages SET PublishStateId = @DraftPublishStateId
			WHERE PublishStateId != ISNULL(@NotPublishedPublishStateId, 0)

			UPDATE ZnodeCMSMessage SET PublishStateId = @DraftPublishStateId
			WHERE PublishStateId != ISNULL(@NotPublishedPublishStateId, 0)

			UPDATE ZnodeCMSSlider SET PublishStateId = @DraftPublishStateId
			WHERE PublishStateId != ISNULL(@NotPublishedPublishStateId, 0)

            SELECT 1 AS ID,CAST(1 AS BIT) AS Status;
							        
			COMMIT TRAN A				
         END TRY
         BEGIN CATCH
		   SELECT ERROR_MESSAGE()
          DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
		   @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
		    @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 
			 'EXEC Znode_DeleteAllPublishedData @UserId = '+@UserId+',@PimCatalogId='+@PimCatalogId;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteAllPublishedData',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;