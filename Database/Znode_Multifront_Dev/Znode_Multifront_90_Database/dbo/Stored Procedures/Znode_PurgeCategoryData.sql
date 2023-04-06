CREATE PROCEDURE [dbo].[Znode_PurgeCategoryData]
(
	@DeleteAllCategory BIT = 0,
	@ExceptCategoryId TransferId Readonly
)
AS
BEGIN 
BEGIN TRY
	SET NOCOUNT ON;
	--Delete categories
	EXECUTE [Znode_PurgeData] @DeleteAllCategory = @DeleteAllCategory,@ExceptCategoryId=@ExceptCategoryId

	--Delete publish references of deleted categories
	DECLARE @DeletedIds TransferId 
	INSERT INTO @DeletedIds
	SELECT ZPP.PublishCategoryId
	FROM ZnodePublishCategory ZPP
	WHERE NOT EXISTS(SELECT * FROM ZnodePimCategory ZPP1 WHERE ZPP1.PimCategoryId = ZPP.PimCategoryId)

	
	DELETE FROM ZnodePromotionCategory
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePromotionCategory.PublishCategoryId = D.Id)

	DELETE FROM ZnodePublishCategoryDetail
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCategoryDetail.PublishCategoryId = D.Id)

	DELETE FROM ZnodePublishCategoryProduct
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCategoryProduct.PublishCategoryId = D.Id)

	DELETE FROM ZnodePublishedPortalXml
	WHERE EXISTS(SELECT * FROM ZnodePublishPortalLog
		WHERE ZnodePublishedPortalXml.PublishPortalLogId = ZnodePublishPortalLog.PublishPortalLogId AND
		EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishPortalLog.PublishCategoryId = D.Id))

	DELETE FROM ZnodePublishPortalLog
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishPortalLog.PublishCategoryId = D.Id)

	DELETE FROM ZnodeCMSWidgetCategory
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeCMSWidgetCategory.PublishCategoryId = D.Id)

	DELETE FROM ZnodeSearchGlobalProductCategoryBoost
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodeSearchGlobalProductCategoryBoost.PublishCategoryId = D.Id)

	DELETE FROM ZnodePublishCategory
	WHERE EXISTS(SELECT * FROM @DeletedIds D WHERE ZnodePublishCategory.PublishCategoryId = D.Id)

END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()	
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max) = 'EXEC Znode_PurgeCategoryData'
		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_PurgeCategoryData', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END