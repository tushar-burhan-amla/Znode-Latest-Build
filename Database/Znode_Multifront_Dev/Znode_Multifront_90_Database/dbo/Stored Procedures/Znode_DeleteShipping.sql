
CREATE PROCEDURE [dbo].[Znode_DeleteShipping]
(   @ShippingId VARCHAR(max),
    @Status     INT OUT,
	@IsForceFullyDelete BIT = 0 )
AS
/*
Summary: This Procedure is used to delete shipping details
Unit Testing
begin tran
  EXEC [dbo].[Znode_DeleteShipping] 1,0
 rollback tran
 */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			  DECLARE @StatusOut Table (Id INT ,Message NVARCHAR(max), Status BIT )
			  DECLARE @DeletedIds TransferId 
             BEGIN TRAN A;
             DECLARE @DeletdShippingId TABLE(ShippingId INT);
             INSERT INTO @DeletdShippingId
                    SELECT Item
                    FROM dbo.split(@ShippingId, ',') AS a
					WHERE (NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsOrderDetails asa WHERE asa.ShippingId = a.Item )	 OR @IsForceFullyDelete =1 ) 
					AND @ShippingId <> ''
					AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeShipping s WHERE s.ShippingId = a.Item  AND s.ShippingCode = 'FreeShipping')
					;
             DELETE FROM ZnodeShippingSKU
             WHERE ShippingRuleId IN
             (
                 SELECT ShippingRuleId
                 FROM ZNodeShippingRule
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @DeletdShippingId AS a
                     WHERE a.ShippingId = ZNodeShippingRule.ShippingId
                 )
             );
             DELETE FROM ZNodeShippingRule
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZNodeShippingRule.ShippingId
             );

			INSERT INTO @DeletedIds 
			SELECT DISTINCT a.OmsOrderId 
			FROM ZnodeOmsOrder A 
			INNER JOIN ZnodeOMsOrderDetails b  ON (b.OmsOrderId = a.OmsOrderId )
			WHERE   EXISTS ( SELECT TOP 1 1 FROM @DeletdShippingId AS TBP WHERE TBP.ShippingId = b.ShippingId)

			INSERT INTO @StatusOut (Id ,Status) 
			EXEC [dbo].[Znode_DeleteOrderById] @OmsOrderIds = @DeletedIds , @status = 0 
		    
			DELETE FROM ZnodeOMSQuoteApproval WHERE OmsQuoteId 
			IN (SELECT OmsQuoteId FROM ZnodeOmsQuote WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZnodeOmsQuote.ShippingId
             ));

		
			 DELETE FROM ZnodeOmsNotes WHERE  OmsQuoteId 
			IN (SELECT OmsQuoteId FROM ZnodeOmsQuote WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZnodeOmsQuote.ShippingId
             ));


			DELETE FROM ZnodeOmsQuotePersonalizeItem WHERE OmsQuoteLineItemId IN (SELECT OmsQuoteLineItemId FROM ZnodeOmsQuoteLineItem WHERE OmsQuoteId 
			IN (SELECT OmsQuoteId FROM ZnodeOmsQuote WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZnodeOmsQuote.ShippingId
             )))
			DELETE FROM ZnodeOmsQuoteLineItem WHERE OmsQuoteId 
			IN (SELECT OmsQuoteId FROM ZnodeOmsQuote WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZnodeOmsQuote.ShippingId
             ))
			DELETE FROM ZnodeOmsQuote WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZnodeOmsQuote.ShippingId
             )
			DELETE FROM ZnodePortalShipping  WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZnodePortalShipping.ShippingId
             )

			 DELETE FROM ZnodeProfileShipping WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZnodeProfileShipping.ShippingId
             );

			 DELETE FROM ZnodeOmsOrderLineItems
			 where exists(select * FROM ZnodeOmsOrderShipment B WHERE EXISTS
							 (
								 SELECT TOP 1 1
								 FROM @DeletdShippingId AS a
								 WHERE a.ShippingId = B.ShippingId
							 )
			 AND ZnodeOmsOrderLineItems.OmsOrderShipmentId = B.OmsOrderShipmentId );

			  DELETE FROM ZnodeOmsOrderShipment WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZnodeOmsOrderShipment.ShippingId
             );


             DELETE FROM ZnodeShipping
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @DeletdShippingId AS a
                 WHERE a.ShippingId = ZnodeShipping.ShippingId
             );
            
			 IF
             (
                 SELECT COUNT(1)
                 FROM @DeletdShippingId
             ) =
             (
                 SELECT COUNT(1)
                 FROM dbo.split(@ShippingId, ',') AS a WHERE @ShippingId <> ''
             )
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS Status;
							SET @Status = 1;

                 END;

			ELSE IF EXISTS (SELECT Item
            FROM dbo.split(@ShippingId, ',') AS a WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeShipping s WHERE s.ShippingId = a.Item  AND s.ShippingCode = 'FreeShipping'))
			BEGIN
				 
			SELECT 2 AS ID,
                    CAST(0 AS BIT)  AS Status;
					SET @Status = 0;

			END
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS Status;
							SET @Status = 0;
                 END;
             
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
		     SELECT ERROR_MESSAGE()
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteShipping @ShippingId = '+@ShippingId+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteShipping',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;