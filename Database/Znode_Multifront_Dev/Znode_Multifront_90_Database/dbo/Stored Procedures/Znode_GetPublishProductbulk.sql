CREATE PROCEDURE [dbo].[Znode_GetPublishProductbulk]
(
@PublishCatalogId INT = 0 
,@VersionId       VARCHAR(50) = 0 
,@PimProductId    TransferId Readonly
,@UserId		  INT = 0 
,@PimCategoryHierarchyId  INT = 0 
,@PimCatalogId INT = 0 
,@LocaleIds TransferId READONLY
,@PublishStateId INT = 0 
)
With RECOMPILE
AS
/*
DECLARE @rrte transferId 
INSERT INTO @rrte
select 1

EXEC [Znode_GetPublishProductbulk] @PublishCatalogId=3,@UserId= 2 ,@localeIDs = @rrte,@PublishStateId = 3 

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PortalId INT = (SELECT TOP 1 POrtalId FROM ZnodePortalCatalog WHERE PublishCatalogId = @PublishCatalogId)
	DECLARE @PriceListId INT = (SELECT TOP 1 PriceListId FROM ZnodePriceListPortal WHERE PortalId = @PortalId )
	DECLARE @DomainUrl varchar(max) = (select TOp 1 URL FROM ZnodeMediaConfiguration WHERE IsActive =1)
	DECLARE @MaxSmallWidth INT  = (SELECT TOP 1  MAX(MaxSmallWidth) FROM ZnodeGlobalMediaDisplaySetting)
	DECLARE @PimMediaAttributeId INT = dbo.Fn_GetProductImageAttributeId()

	DECLARE --@ProductNamePimAttributeId INT = dbo.Fn_GetProductNameAttributeId(),
	@DefaultLocaleId INT= Dbo.Fn_GetDefaultLocaleId(),@LocaleId INT = 0 
		--,@SkuPimAttributeId  INT =  dbo.Fn_GetProductSKUAttributeId() , @IsActivePimAttributeId INT =  dbo.Fn_GetProductIsActiveAttributeId()
   DECLARE @GetDate DATETIME =dbo.Fn_GetDate()

   declare @DefaultPortal int, @IsAllowIndexing int
	select @DefaultPortal = ZPC.PortalId, @IsAllowIndexing = 1 from ZnodePimCatalog ZPC Inner Join ZnodePublishCatalog ZPC1 ON ZPC.PimCatalogId = ZPC1.PimCatalogId where ZPC1.PublishCatalogId =  @PublishCatalogId and isnull(ZPC.IsAllowIndexing,0) = 1 

	-----delete unwanted publish data
	delete ZPC from ZnodePublishCategoryProduct ZPC
	where not exists(select * from ZnodePublishCategory ZC where ZPC.PublishCategoryId = ZC.PublishCategoryId )

	delete ZPP from ZnodePublishCategoryProduct ZPP
	where not exists(select * from ZnodePublishProduct ZP where ZPP.PublishProductId = ZP.PublishProductId )

	delete ZPP from ZnodePublishCatalogProductDetail ZPP
	where not exists(select * from ZnodePublishProduct ZP where ZPP.PublishProductId = ZP.PublishProductId )

	delete ZPCP from ZnodePublishCatalogProductDetail ZPCP
	inner join ZnodePublishProduct b on ZPCP.PublishProductId = b.PublishProductId 
	where not exists(select * from ZnodePimCategoryProduct a
	inner join ZnodePimCategoryHierarchy ZPCH on ZPCH.PimCategoryID = a.PimCategoryId 
	where b.PimProductId = A.PimProductId and ZPCP.PimCategoryHierarchyId = ZPCH.PimCategoryHierarchyId)
	and isnull(ZPCP.PimCategoryHierarchyId,0) <> 0 and b.PublishCatalogId = @PublishCatalogId
	---------

	 DECLARE @TBL_LocaleId  TABLE (RowId INT IDENTITY(1,1) PRIMARY KEY  , LocaleId INT )
			
	INSERT INTO @TBL_LocaleId (LocaleId)
	SELECT  LocaleId
	FROM ZnodeLocale MT 
	WHERE IsActive = 1
	AND (EXISTS (SELECT TOP 1 1  FROM @LocaleIds RT WHERE RT.Id = MT.LocaleId )
	OR NOT EXISTS (SELECT TOP 1 1 FROM @LocaleIds )) 

	DECLARE @Counter INT =1 ,@maxCountId INT = (SELECT max(RowId) FROM @TBL_LocaleId ) 

	create table #ZnodePrice (RetailPrice numeric(28,13),SalesPrice numeric(28,13),CurrencyCode varchar(100), CultureCode varchar(100), CurrencySuffix varchar(100), PublishProductId int)
	
	create table #ProductSKU (SEOUrl nvarchar(max), SEODescription  nvarchar(max),SEOKeywords  nvarchar(max),SEOTitle  nvarchar(max), PublishProductId int)

	create table #ProductImages (PublishProductId int, ImageSmallPath  varchar(max))

	EXEC Znode_InsertUpdatePimAttributeXML 1 
	EXEC Znode_InsertUpdateCustomeFieldXML 1
	EXEC Znode_InsertUpdateAttributeDefaultValue 1 

	EXEC [Znode_InsertUpdatePimCatalogProductDetail] @PublishCatalogId=@PublishCatalogId,@LocaleId=@LocaleIds,@UserId=@UserId

	if (@IsAllowIndexing=1)
	begin 
		insert into #ZnodePrice
		SELECT RetailPrice,SalesPrice,ZC.CurrencyCode,ZCC.CultureCode ,ZCC.Symbol CurrencySuffix,TYU.PublishProductId
		FROM ZnodePrice ZP 
		INNER JOIN ZnodePriceList ZPL ON (ZPL.PriceListId = ZP.PriceListId)
		INNER JOIN ZnodeCurrency ZC oN (ZC.CurrencyId = ZPL.CurrencyId )
		INNER JOIN ZnodeCulture ZCC ON (ZCC.CultureId = ZPL.CultureId)
		INNER JOIN ZnodePublishProductDetail TY ON (TY.SKU = ZP.SKU ) 
		INNER JOIN ZnodePublishProduct TYU ON (TYU.PublishProductId = TY.PublishProductId) 
		WHERE ZP.PriceListId = @PriceListId 
		AND TY.LocaleId = @DefaultLocaleId
		AND TYU.PublishCatalogId = @PublishCatalogId
		AND EXISTS (SELECT TOP 1 1 FROM ZnodePriceListPortal ZPLP 
		INNER JOIN ZnodePimCatalog ZPC on ZPC.PortalId=ZPLP.PortalId WHERE ZPLP.PriceListId=ZP.PriceListId )
		AND EXISTS(select * from ZnodePimProduct ZPP1 where TYU.PimProductId = ZPP1.PimProductId )
	
		--CAST(@DefaultPortal AS VARCHAr(100)) + '/'+
		insert INTO #ProductImages
		SELECT  TYU.PublishProductId , @DomainUrl +'Catalog/'  +  CAST(@MaxSmallWidth AS VARCHAR(100)) + '/' + RT.MediaPath AS ImageSmallPath   
		FROM ZnodePimAttributeValue ZPAV 
		INNER JOIN ZnodePublishProduct TYU ON (TYU.PimProductId  = ZPAV.PimProductId)
		INNER JOIN ZnodePimProductAttributeMedia  RT ON ( RT.PimAttributeValueId = ZPAV.PimAttributeValueId )
		WHERE  TYU.PublishCatalogId = @PublishCatalogId
		AND RT.LocaleId = @DefaultLocaleId
		AND ZPAV.PimAttributeId = (SELECT TOp 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'ProductImage')
		AND EXISTS(select * from ZnodePimProduct ZPP1 where TYU.PimProductId = ZPP1.PimProductId )
	
		insert INTO #ProductSKU 
		SELECT ZCSD.SEOUrl , ZCDL.SEODescription,ZCDL.SEOKeywords ,ZCDL.SEOTitle, TYU.PublishProductId
		FROM ZnodeCMSSEODetail ZCSD 
		INNER JOIN ZnodeCMSSEODetailLocale ZCDL ON (ZCDL.CMSSEODetailId = ZCSD.CMSSEODetailId)
		INNER JOIN ZnodePublishProductDetail TY ON (TY.SKU = ZCSD.SEOCode AND ZCDL.LocaleId = TY.LocaleId) 
		INNER JOIN ZnodePublishProduct TYU ON (TYU.PublishProductId = TY.PublishProductId)
		WHERE CMSSEOTypeId = (SELECT TOP 1 CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Product') 
		AND ZCDL.LocaleId = @DefaultLocaleId
		AND TYU.PublishCatalogId = @PublishCatalogId
		--AND ZCSD.PublishStateId = @PublishStateId
		AND ZCSD.PortalId = @DefaultPortal
		AND EXISTS(select * from ZnodePimProduct ZPP1 where TYU.PimProductId = ZPP1.PimProductId )

	end
	
CREATE NONCLUSTERED INDEX Idx_#ProductSKU_PublishProductId
ON [dbo].[#ProductSKU] ([PublishProductId])


CREATE NONCLUSTERED INDEX Idx_#ProductImages_PublishProductId
ON [dbo].#ProductImages ([PublishProductId])

CREATE NONCLUSTERED INDEX Idx_#ZnodePrice_PublishProductId
ON [dbo].#ZnodePrice ([PublishProductId])



SELECT ZPP.Pimproductid,ZPCPD.LocaleId,(SELECT Attributes as AttributeEntity  from ZnodePublishProductAttributeXML a where a.pimproductid = ZPP.pimproductid and a.LocaleId = ZPCPD.LocaleId FOR XML PATH('Attributes'), TYPE)   ProductXML
into #ProductAttributeXML
FROM [ZnodePublishCatalogProductDetail] ZPCPD 
INNER JOIN ZnodePublishProduct ZPP ON ZPCPD.PublishProductId = ZPP.PublishProductId and ZPCPD.PublishCatalogId = ZPP.PublishCatalogId --where TY.PimProductId = ZPP.PimProductId  AND TY.LocaleId = ZPCPD.LocaleId 
WHERE ZPCPD.PublishCatalogId = @PublishCatalogId
group by pimproductid,ZPCPD.LocaleId


CREATE NONCLUSTERED INDEX Idx_#ProductAttributeXML_PimProductId_LocaleId
ON [dbo].#ProductAttributeXML (PimProductId,LocaleId)

DECLARE @MaxCount INT, @MinRow INT, @MaxRow INT, @Rows numeric(10,2);
		SELECT @MaxCount = COUNT(*) FROM [ZnodePublishCatalogProductDetail] WHERE PublishCatalogId = @PublishCatalogId;

		SELECT @Rows = 5000
        
		SELECT @MaxCount = CEILING(@MaxCount / @Rows);

select PimCatalogProductDetailId, PublishProductId,Row_Number() over(Order by PublishProductId) ID into #ZnodePublishCatalogProductDetail from [ZnodePublishCatalogProductDetail] WHERE PublishCatalogId = @PublishCatalogId


--CREATE NONCLUSTERED INDEX #ZnodePublishCatalogProductDetail

		IF OBJECT_ID('tempdb..#Temp_ImportLoop') IS NOT NULL
            DROP TABLE #Temp_ImportLoop;
        
		---- To get the min and max rows for import in loop
		;WITH cte AS 
		(
			SELECT RowId = 1, 
				   MinRow = 1, 
                   MaxRow = cast(@Rows as int)
            UNION ALL
            SELECT RowId + 1, 
                   MinRow + cast(@Rows as int), 
                   MaxRow + cast(@Rows as int)
            FROM cte
            WHERE RowId + 1 <= @MaxCount
		)
        SELECT RowId, MinRow, MaxRow
        INTO #Temp_ImportLoop
        FROM cte
		option (maxrecursion 0);


		DECLARE cur_BulkData CURSOR LOCAL FAST_FORWARD
        FOR SELECT MinRow, MaxRow, B.LocaleId 
		FROM #Temp_ImportLoop L
		CROSS APPLY @TBL_LocaleId B;

        OPEN cur_BulkData;
        FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow,@LocaleId;

        WHILE @@FETCH_STATUS = 0
        BEGIN

			if @VersionId = 0 and @PimCatalogId <> 0
				select @VersionId = max(PublishCatalogLogId) from ZnodePublishCatalogLog 
				where PublishCatalogId = (select top 1 PublishCatalogId from ZnodePublishCatalog where PimCatalogId = @PimCatalogId )
				AND LocaleId = @LocaleId

			if @VersionId = 0 and @PublishCatalogId <> 0 and @PimCatalogId = 0
				select @VersionId = max(PublishCatalogLogId) from ZnodePublishCatalogLog where PublishCatalogId = PublishCatalogId AND LocaleId = @LocaleId

			--SET @LocaleId = 1
			--(SELECT TOP 1 LocaleId FROM @TBL_LocaleId MT 
			--WHERE  RowId = @Counter)
			--if OBJECT_ID('tempdb..#ConfigProductDetail') is not null
			--	drop table #ConfigProductDetail

			--SELECT DISTINCT ZPCPA.PimProductId, --ZPA.AttributeCode, 
			--'<SelectValues>'+STUFF((select  ' '+'<SelectValuesEntity>','<VariantDisplayOrder>'+CAST(ISNULL(ZPPTA.DisplayOrder,0) AS VARCHAR(200))+'</VariantDisplayOrder>
			--					<VariantSKU>'+ISNULL((SELECT ''+ZPAVL_SKU.AttributeValue FOR XML Path ('')) ,'')+'</VariantSKU>
			--					<VariantImagePath>'+ISNULL((SELECT ''+ZM.Path FOR XML Path ('')),'')+'</VariantImagePath></SelectValuesEntity>'  
			--			from ZnodePimProductTypeAssociation ZPPTA--YUP ON (YUP.PimProductId = ZPAV1.PimProductId)
			--				 INNER JOIN ZnodePimAttributeValue ZPAV_SKU ON(ZPPTA.PimProductId = ZPAV_SKU.PimProductId)
			--				 INNER JOIN ZnodePimAttributeValueLocale ZPAVL_SKU ON (ZPAVL_SKU.PimAttributeValueId = ZPAV_SKU.PimAttributeValueId)
			--				 inner join ZnodePimAttribute ZPA1 ON ZPA1.PimAttributeId = ZPAV_SKU.PimAttributeId
			--				 INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId =ZPAV_SKU.PimProductId)
			--				 INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPAV.PimAttributevalueid = ZPAVL.PimAttributeValueId AND ZPAVL.AttributeValue = 'True')
			--				 inner join ZnodePimAttribute ZPA2 ON ZPA2.PimAttributeId = ZPAV.PimAttributeId
			--				 LEFT  JOIN ZnodePimAttributeValue ZPAV12 ON (ZPAV12.PimProductId= ZPPTA.PimProductId  AND ZPAV12.PimAttributeId = @PimMediaAttributeId ) 
			--				 LEFT JOIN ZnodePimProductAttributeMedia ZPAVM ON (ZPAVM.PimAttributeValueId= ZPAV12.PimAttributeValueId ) 
			--				 LEFT JOIN ZnodeMedia ZM ON (ZM.MediaId = ZPAVM.MediaId)
			--			where ZPPTA.PimParentProductId = ZPCPA.PimProductId AND ZPA1.AttributeCode = 'SKU' and ZPA2.AttributeCode = 'IsActive'
			--FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</SelectValues>' ConfigDataXML
			----FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</SelectValues>' ConfigDataXML
			--into #ConfigProductDetail
			--FROM ZnodePimConfigureProductAttribute ZPCPA
			--inner join ZnodePimAttribute ZPA ON ZPA.PimAttributeId = ZPCPA.PimAttributeId
			--where exists(select * from #ZnodePublishCatalogProductDetail ZPCPD 
			--inner join ZnodePublishProduct ZPP ON ZPCPD.PublishProductId = ZPP.PublishProductId
			--where ZPCPA.PimProductId = ZPP.PimProductId and ZPCPD.Id BETWEEN @MinRow AND @MaxRow)

			INSERT INTO ZnodePublishedXml (PublishCatalogLogId
				,PublishedId
				,PublishedXML
				,IsProductXML
				,LocaleId
				,CreatedBy
				,CreatedDate
				,ModifiedBy
				,ModifiedDate
				,PublishCategoryId)
			SELECT @VersionId,ZPCPD.PublishProductId, cast(replace(replace(replace('<ProductEntity><VersionId>'+CAST(@VersionId AS VARCHAR(50)) +'</VersionId><ZnodeProductId>'+CAST(ZPCPD.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><ZnodeCategoryIds>'+CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50))+'</ZnodeCategoryIds><Name>'+CAST(ISNULL((SELECT ''+isnull(ZPCPD.ProductName,'') FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</Name>'+'<SKU>'+CAST(ISNULL((SELECT ''+ZPCPD.SKU FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKU>'+'<SKULower>'+CAST(ISNULL((SELECT ''+LOWER(ZPCPD.SKU) FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKULower>'+'<IsActive>'+CAST(ISNULL(ZPCPD.IsActive ,'0') AS VARCHAR(50))+'</IsActive>' 
				+'<ZnodeCatalogId>'+CAST(ZPCPD.PublishCatalogId  AS VARCHAR(50))+'</ZnodeCatalogId><IsParentProducts>'+CASE WHEN ZPCP.PublishCategoryId IS NULL THEN '0' ELSE '1' END  +'</IsParentProducts><CategoryName>'+CAST(ISNULL((SELECT ''+isnull(ZPCPD.CategoryName,'') FOR XML PATH ('')),'') AS NVARCHAR(2000)) +'</CategoryName><CatalogName>'+CAST(ISNULL((SELECT ''+isnull(ZPCPD.CatalogName,'') FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</CatalogName><LocaleId>'+CAST( Isnull(ZPCPD.LocaleId,'') AS VARCHAR(50))+'</LocaleId>'
			+Case When @IsAllowIndexing = 1 then 	
				+'<RetailPrice>'+ISNULL(CAST(TBZP.RetailPrice  AS VARCHAr(500)),'')+'</RetailPrice>'
				+'<SalesPrice>'+ISNULL(CAST(TBZP.SalesPrice AS VARCHAr(500)), '') +'</SalesPrice>'
				+'<CurrencyCode>'+ISNULL(TBZP.CurrencyCode,'') +'</CurrencyCode>'
				+'<CultureCode>'+ISNULL(TBZP.CultureCode,'') +'</CultureCode>'
				+'<CurrencySuffix>'+ISNULL(TBZP.CurrencySuffix,'') +'</CurrencySuffix>'
				+'<SeoUrl>'+ISNULL(TBPS.SEOUrl,'') +'</SeoUrl>'
				+'<SeoDescription>'+ISNULL((SELECT ''+TBPS.SEODescription FOR XML PATH('') ),'') +'</SeoDescription>'
				+'<SeoKeywords>'+ISNULL((SELECT ''+TBPS.SEOKeywords FOR XML PATH('')),'') +'</SeoKeywords>'
				+'<SeoTitle>'+ISNULL((SELECT ''+SEOTitle FOR XML PATH('')),'') +'</SeoTitle>'
				+'<ImageSmallPath>'+ISNULL(TBPI.ImageSmallPath,'') +'</ImageSmallPath>'
			else '' End	
				+'<TempProfileIds>'+ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
								FROM ZnodeProfile ZPFC 
								WHERE isnull(ZPFC.PimCatalogId,0) = isnull(ZPCH.PimCatalogId,0) FOR XML PATH('')),2,8000),'')+'</TempProfileIds>
				<ProductIndex>'+CAST(ZPCPD.ProductIndex AS VARCHAr(100))+'</ProductIndex><IndexId>'+CAST( ISNULL(ZPCP.PublishCategoryProductId,'0') AS VARCHAr(100))+'</IndexId>'+
				'<DisplayOrder>'+CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'+cast(PAX.ProductXML as varchar(max))
				+'</ProductEntity>','&','&amp;'),'&amp;amp;','&amp;'),'&amp;amp;amp;','&amp;') as XML)  xmlvalue,1,ZPCPD.LocaleId,@UserId , @GetDate , @UserId,@GetDate,ZPCP.PublishCategoryId
			FROM [ZnodePublishCatalogProductDetail] ZPCPD
			INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPCPD.PublishCatalogId)
			INNER JOIN ZnodePublishProduct ZPP ON ZPCPD.PublishProductId = ZPP.PublishProductId and ZPCPD.PublishCatalogId = ZPP.PublishCatalogId
			inner join #ProductAttributeXML PAX ON PAX.PimProductId = ZPP.PimProductId  AND PAX.LocaleId = ZPCPD.LocaleId 
			inner join #ZnodePublishCatalogProductDetail z on ZPCPD.PimCatalogProductDetailId = z.PimCatalogProductDetailId
			LEFT JOIN #ZnodePrice TBZP ON (TBZP.PublishProductId = ZPCPD.PublishProductId)
			LEFT JOIN #ProductSKU TBPS ON (TBPS.PublishProductId = ZPCPD.PublishProductId)
			LEFT JOIN #ProductImages TBPI ON (TBPI.PublishProductId = ZPCPD.PublishProductId)
			LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPCPD.PublishProductId AND ZPCP.PublishCatalogId = ZPCPD.PublishCatalogId AND ZPCP.PimCategoryHierarchyId = ZPCPD.PimCategoryHierarchyId)
			LEFT JOIN ZnodePublishCategory ZPC ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId AND ZPCP.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId)
			LEFT JOIN ZnodePimCategoryHierarchy ZPCH ON ( ZPCH.PimCategoryHierarchyId =  ZPC.PimCategoryHierarchyId AND ZPCH.PimCatalogId = ZPCV.PimCatalogId )
			LEFT JOIN ZnodePimCategoryProduct ZPCCF ON ( ZPCH.PimCategoryId = ZPCCF.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId )
			--LEFT JOIN #ConfigProductDetail CPD ON ZPP.PimProductId = CPD.PimProductId
			WHERE ZPCPD.LocaleId = @LocaleId and z.Id BETWEEN @MinRow AND @MaxRow
			AND ZPCPD.PublishCatalogId = @PublishCatalogId

			set @VersionId = 0

			FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow,@LocaleId;
        END;
		CLOSE cur_BulkData;
		DEALLOCATE cur_BulkData;

	
	
END