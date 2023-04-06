  
CREATE PROCEDURE [dbo].[ZnodeReport_GetRecurringOrder](  
 @BeginDate date= NULL,  
 @EndDate date= NULL,  
 @PortalId varchar(max)= '',  
 @OrderStateId varchar(max)= '',  
 @PayemntTypeId varchar(max)= '',  
 @ShippingTypeId varchar(max)= '',  
 @CustomerName nvarchar(400)= '',  
 @EmailId nvarchar(100)= '')  
AS   
/*  
  Summary:- this procedure is used to get the order details   
  Unit Testing:  
  EXEC ZnodeReport_GetRecurringOrder  
*/  
 BEGIN  
  BEGIN TRY  
  SET NOCOUNT ON;  
   SELECT DISTINCT   
       ZOO.[OmsOrderID],ZOO.OrderNumber, ZOOD.[PortalId], ZOOD.[UserId], ZOOD.[OmsOrderStateId], ZOOD.[ShippingID], ZOOD.[PaymentTypeId], OSP.[ShipToFirstName] AS 'ShipFirstName', OSP.[ShipToLastName] AS 'ShipLastName', OSP.[ShipToCompanyName] AS 'ShipCompanyName', OSP.[ShipToStreet1] AS 'ShipStreet', OSP.[ShipToStreet2] AS 'ShipStreet1', OSP.[ShipToCity] AS 'ShipCity', OSP.[ShipToStateCode] AS 'ShipStateCode', OSP.[ShipToPostalCode] AS 'ShipPostalCode', OSP.[ShipToCountry] AS 'ShipCountry', OSP.[ShipToPhoneNumber] AS 'ShipPhoneNumber', OSP.[ShipToEmailID] AS 'ShipEmailID', ZOOD.[BillingFirstName], ZOOD.[BillingLastName], ZOOD.[BillingCompanyName], ZOOD.[BillingStreet1], ZOOD.[BillingStreet2], ZOOD.[BillingCity], ZOOD.[BillingStateCode], ZOOD.[BillingPostalCode], ZOOD.[BillingCountry], ZOOD.[BillingPhoneNumber], ZOOD.[BillingEmailId], [dbo].[Fn_GetDefaultPriceRoundOff]( ZOOD.[TaxCost] ) AS TaxCost, [dbo].[Fn_GetDefaultPriceRoundOff]( ZOOD.[ShippingCost] ) AS [ShippingCost], [dbo].[Fn_GetDefaultPriceRoundO
ff]( ZOOD.[SubTotal] ) AS [SubTotal], [dbo].[Fn_GetDefaultPriceRoundOff]( ZOOD.[DiscountAmount] ) AS [DiscountAmount], [dbo].[Fn_GetDefaultPriceRoundOff]( ZOOD.[Total] ) AS [Total], CAST(ZOOD.[OrderDate] AS date) AS [OrderDate], ZOOD.[ShippingNumber], ZOOD.[TrackingNumber], ZOOD.[CouponCode], ZOOD.[PromoDescription], ZOOD.[ReferralUserId], ZOOD.[PurchaseOrderNumber], ZOOD.[OmsPaymentStateId], ZOOD.[WebServiceDownloadDate], ZOOD.[PaymentSettingID], ZOOD.[ShipDate], ZOOD.[ReturnDate], ZOTOD.[SalesTax], ZOTOD.[VAT], ZOTOD.[GST], ZOTOD.[PST], ZOTOD.[HST], P.[StoreName] AS 'StoreName', OS.[OrderStateName] AS 'OrderStatus', PT.[Name] AS 'PaymentTypeName', ST.[Name] AS 'ShippingTypeName', ISNULL(ZC.Symbol, dbo.Fn_GetDefaultCurrencySymbol()) AS Symbol  
   FROM ZNodeOmsOrder AS ZOO  
     INNER JOIN  
     ZnodeOmsOrderDetails AS ZOOD  
     ON( ZOOD.OmsOrderId = ZOO.OmsOrderId AND   
      IsActive = 1  
       )  
     INNER JOIN  
     ZNodePortal AS P  
     ON P.PortalID = ZOOD.PortalId  
     INNER JOIN  
     ZNodeOmsOrderState AS OS  
     ON OS.OmsOrderStateID = ZOOD.OmsOrderStateID  
     INNER JOIN  
     ZnodeOmsOrderLineItems AS ZOOLI  
     ON(ZOOD.OmsOrderDetailsId = ZOOLI.OmsOrderDetailsId)  
     INNER JOIN  
     ZNodeOmsOrderShipment AS OSP  
     ON(ZOOLI.OmsOrderShipmentId = OSP.OmsOrderShipmentId)  
     LEFT JOIN  
     ZnodeOmsTaxOrderDetails AS ZOTOD  
     ON(ZOTOD.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId)  
     LEFT JOIN  
     ZNodeShipping AS S  
     ON S.ShippingID = OSP.ShippingID  
     LEFT JOIN  
     ZNodeShippingTypes AS ST  
     ON ST.ShippingTypeID = S.ShippingTypeID  
     LEFT JOIN  
     ZNodePaymentType AS PT  
     ON PT.PaymentTypeID = ZOOD.PaymentTypeId  
     LEFT JOIN  
     --ZnodeCurrency AS ZC  
     --ON(ZC.CurrencyCode = ZOOD.CurrencyCode)  
	    ZnodeCulture AS ZC  
     ON(ZC.CultureCode = ZOOD.CurrencyCode)  
        WHERE( ( EXISTS  
       (  
        SELECT TOP 1 1  
        FROM dbo.split( @PortalId, ',' ) AS SP  
        WHERE CAST(P.PortalId AS varchar(100)) = SP.Item OR   
        @PortalId = ''  
       )  
       )  
     ) AND   
     ( ( EXISTS  
       (  
        SELECT TOP 1 1  
        FROM dbo.split( @OrderStateId, ',' ) AS SP  
        WHERE CAST(ZOOD.[OmsOrderStateId] AS varchar(100)) = SP.Item OR   
        @OrderStateId = ''  
       )  
       )  
     ) AND   
     ( ( EXISTS  
       (  
        SELECT TOP 1 1  
        FROM dbo.split( @PayemntTypeId, ',' ) AS SP  
        WHERE CAST(ZOOD.[PaymentTypeId] AS varchar(100)) = SP.Item OR   
        @PayemntTypeId = ''  
       )  
       ) 
     ) AND   
     ( ( EXISTS  
       (  
        SELECT TOP 1 1  
        FROM dbo.split( @ShippingTypeId, ',' ) AS SP  
        WHERE CAST(ST.ShippingTypeID AS varchar(100)) = SP.Item OR   
        @ShippingTypeId = ''   )  )   ) AND   
     ( ZOOD.[BillingFirstName]+' '+ISNULL(ZOOD.[BillingLastName], '') LIKE '%'+@CustomerName+'%' OR   
       @CustomerName = ''  
     ) AND   
     ( ZOOD.[BillingEmailId] LIKE '%'+@EmailId+'%' OR   
       @EmailId = ''  
     ) AND   
     ( CAST(ZOOD.OrderDate AS date) BETWEEN CASE  
              WHEN @BeginDate IS NULL THEN CAST(ZOOD.OrderDate AS date)  
              ELSE @BeginDate  
              END AND CASE  
                WHEN @EndDate IS NULL THEN CAST(ZOOD.OrderDate AS date)  
                ELSE @EndDate  
                END  
     ) AND   
     ZOOLI.IsRecurringBilling = 1;  
  
  END TRY  
  BEGIN CATCH  
   DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetRecurringOrder @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@OrderStateId='+@OrderStateId+',@PayemntTypeId='+@PayemntTypeId+',@Ship
pingTypeId='+@ShippingTypeId+',@CustomerName='+@CustomerName+',@EmailId='+@EmailId+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_GetRecurringOrder',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
  END CATCH  
END;