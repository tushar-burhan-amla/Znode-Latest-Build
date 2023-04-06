
INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'WebstoreAuthentication'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'WebstoreAuthentication')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'Redirections'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'Redirections')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'EnableBudgetManagement'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'EnableBudgetManagement')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'StoreAddressSettings'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'StoreAddressSettings')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'Captcha'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'Captcha')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'WarehouseStockLevels'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'WarehouseStockLevels')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'Cloudflaresetting'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'Cloudflaresetting')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'QuotesSettings'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'QuotesSettings')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'VoucherSettings'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'VoucherSettings')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'ContentSecurityPolicy'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'ContentSecurityPolicy')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'Budgets'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'Budgets')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'OpenAccountBillingDetails'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'OpenAccountBillingDetails')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'UserAddressSettings'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'UserAddressSettings')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'PowerBISettings'), 999,
 2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'PowerBISettings')
)