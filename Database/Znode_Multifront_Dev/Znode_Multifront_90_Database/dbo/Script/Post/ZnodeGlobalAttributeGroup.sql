update ZnodeGlobalAttributeGroup set GlobalEntityId = s.GlobalEntityId from ZnodeGlobalGroupEntityMapper s
inner join ZnodeGlobalAttributeGroup t on s.GlobalAttributeGroupId = t.GlobalAttributeGroupId

update ZnodeGlobalAttribute set GlobalEntityId = s.GlobalEntityId from ZnodeGlobalAttributeGroup s inner join ZnodeGlobalAttributeGroupMapper t 
on s.GlobalAttributeGroupId = t.GlobalAttributeGroupId inner join ZnodeGlobalAttribute v on v.GlobalAttributeId = t.GlobalAttributeId


Declare @GlobalAttributeGroup_EnableECertificate int,@GlobalAttributeGroup_EProSettings int

select @GlobalAttributeGroup_EnableECertificate = GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'ECertificateSettings'
select @GlobalAttributeGroup_EProSettings = GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'EProSettings'

execute [Znode_DeleteGlobalAttributeGroup] @GlobalAttributeGroupId = @GlobalAttributeGroup_EnableECertificate,@Status = 0
execute [Znode_DeleteGlobalAttributeGroup] @GlobalAttributeGroupId = @GlobalAttributeGroup_EProSettings,@Status = 0

Update ZnodeGlobalAttribute Set GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store')
where GlobalEntityId is null

Update ZnodeGlobalAttributeGroup Set GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store')
where GlobalEntityId is null

if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'ZnodeGlobalAttributeGroup' and COLUMN_NAME = 'GlobalEntityId')
begin
alter table ZnodeGlobalAttributeGroup alter column GlobalEntityId INT NOT NULL
end

if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'ZnodeGlobalAttributeGroup' and COLUMN_NAME = 'GlobalEntityId')
and not exists(select*from sys.objects where name='FK_ZnodeGlobalAttributeGroup_ZnodeGlobalEntity')
BEGIN
ALTER TABLE ZnodeGlobalAttributeGroup ADD CONSTRAINT FK_ZnodeGlobalAttributeGroup_ZnodeGlobalEntity
FOREIGN KEY (GlobalEntityId) REFERENCES ZnodeGlobalEntity(GlobalEntityId);
END
--ZPD-11917
if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'ZnodeGlobalAttribute' and COLUMN_NAME = 'GlobalEntityId')
begin
alter table ZnodeGlobalAttribute alter column GlobalEntityId INT NOT NULL
end

if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'ZnodeGlobalAttribute' and COLUMN_NAME = 'GlobalEntityId')
and not exists(select*from sys.objects where name='FK_ZnodeGlobalAttribute_ZnodeGlobalEntity')
BEGIN
ALTER TABLE ZnodeGlobalAttribute
ADD CONSTRAINT FK_ZnodeGlobalAttribute_ZnodeGlobalEntity FOREIGN KEY (GlobalEntityId) REFERENCES ZnodeGlobalEntity(GlobalEntityId);
END
go
INSERT INTO ZnodeGlobalAttributeGroup(GroupCode, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsSystemDefined, GlobalEntityId)
SELECT 'ContentSecurityPolicy', null, 2, getdate(), 2, getdate(), 0, (select top 1 GlobalEntityId from znodeglobalentity  where EntityName = 'store')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'ContentSecurityPolicy')


--dt 21-09-2020 ZPD-12234 --> ZPD-11917
Declare @GlobalAttributeStoreVoucher int
select @GlobalAttributeStoreVoucher = b.GlobalAttributeId
from ZnodeGlobalAttributeGroup a
inner join ZnodeGlobalAttributeGroupMapper c on c.GlobalAttributeGroupId = a.GlobalAttributeGroupId  
inner join ZnodeGlobalAttribute b on c.GlobalAttributeId = b.GlobalAttributeId 
where a.GroupCode = 'VoucherSettings' and b.AttributeCode = 'VoucherExpirationReminderEmailInDays'
and b.GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity d where d.EntityName = 'Store')

Declare @GlobalAttributeAccountVoucher int
select @GlobalAttributeAccountVoucher = b.GlobalAttributeId
from ZnodeGlobalAttributeGroup a
inner join ZnodeGlobalAttributeGroupMapper c on c.GlobalAttributeGroupId = a.GlobalAttributeGroupId  
inner join ZnodeGlobalAttribute b on c.GlobalAttributeId = b.GlobalAttributeId 
where a.GroupCode = 'AccountVoucherSettings' and b.AttributeCode = 'AccountVoucherExpirationReminderEmailInDays'
and b.GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity d where d.EntityName = 'Account')

update ZnodeAccountGlobalAttributeValue set GlobalAttributeId = @GlobalAttributeAccountVoucher
where GlobalAttributeId = @GlobalAttributeStoreVoucher 
