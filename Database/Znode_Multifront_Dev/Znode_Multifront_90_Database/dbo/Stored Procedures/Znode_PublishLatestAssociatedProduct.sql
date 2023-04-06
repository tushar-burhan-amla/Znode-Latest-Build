CREATE Procedure [dbo].[Znode_PublishLatestAssociatedProduct]	
(
	@PublishCatalogId Int = 0,
	@PimProductId TransferId Readonly,
	@UserId int,
	@PublishStateId INT = 0 
)
as
--[Znode_PublishLatestAssociatedProduct] @PublishCatalogId=3,@UserId=2,@PublishStateId=3
begin	
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
		DECLARE @ProductTypePimAttributeId INT =  dbo.Fn_GetProductTypeAttributeId()

		truncate table ZnodePublishAssociatedProductLog

		-- Retrive all catalaog, category and their products   
		SELECT ZPCH.PimCatalogId ,ZPCC.PimCategoryId , ZPCH.PimCategoryHierarchyId , ZPCC.PimProductId , d.AttributeDefaultValueCode ProductType
			   ,ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
						FROM ZnodeProfile ZPFC 
						WHERE ZPCH.PimCatalogId = ZPFC.PimCatalogId  FOR XML PATH('')),2,8000),'') ProfileIds	
	    INTO #PimCatalogCategory 
		FROM ZnodePimCategoryProduct ZPCC 
		INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId
		INNER JOIN ZnodePimAttributeValue b ON (b.PimProductId = ZPCC.PimProductId )	
		INNER JOIN ZnodePimProductAttributeDefaultValue c ON (c.PimAttributeValueId = b.PimAttributeValueId) 	
		INNER JOIN ZnodePimAttributeDefaultValue d ON (d.PimAttributeDefaultValueId = c.PimAttributeDefaultValueId)
		INNER JOIN ZnodePublishCatalog ZPC ON ZPC.PimCatalogId = ZPCH.PimCatalogId
		WHERE b.PimAttributeId  = @ProductTypePimAttributeId		
		AND  ( exists(select * from @PimProductId P where ZPCC.PimProductId = P.Id) OR ZPC.PublishCatalogId = @PublishCatalogId )
	   
	    -- here find all link products and associate to that catalog 
	    SELECT ZPLPD.PimProductId, CTPP.PimCategoryId, CTPP.PimCatalogId,CTPP.PimCategoryHierarchyId,CTPP.PimProductId ParentPimProductId 
				,0 IsConfigurable,0 IsBundle,0 IsGroup,0 IsAddOn,1 IsLink, 1 IsWithCatalog , ZPLPD.DisplayOrder
		INTO #AssociatedProduct
		FROM ZnodePimLinkProductDetail AS ZPLPD
		INNER JOIN #PimCatalogCategory AS CTPP ON ZPLPD.PimParentProductId = CTPP.PimProductId 
		ORDER BY ZPLPD.DisplayOrder , ZPLPD.PimLinkProductDetailId
		
		alter table #AssociatedProduct add IsDefault bit

		-- IsWithCatalog this flag will manage to find the parent product 
		INSERT INTO #AssociatedProduct
		SELECT ZPAPD.PimChildProductId, ISNULL(CTALP.PimCategoryId,0) PimCategoryId ,CTALP.PimCatalogId,CTALP.PimCategoryHierarchyId,CTALP.PimProductId ParentPimProductId
				,0 IsConfigurable,0 IsBundle,0 IsGroup,1 IsAddOn,0 IsLink, 0 IsWithCatalog , ZPAPD.DisplayOrder, 0 IsDefault
		FROM ZnodePimAddOnProductDetail AS ZPAPD 
		INNER JOIN ZnodePimAddOnProduct AS ZPAP ON ZPAP.PimAddOnProductId = ZPAPD.PimAddOnProductId
		INNER JOIN #PimCatalogCategory AS CTALP ON CTALP.PimProductId = ZPAP.PimProductId 
		ORDER BY ZPAPD.DisplayOrder , ZPAPD.PimAddOnProductDetailId

		-- associated product with there flag 
		INSERT INTO #AssociatedProduct
		SELECT ZPTA.PimProductId,ISNULL(CTAAP.PimCategoryId,0),CTAAP.PimCatalogId,ISNULL(CTAAP.PimCategoryHierarchyId,0) PimCategoryHierarchyId ,CTAAP.PimProductId ParentPimProductId
						,CASE WHEN CTAAP.ProductType = 'ConfigurableProduct' THEN 1 ELSE 0 END , CASE WHEN CTAAP.ProductType = 'BundleProduct' THEN 1 ELSE 0 END
						, CASE WHEN CTAAP.ProductType = 'GroupedProduct' THEN 1 ELSE 0 END,0,0, 0 IsWithCatalog , ZPTA.DisplayOrder, ZPTA.IsDefault		
        FROM ZnodePimProductTypeAssociation AS ZPTA 
		INNER JOIN #PimCatalogCategory AS CTAAP ON CTAAP.PimProductId = ZPTA.PimParentProductId 
		ORDER BY ZPTA.DisplayOrder , ZPTA.PimProductTypeAssociationId

		SELECT  PimProductId,PimCatalogId,ParentPimProductId,IsConfigurable,IsBundle,IsGroup,IsAddOn,IsLink, DisplayOrder, IsDefault  
		Into #AssociatedProduct1
		FROM #AssociatedProduct
		GROUP BY  PimProductId,PimCatalogId,ParentPimProductId,IsConfigurable,IsBundle,IsGroup,IsAddOn,IsLink, DisplayOrder, IsDefault

		--update ZPAPL set   
		--from ZnodePublishAssociatedProductLog ZPAPL 
		--inner join #AssociatedProduct1 AP

		insert into ZnodePublishAssociatedProductLog(ParentPimProductId,PimProductId,IsConfigurable,IsBundle,IsGroup,IsAddOn,IsLink,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
				,PimCatalogId,PublishStateId, DisplayOrder, IsDefault)
		select SOURCE.ParentPimProductId,SOURCE.PimProductId,SOURCE.IsConfigurable,SOURCE.IsBundle,SOURCE.IsGroup,SOURCE.IsAddOn,SOURCE.IsLink
				,@UserId ,@GetDate,@UserId ,@GetDate ,SOURCE.PimCatalogId,@PublishStateId, SOURCE.DisplayOrder, IsDefault
		from #AssociatedProduct1 SOURCE
		
end