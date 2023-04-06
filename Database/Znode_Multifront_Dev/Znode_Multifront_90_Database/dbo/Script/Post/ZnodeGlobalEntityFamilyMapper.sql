INSERT INTO ZnodeGlobalEntityFamilyMapper (GlobalAttributeFamilyId, GlobalEntityId, GlobalEntityValueId)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Store'),NULL
WHERE NOT EXISTS (SELECT * FROM  ZnodeGlobalEntityFamilyMapper WHERE 
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') AND
GlobalEntityId = (SELECT top 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Store')
)


INSERT INTO ZnodeGlobalEntityFamilyMapper (GlobalAttributeFamilyId, GlobalEntityId, GlobalEntityValueId)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Account'),
(SELECT top 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'),NULL
WHERE NOT EXISTS (SELECT * FROM  ZnodeGlobalEntityFamilyMapper WHERE 
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Account') AND
GlobalEntityId = (SELECT top 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')
)

INSERT INTO ZnodeGlobalEntityFamilyMapper (GlobalAttributeFamilyId, GlobalEntityId, GlobalEntityValueId)
SELECT 
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User'),
(SELECT top 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'User'),NULL
WHERE NOT EXISTS (SELECT * FROM  ZnodeGlobalEntityFamilyMapper WHERE 
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'User') AND
GlobalEntityId = (SELECT top 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'User')
)