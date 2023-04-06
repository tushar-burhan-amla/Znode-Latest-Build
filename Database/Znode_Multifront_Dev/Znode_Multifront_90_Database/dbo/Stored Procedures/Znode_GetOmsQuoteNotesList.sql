CREATE PROCEDURE [dbo].[Znode_GetOmsQuoteNotesList]    
(     
  @OmsQuoteId int = 0 
 )    
AS     
   /*    
  Summary :- This procedure is used to get the Quote notes list
     Unit Testing  
     EXEC Znode_GetOmsQuoteNotesList @OmsQuoteId = 3      
    
*/    
     BEGIN    
         BEGIN TRY    
			SET NOCOUNT ON; 
			
			SELECT ZU.UserId, ANZU.UserName 
			INTO #QuoteUserDetail
			FROM ZnodeUser ZU 
			INNER JOIN AspNetUsers ANU ON ZU.AspNetUserId = ANU.Id
			INNER JOIN AspNetZnodeUser ANZU ON ANU.UserName = ANZU.AspNetZnodeUserId
			where exists(select * from ZnodeOmsNotes ZON where ZU.Userid = ZON.CreatedBy AND ZON.OmsQuoteId is not null)  
			
			select ZON.CreatedDate, ZON.Notes, UserName as UpdatedBy
			from ZnodeOmsNotes ZON 
			INNER JOIN #QuoteUserDetail ZU ON ZON.ModifiedBy = ZU.UserId
			where ZON.OmsQuoteId is not null AND (ZON.OmsQuoteId = @OmsQuoteId OR @OmsQuoteId = 0 )
        
		END TRY    
        BEGIN CATCH    
		DECLARE @Status BIT ;    
		SET @Status = 0;    
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOmsQuoteNotesList @WhereClause = '+CAST(@OmsQuoteId AS VARCHAR(max));    
                      
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
		EXEC Znode_InsertProcedureErrorLog    
		@ProcedureName = 'Znode_GetOmsQuoteNotesList',    
		@ErrorInProcedure = @Error_procedure,    
		@ErrorMessage = @ErrorMessage,    
		@ErrorLine = @ErrorLine,    
		@ErrorCall = @ErrorCall;    
         END CATCH;    
     END
