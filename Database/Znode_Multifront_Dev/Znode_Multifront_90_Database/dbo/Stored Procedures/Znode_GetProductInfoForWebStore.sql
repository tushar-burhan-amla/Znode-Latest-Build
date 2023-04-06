CREATE PROCEDURE [dbo].[Znode_GetProductInfoForWebStore]
(   
	@PortalId         INT,
    @LocaleId         INT,
	@UserId			  INT = 2,
	@currentUtcDate    VARCHAR(200) = '',
	@ProductDetailsFromWebStore   DBO.ProductDetailsFromWebStore READONLY,
	@IsAllLocation bit =0)
AS 
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 
			 DECLARE @Tlb_SKU TABLE (SKU VARCHAR(100))
			 DECLARE @PublishProductIds  NVARCHAR(max) ,@SKU NVARCHAR(max) 

			 DECLARE @Fn_GetDefaultLocaleId int = Dbo.Fn_GetDefaultLocaleId()
				
			 DECLARE @TBL_PricebyCatalogforAssociateProduct TABLE (PimProductId int ,AssociatedProductId int,ParentSKU NVARCHAR(300),
				ChildSKU NVARCHAR(300),RetailPrice  numeric(28,6),AssociatedProductDisplayOrder int ,
				TypeOfProduct nvarchar(100),SalesPrice  numeric(28,6))
		     DECLARE @tbl_PricingSkuOfAssociatedProduct TABLE (sku nvarchar(200),RetailPrice numeric(28,6),SalesPrice numeric(28,6),TierPrice numeric(28,6),
				TierQuantity numeric(28,6),CurrencyCode varchar(200),CurrencySuffix varchar(2000), ExternalId NVARCHAR(2000))				
		

			--Create TABLE #Tlb_PromotionProductData 
   --          (
			--	  PromotionId      INT,
			--	  PromotionType	   INT, 
			--	  ExpirationDate   Datetime, 
			--	  ActivationDate   Datetime,
			--	  PublishProductId INT,
			--	  PromotionMessage Nvarchar(max)  
   --          );
			 Create TABLE #Tbl_PriceListWisePrice 
             (
				  SKU            VARCHAR(300),
				  RetailPrice    NUMERIC(28, 6),
				  SalesPrice     NUMERIC(28, 6),
				  TierPrice      NUMERIC(28, 6),
				  TierQuantity   NUMERIC(28, 6),
				  CurrencyCode   Varchar(100),
				  CurrencySuffix Varchar(1000),
				  CultureCode      VARCHAr(1000),
				  ExternalId NVARCHAR(2000),
				  Custom1        NVARCHAR(MAX),
				  Custom2        NVARCHAR(MAX),
				  Custom3        NVARCHAR(MAX)
             );

			 CREATE TABLE #Tlb_ProductData 
             (
				  PublishProductId INT,
				  SKU              NVARCHAR(100),
				  SEOTitle         NVARCHAR(200),
				  SEODescription   NVARCHAR(MAX),
				  SEOKeywords      NVARCHAR(MAX),
				  SEOUrl           NVARCHAR(MAX),
				  Rating           Numeric(28,6),
				  TotalReviews     INT,
				  RetailPrice      NUMERIC(28, 6),
				  SalesPrice       NUMERIC(28, 6),
				  TierPrice        NUMERIC(28, 6),
				  TierQuantity     NUMERIC(28, 6),
				  CurrencyCode     Varchar(100),
				  CurrencySuffix   Varchar(1000),
				
				  ExternalId       NVARCHAR(2000),
				  Quantity    NUMERIC(28, 6),
				  ReOrderLevel     NUMERIC(28, 6),
				  Custom1        NVARCHAR(MAX),
				  Custom2        NVARCHAR(MAX),
				  Custom3        NVARCHAR(MAX),
				  CanonicalURL VARCHAR(200),   
				  RobotTag VARCHAR(50)
			   );


			 Create TABLE #Tbl_Inventory
             (
				  SKU            VARCHAR(300),
				  Quantity    NUMERIC(28, 6),
				  ReOrderLevel     NUMERIC(28, 6),
				  PortalId      int,
				  WarehouseName	varchar(100),
			      WarehouseCode	varchar(100),
			      DefaultInventoryCount varchar(1000),
				
             );
			 
			 Create TABLE #Tbl_WarehouseWiseInventory
             (
				  SKU            VARCHAR(300),
				  Quantity    NUMERIC(28, 6),
				  ReOrderLevel NUMERIC(28, 6),
				  PortalId      int,
				  WarehouseCode VARCHAR(100),
				  WarehouseName VARCHAR(100),
				  IsDefaultWarehouse BIT

             );

            INSERT INTO #Tlb_ProductData (PublishProductId,SKU)
            SELECT id,SKU FROM @ProductDetailsFromWebStore
			  		
			SELECT @SKU = Substring((SELECT ',' + SKU FROM @ProductDetailsFromWebStore FOR XML PAth('')),2,4000) 

			SELECT @PublishProductIds = Substring((SELECT ',' + CONVERT(NVARCHAR(100),id ) FROM @ProductDetailsFromWebStore FOR XML PAth('')),2,4000) 
			
			--INSERT INTO  #Tlb_PromotionProductData(PromotionId,PromotionType, ExpirationDate,  ActivationDate, PublishProductId,PromotionMessage)
			--Exec [Znode_GetPromotionByPublishProductId] @PublishProductIds = @PublishProductIds ,@UserId  = @UserId	,@PortalId  = @PortalId  
			 
			INSERT INTO #Tbl_PriceListWisePrice( SKU, RetailPrice,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix,CultureCode,ExternalId,Custom1,Custom2,Custom3)
			EXEC Znode_GetPublishProductPricingBySku @SKU = @SKU ,@PortalId = @PortalId ,@currentUtcDate = @currentUtcDate,@UserId = @UserId 

			IF @IsAllLocation=1
			BEGIN 
				Insert into #Tbl_WarehouseWiseInventory(SKU,	Quantity,ReOrderLevel,PortalId,	WarehouseCode,	WarehouseName,	IsDefaultWarehouse)
				EXEC Znode_GetWarehouseInventoryBySkus  @SKUs=@SKU,@PortalId=@PortalId
			END

			insert into #Tbl_Inventory (SKU,	Quantity,	ReOrderLevel,	PortalId, WarehouseName, WarehouseCode, DefaultInventoryCount)
			EXEC Znode_GetInventoryBySkus @SKUs=@SKU,@PortalId=@PortalId
			
			--Price logic for Associate products
			----INSERT INTO @TBL_PricebyCatalogforAssociateProduct(AssociatedProductId,ChildSKU,ParentSKU,PimProductId,RetailPrice,SalesPrice,TypeOfProduct)
			----SELECT cl.Item, NULL , PR.SKU, PR.ID, null, null , PR.[ProductType]  FROM @ProductDetailsFromWebStore PR
			----Cross Apply dbo.split (AssociateProducts, ',') CL 

			----UPDATE PDI SET PDI.ChildSKU = ZPPD.SKU 
			----from @TBL_PricebyCatalogforAssociateProduct PDI inner join
			----ZnodePublishProductDetail ZPPD On PDI.AssociatedProductId = ZPPD.PublishProductId
			
			----SELECT @SKU = Substring((SELECT ',' + Convert(nvarchar(100),AssociatedProductId) 
			----FROM @TBL_PricebyCatalogforAssociateProduct where AssociatedProductId is not null FOR XML PAth('')),2,4000) 

			----INSERT INTO @tbl_PricingSkuOfAssociatedProduct (SKU,RetailPrice ,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix, ExternalId)	
			----EXEC Znode_GetPublishProductPricingBySku  @Sku ,@portalID  ,@currentUtcDate,@UserId 

			----update PLC SET PLC.RetailPrice = PLCA.RetailPrice ,
			----PLC.SalesPrice = PLCA.SalesPrice 
			----from @TBL_PricebyCatalogforAssociateProduct PLC inner join @tbl_PricingSkuOfAssociatedProduct
			----PLCA on PLC.ChildSKU = PLCA.sku
			
			----Update PBC SET PBC.RetailPrice = 
			----	(Select TOP 1 Isnull(RetailPrice ,SalesPrice) from @TBL_PricebyCatalogforAssociateProduct PCBA  where PCBA.ParentSKU =PBC.SKU
			----		and PCBA.ParentSKU is not null and PCBA.ChildSKU is not null
			----	Order by AssociatedProductDisplayOrder)
			----	from #Tbl_PriceListWisePrice  PBC  where 
			----	Exists (Select TOP 1 1  from @TBL_PricebyCatalogforAssociateProduct PCBA
			----	where PCBA.ParentSKU =PBC.SKU and PCBA.TypeOfProduct = 'ConfigurableProduct')
			----	and PBC.RetailPrice IS null 

			Update PD SET 
			 PD.SKU             = PLWP.SKU            
			,PD.RetailPrice     = PLWP.RetailPrice     
			,PD.SalesPrice      = PLWP.SalesPrice      
			,PD.TierPrice       = PLWP.TierPrice       
			,PD.TierQuantity    = PLWP.TierQuantity    
			,PD.CurrencyCode    = PLWP.CurrencyCode    
			,PD.CurrencySuffix  = PLWP.CurrencySuffix  
			,PD.ExternalId 	    = PLWP.ExternalId
			,PD.Custom1			= PLWP.Custom1
			,PD.Custom2			= PLWP.Custom2
			,PD.Custom3			= PLWP.Custom3
			FROM #Tlb_ProductData PD Inner join #Tbl_PriceListWisePrice PLWP on 
			PD.SKU = PLWP.SKU

			Update PD SET 
			 PD.Quantity = TLI.Quantity,
			 PD.ReOrderLevel= TLI.ReOrderLevel
			 FROM #Tlb_ProductData PD Inner join #Tbl_Inventory TLI on 
			PD.SKU = TLI.SKU

			 

			----Update PD SET 
			----	  PD.PromotionId      =PLWP.PromotionId,
			----	  PD.PromotionType	  =PLWP.PromotionType, 
			----	  PD.ExpirationDate   =PLWP.ExpirationDate, 
			----	  PD.ActivationDate   =PLWP.ActivationDate,
			----	  PD.PublishProductId =PLWP.PublishProductId,
			----	  PD.PromotionMessage  =PLWP.PromotionMessage   
			----from #Tlb_ProductData PD Inner join #Tlb_PromotionProductData PLWP on 
			----PD.PublishProductId = PLWP.PublishProductId


			 DECLARE @Tlb_CustomerAverageRatings TABLE
             (PublishProductId INT,
              Rating           NUMERIC(28,6),
              TotalReviews     INT
             ); 
             -- Calculate Average rating 
             INSERT INTO @Tlb_CustomerAverageRatings(PublishProductId,Rating,TotalReviews)
             SELECT CCR.PublishProductId,SUM(CAST(CCR.Rating AS NUMERIC(28,6)) )/ COUNT(CCR.PublishProductId),COUNT(CCR.PublishProductId) 
			 FROM ZnodeCMSCustomerReview AS CCR
             INNER JOIN #Tlb_ProductData AS PD ON CCR.PublishProductId = PD.PublishProductId AND CCR.Status = 'A' 
			 AND  (CCR.PortalId  = @PortalId OR @PortalId = 0 )
			 GROUP BY CCR.PublishProductId    ;

             UPDATE PD SET PD.Rating = CAR.Rating,PD.TotalReviews = CAR.TotalReviews 
			 FROM @Tlb_CustomerAverageRatings CAR
             INNER JOIN #Tlb_ProductData PD ON CAR.PublishProductId = PD.PublishProductId;

             UPDATE PD SET PD.SEOTitle = ZCSDL.SEOTitle,PD.SEODescription = ZCSDL.SEODescription,PD.SEOKeywords = ZCSDL.SEOKeywords,PD.SEOUrl = ZCSO.SEOUrl,
			               PD.CanonicalURL = ZCSDL.CanonicalURL, PD.RobotTag = ZCSDL.RobotTag
			 FROM #Tlb_ProductData PD
             INNER JOIN ZnodeCMSSEODetail ZCSO ON PD.SKU = ZCSO.SEOCode
             LEFT JOIN ZnodeCMSSEODetailLocale ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSO.CMSSEODetailId AND ZCSDL.LocaleId = @LocaleId)
             INNER JOIN ZnodeCMSSEOType ZCOT ON ZCOT.CMSSEOTypeId = ZCSO.CMSSEOTypeId AND ZCOT.Name = 'Product'
			 WHERE ZCSO.PortalId = @PortalId
             
			 --UPDATE PD SET PD.SEOTitle = ZCSDL.SEOTitle,PD.SEODescription = ZCSDL.SEODescription,PD.SEOKeywords = ZCSDL.SEOKeywords,PD.SEOUrl = ZCSO.SEOUrl 
			 --FROM #Tlb_ProductData PD
    --         INNER JOIN ZnodeCMSSEODetail ZCSO ON PD.SKU = ZCSO.SEOCode
    --         LEFT JOIN ZnodeCMSSEODetailLocale ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSO.CMSSEODetailId AND ZCSDL.LocaleId = @LocaleId)
    --         INNER JOIN ZnodeCMSSEOType ZCOT ON ZCOT.CMSSEOTypeId = ZCSO.CMSSEOTypeId AND ZCOT.Name = 'Product'
			 --WHERE ZCSO.PortalId = @PortalId

             UPDATE PD SET PD.SEOTitle = ZCPS.ProductTitle,PD.SEODescription = ZCPS.ProductDescription,PD.SEOKeywords = ZCPS.ProductKeyword FROM #Tlb_ProductData PD
             INNER JOIN ZnodeCMSPortalSEOSetting ZCPS ON ZCPS.PortalId = @PortalId WHERE PD.SEOTitle IS NULL AND PD.SEODescription IS NULL AND PD.SEOKeywords IS NULL AND PD.SEOUrl IS NULL
			  --AND ZCSO.PortalId = @PortalId

			 
			 -- ;With Cte_Catalogdaata AS 
			 --(
			   SELECT  a.PublishCatalogId ,Max(PublishCatalogLogId) PublishCatalogLogId
			   into #Cte_Catalogdaata
			   FROM ZnodePortalCatalog a 
			   INNER JOIN ZnodePublishCatalogLog b ON (b.PublishCatalogId = a.PublishCatalogId )
			   WHERE a.PortalId = @PortalId
			   GROUP BY a.PublishCatalogId 
	 
			 --)
			 SELECT Row_Number()Over( PARTITION BY  BTY.SKU ORDER BY ZPAP.DisplayOrder, ZPAP.PublishAssociatedProductId) RowId ,
					BTY.SKU ParentSKU, BTY1.SKU --, UI.Quantity, UI.ReOrderLevel ,UI.WarehouseId
			 INTO #TempPublishData
			 FROM ZnodePublishProduct CTR 
			 INNER JOIN ZnodePublishProductDetail BTY ON (BTY.PublishProductId = CTR.PublishProductId)
			 INNER JOIN ZnodePublishAssociatedProduct ZPAP ON (ZPAP.ParentPimProductId = CTR.PimProductId)	
			 INNER JOIN ZnodePublishProduct CTR1 ON (ZPAP.PimProductId = CTR1.PimProductId)
			 INNER JOIN ZnodePublishProductDetail BTY1 ON (BTY1.PublishProductId = CTR1.PublishProductId)
			 --LEFT JOIN ZnodeInventory UI ON (UI.SKU = BTY1.SKU)
			 WHERE ZPAP.IsConfigurable = 1  
			 AND EXISTS (SELECT TOP 1 1 FROM #Cte_Catalogdaata TY WHERE TY.PublishCatalogId = CTR.PublishCatalogId )-- AND TY.PublishCatalogLogId =ZPXML.PublishCatalogLogId)
			 AND EXISTS (SELECT TOP 1 1 FROM #Tlb_ProductData TU WHERE TU.SKU = BTY.SKU)
			 AND BTY.LocaleId = @Fn_GetDefaultLocaleId
			 AND BTY1.LocaleId = @Fn_GetDefaultLocaleId

			 alter table #TempPublishData add Quantity numeric(28,6), ReOrderLevel numeric(28,6), WarehouseId int

			 update TPD set Quantity = UI.Quantity ,  ReOrderLevel = UI.ReOrderLevel , WarehouseId = UI.WarehouseId
			 from #TempPublishData TPD
			 inner join ZnodeInventory UI ON (UI.SKU = TPD.SKU)

			DELETE FROM #TempPublishData WHERE RowId <> 1
			IF @IsAllLocation=1
			BEGIN 
				SELECT A.PublishProductId,a.SKU,a.SEOTitle,a.SEODescription,a.SEOKeywords,a.SEOUrl,a.Rating,a.TotalReviews ,  
				a.RetailPrice,a.SalesPrice,a.TierPrice, a.TierQuantity,a.CurrencyCode,a.CurrencySuffix,a.ExternalId, 
				CASE WHEN TYI.ParentSKU IS NULL AND ZPCPA.PimProductId IS NULL THEN  b.Quantity ELSE ISNULL(TYI.Quantity,0) END as Quantity , 
				CASE WHEN TYI.ParentSKU IS NULL THEN  b.ReOrderLevel ELSE TYI.ReOrderLevel END ReOrderLevel, a.CanonicalURL, a.RobotTag,
				INV.Quantity AllLocationQuantity,
				b.WarehouseCode,b.WarehouseName,ISNULL(b.IsDefaultWarehouse,0) AS IsDefaultWarehouse, INV.DefaultInventoryCount
				FROM #Tlb_ProductData a
				LEFT JOIN #TempPublishData TYI ON (TYI.ParentSKU = a.SKU AND  TYI.WarehouseId  IN (SELECT  WarehouseId FROM ZnodePortalWarehouse WHERE PortalId = @PortalId))
				LEFT JOIN  #Tbl_WarehouseWiseInventory   b ON b.SKU  = a.SKU   
				LEFT JOIN ZnodePublishProduct ZPP on a.PublishProductId = ZPP.PublishProductId 
				LEFT join ZnodePimConfigureProductAttribute ZPCPA on ZPP.PimProductId = ZPCPA.PimProductId
				LEFT JOIN #Tbl_Inventory INV ON b.SKU = INV.SKU
			END
			ELSE 
			Begin 
			declare @wid int;
			SELECT  @wid=WarehouseId FROM ZnodePortalWarehouse WHERE PortalId = @PortalId
			 SELECT A.PublishProductId,a.SKU,a.SEOTitle,a.SEODescription,a.SEOKeywords,a.SEOUrl,a.Rating,a.TotalReviews ,  
					a.RetailPrice,a.SalesPrice,a.TierPrice, a.TierQuantity,a.CurrencyCode,a.CurrencySuffix,a.ExternalId, 
					CASE WHEN TYI.ParentSKU IS NULL AND ZPCPA.PimProductId IS NULL THEN  ISNULL(ISNULL(b.Quantity,a.Quantity),0) ELSE ISNULL(TYI.Quantity,0) END as Quantity , 
			 CASE WHEN TYI.ParentSKU IS NULL THEN  b.ReOrderLevel ELSE TYI.ReOrderLevel END ReOrderLevel, a.CanonicalURL, a.RobotTag,
			 ISNULL(ISNULL(b.Quantity,a.Quantity),0) AllLocationQuantity
			 FROM #Tlb_ProductData a
			 LEFT JOIN #TempPublishData TYI ON (TYI.ParentSKU = a.SKU AND  TYI.WarehouseId  IN (SELECT  WarehouseId FROM ZnodePortalWarehouse WHERE PortalId = @PortalId))
			 LEFT JOIN ZnodeInventory b ON (b.SKU  = a.SKU  AND  b.WarehouseId  IN (SELECT  WarehouseId FROM ZnodePortalWarehouse WHERE PortalId = @PortalId)) 
			 LEFT JOIN ZnodePimProduct ZPP on a.PublishProductId = ZPP.PimProductId 
			 LEFT join ZnodePimConfigureProductAttribute ZPCPA on ZPP.PimProductId = ZPCPA.PimProductId
			 LEFT JOIN #Tbl_Inventory INV ON b.SKU = INV.SKU
			
			 END
	
		
         END TRY
         BEGIN CATCH
		
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
			 @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
			  @ErrorLine VARCHAR(100)= ERROR_LINE(),
			   @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductInfoForWebStore @PortalId='+CAST(@PortalId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetProductInfoForWebStore',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;