CREATE Procedure [dbo].[ZnodeDevExpressReport_GetCaseRequest]
(
	 @BeginDate    DATETIME = NULL ,
	--,@PortalId     VARCHAR(max) = '' 
	--,@CaseStatusId VARCHAR(max) = '' 
	--,@Title        VARCHAR(255) = ''
	--,@Name		   NVARCHAR(600)= ''
	--,@EmailId	   VARCHAR(200) = ''
	--,@CompanyName  VARCHAR(100) = ''
	--,@PhoneNumber  VARCHAR(20) = ''
	@WhereClause NVARCHAR(max) = ''
)
AS 
/*
	 Summary :- This procedure is used tot find the case request details 
	 Unit Testing 
	 EXEC ZnodeReport_GetCaseRequest
	*/
BEGIN 
 BEGIN TRY 
  SET NOCOUNT ON ; 
   DECLARE @SQL NVARCHAR(MAX);

			 SET @SQL = '
			  ;WITH CTE_GetCaseRequestDetails AS
			  (
				SELECT CR.CaseRequestId, CR.Title, CR.Description, CR.FirstName + '' '' + CR.LastName AS Name, CR.CompanyName, CR.EmailId, CR.PhoneNumber, CR.CreatedDate, 
							CS.CaseStatusName AS ''StatusName'', ZP.StoreName
				FROM ZnodeCaseStatus AS CS INNER JOIN
					 ZnodeCaseRequest AS CR ON CS.CaseStatusId = CR.CaseStatusId INNER JOIN
					 ZnodePortal AS ZP ON ZP.PortalId = CR.PortalId
				WHERE  
				 CASE WHEN '''+CAST(@BeginDate AS VARCHAR(30))+''' IS nULL THEN 0 ELSE CR.CreatedDate END >= ISNULL('''+CAST(@BeginDate AS VARCHAR(30))+''' , 0 )
				 )

				  , CTE_GetCaseRequest AS
			  (
			     SELECT CaseRequestId,Title,Description,Name,CompanyName,EmailId,PhoneNumber,CreatedDate,StatusName,StoreName
				 FROM CTE_GetCaseRequestDetails
			  )

			  SELECT CaseRequestId,Title,Description,Name,CompanyName,EmailId,PhoneNumber,CreatedDate,StatusName,StoreName
				 FROM CTE_GetCaseRequest
				 '

				 PRINT @SQL
				EXEC(@SQL)
				
 END TRY 
 BEGIN CATCH 
  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetCaseRequest @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetCaseRequest',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
 END CATCH 
END