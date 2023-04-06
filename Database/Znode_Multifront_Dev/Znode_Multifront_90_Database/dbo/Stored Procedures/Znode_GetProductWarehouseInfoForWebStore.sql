CREATE PROCEDURE [dbo].[Znode_GetProductWarehouseInfoForWebStore]
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
				  WarehouseCityName        NVARCHAR(MAX),
				  WarehouseStateName        NVARCHAR(MAX),
				  WarehousePostalCode        NVARCHAR(MAX)
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
				  WarehouseCityName        NVARCHAR(MAX),
				  WarehouseStateName        NVARCHAR(MAX),
				  WarehousePostalCode        NVARCHAR(MAX),
				  CanonicalURL VARCHAR(200),   
				  RobotTag VARCHAR(50)
			   );


			 Create TABLE #Tbl_Inventory
             (
				  SKU            VARCHAR(300),
				  Quantity    NUMERIC(28, 6),
				  ReOrderLevel     NUMERIC(28, 6),
				  PortalId      int
				
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

			Create Table #WareHouseAddress 
			(
				WarehouseCode varchar(100), 
				CiyName varchar(1000), 
				StateName varchar(1000), 
				PostalCode varchar(50), 
				WarehouseAddress varchar(2000)
			)

			insert into #WareHouseAddress(WarehouseCode, CiyName, StateName, PostalCode, WarehouseAddress)
			select ZW.WarehouseCode, ZA.CityName, ZA.StateName, ZA.PostalCode, ISNULL(ZA.Address1,'')+' '+ISNULL(ZA.Address2,'') as WarehouseAddress 
			from ZnodeWarehouseAddress ZWA
			inner join ZnodeAddress ZA ON ZWA.AddressId = ZA.AddressId
			inner join ZnodeWarehouse ZW ON ZWA.WarehouseId = ZW.WarehouseId
			 

            INSERT INTO #Tlb_ProductData (PublishProductId,SKU)
            SELECT id,SKU FROM @ProductDetailsFromWebStore
			  		
			SELECT @SKU = Substring((SELECT ',' + SKU FROM @ProductDetailsFromWebStore FOR XML PAth('')),2,4000) 

			SELECT @PublishProductIds = Substring((SELECT ',' + CONVERT(NVARCHAR(100),id ) FROM @ProductDetailsFromWebStore FOR XML PAth('')),2,4000) 
			
			--INSERT INTO  #Tlb_PromotionProductData(PromotionId,PromotionType, ExpirationDate,  ActivationDate, PublishProductId,PromotionMessage)
			--Exec [Znode_GetPromotionByPublishProductId] @PublishProductIds = @PublishProductIds ,@UserId  = @UserId	,@PortalId  = @PortalId  
			 
			INSERT INTO #Tbl_PriceListWisePrice( SKU, RetailPrice,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix,CultureCode,ExternalId,WarehouseCityName,WarehouseStateName,WarehousePostalCode)
			EXEC Znode_GetPublishProductPricingBySku @SKU = @SKU ,@PortalId = @PortalId ,@currentUtcDate = @currentUtcDate,@UserId = @UserId 

			IF @IsAllLocation=1
			BEGIN 
				Insert into #Tbl_WarehouseWiseInventory(SKU,	Quantity,ReOrderLevel,PortalId,	WarehouseCode,	WarehouseName,	IsDefaultWarehouse)
				EXEC Znode_GetWarehouseInventoryBySkus  @SKUs=@SKU,@PortalId=@PortalId
			END


			Update PD SET 
			 PD.SKU             = PLWP.SKU            
			,PD.RetailPrice     = PLWP.RetailPrice     
			,PD.SalesPrice      = PLWP.SalesPrice      
			,PD.TierPrice       = PLWP.TierPrice       
			,PD.TierQuantity    = PLWP.TierQuantity    
			,PD.CurrencyCode    = PLWP.CurrencyCode    
			,PD.CurrencySuffix  = PLWP.CurrencySuffix  
			,PD.ExternalId 	    = PLWP.ExternalId
			,PD.WarehouseCityName			= PLWP.WarehouseCityName
			,PD.WarehouseStateName			= PLWP.WarehouseStateName
			,PD.WarehousePostalCode			= PLWP.WarehousePostalCode
			FROM #Tlb_ProductData PD Inner join #Tbl_PriceListWisePrice PLWP on 
			PD.SKU = PLWP.SKU

			Update PD SET 
			 PD.Quantity = TLI.Quantity,
			 PD.ReOrderLevel= TLI.ReOrderLevel
			 FROM #Tlb_ProductData PD Inner join #Tbl_Inventory TLI on 
			PD.SKU = TLI.SKU

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
             
             UPDATE PD SET PD.SEOTitle = ZCPS.ProductTitle,PD.SEODescription = ZCPS.ProductDescription,PD.SEOKeywords = ZCPS.ProductKeyword FROM #Tlb_ProductData PD
             INNER JOIN ZnodeCMSPortalSEOSetting ZCPS ON ZCPS.PortalId = @PortalId WHERE PD.SEOTitle IS NULL AND PD.SEODescription IS NULL AND PD.SEOKeywords IS NULL AND PD.SEOUrl IS NULL
			  --AND ZCSO.PortalId = @PortalId

			 
			   SELECT  a.PublishCatalogId ,Max(PublishCatalogLogId) PublishCatalogLogId
			   into #Cte_Catalogdaata
			   FROM ZnodePortalCatalog a 
			   INNER JOIN ZnodePublishCatalogLog b ON (b.PublishCatalogId = a.PublishCatalogId )
			   WHERE a.PortalId = @PortalId
			   GROUP BY a.PublishCatalogId 
	 

			 SELECT Row_Number()Over( PARTITION BY  BTY.SKU ORDER BY ZPAP.DisplayOrder, ZPAP.PublishAssociatedProductId) RowId ,
					BTY.SKU ParentSKU, BTY1.SKU --, UI.Quantity, UI.ReOrderLevel ,UI.WarehouseId
			 INTO #TempPublishData
			 FROM ZnodePublishProduct CTR 
			 INNER JOIN ZnodePublishProductDetail BTY ON (BTY.PublishProductId = CTR.PublishProductId)
			 INNER JOIN ZnodePublishAssociatedProduct ZPAP ON (ZPAP.ParentPimProductId = CTR.PimProductId)	
			 INNER JOIN ZnodePublishProduct CTR1 ON (ZPAP.PimProductId = CTR1.PimProductId)
			 INNER JOIN ZnodePublishProductDetail BTY1 ON (BTY1.PublishProductId = CTR1.PublishProductId)
			 WHERE ZPAP.IsConfigurable = 1  
			 AND EXISTS (SELECT TOP 1 1 FROM #Cte_Catalogdaata TY WHERE TY.PublishCatalogId = CTR.PublishCatalogId )-- AND TY.PublishCatalogLogId =ZPXML.PublishCatalogLogId)
			 AND EXISTS (SELECT TOP 1 1 FROM #Tlb_ProductData TU WHERE TU.SKU = BTY.SKU)
			 AND BTY.LocaleId = @Fn_GetDefaultLocaleId
			 AND BTY1.LocaleId = @Fn_GetDefaultLocaleId

			 alter table #TempPublishData add Quantity numeric(28,6), ReOrderLevel numeric(28,6), WarehouseId int

			 update TPD set Quantity = ISNULL(UI.Quantity,0) ,  ReOrderLevel = UI.ReOrderLevel , WarehouseId = UI.WarehouseId
			 from #TempPublishData TPD
			 inner join ZnodeInventory UI ON (UI.SKU = TPD.SKU)

			DELETE FROM #TempPublishData WHERE RowId <> 1
			IF @IsAllLocation=1
			BEGIN 
				SELECT A.PublishProductId,a.SKU,a.SEOTitle,a.SEODescription,a.SEOKeywords,a.SEOUrl,a.Rating,a.TotalReviews ,  
				a.RetailPrice,a.SalesPrice,a.TierPrice, a.TierQuantity,a.CurrencyCode,a.CurrencySuffix,a.ExternalId, 
				CASE WHEN TYI.ParentSKU IS NULL AND ZPCPA.PimProductId IS NULL THEN  ISNULL(b.Quantity,0) ELSE ISNULL(TYI.Quantity,0) END as Quantity , 
				CASE WHEN TYI.ParentSKU IS NULL THEN  b.ReOrderLevel ELSE TYI.ReOrderLevel END ReOrderLevel, a.CanonicalURL, a.RobotTag,
				INV.Quantity AllLocationQuantity,
				b.WarehouseCode,b.WarehouseName,ISNULL(b.IsDefaultWarehouse,0) AS IsDefaultWarehouse,
				WHA.CiyName as WarehouseCityName, WHA.StateName as WarehouseStateName, WHA.PostalCode as WarehousePostalCode, WHA.WarehouseAddress as WarehouseAddress
				FROM #Tlb_ProductData a
				LEFT JOIN #TempPublishData TYI ON (TYI.ParentSKU = a.SKU AND  TYI.WarehouseId  IN (SELECT  WarehouseId FROM ZnodePortalWarehouse WHERE PortalId = @PortalId))
				LEFT JOIN  #Tbl_WarehouseWiseInventory   b ON b.SKU  = a.SKU   
				LEFT JOIN ZnodePublishProduct ZPP on a.PublishProductId = ZPP.PublishProductId 
				LEFT join ZnodePimConfigureProductAttribute ZPCPA on ZPP.PimProductId = ZPCPA.PimProductId
				LEFT JOIN #Tbl_Inventory INV ON b.SKU = INV.SKU
				LEFT JOIN #WareHouseAddress WHA ON WHA.WarehouseCode = b.WarehouseCode 
			END
	
			
         END TRY
         BEGIN CATCH
		
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
			 @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
			  @ErrorLine VARCHAR(100)= ERROR_LINE(),
			   @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductWarehouseInfoForWebStore @PortalId='+CAST(@PortalId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetProductWarehouseInfoForWebStore',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;