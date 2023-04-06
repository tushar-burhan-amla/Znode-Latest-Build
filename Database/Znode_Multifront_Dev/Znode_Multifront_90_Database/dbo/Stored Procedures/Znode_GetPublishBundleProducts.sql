
CREATE Procedure [dbo].[Znode_GetPublishBundleProducts] 
(
	@PimCatalogId NVARCHAR(max) ,
	@PimProductId NVARCHAR(max) = '',
	@PimProducttType NVARCHAR(300) = 'BundleProduct'
	--GroupedProduct
	--BundleProduct
	--ConfigurableProduct
)
AS 
------------------------------------------------------------------------------
--Summary : if PimcatalogId is provided get all products with Bundles and provide above mentioned data
--          if PimProductId is provided get all Bundles if associated with given product id and provide above mentioned data
--			Input: @PimcatalogId or @PimProductId
--		    output should be in XML format
--          sample xml5
--			<BundleProductEntity>
--			<ZnodeProductId></ZnodeProductId>
--			<ZnodeCatalogId></ZnodeCatalogId>
--			<TempAsscociadedZnodeProductIds></TempAsscociadedZnodeProductIds>
--			</BundleProductEntity>

--<BundleProductEntity>
--<ZnodeProductId>16</ZnodeProductId>
--<ZnodeCatalogId>2</ZnodeCatalogId>
--<TempAsscociadedZnodeProductIds>6,7,11,14,53,54,55,56,56,57,60,62</TempAsscociadedZnodeProductIds>
--</BundleProductEntity>

--Unit Testing 
--EXEC [dbo].[Znode_GetPublishBundleProducts] @PimCatalogId =2 ,@PimProductId =14

--EXEC [dbo].[Znode_GetPublishBundleProducts] @PimCatalogId =2 ,@PimProductId ='' 


