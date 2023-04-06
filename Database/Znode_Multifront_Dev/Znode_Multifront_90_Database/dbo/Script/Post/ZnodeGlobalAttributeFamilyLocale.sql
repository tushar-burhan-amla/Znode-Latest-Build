INSERT INTO ZnodeGlobalAttributeFamilyLocale (LocaleId, GlobalAttributeFamilyId, AttributeFamilyName, Description, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 1,
(select top 1 GlobalAttributeFamilyId from ZnodeGlobalAttributeFamily where FamilyCode = 'Store'),
'Default Store Family', NULL,  2, getdate(), 2, getdate()
WHERE NOT EXISTS (SELECT * FROM  ZnodeGlobalAttributeFamilyLocale WHERE 
GlobalAttributeFamilyId = (select top 1 GlobalAttributeFamilyId from ZnodeGlobalAttributeFamily where FamilyCode = 'Store') AND
LocaleId = 1)

INSERT INTO ZnodeGlobalAttributeFamilyLocale (LocaleId, GlobalAttributeFamilyId, AttributeFamilyName, Description, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 1,
(select top 1 GlobalAttributeFamilyId from ZnodeGlobalAttributeFamily where FamilyCode = 'Account'),
'Default Account Family', NULL,  2, getdate(), 2, getdate()
WHERE NOT EXISTS (SELECT * FROM  ZnodeGlobalAttributeFamilyLocale WHERE 
GlobalAttributeFamilyId = (select top 1 GlobalAttributeFamilyId from ZnodeGlobalAttributeFamily where FamilyCode = 'Account') AND
LocaleId = 1)

INSERT INTO ZnodeGlobalAttributeFamilyLocale (LocaleId, GlobalAttributeFamilyId, AttributeFamilyName, Description, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 1,
(select top 1 GlobalAttributeFamilyId from ZnodeGlobalAttributeFamily where FamilyCode = 'User'),
'Default User Family', NULL,  2, getdate(), 2, getdate()
WHERE NOT EXISTS (SELECT * FROM  ZnodeGlobalAttributeFamilyLocale WHERE 
GlobalAttributeFamilyId = (select top 1 GlobalAttributeFamilyId from ZnodeGlobalAttributeFamily where FamilyCode = 'User') AND
LocaleId = 1)