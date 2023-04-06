CREATE PROCEDURE [dbo].[Znode_GetBillingShippingAddress]
(
	@BillingaddressId INT   = 0,
    @OrderShipmentId  INT   = 0
)
AS 
/*
	 Summary :- This Procedure is used to get the display the shipping and billing address 
	 Unit Testig 
	 EXEC  GetBillingShippingAddress 1,1
	 
*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON ;

	IF OBJECT_ID('tempdb..#ZnodeAddress') IS NOT NULL 
		DROP TABLE #ZnodeAddress

	SELECT 0 as OmsOrderShipmentId,
		AddressId,
		FirstName,
		LastName,
		DisplayName,
		CompanyName,
		Address1,
		Address2,
		Address3,
		CountryName,
		StateName StateCode,
		CityName,
		PostalCode,
		PhoneNumber,
		Mobilenumber,
		AlternateMobileNumber,
		FaxNumber,
		IsDefaultBilling,
		IsDefaultShipping,
		IsActive,
		ExternalId,
		IsShipping,
		IsBilling,
		EmailAddress  
	INTO #ZnodeAddress  
	FROM ZnodeAddress 
	WHERE AddressId= @BillingaddressId 
	UNION ALL
	SELECT OmsOrderShipmentId,
		AddressId,
		ShipToFirstName AS FirstName,
		ShipToLastName AS LastName,
		ShipName AS DisplayName,
		ShipToCompanyName AS CompanyName,
		ShipToStreet1 AS Address1,
		ShipToStreet2 AS Address2,
		'' AS Address3,
		ShipToCountry AS CountryName,
		ShipToStateCode AS StateCode,
		ShipToCity AS CityName,
		ShipToPostalCode AS PostalCode,
		ShipToPhoneNumber AS PhoneNumber,
		'' AS Mobilenumber,
		'' AS AlternateMobileNumber,
		'' AS FaxNumber,
		CAST(0 AS BIT) AS IsDefaultBilling,
		CAST(0 AS BIT) AS IsDefaultShipping,
		Cast(1 AS BIT) AS IsActive,
		'' AS ExternalId,
		CAST(1 AS BIT) AS IsShipping,
		CAST(0 AS BIT) AS IsBilling,
		ShipToEmailId AS EmailAddress
	FROM ZnodeOmsOrderShipment
	WHERE OmsOrderShipmentId = @OrderShipmentId

	SELECT ZA.*,ZS.StateName 
	FROM #ZnodeAddress ZA
	Left JOIN ZnodeState ZS on ZS.StateCode = ZA.StateCode and ZS.CountryCode = ZA.CountryName
		
END TRY 
BEGIN CATCH 
	DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetBillingShippingAddress @BillingaddressId = '''+ISNULL(@BillingaddressId,'''''')+''',@orderShipmentId='+ISNULL(CAST(@orderShipmentId AS
	VARCHAR(50)),'''''')
              			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetBillingShippingAddress',
			@ErrorInProcedure = 'Znode_GetBillingShippingAddress',
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
END CATCH 
END