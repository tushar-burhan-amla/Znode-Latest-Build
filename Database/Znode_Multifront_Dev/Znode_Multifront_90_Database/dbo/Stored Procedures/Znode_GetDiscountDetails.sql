CREATE PROCEDURE [dbo].[Znode_GetDiscountDetails]
(
  @RmaReturnDetailsId INT 
)
AS
/*
   
    Summary : Get discount details for return line items
    EXEC Znode_GetCaseRequest @WhereClause = '',@Rows = 50,@PageNo = 1 ,@Order_BY = NULL,@RowsCount = 0,@IsCaseHistory = 0 
    
  */
BEGIN
SET NOCOUNT ON;
BEGIN TRY
	SELECT OD.OmsOrderDiscountId,OD.OmsOrderDetailsId,RLI.OmsOrderLineItemsId AS OmsOrderLineItemId, OLI.ParentOmsOrderLineItemsId, 
		OD.OmsDiscountTypeId,OD.DiscountCode,OD.DiscountAmount,OD.Description,OD.OmsDiscountTypeId,OD.PerQuantityDiscount,
		OD.DiscountLevelTypeId,OD.PromotionTypeId,OD.DiscountAppliedSequence,OD.DiscountMultiplier,OD.PromotionName,OD.PromotionMessage
	FROM ZnodeRmaReturnLineItems RLI 
	INNER JOIN ZnodeOmsOrderLineItems OLI ON OLI.OmsOrderLineItemsId = RLI.OmsOrderLineItemsId
	INNER JOIN ZnodeOmsOrderDiscount OD ON ( OD.OmsOrderLineItemId = OLI.OmsOrderLineItemsId OR OD.OmsOrderLineItemId = OLI.ParentOmsOrderLineItemsId )
	WHERE RLI.RmaReturnDetailsId = @RmaReturnDetailsId			 									 			 			 
END TRY
BEGIN CATCH
	DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetDiscountFDetails @RmaReturnDetailsId ='+CAST(@RmaReturnDetailsId AS VARCHAR(50));
              			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetDiscountDetails',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH;
END;