CREATE PROCEDURE [Znode_PurgeProductData]
(
	@DeleteAllProduct BIT = 0,
	@ExceptProductId TransferId Readonly
)
AS
BEGIN 
BEGIN TRY
	SET NOCOUNT ON;		
	--Delete products
	EXECUTE [Znode_PurgeData] @DeleteAllProduct = @DeleteAllProduct,@ExceptProductId=@ExceptProductId

	--Delete publish references of deleted products
	DECLARE @DeletedIds TransferId 
	INSERT INTO @DeletedIds
	SELECT ZPP.PublishProductId
	FROM ZnodePublishProduct ZPP
	WHERE NOT EXISTS(SELECT * FROM ZnodePimProduct ZPP1 WHERE ZPP1.PimProductId = ZPP.PimProductId)

	DELETE FROM ZnodePublishProductRecordset
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishProductRecordset.PublishProductId = D.Id)
	DELETE FROM ZnodeSearchGlobalProductBoost
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeSearchGlobalProductBoost.PublishProductId = D.Id)
	DELETE FROM ZnodeSearchGlobalProductBoost
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeSearchGlobalProductBoost.PublishProductId = D.Id)
	DELETE FROM ZnodeCMSCustomerReview
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeCMSCustomerReview.PublishProductId = D.Id)
	DELETE FROM ZnodePromotionProduct
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePromotionProduct.PublishProductId = D.Id)
	DELETE FROM ZnodePublishCategoryProduct
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCategoryProduct.PublishProductId = D.Id)

	DELETE FROM ZnodePublishCatalogProductDetail
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCatalogProductDetail.PublishProductId = D.Id)
	DELETE FROM ZnodePublishProductDetail
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishProductDetail.PublishProductId = D.Id)
	DELETE FROM ZnodeCMSWidgetProduct
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeCMSWidgetProduct.PublishProductId = D.Id)
	DELETE FROM ZnodePublishProduct
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishProduct.PublishProductId = D.Id)

END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()	
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max) = 'EXEC Znode_PurgeProductData'
		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_PurgeProductData', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END