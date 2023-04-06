CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetCouponFiltered]  
(   
 @BeginDate  DATE        ,  
 @EndDate  DATE         ,  
 @StoreName  VARCHAR(MAX)  = '',  
 @DiscountType   NVARCHAR(MAX) 
)  
AS   
/*  
     Summary:- this procedure is used to get coupon filtered ( include promotion details )  
      
     EXEC ZnodeDevExpressReport_GetCouponFiltered_bak @PromotionCode = 'sale10' ,@DiscountType = 'pro', @BeginDate = '2019-05-07 10:46:57.190', @EndDate = '2019-05-07 10:53:17.617'  
  
   EXEC ZnodeDevExpressReport_GetCouponFiltered  @BeginDate = '2019-05-08 11:56:45.570', @EndDate = '2019-05-15 12:49:34.120'  , @DiscountType ='PROMOCODE|COUPONCODE'
*/  
     BEGIN  
  BEGIN TRY  
         SET NOCOUNT ON;   
  
  
      DECLARE @RoundOffValue INT= dbo.Fn_GetDefaultValue('PriceRoundOff'); 
	  DECLARE @DefaultCurrencySymbol varchar(100) = dbo.Fn_GetDefaultCurrencySymbol() 
  
   CREATE TABLE #TBL_PortalId (PortalId INT );  
   INSERT INTO #TBL_PortalId  
   SELECT PortalId   
   FROM ZnodePortal ZP   
   INNER JOIN dbo.split(@StoreName,'|') SP ON (SP.Item = ZP.StoreName)  
  
   CREATE TABLE #ZnodeOmsOrderDiscount ( OmsOrderDetailsId  INT,OmsOrderLineItemId INT,OmsDiscountTypeId  INT,DiscountCode VARCHAR(300),DiscountAmount NUMERIC(28, 6),  
   Description NVARCHAR(MAX),CreatedBy INT,CreatedDate DATETIME,ModifiedBy INT,ModifiedDate DATETIME,PortalId INT,Total NUMERIC(28, 6), Symbol NVARCHAR(100), OmsOrderDate Datetime,
   OmsOrderDiscountId INT);  
  

   INSERT INTO #ZnodeOmsOrderDiscount  
   (OmsOrderDetailsId,OmsDiscountTypeId,DiscountCode,DiscountAmount,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalId,Total,Symbol, OmsOrderDate,OmsOrderDiscountId)  
   SELECT Distinct ISNULL(zood.OmsOrderDetailsId, zooli.OmsOrderDetailsId) OmsOrderDetailsId,zood.OmsDiscountTypeId,zood.DiscountCode,zood.DiscountAmount,zood.Description,  
   zood.CreatedBy,zood.CreatedDate, zood.ModifiedBy,zood.ModifiedDate,zoods.PortalId, zoods.Total, ISNULL(ZC.Symbol,@DefaultCurrencySymbol) Symbol,zoods.OrderDate
   ,zood.OmsOrderDiscountId  
   FROM ZnodeOmsOrderDiscount AS zood  
   INNER JOIN ZnodeOmsDiscountType ODT ON (ODT.OmsDiscountTypeId = zood.OmsDiscountTypeId)  
   LEFT  JOIN ZnodeOmsOrderLineItems AS zooli ON zood.OmsOrderLineItemId = zooli.OmsOrderLineItemsId  
   LEFT  JOIN ZnodeOmsOrderDetails AS zoods ON ISNULL(zood.OmsOrderDetailsId, zooli.OmsOrderDetailsId) = zoods.OmsOrderDetailsId  
   INNER JOIN ZnodeOmsOrderState ZOOS ON (ZOODS.OmsOrderStateId = ZOOS.OmsOrderStateId)   
   LEFT JOIN ZnodeCulture ZC ON (ZC.CultureCode = ZOODS.CurrencyCode)  
   WHERE (EXISTS (SELECT TOP 1 1 FROM #TBL_PortalId rt WHERE rt.PortalId = zoods.PortalId)  
   OR NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PortalId ))  
   AND ZOODS.IsActive = 1  
   AND ZOOS.OrderStateName NOT IN ('CANCELED','RETURNED','REJECTED','FAILED')   
   AND ODT.Name NOT IN ('PARTIALREFUND','GIFTCARD')
        
 	 
   --;WITH CTE_GetCouponFiltered AS
   --(    
   SELECT zood.DiscountCode CouponCode,zdt.[Name] AS DiscountType,zp.[StoreName] AS StoreName,COUNT(zood.OmsOrderDiscountId) AS TotalCount,  
  [dbo].[Fn_GetDefaultPriceRoundOff](zood.Total) AS Total 
   ,[dbo].[Fn_GetDefaultPriceRoundOff](SUM(zood.DiscountAmount)) AS DiscountAmount,  
   ISNULL(zpr.[description], znpc.[description]) AS Description,  
   ISNULL(zpr.StartDate, znpc.StartDate) AS BeginDate,ISNULL(zpr.EndDate, znpc.EndDate) AS EndDate,Symbol, zpr.PromoCode AS PromotionCode , zpr.Name AS PromotionName  
   ,OmsOrderDetailsId
   INTO #CTE_GetCouponFiltered
   FROM #ZnodeOmsOrderDiscount AS zood   
   INNER JOIN ZnodePortal AS zp ON zood.PortalId = zp.PortalId  
   LEFT JOIN ZNodePromotion AS zpr ON zood.DiscountCode = zpr.PromoCode  
   LEFT JOIN ZnodePromotionCoupon AS zprc ON zood.DiscountCode = zprc.Code  
   LEFT JOIN ZNodePromotion AS znpc ON zprc.PromotionId = znpc.PromotionId  
   LEFT JOIN ZnodeOmsDiscountType AS zdt ON zood.OmsDiscountTypeId = zdt.OmsDiscountTypeId  
    WHERE CAST(ZOOD.OmsOrderDate AS DATETIME) BETWEEN @BeginDate AND @EndDate       
    AND zdt.[Name] IN (select Item from dbo.split(@DiscountType,'|'))    
    GROUP BY zp.StoreName,zood.DiscountCode
	,ISNULL(zpr.[description], znpc.[description]),ISNULL(zpr.StartDate, znpc.StartDate),ISNULL(zpr.EndDate, znpc.EndDate),  
    zdt.Name,zood.PortalId,Symbol,zpr.PromoCode,zpr.Name, zood.OmsOrderDetailsId,zood.Total
	--)  
    
   

   SELECT CouponCode, --CASE WHEN DiscountType='promocode' THEN '' ELSE CouponCode END 
   DiscountType,StoreName,SUM(TotalCount) AS NumberOfUses
   ,SUM(convert(numeric(28,6),Total)) SalesTotal
   ,SUM(convert(numeric(28,6),DiscountAmount)) DiscountTotal,
   Description,BeginDate AS ActivationDate,EndDate AS ExpirationDate,
   CASE WHEN DiscountType='couponcode' THEN '' ELSE ISNULL(PromotionCode,CouponCode) END 
   PromotionCode,
   CASE WHEN DiscountType='couponcode' THEN '' ELSE PromotionName END 
   PromotionName 
   FROM #CTE_GetCouponFiltered 
   GROUP BY CouponCode ,DiscountType,StoreName,Description,BeginDate,EndDate,CASE WHEN DiscountType='couponcode' THEN '' ELSE ISNULL(PromotionCode,CouponCode) END 
   ,PromotionName --CASE WHEN DiscountType='promocode' THEN '' ELSE CouponCode END 

   
       
  END TRY  
  BEGIN CATCH  
  DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetCouponFiltered @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_GetCouponFiltered',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
  END CATCH  
     END;