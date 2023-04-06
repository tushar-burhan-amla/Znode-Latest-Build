

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