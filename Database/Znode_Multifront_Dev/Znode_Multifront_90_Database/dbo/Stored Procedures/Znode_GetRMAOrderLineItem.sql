CREATE PROCEDURE [dbo].[Znode_GetRMAOrderLineItem]
(   @OmsOrderDetailsId INT,
	@RmaId             INT         = 0,
	@IsReturnable      INT         = 0,
	@Flag              VARCHAR(10) = ''
)
AS
/*
     Summary :- This Procedure is Used to get the RMA order line item details 
     Unit Testing 
     EXEC [dbo].[Znode_GetRMAOrderLineItem] 7,0,1,''
*/

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TBL_OmsTaxOrderLineDetails TABLE
             (OmsOrderLineItemsId INT,
              SalesTax            NUMERIC(28, 6),
              VAT                 NUMERIC(28, 6),
              GST                 NUMERIC(28, 6),
              PST                 NUMERIC(28, 6),
              HST                 NUMERIC(28, 6),
              SaleTaxTotal        NUMERIC(28, 6)
             );
             DECLARE @TBL_OmsOrderLineItem TABLE
             ([OmsOrderLineItemsId] INT,
              [OmsOrderDetailsId]   INT,
              CurrencyCode          VARCHAR(100),
              [ProductName]         NVARCHAR(MAX),
              [Description]         NVARCHAR(MAX),
              [Quantity]            NUMERIC(28, 6),
              [Price]               NUMERIC(28, 6),
              [SKU]                 NVARCHAR(MAX),
              [DiscountAmount]      NUMERIC(28, 6),
              [ShippingCost]        NUMERIC(28, 6),
              PromoDescription      NVARCHAR(MAX),
			  OrderNumber		    VARCHAR(200)		
             );
             DECLARE @TBL_RMARequestItem TABLE
             ([OmsOrderLineItemsId] INT,
              RMARequestId          INT,
              ISReturnable          BIT,
              ISReceived            BIT,
              Quantity              NUMERIC(28, 6),
              RmaReasonForReturnId  INT,
              ReasonForReturn       NVARCHAR(1000),
              TaxCost               NUMERIC(28, 6),
              SubTotal              NUMERIC(28, 6),
              Total                 NUMERIC(28, 6)
             );
             DECLARE @SumOfRAMQuantity NUMERIC(28, 6), @SumOfRAMmaxQuantity NUMERIC(28, 6);

             INSERT INTO @TBL_OmsTaxOrderLineDetails (OmsOrderLineItemsId,SalesTax,VAT,GST,PST,HST)
             SELECT ZOTOLD.OmsOrderLineItemsId,SUM(SalesTax),SUM(VAT),SUM(GST),SUM(PST),SUM(HST) FROM ZnodeOmsTaxOrderLineDetails ZOTOLD
             INNER JOIN ZnodeOmsOrderLineItems ZOOLI ON ZOTOLD.OmsOrderLineItemsId = ZOOLI.OmsOrderLineItemsId WHERE ZOOLI.OmsOrderDetailsId = @OmsOrderDetailsId
             GROUP BY ZOTOLD.OmsOrderLineItemsId;

             INSERT INTO @TBL_OmsOrderLineItem ([OmsOrderLineItemsId],[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],[Quantity],[Price],[SKU],
             [DiscountAmount],[ShippingCost],PromoDescription,OrderNumber)
             SELECT [OmsOrderLineItemsId],ZOOLI.[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],[Quantity],[Price],[SKU],ZOOLI.[DiscountAmount],
             ZOOLI.[ShippingCost],ZOOLI.PromoDescription,ZOO.OrderNumber FROM ZnodeOmsOrderLIneItems ZOOLI
             INNER JOIN ZnodeOmsOrderDetails ZOODT ON(ZOODT.OmsOrderDetailsId = ZOOLI.OmsOrderDetailsId)
             INNER JOIN ZnodeOmsOrder ZOO ON (ZOO.OmsOrderId = ZOODT.OmsOrderId) WHERE ZOOLI.OmsOrderDetailsId = @OmsOrderDetailsId;

             INSERT INTO @TBL_RMARequestItem (OmsOrderLineItemsId,RMARequestId,ISReturnable,ISReceived,Quantity,RmaReasonForReturnId,ReasonForReturn,TaxCost,
             SubTotal,Total)
             SELECT ZRR.OmsOrderLineItemsId,ZRR.RMARequestId,ISNULL(ISReturnable, 0),ISNULL(ISReceived, 0),Quantity,ZRR.RmaReasonForReturnId,ZRRFR.Name ReasonForReturn,
             ZR.TaxCost,ZR.SubTotal,ZR.Total FROM ZNodeRMARequestItem ZRR LEFT JOIN ZNodeRMARequest ZR ON ZR.RMARequestID = ZRR.RMARequestID
             LEFT JOIN [ZnodeRmaReasonForReturn] ZRRFR ON(ZRRFR.RmaReasonForReturnId = ZRR.RmaReasonForReturnId)
             WHERE ISNULL(ZR.RmaRequestStatusId, 0) != ISNULL([dbo].[Fn_GetVoidRmaRequestStatusId](), 0) AND EXISTS(SELECT TOP 1 1 FROM @TBL_OmsOrderLineItem TBORL
             WHERE TBORL.OmsOrderLineItemsId = ZRR.OmsOrderLineItemsId) AND ISNULL(ZRR.RmaReasonForReturnId, 0) >= 0;

			 -- SELECT * FROM  @TBL_RMARequestItem

             SET @SumOfRAMmaxQuantity = ISNULL((SELECT SUM(Quantity) FROM @TBL_RMARequestItem TBRI 
			                            WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_OmsOrderLineItem TBOLT WHERE TBOLT.OmsOrderLineItemsId = TBRI.OmsOrderLineItemsId
                                                     AND TBRI.RMARequestId = @RmaId AND TBRI.RmaReasonForReturnId IS NOT NULL)
                                              ), 0);

             SET @SumOfRAMQuantity = ISNULL((SELECT SUM(Quantity) FROM @TBL_RMARequestItem TBRIT
                                     WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_OmsOrderLineItem TBOLT WHERE TBOLT.OmsOrderLineItemsId = TBRIT.OmsOrderLineItemsId
                                                  AND TBRIT.RMARequestId = @RmaId AND TBRIT.RmaReasonForReturnId IS NULL)
                                           ), 0);
             
             IF @Flag = '' AND @RmaId = 0
                 BEGIN
                     SELECT TBOLT.[OmsOrderLineItemsId],[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],TBOLT.[Quantity],[Price],[SKU],[DiscountAmount],
                     [ShippingCost],PromoDescription,OrderNumber,
                     (TBOTIL.[SalesTax] / TBOLT.Quantity + TBOTIL.[VAT] / TBOLT.Quantity + TBOTIL.[GST] / TBOLT.Quantity + TBOTIL.[PST] / TBOLT.Quantity + TBOTIL.[HST] / TBOLT.Quantity) AS SalesTax,
                     ISNULL(TBRI.ISReturnable, 0) ISReturnable,ISNULL(TBRI.ISReceived, 0) ISReceived,SUM(TBRI.Quantity) RMAMaxQuantity,SUM(TBRIT.Quantity) RMAQuantity,
                     TBRI.RmaReasonForReturnId,TBRI.ReasonforReturn FROM @TBL_OmsOrderLineItem TBOLT
                     LEFT JOIN @TBL_OmsTaxOrderLineDetails TBOTIL ON(TBOTIL.OmsOrderLineItemsId = TBOLT.OmsOrderLineItemsId)
                     LEFT JOIN @TBL_RMARequestItem TBRI ON(TBOLT.OmsOrderLineItemsId = TBRI.OmsOrderLineItemsId)
                     LEFT JOIN @TBL_RMARequestItem TBRIT ON(TBOLT.OmsOrderLineItemsId = TBRIT.OmsOrderLineItemsId AND TBRI.RMARequestId = @RmaId)
                     WHERE TBOLT.Price > 0 AND  ISNULL(TBRI.ISReturnable, 0) = @IsReturnable  AND NOT EXISTS
                     (SELECT TOP 1 1 FROM @TBL_RMARequestItem TBRIO WHERE TBOLT.OmsOrderLineItemsId = TBRIO.OmsOrderLineItemsId AND ISNULL(TBRIO.ISReturnable,0) = @IsReturnable  
                     GROUP BY TBRIO.Quantity 
					 HAVING SUM(TBRIO.Quantity) > TBOLT.[Quantity])
					 GROUP BY TBOLT.[OmsOrderLineItemsId],[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],TBOLT.[Quantity],[Price],[SKU],[DiscountAmount],
                     [ShippingCost],PromoDescription,OrderNumber,
                     (TBOTIL.[SalesTax] / TBOLT.Quantity + TBOTIL.[VAT] / TBOLT.Quantity + TBOTIL.[GST] / TBOLT.Quantity + TBOTIL.[PST] / TBOLT.Quantity + TBOTIL.[HST] / TBOLT.Quantity),
                     TBRI.ISReturnable,TBRI.ISReceived,TBRI.RmaReasonForReturnId,TBRI.ReasonforReturn
					  HAVING TBOLT.[Quantity] > SUM(ISNULL(TBRI.Quantity,0)   ) ;
                 END;
             ELSE
             IF @Flag = 'View' AND @RmaId <> 0
                
                 BEGIN
                     SELECT TBOLT.[OmsOrderLineItemsId],[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],TBOLT.[Quantity],[Price],[SKU],[DiscountAmount],
                     [ShippingCost],PromoDescription,OrderNumber,
                     (TBOTIL.[SalesTax] / TBOLT.Quantity + TBOTIL.[VAT] / TBOLT.Quantity + TBOTIL.[GST] / TBOLT.Quantity + TBOTIL.[PST] / TBOLT.Quantity + TBOTIL.[HST] / TBOLT.Quantity) AS SalesTax,
                     TBRI.ISReturnable,TBRI.ISReceived,TBRI.[Quantity] RMAMaxQuantity,ISNULL(TBRIT.Quantity,0) RMAQuantity,TBRI.RmaReasonForReturnId,TBRI.ReasonforReturn,
                     TBRI.TaxCost,TBRI.SubTotal,TBRI.Total FROM @TBL_OmsOrderLineItem TBOLT
                     LEFT JOIN @TBL_OmsTaxOrderLineDetails TBOTIL ON(TBOTIL.OmsOrderLineItemsId = TBOLT.OmsOrderLineItemsId)
                     INNER JOIN @TBL_RMARequestItem TBRI ON(TBOLT.OmsOrderLineItemsId = TBRI.OmsOrderLineItemsId AND TBRI.RMARequestId = @RmaId)
                     LEFT JOIN @TBL_RMARequestItem TBRIT ON(TBOLT.OmsOrderLineItemsId = TBRIT.OmsOrderLineItemsId AND TBRIT.RMARequestId = @RmaId
                     AND TBRIT.RmaReasonForReturnId IS NULL) WHERE TBOLT.Price > 0 AND TBRI.RMARequestId = @RmaId AND (TBRI.ISReturnable IS NULL OR TBRI.ISReturnable = @IsReturnable)
                     GROUP BY TBOLT.[OmsOrderLineItemsId],[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],TBOLT.[Quantity],[Price],[SKU],[DiscountAmount],
                     [ShippingCost],PromoDescription,OrderNumber,
                     (TBOTIL.[SalesTax] / TBOLT.Quantity + TBOTIL.[VAT] / TBOLT.Quantity + TBOTIL.[GST] / TBOLT.Quantity + TBOTIL.[PST] / TBOLT.Quantity + TBOTIL.[HST] / TBOLT.Quantity),
                     TBRI.ISReturnable,TBRI.ISReceived,TBRI.RmaReasonForReturnId,TBRI.ReasonforReturn,TBRI.TaxCost,TBRI.SubTotal,TBRI.Total,TBRI.Quantity,TBRIT.Quantity;
                 END;
             ELSE
             IF @Flag = 'Edit' AND @RmaId <> 0
                
                 BEGIN
                     SELECT TBOLT.[OmsOrderLineItemsId],[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],TBOLT.[Quantity],[Price],[SKU],[DiscountAmount],
                     [ShippingCost],PromoDescription,OrderNumber,
                     (TBOTIL.[SalesTax] / TBOLT.Quantity + TBOTIL.[VAT] / TBOLT.Quantity + TBOTIL.[GST] / TBOLT.Quantity + TBOTIL.[PST] / TBOLT.Quantity + TBOTIL.[HST] / TBOLT.Quantity) AS SalesTax,
                     TBRI.ISReturnable,TBRI.ISReceived,SUM(TBRI.Quantity) RMAMaxQuantity,SUM(TBRIT.Quantity) RMAQuantity,TBRI.RmaReasonForReturnId,TBRI.ReasonforReturn,
                     TBRI.TaxCost,TBRI.SubTotal,TBRI.Total FROM @TBL_OmsOrderLineItem TBOLT
                     LEFT JOIN @TBL_OmsTaxOrderLineDetails TBOTIL ON(TBOTIL.OmsOrderLineItemsId = TBOLT.OmsOrderLineItemsId)
                     LEFT JOIN @TBL_RMARequestItem TBRI ON(TBOLT.OmsOrderLineItemsId = TBRI.OmsOrderLineItemsId AND TBRI.RMARequestId = @RmaId)
                     LEFT JOIN @TBL_RMARequestItem TBRIT ON(TBOLT.OmsOrderLineItemsId = TBRIT.OmsOrderLineItemsId AND TBRIT.RMARequestId = @RmaId
                     AND TBRIT.RmaReasonForReturnId IS NULL) WHERE TBOLT.Price > 0 AND TBRI.RMARequestId = @RmaId AND (TBRI.ISReturnable = 0 OR TBRI.ISReturnable = @IsReturnable)
                     GROUP BY TBOLT.[OmsOrderLineItemsId],[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],TBOLT.[Quantity],[Price],[SKU],[DiscountAmount],
                     [ShippingCost],PromoDescription,OrderNumber,
                     (TBOTIL.[SalesTax] / TBOLT.Quantity + TBOTIL.[VAT] / TBOLT.Quantity + TBOTIL.[GST] / TBOLT.Quantity + TBOTIL.[PST] / TBOLT.Quantity + TBOTIL.[HST] / TBOLT.Quantity),
                     TBRI.ISReturnable,TBRI.ISReceived,TBRI.RmaReasonForReturnId,TBRI.ReasonforReturn,TBRI.TaxCost,TBRI.SubTotal,TBRI.Total;
                 END;
             ELSE
             IF @Flag = 'Append' AND @RmaId <> 0
               
                 BEGIN
                     SET @SumOfRAMmaxQuantity = ISNULL((SELECT SUM(Quantity) FROM @TBL_RMARequestItem TBRI WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_OmsOrderLineItem TBOLT
                                                        WHERE TBOLT.OmsOrderLineItemsId = TBRI.OmsOrderLineItemsId AND TBRI.RMARequestId = @RmaId
                                                        AND TBRI.RmaReasonForReturnId IS NOT NULL)), 0);

                     SET @SumOfRAMQuantity = ISNULL((SELECT SUM(Quantity) FROM @TBL_RMARequestItem TBRIT WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_OmsOrderLineItem TBOLT
                                                     WHERE TBOLT.OmsOrderLineItemsId = TBRIT.OmsOrderLineItemsId AND TBRIT.RMARequestId = @RmaId
                                                     AND TBRIT.RmaReasonForReturnId IS NULL)), 0);

                     SELECT TBOLT.[OmsOrderLineItemsId],[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],TBOLT.[Quantity],[Price],[SKU],[DiscountAmount],
                     [ShippingCost],PromoDescription,OrderNumber,
                     (TBOTIL.[SalesTax] / TBOLT.Quantity + TBOTIL.[VAT] / TBOLT.Quantity + TBOTIL.[GST] / TBOLT.Quantity + TBOTIL.[PST] / TBOLT.Quantity + TBOTIL.[HST] / TBOLT.Quantity) AS SalesTax,
                     TBRI.ISReturnable,TBRI.ISReceived,TBRI.Quantity RMAMaxQuantity,SUM(ISNULL(TBRIT.Quantity,0))  RMAQuantity,TBRI.RmaReasonForReturnId,
                     TBRI.ReasonforReturn,TBRI.TaxCost,TBRI.SubTotal,TBRI.Total FROM @TBL_OmsOrderLineItem TBOLT
                     LEFT JOIN @TBL_OmsTaxOrderLineDetails TBOTIL ON(TBOTIL.OmsOrderLineItemsId = TBOLT.OmsOrderLineItemsId)
                     LEFT JOIN @TBL_RMARequestItem TBRI ON(TBOLT.OmsOrderLineItemsId = TBRI.OmsOrderLineItemsId AND TBRI.RMARequestId = @RmaId AND TBRI.RmaReasonForReturnId IS NOT NULL)
                     LEFT JOIN @TBL_RMARequestItem TBRIT ON(TBOLT.OmsOrderLineItemsId = TBRIT.OmsOrderLineItemsId AND TBRIT.RMARequestId = @RmaId
                     AND TBRIT.RmaReasonForReturnId IS NULL) 
					 WHERE TBOLT.Price > 0 AND TBRI.RMARequestId = @RmaId AND (TBRI.ISReturnable IS NULL 
					 OR TBRI.ISReturnable = @IsReturnable) AND @SumOfRAMmaxQuantity > @SumOfRAMQuantity
					
                     GROUP BY TBOLT.[OmsOrderLineItemsId],[OmsOrderDetailsId],CurrencyCode,[ProductName],[Description],TBOLT.[Quantity],[Price],[SKU],[DiscountAmount],
                     [ShippingCost],PromoDescription,OrderNumber,
                     (TBOTIL.[SalesTax] / TBOLT.Quantity + TBOTIL.[VAT] / TBOLT.Quantity + TBOTIL.[GST] / TBOLT.Quantity + TBOTIL.[PST] / TBOLT.Quantity + TBOTIL.[HST] / TBOLT.Quantity),
                     TBRI.ISReturnable,TBRI.ISReceived,TBRI.RmaReasonForReturnId,TBRI.ReasonforReturn,TBRI.TaxCost,TBRI.SubTotal,TBRI.Total,TBRI.Quantity
					 HAVING TBRI.Quantity > SUM(ISNULL(TBRIT.Quantity,0)) 
					 ;
                 END;;; 
				
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetRMAOrderLineItem @OmsOrderDetailsId = '+CAST(@OmsOrderDetailsId AS VARCHAR(max))+',@RmaId='+CAST(@RmaId AS VARCHAR(50))+',@IsReturnable='+CAST(@IsReturnable AS VARCHAR(50))+',@Flag='+@Flag+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetRMAOrderLineItem',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;