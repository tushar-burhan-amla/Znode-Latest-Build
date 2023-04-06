
CREATE PROCEDURE [dbo].[ZnodeReport_GetCouponFiltered]  
(   
	@BeginDate    DATE         = NULL,  
	@EndDate      DATE         = NULL,  
	@PortalId     VARCHAR(MAX) = '',  
	@DiscountCode NVARCHAR(300) = '',  
	@DiscountType VARCHAR(600) = ''
 )  
AS   
/*  
     Summary:- this procedure is used to get coupon filtered ( include promotion details )  
      
     EXEC ZnodeReport_GetCouponFiltered @DiscountCode = 'o' ,@PortalId = '2,4,5' ,@DiscountType = 'pro'  
*/  
  BEGIN  
  BEGIN TRY  
         SET NOCOUNT ON;
		 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
		 DECLARE @DefSymbol NVARCHAR(5) =  dbo.Fn_GetDefaultCurrencySymbol()
		 DECLARE @Portal TABLE (PortalId INT)
		 INSERT INTO @portal 
		 SELECT  SP.Item
		 FROM dbo.split(@PortalId, ',') SP
		 
		 IF OBJECT_ID('Tempdb..[#ZnodeOmsOrderDiscount]') IS NOT NULL 
		 DROP TABLE Tempdb..[#ZnodeOmsOrderDiscount]
		    
  
         CREATE TABLE #ZnodeOmsOrderDiscount   
         (OmsOrderDiscountId INT,  
          OmsOrderDetailsId  INT,  
          OmsOrderLineItemId INT,  
          OmsDiscountTypeId  INT,  
          DiscountCode       VARCHAR(300),  
          DiscountAmount     NUMERIC(28, 6),  
          Description        NVARCHAR(MAX),  
          CreatedBy          INT,  
          CreatedDate        DATETIME,  
          ModifiedBy         INT,  
          ModifiedDate       DATETIME,  
          PortalId           INT,  
          Total              NUMERIC(28, 6),  
          Symbol    NVARCHAR(100),  
          OmsOrderDate       Datetime,  
		  CultureCode VARCHAR(100)
         );  
         INSERT INTO #ZnodeOmsOrderDiscount  
         (OmsOrderDiscountId,  
          OmsOrderDetailsId,  
            
          OmsDiscountTypeId,  
          DiscountCode,  
          DiscountAmount,  
          Description,  
          CreatedBy,  
          CreatedDate,  
          ModifiedBy,  
          ModifiedDate,  
          PortalId,  
          Total,
		  --Symbol,  
		  OmsOrderDate ,
		  CultureCode 
         )  
                SELECT DISTINCT zood.OmsOrderDiscountId,  
                       ISNULL(zood.OmsOrderDetailsId, zooli.OmsOrderDetailsId),  
                       zood.OmsDiscountTypeId,  
                       zood.DiscountCode,  
                       zood.DiscountAmount,  
                       zood.Description,  
                       zood.CreatedBy,  
                       zood.CreatedDate,  
                       zood.ModifiedBy,  
                       zood.ModifiedDate,  
                       zoods.PortalId,  
                       zoods.Total, 
					   --ISNULL(ZC.Symbol,@DefSymbol) Symbol,
					   zoods.OrderDate,
					   ZOODS.CultureCode
                FROM ZnodeOmsOrderDiscount AS zood  
                     LEFT OUTER JOIN ZnodeOmsOrderLineItems AS zooli ON zood.OmsOrderLineItemId = zooli.OmsOrderLineItemsId  
                     LEFT OUTER JOIN ZnodeOmsOrderDetails AS zoods ON (ISNULL(zood.OmsOrderDetailsId, zooli.OmsOrderDetailsId) = zoods.OmsOrderDetailsId and zoods.IsActive=1) 
					 --LEFT JOIN ZnodeCulture ZC ON (ZC.CultureCode = ZOODS.CurrencyCode);
		     
		 UPDATE ZOODS SET ZOODS.Symbol = ISNULL(ZC.Symbol,@DefSymbol)  
		 FROM #ZnodeOmsOrderDiscount ZOODS
		 INNER JOIN ZnodeCulture ZC ON (ZC.CultureCode = ZOODS.CultureCode)

         SELECT zood.DiscountCode AS CouponCode,  
                zdt.[Name] AS DiscountType,  
                zp.[StoreName] AS StoreName,  
                COUNT(zood.DiscountCode) AS TotalCount,  
                [dbo].[Fn_GetDefaultPriceRoundOff](SUM(zood.Total)) AS Total,  
                [dbo].[Fn_GetDefaultPriceRoundOff](SUM(zood.DiscountAmount)) AS DiscountAmount,  
                ISNULL(zpr.[description], znpc.[description]) AS Description,  
                ISNULL(zpr.StartDate, znpc.StartDate) AS StartDate,  
                ISNULL(zpr.EndDate, znpc.EndDate) AS EndDate,Symbol  
         FROM #ZnodeOmsOrderDiscount AS zood  
              INNER JOIN ZnodePortal AS zp ON zood.PortalId = zp.PortalId  
              LEFT OUTER JOIN ZNodePromotion AS zpr ON zood.DiscountCode = zpr.PromoCode  
              LEFT OUTER JOIN ZnodePromotionCoupon AS zprc ON zood.DiscountCode = zprc.Code  
              LEFT OUTER JOIN ZNodePromotion AS znpc ON zprc.PromotionId = znpc.PromotionId  
              LEFT OUTER JOIN ZnodeOmsDiscountType AS zdt ON zood.OmsDiscountTypeId = zdt.OmsDiscountTypeId  
         WHERE((EXISTS  
               (  
                   SELECT TOP 1 1 
					FROM @portal TBL
					WHERE ZP.PortalId = TBL.PortalId  OR @PortalId = ''   
               )))  
              AND (ISNULL(zood.DiscountCode, '') LIKE '%'+@DiscountCode+'%'  
                   OR @DiscountCode = '')  
              AND (ISNULL(zdt.[Name], '') LIKE '%'+@DiscountType+'%'  
                   OR @DiscountType = '')  
			AND (CAST(zood.OmsOrderDate AS DATE) BETWEEN CASE  
			WHEN @BeginDate IS NULL  
			THEN CAST(zood.OmsOrderDate AS DATE)  
			ELSE @BeginDate  
			END AND CASE  
			WHEN @EndDate IS NULL  
			THEN CAST(zood.OmsOrderDate AS DATE)  
			ELSE @EndDate  
			END
   )  
         GROUP BY zp.StoreName,  
                  zood.DiscountCode,  
                  ISNULL(zpr.[description], znpc.[description]),  
                  ISNULL(zpr.StartDate, znpc.StartDate),  
                  ISNULL(zpr.EndDate, znpc.EndDate),  
                  zdt.Name,  
                  zood.PortalId,Symbol;  
		IF OBJECT_ID('Tempdb..[#ZnodeOmsOrderDiscount]') IS NOT NULL 
		DROP TABLE Tempdb..[#ZnodeOmsOrderDiscount]

  END TRY  
  BEGIN CATCH  
  DECLARE @Status BIT ;  
	SET @Status = 0;  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetCouponFiltered @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@DiscountCode='+@DiscountCode+',@DiscountType='+@DiscountType+',@Status	='+CAST(@Status AS VARCHAR(10));  
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
	EXEC Znode_InsertProcedureErrorLog  
	@ProcedureName = 'ZnodeReport_GetCouponFiltered',  
	@ErrorInProcedure = @Error_procedure,  
	@ErrorMessage = @ErrorMessage,  
	@ErrorLine = @ErrorLine,  
	@ErrorCall = @ErrorCall;  
  END CATCH  
END;