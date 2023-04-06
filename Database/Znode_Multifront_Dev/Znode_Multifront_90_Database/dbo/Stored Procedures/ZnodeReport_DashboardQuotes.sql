CREATE PROCEDURE [dbo].[ZnodeReport_DashboardQuotes]              
(             
	@PortalId  bigint  = null,        
	@AccountId bigint  = null,    
	@SalesRepUserId int = 0                
)              
AS               
/*              
     Summary:- This procedure is used to get the order details               
    Unit Testing:              
     EXEC [ZnodeReport_DashboardTopCategory]              
*/              
BEGIN              
BEGIN TRY              
SET NOCOUNT ON;              
	DECLARE @GetDate DATE = dbo.Fn_GetDate();
	DECLARE @TopItem TABLE (ItemName nvarchar(100),CustomerName varchar(302),ItemId nvarchar(10), Total numeric(28,6) , Date datetime,Symbol NVARCHAR(10))               
     
	DECLARE @RoundOffValue INT= dbo.Fn_GetDefaultValue('PriceRoundOff')      
        
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
  
	DECLARE @TBL_CultureCurrency TaBLE (Symbol Varchar(100),CurrencyCode varchar(100))              
	INSERT INTO @TBL_CultureCurrency (Symbol,CurrencyCode)              
	SELECT Symbol,CultureCode FROM  ZnodeCulture ZC               
	DECLARE @PortalCurrencySymbol nvarchar(20)          
	DECLARE @DefaultCurrencySymbol nvarchar(20)            
            
	SET @PortalCurrencySymbol = [dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER) )          
	SET @DefaultCurrencySymbol = [dbo].[Fn_GetDefaultCurrencySymbol]()           
          
	IF @PortalCurrencySymbol IS NULL           
	UPDATE @TBL_CultureCurrency SET Symbol  =@DefaultCurrencySymbol WHERE  Symbol IS NULL          
	ELSE           
	UPDATE @TBL_CultureCurrency SET Symbol  =@PortalCurrencySymbol WHERE  Symbol IS NULL          
     
	CREATE TABLE #User(UserId INT, FirstName varchar(100), MiddleName varchar(100), LastName varchar(100), Email varchar(50), PhoneNumber varchar(50))

	IF @AccountId = -1
		INSERT INTO #User(UserId, FirstName, MiddleName, LastName, Email , PhoneNumber )
		SELECT ZU.UserId, ZU.FirstName, ZU.MiddleName, ZU.LastName, ZU.Email , ZU.PhoneNumber                    
		FROM ZnodeUser ZU           
		WHERE EXISTS(SELECT * FROM ZnodeOmsQuote ZOQ where ZU.UserId = ZOQ.UserID )        
		and ISNULL(ZU.AccountId,0) = 0      
	Else
		INSERT INTO #User(UserId, FirstName, MiddleName, LastName, Email , PhoneNumber )
		SELECT ZU.UserId, ZU.FirstName, ZU.MiddleName, ZU.LastName, ZU.Email , ZU.PhoneNumber          
		FROM ZnodeUser ZU           
		WHERE EXISTS(SELECT * FROM ZnodeOmsQuote ZOQ where ZU.UserId = ZOQ.UserID )        
		and (ZU.AccountId = @AccountId or isnull(@AccountId,0) = 0 ) 
          
	Update ZOQ set OmsOrderStateId = (select top 1 OmsOrderStateId from ZnodeOMSOrderState where OrderStateName = 'EXPIRED')          
	from ZnodeOmsQuote ZOQ          
	Inner Join ZnodeOmsQuoteType ZOQT ON ZOQ.OmsQuoteTypeId = ZOQT.OmsQuoteTypeId          
	INNER JOIN #User U ON ZOQ.UserId = U.UserId           
	INNER JOIN ZnodePortal ZP ON ZOQ.PortalID = ZP.PortalID          
	INNER JOIN ZnodeOMSOrderState ZOOS ON ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId          
	where  (ZOQ.PortalID = @PortalId OR @PortalId = 0 OR @PortalId is null)          
	and cast(ZOQ.QuoteExpirationDate as date) < @GetDate         
	and ZOQ.OmsOrderStateId <> (select top 1 OmsOrderStateId from ZnodeOMSOrderState where OrderStateName = 'EXPIRED')          
          
	insert into @TopItem(ItemId, ItemName,CustomerName,Date,Total,Symbol)          
	Select ZOQ.OmsQuoteId,ZOQ.QuoteNumber as QuoteNumber,isnull(U.FirstName,'')+case when U.MiddleName is not null then ' ' else '' end+ isnull(U.MiddleName,'')+' '+isnull(U.LastName,'') as CustomerName,          
	ZOQ.CreatedDate as QuoteDate,round(ZOQ.QuoteOrderTotal,@RoundOffValue) as TotalAmount        
	,COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalId AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol              
	from ZnodeOmsQuote ZOQ          
	Inner Join ZnodeOmsQuoteType ZOQT ON ZOQ.OmsQuoteTypeId = ZOQT.OmsQuoteTypeId          
	INNER JOIN #User U ON ZOQ.UserId = U.UserId           
	INNER JOIN ZnodePortal ZP ON ZOQ.PortalID = ZP.PortalID          
	INNER JOIN ZnodeOMSOrderState ZOOS ON ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId          
	LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZOQ.CultureCode )           
	where  (ZOQ.PortalID = @PortalId OR @PortalId = 0 OR @PortalId is null)          
	and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = @SalesRepUserId and U.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)              
	and ZOQT.QuoteTypeCode = 'QUOTE'
	
	SELECT TOP 5 ItemId, ItemName,CustomerName,Date,Total,Symbol FROM @TopItem Order by  Convert(datetime,Date )  desc                
   
END TRY              
              
BEGIN CATCH              
	DECLARE @Status BIT ;              
		SET @Status = 0;              
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),              
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardQuotes @PortalId = '+CAST(@PortalId AS VARCHAR(50));             
                                
				SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                                  
                  
				EXEC Znode_InsertProcedureErrorLog              
	@ProcedureName = 'ZnodeReport_DashboardQuotes',              
	@ErrorInProcedure = @Error_procedure,              
	@ErrorMessage = @ErrorMessage,              
	@ErrorLine = @ErrorLine,              
	@ErrorCall = @ErrorCall;              
END CATCH              
              
END;
