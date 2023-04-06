CREATE Procedure [dbo].[Znode_GetCatalogCategoryProduts]
(
	 @PimCatalogId Int  = NULL,
	 @PimCategoryId Int,
	 @LocaleId  int  = null 
)  
AS 
-- Get catalog / category wise product list 
BEGIN
	BEGIN TRY 
 			IF @LocaleId IS NULL 
			BEGIN 
				SET @LocaleId = (SELECT FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale')
			END 

			SELECT * FROM View_GetCatalogCategoryProduts a
			WHERE a.PimCatalogId=@PimCatalogId AND a.PimCategoryId=@PimCategoryId
			AND Localeid = @LocaleId
			SELECT 1 ID , 1 STATUS 
	END TRY 
	BEGIN CATCH 
			SELECT 1 ID , 0 STATUS 
			--SELECT ERROR_MESSAGE () , ERROR_LINE () 
	END CATCH 
END