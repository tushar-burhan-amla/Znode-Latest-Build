CREATE PROCEDURE [dbo].[Znode_GetPaymentSettingByUserDetails]
(
	@PortalId	INT,
	@UserId		INT,
	@RowsCount  INT OUT
)
AS
/*
	Summary: This Procedure is used to get payment details according to associated profile or portal.

	Unit Testing:

	EXEC [Znode_GetPaymentSettingByUserDetails] @PortalId=1, @UserId=1, @RowsCount=0
*/	
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @GuestProfileId INT;

		SELECT TOP 1 @GuestProfileId=ProfileId FROM ZnodeProfile WHERE ProfileName = 'Anonymous' AND @UserId = -1;
		SET @GuestProfileId=ISNULL(@GuestProfileId,0);

		DECLARE @ProfileIds As TABLE (ProfileId INT);

		INSERT INTO @ProfileIds
		SELECT DISTINCT ProfileId
		FROM ZnodeUserProfile
		WHERE UserId = @UserId
		UNION 
		SELECT DISTINCT ProfileId
		FROM ZnodeProfile
		WHERE ProfileId=@GuestProfileId AND @UserId = -1;

		DECLARE @ProfilePaymentSetting As TABLE (PaymentSettingId INT, DisplayOrder INT, PortalId INT);

		INSERT INTO @ProfilePaymentSetting
		SELECT DISTINCT PaymentSettingId, DisplayOrder, @PortalId As PortalId
		FROM ZnodeProfilePaymentSetting
		WHERE ProfileId IN (SELECT ProfileId FROM @ProfileIds) OR (ProfileId=@GuestProfileId AND @UserId = -1);

		DECLARE @PortalPaymentSetting As TABLE (PaymentSettingId INT, PaymentDisplayName NVARCHAR(1200), PortalId INT);

		INSERT INTO @PortalPaymentSetting
		SELECT DISTINCT PaymentSettingId, PaymentDisplayName, PortalId
		FROM ZnodePortalPaymentSetting
		WHERE IsUsedForWebStorePayment=1
			AND PortalId = @PortalId;

		DROP TABLE IF EXISTS #PaymentSetting;
		CREATE TABLE #PaymentSetting (PaymentSettingId INT, PaymentDisplayName NVARCHAR(1200), DisplayOrder INT, PortalId INT);

		IF EXISTS (SELECT * FROM @ProfileIds P WHERE EXISTS (SELECT * FROM ZnodePortalProfile WHERE ProfileId=P.ProfileId))
			OR EXISTS (SELECT * FROM @ProfileIds P WHERE EXISTS (SELECT * FROM ZnodeProfilePaymentSetting WHERE ProfileId=P.ProfileId))
		BEGIN
			INSERT INTO #PaymentSetting
			SELECT DISTINCT PoPS.PaymentSettingId, PoPS.PaymentDisplayName, PrPS.DisplayOrder, PoPS.PortalId
			FROM @PortalPaymentSetting PoPS
			INNER JOIN @ProfilePaymentSetting PrPS ON PoPS.PaymentSettingId=PrPS.PaymentSettingId;

			SET @RowsCount=@@ROWCOUNT;
		END

		IF @RowsCount=0
		BEGIN
			INSERT INTO #PaymentSetting
			SELECT DISTINCT PoPS.PaymentSettingId, PoPS.PaymentDisplayName, NULL AS DisplayOrder, @PortalId As PortalId
			FROM @PortalPaymentSetting PoPS;

			SET @RowsCount=@@ROWCOUNT;
		END

		DROP TABLE IF EXISTS #ZnodePortalPaymentApprovers;

		--Getting the payment portal approver details
		SELECT PaymentSettingId ,ZPPG.PortalPaymentGroupId,ZPA.PortalId 
		INTO #ZnodePortalPaymentApprovers
		FROM ZnodePortalPaymentApprovers ZPPA
		INNER JOIN [ZnodePortalPaymentGroup] ZPPG ON( ZPPA.PortalPaymentGroupId = ZPPG.PortalPaymentGroupId)
		INNER JOIN [ZnodePortalApproval] ZPA ON (ZPA.PortalApprovalId = ZPPG.PortalApprovalId)
		WHERE ZPA.EnableApprovalManagement =1 AND ZPA.PortalId = @PortalId AND ZPPG.isActive = 1;

		--Getting the payment methods details
		SELECT DISTINCT ZPS.PaymentSettingId, ZPS.PaymentApplicationSettingId, ZPS.PaymentTypeId, ZPS.PaymentGatewayId,
			ZPS.PaymentName, ZPS.IsActive, ISNULL(ZPPS.DisplayOrder, ZPS.DisplayOrder) DisplayOrder, ZPS.IsTestMode,
			ZPS.IsPoDocUploadEnable, ZPS.IsPoDocRequire, ZPS.CreatedBy, ZPS.CreatedDate, ZPS.ModifiedBy, ZPS.ModifiedDate,
			ZPPS.PortalId, ZP1.StoreName, NULL IsAssociated, NULL As ProfileId, NULL As ProfileName, ZPT.BehaviorType PaymentTypeName,
			ZPG.GatewayName,
			CASE WHEN @PortalId > 0 AND ZPPS.PaymentDisplayName IS NOT NULL
				THEN ZPPS.PaymentDisplayName ELSE ZPS.PaymentDisplayName END PaymentDisplayName, 
			NULL PaymentExternalId, 
			CAST(CASE WHEN YU.PaymentSettingId IS NOT NULL THEN 1 ELSE 0 END As BIT) AS IsApprovalRequired,
			ZPS.PaymentCode, ZPG.GatewayCode, ZPT.IsCallToPaymentAPI, ZPS.IsBillingAddressOptional, ZPS.IsOABRequired,
			YU.PortalPaymentGroupId, '' AS PublishState
		FROM ZnodePaymentSetting ZPS
		INNER JOIN ZnodePaymentType ZPT ON (ZPT.PaymentTypeId = ZPS.PaymentTypeId)
		LEFT JOIN ZnodePaymentGateway ZPG ON (ZPG.PaymentGatewayId= ZPS.PaymentGatewayId)
		INNER JOIN #PaymentSetting ZPPS ON (ZPS.PaymentSettingId = ZPPS.PaymentSettingId )
		INNER JOIN ZnodePortal ZP1 ON ZP1.PortalId = ZPPS.PortalId
		LEFT JOIN #ZnodePortalPaymentApprovers YU ON (YU.PaymentSettingId = ZPS.PaymentSettingId)
		WHERE ZPS.IsActive=1;

		DROP TABLE IF EXISTS #ZnodePortalPaymentApprovers;
		DROP TABLE IF EXISTS #PaymentSetting;
	END TRY
	BEGIN CATCH
		DECLARE @Status BIT;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPaymentSettingByUserDetails 
					@PortalId='+CAST(@PortalId AS VARCHAR(50))+',
					@UserId='+CAST(@UserId AS VARCHAR(50))+',
					@RowsCount='+CAST(@RowsCount AS VARCHAR(50));

		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetPaymentSettingByUserDetails',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END