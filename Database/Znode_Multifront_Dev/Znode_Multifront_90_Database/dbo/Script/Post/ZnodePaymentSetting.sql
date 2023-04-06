----ZPD-18048---
--[Script File For ZnodePortalPaymentSetting]--
--Add Coloumn IsUsedForOfflinePayment--
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsUsedForOfflinePayment' AND TABLE_NAME = 'ZnodePortalPaymentSetting')
BEGIN
        ALTER TABLE ZnodePortalPaymentSetting ADD IsUsedForOfflinePayment BIT NOT NULL DEFAULT ((0))
END

--Add Coloumn IsUsedForWebStorePayment--
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsUsedForWebStorePayment' AND TABLE_NAME = 'ZnodePortalPaymentSetting')
BEGIN
        ALTER TABLE ZnodePortalPaymentSetting ADD IsUsedForWebStorePayment BIT NOT NULL DEFAULT ((0))
END
go
update ZnodePortalPaymentSetting set IsUsedForWebStorePayment = 1
update ZnodePortalPaymentSetting set IsUsedForOfflinePayment = 0 where IsUsedForOfflinePayment is null 

Update ZnodePaymentSetting set IsActive = 0
where isnull(PaymentGatewayId,0) in (select PaymentGatewayId from ZnodePaymentGateway WHERE GatewayCode = 'cybersource')
