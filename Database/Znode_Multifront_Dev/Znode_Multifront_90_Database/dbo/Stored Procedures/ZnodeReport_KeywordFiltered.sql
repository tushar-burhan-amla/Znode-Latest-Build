CREATE PROCEDURE [dbo].[ZnodeReport_KeywordFiltered]
    (
         @PortalId		VARCHAR(max)  = ''
		,@BeginDate		DATE		  = NULL 
		,@EndDate       DATE          = NULL  
    )
AS 
/*
	 Summary:- This Procedure is used to get the activity log keyword search result on the basis of portal id 
	 Unit test 
	 EXEC ZnodeReport_KeywordFiltered 
*/
    BEGIN  
	BEGIN TRY                                    
        SET NOCOUNT ON ;  

        SELECT  Data1 As 'Search_Phrase',
                Count(Data1) As 'Times_Searched'
        FROM    ZNodeActivityLog
        WHERE   ActivityLogTypeId IN ( 9500, 9501, 9502 )
                AND  (CAST([ActivityCreateDate] AS DATE) BETWEEN CASE
                                                                     WHEN @BeginDate IS NULL
                                                                     THEN CAST([ActivityCreateDate] AS DATE)
                                                                     ELSE @BeginDate
                                                                 END AND CASE
                                                                             WHEN @EndDate IS NULL
                                                                             THEN CAST([ActivityCreateDate]AS DATE)
                                                                             ELSE @EndDate
                                                                         END)
               AND EXISTS(SELECT TOP 1 1 FROM dbo.split(@PortalId,',') SP WHERE (PortalId = SP.Item OR @PortalId = '')  )
        GROUP BY DATA1
        ORDER BY 'Times_Searched' DESC

		END TRY
		BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_KeywordFiltered @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_KeywordFiltered',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH
    END