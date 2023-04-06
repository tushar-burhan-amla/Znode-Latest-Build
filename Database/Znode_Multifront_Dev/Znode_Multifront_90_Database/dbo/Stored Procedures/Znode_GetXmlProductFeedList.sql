CREATE PROCEDURE [dbo].[Znode_GetXmlProductFeedList]  
(
	@PortalId   VARCHAR(2000) = NULL,   
	@LocaleId   INT,  
	@FeedType   NVARCHAR(MAX) = NULL   
)
AS  
/*  
Summary: This Procedure is used to get effective keyword feeding of Product list  
 SELECT * FROM ZnodePublishProductDetail  
 SELECT * FROM ZnodePublishProduct WHERE PublishCatalogId = 3  
 SELECT * FROM ZnodePortalCatalog   
 Unit Testing:  
 EXEC [Znode_GetXmlProductFeedList] @PortalId='0',@ProductIds = '116,117,118'  
 ,@LocaleId=1,@FeedType='Bing'   
  
*/  
BEGIN  
BEGIN TRY  
SET NOCOUNT ON;        
           
		IF OBJECT_ID('tempdb..#TBL_DomainName') IS NOT NULL
		DROP TABLE #TBL_DomainName

		IF OBJECT_ID('tempdb..#TBL_SEODetails') IS NOT NULL
		DROP TABLE #TBL_SEODetails

		IF OBJECT_ID('tempdb..#TBL_CompleteDetailes') IS NOT NULL
		DROP TABLE #TBL_CompleteDetailes

		IF OBJECT_ID('tempdb..#TBL_CompleteDetailes') IS NOT NULL
		DROP TABLE #TBL_CompleteDetailes

		IF OBJECT_ID('tempdb..#TBL_PortalIds') IS NOT NULL
		DROP TABLE #TBL_PortalIds

		IF OBJECT_ID('tempdb..#TBL_PricePrecedence') IS NOT NULL
		DROP TABLE #TBL_PricePrecedence

		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		CREATE TABLE #TBL_DomainName 
		(PortalId   INT,
		DomainName NVARCHAR(300),
		RowId      INT
		);  	
         
		CREATE TABLE #TBL_SEODetails   
		(loc                   NVARCHAR(MAX),  
		lastmod               DATETIME,  
		[g:condition]         VARCHAR(1000),  
		[description]         NVARCHAR(MAX), 
		[SEOTitle]			  NVARCHAR(MAX),	
		[g:id]                INT,  
		link                  VARCHAR(1000),  
		[g:identifier_exists] VARCHAR(2000),  
		DomainName            NVARCHAR(3000),  
		PortalId              INT , 
		SEOCode             NVARCHAR(4000), 
		CanonicalURL        VARCHAR(2000), 
		RobotTag            VARCHAR(500)
		);  
		
		DECLARE @DefaultLocaleId INT=dbo.Fn_GetDefaultLocaleId()  ;
		CREATE TABLE #TBL_PortalIds (PortalId INT);  
		
		INSERT INTO #TBL_PortalIds  

		SELECT Zp.PortalId   
		FROM Znodeportal AS ZP   
		INNER JOIN ZnodePortalCatalog AS ZPC ON(ZPC.PortalId = Zp.PortalId)  
		INNER JOIN ZnodePublishCatalog AS ZPPC ON(ZPPC.PimCatalogId = ZPC.PublishCatalogId)
		INNER JOIN ZnodePublishProductEntity AS ZPPE ON (ZPPC.PimCatalogId  = ZPPE.ZnodeCatalogId)
		WHERE		
		EXISTS(SELECT TOP 1 1 FROM DBO.Split(@PortalId, ',') AS Sp  
		WHERE(CAST(sp.Item AS INT)) = Zp.PortalId  OR @PortalId = '0')  
		AND EXISTS (SELECT TOP 1 1 FROM ZnodeDomain ZD WHERE ZP.PortalId = ZD.PortalId  
		AND IsActive = 1 AND ApplicationType = 'Webstore')  
		GROUP BY Zp.PortalId;   
	
		INSERT INTO #TBL_DomainName   
		SELECT  PortalId,DomainName,ROW_NUMBER() OVER(PARTITION BY PortalId ORDER BY DomainName)   
		FROM ZnodeDomain AS ZD   
		WHERE EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZD.PortalId)  
		AND IsActive = 1 AND ApplicationType = 'Webstore' AND IsDefault = 1 

		if not exists (select top 1 1 from #TBL_DomainName)
		INSERT INTO #TBL_DomainName   
		SELECT  PortalId,DomainName,ROW_NUMBER() OVER(PARTITION BY PortalId ORDER BY DomainName)   
		FROM ZnodeDomain AS ZD   
		WHERE EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZD.PortalId)  
		AND IsActive = 1 AND ApplicationType = 'Webstore'

	
		
	    	SELECT DISTINCT ZCSD.CMSSEODetailId,ZCSD.SEOURL AS loc,ZCSD.ModifiedDate AS lastmod,'new' AS [g:condition],ZCSDL.SEODescription AS [description],ZCSDL.SEOTitle AS [SEOTitle], ZPCC.ZnodeProductId AS [g:id],  
            ZCSD.SEOUrl AS link,'false' AS [g:identifier_exists],TBDN.DomainName,ZPC.PortalId,ISNULL(ZCSDL.LocaleId, @DefaultLocaleId) AS LocaleId , ZCSD.SEOCode , ZCSDL.CanonicalURL, ZCSDL.RobotTag 
			INTO #Cte_SeoDetailsWithLocale
			FROM ZnodePublishProductEntity AS ZPCC   
			INNER JOIN ZnodePortalCatalog AS ZPC ON(ZPC.PublishCatalogId = ZPCC.ZnodeCatalogId)  
			LEFT JOIN ZnodeCMSSEODetail AS ZCSD ON(ZPCC.SKU = ZCSD.SEOCode and ZCSD.PortalId = ZPC.PortalId)  
			LEFT JOIN ZnodeCMSSEOType AS ZCST ON(ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId AND ZCST.Name = 'Product')  
			LEFT JOIN ZnodeCMSSEODetailLocale AS ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId AND ZCSDL.LocaleId IN(@LocaleId, @DefaultLocaleId))  
			LEFT JOIN #TBL_DomainName AS TBDN ON(TBDN.RowId = 1 AND TBDN.PortalId = zpc.PortalId)   
			WHERE EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZPC.PortalId)  
			
			SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[SEOTitle],[g:id],link,[g:identifier_exists],DomainName,PortalId,LocaleId,SEOCode, CanonicalURL, RobotTag  
			INTO #Cte_SeoDetailsWithFirstLocale
			FROM #Cte_SeoDetailsWithLocale   
			WHERE LocaleId = @LocaleId  
    
			SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[SEOTitle],[g:id],link,[g:identifier_exists],DomainName,PortalId,LocaleId,SEOCode , CanonicalURL, RobotTag 
			INTO #Cte_SeoDetailsWithDefaultLocale
			FROM #Cte_SeoDetailsWithFirstLocale  
			
			INSERT INTO #Cte_SeoDetailsWithDefaultLocale 
			SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[SEOTitle],[g:id],link,[g:identifier_exists],DomainName,PortalId,LocaleId,SEOCode, CanonicalURL, RobotTag  
			FROM #Cte_SeoDetailsWithLocale AS CTSDWL  
			WHERE LocaleId = @DefaultLocaleId   
			AND NOT EXISTS(SELECT TOP 1 1 FROM #Cte_SeoDetailsWithFirstLocale AS CTSDWDL WHERE CTSDWDL.CMSSEODetailId = CTSDWL.CMSSEODetailId)  
         
		 
			INSERT INTO #TBL_SEODetails  
			SELECT DISTINCT loc,lastmod,[g:condition],[description],[SEOTitle],[g:id],link,[g:identifier_exists],DomainName,PortalId ,SEOCode, CanonicalURL, RobotTag  
			FROM #Cte_SeoDetailsWithDefaultLocale;
			
			select DISTINCT a.ZnodeProductId, d.AttributeDefaultValueCode as OutOfStockOptions
			into #TempProduct_OutOfStockOptions
			from ZnodePublishProductEntity a
			inner join ZnodePimAttributeValue b on a.ZnodeProductId = b.PimProductId
			inner join ZnodePimProductAttributeDefaultValue c on b.PimAttributeValueId = c.PimAttributeValueId
			inner join ZnodePimAttributeDefaultValue d on c.PimAttributeDefaultValueId = d.PimAttributeDefaultValueId
			where EXISTS(select top 1 1 from ZnodePimAttribute ZPA where AttributeCode = 'OutOfStockOptions' AND b.PimAttributeId = ZPA.PimAttributeId)
			
			SELECT DISTINCT TBSD.loc,TBSD.lastmod,TBSD.[g:condition],TBSD.[description],TBSD.[SEOTitle],TBSD.[g:id],TBSD.link,TBSD.[g:identifier_exists],TBSD.DomainName,TBSD.PortalId,OOS.OutOfStockOptions,
			CASE WHEN SUM(ISNULL(ZI.Quantity,0)) > 0 THEN 'In Stock'
			     WHEN SUM(ISNULL(ZI.Quantity,0)) = 0 AND ISNULL(OOS.OutOfStockOptions,'') <> 'DisablePurchasing'  THEN 'In Stock'
                 WHEN @FeedType = 'Xml'THEN 'Out Of Stock' 
                 ELSE 'Not In Stock'  END AS [g:availability],
				 
			ZPP.SKU ,TBSD.SEOCode, TBSD.CanonicalURL, TBSD.RobotTag, ZPP.ZnodeProductId,ZPP.Name As ProductName
			INTO #TBL_CompleteDetailes  
			FROM ZnodePublishProductEntity AS ZPP   
			inner join #TempProduct_OutOfStockOptions OOS ON ZPP.ZnodeProductId = OOS.ZnodeProductId
			LEFT JOIN #TBL_SEODetails AS TBSD ON(ZPP.ZnodeProductId = TBSD.[g:id] )  
			LEFT JOIN ZnodePortalWarehouse AS ZPW ON(ZPW.PortalId = TBSD.PortalId)  
			LEFT JOIN ZnodePortalAlternateWarehouse AS ZAPW ON(ZAPW.PortalWarehouseId = ZPW.PortalWarehouseId)  
			LEFT JOIN ZnodeInventory AS ZI ON(ZI.SKU = ZPP.SKU AND (ZI.WarehouseId = ZPW.WarehouseId OR ZI.WarehouseId = ZAPW.WarehouseId))  			
			WHERE  ZPP.LocaleId = @LocaleId
			GROUP BY loc,lastmod,[g:condition],[description],TBSD.[SEOTitle],[g:id],link,[g:identifier_exists],DomainName,TBSD.PortalId,ZPP.SKU,ZPP.LocaleId, TBSD.SEOCode, TBSD.CanonicalURL, TBSD.RobotTag, ZPP.ZnodeProductId,ZPP.Name ,OOS.OutOfStockOptions;    
			
			DECLARE @MediaConfiguration NVARCHAR(2000)=((SELECT TOP 1 ISNULL(CASE WHEN CDNURL = '' THEN NULL ELSE CDNURL END,URL) FROM ZnodeMediaConfiguration WHERE IsActive = 1));    
  
			CREATE INDEX Idx_#TBL_CompleteDetailes ON #TBL_CompleteDetailes(PortalId,SKU)
			
			select * into #TBL_PricePrecedence from(
			select ROW_NUMBER() OVER(PARTITION BY sku ORDER BY Precedence) rnk_min, sku, RetailPrice,SalesPrice,znodeprice.PriceListId,ZnodePriceList.ListCode,
			ZnodePriceListPortal.Precedence
			from znodeprice
			left join ZnodePriceList on ZnodePriceList.PriceListId = znodeprice.PriceListId
			left join  ZnodePriceListPortal on ZnodePriceList.PriceListId= ZnodePriceListPortal.PriceListId where PortalId = @PortalId 
			)a
			order by a.sku,Precedence
			delete from #TBL_PricePrecedence where rnk_min<>1 
	
			SELECT zp.PortalId,round(isnull(Zps.SalesPrice,Zps.RetailPrice),2) as Price,Zps.SKU,tbcd.SEOCode,ROW_NUMBER() OVER(PARTITION BY Zps.SKU,zp.PortalId ORDER BY ZPS.RetailPrice) AS RowId  
			INTO #Cte_PortalList
			FROM ZnodePriceList AS ZPL   
			LEFT JOIN ZnodePriceListPortal AS ZPLP ON ZPL.PriceListId = ZPLP.PriceListId  
			LEFT JOIN ZnodeCulture AS zc ON ZPL.CultureId = zc.CultureId
			LEFT JOIN ZnodePortal AS zp ON ZPLP.PortalId = zp.PortalId  
		    LEFT JOIN #TBL_PricePrecedence Zps ON(Zps.PriceListId = ZPL.PriceListId) 
			--LEFT JOIN ZnodePrice AS Zps ON(Zps.PriceListId = ZPL.PriceListId)   
			LEFT JOIN #TBL_CompleteDetailes AS TBCD ON(TBCD.PortalId = Zp.PortalId AND TBCD.SKU = Zps.Sku)   
			WHERE CAST(@GetDate AS DATE) BETWEEN ZPL.ActivationDate AND ZPL.ExpirationDate   
			AND EXISTS( SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZPLP.PortalId)   
			GROUP BY zp.PortalId,ZPS.RetailPrice,Zps.SalesPrice,Zps.SKU ,TBCD.SEOCode  
			
			SELECT D.ZnodeProductId,C.AttributeDefaultValueCode AS Brand
			INTO #BrandDetails
			FROM ZnodePimAttributeValue A
			INNER JOIN ZnodePimProductAttributeDefaultValue B ON B.PimAttributeValueId=A.PimAttributeValueId
			INNER JOIN ZnodePimAttributeDefaultValue C ON C.PimAttributeDefaultValueId=A.PimAttributeDefaultValueId	
			INNER JOIN ZnodepublishproductEntity D ON  A.PimProductId = D.ZnodeProductId 
			WHERE EXISTS(SELECT * FROM ZnodePimAttribute D WHERE D.PimAttributeId=A.PimAttributeId AND D.AttributeCode='Brand' )
			AND EXISTS(SELECT * FROM #TBL_CompleteDetailes E Where E.ZnodeProductId = D.ZnodeProductId)		
			
			SELECT A.CategoryName , A.ZnodeProductId AS PublishProductId, ROW_NUMBER() OVER(PARTITION BY A.ZnodeProductId ORDER BY A.ZnodeProductId) AS RowId
			INTO #ProductCategories
			FROM ZnodePublishProductEntity A 
			WHERE EXISTS(SELECT * FROM #TBL_CompleteDetailes B WHERE A.ZnodeProductId = B.ZnodeProductId)
			AND ISNULL(A.CategoryName,'') <> ''

			DELETE FROM #ProductCategories WHERE RowId <> 1
 
			SELECT SKU,SUM(cast(Quantity as numeric(28,0)))AS Inventory 
			into #InventotyDetails 
			FROM ZnodeInventory A 
			WHERE EXISTS(SELECT * FROM #TBL_CompleteDetailes B where A.SKU=B.SKU)
			GROUP BY SKU  

			SELECT F.ZnodeProductId,[Path] AS ImagePath
			INTO #ProductImage
			FROM ZnodePimAttributeValue a
			INNER JOIN ZnodePimProductAttributeMedia b on a.PimAttributeValueId = b.PimAttributeValueId
			INNER JOIN ZnodeMedia d on b.MediaId = d.MediaId
			INNER JOIN ZnodePublishProductEntity F ON  A.PimProductId = F.ZnodeProductId 
			WHERE EXISTS(SELECT * FROM ZnodePimAttribute C WHERE C.PimAttributeId=A.PimAttributeId AND C.AttributeCode='PRODUCTIMAGE' )
			AND EXISTS(SELECT * FROM #TBL_CompleteDetailes E Where E.ZnodeProductId = F.ZnodeProductId)		

						 
			SELECT 
			DomainName AS DomainName,
			isnull(ProductName,SEOTitle) as [Title],
			--lastmod,
			--[g:condition],
			[description] AS [Description],
			CAST([g:id] AS nvarchar(50) ) AS ProductID,
			[g:availability] AS [Availability],
			--[g:identifier_exists],	
			--TBCD.PortalId,
			Cast(CAST(CTPL.Price AS numeric(28,3))as nvarchar(100)) AS Price,
			@MediaConfiguration AS MediaConfiguration,
			--TBCD.SEOCode,
			--TBCD.CanonicalURL,
			--TBCD.RobotTag,
			PC.CategoryName AS Category,
			CAST(ID.Inventory AS nvarchar(50) ) AS Inventory,
			BD.Brand AS Brand,
			link AS Link,
			PM.IMAGEPATH AS Image_Link,
			CTPL.SKU AS ID
			FROM #TBL_CompleteDetailes AS TBCD   
			LEFT JOIN #Cte_PortalList AS CTPL ON(CTPL.PortalId = TBCD.PortalId AND CTPL.SKU = TBCD.SKU AND CTPL.RowID = 1)  
			LEFT JOIN #ProductCategories PC ON TBCD.ZnodeProductId = PC.PublishProductId
			LEFT JOIN #InventotyDetails ID ON TBCD.SKU = ID.SKU 
			LEFT JOIN #BrandDetails BD on TBCD.ZnodeProductId=BD.ZnodeProductId
			LEFT JOIN #ProductImage PM on TBCD.ZnodeProductId=PM.ZnodeProductId
			WHERE  EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = TBCD.PortalId)

		IF OBJECT_ID('tempdb..#TBL_DomainName') IS NOT NULL
		DROP TABLE #TBL_DomainName

		IF OBJECT_ID('tempdb..#TBL_SEODetails') IS NOT NULL
		DROP TABLE #TBL_SEODetails

		IF OBJECT_ID('tempdb..#TBL_CompleteDetailes') IS NOT NULL
		DROP TABLE #TBL_CompleteDetailes

		IF OBJECT_ID('tempdb..#TBL_CompleteDetailes') IS NOT NULL
		DROP TABLE #TBL_CompleteDetailes

		IF OBJECT_ID('tempdb..#TBL_PortalIds') IS NOT NULL
		DROP TABLE #TBL_PortalIds

		IF OBJECT_ID('tempdb..#TBL_PricePrecedence') IS NOT NULL
		DROP TABLE #TBL_PricePrecedence
		
END TRY  
BEGIN CATCH  
	DECLARE @Status BIT ;  
	SET @Status = 0;  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetXmlProductFeedList @PortalId = '+@PortalId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@FeedType='+CAST(@FeedType AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
     
	EXEC Znode_InsertProcedureErrorLog  
	@ProcedureName = 'Znode_GetXmlProductFeedList',  
	@ErrorInProcedure = @Error_procedure,  
	@ErrorMessage = @ErrorMessage,  
	@ErrorLine = @ErrorLine,  
	@ErrorCall = @ErrorCall;  
END CATCH  
   
END;