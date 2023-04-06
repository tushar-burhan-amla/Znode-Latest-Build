CREATE PROCEDURE [dbo].[Znode_GetQuickOrderPublishProduct]
(
	@PublishCatalogId int,
	@LocaleId int,
	@PublishCategoryIds NVARCHAR(MAX),
	@SKUs NVARCHAR(MAX),
	@VersionId int,
	@ProductIndex int,
	@PortalId int 
)
AS
/*
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	 EXEC Znode_GetQuickOrderPublishProduct 
	@PublishCatalogId= '3',
	@LocaleId= '1',
	@PublishCategoryIds = '34,35,36,37,38,39,40,41,42,43,50,79,82,83,84,85,86,87,88,90' ,
	@SKUs= 'TestConfig1',
	@VersionId= '5942',
	@ProductIndex= '1',
	@PortalId =1 
*/
BEGIN

SET NOCOUNT ON;
BEGIN TRY
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
   -- Insert statements for procedure here
	Create table #TBL_SKUs (SKU NVARCHAR(MAX));
	Create table #TBL_CategoryIds (CategoryId INT);

	INSERT INTO #TBL_SKUs
	SELECT item
	FROM dbo.split(@SKUs, ',');

	INSERT INTO #TBL_CategoryIds
	SELECT item
	FROM dbo.split(@PublishCategoryIds, ',');


	----getting publish product associations
	select ZPAVL.AttributeValue as ParentSKU, ZPAP.ParentPimProductId,ZPAP.PimProductId, ZPAVL1.attributeValue as SKU, 
		   IsConfigurable,IsBundle,IsGroup,IsAddOn,IsLink, ZPAP.DisplayOrder, ZPAP.PublishAssociatedProductId
	into #TempProductassociation
	from ZnodePublishAssociatedProduct ZPAP
	inner join ZnodePimAttributeValue ZPAV ON ZPAV.PimProductId = ZPAP.ParentPimProductId
	inner join ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimattributeValueId = ZPAVL.PimAttributeValueId
	inner join ZnodePimAttributeValue ZPAV1 ON ZPAV1.PimProductId = ZPAP.PimProductId
	inner join ZnodePimAttributeValueLocale ZPAVL1 ON ZPAV1.PimattributeValueId = ZPAVL1.PimAttributeValueId
	where ZPAV.PimattributeId = (select Top 1 PimattributeId from ZnodePimAttribute ZPA where ZPa.AttributeCode = 'SKU')
	AND ZPAV1.PimattributeId = (select Top 1 PimattributeId from ZnodePimAttribute ZPA where ZPa.AttributeCode = 'SKU') 
	and Exists(select * from #TBL_SKUs s where s.sku = ZPAVL.AttributeValue )
	

	----getting publish product associations
	select ParentSKU,  SKU,
		    Row_Number()Over( PARTITION BY  ParentSKU ORDER BY DisplayOrder, PublishAssociatedProductId) RowId
	into #TempConfigProduct
	from #TempProductassociation
	where IsConfigurable = 1

	
	SELECT sku.SKU as ParentSKU, case when ParentSKU is null then sku.SKU else config.sku  end as childsku, ParentSKU as ConfigurableProductSKUs
	into #SKUs
	FROM #TBL_SKUs sku
	LEFT JOIN #TempConfigProduct config on sku.sku = config.ParentSKU and RowId = 1
	
	SELECT P.Name,P.SKU,P.ZnodeProductId as Id, P.IsActive as IsActive, P.Attributes, s.ParentSKU , s.ConfigurableProductSKUs 
	into #ProductDetail
	FROM ZnodePublishProductEntity  AS P
	INNER JOIN #SKUs AS S ON P.SKU=S.childsku
	INNER JOIN #TBL_CategoryIds AS C ON C.CategoryId = P.ZnodeCategoryIds
	WHERE ZnodeCatalogId = @PublishCatalogId and LocaleId = @LocaleId and IsActive = 'true' and ZnodeCategoryIds != 0 and ProductIndex = @ProductIndex and VersionId = @VersionId  

	CREATE TABLE #TempSKUInventory (SKU varchar(600),Quantity numeric(28,6),ReOrderLevel numeric(28,6),PortalId int, WarehouseName varchar(100), WarehouseCode varchar(100), DefaultInventoryCount numeric(28,6))

	CREATE TABLE #TempProductPrice 
	(	
		SKU varchar(600),RetailPrice numeric(28,6),SalesPrice numeric(28,6),TierPrice  numeric(28,6),TierQuantity  numeric(28,6),CurrencyCode varchar(100),
		CurrencySuffix varchar(1000),CultureCode varchar(100),ExternalId varchar(1000),Custom1 varchar(1000),Custom2 varchar(1000),Custom3 varchar(1000)
	)

	Declare @NewSKU vaRCHAR(MAX)

	SET @NewSKU = SUBSTRING((SELECT ','+SKU FROM #ProductDetail FOR XML PATH(''),Type).value('.', 'varchar(max)'), 2, 4000);

	----getting products inventory details
	insert into #TempSKUInventory(SKU ,Quantity ,ReOrderLevel ,PortalId, WarehouseName, WarehouseCode, DefaultInventoryCount)
	Execute [Znode_GetInventoryBySkus] @SKUs = @NewSKU, @PortalId = @PortalId

	----getting product pricing details
	insert into #TempProductPrice(SKU,RetailPrice,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix,CultureCode,ExternalId ,Custom1,Custom2 ,Custom3 )
	Execute [Znode_GetPublishProductPricingBySku] @SKU = @NewSKU, @PortalId=@PortalId, @currentUtcDate=@Getdate

	--getting comma seperated Addon Product
	select TPA.ParentSKU,
		stuff( (SELECT ','+TPA1.SKU FROM #TempProductassociation TPA1 
		WHERE (TPA.ParentPimProductId = TPA1.ParentPimProductId and IsAddOn = 1)
					 FOR XML PATH(''),Type).value('.', 'varchar(max)'), 1, 1, '') as AddOnProductSkus
	into #AddOnProductSKUS
	from #TempProductassociation TPA
	where IsAddOn = 1


	--getting comma seperated Group Products
	select TPA.ParentSKU,
		stuff( (SELECT ','+TPA1.SKU FROM #TempProductassociation TPA1 
		WHERE (TPA.ParentPimProductId = TPA1.ParentPimProductId and IsGroup = 1)
					 FOR XML PATH(''),Type).value('.', 'varchar(max)'), 1, 1, '') GroupProductSKUs
		,count(TPA.SKU) as GroupProductsQuantity
	into #GroupProductSKUs
	from #TempProductassociation TPA
	where IsGroup = 1
	group by TPA.ParentSKU, TPA.ParentPimProductId 
		

	select SKU,sum(ISNULL(Quantity,0)) as Quantity 
	into #TempSKUInventorySum
	from #TempSKUInventory
	group by SKU

	Select PD.SKU, ZPP.PromotionId 
	into #TempProductPromotion
	from #ProductDetail PD
	inner join ZnodePromotionProduct ZPP ON PD.Id = ZPP.PublishProductId 
	where exists(select * from ZnodePromotion ZP inner join ZnodePromotionType ZPT ON ZP.PromotionTypeId = ZPT.PromotionTypeId
	      where ZPP.PromotionId = ZP.PromotionId and ZPT.Name = 'Call For Pricing')

	select DISTINCT PD.Name,PD.SKU,PD.Id as Id, PD.IsActive as IsActive, PD.Attributes, TPP.RetailPrice, 
	       addon.AddOnProductSkus, PD.ConfigurableProductSKUs, grp.GroupProductSKUs, ISNULL(grp.GroupProductsQuantity,0) AS GroupProductsQuantity,
		   ISNULL(Inv.Quantity,0) as QuantityOnHand, Cast(case when Promo.PromotionId is null then 'false' else 'true' end as bit) HasPromotion
	from #ProductDetail PD
	left join #TempProductPrice TPP ON PD.SKU = TPP.SKU
	left join #AddOnProductSKUS addon ON PD.SKU = addon.ParentSKU
	left join #GroupProductSKUs grp  ON PD.SKU = grp.ParentSKU
	left join #TempSKUInventorySum Inv  ON PD.SKU = Inv.SKU
	left join #TempProductPromotion Promo  ON PD.SKU = Promo.SKU
	
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetQuickOrderPublishProduct @PublishCatalogId = '+cast(@PublishCatalogId as varchar(10))+',@LocaleId= '+cast(@LocaleId as varchar(10))+',@VersionId='+CAST(@VersionId AS VARCHAR(50))+',@PublishCategoryIds='+CAST(@PublishCategoryIds AS VARCHAR(50))+',@SKUs='+CAST(@SKUs AS VARCHAR(10))+',@ProductIndex='+CAST(@ProductIndex AS VARCHAR(10))+',@PortalId='+CAST(@PortalId AS VARCHAR(10));
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		ROLLBACK TRANSACTION GetPublishAssociatedProducts;
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetQuickOrderPublishProduct',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH;
END