------------------------------------------------------------------------------
BEGIN
BEGIN TRAN 
BEGIN TRY 

		DECLARE @Tlb_Product TABLE (ProductId Int)
		DECLARE @Tlb_Catalog TABLE (CatalogId Int)
		DECLARE @Tlb_BundleEntity TABLE (ZnodeProductId int ,ZnodeCatalogId int,TempAsscociadedZnodeProductIds int)
		DECLARE @Tlb_BundleEntityWithPublish TABLE (ZnodeProductId int ,ZnodeCatalogId int,TempAsscociadedZnodeProductIds int)

		If ISNULL(@PimCatalogId ,'') <> '' 
		BEGIN
			INSERT INTO @Tlb_Catalog(CatalogId)  	 
			SELECT Item FROM Dbo.split(@PimCatalogId ,',') 
		END
		If ISNULL(@PimProductId ,'') <> '' 
		BEGIN
			INSERT INTO @Tlb_Product(ProductId)  	 
			SELECT Item FROM Dbo.split(@PimProductId ,',')
		END
		
		IF ISNULL(@PimProductId,'') <> ''
		Begin 
			INSERT INTO @Tlb_BundleEntity (ZnodeProductId ,ZnodeCatalogId ,TempAsscociadedZnodeProductIds )
			Select  ZPP.PublishProductId ZnodeProductId,
					ZPP.PublishCatalogId ZnodeCatalogId, 
					ZPPTA.PimProductId TempAsscociadedZnodeProductIds 
			from ZnodePimAttribute(NOLOCK) ZPA 
			INNER JOIN ZnodePimAttributeValue (NOLOCK) ZPAV ON ZPA.PimAttributeId = ZPAV.PimAttributeId 
			INNER JOIN ZnodePimAttributeValuelocale(NOLOCK) ZPADVL on ZPAV.PimAttributeValueId = ZPADVL.PimAttributeValueId
			INNER JOIN ZnodePimProductTypeAssociation(NOLOCK) ZPPTA ON ZPAV.PimProductId  = ZPPTA.PimParentProductId
			INNER JOIN  ZNodePublishProduct (NOLOCK) ZPP	 ON ZPP.PimProductId		  =  ZPPTA.PimParentProductId
			--INNER JOIN @Tlb_Catalog TC on ZPP.PublishCatalogId = TC.CatalogId  
			INNER JOIN @Tlb_Product TP ON ZPP.PimProductId = TP.ProductId
					where ZPA.AttributeCode = 'ProductType' and ZPADVL.AttributeValue like  '%' + @PimProducttType + '%' 
					Order by  ZPPTA.PimParentProductId, ZPPTA.PimProductId
		END 
		Else 
		Begin
			INSERT INTO @Tlb_BundleEntity (ZnodeProductId ,ZnodeCatalogId ,TempAsscociadedZnodeProductIds )
			Select  ZPP.PublishProductId ZnodeProductId,
					ZPP.PublishCatalogId ZnodeCatalogId, 
					ZPPTA.PimProductId TempAsscociadedZnodeProductIds 
			from ZnodePimAttribute(NOLOCK) ZPA 
			INNER JOIN ZnodePimAttributeValue (NOLOCK) ZPAV ON ZPA.PimAttributeId = ZPAV.PimAttributeId 
			INNER JOIN ZnodePimAttributeValuelocale(NOLOCK) ZPADVL on ZPAV.PimAttributeValueId = ZPADVL.PimAttributeValueId
			INNER JOIN ZnodePimProductTypeAssociation(NOLOCK) ZPPTA ON ZPAV.PimProductId  = ZPPTA.PimParentProductId
			INNER JOIN  ZNodePublishProduct (NOLOCK) ZPP	 ON ZPP.PimProductId		  =  ZPPTA.PimParentProductId
			INNER JOIN @Tlb_Catalog TC on ZPP.PublishCatalogId = TC.CatalogId  
			where ZPA.AttributeCode = 'ProductType' and ZPADVL.AttributeValue like '%' + @PimProducttType + '%' 
			Order by  ZPPTA.PimParentProductId, ZPPTA.PimProductId
			
		END 
		--print 'hack'
		INSERT INTO @Tlb_BundleEntityWithPublish(ZnodeProductId ,ZnodeCatalogId ,TempAsscociadedZnodeProductIds )
			Select ZnodeProductId ,ZnodeCatalogId ,ZPP.PublishProductId TempAsscociadedZnodeProductIds  from @Tlb_BundleEntity TAE 
			INNER JOIN ZNodePublishProduct ZPP ON TAE.TempAsscociadedZnodeProductIds = ZPP.PimProductId
			INNER JOIN @Tlb_Catalog TC on ZPP.PublishCatalogId = TC.CatalogId  

		IF @PimProducttType = 'BundleProduct' 
			Begin
				print 'hack'
				Select ZnodeProductId,ZnodeCatalogId,
				Substring((SELECT ',' + CAST(TempAsscociadedZnodeProductIds as Varchar(100)) FROM @Tlb_BundleEntityWithPublish q WHERE q.ZnodeProductId = a.ZnodeProductId 
				FOR XML path ('')),2,4000) 
				TempAsscociadedZnodeProductIds 
				from @Tlb_BundleEntityWithPublish  a  Group by ZnodeProductId,ZnodeCatalogId
				FOR XML path (''),  Root('BundleProductEntity')
			end 
		Else if @PimProducttType = 'GroupedProduct' 
			Select ZnodeProductId,ZnodeCatalogId,
			Substring((SELECT ',' + CAST(TempAsscociadedZnodeProductIds as Varchar(100)) FROM @Tlb_BundleEntityWithPublish q WHERE q.ZnodeProductId = a.ZnodeProductId 
			FOR XML path ('')),2,4000) 
			TempAsscociadedZnodeProductIds 
			from @Tlb_BundleEntityWithPublish  a  Group by ZnodeProductId,ZnodeCatalogId
			FOR XML path (''),  Root('GroupedProductEntity')
		Else if @PimProducttType = 'ConfigurableProduct' 
			Select ZnodeProductId,ZnodeCatalogId,
			Substring((SELECT ',' + CAST(TempAsscociadedZnodeProductIds as Varchar(100)) FROM @Tlb_BundleEntityWithPublish q WHERE q.ZnodeProductId = a.ZnodeProductId 
			FOR XML path ('')),2,4000) 
			TempAsscociadedZnodeProductIds 
			from @Tlb_BundleEntityWithPublish  a  Group by ZnodeProductId,ZnodeCatalogId
			FOR XML path (''),  Root('ConfigurableProductEntity')
				
  COMMIT TRAN 
  END TRY 
  BEGIN CATCH
	SELECT ERROR_MESSAGE ()
	ROLLBACK TRAN  
	END CATCH 
END