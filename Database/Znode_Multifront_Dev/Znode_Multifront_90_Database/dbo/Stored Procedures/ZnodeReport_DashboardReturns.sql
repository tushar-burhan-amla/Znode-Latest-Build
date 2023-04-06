CREATE PROCEDURE [dbo].[ZnodeReport_DashboardReturns]              
(             
	 @PortalId  bigint  = null,        
	 @AccountId bigint  = null  ,    
	 @SalesRepUserId int = 0              
)              
AS               
/*              
     Summary:- This procedure is used to get the order details               
    Unit Testing:              
     EXEC [ZnodeReport_DashboardReturns]              
*/              
BEGIN              
BEGIN TRY              
  SET NOCOUNT ON;              
  DECLARE @TopItem TABLE (ItemName nvarchar(100),CustomerName nvarchar(100),ItemId nvarchar(10), Total numeric(28,6) , Date datetime,Symbol NVARCHAR(10))               
            
                 
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
              
   IF @AccountId = -1
   Begin
		INSERT INTO @TopItem(ItemId, ItemName,CustomerName,Date,Total,Symbol)                
		select ZRRD.RmaReturnDetailsId,ZRRD.ReturnNumber, isnull(ZU.FirstNAme,'''')+' '+isnull(ZU.LastName,'') as UserName,ZRRD.ReturnDate,          
		round(ZRRD.TotalReturnAmount,@RoundOffValue) TotalReturnAmount,          
		COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalId AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]())               
		from ZnodeRmaReturnDetails ZRRD          
		inner join ZnodeUser ZU ON ZRRD.UserId = ZU.UserId          
		inner join ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId           
		inner join ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId          
		LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZRRD.CultureCode )         
		where isnull(ZRRD.RmaReturnStateId,0) not in (select isnull(RmaReturnStateId,0) from ZnodeRmaReturnState where ReturnStateName = 'Not Submitted')          
		AND (ZRRD.PortalId = @PortalId OR  isnull(@PortalId,0)= 0)            
		and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)       
		and IsNull(ZU.AccountId,0) = 0
   End
   Else
   Begin
	   INSERT INTO @TopItem(ItemId, ItemName,CustomerName,Date,Total,Symbol)  
	   select ZRRD.RmaReturnDetailsId,ZRRD.ReturnNumber, isnull(ZU.FirstNAme,'''')+' '+isnull(ZU.LastName,'') as UserName,ZRRD.ReturnDate,          
		round(ZRRD.TotalReturnAmount,@RoundOffValue) TotalReturnAmount,          
		COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalId AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]())               
	   from ZnodeRmaReturnDetails ZRRD          
	   inner join ZnodeUser ZU ON ZRRD.UserId = ZU.UserId          
	   inner join ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId           
	   inner join ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId          
	   LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZRRD.CultureCode )         
	   where isnull(ZRRD.RmaReturnStateId,0) not in (select isnull(RmaReturnStateId,0) from ZnodeRmaReturnState where ReturnStateName = 'Not Submitted')          
	   AND (ZRRD.PortalId = @PortalId OR  isnull(@PortalId,0)= 0) AND (ZU.AccountId = @AccountId OR  ISNULL(@AccountId,0) = 0)            
	   and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)       
   End

   SELECT TOP 5 ItemId, ItemName,CustomerName,Date,Total,Symbol FROM @TopItem Order by  Convert(numeric,Total )  desc                
   
END TRY              
              
BEGIN CATCH              
	DECLARE @Status BIT ;              
		SET @Status = 0;              
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),              
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardReturns @PortalId = '+@PortalId;             
                                
				SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                                  
                  
				EXEC Znode_InsertProcedureErrorLog              
	@ProcedureName = 'ZnodeReport_DashboardReturns',              
	@ErrorInProcedure = @Error_procedure,              
	@ErrorMessage = @ErrorMessage,              
	@ErrorLine = @ErrorLine,              
	@ErrorCall = @ErrorCall;              
END CATCH              
              
END;