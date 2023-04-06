CREATE PROCEDURE [dbo].[Znode_GetCaseRequest]
( @WhereClause NVarchar(Max)   = '',
  @Rows          INT           = 100,
  @PageNo        INT           = 1,
  @Order_BY      VARCHAR(1000) = '',
  @RowsCount     INT OUT,
  @IsCaseHistory BIT           = 0)
AS
/*
   
    Summary : Get Case request with their details i.e. priority , status and type 
    SELECT * FROM Znode
    SELECT * FROM ASpNetZnodeUser 
    Unit Testing 
    DECLARE @RowsCount INT;
    EXEC Znode_GetCaseRequest @WhereClause = '',@Rows = 50,@PageNo = 1 ,@Order_BY = NULL,@RowsCount = 0,@IsCaseHistory = 0 
    
  */
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
		     DECLARE @TBL_CaseRequest TABLE(CaseRequestId INT,PortalId INT,UserId INT,OwnerUserId INT,CaseStatusId INT,CaseStatusName VARCHAR(100),CasePriorityId INT
											 ,CasePriorityName VARCHAR(100),CaseTypeId INT,CaseTypeName VARCHAR(100),CaseOrigin VARCHAR(50),Title VARCHAR(255),Description NVARCHAR(MAX)
											 ,FirstName VARCHAR(100),LastName VARCHAR(100),CompanyName VARCHAR(100),EmailId VARCHAR(200),PhoneNumber VARCHAR(20),CreatedBy INT,CreatedDate DATETIME
											 ,ModifiedBy INT,ModifiedDate DATETIME,StoreName NVARCHAR(MAX),FullName VARCHAR(100),RowId INT,CountNo INT ,CaseRequestHistoryId INT ,UserName NVARCHAR(512),EmailSubject VARCHAR(300),EmailMessage NVARCHAR(max))
             DECLARE @SQL NVARCHAR(MAX);
			 
             SET @SQL = '  
			 
			        ;with Cte_GetcaseRequest AS 
					(
			   
							 Select ZCR.CaseRequestId,ZCR.PortalId,ZCR.UserId,ZCR.OwnerUserId,ZCR.CaseStatusId,ZCS.CaseStatusName
									 ,ZCR.CasePriorityId,ZCP.CasePriorityName,ZCR.CaseTypeId,ZCT.CaseTypeName ,ZCR.CaseOrigin,ZCR.Title
									 ,ZCR.Description,ZCR.FirstName,ZCR.LastName,ZCR.CompanyName,ZCR.EmailId,ZCR.PhoneNumber,ZCR.CreatedBy
									 ,ZCR.CreatedDate,ZCR.ModifiedBy,ZCR.ModifiedDate,zp.StoreName
									 ,CASE WHEN ZCR.FirstName IS NULL THEN '''' ELSE ZCR.FirstName END   
										+CASE WHEN ZCR.LastName IS NULL THEN '''' ELSE '' ''+ZCR.LastName END  FullName , NULL CaseRequestHistoryId,NULL UserName
										,NULL EmailSubject, NULL EmailMessage	
 							 FROM ZnodeCaseRequest ZCR 
							 INNER JOIN ZnodeCasePriority ZCP ON ZCR.CasePriorityId = ZCP.CasePriorityId 
							 INNER JOIN ZnodeCaseStatus ZCS ON ZCR.CaseStatusId = ZCS.CaseStatusId 
							 INNER JOIN ZnodeCaseType ZCT ON ZCR.CaseTypeId = ZCT.CaseTypeId
							 LEFT JOIN ZnodePortal zp ON (zp.PortalId = zcr.portalId)

					) ';
             IF @IsCaseHistory = 1
                 BEGIN
                     SET @SQL = '
				;With Cte_GetcaseRequest AS 
				(
							 SELECT ZCR.CaseRequestId ,ZCRH.CaseRequestHistoryId, CASE WHEN ZCR.FirstName IS NULL THEN '''' ELSE ZCR.FirstName END   
										+CASE WHEN ZCR.LastName IS NULL THEN '''' ELSE '' ''+ZCR.LastName END  FullName ,APZU.UserName
										,ZCRH.EmailSubject,ZCRH.EmailMessage ,ZCRH.CreatedDate,zp.PortalId,ZU.UserId
										,ZCR.OwnerUserId,ZCR.CaseStatusId, ZCS.CaseStatusName,ZCP.CasePriorityId ,ZCP.CasePriorityName
										,ZCR.CaseTypeId ,ZCT.caseTypeName ,ZCR.CaseOrigin ,Zcr.Title ,ZCR.Description ,ZCR.FirstName ,ZCR.LastName ,ZCR.CompanyName 
										,ZCR.EmailId ,ZCR.PhoneNumber ,ZCR.CreatedBy ,ZCR.ModifiedBy ,ZCR.ModifiedDate , ZP.StoreName
							 FROM ZnodeCaseRequest ZCR  
							 INNER JOIN ZnodeCaseRequestHistory ZCRH ON ZCR.CaseRequestId = ZCRH.CaseRequestId
							 LEFT JOIN ZnodeCasePriority ZCP ON ZCR.CasePriorityId = ZCP.CasePriorityId 
							 LEFT JOIN ZnodeCaseType ZCT ON ZCR.CaseTypeId = ZCT.CaseTypeId
							 LEFT JOIN ZnodeCaseStatus ZCS ON ZCR.CaseStatusId = ZCS.CaseStatusId 
							 LEFT JOIN ZnodePortal zp ON (zp.PortalId = zcr.portalId)
							 LEFT JOIN ZnodeUser ZU ON (ZU.UserId = ZCR.UserId)
							 LEFT JOIN AspNetUsers APNU ON (APNU.Id = ZU.AspNetUserId)
							 LEFT JOIN ASpNetZnodeUser APZU ON (APZU.AspNetZnodeUserId = APNU.UserName AND (APZU.PortalId = ZCR.PortalId OR APZU.PortalId  IS NULL ) )
				)
							';
                 END;
             SET @SQL = @SQL+'	
			      
				             , CTE_CaseRequest AS
							 (
							  SELECT CaseRequestId,PortalId,UserId,OwnerUserId,CaseStatusId,CaseStatusName,CasePriorityId ,CasePriorityName,CaseTypeId,CaseTypeName,CaseOrigin,Title,Description ,FirstName ,LastName,CompanyName ,EmailId ,PhoneNumber ,CreatedBy ,CreatedDate,ModifiedBy ,ModifiedDate ,StoreName ,FullName 							 
													,'+dbo.Fn_GetPagingRowId(@Order_BY,'CaseRequestId DESC')+',Count(*)Over() CountNo,CaseRequestHistoryId,UserName
										,EmailSubject,EmailMessage
							  FROM Cte_GetcaseRequest
							  where  1=1
													 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
							 )
			 
							  SELECT CaseRequestId,PortalId,UserId,OwnerUserId,CaseStatusId,CaseStatusName,CasePriorityId ,CasePriorityName,CaseTypeId,CaseTypeName,CaseOrigin,Title,Description ,FirstName ,LastName,CompanyName ,EmailId ,PhoneNumber ,CreatedBy ,CreatedDate,ModifiedBy ,ModifiedDate ,StoreName ,FullName ,RowId ,CountNo,CaseRequestHistoryId,UserName
										,EmailSubject,EmailMessage							 
							  FROM CTE_CaseRequest
													 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
			 
			
			 INSERT INTO @TBL_CaseRequest(CaseRequestId,PortalId,UserId,OwnerUserId,CaseStatusId,CaseStatusName,CasePriorityId ,CasePriorityName,CaseTypeId,CaseTypeName,CaseOrigin,Title,Description ,FirstName ,LastName,CompanyName ,EmailId ,PhoneNumber ,CreatedBy ,CreatedDate,ModifiedBy ,ModifiedDate ,StoreName ,FullName ,RowId ,CountNo ,CaseRequestHistoryId,UserName
										,EmailSubject,EmailMessage)																						 											 
			 EXEC(@SQL)

			 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_CaseRequest),0)
			 
			 SELECT CaseRequestId,PortalId,UserId,OwnerUserId,CaseStatusId,CaseStatusName,CasePriorityId,CasePriorityName,CaseTypeId,CaseTypeName,CaseOrigin,Title
					,Description ,FirstName,LastName,CompanyName,EmailId,PhoneNumber,CreatedBy,CreatedDate,ModifiedBy ,ModifiedDate ,StoreName ,FullName	,CaseRequestHistoryId,UserName
										,EmailSubject,EmailMessage																					  
			 FROM @TBL_CaseRequest
			 									 			 			 
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCaseRequest @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@IsCaseHistory = '+CAST(@IsCaseHistory AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCaseRequest',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;