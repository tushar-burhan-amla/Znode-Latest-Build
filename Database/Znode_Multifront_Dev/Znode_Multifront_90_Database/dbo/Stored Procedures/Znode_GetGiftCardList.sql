CREATE PROCEDURE [dbo].[Znode_GetGiftCardList]
(
	@WhereClause	NVARCHAR(MAX),
	@Rows			INT            = 100,
	@PageNo			INT            = 1,
	@Order_BY		VARCHAR(1000)  = '',
	@RowsCount		INT  OUT,
	@PortalId		NVARCHAR(MAX),
	@ExpirationDate VARCHAR(100) = '',
	@SalesRepUserId INT = 0 
)
AS
/*
	Summary: This procedure is used to find the GiftCardList of user for portal
	Unit Testing:
	declare @aa int
	EXEC Znode_GetGiftCardList @WhereClause='Userid = 5 ' ,@PortalId ='1,2,3,4,6,7,9,10,1010,1011,1012,1013,1014,1015,1016,1020,1021,1023,1024,1025,1028,1029,1030',  @RowsCount= 0,@ExpirationDate = '2017-04-06'  

	EXEC Znode_GetGiftCardList @WhereClause='' ,@PortalId ='1', @RowsCount= 0,@ExpirationDate = '' ,@SalesRepUserId=2
*/
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @SQL NVARCHAR(MAX);
		DECLARE @TBL_GiftCardList TABLE 
		(StoreName NVARCHAR(MAX),Name NVARCHAR(600), CardNumber NVARCHAR(600),CreatedDate DATETIME,StartDate DATETIME,
			ExpirationDate DATETIME,Amount NUMERIC(28,6),RemainingAmount NUMERIC(28,6),CustomerId INT,
			CustomerName NVARCHAR(512),AccountName NVARCHAR(512),IsActive BIT ,GiftCardId INT,UserId INT,CultureCode VARCHAR(100), 
			RowId INT, CountNo INT, AccountCode nvarchar(100)
		)
	
		----Verifying that the @SalesRepUserId is having 'Sales Rep' role
		IF NOT EXISTS
		(
			SELECT * FROM ZnodeUser ZU
			INNER JOIN AspNetZnodeUser ANZU ON ZU.UserName = ANZU.UserName
			INNER JOIN AspNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName
			INNER JOIN AspNetUserRoles ANUR ON ANU.Id = ANUR.UserId
			Where Exists(select * from AspNetRoles ANR Where Name = 'Sales Rep' AND ANUR.RoleId = ANR.Id) 
			AND ZU.UserId = @SalesRepUserId
		)
		Begin
			SET @SalesRepUserId = 0
		End

		SET @SQL ='  
		  DECLARE @TBL_PortalId TABLE (PortalId INT)  
					  INSERT INTO @TBL_PortalId    
		  SELECT  ITEM  FROM dbo.split( '''+@PortalId+''','','') AS a;   

		  ;WITH CTE_GetGiftCard AS  
		  (  
		  SELECT ZP.StoreName,ZGC.Name,CardNumber,ZGC.CreatedDate,StartDate, ExpirationDate,Amount,RemainingAmount,ZGC.UserId AS CustomerId,  
		  CASE WHEN ZU.FirstName IS NULL THEN '''' ELSE ZU.FirstName END + CASE WHEN ZU.LastName IS NULL  THEN '''' ELSE '' ''+ZU.LastName END as CustomerName,ZA.Name As AccountName,ZGC.IsActive  
		  ,GiftCardId, ZU.UserId,IsReferralCommission,zc.CultureCode AS CurrencyCode, ZA.AccountCode
		  FROM ZnodeGiftCard ZGC   
		  INNER JOIN ZnodePortal ZP ON (ZGC.PortalId = ZP.PortalId)  
		  INNER JOIN ZnodePortalUnit zpu on (zp.PortalId = zpu.PortalId)  
		  LEFT JOIN ZnodeCulture zc on (zc.CultureId = zpu.CultureId)  
		  LEFT JOIN ZnodeUser ZU ON (ZU.UserId = ZGC.UserId)  
		  LEFT JOIN ZnodeAccount ZA ON (ZA.AccountId= ZU.AccountId)
		  LEFT JOIN @TBL_PortalId TP ON (TP.PortalId = ZGC.PortalId)  
		  WHERE ((CONVERT(date,''' +@ExpirationDate+''' ) <= CONVERT(DATE,ZGC.ExpirationDate) OR ZGC.ExpirationDate IS  NULL)  OR '''+@ExpirationDate+''' = '''') AND ZGC.PortalId in ('+@PortalId+') 
		  and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where (SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and ZGC.UserId = SalRep.CustomerUserid)) or ZGC.createdby= ' + cast(@SalesRepUserId as varchar(10)) + ' or '+cast(@SalesRepUserId as varchar(10))+' = 0)
		  )  
		  , CTE_GetGiftCardList AS  
		  (  
		  SELECT StoreName,Name,CardNumber,CreatedDate,StartDate,ExpirationDate,Amount,RemainingAmount,CustomerId,CustomerName,AccountName,IsActive,GiftCardId,UserId,CurrencyCode, AccountCode, 
		  '+dbo.Fn_GetPagingRowId(@Order_BY,'GiftCardId DESC')+',Count(*)Over() CountNo   
		  FROM CTE_GetGiftCard  
		  WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'       
		  )  

		  SELECT StoreName,Name,CardNumber,CreatedDate,StartDate,ExpirationDate,Amount,RemainingAmount,CustomerId,CustomerName,AccountName,IsActive,GiftCardId,UserId,CurrencyCode,RowId,CountNo, AccountCode  
		  FROM CTE_GetGiftCardList
		  '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

		INSERT INTO @TBL_GiftCardList
		EXEC(@SQL)

		SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_GiftCardList ),0)

		SELECT StoreName,Name,CardNumber,CreatedDate,StartDate,ExpirationDate,Amount,RemainingAmount,CustomerId,CustomerName,AccountName,IsActive,GiftCardId,UserId,CultureCode, AccountCode 
		FROM @TBL_GiftCardList
	END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetGiftCardList @WhereClause = '+CAST(@WhereClause AS VARCHAR(MAX))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@PortalId='+@PortalId+',@ExpirationDate='+CAST(@ExpirationDate AS VARCHAR(50))+'@Status='+CAST(@Status AS VARCHAR(10));  

		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetGiftCardList',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END