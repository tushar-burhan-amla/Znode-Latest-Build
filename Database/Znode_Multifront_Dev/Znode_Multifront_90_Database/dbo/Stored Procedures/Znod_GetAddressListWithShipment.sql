Create PROCEDURE [dbo].Znod_GetAddressListWithShipment
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

		if ((select AccountId from znodeuser where UserId =@UserId) is not null)
		begin
		 Select ZA.*
	  , cast(Case when ZU.AspNetUserId is null then 1 else 0 end as bit) IsGuest
	  ,(select top 1 omsOrderShipmentId from ZnodeOmsOrderLineItems ZOLI inner join 
				 ZnodeOmsOrderDetails	Zood on ZOOD.OmsOrderDetailsId = ZOLI.OmsOrderDetailsId
				 where ZOOD.OmsOrderId= @OrderId) omsOrderShipmentId
	  from ZnodeUser ZU inner join ZnodeAccountAddress ZAA on ZAA.AccountId =ZU.AccountId
			inner Join ZnodeAddress ZA on ZA.AddressId =ZAA.AddressId
			where ZU.UserId =@UserId
		end 
		else 
		begin
				 Select ZA.*
	  , cast(Case when ZU.AspNetUserId is null then 1 else 0 end as bit) IsGuest
	  ,(select top 1 omsOrderShipmentId from ZnodeOmsOrderLineItems ZOLI inner join 
				 ZnodeOmsOrderDetails	Zood on ZOOD.OmsOrderDetailsId = ZOLI.OmsOrderDetailsId
				 where ZOOD.OmsOrderId= @OrderId) omsOrderShipmentId
	   
	  from ZnodeUser ZU inner join ZnodeUserAddress ZUA on ZUA.UserId =ZU.UserId
			inner Join ZnodeAddress ZA on ZA.AddressId =ZUA.AddressId
			where ZU.UserId =@UserId
		end
		
		
			
		 END TRY 
		 BEGIN CATCH 
			DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			@ErrorCall NVARCHAR(MAX)= 'EXEC Znod_GetAddressListWithShipment @UserId = '''+ISNULL(@UserId,'''''')+''',@OrderId='+ISNULL(CAST(@OrderId AS
			VARCHAR(50)),'''''')
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znod_GetAddressListWithShipment',
					@ErrorInProcedure = 'Znod_GetAddressListWithShipment',
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
		 END CATCH 
   END
GO


