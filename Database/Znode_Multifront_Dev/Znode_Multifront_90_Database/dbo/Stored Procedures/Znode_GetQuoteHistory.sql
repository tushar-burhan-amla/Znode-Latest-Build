CREATE PROCEDURE [dbo].[Znode_GetQuoteHistory]
( 
	@OMSQuoteId  INT 
)
AS 
   /*  
    Summary : This procedure is used to get the quote history                 
  Records are fetched for those have history form Notes and History table	 	 
Unit Testing:

    EXEC [Znode_GetQuoteHistory] 12
       
*/
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY

		;With Cte_GetHistory AS 
		(
			 SELECT  zhst.OMSQuoteId, '' Notes,zhst.Message, OrderAmount,zhst.CreatedBy,zhst.CreatedDate ,zhst.ModifiedBy,zhst.ModifiedDate
			 FROM ZnodeOmsQuoteHistory zhst
			 WHERE zhst.OMSQuoteId = @OMSQuoteId
			 union all
			 select  zon.OMSQuoteId,Notes,'Quote notes added successfully' Message, null OrderAmount,zon.CreatedBy ,zon.createdDate,zon.ModifiedBy,zon.ModifiedDate
			 from ZnodeOmsNotes zon 
			 WHERE zon.OMSQuoteId = @OMSQuoteId
		)
		select GH.OMSQuoteId,Notes,Message,GH.OrderAmount,GH.CreatedBy,GH.CreatedDate,GH.ModifiedBy,GH.ModifiedDate,zusr.Email 'UpdatedBy'
		from Cte_GetHistory GH
		left join ZnodeUser zusr on zusr.UserId = GH.ModifiedBy 
		order by GH.CreatedDate desc
				

    END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetQuoteHistory '+ISNULL(CAST(@OMSQuoteId AS VARCHAR(50)),'''');
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetQuoteHistory',
		@ErrorInProcedure = 'Znode_GetQuoteHistory',
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH;
END