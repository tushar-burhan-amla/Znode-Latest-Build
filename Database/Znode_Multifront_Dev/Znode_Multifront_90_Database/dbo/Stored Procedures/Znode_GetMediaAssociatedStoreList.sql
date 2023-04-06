CREATE PROCEDURE [dbo].[Znode_GetMediaAssociatedStoreList]
( 
	@MediaPath NVARCHAR(MAX) 
)
AS
/*
	Summary : This procedure is used to get PortalId for associated store of media
	Unit Testing:
	EXEC [Znode_GetAssociatedStoreOfMedia] ='d9a98db6-52df-4692-b7ae-b50f55980dc8grapes.jpg'
	
*/
     BEGIN
         SET NOCOUNT ON;
		 BEGIN TRY
				DECLARE @Tbl_Portal TABLE (PortalId INT)

				INSERT INTO @Tbl_Portal (PortalId)
				SELECT DISTINCT ptc.[PortalId] from ZnodePimProductAttributeMedia pam 
				INNER JOIN ZnodePimAttributeValue pav on pav.PimAttributeValueId=pam.PimAttributeValueId
				INNER JOIN ZnodePimProduct pp on pp.PimProductId=pav.PimProductId
				INNER JOIN ZnodePimCategoryProduct ZPCP ON ZPCP.PimProductId=pp.PimProductId
				INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId
				INNER JOIN ZnodePublishCatalog pc on pc.PimCatalogId=ZPCH.PimCatalogId
				INNER JOIN ZnodePortalCatalog ptc on ptc.PublishCatalogId=pc.PublishCatalogId
				where pam.MediaPath=@MediaPath
				UNION ALL
				SELECT DISTINCT ZPC.[PortalId]
				FROM ZnodePortal ZPC 
				CROSS APPLY ZnodeBrandDetails ZBD  
				INNER JOIN ZnodeBrandDetailLocale ZBDL ON(ZBD.BrandId = ZBDL.BrandId) 
				--LEFT JOIN ZnodeBrandPortal ZBP ON (ZBP.BrandId = ZBD.BrandId AND ZBP.PortalId = ZPC.PortalId) 
				INNER JOIN ZnodeMedia ZM ON(ZM.MediaId = ZBD.MediaId)
				where ZM.Path=@MediaPath
				UNION ALL
				SELECT DISTINCT PortalId
				from ZnodePimCategoryAttributeValue pam 
				INNER JOIN ZnodePimCategoryAttributeValueLocale pav on pav.PimCategoryAttributeValueId=pam.PimCategoryAttributeValueId
				INNER JOIN ZnodePimCategoryProduct ZPCP ON ZPCP.PimCategoryId = pam.PimCategoryId
				INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId
				INNER JOIN ZnodePublishCatalog pc on pc.PimCatalogId=ZPCH.PimCatalogId
				INNER JOIN ZnodePortalCatalog ptc on ptc.PublishCatalogId=pc.PublishCatalogId
				INNER JOIN Znodemedia ZM on (CAST(ZM.mediaid AS NVARCHAR(MAX)) = pav.categoryvalue)
				WHERE pam.PimAttributeId in (SELECT pimattributeid from ZnodePimAttribute where IsCategory =1 and AttributeCode = 'Categoryimage')
				AND ZM.Path=@MediaPath

				SELECT DISTINCT PortalId FROM @Tbl_Portal

		END TRY
		BEGIN CATCH
	
		  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetMediaAssociatedStoreList @MediaPath = '+@MediaPath;
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetMediaAssociatedStoreList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH

     END;