
CREATE PROCEDURE [dbo].[Znode_DeletePublishCatalog]
(   @PublishCatalogIds  VARCHAR(1000),
	@PublishCategoryIds VARCHAR(1000) = NULL,
	@PublishProductIds  VARCHAR(1000) = NULL,
	@IsDeleteCatalogId  BIT           = 0)
AS
   /*
    Summary:Delete the publish product from all related references 
    Unit Testing   
	begin tran
	SELECT * FROM ZnodePublishCatalog 
    EXEC Znode_DeletePublishCatalog @PublishCatalogIds =5 ,@IsDeleteCatalogId = 1 , @PublishCategoryIds = ''
	rollback tran
   */ 
     BEGIN
         BEGIN TRANSACTION A;
         BEGIN TRY
             SET NOCOUNT ON;
	
             DECLARE @TBL_DeletedCatalog TABLE(PublishCatalogId INT);
             DECLARE @TBL_DeletedCategory TABLE([PublishCategoryId] INT);
             DECLARE @TBL_DeleteProduct TABLE([PublishProductId] INT);
             INSERT INTO @TBL_DeletedCatalog
                    SELECT [item]
                    FROM [dbo].[Split](@PublishCatalogIds, ',');

             INSERT INTO @TBL_DeletedCategory
                    SELECT PublishCategoryId
                    FROM ZnodePublishCategory AS PC
                    WHERE( EXISTS
                         (
                             SELECT TOP 1 1
                             FROM [dbo].[Split](@PublishCategoryIds, ',') AS m
                             WHERE m.Item = PC.PublishCategoryId
                         ));

             INSERT INTO @TBL_DeleteProduct
                    SELECT [PublishProductId]
                    FROM [ZNodePublishProduct] AS adf
                    WHERE(EXISTS
                         (
                             SELECT TOP 1 1
                             FROM [dbo].[Split](@PublishProductIds, ',') AS de
                             WHERE de.[Item] = adf.[PublishProductId]
                         )
                   );
             --DELETE FROM [ZnodeCMSWidgetCategory]
             --WHERE EXISTS
             --(
             --    SELECT TOP 1 1
             --    FROM @TBL_DeletedCategory AS w
             --    WHERE w.[PublishCategoryId] = [ZnodeCMSWidgetCategory].[PublishCategoryId]
             --);
         
             --DELETE FROM [ZnodeCMSWidgetProduct]
             --WHERE EXISTS
             --(
             --    SELECT TOP 1 1
             --    FROM @TBL_DeleteProduct AS qw
             --    WHERE qw.[PublishProductId] = [ZnodeCMSWidgetProduct].[PublishProductId]
             --);
             --DELETE FROM [ZnodeCMSCustomerReview]
             --WHERE EXISTS
             --(
             --    SELECT TOP 1 1
             --    FROM @TBL_DeleteProduct AS qw
             --    WHERE qw.[PublishProductId] = [ZnodeCMSCustomerReview].[PublishProductId]
             --);
             DELETE FROM ZnodeUserWishList
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteProduct AS qw
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM ZnodePublishProductDetail ZPPD
                     WHERE ZPPD.SKU = ZnodeUserWishList.SKU
                           AND qw.[PublishProductId] = ZPPD.[PublishProductId]
                 )
             );
            

             DELETE FROM [ZnodePromotionProduct]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteProduct AS qw
                 WHERE qw.[PublishProductId] = [ZnodePromotionProduct].[PublishProductId]
             );
             DELETE FROM [ZnodePromotionCategory]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedCategory AS w
                 WHERE w.[PublishCategoryId] = [ZnodePromotionCategory].[PublishCategoryId]
             );
             DELETE FROM [ZnodePublishCategoryDetail]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedCategory AS w
                 WHERE w.[PublishCategoryId] = [ZnodePublishCategoryDetail].[PublishCategoryId]
             );	
            
             --DELETE FROM ZnodeCMSSEODetailLocale
             --WHERE EXISTS
             --(
             --    SELECT TOP 1 1
             --    FROM [ZnodeCMSSEODetail]
             --    WHERE EXISTS
             --    (
             --        SELECT TOP 1 1
             --        FROM @TBL_DeleteProduct AS qw
             --        WHERE qw.[PublishProductId] = [ZnodeCMSSEODetail].[SEOId]
             --    )
             --          AND EXISTS
             --    (
             --        SELECT TOP 1 1
             --        FROM [ZnodeCMSSEOType] AS we
             --        WHERE we.[CMSSEOTypeId] = [ZnodeCMSSEODetail].[CMSSEODetailId]
             --              AND [Name] = 'Product'
             --    )
             --          AND [ZnodeCMSSEODetail].CMSSEODetailId = ZnodeCMSSEODetailLocale.CMSSEODetailId
             --);
             --DELETE FROM [ZnodeCMSSEODetail]
             --WHERE EXISTS
             --(
             --    SELECT TOP 1 1
             --    FROM @TBL_DeleteProduct AS qw
             --    WHERE qw.[PublishProductId] = [ZnodeCMSSEODetail].[SEOId]
             --)
             --      AND EXISTS
             --(
             --    SELECT TOP 1 1
             --    FROM [ZnodeCMSSEOType] AS we
             --    WHERE we.[CMSSEOTypeId] = [ZnodeCMSSEODetail].[CMSSEODetailId]
             --          AND [Name] = 'Product'
             --);
            
             DELETE FROM [ZnodePublishCategoryProduct]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteProduct AS qw
                 WHERE qw.[PublishProductId] = [ZnodePublishCategoryProduct].[PublishProductId]
             );
             DELETE FROM [ZnodePublishCategoryProduct]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedCategory AS w
                 WHERE w.[PublishCategoryId] = [ZnodePublishCategoryProduct].[PublishCategoryId]
             );
             DELETE FROM [ZnodePublishProductDetail]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteProduct AS qw
                 WHERE qw.[PublishProductId] = [ZnodePublishProductDetail].[PublishProductId]
             );
             DELETE FROM [ZNodePublishProduct]
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteProduct AS qw
                 WHERE qw.[PublishProductId] = [ZNodePublishProduct].[PublishProductId]
             
			 );

			 --DELETE FROM ZnodePublishCategoryDetail 
			 -- WHERE EXISTS
			 --(
				--SELECT TOP 1 1 
				--FROM @TBL_DeletedCategory AS w
				--WHERE w.PublishCategoryId = ZnodePublishCategoryDetail.PublishCategoryId			 
			 --);

			 DELETE FROM ZnodePublishCategory
			 WHERE EXISTS
			 (
				SELECT TOP 1 1 
				FROM @TBL_DeletedCategory AS w
				WHERE w.PublishCategoryId = ZnodePublishCategory.PublishCategoryId			 
			 );

			 DELETE FROM ZnodePublishCategoryProduct 
			 WHERE [PublishCatalogId] = @PublishCatalogIds AND @IsDeleteCatalogId = 1

			 DELETE FROM ZnodePublishProductDetail 
			 WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishProduct  
						WHERE [PublishCatalogId] = @PublishCatalogIds AND @IsDeleteCatalogId = 1 
						AND ZnodePublishProductDetail.PublishProductId = ZnodePublishProduct.PublishProductId  )
             DELETE FROM ZnodePublishProduct  
						WHERE [PublishCatalogId] = @PublishCatalogIds AND @IsDeleteCatalogId = 1 

			 DELETE FROM ZnodePublishCategoryDetail 
			 WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishCategory  
						WHERE [PublishCatalogId] = @PublishCatalogIds AND @IsDeleteCatalogId = 1 
						AND ZnodePublishCategoryDetail.PublishCategoryId = ZnodePublishCategory.PublishCategoryId)
             DELETE FROM ZnodePublishCategory  
						WHERE [PublishCatalogId] = @PublishCatalogIds AND @IsDeleteCatalogId = 1 

			 DELETE FROM ZnodePublishedXml WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishCataLogLog 
						WHERE [PublishCatalogId] = @PublishCatalogIds AND @IsDeleteCatalogId = 1 
						AND ZnodePublishedXml.PublishCatalogLogId = ZnodePublishCataLogLog.PublishCatalogLogId  )

			 DELETE FROM ZnodePublishCataLogLog WHERE [PublishCatalogId] = @PublishCatalogIds AND @IsDeleteCatalogId = 1;

			 DELETE FROM znodeSearchIndexServerStatus 
					WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeSearchIndexMonitor a WHERE EXISTS 
					(SELECT TOP 1 1 FROM ZnodeCatalogIndex b WHERE b.publishcatalogid =@PublishCatalogIds AND @IsDeleteCatalogId = 1 AND b.catalogindexid = a.catalogindexid) AND a.SearchIndexMonitorid = znodeSearchIndexServerStatus.SearchIndexMonitorid )

			 DELETE FROM ZnodeSearchIndexMonitor
					WHERE EXISTS (SELECT TOP 1 1  FROM ZnodeCatalogIndex b where b.publishcatalogid =@PublishCatalogIds AND @IsDeleteCatalogId = 1 AND b.catalogindexid = ZnodeSearchIndexMonitor.catalogindexid)


			 DELETE FROM ZnodeCatalogIndex
			 WHERE publishcatalogid =@PublishCatalogIds AND @IsDeleteCatalogId = 1

			
             DELETE FROM ZnodePublishCatalog
             WHERE [PublishCatalogId] = @PublishCatalogIds
                   AND @PublishCategoryIds IS NULL
                   AND @PublishProductIds IS NULL
                   AND @IsDeleteCatalogId = 1;
				  
				
             COMMIT TRANSACTION A;
			 
         END TRY
         BEGIN CATCH
		     
			  DECLARE @Status BIT ;
              DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			  @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePublishCatalog @PublishCatalogIds = '''+ISNULL(@PublishCatalogIds,'''')+''',@PublishCategoryIds='''+ISNULL(@PublishCategoryIds,'''')+''',@PublishProductIds='''+ISNULL(@PublishProductIds,'''')+''',@IsDeleteCatalogId='+ISNULL(CAST(@IsDeleteCatalogId AS VARCHAR(200)),'''')+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeletePublishCatalog',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;