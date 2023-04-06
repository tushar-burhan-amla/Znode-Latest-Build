INSERT INTO ZnodeGlobalAttributeFamily(FamilyCode, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsSystemDefined, GlobalEntityId)
SELECT 'Store', 2, getdate(), 2, getdate(), 1, 1
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store')

INSERT INTO ZnodeGlobalAttributeFamily(FamilyCode, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsSystemDefined, GlobalEntityId)
SELECT 'User', 2, getdate(), 2, getdate(), 1, 2
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User')

INSERT INTO ZnodeGlobalAttributeFamily(FamilyCode, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsSystemDefined, GlobalEntityId)
SELECT 'Account', 2, getdate(), 2, getdate(), 1, 3
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Account')

if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'ZnodeGlobalAttributeFamily' and COLUMN_NAME = 'GlobalEntityId')
BEGIN
ALTER TABLE ZnodeGlobalAttributeFamily ADD CONSTRAINT FK_ZnodeGlobalAttributeFamily_ZnodeGlobalEntity
FOREIGN KEY (GlobalEntityId) REFERENCES ZnodeGlobalEntity(GlobalEntityId);
END