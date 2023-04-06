CREATE PROCEDURE [dbo].[Znode_PurgeCatalogData]
(
	@DeleteAllCatalog BIT = 0,
	@ExceptCatalogId TransferId Readonly
)
AS
BEGIN 
BEGIN TRY
	SET NOCOUNT ON;
	--Delete catalogs
	EXECUTE [Znode_PurgeData] @DeleteAllCatalog = @DeleteAllCatalog,@ExceptCatalogId=@ExceptCatalogId

	--Delete publish references of deleted catalog
	DECLARE @DeletedIds TransferId 
	INSERT INTO @DeletedIds
	SELECT ZPP.PublishCatalogId
	FROM ZnodePublishCatalog ZPP
	WHERE NOT EXISTS(SELECT * FROM ZnodePimCatalog ZPP1 WHERE ZPP1.PimCatalogId = ZPP.PimCatalogId)

	DELETE FROM ZnodePublishCategoryDetail 
	WHERE EXISTS(SELECT * FROM ZnodePublishCategory WHERE ZnodePublishCategoryDetail.PublishCategoryId = ZnodePublishCategory.PublishCategoryId
		AND EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCategory.PublishCatalogId = D.Id))
	
	DELETE FROM ZnodePublishCategoryProduct 
	WHERE  EXISTS(SELECT * FROM ZnodePublishCategory WHERE ZnodePublishCategory.PublishCategoryId = ZnodePublishCategoryProduct.PublishCategoryId
		AND EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCategory.PublishCatalogId = D.Id))
	
	DELETE FROM ZnodePromotionCategory 
	WHERE  EXISTS(SELECT * FROM ZnodePublishCategory WHERE ZnodePublishCategory.PublishCategoryId = ZnodePromotionCategory.PublishCategoryId
		AND EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCategory.PublishCatalogId = D.Id))
	
	DELETE FROM ZnodeSearchSynonyms
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeSearchSynonyms.PublishCatalogId = D.Id)

	DELETE FROM ZnodeSearchKeywordsRedirect
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeSearchKeywordsRedirect.PublishCatalogId = D.Id)
	
	DELETE FROM ZnodePublishCategory 
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCategory.PublishCatalogId = D.Id)

	DELETE FROM ZnodePublishCatalogLog 
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCatalogLog .PublishCatalogId = D.Id)

	DELETE FROM ZnodeSearchIndexServerStatus 
	WHERE EXISTS(SELECT * FROM ZnodeSearchIndexMonitor 
			WHERE ZnodeSearchIndexServerStatus.SearchIndexMonitorId = ZnodeSearchIndexMonitor.SearchIndexMonitorId 
			AND CatalogIndexId in (SELECT CatalogIndexId FROM ZnodeCatalogIndex WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeCatalogIndex .PublishCatalogId = D.Id)))
	
	
	DELETE FROM ZnodeSearchIndexMonitor 
	WHERE EXISTS(SELECT * FROM ZnodeCatalogIndex WHERE ZnodeSearchIndexMonitor.CatalogIndexId = ZnodeCatalogIndex.CatalogIndexId
			AND EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeCatalogIndex .PublishCatalogId = D.Id))
	
	DELETE FROM ZnodeCatalogIndex 
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeCatalogIndex .PublishCatalogId = D.Id)

	DELETE FROM ZnodePublishProductDetail
	WHERE EXISTS(SELECT * FROM ZnodePublishProduct WHERE ZnodePublishProductDetail.PublishProductId = ZnodePublishProduct.PublishProductId 
			AND EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishProduct .PublishCatalogId = D.Id))
	
	DELETE FROM ZnodePromotionProduct 
	WHERE EXISTS(SELECT * FROM ZnodePublishProduct WHERE ZnodePromotionProduct.PublishProductId = ZnodePublishProduct.PublishProductId 
			AND EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishProduct .PublishCatalogId = D.Id))
	
	DELETE FROM ZnodeSearchGlobalProductBoost 
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeSearchGlobalProductBoost .PublishCatalogId = D.Id)

	DELETE FROM ZnodeSearchGlobalProductCategoryBoost  
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeSearchGlobalProductCategoryBoost .PublishCatalogId = D.Id)

	DELETE FROM ZnodePublishProduct 
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishProduct .PublishCatalogId = D.Id)

	DELETE FROM ZnodePublishCatalogSearchProfile
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCatalogSearchProfile .PublishCatalogId = D.Id)
	
	DELETE FROM ZnodeportalCatalog 
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeportalCatalog .PublishCatalogId = D.Id)

	DELETE FROM ZnodePortalSearchProfile
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePortalSearchProfile .PublishCatalogId = D.Id)

	DELETE FROM ZnodePublishCatalog 
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCatalog .PublishCatalogId = D.Id)

END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()	
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max) = 'EXEC Znode_PurgeCatalogData'
		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_PurgeCatalogData', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END