CREATE PROCEDURE [dbo].[ZnodeReport_GetOrderDetails]
(   @BeginDate      DATE          = NULL,
    @EndDate        DATE          = NULL,
    @PortalId       VARCHAR(MAX)  = '',
    @OrderStateId   VARCHAR(MAX)  = '',
    @PayemntTypeId  VARCHAR(MAX)  = '',
    @ShippingTypeId VARCHAR(MAX)  = '',
    @CustomerName   NVARCHAR(400) = '',
    @EmailId        NVARCHAR(100) = '')
AS 
/*
     Summary:- this procedure is used to get the order details 
    
     EXEC ZnodeReport_GetOrderDetails 
*/
     BEGIN
	 BEGIN TRY
         SET NOCOUNT ON;
         Declare @DateFormateId int 
		 DECLARE @RoundOffValue INT= dbo.Fn_GetDefaultValue('PriceRoundOff');
		 DECLARE @DefaultCurrencySymbol VARCHAR(100) = [dbo].[Fn_GetDefaultCurrencySymbol]()

	    Select @DateFormateId  = DBO.Fn_GetDateTimeFormat();
         SELECT DISTINCT
			 ZOO.OrderNumber,
                ZOO.[OmsOrderID],
                ZOOD.[PortalId],
                ZOOD.[UserId],
                ZOOD.[OmsOrderStateId],
                ZOOD.[ShippingID],
                ZOOD.[PaymentTypeId],
                ZOOD.[BillingFirstName],
                ZOOD.[BillingLastName],
                ZOOD.[BillingCompanyName],
                ZOOD.[BillingStreet1],
                ZOOD.[BillingStreet2],
                ZOOD.[BillingCity],
                ZOOD.[BillingStateCode],
                ZOOD.[BillingPostalCode],
                ZOOD.[BillingCountry],
                ZOOD.[BillingPhoneNumber],
                ZOOD.[BillingEmailId],
				SUBSTRING(CAST(ZOOD.[TaxCost] AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[TaxCost] AS VARCHAR(100)))+@RoundOffValue) as TaxCost,
				SUBSTRING(CAST(ZOOD.[ShippingCost] AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[ShippingCost] AS VARCHAR(100)))+@RoundOffValue) as [ShippingCost],
				SUBSTRING(CAST(ZOOD.[SubTotal] AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[SubTotal] AS VARCHAR(100)))+@RoundOffValue) as [SubTotal],
				SUBSTRING(CAST(ZOOD.[DiscountAmount] AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[DiscountAmount] AS VARCHAR(100)))+@RoundOffValue) as [DiscountAmount],
				SUBSTRING(CAST(ZOOD.[Total] AS VARCHAR(100)),1, CHARINDEX('.', CAST(ZOOD.[Total] AS VARCHAR(100)))+@RoundOffValue) as [Total],
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
                --ZOTOD.[SalesTax],
                --ZOTOD.[VAT],
                --ZOTOD.[GST],
                --ZOTOD.[PST],
                --ZOTOD.[HST],
                P.[StoreName] AS 'StoreName',
                OS.[OrderStateName] AS 'OrderStatus',
                PT.[Name] AS 'PaymentTypeName',
                ST.[Name] AS 'ShippingTypeName',
			 ISNULL(ZC.Symbol,@DefaultCurrencySymbol) Symbol,
			 -------
			 ZOOD.OmsOrderDetailsId
         INTO #GetOrderDetails
		 FROM ZNodeOmsOrder ZOO
              INNER JOIN ZnodeOmsOrderDetails ZOOD ON(ZOOD.OmsOrderId = ZOO.OmsOrderId
                                                      AND IsActive = 1)
              INNER JOIN ZNodePortal P ON P.PortalID = ZOOD.PortalId
              LEFT JOIN ZNodeOmsOrderState OS ON OS.OmsOrderStateID = ZOOD.OmsOrderStateID
              LEFT JOIN ZNodeOmsOrderShipment OSP ON(EXISTS
                                                    (
                                                        SELECT TOP 1 1
                                                        FROM ZnodeOmsOrderLineItems ZOOLI
                                                        WHERE ZOOLI.OmsOrderShipmentId = OSP.OmsOrderShipmentId
                                                              AND ZOOLI.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId
                                                    ))
              --LEFT JOIN ZnodeOmsTaxOrderDetails ZOTOD ON(ZOTOD.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId)
              LEFT JOIN ZNodeShipping S ON S.ShippingID = OSP.ShippingID
              LEFT JOIN ZNodeShippingTypes ST ON ST.ShippingTypeID = S.ShippingTypeID
              LEFT JOIN ZNodePaymentType PT ON PT.PaymentTypeID = ZOOD.PaymentTypeId
			  --LEFT JOIN ZnodeCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode )
			  LEFT JOIN ZnodeCulture ZC ON (ZC.CultureCode = ZOOD.CurrencyCode )
         WHERE
	    ((EXISTS
               (
                   SELECT TOP 1 1
                   FROM dbo.split(@PortalId, ',') SP
                   WHERE CAST(P.PortalId AS VARCHAR(100)) = SP.Item
                         OR @PortalId = ''
               )))
              AND ((EXISTS
                   (
                       SELECT TOP 1 1
                       FROM dbo.split(@OrderStateId, ',') SP
                       WHERE CAST(ZOOD.[OmsOrderStateId] AS VARCHAR(100)) = SP.Item
                             OR @OrderStateId = ''
                   )))
		  AND ((EXISTS
				(
				    SELECT TOP 1 1
				    FROM dbo.split(@PayemntTypeId, ',') SP
				    WHERE CAST(ZOOD.[PaymentTypeId] AS VARCHAR(100)) = SP.Item
					   OR @PayemntTypeId = '' OR ZOOD.PaymentTypeId IS NULL
				)))
		  AND ((EXISTS
				(
				    SELECT TOP 1 1
				    FROM dbo.split(@ShippingTypeId, ',') SP
				    WHERE CAST(ST.ShippingTypeID AS VARCHAR(100)) = SP.Item
					   OR @ShippingTypeId = ''
				)))
		  AND (ZOOD.[BillingFirstName]+' '+ISNULL(ZOOD.[BillingLastName], '') LIKE '%'+@CustomerName+'%'
				OR @CustomerName = '')
		  AND (ZOOD.[BillingEmailId] LIKE '%'+@EmailId+'%'
				OR @EmailId = '')
		  AND (CAST(ZOOD.OrderDate AS DATE) BETWEEN CASE
												WHEN @BeginDate IS NULL
												THEN CAST(ZOOD.OrderDate AS DATE)
												ELSE @BeginDate
											 END AND CASE
													   WHEN @EndDate IS NULL
													   THEN CAST(ZOOD.OrderDate AS DATE)
													   ELSE @EndDate
												    END);
			alter table #GetOrderDetails add [SalesTax] numeric(28,13),[VAT] numeric(28,13),[GST] numeric(28,13),[PST] numeric(28,13),[HST] numeric(28,13)

			UPDATE OD set [SalesTax] = ZOTOD.[SalesTax],[VAT] = ZOTOD.[VAT], [GST] = ZOTOD.[GST], [PST] = ZOTOD.[PST], [HST] = ZOTOD.[HST]
			FROM #GetOrderDetails OD 
			INNER JOIN ZnodeOmsTaxOrderDetails ZOTOD ON(ZOTOD.OmsOrderDetailsId = OD.OmsOrderDetailsId)

			select OrderNumber,	OmsOrderID,	PortalId,	UserId,	OmsOrderStateId	,ShippingID,	PaymentTypeId,	BillingFirstName,	
				BillingLastName,	BillingCompanyName,	BillingStreet1,	BillingStreet2,	BillingCity,	BillingStateCode,	
				BillingPostalCode,	BillingCountry,	BillingPhoneNumber,	BillingEmailId,	TaxCost,	ShippingCost,	SubTotal,	
				DiscountAmount,	Total,	OrderDate,	ShippingNumber,	TrackingNumber,	CouponCode,	PromoDescription,	ReferralUserId,
				PurchaseOrderNumber,	OmsPaymentStateId,	WebServiceDownloadDate,	PaymentSettingID,	ShipDate,	ReturnDate,	SalesTax,
				VAT,	GST,	PST,	HST,	StoreName,	OrderStatus,	PaymentTypeName,	ShippingTypeName,	Symbol
			from #GetOrderDetails 

		END TRY
		BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetOrderDetails @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@OrderStateId='+@OrderStateId+',@PayemntTypeId='+@PayemntTypeId+',@ShippingTypeId='+@ShippingTypeId+',@CustomerName='+@CustomerName+',@EmailId='+@EmailId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetOrderDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH
     END;