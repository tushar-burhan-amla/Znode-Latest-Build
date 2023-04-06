CREATE PROCEDURE Znode_CatalogProductDraftForPublish
(
	@PublishCatalogId INT
)
AS
BEGIN
BEGIN TRY 
SET NOCOUNT ON 
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

	SELECT ZPCP.PimProductId
	INTO #DraftProduct
	FROM ZnodePimCategoryProduct ZPCP WITH(NOLOCK)
	INNER JOIN ZnodePimCategoryHierarchy ZPCH WITH(NOLOCK) ON ZPCP.PimCategoryId = ZPCH.PimCategoryId 
	WHERE ZPCH.PimCatalogId = @PublishCatalogId

	Update ZPP SET PublishStateId = 2, IsProductPublish = 0 , ModifiedDate = @GetDate 
	FROM ZnodePimProduct ZPP 
	WHERE EXISTS(SELECT * FROM #DraftProduct DP WHERE ZPP.PimProductId = DP.PimProductId)

	Update ZPP SET PublishStateId = 2, IsProductPublish = 0 , ModifiedDate = @GetDate 
	FROM ZnodePimProduct ZPP WITH(NOLOCK)
	where EXISTS(SELECT * FROM ZnodePimProductTypeAssociation PTA WITH(NOLOCK) WHERE ZPP.PimProductId = PTA.PimProductId 
			AND EXISTS(SELECT * FROM #DraftProduct DP WHERE PTA.PimParentProductId = DP.PimProductId))

	Update ZPP SET PublishStateId = 2, IsProductPublish = 0 , ModifiedDate = @GetDate 
	FROM ZnodePimProduct ZPP WITH(NOLOCK)
	where EXISTS(SELECT * FROM ZnodePimLinkProductDetail PTA WITH(NOLOCK) WHERE ZPP.PimProductId = PTA.PimProductId 
			AND EXISTS(SELECT * FROM #DraftProduct DP WHERE PTA.PimParentProductId = DP.PimProductId))

	Update ZPP SET PublishStateId = 2, IsProductPublish = 0 , ModifiedDate = @GetDate 
	FROM ZnodePimProduct ZPP WITH(NOLOCK)
	where EXISTS(SELECT * FROM ZnodePimLinkProductDetail PTA WITH(NOLOCK) WHERE ZPP.PimProductId = PTA.PimProductId 
			AND EXISTS(SELECT * FROM #DraftProduct DP WHERE PTA.PimParentProductId = DP.PimProductId))

	Update ZPP SET PublishStateId = 2, IsProductPublish = 0 , ModifiedDate = @GetDate 
	FROM ZnodePimProduct ZPP
	where EXISTS(SELECT * FROM ZnodePimAddOnProduct PTA WITH(NOLOCK)
			INNER JOIN ZnodePimAddOnProductDetail ZPA1 WITH(NOLOCK) ON PTA.PimAddOnProductId = ZPA1.PimAddOnProductId
			WHERE ZPP.PimProductId = PTA.PimProductId 
			AND EXISTS(SELECT * FROM #DraftProduct DP WHERE ZPA1.PimChildProductId = DP.PimProductId))
END TRY
BEGIN CATCH
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_CatalogProductDraftForPublish @PublishCatalogId = '+CAST(@PublishCatalogId AS VARCHAR(200));

    EXEC Znode_InsertProcedureErrorLog
        @ProcedureName = 'Znode_CatalogProductDraftForPublish',
        @ErrorInProcedure = @Error_procedure,
        @ErrorMessage = @ErrorMessage,
        @ErrorLine = @ErrorLine,
        @ErrorCall = @ErrorCall;
           
END CATCH;
END