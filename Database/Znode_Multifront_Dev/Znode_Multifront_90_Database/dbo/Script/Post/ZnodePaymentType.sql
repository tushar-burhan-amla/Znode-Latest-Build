--ZPD-16313
UPDATE ZnodePaymentType SET BehaviorType='COD' WHERE Code = 'INVOICEME' AND BehaviorType <> 'COD'


insert into ZnodePaymentType (Code,Name,Description,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsCallToPaymentAPI,BehaviorType)
select 'AUTOMATEDCLEARINGHOUSE','ACH','Automated Clearing House',1,2,getdate(),2,getdate(),1,'ACH'
where not exists(select * from ZnodePaymentType where Code = 'AUTOMATEDCLEARINGHOUSE')

---ZPD-18048---
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsUsedForOfflinePayment' AND TABLE_NAME = 'ZnodePaymentType')
BEGIN
        ALTER TABLE ZnodePaymentType ADD IsUsedForOfflinePayment BIT NOT NULL DEFAULT ((0))
END

---ZPD-18048---
UPDATE ZnodePaymentType SET IsActive=1 WHERE Code IN ('CREDITCARD','AUTOMATEDCLEARINGHOUSE')

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'IsUsedForOfflinePayment' AND TABLE_NAME = 'ZnodePaymentType')
BEGIN
        ALTER TABLE ZnodePaymentType ALTER COLUMN IsUsedForOfflinePayment BIT NOT NULL
END
