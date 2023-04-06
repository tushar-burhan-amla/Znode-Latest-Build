CREATE Procedure [dbo].[ZnodeReport_GetCaseRequest]
(
	 @BeginDate    DATETIME = NULL 
	,@PortalId     VARCHAR(max) = '' 
	,@CaseStatusId VARCHAR(max) = '' 
	,@Title        VARCHAR(255) = ''
	,@Name		   NVARCHAR(600)= ''
	,@EmailId	   VARCHAR(200) = ''
	,@CompanyName  VARCHAR(100) = ''
	,@PhoneNumber  VARCHAR(20) = ''
	
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
				SELECT CR.CaseRequestId, CR.Title, CR.Description, CR.FirstName + ' ' + CR.LastName AS Name, CR.CompanyName, CR.EmailId, CR.PhoneNumber, CR.CreatedDate, 
							CS.CaseStatusName AS 'StatusName', ZP.StoreName
				FROM ZnodeCaseStatus AS CS INNER JOIN
					 ZnodeCaseRequest AS CR ON CS.CaseStatusId = CR.CaseStatusId INNER JOIN
					 ZnodePortal AS ZP ON ZP.PortalId = CR.PortalId
				WHERE ((EXISTS ( SELECT TOP 1 1 FROM dbo.split(@PortalId,',') SP WHERE  CAST(ZP.PortalId  AS VARCHAR(100)) = SP.Item  OR @PortalId = '')) )
				AND ((EXISTS ( SELECT TOP 1 1 FROM dbo.split(@CaseStatusId,',') SP WHERE  CAST(CS.CaseStatusId  AS VARCHAR(100)) = SP.Item  OR @PortalId = '')) )
				AND ( CR.Title like '%' + @Title + '%' OR @Title = '' ) 
				AND ( CR.FirstName + ' ' + CR.LastName like '%' + @Name + '%' OR @Name = '' ) 
				AND ( CR.CompanyName like '%' + @CompanyName + '%' OR @CompanyName = '' ) 
				AND ( CR.EmailId like '%' + @EmailId + '%' OR @EmailId = '' ) 
				AND ( CR.PhoneNumber like '%' + @PhoneNumber + '%' OR @PhoneNumber = '' ) 
				AND CASE WHEN @BeginDate IS nULL THEN 0 ELSE CR.CreatedDate END >= ISNULL(@BeginDate , 0 )
				
 END TRY 
 BEGIN CATCH 
  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetCaseRequest @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@CaseStatusId='+@CaseStatusId+',@Title='+@Title+',@Name='+@Name+',@EmailId='+@EmailId+',@CompanyName='+@CompanyName+',@PhoneNumber='+@PhoneNumber+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetCaseRequest',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
 END CATCH 
END