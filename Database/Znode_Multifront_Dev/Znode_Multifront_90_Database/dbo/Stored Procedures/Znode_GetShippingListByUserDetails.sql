CREATE PROCEDURE [dbo].[Znode_GetShippingListByUserDetails]
(
	@PortalId	INT,
	@UserId		INT,
	@RowsCount  INT OUT
)
AS
/*
	Summary: This Procedure is used to get shipping details according to associated profile or portal.

	Unit Testing:

	EXEC [Znode_GetShippingListByUserDetails]  @PortalId=1, @UserId=1, @RowsCount=0
*/
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @GuestProfileId INT;

		SELECT TOP 1 @GuestProfileId=ProfileId FROM ZnodeProfile WHERE ProfileName = 'Anonymous' AND @UserId = -1;

		DECLARE @ProfileIds As TABLE (ProfileId INT);

		INSERT INTO @ProfileIds
		SELECT DISTINCT ProfileId
		FROM ZnodeUserProfile
		WHERE UserId = @UserId
		UNION 
		SELECT DISTINCT ProfileId
		FROM ZnodeProfile
		WHERE ProfileId=@GuestProfileId AND @UserId = -1;

		DECLARE @ProfileShipping As TABLE (ShippingId INT, DisplayOrder INT, PortalId INT);

		INSERT INTO @ProfileShipping
		SELECT DISTINCT ShippingId, DisplayOrder, @PortalId As PortalId
		FROM ZnodeProfileShipping
		WHERE ProfileId IN (SELECT ProfileId FROM @ProfileIds) OR (ProfileId=@GuestProfileId AND @UserId = -1);

		DECLARE @PortalShipping As TABLE (ShippingId INT, PortalShippingId INT, PortalId INT);

		INSERT INTO @PortalShipping
		SELECT DISTINCT ShippingId, PortalShippingId, PortalId
		FROM ZnodePortalShipping
		WHERE PortalId = @PortalId;

		DROP TABLE IF EXISTS #Shipping;
		CREATE TABLE #Shipping (ShippingId INT, PortalShippingId INT, DisplayOrder INT, PortalId INT);

		IF EXISTS (SELECT * FROM @ProfileIds P WHERE EXISTS (SELECT * FROM ZnodePortalProfile WHERE ProfileId=P.ProfileId))
			OR EXISTS (SELECT * FROM @ProfileIds P WHERE EXISTS (SELECT * FROM ZnodeProfileShipping WHERE ProfileId=P.ProfileId))
		BEGIN
			INSERT INTO #Shipping
			SELECT DISTINCT PoS.ShippingId, PoS.PortalShippingId, PrS.DisplayOrder, PoS.PortalId
			FROM @PortalShipping PoS
			INNER JOIN @ProfileShipping PrS ON PoS.ShippingId=PrS.ShippingId;

			SET @RowsCount=@@ROWCOUNT;
		END

		IF @RowsCount=0
		BEGIN
			INSERT INTO #Shipping
			SELECT DISTINCT PoS.ShippingId, PoS.PortalShippingId, NULL AS DisplayOrder, @PortalId As PortalId
			FROM @PortalShipping PoS;

			SET @RowsCount=@@ROWCOUNT;
		END

		--Getting the shipping methods details
		SELECT DISTINCT NULL As ProfileId, NULL As ProfileName,ZPS.PortalId, ZP1.StoreName,ZS.ShippingId,ZS.ShippingTypeId,
			ZS.ShippingCode,ZS.HandlingCharge,ZS.HandlingChargeBasedOn,ZS.DestinationCountryCode,ZS.StateCode,ZS.CountyFIPS,
			ZS.Description,ZS.IsActive,ISNULL(ZPS.DisplayOrder, ZS.DisplayOrder) DisplayOrder, ZS.ZipCode,ZS.CreatedDate,
			ZS.ModifiedDate, NULL IsAssociated ,ZST.Name ShippingTypeName , ZPS.PortalShippingId, NULL As ProfileShippingId,
			ZS.ShippingName,ZST.ClassName,ZS.DeliveryTimeframe
		FROM ZnodeShipping ZS
		INNER JOIN ZnodeShippingTypes ZST ON (ZST.ShippingTypeId = ZS.ShippingTypeId)
		INNER JOIN #Shipping ZPS ON ZS.ShippingId=ZPS.ShippingId
		INNER JOIN ZnodePortal ZP1 ON ZP1.PortalId = ZPS.PortalId
		WHERE ZS.IsActive=1;

		DROP TABLE IF EXISTS #Shipping;
	END TRY
	BEGIN CATCH
		DECLARE @Status BIT;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetShippingListByUserDetails 
					@PortalId='+CAST(@PortalId AS VARCHAR(50))+',
					@UserId='+CAST(@UserId AS VARCHAR(50))+',
					@RowsCount='+CAST(@RowsCount AS VARCHAR(50));

        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetShippingListByUserDetails',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH;
END;