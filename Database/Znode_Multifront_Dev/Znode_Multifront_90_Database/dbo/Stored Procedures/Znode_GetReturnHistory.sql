CREATE PROCEDURE [dbo].[Znode_GetReturnHistory]
( 
	@RmaReturnDetailsId  INT 
)
AS
  /*  
   Summary : This procedure is used to get the return history                
 Records are fetched for those have history form Notes and History table
Unit Testing:

   EXEC [Znode_GetReturnHistory] 12
     
*/
BEGIN
   SET NOCOUNT ON
   BEGIN TRY

		;With Cte_GetReturnHistory AS
		(
			SELECT  zrh.RmaReturnDetailsId,TransactionId, '' Notes,Message ,ReturnAmount,zrh.CreatedBy,zrh.CreatedDate ,zrh.ModifiedBy,zrh.ModifiedDate
			FROM ZnodeRmaReturnHistory zrh
			WHERE zrh.RmaReturnDetailsId = @RmaReturnDetailsId
			UNION ALL
			select  zrn.RmaReturnDetailsId,null TransactionId,  Notes,'Return notes added successfully' Message ,null ReturnAmount,CreatedBy ,createdDate, ModifiedBy,ModifiedDate
			from ZnodeRmaReturnNotes zrn
			WHERE zrn.RmaReturnDetailsId = @RmaReturnDetailsId
		)
		SELECT  RmaReturnDetailsId,TransactionId,  Notes, Message , ReturnAmount,tHst.CreatedBy ,tHst.createdDate, tHst.ModifiedBy,tHst.ModifiedDate,zusr.Email 'UpdatedBy'
		FROM Cte_GetReturnHistory tHst
		left join ZnodeUser zusr on zusr.UserId =tHst.ModifiedBy
		order by tHst.CreatedDate desc

	END TRY
    BEGIN CATCH
           
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetReturnHistory @WhereClause = '+CAST(@RmaReturnDetailsId AS varchar(10));

        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetReturnHistory',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
    END CATCH;
END;