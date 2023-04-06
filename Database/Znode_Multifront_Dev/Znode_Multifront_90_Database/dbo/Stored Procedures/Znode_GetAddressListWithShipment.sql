CREATE PROCEDURE [dbo].[Znode_GetAddressListWithShipment]
(
	 @UserId       INT   = 0,
    @OrderId        INT   = 0   
)
AS 
/*
	 Summary :- This Procedure is used to returns address and shipment id(omsOrderShipmentId).
	 Unit Testig 
	 EXEC  Znod_GetAddressListWithShipment 4,1
	 
*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON ;

	IF ((SELECT AccountId FROM ZnodeUser WHERE UserId =@UserId) is not null)
	BEGIN
		SELECT ZA.*, cast(Case when ZU.AspNetUserId is null then 1 else 0 end as bit) IsGuest
			,(SELECT top 1 omsOrderShipmentId FROM ZnodeOmsOrderLineItems ZOLI INNER JOIN 
				ZnodeOmsOrderDetails	Zood on ZOOD.OmsOrderDetailsId = ZOLI.OmsOrderDetailsId
				WHERE ZOOD.OmsOrderId= @OrderId) omsOrderShipmentId
		FROM ZnodeUser ZU INNER JOIN ZnodeAccountAddress ZAA on ZAA.AccountId =ZU.AccountId
		INNER JOIN ZnodeAddress ZA on ZA.AddressId =ZAA.AddressId
		WHERE ZU.UserId =@UserId
	END 
	ELSE 
	BEGIN
		SELECT ZA.*  , cast(Case when ZU.AspNetUserId is null then 1 else 0 end as bit) IsGuest
			,(SELECT TOP 1 omsOrderShipmentId FROM ZnodeOmsOrderLineItems ZOLI INNER JOIN 
				ZnodeOmsOrderDetails	Zood on ZOOD.OmsOrderDetailsId = ZOLI.OmsOrderDetailsId
				WHERE ZOOD.OmsOrderId= @OrderId) omsOrderShipmentId	   
		FROM ZnodeUser ZU INNER JOIN ZnodeUserAddress ZUA on ZUA.UserId =ZU.UserId
		INNER JOIN ZnodeAddress ZA on ZA.AddressId =ZUA.AddressId
		WHERE ZU.UserId =@UserId
	END
			
END TRY 
BEGIN CATCH 
	DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAddressListWithShipment @UserId = '''+ISNULL(@UserId,'''''')+''',@OrderId='+ISNULL(CAST(@OrderId AS
	VARCHAR(50)),'''''')
              			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetAddressListWithShipment',
			@ErrorInProcedure = 'Znode_GetAddressListWithShipment',
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
END CATCH 
END