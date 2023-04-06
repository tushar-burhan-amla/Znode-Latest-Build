Create PROCEDURE [dbo].[Znode_GetOrderPricList]
(  
@OmsOrderId        INT,
@SKU              VARCHAR(MAX),
@PublishProductId TransferId READONLY,
@IsDebug          BIT          = 0
)
AS
   /*
    --Summary: Retrive Price of product from Order tables
    --Input Parameters:
    --orderId
    --Conditions :
    --1. If OrderId is greater then return PriceList of associated active order.
       --Unit Testing  
    --Exec Znode_GetOrderPricing  @OrderId =1
*/
   
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
    -- DECLARE #TLB_SKUPRICELIST TABLE
          CREATE TABLE #TLB_SKUPRICELIST
    (SKU          VARCHAR(100),
              RetailPrice  NUMERIC(28, 6),
              SalesPrice   NUMERIC(28, 6),
              PriceListId  INT,
              TierPrice    NUMERIC(28, 6),
              TierQuantity NUMERIC(28, 6),
 ExternalId NVARCHAR(2000),
 Custom1 NVARCHAR(MAX),
 Custom2 NVARCHAR(MAX),
 Custom3 NVARCHAR(MAX)
             );
             DECLARE @PriceRoundOff INT;
             SELECT @PriceRoundOff = CONVERT( INT, FeatureValues)
             FROM ZnodeGlobalSetting
             WHERE FeatureName = 'PriceRoundOff';

DECLARE @Tlb_SKU TABLE
             (
SKU        VARCHAR(100),
SequenceNo INT IDENTITY
             );
DECLARE @DefaultLocaleId INT = dbo.FN_GETDEFAULTLocaleId()

IF @SKU = ''
BEGIN
INSERT INTO @Tlb_SKU(SKU)
SELECT SKU
FROM ZnodePublishProductDetail a
INNER JOIN @PublishProductId b ON (b.Id = a.PublishProductId )
WHERE LocaleId = @DefaultLocaleId

END
ELSE
BEGIN
INSERT INTO @Tlb_SKU(SKU)
SELECT Item FROM Dbo.split(@SKU, ',');
END
     
            IF (@OmsOrderId > 0)

    BEGIN
INSERT INTO #TLB_SKUPRICELIST( PriceListId, SKU,  SalesPrice,RetailPrice, TierPrice, TierQuantity, ExternalId)
SELECT distinct ZP.PriceListId, ZP.SKU, Ol.Price SalesPrice, Ol.Price RetailPrice, NULL Price,NULL Quantity, ZP.ExternalId
FROM [ZnodePrice] AS ZP
   INNER JOIN ZnodeOmsOrderLineItems ol on Zp.SKU=Ol.Sku
   INNER JOIN ZnodeOmsOrderDetails od on ol.OmsOrderDetailsId=od.OmsOrderDetailsId and ol.RmaReasonForReturnId is null
   where od.OmsOrderId=OmsOrderId and od.IsActive=1
   and od.OmsOrderId = @OmsOrderId and isnull(Ol.Price,0) <> 0
and exists(select * from @Tlb_SKU s where s.SKU = ZP.SKU)
                 END;                  

             SELECT Distinct SKU,
                    ROUND(RetailPrice, @PriceRoundOff) AS RetailPrice,
                    ROUND(SalesPrice, @PriceRoundOff) AS SalesPrice,
                    ROUND(TierPrice, @PriceRoundOff) AS TierPrice,
                    ROUND(TierQuantity, @PriceRoundOff) AS TierQuantity,
ZCC.CurrencyCode  AS CurrencyCode,    
                    ZC.Symbol AS CurrencySuffix,  ZC.CultureCode,
TSPL.ExternalId,
Custom1,Custom2,Custom3
             FROM #TLB_SKUPRICELIST AS TSPL
                  INNER JOIN ZnodePriceList AS ZPL ON TSPL.PriceListId = ZPL.PriceListId
                  INNER JOIN ZnodeCulture AS ZC ON ZPL.CultureId = ZC.CultureId    
 LEFT JOIN ZnodeCurrency AS ZCC ON ZC.CurrencyId = ZCC.CurrencyId  
 ORDER BY SKU ASC;

 
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
SET @Status = 0;
DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOrderPricList @OrderId = '+ CAST(@OmsOrderId AS VARCHAR(10));
             
SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
 
EXEC Znode_InsertProcedureErrorLog
@ProcedureName = 'Znode_GetOrderPricList',
@ErrorInProcedure = @Error_procedure,
@ErrorMessage = @ErrorMessage,
@ErrorLine = @ErrorLine,
@ErrorCall = @ErrorCall;
         END CATCH;
     END;