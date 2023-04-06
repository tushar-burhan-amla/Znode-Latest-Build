INSERT INTO ZnodeGlobalEntity(EntityName, IsActive, TableName, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsFamilyUnique)
SELECT 'Content Containers', 1, 'ZnodeWidgetGlobalAttributeValue', 2, getdate(), 2, getdate(), 0
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers')

Update ZnodeGlobalEntity set IsFamilyUnique = 1 where EntityName = 'Store' or EntityName = 'Account' or EntityName = 'User'

Update ZnodeGlobalEntity set IsFamilyUnique = 0 where EntityName = 'Content Containers' 


if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'ZnodeGlobalEntity' and COLUMN_NAME = 'IsFamilyUnique')
begin
alter table ZnodeGlobalEntity alter column [IsFamilyUnique]  BIT NOT NULL 
end

