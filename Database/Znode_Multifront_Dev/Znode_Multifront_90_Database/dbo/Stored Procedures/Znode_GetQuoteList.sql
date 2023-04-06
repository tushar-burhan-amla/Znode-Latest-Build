CREATE PROCEDURE [dbo].[Znode_GetQuoteList]
( 
	@WhereClause NVARCHAR(MAx) = '',
	@Rows INT = 100,
    @PageNo INT = 1,
    @Order_BY VARCHAR(1000)  = '',
    @RowsCount INT OUT			,
    @UserId INT = 0,
	@OmsQuoteTypeId int,
	@SalesRepUserId int = 0
)
AS
  /*
     Summary : This procedure is used to get the oms order detils
			   Records are fetched for those users who placed the order i.e UserId is Present in ZnodeUser and  ZnodeOmsOrderDetails tables
	 Unit Testing:

     EXEC Znode_GetQuoteList @Order_BY = 'omsquoteid desc',@RowsCount= 0, @UserId = 0 ,@Rows = 100, @PageNo = 2, @OmsQuoteTypeId=3,@SalesRepUserId=0
*/
BEGIN
SET NOCOUNT ON;
BEGIN TRY
	DECLARE @SQL nvarchar(max) = ''
	DECLARE @Fn_GetPaginationWhereClause VARCHAR(500) = dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
	DECLARE @GetDate DATE = dbo.Fn_GetDate();

	CREATE TABLE #TBL_RowCount(RowsCount INT )

	----Verifying that the @SalesRepUserId is having 'Sales Rep' role
	IF NOT EXISTS
	(
		SELECT * FROM ZnodeUser ZU
		INNER JOIN AspNetZnodeUser ANZU ON ZU.UserName = ANZU.UserName
		INNER JOIN AspNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName
		INNER JOIN AspNetUserRoles ANUR ON ANU.Id = ANUR.UserId
		Where Exists(SELECT * FROM AspNetRoles ANR WHERE Name = 'Sales Rep' AND ANUR.RoleId = ANR.Id) 
		AND ZU.UserId = @SalesRepUserId
	)   
	BEGIN
		SET @SalesRepUserId = 0
	END

	CREATE TABLE #QuoteInfo(OmsQuoteId INT, QuoteNumber VARCHAR(200),UserID INT, CustomerName VARCHAR(300), EmailID VARCHAR(50), PhoneNumber VARCHAR(50), PortalID INT, StoreName VARCHAR(500), QuoteStatus VARCHAR(500),  TotalAmount NUMERIC(28,6),QuoteDate DATETIME,QuoteExpirationDate  DATETIME,CultureCode VARCHAR(100),RowId INT)

	UPDATE ZOQ SET OmsOrderStateId = (SELECT TOP 1 OmsOrderStateId FROM ZnodeOMSOrderState WHERE OrderStateName = 'EXPIRED')
	FROM ZnodeOmsQuote ZOQ
	INNER JOIN ZnodeOmsQuoteType ZOQT ON ZOQ.OmsQuoteTypeId = ZOQT.OmsQuoteTypeId
	INNER JOIN ZnodeOMSOrderState ZOOS ON ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId
	WHERE ZOQ.OmsQuoteTypeId = @OmsQuoteTypeId AND (ZOQ.UserId = @UserId OR @UserId = 0 )
	AND CAST(ZOQ.QuoteExpirationDate AS DATE) < @GetDate
	AND ZOQ.OmsOrderStateId <> (SELECT TOP 1 OmsOrderStateId FROM ZnodeOMSOrderState WHERE OrderStateName = 'EXPIRED')
	
	--For enhancemet purpose getting order by clause column from exact table
	IF @Order_BY <> ''
	BEGIN
		SET @Order_BY = REPLACE(@Order_BY,'OmsQuoteId','ZOQ.OmsQuoteId')
		SET @Order_BY = REPLACE(@Order_BY,'UserID','ZOQ.UserID')
		SET @Order_BY = REPLACE(@Order_BY,'EmailID','ZOQ.Email')
		SET @Order_BY = REPLACE(@Order_BY,'OmsQuoteTypeId','ZOQ.OmsQuoteTypeId')
		SET @Order_BY = REPLACE(@Order_BY,'QuoteNumber','ZOQ.QuoteNumber')
		SET @Order_BY = REPLACE(@Order_BY,'PhoneNumber','ZOQ.PhoneNumber')
		SET @Order_BY = REPLACE(@Order_BY,'PortalID','ZP.PortalID')
		SET @Order_BY = REPLACE(@Order_BY,'StoreName','ZP.StoreName')
		SET @Order_BY = REPLACE(@Order_BY,'QuoteDate','ZOQ.CreatedDate')
		SET @Order_BY = REPLACE(@Order_BY,'QuoteStatus','ZOOS.Description')
		SET @Order_BY = REPLACE(@Order_BY,'UserName','ISNULL(RTRIM(LTRIM(ZOQ.FirstName)),'''')+'' ''+ISNULL(RTRIM(LTRIM(ZOQ.LastName)),'''')')
		SET @Order_BY = REPLACE(@Order_BY,'CustomerName','ISNULL(RTRIM(LTRIM(ZOQ.FirstName)),'''')+'' ''+ISNULL(RTRIM(LTRIM(ZOQ.LastName)),'''')')
		SET @Order_BY = REPLACE(@Order_BY,'TotalAmount','ZOQ.QuoteOrderTotal')
		SET @Order_BY = REPLACE(@Order_BY,'QuoteExpirationDate','ZOQ.QuoteExpirationDate')
		SET @Order_BY = REPLACE(@Order_BY,'CultureCode','ZOQ.CultureCode')
	END
	--For enhancemet purpose getting where clause column from exact table
	IF @WhereClause <> ''
	BEGIN
		SET @WhereClause = REPLACE(@WhereClause,'OmsQuoteId','ZOQ.OmsQuoteId')
		SET @WhereClause = REPLACE(@WhereClause,'OmsQuoteTypeId','ZOQ.OmsQuoteTypeId')
		SET @WhereClause = REPLACE(@WhereClause,'EmailID','ZOQ.Email')
		SET @WhereClause = REPLACE(@WhereClause,'UserID','ZOQ.UserID')
		SET @WhereClause = REPLACE(@WhereClause,'QuoteNumber','ZOQ.QuoteNumber')
		SET @WhereClause = REPLACE(@WhereClause,'PhoneNumber','ZOQ.PhoneNumber')
		SET @WhereClause = REPLACE(@WhereClause,'PortalID','ZP.PortalID')
		SET @WhereClause = REPLACE(@WhereClause,'StoreName','ZP.StoreName')
		SET @WhereClause = REPLACE(@WhereClause,'QuoteDate','ZOQ.CreatedDate')
		SET @WhereClause = REPLACE(@WhereClause,'QuoteStatus','ZOOS.Description')
		SET @WhereClause = REPLACE(@WhereClause,'UserName','ISNULL(RTRIM(LTRIM(ZOQ.FirstName)),'''')+'' ''+ISNULL(RTRIM(LTRIM(ZOQ.LastName)),'''')')
		SET @WhereClause = REPLACE(@WhereClause,'CustomerName','ISNULL(RTRIM(LTRIM(ZOQ.FirstName)),'''')+'' ''+ISNULL(RTRIM(LTRIM(ZOQ.LastName)),'''')')
		SET @WhereClause = REPLACE(@WhereClause,'TotalAmount','ZOQ.QuoteOrderTotal')
		SET @WhereClause = REPLACE(@WhereClause,'QuoteExpirationDate','ZOQ.QuoteExpirationDate')
		SET @WhereClause = REPLACE(@WhereClause,'CultureCode','ZOQ.CultureCode')
		SET @WhereClause = replace(@WhereClause,'ZOQ.CreatedDate','CAST((FORMAT(ZOQ.CreatedDate,''yyyy-MM-dd HH:mm'')) as datetime)')
	END
	--To get the maximum rows to fetch
	SET @Rows = @PageNo * @Rows

	SET @SQL = '
	INSERT INTO #TBL_RowCount
	SELECT COUNT(*)
	FROM ZnodeOmsQuote ZOQ
	INNER JOIN ZnodeOmsQuoteType ZOQT ON ZOQ.OmsQuoteTypeId = ZOQT.OmsQuoteTypeId
	INNER JOIN ZnodePortal ZP ON ZOQ.PortalID = ZP.PortalID
	INNER JOIN ZnodeOMSOrderState ZOOS ON ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId
	WHERE ZOQ.OmsQuoteTypeId = '+CAST(@OmsQuoteTypeId AS VARCHAR(10))+' AND (ZOQ.UserId = '+CAST(@UserId AS VARCHAR(10))+' OR '+CAST(@UserId AS VARCHAR(10))+'= 0 )
	AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = '+CAST(@SalesRepUserId AS VARCHAR(10))+' and ZOQ.UserId = SalRep.CustomerUserid) OR '+CAST(@SalesRepUserId AS VARCHAR(10))+' = 0)
	 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'

	SELECT TOP '+CAST(@Rows AS VARCHAR(10))+' ZOQ.OmsQuoteId, ZOQ.OmsQuoteTypeId,ZOQ.UserID, ZOQ.QuoteNumber AS QuoteNumber,ISNULL(ZOQ.FirstName,'''')+CASE WHEN ZOQ.MiddleName is not null THEN '' '' ELSE '''' END+ ISNULL(ZOQ.MiddleName,'''')+'' ''+isnull(ZOQ.LastName,'''') AS CustomerName,
	ZOQ.Email AS EmailID ,ZOQ.PhoneNumber,ZP.PortalID,ZP.StoreName ,ZOQ.CreatedDate AS QuoteDate,ZOOS.Description AS QuoteStatus,ZOQ.QuoteOrderTotal as TotalAmount, ZOQ.QuoteExpirationDate , ZOQ.CultureCode,
	'+dbo.Fn_GetPagingRowId(@Order_BY,'ZOQ.OmsQuoteId DESC')+' 
	INTO #QuoteDetail
	FROM ZnodeOmsQuote ZOQ
	INNER JOIN ZnodeOmsQuoteType ZOQT ON ZOQ.OmsQuoteTypeId = ZOQT.OmsQuoteTypeId
	INNER JOIN ZnodePortal ZP ON ZOQ.PortalID = ZP.PortalID
	INNER JOIN ZnodeOMSOrderState ZOOS ON ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId
	WHERE ZOQ.OmsQuoteTypeId = '+CAST(@OmsQuoteTypeId AS VARCHAR(10))+' AND (ZOQ.UserId = '+CAST(@UserId AS VARCHAR(10))+' OR '+CAST(@UserId AS VARCHAR(10))+'= 0 )
	AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = '+CAST(@SalesRepUserId AS VARCHAR(10))+' and ZOQ.UserId = SalRep.CustomerUserid) or '+CAST(@SalesRepUserId AS VARCHAR(10))+' = 0)
	 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'

	SELECT OmsQuoteId, QuoteNumber, CustomerName, EmailID, PhoneNumber, PortalID, StoreName, QuoteStatus, QuoteDate,
			   TotalAmount,QuoteExpirationDate, CultureCode , RowId
	FROM #QuoteDetail
	'+@Fn_GetPaginationWhereClause

	INSERT INTO #QuoteInfo(OmsQuoteId, QuoteNumber,  CustomerName, EmailID, PhoneNumber, PortalID, StoreName, QuoteStatus, QuoteDate, TotalAmount, QuoteExpirationDate, CultureCode , RowId)
	EXEC(@SQL)

	SELECT OmsQuoteId, QuoteNumber, CustomerName, EmailID, PhoneNumber, StoreName, QuoteStatus, QuoteDate, TotalAmount, QuoteExpirationDate, CultureCode
	FROM #QuoteInfo
	ORDER BY RowId

	SET @RowsCount = ISNULL((SELECT TOP 1 RowsCount FROM #TBL_RowCount),0)

	IF OBJECT_ID('tempdb..#QuoteDetail') is not null
		DROP TABLE #QuoteDetail
	IF OBJECT_ID('tempdb..#QuoteInfo') is not null
		DROP TABLE #QuoteInfo

END TRY
BEGIN CATCH
    DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetQuoteList @WhereClause = '''+ISNULL(CAST(@WhereClause AS VARCHAR(max)),'''''')+''',@Rows='''+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+''',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',
	@Order_BY='+ISNULL(@Order_BY,'''''')+',@UserId = '+ISNULL(CAST(@UserId AS VARCHAR(50)),'''')+',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''');
              			 
    SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
    EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetQuoteList',
	@ErrorInProcedure = 'Znode_GetQuoteList',
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH;
END

