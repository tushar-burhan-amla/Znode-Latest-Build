CREATE  PROCEDURE [dbo].[Znode_DeletePublishCategoryProducts]
(
 @PublishCatalogId  INT  
,@PublishCategoryIds VARCHAR(1000) = NULL 
,@PublishProductIds  VARCHAR(1000)  = NULL  
) 
AS
-----------------------------------------------------------------------------
--Summary:Delete the publish product from all related referances 
--Unit Testing   

--EXEC Znode_DeletePublishCategoryProducts @PublishCatalogId =1 , @PublishCategoryIds = '3,4,33'

----------------------------------------------------------------------------- 
BEGIN
	BEGIN TRANSACTION A; 
	BEGIN TRY 
		SET NOCOUNT ON;

			DECLARE @TBL_DeletedCategory TABLE ([PublishCategoryId] INT );
			DECLARE @TBL_DeleteProduct TABLE ([PublishProductId] INT );

			INSERT INTO @TBL_DeletedCategory 
			SELECT [Item] 
			FROM [dbo].[Split](@PublishCategoryIds , ',');

			INSERT INTO @TBL_DeleteProduct 	SELECT [PublishProductId] 
			FROM [ZNodePublishProduct] adf WHERE (EXISTS ( SELECT TOP 1 1 FROM [dbo].[Split](@PublishProductIds , ',') de WHERE de.[Item] = adf.[PublishProductId]) 
			OR EXISTS (SELECT TOP 1 1 FROM [ZnodePublishCategoryProduct] df INNER JOIN @TBL_DeletedCategory fd ON (fd.[PublishCategoryId] = df.[PublishCategoryId]) 
			WHERE df.[PublishProductId] = adf.[PublishProductId]  AND df.[PublishCatalogId] = @PublishCatalogId));

			DELETE FROM [ZnodeCMSWidgetCategory] WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeletedCategory w WHERE w.[PublishCategoryId] = [ZnodeCMSWidgetCategory].[PublishCategoryId] );
			--DELETE FROM ZnodeCategoryPromotion WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeletedCategory w WHERE w.PublishCategoryId = ZnodeCategoryPromotion.PublishCategoryId )

			DELETE FROM [ZnodeCMSWidgetProduct]  WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeleteProduct qw WHERE qw.[PublishProductId]=[ZnodeCMSWidgetProduct].[PublishProductId]);
			DELETE FROM [ZnodeCMSCustomerReview] WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeleteProduct qw WHERE qw.[PublishProductId]=[ZnodeCMSCustomerReview].[PublishProductId]);
	
			DELETE FROM [ZnodePromotionCoupon] WHERE [PromotionId] IN (SELECT  [PromotionId] FROM [ZnodePromotion]  
			WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeletedCategory w WHERE w.[PublishCategoryId] = [ZnodePromotion].[PublishCategoryId] )
			OR EXISTS (SELECT TOP 1 1 FROM @TBL_DeleteProduct qw WHERE qw.[PublishProductId]=[ZnodePromotion].[PublishProductId]));

			DELETE FROM [ZnodePromotion]  WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeleteProduct qw WHERE qw.[PublishProductId]=[ZnodePromotion].[PublishProductId]);
			DELETE FROM [ZnodePromotion]  WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeletedCategory w WHERE w.[PublishCategoryId] = [ZnodePromotion].[PublishCategoryId] );
    	    	
			DELETE FROM [ZnodeCMSSEODetail] 	   WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeleteProduct qw WHERE qw.[PublishProductId] = [ZnodeCMSSEODetail].[SEOId] )
												AND EXISTS (SELECT TOP 1 1 FROM [ZnodeCMSSEOType] we WHERE we.[CMSSEOTypeId] = [ZnodeCMSSEODetail].[CMSSEODetailId] AND [Name] = 'Product' );
			
			DELETE FROM [ZnodePublishCategoryProduct]  WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeleteProduct qw WHERE qw.[PublishProductId] = [ZnodePublishCategoryProduct].[PublishProductId] )
														AND  EXISTS (SELECT TOP 1 1 FROM @TBL_DeletedCategory w WHERE w.[PublishCategoryId] = [ZnodePublishCategoryProduct].[PublishCategoryId] );
			DELETE FROM [ZNodePublishProduct] WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeleteProduct qw WHERE qw.[PublishProductId] = [ZNodePublishProduct].[PublishProductId] );
	COMMIT TRANSACTION A; 
	END TRY 
	BEGIN CATCH 
		SELECT ERROR_MESSAGE (),ERROR_PROCEDURE();
		ROLLBACK TRANSACTION A; 
	END CATCH;  
END;