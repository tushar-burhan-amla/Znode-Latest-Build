CREATE  PROCEDURE [dbo].[Znode_GetPublishProductRecordset]
(
   @ImportGUID Varchar(500) = '' 
)
AS
/*
	To publish all catalog product and their details
	Unit Testing : 
*/
BEGIN
BEGIN TRY 
SET NOCOUNT ON
		SELECT TPC.PublishProductId  ,TPC.PublishCatalogId ,VersionId,TPC.LocaleId ,PP.SKU, ZCSD.SEOUrl,ZCSD.PortalId 
			FROM ZnodePublishProductRecordset TPC
			INNER JOIN ZnodePublishProductDetail PP ON (PP.PublishProductId = TPC.PublishProductId)
			inner join ZnodePublishCatalog ZPC ON TPC.PublishCatalogId = ZPC.PublishCatalogId
			left join ZnodePortalCatalog ZPCat on ZPC.PimCatalogId = ZPCat.PortalCatalogId
			left join ZnodeCMSSEODetail ZCSD on PP.SKU = ZCSD.SEOCode and ZPCat.PortalId = ZCSD.PortalId 
				AND ZCSD.CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Product')
				where ImportGUID = @ImportGUID 
			GROUP BY TPC.PublishProductId  ,TPC.PublishCatalogId ,VersionId,TPC.LocaleId,PP.SKU, ZCSD.SEOUrl,ZCSD.PortalId 

		Select B.* from ZnodePublishProductRecordset A Inner join ZnodePublishProductEntity B on 
			A.PublishProductId = B.ZnodeProductId 
			and a.VersionId = b.VersionId
			and a.PublishCatalogId = b.ZnodeCatalogId 
			where ImportGUID = @ImportGUID 

END TRY 
BEGIN CATCH 
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishProductRecordset
		@ImportGUID='+CAST(@ImportGUID AS VARCHAR(500))

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetPublishProductRecordset',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
END CATCH
END