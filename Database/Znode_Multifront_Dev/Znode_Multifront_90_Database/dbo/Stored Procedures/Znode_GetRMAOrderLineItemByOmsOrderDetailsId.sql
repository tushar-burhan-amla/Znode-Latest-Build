     
CREATE PROCEDURE [dbo].[Znode_GetRMAOrderLineItemByOmsOrderDetailsId]
(   @OmsOrderDetailsId INT,
	@RMAID             INT         = 0,
	@IsReturnable      INT         = 0,
	@Flag              VARCHAR(10) = NULL,
	@IsDebug           BIT         = 0)
AS
/*
Summary: RETURNS OrderLineItems based on the OmsOrderDetailsId and RMA RequestId  
Unit Testing:
EXEC Znode_GetRMAOrderLineItemByOmsOrderDetailsId 80,15,0,'append'
*/
 BEGIN
      BEGIN TRY
      SET NOCOUNT ON;
     DECLARE @TBL_ZnodeOmsTaxOrderLineDetails TABLE
     (OmsOrderLineItemsId INT,
      SalesTax            NUMERIC(28, 6),
      VAT                 NUMERIC(28, 6),
      GST                 NUMERIC(28, 6),
      PST                 NUMERIC(28, 6),
      HST                 NUMERIC(28, 6)
     );

     INSERT INTO @TBL_ZnodeOmsTaxOrderLineDetails (OmsOrderLineItemsId,SalesTax,VAT,GST,PST,HST)
     SELECT ZOTOLD.OmsOrderLineItemsId,SUM(SalesTax),SUM(VAT),SUM(GST),SUM(PST),SUM(HST) FROM ZnodeOmsTaxOrderLineDetails ZOTOLD
     INNER JOIN ZnodeOmsOrderLineItems zooli ON ZOTOLD.OmsTaxOrderLineDetailsId = zooli.OmsOrderLineItemsId GROUP BY ZOTOLD.OmsOrderLineItemsId;

     IF(@Flag = 'append')
         BEGIN
             SELECT RItem.[OmsOrderLineItemsId] AS [OmsOrderLineItemID],OItem.[OmsOrderDetailsId],zood.CurrencyCode,OItem.[ProductName],OItem.[Description],
             OItem.[Quantity],OItem.[Price],OItem.[SKU],OItem.[DiscountAmount],OItem.[ShippingCost],OItem.[PromoDescription],
             (ZOTOLD.[SalesTax] / OItem.Quantity + ZOTOLD.[VAT] / OItem.Quantity + ZOTOLD.[GST] / OItem.Quantity + ZOTOLD.[PST] / OItem.Quantity + ZOTOLD.[HST] / OItem.Quantity) AS SalesTax,
             ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
                     AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 AND B.RMARequestId = @RMAId ), 0) RMAMaxQuantity,
             ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
                     AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND B.RMARequestId = @RMAId AND RmaReasonForReturnId IS NULL), 0) RMAQuantity,
             ISNULL((SELECT TOP 1 ISReturnable FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
                     AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 ), 0) ISReturnable,
             ISNULL((SELECT TOP 1 ISReceived FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
                     AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 ), 0) ISReceived,RItem.RmaReasonForReturnId,
             (SELECT Name FROM [dbo].[ZnodeRmaReasonForReturn] WHERE RmaReasonForReturnId = RItem.RmaReasonForReturnId) AS ReasonforReturn,
             (SELECT TOP 1 TaxCost FROM ZNodeRMARequest WHERE RMARequestID = @RMAId) TaxCost,
             (SELECT TOP 1 SubTotal FROM ZNodeRMARequest WHERE RMARequestID = @RMAId) SubTotal,
             (SELECT TOP 1 Total FROM ZNodeRMARequest WHERE RMARequestID = @RMAId) Total
             FROM [dbo].[ZnodeOmsOrderLineItems] OItem,[dbo].[ZnodeRMARequestItem] RItem,@TBL_ZnodeOmsTaxOrderLineDetails ZOTOLD,ZnodeOMSOrderDetails zood
             WHERE OItem.[OmsOrderDetailsId] = @OmsOrderDetailsId AND OItem.OmsOrderLineItemsId = RItem.OmsOrderLineItemsId AND ZOTOLD.OmsOrderLineItemsId = OItem.OmsOrderLineItemsId
             AND zood.OmsOrderDetailsId = OItem.OmsOrderDetailsId AND zood.IsActive = 1 AND RItem.RMARequestId = @RMAId
             AND (RItem.ISReturnable IS NULL OR RItem.ISReturnable = @IsReturnable)
             AND ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
                       AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 AND B.RMARequestId = @RMAId
             ), 0) > ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
                            AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND B.RMARequestId = @RMAId AND RmaReasonForReturnId IS NULL), 0);
         END;
     ELSE
     IF(@Flag = 'view')
         BEGIN
             SELECT RItem.[OmsOrderLineItemsId] AS [OrderLineItemID],OItem.[OmsOrderDetailsId],Zood.CurrencyCode,OItem.[ProductName],OItem.[Description],
             OItem.[Quantity],OItem.[Price],OItem.[SKU],OItem.[DiscountAmount],OItem.[ShippingCost],OItem.[PromoDescription],
             (ZOTOLD.[SalesTax] / OItem.Quantity + ZOTOLD.[VAT] / OItem.Quantity + ZOTOLD.[GST] / OItem.Quantity + ZOTOLD.[PST] / OItem.Quantity + ZOTOLD.[HST] / OItem.Quantity) AS SalesTax,
             ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
                     AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 AND B.RMARequestId = @RMAId ), 0) RMAMaxQuantity,
             ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
					 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND B.RMARequestId = @RMAId AND RmaReasonForReturnId IS NULL ), 1) RMAQuantity,
             ISNULL((SELECT TOP 1 ISReturnable FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
					 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 ), 0) ISReturnable,
             ISNULL((SELECT TOP 1 ISReceived FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
					 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 ), 0) ISReceived,RItem.RmaReasonForReturnId,
             (SELECT Name FROM [dbo].[ZnodeRmaReasonForReturn] WHERE RmaReasonForReturnId = RItem.RmaReasonForReturnId) AS ReasonforReturn,
             (SELECT TOP 1 TaxCost FROM ZNodeRMARequest WHERE RMARequestID = @RMAId) TaxCost,
             (SELECT TOP 1 SubTotal FROM ZNodeRMARequest WHERE RMARequestID = @RMAId) SubTotal,
             (SELECT TOP 1 Total FROM ZNodeRMARequest WHERE RMARequestID = @RMAId ) Total FROM [dbo].[ZnodeOmsOrderLineItems] OItem
             INNER JOIN [dbo].[ZNodeRMARequestItem] RItem ON OItem.OmsOrderLineItemsId = RItem.OmsOrderLineItemsId
             INNER JOIN ZNodeRMARequest B ON B.RMARequestID = RItem.RMARequestID
             INNER JOIN @TBL_ZnodeOmsTaxOrderLineDetails ZOTOLD ON ZOTOLD.OmsOrderLineItemsId = OItem.OmsOrderLineItemsId
             INNER JOIN ZnodeOMSOrderDetails zood ON zood.OmsOrderDetailsId = OItem.OmsOrderDetailsId AND zood.IsActive = 1                                                          
             WHERE zood.[OmsOrderDetailsId] = @OmsOrderDetailsId AND RItem.RMARequestId = @RMAId AND (RItem.ISReturnable IS NULL OR RItem.ISReturnable = @IsReturnable);
         END;
     ELSE
         BEGIN
             IF(@RMAID = 0)
                 BEGIN
                     SELECT OItem.[OmsOrderLineItemsId],OItem.[OmsOrderDetailsId],zood.CurrencyCode,OItem.[ProductName],OItem.[Description],OItem.[Quantity],
                     OItem.[Price] [UnitPrice],OItem.[SKU],OItem.[DiscountAmount],OItem.[ShippingCost],OItem.[PromoDescription],
                     (ZOTOLD.[SalesTax] / OItem.Quantity + ZOTOLD.[VAT] / OItem.Quantity + ZOTOLD.[GST] / OItem.Quantity + ZOTOLD.[PST] / OItem.Quantity + ZOTOLD.[HST] / OItem.Quantity) AS SalesTax,
                     ISNULL((SELECT TOP 1 ISReturnable FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
							 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND B.RMARequestId = @RMAId ), 0) ISReturnable,
                     ISNULL((SELECT TOP 1 ISReceived FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
							 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND B.RMARequestId = @RMAId ), 0) ISReceived,
                     ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
							 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 ), 0) RMAMaxQuantity,
                     ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
							 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND B.RMARequestId = @RMAId AND RmaReasonForReturnId > 0), 0) RMAQuantity,
                     0 RmaReasonForReturnId,'' AS ReasonforReturn FROM [dbo].[ZnodeOmsOrderLineItems] OItem
                     LEFT JOIN @TBL_ZnodeOmsTaxOrderLineDetails ZOTOLD ON ZOTOLD.OmsOrderLineItemsId = OItem.OmsOrderLineItemsId
                     LEFT JOIN ZnodeOMSOrderDetails zood ON zood.OmsOrderDetailsId = OItem.OmsOrderDetailsId  AND zood.ISActive = 1                                                                
                     WHERE OItem.[OmsOrderDetailsId] = @OmsOrderDetailsId  AND OItem.[Price] > 0
                     AND OItem.[Quantity] > ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID
                     WHERE B.RmaRequestStatusId != 4 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0), 0);
                 END;
             ELSE
                 BEGIN
                     SELECT RItem.[OmsOrderLineItemsId] AS [OmsOrderLineItemsId],OItem.[OmsOrderDetailsId],zood.CurrencyCode,OItem.[ProductName],OItem.[Description],
                     OItem.[Quantity],OItem.[Price],OItem.[SKU],OItem.[DiscountAmount],OItem.[ShippingCost],OItem.[PromoDescription],
                     (ZOTOLD.[SalesTax] / OItem.Quantity + ZOTOLD.[VAT] / OItem.Quantity + ZOTOLD.[GST] / OItem.Quantity + ZOTOLD.[PST] / OItem.Quantity + ZOTOLD.[HST] / OItem.Quantity) AS SalesTax,
                     ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
							 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 AND B.RMARequestId = @RMAId), 0) RMAMaxQuantity,
                     ISNULL((SELECT SUM(Quantity) FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
							 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND B.RMARequestId = @RMAId AND RmaReasonForReturnId IS NULL), 0) RMAQuantity,
                     ISNULL((SELECT TOP 1 ISReturnable FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
							 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 ), 0) ISReturnable,
                     ISNULL((SELECT TOP 1 ISReceived FROM ZNodeRMARequestItem A INNER JOIN ZNodeRMARequest B ON B.RMARequestID = A.RMARequestID WHERE B.RmaRequestStatusId != 4
							 AND OmsOrderLineItemsId = OItem.[OmsOrderLineItemsId] AND RmaReasonForReturnId > 0 ), 0) ISReceived,RItem.RmaReasonForReturnId,
                     (SELECT Name FROM [dbo].[ZnodeRmaReasonForReturn] WHERE [ZnodeRmaReasonForReturn].RmaReasonForReturnId = RItem.RmaReasonForReturnId                     ) AS ReasonforReturn,
                     (SELECT TOP 1 TaxCost FROM ZNodeRMARequest WHERE RMARequestID = @RMAId ) TaxCost,
                     (SELECT TOP 1 SubTotal FROM ZNodeRMARequest WHERE RMARequestID = @RMAId) SubTotal,
                     (SELECT TOP 1 Total FROM ZNodeRMARequest WHERE RMARequestID = @RMAId ) Total
                      FROM [dbo].[ZnodeOmsOrderLineItems] OItem  INNER JOIN [dbo].[ZNodeRMARequestItem] RItem ON OItem.OmsOrderLineItemsId = RItem.OmsOrderLineItemsId                         
                      INNER JOIN ZNodeRMARequest B ON B.RMARequestID = RItem.RMARequestID
                      INNER JOIN @TBL_ZnodeOmsTaxOrderLineDetails ZOTOLD ON ZOTOLD.OmsOrderLineItemsId = OItem.OmsOrderLineItemsId
                      INNER JOIN ZnodeOMSOrderDetails zood ON zood.OmsOrderDetailsId = OItem.OmsOrderDetailsId  AND zood.ISActive = 1                                                                 
                      WHERE OItem.[OmsOrderDetailsId] = @OmsOrderDetailsId AND B.RmaRequestStatusId != 4 AND RItem.RMARequestId = @RMAId 
					  AND (RItem.ISReturnable IS NULL OR RItem.ISReturnable = @IsReturnable);
                 END;
         END;;
		 
		 END TRY
		 BEGIN CATCH
		     DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetRMAOrderLineItemByOmsOrderDetailsId @OmsOrderDetailsId = '+CAST(@OmsOrderDetailsId AS VARCHAR(max))+',@RMAID='+CAST(@RMAID AS VARCHAR(50))+',@IsReturnable='+CAST(@IsReturnable AS VARCHAR(50))+',@Flag='+@Flag+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetRMAOrderLineItemByOmsOrderDetailsId',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		 END CATCH

		 END