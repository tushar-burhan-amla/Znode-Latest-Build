
Create PROCEDURE [dbo].[Znode_GetInventoryPromotionPricingBySkuWrapper]  
(     
	 @PortalId         INT,  
	 @LocaleId         INT,  
	 @UserId     INT = 2,  
	 @ProductDetailsFromWebStore   DBO.ProductDetailsFromWebStore READONLY,  
	 @currentUtcDate    VARCHAR(200) = '',  
	 @NavigationProperties varchar(500)='Default',
	 @OmsOrderId         INT=0   ,
	 @IsFetchPriceFromOrder Bit = 0
 )  
AS   
     BEGIN  
       
         BEGIN TRY  
             SET NOCOUNT ON;  
      
    DECLARE @PublishProductIds  NVARCHAR(max) ,@SKU NVARCHAR(max)   
    DECLARE @Tlb_CustomerAverageRatings TABLE  
             (PublishProductId INT,  
              Rating           NUMERIC(28,6),  
              TotalReviews     INT  
             );   
    
   Declare  @Tlb_PromotionProductData TABLE  
             (  
      PromotionId      INT,  
      PromotionType    nvarchar(500),   
      ExpirationDate   Datetime,   
      ActivationDate   Datetime,  
      PublishProductId INT,  
      PromotionMessage Nvarchar(max)   
             );  
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
      PromotionId      INT,  
      PromotionType   nvarchar(500),  
      ExpirationDate   DATETIME,  
      ActivationDate   DATETIME,  
      PromotionMessage NVARCHAR(MAX)   
      );  
  
  
    Create TABLE #Tbl_Inventory  
    (  
		SKU            VARCHAR(300),  
		Quantity    NUMERIC(28, 6),  
		ReOrderLevel     NUMERIC(28, 6),  
		PortalId      int , 
        WarehouseName	varchar(100),
		WarehouseCode	varchar(100),
		DefaultInventoryCount varchar(1000),
    );  
     
            INSERT INTO #Tlb_ProductData (PublishProductId,SKU)  
            SELECT id,SKU FROM @ProductDetailsFromWebStore  
   
   IF ISDATE(@currentUtcDate)=0
	BEGIN
	SET  @currentUtcDate=convert(datetime,@currentUtcDate,103)
	END
         
   SELECT @SKU = Substring((SELECT ',' + SKU FROM @ProductDetailsFromWebStore FOR XML PAth('')),2,4000)   
     
     
   SELECT @PublishProductIds = Substring((SELECT ',' + CONVERT(NVARCHAR(100),id ) FROM @ProductDetailsFromWebStore FOR XML PAth('')),2,4000)   
    
   IF Exists (SELECT TOP 1 1 FROM dbo.Split(@NavigationProperties,',') WHERE Item IN ('promotions','Default'))  
   BEGIN   
     INSERT INTO  @Tlb_PromotionProductData(PromotionId,PromotionType, ExpirationDate,  ActivationDate, PublishProductId,PromotionMessage)  
    Exec [Znode_GetPromotionByPublishProductId] @PublishProductIds = @PublishProductIds ,@UserId  = @UserId ,@PortalId  = @PortalId    
      
   END  
     
   IF Exists (SELECT TOP 1 1 FROM dbo.Split(@NavigationProperties,',') WHERE Item IN ('Pricing','Default'))  
   BEGIN   
  
     INSERT INTO #Tbl_PriceListWisePrice( SKU, RetailPrice,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix,CultureCode,ExternalId,Custom1,Custom2,Custom3)  
	   EXEC Znode_GetPublishProductPricingBySku @SKU = @SKU ,@PortalId = @PortalId ,@currentUtcDate = @currentUtcDate,@UserId = @UserId,@OmsOrderId=@OmsOrderId,@IsFetchPriceFromOrder=@IsFetchPriceFromOrder
	   
	   	   
   END  
       
   IF Exists (SELECT TOP 1 1 FROM dbo.Split(@NavigationProperties,',') WHERE Item IN ('Inventory','Default'))  
   BEGIN  
   INSERT INTO #Tbl_Inventory (SKU, Quantity, ReOrderLevel, PortalId, WarehouseName, WarehouseCode, DefaultInventoryCount)  
   EXEC Znode_GetInventoryBySkus @SKUs=@SKU,@PortalId=@PortalId  
   END  
     
   
    SELECT * FROM @Tlb_PromotionProductData  
    SELECT * FROM #Tbl_PriceListWisePrice  
    SELECT SKU, Quantity, ReOrderLevel, PortalId FROM #Tbl_Inventory  
    
	END TRY  
	BEGIN CATCH  
	DECLARE @Status BIT ;  
	SET @Status = 0;  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),   
	@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),  
	@ErrorLine VARCHAR(100)= ERROR_LINE(),  
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetInventoryPromotionPricingBySkuWrapper @PortalId='+CAST(@PortalId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@currentUtcDate='+CAST(@currentUtcDate 
	AS VARCHAR(50))+',@NavigationProperties='+CAST(@NavigationProperties AS VARCHAR(50))  
                    
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
	EXEC Znode_InsertProcedureErrorLog  
	@ProcedureName = 'Znode_GetInventoryPromotionPricingBySkuWrapper',  
	@ErrorInProcedure = @Error_procedure,  
	@ErrorMessage = @ErrorMessage,  
	@ErrorLine = @ErrorLine,  
	@ErrorCall = @ErrorCall;  
	END CATCH;  
	END;