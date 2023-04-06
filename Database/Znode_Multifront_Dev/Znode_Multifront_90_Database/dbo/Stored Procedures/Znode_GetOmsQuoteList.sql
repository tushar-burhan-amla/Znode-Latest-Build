CREATE PROCEDURE [dbo].[Znode_GetOmsQuoteList]    
(     
  @WhereClause NVARCHAR(MAX),    
  @Rows        INT            = 100,    
  @PageNo      INT            = 1  ,    
  @Order_BY    VARCHAR(1000)  = '' ,    
  @RowsCount   INT OUT             ,    
  @AccountId   INT,    
  @UserId      INT            = 0,     
  @IsPendingPayment BIT = 0  ,     
  @IsParentPendingOrder  BIT = 1,
  @SalesRepUserId int = 0    
 )    
AS     
   /*    
  Summary :- This procedure is used to get the Quote list of account and Users    
    Fn_GetRecurciveAccounts is used to fetch AccountId and Its recursive ParentId      
    @InnerWhereClause contains AccountId fetched from the Function Fn_GetRecurciveAccounts     
    OrderDetails are fetched from the tables filtered by AccountId Present in @InnerWhereClause    
    OrderDetails are fetched in Descending order of OmsQuoteId    
     Unit Testing     
     
     EXEC Znode_GetOmsQuoteList '' ,@RowsCount = 50 ,@AccountId = 1,@UserId = 0      
    
*/    
     BEGIN    
         BEGIN TRY    
			SET NOCOUNT ON;    
			DECLARE @SQL VARCHAR(MAX)= '', @InnerWhereClause VARCHAR(MAX)= '', @ProcessType  varchar(50)='Quote',@QuoteFilter NVARCHAr(max)='';    
			
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

            DECLARE @TBL_QuoteDetails TABLE (OmsQuoteId INT,UserName NVARCHAR(300),AccountName NVARCHAR(400),QuoteOrderTotal NUMERIC(28, 6),[OrderStatus] VARCHAR(300),    
            CreatedDate DATETIME,StoreName NVARCHAR(Max),CurrencyCode VARCHAR(100),CultureCode VARCHAR(100),PublishState nvarchar(600),RowId INT,CountNo INT,CreatedByName NVARCHAr(max) ,ModifiedByName NVARCHAR(max),IsConvertedToOrder bit,OrderType varchar(50), UserId INT, AccountCode nvarchar(100));    
           
             IF @UserId <> 0  AND @IsParentPendingOrder   = 1           
                 BEGIN    
                     SET @InnerWhereClause = ' AND '''+CAST(@UserId AS VARCHAR(max))+''' = ZU.UserId ';    
                    -- SET @AccountId = (SELECT TOP 1 AccountID FROM ZnodeUser WHERE UserId = @UserId);    
                 END    
             ELSE IF @IsParentPendingOrder   = 0     
                BEGIN    
				SET @InnerWhereClause = ' AND  EXISTS (SELECT TOP 1 1 FROM [dbo].[Fn_GetRecurciveUserId] ('+CAST(@UserId AS VARCHAR(50))+','''+@ProcessType+''') SP WHERE (SP.UserId = ZU.UserId OR SP.UserId IS NULL)  )';   
   
				SET @QuoteFilter =' AND EXISTS (SELECT TOP 1 1 FROM ZnodeOMSQuoteApproval WR WHERE WR.OmsQuoteId = ZOQ.OmsQuoteId AND ( Wr.ApproverUserId ='+CAST(@UserId AS VARCHAR(50))+' OR Wr.UserId = '+CAST(@UserId AS VARCHAR(50))+'  ) ) ';        
				END    
    ELSE     
    BEGIN     
      SET @InnerWhereClause = ''    
    END       
          
    IF @IsPendingPayment =1     
    BEGIN     
       
     SET @InnerWhereClause = @InnerWhereClause+' AND NOT EXISTS ( SELECT TOP 1 1 FROM ZnodeUserGlobalAttributeValue a     
    INNER JOIN ZnodeUserGlobalAttributeValueLocale b  on (b.UserGlobalAttributeValueId = a.UserGlobalAttributeValueId)    
    INNER JOIN ZnodeGlobalAttribute c ON (c.GlobalAttributeid = a.GlobalAttributeId )    
    WHERE c.AttributeCOde = ''BillingAccountNumber'' AND a.UserId =  ZU.UserId AND b.AttributeValue = '''' ) AND ZOQ.IsPendingPayment =  1    '    
         
    END     
    ELSE     
    BEGIN    
       SET @InnerWhereClause = @InnerWhereClause+' AND ZOQ.IsPendingPayment = 0   '    
    END     
    
    SET @InnerWhereClause = @InnerWhereClause + CASE WHEN @AccountId > 0 THEN 
		' AND CASE WHEN ISNULL(ZOQ.AccountId,0)<>0 THEN ZOQ.AccountId ELSE ZA.AccountId END ='+CAST(@AccountId AS VARCHAR(200)) ELSE '' END     
    
    SET @SQL = '       
		;With Cte_GetQuoteDetail AS     
		(    
			SELECT distinct Zu.UserId ,ZOQ.OmsQuoteId,ZU.FirstName + CASE WHEN ZU.LastName IS NULL THEN '''' ELSE '' ''+Zu.LastName END UserName , QuoteOrderTotal , ZOOS.Description [OrderStatus]    
			,ZOQ.CreatedDate,ZA.Name AccountName,ZP.PortalId,Zp.StoreName , ZCC.CurrencyCode AS CurrencyCode, ZC.CultureCode AS CultureCode ,ZVGD.UserName CreatedByName , ZVGDI.UserName ModifiedByName,    
			case When ZOQ.IsConvertedToOrder IS NULL THEN 0 ELSE ZOQ.IsConvertedToOrder END IsConvertedToOrder,ISNULL(DT.QuoteTypeCode,'''') QuoteTypeCode,ZODPS.DisplayName as PublishState,
			'+case when cast(@IsParentPendingOrder as varchar(10)) = 0  then +'Case  When TYUI.ApproverUserId ='+CAST(@UserId AS VARCHAR(50))  + ' then ''Approval Requested'' 
			else ''Pending For Approval'' END' else '''''' end +' OrderType, ZA.AccountCode    
			FROM ZnodeOmsQuote ZOQ    
			INNER JOIN ZnodeUser ZU ON (ZU.UserId = ZOQ.UserId)    
			LEFT JOIN ZnodePublishState ZODPS ON (ZODPS.PublishStateId = ZOQ.PublishStateId)  
			LEFT JOIN ZnodeUserPortal ZUP ON ZU.UserId = ZUP.UserId    
			inner JOIN ZnodePortal ZP ON ZP.PortalId = Zoq.PortalId    
			'+CASE WHEN @IsParentPendingOrder = 0  THEN ' LEFT JOIN ZnodeOMSQuoteApproval TYUI ON (TYUI.OmsQuoteId = ZOQ.OmsQuoteId AND TYUI.ApproverUserId ='+CAST(@UserId AS VARCHAR(50))+' ) ' ELSE '' END +'    
			LEFT JOIN ZnodePortalUnit ZPU ON (ZPU.PortalId = Zp.PortalId)    
			LEFT JOIN ZnodeCulture ZC ON (ZPU.CultureId = ZC.CultureId)    --- Changed join condition from CurrencyId to CultureId    
			LEFT JOIN ZnodeCurrency ZCC ON (ZC.CurrencyId = ZCC.CurrencyId)    --- Joined ZnodeCulture and ZnodeCurrency   
			LEFT JOIN ZnodeOmsOrderState ZOOS ON (ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId)     
			LEFT JOIN ZnodeAccount ZA ON (ZA.AccountId = (CASE WHEN ISNULL(ZOQ.AccountId,0)<>0 THEN ZOQ.AccountId ELSE ZU.AccountId END))    
			LEFT JOIN [dbo].[View_GetUserDetails]  ZVGD ON (ZVGD.UserId = ZOQ.CreatedBy )    
			LEFT JOIN [dbo].[View_GetUserDetails]  ZVGDI ON (ZVGDI.UserId = ZOQ.ModifiedBy)    
			INNER JOIN ZnodeOmsQuoteType DT ON (DT.OmsQuoteTypeId = ZOQ.OmsQuoteTypeId)    
			WHERE DT.OmsQuoteTypeId <> (select top 1 OmsQuoteTypeId from ZnodeOmsQuoteType where QuoteTypeCode = ''QUOTE'')'+' '+@InnerWhereClause+@QuoteFilter+'    
			and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and ZOQ.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as varchar(10))+' = 0)
			UNION 
			SELECT Zu.UserId ,ZOQ.OmsQuoteId,ZU.FirstName + CASE WHEN ZU.LastName IS NULL THEN '''' ELSE '' ''+Zu.LastName END UserName , QuoteOrderTotal , ZOOS.Description [OrderStatus]    
			,ZOQ.CreatedDate,ZA.Name AccountName,ZP.PortalId,Zp.StoreName , ZCC.CurrencyCode AS CurrencyCode, ZC.CultureCode AS CultureCode ,ZVGD.UserName CreatedByName , ZVGDI.UserName ModifiedByName,    
			case When ZOQ.IsConvertedToOrder IS NULL THEN 0 ELSE ZOQ.IsConvertedToOrder END IsConvertedToOrder,ISNULL(DT.QuoteTypeCode,'''') QuoteTypeCode,ZODPS.DisplayName as PublishState,
			'+case when cast(@IsParentPendingOrder as varchar(10)) = 0  then +'Case  When TYUI.ApproverUserId ='+CAST(@UserId AS VARCHAR(50))  + ' then ''Approval Requested'' 
			else ''Pending For Approval'' END' else '''''' end +' OrderType, ZA.AccountCode    
			FROM ZnodeOmsQuote ZOQ    
			INNER JOIN ZnodeUser ZU ON (ZU.UserId = ZOQ.UserId)    
			LEFT JOIN ZnodePublishState ZODPS ON (ZODPS.PublishStateId = ZOQ.PublishStateId)  
			LEFT JOIN ZnodeUserPortal ZUP ON ZU.UserId = ZUP.UserId    
			inner JOIN ZnodePortal ZP ON ZP.PortalId = Zoq.PortalId    
			'+CASE WHEN @IsParentPendingOrder = 0  THEN ' LEFT JOIN ZnodeOMSQuoteApproval TYUI ON (TYUI.OmsQuoteId = ZOQ.OmsQuoteId AND TYUI.ApproverUserId ='+CAST(@UserId AS VARCHAR(50))+' ) ' ELSE '' END +'    
			LEFT JOIN ZnodePortalUnit ZPU ON (ZPU.PortalId = Zp.PortalId)    
			LEFT JOIN ZnodeCulture ZC ON (ZPU.CultureId = ZC.CultureId)    --- Changed join condition from CurrencyId to CultureId    
			LEFT JOIN ZnodeCurrency ZCC ON (ZC.CurrencyId = ZCC.CurrencyId)    --- Joined ZnodeCulture and ZnodeCurrency   
			LEFT JOIN ZnodeOmsOrderState ZOOS ON (ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId)     
			LEFT JOIN ZnodeAccount ZA ON (ZA.AccountId = (CASE WHEN ISNULL(ZOQ.AccountId,0)<>0 THEN ZOQ.AccountId ELSE ZU.AccountId END))    
			LEFT JOIN [dbo].[View_GetUserDetails]  ZVGD ON (ZVGD.UserId = ZOQ.CreatedBy )    
			LEFT JOIN [dbo].[View_GetUserDetails]  ZVGDI ON (ZVGDI.UserId = ZOQ.ModifiedBy)    
			INNER JOIN ZnodeOmsQuoteType DT ON (DT.OmsQuoteTypeId = ZOQ.OmsQuoteTypeId)    
			WHERE  DT.OmsQuoteTypeId <> (select top 1 OmsQuoteTypeId from ZnodeOmsQuoteType where QuoteTypeCode = ''QUOTE'')'+
			'AND DT.OmsQuoteTypeId = (select top 1 OmsQuoteTypeId from ZnodeOmsQuoteType where QuoteTypeCode = ''OAB'')'+' '+@InnerWhereClause+'    
			and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and ZOQ.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as varchar(10))+' = 0)
		)    
		, Cte_GetQuote AS     
		(    
			SELECT distinct OmsQuoteId,UserName ,AccountName , QuoteOrderTotal QuoteAmount, [OrderStatus]  ,CreatedDate ,StoreName,CurrencyCode, CultureCode,PublishState,CreatedByName , ModifiedByName ,IsConvertedToOrder,OrderType,'+dbo.Fn_GetPagingRowId(@Order_BY,'CreatedDate DESC 
			,OmsQuoteId DESC')+',Count(*)Over() CountNo ,UserId, AccountCode      
			FROM Cte_GetQuoteDetail    
			WHERE 1=1     
			'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'    
		)    
		SELECT distinct OmsQuoteId,UserName ,AccountName ,  QuoteAmount, [OrderStatus]  ,CreatedDate ,StoreName,CurrencyCode, CultureCode,PublishState,RowId,CountNo,CreatedByName , ModifiedByName,IsConvertedToOrder,OrderType, UserId , AccountCode   
		FROM Cte_GetQuote     
		'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows) 
	
      print @SQL
        INSERT INTO @TBL_QuoteDetails (OmsQuoteId, UserName, AccountName, QuoteOrderTotal ,OrderStatus, CreatedDate, StoreName,CurrencyCode, CultureCode,PublishState, RowId ,CountNo,CreatedByName , ModifiedByName,IsConvertedToOrder,OrderType, UserId, AccountCode)          
        EXEC (@SQL);   	
        SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_QuoteDetails), 0);    

        SELECT  OmsQuoteId,UserName,AccountName,QuoteOrderTotal,[OrderStatus],CreatedDate,StoreName,CurrencyCode, CultureCode,PublishState,CreatedByName , ModifiedByName,IsConvertedToOrder  ,OrderType , UserId, AccountCode 
        FROM @TBL_QuoteDetails 
		ORDER BY  RowId
		      
         END TRY    
         BEGIN CATCH    
		DECLARE @Status BIT ;    
		SET @Status = 0;    
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOmsQuoteList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max)  
		)+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@AccountId='+CAST(@AccountId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@PortalId='+''  
		+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
		EXEC Znode_InsertProcedureErrorLog    
		@ProcedureName = 'Znode_GetOmsQuoteList',    
		@ErrorInProcedure = @Error_procedure,    
		@ErrorMessage = @ErrorMessage,    
		@ErrorLine = @ErrorLine,    
		@ErrorCall = @ErrorCall;    
    END CATCH;    
END