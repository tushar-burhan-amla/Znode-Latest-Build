CREATE PROCEDURE [dbo].[Znode_GetCommonPaymentSettingForOfflinePayment]
(
	@PortalIds VARCHAR(2000) = '',
	@ProfileIds VARCHAR(2000) = '',
	@PaymentSettingIds VARCHAR(2000) = '' OUT
)
AS
/*
Summary:- This procedure is used to get payment setting of portal ids and profile ids
Unit Testing
EXEC Znode_GetCommonPaymentSetting
*/
BEGIN
BEGIN TRY
SET NOCOUNT ON
     
	DECLARE @TBL_PaymentSetting TABLE(PaymentSettingId INT , ProfileId int )

	INSERT INTO @TBL_PaymentSetting (PaymentSettingId,ProfileId)
	SELECT PaymentSettingId,ProfileId
	FROM ZnodeProfilePaymentSetting ZPPS
	WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@ProfileIds,',') SP WHERE ZPPS.ProfileId = SP.Item)

	IF EXISTS (SELECT TOP 1 1 FROM @TBL_PaymentSetting )
	BEGIN
		SET @PaymentSettingIds = ISNULL(SUBSTRING(( SELECT ','+CAST(PaymentSettingId AS VARCHAR(50))
		FROM ZnodePortalPaymentSetting ZPPS
		WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@PortalIds,',') SP WHERE ZPPS.PortalId = SP.Item)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_PaymentSetting  TBPS WHERE TBPS.PaymentSettingId = ZPPS.PaymentSettingId ) FOR XML PATH ('')),2,4000),'')
	END
	
	IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_PaymentSetting ) OR @PaymentSettingIds = ''
	BEGIN
		SET @PaymentSettingIds = SUBSTRING((  SELECT ','+CAST(PaymentSettingId AS VARCHAR(50))
		FROM ZnodePortalPaymentSetting ZPPS
		WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@PortalIds,',') SP WHERE ZPPS.PortalId = SP.Item) FOR XML PATH ('')),2,4000)
		
	END
 
END TRY
BEGIN CATCH
	DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCommonPaymentSetting @PortalIds = '+@PortalIds+',@ProfileIds='+@ProfileIds+',@PaymentSettingIds='+@PaymentSettingIds+',@Status='+CAST(@Status AS VARCHAR(10));
             
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
 
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetCommonPaymentSetting',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH
END