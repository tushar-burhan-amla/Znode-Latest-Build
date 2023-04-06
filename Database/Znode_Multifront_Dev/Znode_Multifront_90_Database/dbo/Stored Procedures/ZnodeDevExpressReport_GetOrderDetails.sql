CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetOrderDetails]
(   
	@BeginDate      DATETIME         ,
    @EndDate        DATETIME         ,
    @StoreName      NVARCHAR(max) = '',
	@OrderStatus    NVARCHAR(MAX) = ''
	)
AS 
/*
     Summary:- this procedure is used to get the order details 
    
     EXEC ZnodeDevExpressReport_GetOrderDetails @BeginDate = '2019-07-01 00:00:00.513', @EndDate = '2019-07-30 00:00:00.513',@OrderStatus =  'submitted|shipped|in progress|in production|pending payment|CANCELLED|RETURNED|FAILED|Success'
*/
     BEGIN
	 BEGIN TRY
         SET NOCOUNT ON;

		IF OBJECT_ID('TEMPDB..#TBL_OMSOrder') IS NOT NULL
		DROP TABLE #TBL_OMSOrder

		IF OBJECT_ID('TEMPDB..#TlbGiftCardAmount') IS NOT NULL
		DROP TABLE #TlbGiftCardAmount


		DECLARE @SQL NVARCHAR(MAX)
        DECLARE @DateFormateId int 
	    SET @DateFormateId  = DBO.Fn_GetReportDateTimeFormat();
		DECLARE @RoundOffValue INT= dbo.Fn_GetDefaultValue('PriceRoundOff');
		
		DECLARE @TBL_PortalId TABLE (PortalId INT );
		INSERT INTO @TBL_PortalId
		SELECT PortalId 
		FROM ZnodePortal ZP 
		INNER JOIN dbo.split(@StoreName,'|') SP ON (SP.Item = ZP.StoreName)
		
		SELECT OmsOrderDetailsId, sum(DiscountAmount) DiscountAmount into #TlbGiftCardAmount FROM ZnodeOmsOrderDiscount where OmsDiscountTypeId in  (select OmsDiscountTypeId from ZnodeOmsDiscountType where NAME = 'GIFTCARD') 
		Group by OmsOrderDetailsId 
		
		SELECT DISTINCT
		ZOO.OrderNumber,ZOO.[OmsOrderID],ZOOD.[PortalId],ZOOD.[UserId],ZOOD.[OmsOrderStateId],ZOOD.[ShippingID],ZOOD.[PaymentTypeId],ZOOD.[BillingFirstName],ZOOD.[BillingLastName],
        ZOOD.[BillingCompanyName], ZOOD.[BillingStreet1],ZOOD.[BillingStreet2], ZOOD.[BillingCity],ZOOD.[BillingStateCode],ZOOD.[BillingPostalCode],ZOOD.[BillingCountry], ZOOD.[BillingPhoneNumber],ZOOD.[BillingEmailId],
		SUBSTRING(CAST(round(ZOOD.[TaxCost],@RoundOffValue) AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[TaxCost] AS VARCHAR(100)))+@RoundOffValue) as TaxCost,
		SUBSTRING(CAST(round(ZOOD.[ShippingCost],@RoundOffValue) AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[ShippingCost] AS VARCHAR(100)))+@RoundOffValue) as [ShippingCost],
		SUBSTRING(CAST(round(ZOOD.[SubTotal],@RoundOffValue) AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[SubTotal] AS VARCHAR(100)))+@RoundOffValue) as [SubTotal],
		SUBSTRING(CAST((Isnull(round(ZOOD.[DiscountAmount],@RoundOffValue),0) + Isnull(TGCA.DiscountAmount,0) )  AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[DiscountAmount] AS VARCHAR(100)))+@RoundOffValue)
		
		 as [DiscountAmount],
		SUBSTRING(CAST(round(ZOOD.[Total],@RoundOffValue) AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[Total] AS VARCHAR(100)))+@RoundOffValue) as [TotalAmount],
        CONVERT(varchar(100),ZOOD.[OrderDate],@DateFormateId) OrderDate,
        ZOOD.[ShippingNumber],
		ZOOD.[TrackingNumber],
		ZOOD.[CouponCode],
		ZOOD.[PromoDescription],
		ZOOD.[ReferralUserId],
		ZOOD.[PurchaseOrderNumber],
		ZOOD.[OmsPaymentStateId],
        ZOOD.[WebServiceDownloadDate],
		ZOOD.[PaymentSettingID],
		ZOOD.[ShipDate],
		ZOOD.[ReturnDate],
		ZOTOD.[SalesTax],ZOTOD.[VAT],ZOTOD.[GST],ZOTOD.[PST],ZOTOD.[HST],
        P.[StoreName] AS 'StoreName',
		OS.[Description] AS 'OrderStatus',
		ZOOD.PaymentDisplayName AS 'PaymentTypeName',
		ST.[Name] AS 'ShippingTypeName' ,
		CASE WHEN REPLACE(ISNULL(ZU.FirstName,'')+' '+ISNULL(ZU.LastName,''),' ','') = '' THEN APZ.UserName ELSE ISNULL(ZU.FirstName,'')+' '+ISNULL(ZU.LastName,'') END  CustomerName ,
		ISNULL(ZC.Symbol,[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol,
		OSP.ShipToCity AS 'ShippingCity',
		OSP.ShipToStateCode AS 'ShippingStateCode',
		OSP.ShipToCountry AS 'ShippingCountryCode'
		INTO #TBL_OMSOrder
		FROM ZNodeOmsOrder ZOO 
        INNER JOIN ZnodeOmsOrderDetails ZOOD ON(ZOOD.OmsOrderId = ZOO.OmsOrderId AND Zood.IsActive = 1) 
		INNER JOIN ZNodePortal P ON P.PortalID = ZOOD.PortalId
        LEFT JOIN ZNodeOmsOrderState OS ON OS.OmsOrderStateID = ZOOD.OmsOrderStateID
        LEFT JOIN ZNodeOmsOrderShipment OSP ON(EXISTS
                                            (SELECT TOP 1 1 FROM ZnodeOmsOrderLineItems ZOOLI WHERE ZOOLI.OmsOrderShipmentId = OSP.OmsOrderShipmentId AND ZOOLI.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId ))
        LEFT JOIN ZnodeOmsTaxOrderDetails ZOTOD ON(ZOTOD.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId)
        LEFT JOIN ZNodeShipping S ON S.ShippingID = OSP.ShippingID
        LEFT JOIN ZNodeShippingTypes ST ON ST.ShippingTypeID = S.ShippingTypeID
        LEFT JOIN ZNodePaymentType PT ON PT.PaymentTypeID = ZOOD.PaymentTypeId
		LEFT JOIN ZnodeCulture ZC ON (ZC.CultureCode  = ZOOD.CultureCode )
		LEFT JOIN ZnodeUser  ZU ON (ZU.UserId = ZOOD.UserId )
		LEFT JOIN AspNetUsers AP ON (AP.Id = ZU.AspNetUserId)
		LEFT JOIN AspNetZnodeUser APZ ON (APZ.AspNetZnodeUserId = AP.UserName)
		LEFT OUTER JOIN #TlbGiftCardAmount TGCA on ZOOD.OmsOrderDetailsId = TGCA.OmsOrderDetailsId 
        WHERE CAST(ZOOD.OrderDate AS DATETIME) BETWEEN @BeginDate AND @EndDate
		AND (EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId rt WHERE rt.PortalId = ZOOD.PortalId)
				OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId ))
		AND EXISTS (SELECT TOP 1 1 FROM dbo.split(@OrderStatus,'|') SP WHERE SP.Item = OS.OrderStateName)

	
		
		SELECT OrderNumber,OmsOrderID,PortalId ,UserId,OmsOrderStateId,ShippingID,PaymentTypeId,BillingFirstName,BillingLastName,
		BillingCompanyName,BillingStreet1,BillingStreet2, BillingCity,BillingStateCode,BillingPostalCode,BillingCountry,BillingPhoneNumber,BillingEmailId,
		CAST(tbl.TaxCost AS NUMERIC(28,6)) TaxCost,CAST(ShippingCost AS NUMERIC(28,6)) ShippingCost,
		CAST(SubTotal AS NUMERIC(28,6)) SubTotal,CAST(DiscountAmount AS NUMERIC(28,6)) DiscountAmount,CAST(TotalAmount AS NUMERIC(28,6)) TotalAmount,OrderDate,ShippingNumber,TrackingNumber,CouponCode,PromoDescription,ReferralUserId,PurchaseOrderNumber,OmsPaymentStateId,
        WebServiceDownloadDate,PaymentSettingID,ShipDate,ReturnDate, SalesTax,VAT,GST,PST,HST,StoreName,OrderStatus,PaymentTypeName,ShippingTypeName,CustomerName,Symbol 
		,ShippingCity,ShippingCountryCode,ShippingStateCode
		FROM #TBL_OMSOrder tbl
		Order by CAST(OrderDate  AS DATETIME)
			
		IF OBJECT_ID('TEMPDB..#TBL_OMSOrder') IS NOT NULL
		DROP TABLE #TBL_OMSOrder

		END TRY
		BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetOrderDetails @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetOrderDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH
     END;