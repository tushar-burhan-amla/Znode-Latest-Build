CREATE PROCEDURE [dbo].[ZnodeReport_DashboardOrders]              
(            
	@PortalId  bigint  = null,        
	@AccountId bigint  = null  ,    
	@SalesRepUserId int = 0              
)              
AS              
/*              
Summary:- This procedure is used to get the order details              
Unit Testing:              
EXEC [ZnodeReport_DashboardOrders]              
*/              
BEGIN              
BEGIN TRY              
SET NOCOUNT ON;              
	DECLARE @TopItem TABLE (ItemName nvarchar(100),CustomerName nvarchar(1000),ItemId nvarchar(10), Total numeric(28,6) , Date datetime,Symbol NVARCHAR(10))              
           
                 
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
		SELECT Zoo.OmsOrderId,Zoo.OrderNumber,ISNULL(RTRIM(LTRIM(ZODD.FirstName)),'')          
		+' '+ISNULL(RTRIM(LTRIM(ZODD.LastName)),'') UserName ,ZODD.OrderDate,round(ZODD.Total,@RoundOffValue),        
		COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalId AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]())          
		FROM ZnodeOmsOrder (nolock) ZOO          
		INNER JOIN ZnodeOmsOrderDetails (nolock) ZODD ON (ZODD.OmsOrderId = ZOO.OmsOrderId AND  ZODD.IsActive = 1)          
		INNER JOIN ZnodePortal (nolock) ZP ON (ZP.PortalId = ZODD.portalId )          
		INNER JOIN ZnodePublishState ZODPS ON (ZODPS.PublishStateId = ZOO.PublishStateId)          
		LEFT JOIN ZnodePaymentType (nolock) ZPS ON (ZPS.PaymentTypeId = ZODD.PaymentTypeId )          
		LEFT JOIN  ZnodeOmsOrderStateShowToCustomer (nolock) ZOSC ON (ZOSC.OmsOrderStateId = ZODD.OmsOrderStateId)          
		LEFT JOIN ZnodeOmsOrderState (nolock) ZOS ON (ZOS.OmsOrderStateId = ZODD.OmsOrderStateId)          
		LEFT JOIN ZnodeOmsPaymentState (nolock) ZOPS ON (ZOPS.OmsPaymentStateId = ZODD.OmsPaymentStateId)          
		LEft JOIN ZnodeUser ZU ON (ZU.UserId = ZODD.UserId)          
		LEFT JOIN [dbo].[View_GetUserDetails]  (nolock) ZVGD ON (ZVGD.UserId = ZODD.CreatedBy )          
		LEFT JOIN [dbo].[View_GetUserDetails]  (nolock) ZVGDI ON (ZVGDI.UserId = ZODD.ModifiedBy)          
		LEFT JOIN ZnodeShipping ZS ON (ZS.ShippingId = ZODD.ShippingId)          
		LEFT OUTER JOIN ZnodePaymentSetting (nolock) ZPSS ON (ZPSS.PaymentSettingId = ZODD.PaymentSettingId)          
		LEFT JOIN ZnodePortalPaymentSetting (nolock) ZPPS ON (ZPPS.PaymentSettingId = ZPSS.PaymentSettingId  AND ZPPS.PortalId = ZODD.PortalId   )          
		LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZODD.CultureCode )        
		WHERE (ZP.PortalId = @PortalId OR  ISNULL(@PortalId,0) = 0)        
		and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)      
		and not Exists(Select * from ZnodeAccount ZA Where isnull(ZU.AccountId,0) = ZA.AccountId)
	End
	Else
	Begin
		INSERT INTO @TopItem(ItemId, ItemName,CustomerName,Date,Total,Symbol)          
		SELECT Zoo.OmsOrderId,Zoo.OrderNumber,ISNULL(RTRIM(LTRIM(ZODD.FirstName)),'')          
		+' '+ISNULL(RTRIM(LTRIM(ZODD.LastName)),'') UserName ,ZODD.OrderDate,round(ZODD.Total,@RoundOffValue),        
		COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalId AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]())          
		FROM ZnodeOmsOrder (nolock) ZOO          
		INNER JOIN ZnodeOmsOrderDetails (nolock) ZODD ON (ZODD.OmsOrderId = ZOO.OmsOrderId AND  ZODD.IsActive = 1)          
		INNER JOIN ZnodePortal (nolock) ZP ON (ZP.PortalId = ZODD.portalId )          
		INNER JOIN ZnodePublishState ZODPS ON (ZODPS.PublishStateId = ZOO.PublishStateId)          
		LEFT JOIN ZnodePaymentType (nolock) ZPS ON (ZPS.PaymentTypeId = ZODD.PaymentTypeId )          
		LEFT JOIN  ZnodeOmsOrderStateShowToCustomer (nolock) ZOSC ON (ZOSC.OmsOrderStateId = ZODD.OmsOrderStateId)          
		LEFT JOIN ZnodeOmsOrderState (nolock) ZOS ON (ZOS.OmsOrderStateId = ZODD.OmsOrderStateId)          
		LEFT JOIN ZnodeOmsPaymentState (nolock) ZOPS ON (ZOPS.OmsPaymentStateId = ZODD.OmsPaymentStateId)          
		LEFT JOIN ZnodeUser ZU ON (ZU.UserId = ZODD.UserId)          
		LEFT JOIN [dbo].[View_GetUserDetails]  (nolock) ZVGD ON (ZVGD.UserId = ZODD.CreatedBy )          
		LEFT JOIN [dbo].[View_GetUserDetails]  (nolock) ZVGDI ON (ZVGDI.UserId = ZODD.ModifiedBy)          
		LEFT JOIN ZnodeShipping ZS ON (ZS.ShippingId = ZODD.ShippingId)          
		LEFT OUTER JOIN ZnodePaymentSetting (nolock) ZPSS ON (ZPSS.PaymentSettingId = ZODD.PaymentSettingId)          
		LEFT JOIN ZnodePortalPaymentSetting (nolock) ZPPS ON (ZPPS.PaymentSettingId = ZPSS.PaymentSettingId  AND ZPPS.PortalId = ZODD.PortalId   )          
		LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZODD.CultureCode )        
		WHERE (ZP.PortalId = @PortalId OR  ISNULL(@PortalId,0) = 0) AND (ZU.AccountId = @AccountId OR  ISNULL(@AccountId,0) = 0)        
		and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)      
	End

	SELECT TOP 5 ItemId, ItemName,CustomerName,Date,Total,Symbol FROM @TopItem Order by  Convert(datetime,Date )  desc                
   
END TRY              
             
BEGIN CATCH              
	DECLARE @Status BIT ;        
	SET @Status = 0;              
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),              
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardOrders @PortalId = '+@PortalId;            
                               
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                                  
                 
	EXEC Znode_InsertProcedureErrorLog              
	@ProcedureName = 'ZnodeReport_DashboardOrders',              
	@ErrorInProcedure = @Error_procedure,              
	@ErrorMessage = @ErrorMessage,              
	@ErrorLine = @ErrorLine,              
	@ErrorCall = @ErrorCall;              
END CATCH              
             
END;