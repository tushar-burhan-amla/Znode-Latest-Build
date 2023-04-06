--dt 15/02/2021 - ZPD-12855 
INSERT INTO ZnodePaymentGateway (GatewayName,	WebsiteURL	,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, GatewayCode )
SELECT 'Card Connect','https://cardpointe.com/',2,GETDATE(),2,GETDATE(), 'cardconnect'
WHERE NOT EXISTS(SELECT * FROM ZnodePaymentGateway WHERE GatewayCode = 'cardconnect')

DELETE FROM ZnodeProfilePaymentSetting
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))
UPDATE ZnodeOmsOrderDetails SET PaymentSettingId = NULL
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))
UPDATE ZnodeOmsQuote SET PaymentSettingId = NULL
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))
DELETE FROM ZnodeRmaReturnPaymentDetails
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))
DELETE FROM ZnodePortalPaymentApprovers
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))
DELETE FROM ZnodePortalPaymentSetting
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))
DELETE FROM ZnodePaymentSetting
WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal')
DELETE FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'

-- dt 16/03/2022 - ZPD-18411
IF NOT EXISTS (	SELECT TOP 1 1 FROM ZnodePaymentSetting PS
			INNER JOIN ZnodePaymentGateway PG ON PS.PaymentGatewayId=PG.PaymentGatewayId
			WHERE PG.GatewayCode = 'paymentech')
BEGIN
	DELETE PS
	FROM ZnodePaymentSetting PS
	INNER JOIN ZnodePaymentGateway PG ON PS.PaymentGatewayId=PG.PaymentGatewayId
	WHERE PG.GatewayCode = 'paymentech'

	DELETE FROM ZnodePaymentGateway WHERE GatewayCode = 'paymentech'
END

IF NOT EXISTS (	SELECT TOP 1 1 FROM ZnodePaymentSetting PS
			INNER JOIN ZnodePaymentGateway PG ON PS.PaymentGatewayId=PG.PaymentGatewayId
			WHERE PG.GatewayCode = 'stripe')
BEGIN
	DELETE PS
	FROM ZnodePaymentSetting PS
	INNER JOIN ZnodePaymentGateway PG ON PS.PaymentGatewayId=PG.PaymentGatewayId
	WHERE PG.GatewayCode = 'stripe'

	DELETE FROM ZnodePaymentGateway WHERE GatewayCode = 'stripe'
END

IF NOT EXISTS (	SELECT TOP 1 1 FROM ZnodePaymentSetting PS
			INNER JOIN ZnodePaymentGateway PG ON PS.PaymentGatewayId=PG.PaymentGatewayId
			WHERE PG.GatewayCode = 'worldpay')
BEGIN
	DELETE PS
	FROM ZnodePaymentSetting PS
	INNER JOIN ZnodePaymentGateway PG ON PS.PaymentGatewayId=PG.PaymentGatewayId
	WHERE PG.GatewayCode = 'worldpay'

	DELETE FROM ZnodePaymentGateway WHERE GatewayCode = 'worldpay'
END

-- ZPD-21236 Dt: 22-Sep-2022
INSERT INTO ZNodePaymentGateway
   ([GatewayName], [WebsiteURL], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GatewayCode])
SELECT
    N'PaypalExpress', N'http://www.paypal.com', 2, GETDATE(), 2, GETDATE(), N'paypalexpress'
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZNodePaymentGateway 
                WHERE [GatewayCode] = 'paypalexpress')

