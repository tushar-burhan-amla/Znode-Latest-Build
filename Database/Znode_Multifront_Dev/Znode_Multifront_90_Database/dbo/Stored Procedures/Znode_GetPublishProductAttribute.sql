
CREATE PROCEDURE [dbo].[Znode_GetPublishProductAttribute]
(@PublishCatalogId INT)
AS 
/*
     Summary :- This Procedure is used to get the publish Product attribute details for a PublishCatalogId 
     Unit Testing 
     EXEC Znode_GetPublishProductAttribute 6
	 select * from znodepublishcatalog
	 select  * from znodeportal
     
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
		
 
		     SELECT a.PimAttributeId,ISNULL(c.PimAttributeDefaultValueId,0) PimAttributeDefaultValueId
			 INTO #ProductPimAttribute
			 FROM ZnodePimAttributeValue a 
			 INNER JOIN ZnodePublishProduct b ON (b.PimProductId = a.PimProductId )
			 LEFT JOIN ZnodePimProductAttributeDefaultValue c ON (c.PimAttributeValueId = a.PimAttributeValueId)
			 WHERE PublishCatalogId = @PublishCatalogId
			 GROUP BY  PimAttributeId,ISNULL(c.PimAttributeDefaultValueId,0)	


			SELECT  @PublishCatalogId  ZnodeCatalogId,AttributeCode,AttributeTypeName,IsComparable,IsHtmlTags,IsFacets,IsUseInSearch,IsPersonalizable,                   
			IsConfigurable , ZPAL.AttributeName , ZPAL.LocaleId ,ZPA.DisplayOrder,zpa.PimAttributeId
			INTO #AttributeValueFacet
			FROM ZnodePimAttribute ZPA 
			INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
			INNER JOIN ZnodePimAttributeLocale ZPAL on ( ZPA.PimAttributeId = ZPAL.PimAttributeId ) 
			INNER JOIN ZnodePimFrontendProperties ZPFP ON(ZPA.PimAttributeId = ZPFP.PimAttributeId)
			WHERE EXISTS (SELECT TOP 1 1 FROM #ProductPimAttribute TYU WHERE TYU.PimAttributeId = ZPA.PimAttributeId)
		

            SELECT  ZPA.ZnodeCatalogId,AttributeCode,AttributeTypeName,IsComparable,IsHtmlTags,IsFacets,IsUseInSearch,IsPersonalizable,                   
			IsConfigurable , ZPA.AttributeName , ZPA.LocaleId 
			,ZPA.DisplayOrder,ZPDAV.DisplayOrder AS DefaultValueDisplayOrder, CASE WHEN AttributeCode IN ('Brand','ProductType','OutOfStockOptions') THEN ZPDAV.AttributeDefaultValueCode ELSE ZPDAVL.AttributeDefaultValue END
			AttributeDefaultValue
			FROM #AttributeValueFacet ZPA 
			INNER JOIN ZnodePimAttributeDefaultValue ZPDAV ON (ZPDAV.PimAttributeId = ZPA.PimAttributeId)
			INNER JOIN ZnodePimAttributeDefaultValueLocale ZPDAVL ON (ZPDAVL.PimAttributeDefaultValueId = ZPDAV.PimAttributeDefaultValueId)
			WHERE EXISTS (SELECT TOP 1 1 FROM #ProductPimAttribute TY WHERE TY.PimAttributeDefaultValueId = ZPDAV.PimAttributeDefaultValueId )


		    UNION ALL
			SELECT DISTINCT ZPA.ZnodeCatalogId,AttributeCode,AttributeTypeName,IsComparable,IsHtmlTags,IsFacets,IsUseInSearch,IsPersonalizable,                   
			IsConfigurable , ZPA.AttributeName , ZPA.LocaleId 
			,ZPA.DisplayOrder,NULL AS DefaultValueDisplayOrder, NULL AS AttributeDefaultValue
			FROM #AttributeValueFacet ZPA 
			
			UNION ALL
			SELECT DISTINCT ZPA.ZnodeCatalogId,AttributeCode,AttributeTypeName,IsComparable,IsHtmlTags,IsFacets,IsUseInSearch,IsPersonalizable,                   
			IsConfigurable , ZPA.AttributeName , ZPA.LocaleId 
			,ZPA.DisplayOrder,NULL AS DefaultValueDisplayOrder, NULL AS AttributeDefaultValue
			FROM #AttributeValueFacet ZPA 
			
			UNION ALL
			SELECT DISTINCT ZPA.ZnodeCatalogId,AttributeCode,AttributeTypeName,IsComparable,IsHtmlTags,IsFacets,IsUseInSearch,IsPersonalizable,                   
			IsConfigurable , ZPA.AttributeName , ZPA.LocaleId 
			,ZPA.DisplayOrder,NULL AS DefaultValueDisplayOrder, NULL AS AttributeDefaultValue
			FROM #AttributeValueFacet ZPA 
			
			

         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
			 @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishProductAttribute @PublishCatalogId= '+CAST(@PublishCatalogId AS VARCHAR(10))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPublishProductAttribute',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;