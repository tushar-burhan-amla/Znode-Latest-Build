CREATE PROCEDURE [dbo].[Znode_GetSalesRepAccessConfirmation]
(
	@SalesRepUserId int,
	@EntityType Varchar(100),
	@EntityId int,
	@Status bit = 0 Out,
	@ReturnNo Varchar(100) = ''
)
--Exec Znode_GetSalesRepAccessConfirmation @SalesRepUserId=2,@EntityType='User',@EntityId=48
As
Begin
Begin Try
	SET NOCOUNT ON;
	SET @Status = 0
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
		SET @Status = 1
		SELECT 1 AS ID,@Status AS Status
		return
	End

	IF @EntityType = 'QUOTE'
	Begin
		SET @Status = (SELECT top 1 1 FROM ZnodeOmsQuote ZO with (nolock)
			where exists(select * from ZnodeSalesRepCustomerUserPortal Z Where Z.SalesRepUserId = @SalesRepUserId AND ZO.UserId = Z.CustomerUserid)
			and ZO.OmsQuoteId = @EntityId )
	End
	Else IF @EntityType = 'Order'
	Begin
		SET @Status = (SELECT top 1 1 FROM ZnodeOmsOrderDetails ZO with (nolock)
			where exists(select * from ZnodeSalesRepCustomerUserPortal Z Where Z.SalesRepUserId = @SalesRepUserId AND ZO.UserId = Z.CustomerUserid)
			and ZO.OmsOrderId = @EntityId )
	End
	Else IF @EntityType = 'Return'
	Begin
		SET @Status = (SELECT top 1 1 FROM ZnodeRmaReturnDetails ZO with (nolock)
			where exists(select * from ZnodeSalesRepCustomerUserPortal Z Where Z.SalesRepUserId = @SalesRepUserId AND ZO.UserId = Z.CustomerUserid)
			and ZO.ReturnNumber = @ReturnNo )
	End
	Else IF @EntityType = 'Voucher'
	Begin
		SET @Status = (SELECT top 1 1 FROM ZnodeGiftCard ZO with (nolock)
			where (exists(select * from ZnodeSalesRepCustomerUserPortal Z Where (Z.SalesRepUserId = @SalesRepUserId  AND ZO.UserId = Z.CustomerUserid)) or (@SalesRepUserId =ZO.createdby))
			and ZO.GiftCardId = @EntityId )
	End
	Else IF @EntityType = 'User'
	Begin
		SET @Status = (SELECT top 1 1 FROM ZnodeUser ZO with (nolock)
			where exists(select * from ZnodeSalesRepCustomerUserPortal Z Where Z.SalesRepUserId = @SalesRepUserId AND ZO.UserId = Z.CustomerUserid)
			and ZO.UserId = @EntityId )
	End 
	Else IF @EntityType = 'Pending Payment'
	Begin
		SET @Status = (SELECT top 1 1 FROM ZnodeOmsQuote ZOQ with (nolock)
			where exists(select * from ZnodeSalesRepCustomerUserPortal Z Where Z.SalesRepUserId = @SalesRepUserId AND ZOQ.UserId = Z.CustomerUserid)
			and ZOQ.UserId = @EntityId )
	End 
	Set @Status = CAST(ISNULL(@Status,0) as bit)
	SELECT 1 AS ID,@Status AS Status
End Try              
              
Begin Catch        
  
    SET @Status = 0;              
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),              
    @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSalesRepAccessConfirmation @PortalId = '+cast(@SalesRepUserId as varchar(10));             
                                
    SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                                  
                  
    EXEC Znode_InsertProcedureErrorLog              
    @ProcedureName = 'Znode_GetSalesRepAccessConfirmation',              
    @ErrorInProcedure = @Error_procedure,              
    @ErrorMessage = @ErrorMessage,              
    @ErrorLine = @ErrorLine,              
    @ErrorCall = @ErrorCall;              
End Catch
End
