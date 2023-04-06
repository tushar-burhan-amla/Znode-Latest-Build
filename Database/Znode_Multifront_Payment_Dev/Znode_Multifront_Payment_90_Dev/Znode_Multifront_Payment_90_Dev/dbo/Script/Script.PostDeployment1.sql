
--DELETE FROM [dbo].[ZNodeDomain]
--GO
--DELETE FROM [dbo].[ZNodeActivityLog]
--GO
--DELETE FROM [dbo].[ZnodeTransactions]
--GO
--DELETE FROM [dbo].[ZNodePaymentSettingCredential]
--GO
--DELETE FROM [dbo].[ZnodePaymentMethods]
--GO
--DELETE FROM [dbo].[ZNodePaymentSetting]
--GO
--DELETE FROM [dbo].[ZNodePaymentType]
--GO
--DELETE FROM [dbo].[ZNodePaymentGateway]
--GO
--DELETE FROM [dbo].[ZnodePaymentCustomers]
--GO
--DELETE FROM [dbo].[ZnodePaymentAddress]
--GO
--SET IDENTITY_INSERT [dbo].[ZNodePaymentGateway] ON 
--GO
--INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (1, N'Authorize.Net', N'http://www.authorize.net', N'AuthorizeNetCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (4, N'Stripe', N'', N'StripeCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (5, N'Paypal', N'http://www.paypal.com', N'PaypalCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (6, N'Chase Paymentech', N'https://securevar.paymentech.com/manager', N'PaymentTechProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (7, N'WorldPay', N'http://www.wordlpay.com', N'SecureNetCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (8, N'Braintree', N'https://www.braintreepayments.com/', N'BraintreeProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (9, N'PayFlow', N'http://www.manager.paypal.com', N'PayFlowCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--SET IDENTITY_INSERT [dbo].[ZNodePaymentGateway] OFF
--GO
--SET IDENTITY_INSERT [dbo].[ZNodePaymentType] ON 
--GO
--INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (1, N'Credit Card', N'Credit Card', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (2, N'Purchase Order', N'Purchase Order', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (3, N'Paypal Express', N'Paypal Express', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (4, N'COD', N'Charge On Delivery', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
--GO
--SET IDENTITY_INSERT [dbo].[ZNodePaymentType] OFF
--GO
--SET IDENTITY_INSERT [dbo].[ZNodePaymentSetting] ON 
--GO
--INSERT [dbo].[ZNodePaymentSetting] ([PaymentSettingId], [PaymentTypeId], [PaymentGatewayId], [EnableVisa], [EnableMasterCard], [EnableAmex], [EnableDiscover], [EnableRecurringPayments], [EnableVault], [IsActive], [DisplayOrder], [PreAuthorize], [IsRMACompatible], [TestMode], [CreatedDate], [ModifiedDate], [EnablePODocUpload], [IsPODocRequired]) VALUES (1, 4, NULL, NULL, NULL, NULL, NULL, 0, 0, 1, 1, 0, NULL, 0, CAST(N'2017-03-31T00:00:00.000' AS DateTime), CAST(N'2017-03-31T00:00:00.000' AS DateTime), 0, 0)
--GO
--SET IDENTITY_INSERT [dbo].[ZNodePaymentSetting] OFF
--GO


SET IDENTITY_INSERT [dbo].[ZNodePaymentGateway] ON 
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT 1, N'Authorize.Net', N'http://www.authorize.net', N'AuthorizeNetCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [PaymentGatewayId] = 1)
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT 4, N'Stripe', N'', N'StripeCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [PaymentGatewayId] = 4)
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT 5, N'Paypal', N'http://www.paypal.com', N'PaypalCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [PaymentGatewayId] = 5)
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT 6, N'Chase Paymentech', N'https://securevar.paymentech.com/manager', N'PaymentTechProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [PaymentGatewayId] = 6)
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT 7, N'WorldPay', N'http://www.wordlpay.com', N'SecureNetCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [PaymentGatewayId] = 7)
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT 8, N'Braintree', N'https://www.braintreepayments.com/', N'BraintreeProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [PaymentGatewayId] = 8)
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT 9, N'PayFlow', N'http://www.manager.paypal.com', N'PayFlowCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [PaymentGatewayId] = 9)
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentGateway] OFF
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentType] ON
GO
INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) 
SELECT 1, N'CREDIT_CARD', N'Credit Card', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentType] WHERE [PaymentTypeId] = 1)
GO
INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) 
SELECT 2, N'PURCHASE_ORDER', N'Purchase Order', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentType] WHERE [PaymentTypeId] = 2)
GO
INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) 
SELECT 3, N'PaypalExpress', N'Paypal Express', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentType] WHERE [PaymentTypeId] = 3)
GO
INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) 
SELECT 4, N'COD', N'Charge On Delivery', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentType] WHERE [PaymentTypeId] = 4)
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentType] OFF
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentSetting] ON
GO
INSERT [dbo].[ZNodePaymentSetting] ([PaymentSettingId], [PaymentTypeId], [PaymentGatewayId], [EnableVisa], [EnableMasterCard], [EnableAmex], [EnableDiscover], [EnableRecurringPayments], [EnableVault], [IsActive], [DisplayOrder], [PreAuthorize], [IsRMACompatible], [TestMode], [CreatedDate], [ModifiedDate], [EnablePODocUpload], [IsPODocRequired]) 
SELECT 1, 4, NULL, NULL, NULL, NULL, NULL, 0, 0, 1, 1, 0, NULL, 0, CAST(N'2017-03-31T00:00:00.000' AS DateTime), CAST(N'2017-03-31T00:00:00.000' AS DateTime), 0, 0
WHERE NOT EXISTS(SELECT * FROM [ZNodePaymentSetting] WHERE [PaymentSettingId] = 1 AND [PaymentTypeId] = 4)
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentSetting] OFF
GO
--GO
INSERT [dbo].[ZNodePaymentGateway] ( [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT  N'Authorize.Net', N'http://www.authorize.net', N'AuthorizeNetCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [GatewayName] = 'Authorize.Net' )
GO
INSERT [dbo].[ZNodePaymentGateway] ( [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT  N'Stripe', N'', N'StripeCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [GatewayName] = 'Stripe' )
GO
INSERT [dbo].[ZNodePaymentGateway] ( [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT  N'Paypal', N'http://www.paypal.com', N'PaypalCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [GatewayName] = 'Paypal' )
GO
INSERT [dbo].[ZNodePaymentGateway] ( [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT  N'Chase Paymentech', N'https://securevar.paymentech.com/manager', N'PaymentTechProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [GatewayName] = 'Chase Paymentech' )
GO
INSERT [dbo].[ZNodePaymentGateway] ( [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT  N'WorldPay', N'http://www.wordlpay.com', N'SecureNetCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [GatewayName] = 'WorldPay' )
GO
INSERT [dbo].[ZNodePaymentGateway] ( [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT  N'Braintree', N'https://www.braintreepayments.com/', N'BraintreeProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [GatewayName] = 'Braintree')
GO
INSERT [dbo].[ZNodePaymentGateway] ( [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) 
SELECT  N'PayFlow', N'http://www.manager.paypal.com', N'PayFlowCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentGateway] WHERE [GatewayName] = 'PayFlow' )

GO
INSERT [dbo].[ZNodePaymentType] ( [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) 
SELECT  N'CREDIT_CARD', N'Credit Card', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentType] WHERE [Name]= 'CREDIT_CARD' )
GO
INSERT [dbo].[ZNodePaymentType] ( [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) 
SELECT  N'PURCHASE_ORDER', N'Purchase Order', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentType] WHERE [Name]= 'PURCHASE_ORDER' )
GO
INSERT [dbo].[ZNodePaymentType] ( [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) 
SELECT  N'PaypalExpress', N'Paypal Express', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentType] WHERE [Name]= 'PaypalExpress' )
GO
INSERT [dbo].[ZNodePaymentType] ( [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) 
SELECT  N'COD', N'Charge On Delivery', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime)
WHERE NOT EXISTS( SELECT * FROM [ZNodePaymentType] WHERE [Name]= 'COD' )
GO

INSERT [dbo].[ZNodePaymentSetting] ( [PaymentTypeId], [PaymentGatewayId], [EnableVisa], [EnableMasterCard], [EnableAmex], [EnableDiscover], [EnableRecurringPayments], [EnableVault], [IsActive], [DisplayOrder], [PreAuthorize], [IsRMACompatible], [TestMode], [CreatedDate], [ModifiedDate], [EnablePODocUpload], [IsPODocRequired]) 
SELECT 4, NULL, NULL, NULL, NULL, NULL, 0, 0, 1, 1, 0, NULL, 0, CAST(N'2017-03-31T00:00:00.000' AS DateTime), CAST(N'2017-03-31T00:00:00.000' AS DateTime), 0, 0
WHERE NOT EXISTS(SELECT * FROM [ZNodePaymentSetting] WHERE  [PaymentTypeId] = 4)
GO

if exists(select * from INFORMATION_SCHEMA.columns where table_name = 'ZnodePaymentMethods' and COLUMN_NAME = 'CreditCardLastFourDigit' )
begin
	alter table ZnodePaymentMethods alter column CreditCardLastFourDigit VARCHAR (4)      NULL
end

go

SET  IDENTITY_INSERT ZnodePaymentGateway ON
INSERT INTO ZnodePaymentGateway (PaymentGatewayId,	GatewayName,	WebsiteURL	,ClassName,CreatedDate,ModifiedDate )
SELECT 3,	'CyberSource',	'http://www.cybersource.com','CyberSourceCustomerProvider','2018-03-28 11:58:47.773','2018-03-28 11:58:47.773'
WHERE NOT EXISTS(SELECT * FROM ZnodePaymentGateway WHERE PaymentGatewayId = 3)
SET  IDENTITY_INSERT ZnodePaymentGateway OFF

GO

SET  IDENTITY_INSERT znodePaymenttype ON
INSERT INTO ZnodePaymentType (PaymentTypeId,Name,Description,IsActive,CreatedDate,ModifiedDate )
SELECT 5,'AmazonPay',	'Amazon Pay',1,getdate(),getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodePaymentType WHERE NAME = 'AmazonPay')
SET  IDENTITY_INSERT znodePaymenttype OFF

GO

-- spp merging

SET IDENTITY_INSERT ZnodePaymentType ON 

INSERT INTO ZnodePaymentType (PaymentTypeId,Name,Description,IsActive,CreatedDate,ModifiedDate)
SELECT 6,'Invoice Me','Invoice Me',1,GETDATE(),GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePaymentType WHERE  name = 'Invoice Me')

SET IDENTITY_INSERT ZnodePaymentType OFF 

GO

-- dt\22\08\2018  

update ZNodePaymentType set Name='CREDIT_CARD' where Name = 'Credit Card'
update ZNodePaymentType set Name='PURCHASE_ORDER' where Name = 'Purchase Order'
update ZNodePaymentType set Name='PAYPAL_EXPRESS' where Name = 'Paypal Express'
update ZNodePaymentType set Name='COD' where Name = 'COD'
update ZNodePaymentType set Name='AMAZON_PAY' where Name = 'AmazonPay'

GO

-- dt:   17\08\2018

update ZnodepaymentType
set Code = Name  
where Code is null

GO
update ZNodePaymentGateway
set GatewayCode = LOWER(REPLACE(REPLACE(REPLACE(GatewayName, ' ', ''), '.', ''),'Chase',''))

GO

update ZnodePaymentSetting 
set PaymentCode = PaymentSettingId
where PaymentCode is null

GO

update ZNodePaymentType set Code ='CREDITCARD' where Name = 'CREDIT_CARD'
update ZNodePaymentType set Code ='PURCHASEORDER' where Name = 'PURCHASE_ORDER'
update ZNodePaymentType set Code ='PAYPALEXPRESS' where Name = 'PAYPAL_EXPRESS'
update ZNodePaymentType set Code ='CHARGEONDELIVERY' where Name = 'COD'
update ZNodePaymentType set Code ='AMAZONPAY' where Name = 'AMAZON_PAY'
update ZNodePaymentType set Code ='INVOICEME' where Name = 'Invoice Me'

GO
update ZnodepaymentType
set BehaviorType = Name

GO

update ZNodePaymentType set Name = 'AmazonPay' where BehaviorType = 'AMAZON_PAY'
update ZNodePaymentType set Name = 'PaypalExpress' where BehaviorType = 'PAYPAL_EXPRESS'

GO

update ZNodePaymentType set BehaviorType = 'COD' where PaymentTypeId =6

-- ZPD-13059, ZPD-12119 (17/11/2020)
IF EXISTS(SELECT 1 FROM sys.columns WHERE [name] = N'BehaviourType'
AND [object_id] = OBJECT_ID(N'ZNodePaymentType'))
BEGIN
EXEC sp_RENAME 'ZNodePaymentType.BehaviourType', 'BehaviorType' , 'COLUMN'
END;

-- ZPD-13059, ZPD-12119 (17/11/2020)
IF EXISTS(SELECT 1 FROM sys.columns WHERE [name] = N'FisrtName'
AND [object_id] = OBJECT_ID(N'ZnodePaymentCustomers'))
BEGIN
EXEC sp_RENAME 'ZnodePaymentCustomers.FisrtName', 'FirstName' , 'COLUMN'
END;
update ZNodePaymentType set BehaviorType = 'COD' where PaymentTypeId =6
go
Update ZnodePaymentType Set BehaviorType = 'PAYPAL_EXPRESS' where Name = 'PaypalExpress' and BehaviorType = 'PaypalExpress'

--dt 11/02/2021

INSERT INTO ZnodePaymentGateway (GatewayName,	WebsiteURL	,ClassName,CreatedDate,ModifiedDate, GatewayCode )
SELECT 'Card Connect','https://cardpointe.com/','CardConnectProvider',GETDATE(),GETDATE(), 'cardconnect'
WHERE NOT EXISTS(SELECT * FROM ZnodePaymentGateway WHERE GatewayCode = 'cardconnect')

--ZPD-15392
Update ZnodeTransactions SET PaymentSettingId = NULL
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))

DELETE FROM ZnodePaymentMethods
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))
DELETE FROM ZNodePaymentSettingCredential
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))
DELETE FROM ZNodePaymentSetting
WHERE PaymentSettingId IN (SELECT PaymentSettingId FROM ZnodePaymentSetting
		WHERE PaymentGatewayId IN (SELECT PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'))

DELETE FROM ZnodePaymentGateway WHERE GatewayName = 'Paypal'

UPDATE ZnodePaymentType SET BehaviorType='COD' WHERE Code = 'INVOICEME' AND BehaviorType <> 'COD'

IF EXISTS(SELECT 1 FROM sys.columns WHERE [name] = N'FisrtName'
AND [object_id] = OBJECT_ID(N'ZnodePaymentCustomers'))
BEGIN
EXEC sp_RENAME 'ZnodePaymentCustomers.FisrtName', 'FirstName' , 'COLUMN'
END;


insert into ZNodePaymentType(Name,Description,IsActive,CreatedDate,ModifiedDate,BehaviorType,Code)
select 'ACH','Automated Clearing House',1,getdate(),getdate(),'ACH','AUTOMATEDCLEARINGHOUSE'
where not exists(select * from ZNodePaymentType where Code = 'AUTOMATEDCLEARINGHOUSE')

UPDATE ZnodePaymentGateway SET IsACHEnabled = 0 WHERE IsACHEnabled IS NULL

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsACHEnabled' AND TABLE_NAME = 'ZnodePaymentGateway')
BEGIN
	IF NOT EXISTS(SELECT * FROM SYS.default_constraints WHERE NAME = 'DF_ZnodePaymentGateway_IsACHEnabled')
	BEGIN
		ALTER TABLE ZnodePaymentGateway ADD CONSTRAINT DF_ZnodePaymentGateway_IsACHEnabled DEFAULT 0 FOR IsACHEnabled
	END
END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsACHEnabled' AND TABLE_NAME = 'ZnodePaymentGateway')
BEGIN
	ALTER TABLE ZnodePaymentGateway ALTER COLUMN IsACHEnabled BIT NOT NULL
END

update ZnodePaymentGateway set IsACHEnabled=1 where GatewayName='Card Connect'

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

GO

--Dt 28-03-2022
INSERT INTO ZNodePaymentSetting(PaymentTypeId, PaymentGatewayId, EnableVisa, EnableMasterCard, EnableAmex, EnableDiscover, EnableRecurringPayments,
	EnableVault, IsActive, DisplayOrder, PreAuthorize, IsRMACompatible, TestMode, CreatedDate, ModifiedDate, EnablePODocUpload, IsPODocRequired, PaymentCode)
SELECT 
(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD'),
(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='authorizenet'),
NULL,NULL,NULL,NULL,0,0,1,1,0,NULL,1,GETDATE(),GETDATE(),0,0,'Authorize'
WHERE NOT EXISTS(SELECT * FROM ZNodePaymentSetting WHERE PaymentTypeId = (SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD') 
        AND PaymentGatewayId = (SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='authorizenet') AND PaymentCode = 'Authorize')

INSERT INTO ZNodePaymentSetting(PaymentTypeId, PaymentGatewayId, EnableVisa, EnableMasterCard, EnableAmex, EnableDiscover, EnableRecurringPayments,
	EnableVault, IsActive, DisplayOrder, PreAuthorize, IsRMACompatible, TestMode, CreatedDate, ModifiedDate, EnablePODocUpload, IsPODocRequired, PaymentCode)
SELECT 
(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='PURCHASEORDER'),
NULL,
NULL,NULL,NULL,NULL,0,0,1,1,0,NULL,1,GETDATE(),GETDATE(),1,0,'PO'
WHERE NOT EXISTS(SELECT * FROM ZNodePaymentSetting WHERE PaymentTypeId = (SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='PURCHASEORDER') 
        AND PaymentGatewayId IS NULL AND PaymentCode = 'PO')

INSERT INTO ZNodePaymentSetting(PaymentTypeId, PaymentGatewayId, EnableVisa, EnableMasterCard, EnableAmex, EnableDiscover, EnableRecurringPayments,
	EnableVault, IsActive, DisplayOrder, PreAuthorize, IsRMACompatible, TestMode, CreatedDate, ModifiedDate, EnablePODocUpload, IsPODocRequired, PaymentCode)
SELECT 
(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD'),
(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='authorizenet'),
NULL,NULL,NULL,NULL,0,0,1,1,0,NULL,1,GETDATE(),GETDATE(),0,0,'Credit'
WHERE NOT EXISTS(SELECT * FROM ZNodePaymentSetting WHERE PaymentTypeId = (SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD') 
        AND PaymentGatewayId = (SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='authorizenet') AND PaymentCode = 'Credit')

INSERT INTO ZNodePaymentSetting(PaymentTypeId, PaymentGatewayId, EnableVisa, EnableMasterCard, EnableAmex, EnableDiscover, EnableRecurringPayments,
	EnableVault, IsActive, DisplayOrder, PreAuthorize, IsRMACompatible, TestMode, CreatedDate, ModifiedDate, EnablePODocUpload, IsPODocRequired, PaymentCode)
SELECT 
(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD'),
(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='cybersource'),
NULL,NULL,NULL,NULL,0,0,1,1,0,NULL,1,GETDATE(),GETDATE(),0,0,'card1234'
WHERE NOT EXISTS(SELECT * FROM ZNodePaymentSetting WHERE PaymentTypeId = (SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD') 
        AND PaymentGatewayId = (SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='cybersource') AND PaymentCode = 'card1234')

INSERT INTO ZNodePaymentSetting(PaymentTypeId, PaymentGatewayId, EnableVisa, EnableMasterCard, EnableAmex, EnableDiscover, EnableRecurringPayments,
	EnableVault, IsActive, DisplayOrder, PreAuthorize, IsRMACompatible, TestMode, CreatedDate, ModifiedDate, EnablePODocUpload, IsPODocRequired, PaymentCode)
SELECT 
(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD'),
(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='stripe'),
NULL,NULL,NULL,NULL,0,0,1,1,0,NULL,1,GETDATE(),GETDATE(),0,0,'Stripe'
WHERE NOT EXISTS(SELECT * FROM ZNodePaymentSetting WHERE PaymentTypeId = (SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD') 
        AND PaymentGatewayId = (SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='stripe') AND PaymentCode = 'Stripe')

INSERT INTO ZNodePaymentSettingCredential (Partner,Vendor,TestMode,PaymentSettingId,GatewayUsername,GatewayPassword,TransactionKey,
	CreatedDate,ModifiedDate,Custom1,Custom2,Custom3,Custom4,Custom5)
SELECT '','',1,
(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD')
	AND PaymentGatewayId=(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='authorizenet')
	AND PaymentCode='Authorize'),
'','','',GETDATE(),GETDATE(),NULL,NULL,NULL,NULL,NULL
WHERE NOT EXISTS (SELECT * FROM ZNodePaymentSettingCredential WHERE PaymentSettingId=(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting 
											WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD')
										AND PaymentGatewayId=(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='authorizenet')
	AND PaymentCode='Authorize'))

INSERT INTO ZNodePaymentSettingCredential (Partner,Vendor,TestMode,PaymentSettingId,GatewayUsername,GatewayPassword,TransactionKey,
	CreatedDate,ModifiedDate,Custom1,Custom2,Custom3,Custom4,Custom5)
SELECT '','',1,
(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='PURCHASEORDER')
	AND PaymentGatewayId IS NULL AND PaymentCode='PO'),
'','','',GETDATE(),GETDATE(),NULL,NULL,NULL,NULL,NULL
WHERE NOT EXISTS (SELECT * FROM ZNodePaymentSettingCredential WHERE PaymentSettingId=(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting 
											WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='PURCHASEORDER')
										AND PaymentGatewayId IS NULL AND PaymentCode='PO'))

INSERT INTO ZNodePaymentSettingCredential (Partner,Vendor,TestMode,PaymentSettingId,GatewayUsername,GatewayPassword,TransactionKey,
	CreatedDate,ModifiedDate,Custom1,Custom2,Custom3,Custom4,Custom5)
SELECT '','',1,
(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD')
	AND PaymentGatewayId=(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='authorizenet')
	AND PaymentCode='Credit'),
'','','',GETDATE(),GETDATE(),NULL,NULL,NULL,NULL,NULL
WHERE NOT EXISTS (SELECT * FROM ZNodePaymentSettingCredential WHERE PaymentSettingId=(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting 
											WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD')
										AND PaymentGatewayId=(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='authorizenet')
	AND PaymentCode='Credit'))

INSERT INTO ZNodePaymentSettingCredential (Partner,Vendor,TestMode,PaymentSettingId,GatewayUsername,GatewayPassword,TransactionKey,
	CreatedDate,ModifiedDate,Custom1,Custom2,Custom3,Custom4,Custom5)
SELECT '','',1,
(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD')
	AND PaymentGatewayId=(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='cybersource')
	AND PaymentCode='card1234'),
'','','',GETDATE(),GETDATE(),NULL,NULL,NULL,NULL,NULL
WHERE NOT EXISTS (SELECT * FROM ZNodePaymentSettingCredential WHERE PaymentSettingId=(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting 
											WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD')
										AND PaymentGatewayId=(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='cybersource')
	AND PaymentCode='card1234'))

INSERT INTO ZNodePaymentSettingCredential (Partner,Vendor,TestMode,PaymentSettingId,GatewayUsername,GatewayPassword,TransactionKey,
	CreatedDate,ModifiedDate,Custom1,Custom2,Custom3,Custom4,Custom5)
SELECT '','',1,
(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD')
	AND PaymentGatewayId=(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='stripe')
	AND PaymentCode='Stripe'),
'','','',GETDATE(),GETDATE(),NULL,NULL,NULL,NULL,NULL
WHERE NOT EXISTS (SELECT * FROM ZNodePaymentSettingCredential WHERE PaymentSettingId=(SELECT TOP 1 PaymentSettingId FROM ZNodePaymentSetting 
											WHERE PaymentTypeId=(SELECT TOP 1 PaymentTypeId FROM ZnodePaymentType WHERE Code ='CREDITCARD')
										AND PaymentGatewayId=(SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway WHERE GatewayCode='stripe')
	AND PaymentCode='Stripe'))

Update ZnodePaymentSetting set IsActive = 0
where isnull(PaymentGatewayId,0) in (select PaymentGatewayId from ZnodePaymentGateway WHERE GatewayCode = 'cybersource')

-- ZPD-21236 Dt: 22-Sep-2022
INSERT INTO ZNodePaymentGateway
   ([GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate],[GatewayCode])
SELECT
    N'PaypalExpress', N'http://www.paypal.com', N'PaypalExpressRestProvider', GETDATE(), GETDATE(), N'paypalexpress'
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZNodePaymentGateway 
                WHERE [GatewayCode] = 'paypalexpress')
