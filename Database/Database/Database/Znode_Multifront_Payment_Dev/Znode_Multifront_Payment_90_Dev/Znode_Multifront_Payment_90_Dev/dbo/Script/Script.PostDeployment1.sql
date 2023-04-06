
DELETE FROM [dbo].[ZNodeDomain]
GO
DELETE FROM [dbo].[ZNodeActivityLog]
GO
DELETE FROM [dbo].[ZnodeTransactions]
GO
DELETE FROM [dbo].[ZNodePaymentSettingCredential]
GO
DELETE FROM [dbo].[ZnodePaymentMethods]
GO
DELETE FROM [dbo].[ZNodePaymentSetting]
GO
DELETE FROM [dbo].[ZNodePaymentType]
GO
DELETE FROM [dbo].[ZNodePaymentGateway]
GO
DELETE FROM [dbo].[ZnodePaymentCustomers]
GO
DELETE FROM [dbo].[ZnodePaymentAddress]
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentGateway] ON 
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (1, N'Authorize.Net', N'http://www.authorize.net', N'AuthorizeNetCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (4, N'Stripe', N'', N'StripeCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (5, N'Paypal', N'http://www.paypal.com', N'PaypalCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (6, N'Chase Paymentech', N'https://securevar.paymentech.com/manager', N'PaymentTechProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (7, N'WorldPay', N'http://www.wordlpay.com', N'SecureNetCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (8, N'Braintree', N'https://www.braintreepayments.com/', N'BraintreeProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
INSERT [dbo].[ZNodePaymentGateway] ([PaymentGatewayId], [GatewayName], [WebsiteURL], [ClassName], [CreatedDate], [ModifiedDate]) VALUES (9, N'PayFlow', N'http://www.manager.paypal.com', N'PayFlowCustomerProvider', CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentGateway] OFF
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentType] ON 
GO
INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (1, N'Credit Card', N'Credit Card', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (2, N'Purchase Order', N'Purchase Order', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (3, N'Paypal Express', N'Paypal Express', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
INSERT [dbo].[ZNodePaymentType] ([PaymentTypeId], [Name], [Description], [IsActive], [CreatedDate], [ModifiedDate]) VALUES (4, N'COD', N'Charge On Delivery', 1, CAST(N'2016-07-29T13:46:19.620' AS DateTime), CAST(N'2016-07-29T13:46:19.620' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentType] OFF
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentSetting] ON 
GO
INSERT [dbo].[ZNodePaymentSetting] ([PaymentSettingId], [PaymentTypeId], [PaymentGatewayId], [EnableVisa], [EnableMasterCard], [EnableAmex], [EnableDiscover], [EnableRecurringPayments], [EnableVault], [IsActive], [DisplayOrder], [PreAuthorize], [IsRMACompatible], [TestMode], [CreatedDate], [ModifiedDate], [EnablePODocUpload], [IsPODocRequired]) VALUES (1, 4, NULL, NULL, NULL, NULL, NULL, 0, 0, 1, 1, 0, NULL, 0, CAST(N'2017-03-31T00:00:00.000' AS DateTime), CAST(N'2017-03-31T00:00:00.000' AS DateTime), 0, 0)
GO
SET IDENTITY_INSERT [dbo].[ZNodePaymentSetting] OFF
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

GO
update ZNodePaymentGateway
set GatewayCode = LOWER(REPLACE(REPLACE(REPLACE(GatewayName, ' ', ''), '.', ''),'Chase',''))

GO

update ZnodePaymentSetting 
set PaymentCode = PaymentSettingId

GO

update ZNodePaymentType set Code ='CREDITCARD' where Name = 'CREDIT_CARD'
update ZNodePaymentType set Code ='PURCHASEORDER' where Name = 'PURCHASE_ORDER'
update ZNodePaymentType set Code ='PAYPALEXPRESS' where Name = 'PAYPAL_EXPRESS'
update ZNodePaymentType set Code ='CHARGEONDELIVERY' where Name = 'COD'
update ZNodePaymentType set Code ='AMAZONPAY' where Name = 'AMAZON_PAY'
update ZNodePaymentType set Code ='INVOICEME' where Name = 'Invoice Me'

GO
update ZnodepaymentType
set Behaviourtype = Name

GO

update ZNodePaymentType set Name = 'AmazonPay' where BehaviourType = 'AMAZON_PAY'
update ZNodePaymentType set Name = 'PaypalExpress' where BehaviourType = 'PAYPAL_EXPRESS'

GO

update ZNodePaymentType set BehaviourType = 'COD' where PaymentTypeId =6

go

UPDATE A
SET A.Vendor = A.GatewayUsername
FROM ZNodePaymentSettingCredential A
WHERE PaymentSettingId = (SELECT TOP 1 PaymentSettingId FROM ZnodePaymentSetting B
WHERE PaymentGatewayId = (SELECT TOP 1 PaymentGatewayId FROM ZnodePaymentGateway C
WHERE C.GatewayCode = 'payflow' AND C.PaymentGatewayId = B.PaymentGatewayId)
AND A.PaymentSettingId = B.PaymentSettingId)