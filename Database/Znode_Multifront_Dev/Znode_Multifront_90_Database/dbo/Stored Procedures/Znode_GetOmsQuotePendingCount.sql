CREATE PROCEDURE [dbo].[Znode_GetOmsQuotePendingCount]    
(     
	@WhereClause NVARCHAR(MAX),    
	@AccountId INT,    
	@UserId INT = 0,     
	@SalesRepUserId INT = 0    
)    
AS     
   /*    
  Summary :- This procedure is used to get the Quote list of account and Users    
    Fn_GetRecurciveAccounts is used to fetch AccountId and Its recursive ParentId      
    @InnerWhereClause contains AccountId fetched from the Function Fn_GetRecurciveAccounts     
    OrderDetails are fetched from the tables filtered by AccountId Present in @InnerWhereClause    
    OrderDetails are fetched in Descending order of OmsQuoteId    
     Unit Testing     
     
    exec sp_executesql N'Znode_GetOmsQuotePendingCount @WhereClause,@AccountId,@UserId,@SalesRepUserId',N'@WhereClause nvarchar(47),@AccountId int,@UserId int,@SalesRepUserId int',
	@WhereClause=N'(PortalId in(''1'',''4'',''5'',''6'',''9'',''10'',''7'',''8''))',
	@AccountId=0,@UserId=1248,@SalesRepUserId=1248     
    
*/    
BEGIN    
BEGIN TRY 

	SET NOCOUNT ON;    
	DECLARE @SQL NVARCHAR(MAX)= '', 
			@InnerWhereClause VARCHAR(MAX)= '', 
			@ProcessType  varchar(50)='Quote',
			
			@IsPendingPayment BIT = 1  ,     
			@IsParentPendingOrder  BIT = 0;    
			
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

    IF @IsPendingPayment = 1 AND @IsParentPendingOrder = 0
	BEGIN
		SET @InnerWhereClause = ' AND  EXISTS (SELECT TOP 1 1 FROM [dbo].[Fn_GetRecurciveUserId] ('+CAST(@UserId AS VARCHAR(50))+','''+@ProcessType+''') SP WHERE (SP.UserId = ZU.UserId OR SP.UserId IS NULL)  )';   
   
		
		SET @InnerWhereClause = @InnerWhereClause+' AND NOT EXISTS ( SELECT TOP 1 1 FROM ZnodeUserGlobalAttributeValue a     
		INNER JOIN ZnodeUserGlobalAttributeValueLocale b WITH(NOLOCK)  on (b.UserGlobalAttributeValueId = a.UserGlobalAttributeValueId)    
		INNER JOIN ZnodeGlobalAttribute c WITH(NOLOCK) ON (c.GlobalAttributeid = a.GlobalAttributeId )    
		WHERE c.AttributeCOde = ''BillingAccountNumber'' AND a.UserId =  ZU.UserId AND b.AttributeValue = '''' ) AND ZOQ.IsPendingPayment =  1    '  

		SET @InnerWhereClause = @InnerWhereClause + CASE WHEN @AccountId > 0 THEN ' AND ZA.AccountId ='+CAST(@AccountId AS VARCHAR(200)) ELSE '' END   

		SET @SQL = ' 
		;With Cte_GetQuoteDetail AS     
		( 
			SELECT Zu.UserId ,ZOQ.OmsQuoteId,ZU.FirstName + CASE WHEN ZU.LastName IS NULL THEN '''' ELSE '' ''+Zu.LastName END UserName , QuoteOrderTotal , ZOOS.Description [OrderStatus]    
				,ZOQ.CreatedDate,ZA.Name AccountName,ZP.PortalId,Zp.StoreName ,case When ZOQ.IsConvertedToOrder IS NULL THEN 0 ELSE ZOQ.IsConvertedToOrder END IsConvertedToOrder,
				ISNULL(DT.QuoteTypeCode,'''') QuoteTypeCode, ZA.AccountCode
			FROM ZnodeOmsQuote ZOQ WITH(NOLOCK)    
			INNER JOIN ZnodeUser ZU WITH(NOLOCK) ON (ZU.UserId = ZOQ.UserId)    
			INNER JOIN ZnodePortal ZP WITH(NOLOCK) ON ZP.PortalId = Zoq.PortalId    
			LEFT JOIN ZnodeOMSQuoteApproval TYUI WITH(NOLOCK) ON (TYUI.OmsQuoteId = ZOQ.OmsQuoteId AND TYUI.ApproverUserId ='+CAST(@UserId AS VARCHAR(50))+' )   
			LEFT JOIN ZnodeOmsOrderState ZOOS WITH(NOLOCK) ON (ZOOS.OmsOrderStateId = '+CASE WHEN @IsParentPendingOrder = 0 AND EXISTS (SELECT TOP 1 1 FROM ZnodeOMSQuoteApproval OQ WHERE OQ.ApproverUserId = @UserId) THEN 'TYUI.OmsOrderStateId ' ELSE 'ZOQ.OmsOrderStateId' END  +' )     
			LEFT JOIN ZnodeAccount ZA WITH(NOLOCK) ON (ZA.AccountId = ZU.AccountId )    
			INNER JOIN ZnodeOmsQuoteType DT WITH(NOLOCK) ON (DT.OmsQuoteTypeId = ZOQ.OmsQuoteTypeId)    
			WHERE DT.OmsQuoteTypeId <> (select top 1 OmsQuoteTypeId from ZnodeOmsQuoteType WITH(NOLOCK) where QuoteTypeCode = ''QUOTE'')'+' '+@InnerWhereClause+'    
			AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WITH(NOLOCK) WHERE SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and ZOQ.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as varchar(10))+' = 0) 
		)
		SELECT COUNT(*) AS PendingPaymentCount
		FROM Cte_GetQuoteDetail
		WHERE 1=1     
		'+dbo.Fn_GetFilterWhereClause(@WhereClause)
		
		PRINT @SQL
		EXEC (@SQL);
	END

	SET @IsPendingPayment = 0
	SET @IsParentPendingOrder = 1
	SET @SQL = ''

	IF @IsPendingPayment = 0 AND @IsParentPendingOrder = 1
	BEGIN

		IF @UserId <> 0  AND @IsParentPendingOrder   = 1           
        BEGIN    
            SET @InnerWhereClause = ' AND '''+CAST(@UserId AS VARCHAR(max))+''' = ZU.UserId ';    
        END    
		ELSE     
		BEGIN     
		  SET @InnerWhereClause = ''    
		END 

		SET @InnerWhereClause = @InnerWhereClause+' AND ZOQ.IsPendingPayment = 0   ' 

		SET @InnerWhereClause = @InnerWhereClause + CASE WHEN @AccountId > 0 THEN ' AND ZA.AccountId ='+CAST(@AccountId AS VARCHAR(200)) ELSE '' END 

		SET @SQL = ' 
		;With Cte_GetQuoteDetail AS     
		( 
			SELECT Zu.UserId ,ZOQ.OmsQuoteId,ZU.FirstName + CASE WHEN ZU.LastName IS NULL THEN '''' ELSE '' ''+Zu.LastName END UserName , QuoteOrderTotal , ZOOS.Description [OrderStatus]    
				,ZOQ.CreatedDate,ZA.Name AccountName,ZP.PortalId,Zp.StoreName ,case When ZOQ.IsConvertedToOrder IS NULL THEN 0 ELSE ZOQ.IsConvertedToOrder END IsConvertedToOrder,
				ISNULL(DT.QuoteTypeCode,'''') QuoteTypeCode, ZA.AccountCode
			FROM ZnodeOmsQuote ZOQ WITH(NOLOCK)   
			INNER JOIN ZnodeUser ZU WITH(NOLOCK)  ON (ZU.UserId = ZOQ.UserId)    
			INNER JOIN ZnodePortal ZP WITH(NOLOCK) ON ZP.PortalId = Zoq.PortalId    
			LEFT JOIN ZnodeOmsOrderState ZOOS WITH(NOLOCK) ON (ZOOS.OmsOrderStateId = '+CASE WHEN @IsParentPendingOrder = 0 AND EXISTS (SELECT TOP 1 1 FROM ZnodeOMSQuoteApproval OQ WHERE OQ.ApproverUserId = @UserId) THEN 'TYUI.OmsOrderStateId ' ELSE 'ZOQ.OmsOrderStateId' END  +' )     
			LEFT JOIN ZnodeAccount ZA WITH(NOLOCK) ON (ZA.AccountId = ZU.AccountId )    
			INNER JOIN ZnodeOmsQuoteType DT WITH(NOLOCK) ON (DT.OmsQuoteTypeId = ZOQ.OmsQuoteTypeId)    
			WHERE DT.OmsQuoteTypeId <> (select top 1 OmsQuoteTypeId from ZnodeOmsQuoteType WITH(NOLOCK) where QuoteTypeCode = ''QUOTE'')'+' '+@InnerWhereClause+'    
			AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WITH(NOLOCK) WHERE SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and ZOQ.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as varchar(10))+' = 0) 
		)
		SELECT COUNT(*) AS ParentPendingOrderCount
		FROM Cte_GetQuoteDetail
		WHERE 1=1     
		'+dbo.Fn_GetFilterWhereClause(@WhereClause)
		
		PRINT @SQL
		EXEC (@SQL);
	END

END TRY    
BEGIN CATCH    
DECLARE @Status BIT ;    
	SET @Status = 0;    
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOmsQuoteList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max)  )+',@AccountId='+CAST(@AccountId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))
                      
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
	EXEC Znode_InsertProcedureErrorLog    
	@ProcedureName = 'Znode_GetOmsQuotePendingCount',    
	@ErrorInProcedure = @Error_procedure,    
	@ErrorMessage = @ErrorMessage,    
	@ErrorLine = @ErrorLine,    
	@ErrorCall = @ErrorCall;    
END CATCH;    
END