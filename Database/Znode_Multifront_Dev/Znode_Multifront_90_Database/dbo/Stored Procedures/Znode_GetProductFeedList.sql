CREATE PROCEDURE [dbo].[Znode_GetProductFeedList]  
		(
		@PortalId   VARCHAR(2000) = NULL,  
		@SKU  SelectColumnList READONLY,  
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
 EXEC [Znode_GetProductFeedList] @PortalId='0',@ProductIds = '116,117,118'  
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

		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		CREATE TABLE #TBL_DomainName 
		(PortalId   INT,
		DomainName NVARCHAR(300),
		RowId      INT
		);  	
         
		CREATE TABLE #TBL_SEODetails   
		(loc                   NVARCHAR(MAX),  
		lastmod               DATETIME,  
		[g:condition]         VARCHAR(100),  
		[description]         NVARCHAR(MAX),  
		[g:id]                INT,  
		link                  VARCHAR(100),  
		[g:identifier_exists] VARCHAR(200),  
		DomainName            NVARCHAR(300),  
		PortalId              INT , 
		SEOCode             NVARCHAR(4000), 
		CanonicalURL        VARCHAR(200), 
		RobotTag            VARCHAR(50)
		);  
		--CREATE TABLE #TBL_CompleteDetailes   
		--(loc                   NVARCHAR(MAX),  
		--lastmod               DATETIME,  
		--[g:condition]         VARCHAR(100),  
		--[description]         NVARCHAR(MAX),  
		--[g:id]                INT,  
		--link                  VARCHAR(100),  
		--[g:identifier_exists] VARCHAR(200),  
		--DomainName            NVARCHAR(300),  
		--PortalId              INT,  
		--[g:availability]      NVARCHAR(1000),  
		--SKU                   NVARCHAR(1000),  
		--SEOCode               NVARCHAR(4000),
		--CanonicalURL        VARCHAR(200), 
		--RobotTag            VARCHAR(50) 
		--);  
		DECLARE @DefaultLocaleId INT=dbo.Fn_GetDefaultLocaleId()  ;
		CREATE TABLE #TBL_PortalIds (PortalId INT);  
			
		select * into #SKU from @SKU
		create index IDX_#SKU on #SKU(StringColumn)

		INSERT INTO #TBL_PortalIds  
		SELECT Zp.PortalId   
		FROM Znodeportal AS ZP   
		INNER JOIN ZnodePortalCatalog AS ZPC ON(ZPC.PortalId = Zp.PortalId)  
		INNER JOIN ZnodePublishCatalog AS ZPPC ON(ZPPC.PimCatalogId = ZPC.PublishCatalogId)
		INNER JOIN ZnodePublishProductEntity AS ZPPE ON (ZPPC.PimCatalogId  = ZPPE.ZnodeCatalogId)
		WHERE EXISTS(SELECT TOP 1 1 FROM #SKU AS Sp WHERE sp.StringColumn  = ZPPE.SKU OR StringColumn = '0')  
		AND EXISTS(SELECT TOP 1 1 FROM DBO.Split(@PortalId, ',') AS Sp  
		WHERE(CAST(sp.Item AS INT)) = Zp.PortalId  OR @PortalId = '0')  
		AND EXISTS (SELECT TOP 1 1 FROM ZnodeDomain ZD WHERE ZP.PortalId = ZD.PortalId  
		AND IsActive = 1 AND ApplicationType = 'Webstore' )  
		GROUP BY Zp.PortalId;   
		
		INSERT INTO #TBL_DomainName   
		SELECT  TOP 1  PortalId,DomainName,ROW_NUMBER() OVER(PARTITION BY PortalId ORDER BY DomainName)   
		FROM ZnodeDomain AS ZD   
		WHERE EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZD.PortalId)  
		AND IsActive = 1 AND ApplicationType = 'Webstore' AND IsDefault = 1 

		if not exists (select top 1 1 from #TBL_DomainName)
		INSERT INTO #TBL_DomainName   
		SELECT  TOP 1 PortalId,DomainName,ROW_NUMBER() OVER(PARTITION BY PortalId ORDER BY DomainName)   
		FROM ZnodeDomain AS ZD   
		WHERE EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZD.PortalId)  
		AND IsActive = 1 AND ApplicationType = 'Webstore' AND IsDefault = 0 
 
		IF EXISTS(SELECT * FROM #SKU)
        BEGIN
		    --INSERT INTO #Cte_SeoDetailsWithLocale(CMSSEODetailId ,loc ,lastmod ,[g:condition] ,[description] ,[g:id] ,link ,[g:identifier_exists] ,DomainName ,PortalId,LocaleId ,SEOCode ,CanonicalURL ,RobotTag)
			SELECT DISTINCT ZCSD.CMSSEODetailId,ZCSD.SEOURL AS loc,ZCSD.ModifiedDate AS lastmod,'new' AS [g:condition],ZCSDL.SEODescription AS [description],ZPCC.ZnodeProductId AS [g:id],  
            '' AS link,'false' AS [g:identifier_exists],TBDN.DomainName,ZPC.PortalId,ISNULL(ZCSDL.LocaleId, @DefaultLocaleId) AS LocaleId , ZCSD.SEOCode , ZCSDL.CanonicalURL, ZCSDL.RobotTag 
			INTO #Cte_SeoDetailsWithLocale
			FROM ZnodePublishProductEntity AS ZPCC   
			INNER JOIN ZnodePortalCatalog AS ZPC ON(ZPC.PublishCatalogId = ZPCC.ZnodeCatalogId)  
			--INNER JOIN @TBL_PortalIds TBLP ON (TBLP.PortalId = ZPC.PortalId)  
			LEFT JOIN ZnodeCMSSEODetail AS ZCSD ON(ZPCC.SKU = ZCSD.SEOCode and ZCSD.PortalId = ZPC.PortalId)  
			LEFT JOIN ZnodeCMSSEOType AS ZCST ON(ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId AND ZCST.Name = 'Product')  
			LEFT JOIN ZnodeCMSSEODetailLocale AS ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId AND ZCSDL.LocaleId IN(@LocaleId, @DefaultLocaleId))  
			LEFT JOIN #TBL_DomainName AS TBDN ON(TBDN.RowId = 1 AND TBDN.PortalId = zpc.PortalId )   
			WHERE EXISTS(SELECT TOP 1 1 FROM #SKU AS Sp  
			WHERE (sp.StringColumn  = ZPCC.SKU) )--OR StringColumn = '0')  
			AND EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZPC.PortalId)  
			
			SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId,LocaleId,SEOCode, CanonicalURL, RobotTag  
			INTO #Cte_SeoDetailsWithFirstLocale
			FROM #Cte_SeoDetailsWithLocale   
			WHERE LocaleId = @LocaleId  
    
			SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId,LocaleId,SEOCode , CanonicalURL, RobotTag 
			INTO #Cte_SeoDetailsWithDefaultLocale
			FROM #Cte_SeoDetailsWithFirstLocale  
			
			INSERT INTO #Cte_SeoDetailsWithDefaultLocale 
			SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId,LocaleId,SEOCode, CanonicalURL, RobotTag  
			FROM #Cte_SeoDetailsWithLocale AS CTSDWL  
			WHERE LocaleId = @DefaultLocaleId   
			AND NOT EXISTS(SELECT TOP 1 1 FROM #Cte_SeoDetailsWithFirstLocale AS CTSDWDL WHERE CTSDWDL.CMSSEODetailId = CTSDWL.CMSSEODetailId)  
          
			INSERT INTO #TBL_SEODetails  
			SELECT DISTINCT loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId ,SEOCode, CanonicalURL, RobotTag  
			FROM #Cte_SeoDetailsWithDefaultLocale;  

			--CREATE INDEX Idx_#TBL_SEODetails ON #TBL_SEODetails([g:id])

			SELECT DISTINCT TBSD.loc,TBSD.lastmod,TBSD.[g:condition],TBSD.[description],TBSD.[g:id],TBSD.link,TBSD.[g:identifier_exists],TBSD.DomainName,TBSD.PortalId,  
			CASE WHEN SUM(ZI.Quantity) > 0 THEN 'In Stock' ELSE CASE WHEN @FeedType = 'Google' THEN 'Out Of Stock' ELSE 'Not In Stock' END  
			END AS [g:availability],
			ZPP.SKU ,TBSD.SEOCode, TBSD.CanonicalURL, TBSD.RobotTag
			INTO #TBL_CompleteDetailes  
			FROM ZnodePublishProductEntity AS ZPP   
			LEFT JOIN #TBL_SEODetails AS TBSD ON(ZPP.ZnodeProductId = TBSD.[g:id] )  
			LEFT JOIN ZnodePortalWarehouse AS ZPW ON(ZPW.PortalId = TBSD.PortalId)  
			LEFT JOIN ZnodePortalAlternateWarehouse AS ZAPW ON(ZAPW.PortalWarehouseId = ZPW.PortalWarehouseId)  
			LEFT JOIN ZnodeInventory AS ZI ON(ZI.SKU = ZPP.SKU AND (ZI.WarehouseId = ZPW.WarehouseId OR ZI.WarehouseId = ZAPW.WarehouseId))  
			WHERE EXISTS(SELECT TOP 1 1 FROM #SKU AS Sp WHERE (sp.StringColumn  = ZPP.SKU))-- OR sp.StringColumn = '0')
			AND EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = TBSD.PortalId)	
			GROUP BY loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,TBSD.PortalId,ZPP.SKU,ZPP.LocaleId, TBSD.SEOCode, TBSD.CanonicalURL, TBSD.RobotTag;    
			
			DECLARE @MediaConfiguration NVARCHAR(2000)=((SELECT TOP 1 ISNULL(CASE WHEN CDNURL = '' THEN NULL ELSE CDNURL END,URL) FROM ZnodeMediaConfiguration WHERE IsActive = 1));    
  
			CREATE INDEX Idx_#TBL_CompleteDetailes ON #TBL_CompleteDetailes(PortalId,SKU)
	
	
			SELECT DISTINCT zp.PortalId,round(ZPS.RetailPrice,2) as RetailPrice,Zps.SKU,tbcd.SEOCode,ROW_NUMBER() OVER(PARTITION BY Zps.SKU,zp.PortalId ORDER BY ZPS.RetailPrice) AS RowId  
			INTO #Cte_PortalList
			FROM ZnodePriceList AS ZPL   
			LEFT JOIN ZnodePriceListPortal AS ZPLP ON ZPL.PriceListId = ZPLP.PriceListId  
			LEFT JOIN ZnodeCulture AS zc ON ZPL.CultureId = zc.CultureId
			LEFT JOIN ZnodePortal AS zp ON ZPLP.PortalId = zp.PortalId  
			LEFT JOIN ZnodePrice AS Zps ON(Zps.PriceListId = ZPL.PriceListId)   
			LEFT JOIN #TBL_CompleteDetailes AS TBCD ON(TBCD.PortalId = Zp.PortalId AND TBCD.SKU = Zps.Sku)   
			WHERE CAST(@GetDate AS DATE) BETWEEN ZPL.ActivationDate AND ZPL.ExpirationDate   
			AND EXISTS( SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZPLP.PortalId)   
			GROUP BY zp.PortalId,ZPS.RetailPrice,Zps.SKU ,TBCD.SEOCode  
 
			SELECT loc,lastmod,[g:condition],[description],[g:id],link,[g:availability],[g:identifier_exists],DomainName,TBCD.PortalId  
			,CTPL.RetailPrice AS [g:price],@MediaConfiguration AS MediaConfiguration, TBCD.SEOCode, TBCD.CanonicalURL, TBCD.RobotTag 
			FROM #TBL_CompleteDetailes AS TBCD   
			LEFT JOIN #Cte_PortalList AS CTPL ON(CTPL.PortalId = TBCD.PortalId AND CTPL.SKU = TBCD.SKU AND CTPL.RowID = 1)  
			WHERE  EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = TBCD.PortalId )  


		END
     	ELSE
		BEGIN
		    --INSERT INTO #Cte_SeoDetailsWithLocale(CMSSEODetailId ,loc ,lastmod ,[g:condition] ,[description] ,[g:id] ,link ,[g:identifier_exists] ,DomainName ,PortalId,LocaleId ,SEOCode ,CanonicalURL ,RobotTag)
			SELECT DISTINCT ZCSD.CMSSEODetailId,ZCSD.SEOURL AS loc,ZCSD.ModifiedDate AS lastmod,'new' AS [g:condition],ZCSDL.SEODescription AS [description],ZPCC.ZnodeProductId AS [g:id],  
			'' AS link,'false' AS [g:identifier_exists],TBDN.DomainName,ZPC.PortalId,ISNULL(ZCSDL.LocaleId, @DefaultLocaleId) AS LocaleId , ZCSD.SEOCode , ZCSDL.CanonicalURL, ZCSDL.RobotTag 
			INTO #Cte_SeoDetailsWithLocale1
			FROM ZnodePublishProductEntity AS ZPCC   
			INNER JOIN ZnodePortalCatalog AS ZPC ON(ZPC.PublishCatalogId = ZPCC.ZnodeCatalogId)  
			-- INNER JOIN @TBL_PortalIds TBLP ON (TBLP.PortalId = ZPC.PortalId)  
			LEFT JOIN ZnodeCMSSEODetail AS ZCSD ON(ZPCC.SKU = ZCSD.SEOCode and ZCSD.PortalId = ZPC.PortalId)  
			LEFT JOIN ZnodeCMSSEOType AS ZCST ON(ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId AND ZCST.Name = 'Product')  
			LEFT JOIN ZnodeCMSSEODetailLocale AS ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId AND ZCSDL.LocaleId IN(@LocaleId, @DefaultLocaleId))  
			LEFT JOIN #TBL_DomainName AS TBDN ON(TBDN.RowId = 1 AND TBDN.PortalId = zpc.PortalId )   
			WHERE EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZPC.PortalId)
			SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId,LocaleId,SEOCode, CanonicalURL, RobotTag  
			INTO #Cte_SeoDetailsWithFirstLocale1
			FROM #Cte_SeoDetailsWithLocale1   
			WHERE LocaleId = @LocaleId  
    
			SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId,LocaleId,SEOCode , CanonicalURL, RobotTag 
			INTO #Cte_SeoDetailsWithDefaultLocale1
			FROM #Cte_SeoDetailsWithFirstLocale1  
			
			INSERT INTO #Cte_SeoDetailsWithDefaultLocale1 
			SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId,LocaleId,SEOCode, CanonicalURL, RobotTag  
			FROM #Cte_SeoDetailsWithLocale1 AS CTSDWL  
			WHERE LocaleId = @DefaultLocaleId   
			AND NOT EXISTS(SELECT TOP 1 1 FROM #Cte_SeoDetailsWithFirstLocale AS CTSDWDL WHERE CTSDWDL.CMSSEODetailId = CTSDWL.CMSSEODetailId)  
          
			INSERT INTO #TBL_SEODetails  
			SELECT DISTINCT loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId ,SEOCode, CanonicalURL, RobotTag  
			FROM #Cte_SeoDetailsWithDefaultLocale1;  
		  
			SELECT TBSD.loc,TBSD.lastmod,TBSD.[g:condition],TBSD.[description],TBSD.[g:id],TBSD.link,TBSD.[g:identifier_exists],TBSD.DomainName,TBSD.PortalId,  
			CASE WHEN SUM(ZI.Quantity) > 0 THEN 'In Stock' ELSE CASE WHEN @FeedType = 'Google' THEN 'Out Of Stock' ELSE 'Not In Stock' END  
			END AS [g:availability],
			ZPP.SKU ,TBSD.SEOCode, TBSD.CanonicalURL, TBSD.RobotTag  
			INTO #TBL_CompleteDetailes1
			FROM ZnodePublishProductEntity AS ZPP   
			LEFT JOIN #TBL_SEODetails AS TBSD ON(ZPP.ZnodeProductId = TBSD.[g:id] )  
			LEFT JOIN ZnodePortalWarehouse AS ZPW ON(ZPW.PortalId = TBSD.PortalId)  
			LEFT JOIN ZnodePortalAlternateWarehouse AS ZAPW ON(ZAPW.PortalWarehouseId = ZPW.PortalWarehouseId)  
			LEFT JOIN ZnodeInventory AS ZI ON(ZI.SKU = ZPP.SKU AND (ZI.WarehouseId = ZPW.WarehouseId OR ZI.WarehouseId = ZAPW.WarehouseId))  
			WHERE EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = TBSD.PortalId )	
			GROUP BY loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,TBSD.PortalId,ZPP.SKU,ZPP.LocaleId, TBSD.SEOCode, TBSD.CanonicalURL, TBSD.RobotTag;    
			

			DECLARE @MediaConfiguration1 NVARCHAR(2000)=((SELECT TOP 1 ISNULL(CASE WHEN CDNURL = '' THEN NULL ELSE CDNURL END,URL) FROM ZnodeMediaConfiguration WHERE IsActive = 1));    
  
			CREATE INDEX Idx_#TBL_CompleteDetailes1 ON #TBL_CompleteDetailes(PortalId,SKU)
	
	
			SELECT DISTINCT zp.PortalId,round(ZPS.RetailPrice,2) as RetailPrice,Zps.SKU,tbcd.SEOCode,ROW_NUMBER() OVER(PARTITION BY Zps.SKU,zp.PortalId ORDER BY ZPS.RetailPrice) AS RowId  
			INTO #Cte_PortalList1
			FROM ZnodePriceList AS ZPL   
			LEFT JOIN ZnodePriceListPortal AS ZPLP ON ZPL.PriceListId = ZPLP.PriceListId  
			LEFT JOIN ZnodeCulture AS zc ON ZPL.CultureId = zc.CultureId
			LEFT JOIN ZnodePortal AS zp ON ZPLP.PortalId = zp.PortalId  
			LEFT JOIN ZnodePrice AS Zps ON(Zps.PriceListId = ZPL.PriceListId)   
			LEFT JOIN #TBL_CompleteDetailes AS TBCD ON(TBCD.PortalId = Zp.PortalId AND TBCD.SKU = Zps.Sku)   
			WHERE CAST(@GetDate AS DATE) BETWEEN ZPL.ActivationDate AND ZPL.ExpirationDate   
			AND EXISTS( SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = ZPLP.PortalId)   
			GROUP BY zp.PortalId,ZPS.RetailPrice,Zps.SKU ,TBCD.SEOCode  
 
			SELECT loc,lastmod,[g:condition],[description],[g:id],link,[g:availability],[g:identifier_exists],DomainName,TBCD.PortalId  
			,CTPL.RetailPrice AS [g:price],@MediaConfiguration AS MediaConfiguration, TBCD.SEOCode, TBCD.CanonicalURL, TBCD.RobotTag 
			FROM #TBL_CompleteDetailes1 AS TBCD   
			LEFT JOIN #Cte_PortalList1 AS CTPL ON(CTPL.PortalId = TBCD.PortalId AND CTPL.SKU = TBCD.SKU AND CTPL.RowID = 1)  
			WHERE  EXISTS(SELECT TOP 1 1 FROM #TBL_PortalIds AS TBP WHERE TBP.PortalId = TBCD.PortalId )  

		END
 
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
		
		END TRY  
		BEGIN CATCH  
			DECLARE @Status BIT ;  
			SET @Status = 0;  
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductFeedList @PortalId = '+@PortalId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@FeedType='+CAST(@FeedType AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
     
			EXEC Znode_InsertProcedureErrorLog  
			@ProcedureName = 'Znode_GetProductFeedList',  
			@ErrorInProcedure = @Error_procedure,  
			@ErrorMessage = @ErrorMessage,  
			@ErrorLine = @ErrorLine,  
			@ErrorCall = @ErrorCall;  
		END CATCH  
		END
		;