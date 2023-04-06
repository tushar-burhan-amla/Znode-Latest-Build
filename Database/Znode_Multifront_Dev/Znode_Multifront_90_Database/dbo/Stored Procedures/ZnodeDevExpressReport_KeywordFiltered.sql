CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_KeywordFiltered]
    (
         --@PortalId		VARCHAR(max)  = ''
		@BeginDate		DATE		 
		,@EndDate       DATE       
		,@WhereClause NVARCHAR(max) = '' 
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
		  DECLARE @SQL NVARCHAR(MAX);

			 SET @SQL = '
			   ;WITH CTE_GetKeywordFiltered AS
			  (
        SELECT  Data1 As ''Search_Phrase'',
                Count(Data1) As ''Times_Searched''
        FROM    ZNodeActivityLog
        WHERE   ActivityLogTypeId IN ( 9500, 9501, 9502 )
                AND  CAST([ActivityCreateDate] AS DATE) BETWEEN CASE
                                                                     WHEN '''+CAST(@BeginDate AS VARCHAR(30))+''' IS NULL
                                                                     THEN CAST([ActivityCreateDate] AS DATE)
                                                                     ELSE '''+CAST(@BeginDate AS VARCHAR(30))+'''
                                                                 END AND CASE
                                                                             WHEN '''+CAST(@EndDate AS VARCHAR(30))+''' IS NULL
                                                                             THEN CAST([ActivityCreateDate]AS DATE)
                                                                             ELSE '''+CAST(@EndDate AS VARCHAR(30))+'''
                                                                         END
               
        GROUP BY DATA1
        
		)
		
		,CTE_GetKeywordDetailList AS
			  ( SELECT Search_Phrase,Times_Searched FROM CTE_GetKeywordFiltered
			  WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
			  )

			SELECT Search_Phrase,Times_Searched FROM CTE_GetKeywordDetailList
			ORDER BY ''Times_Searched'' DESC 
		'

				PRINT @SQL
				EXEC(@SQL)

		END TRY
		BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_KeywordFiltered @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_KeywordFiltered',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH
    END