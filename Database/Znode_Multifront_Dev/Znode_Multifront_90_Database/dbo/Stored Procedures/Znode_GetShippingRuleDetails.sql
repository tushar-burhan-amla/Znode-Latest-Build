CREATE PROCEDURE [dbo].[Znode_GetShippingRuleDetails] 
(
	@ShippingRuleTypeCode NVARCHAR(500)='',
	@CountryCode VARCHAR(100)='',
	@ShippingId INT,
	@PortalId INT
)
AS 
BEGIN
BEGIN TRY
	SET NOCOUNT ON;

	SELECT ShipRule.[ShippingRuleId] AS [ShippingRuleId], Ship.[ShippingId] AS [ShippingId], ShipRule.[ClassName] AS [ClassName],
	   ShipRule.[LowerLimit] AS [LowerLimit], ShipRule.[UpperLimit] AS [UpperLimit], ShipRule.[BaseCost] AS [BaseCost],
	   ShipRule.[PerItemCost] AS [PerItemCost], ShipRule.[Custom1] AS [Custom1], ShipRule.[Custom2] AS [Custom2],
	   ShipRule.[Custom3] AS [Custom3], ShipRule.[ExternalId] AS [ExternalId], ShipRule.[ShippingRuleTypeCode] AS [ShippingRuleTypeCode]
	FROM [dbo].[ZnodeShippingRule] AS ShipRule
	INNER JOIN [dbo].[ZnodeShipping] AS Ship ON ShipRule.[ShippingId] = Ship.[ShippingId]
	INNER JOIN [dbo].[ZnodePortalShipping] AS PortalShip ON Ship.[ShippingId] = PortalShip.[ShippingId]
	LEFT OUTER JOIN [dbo].[ZnodeCountry] AS Country ON (Ship.[DestinationCountryCode] = Country.[CountryCode]) 
	WHERE (ShipRule.[ShippingRuleTypeCode] = @ShippingRuleTypeCode) AND ((Country.[CountryCode] = @CountryCode) OR ISNULL(@CountryCode,'') = '') 
	AND (PortalShip.[PortalId] = @PortalId) AND (PortalShip.[ShippingId] = @ShippingId) 
END TRY
BEGIN CATCH
	DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetShippingRuleDetails @PortalId = '+cast (@PortalId AS VARCHAR(50))+',@ShippingId='+CAST(@ShippingId AS VARCHAR(50))+',@ShippingRuleTypeCode='+@ShippingRuleTypeCode+',@CountryCode='+@CountryCode;
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetShippingRuleDetails',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
END CATCH;
END