if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'znodeGiftcard' and COLUMN_NAME = 'RestrictToCustomerAccount')
begin
	update znodeGiftcard set RestrictToCustomerAccount = 0 where RestrictToCustomerAccount is null
end
go
if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'znodeGiftcard' and COLUMN_NAME = 'RestrictToCustomerAccount')
begin
	alter table ZnodeGiftCard alter column RestrictToCustomerAccount BIT not null
end
go
if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'znodeGiftcard' and COLUMN_NAME = 'StartDate')
begin
	update znodeGiftcard set StartDate = getdate()  where StartDate is null
end
go
if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'znodeGiftcard' and COLUMN_NAME = 'StartDate')
begin
	alter table znodeGiftcard alter column StartDate Datetime not null
end
go


if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'znodeGiftcard' and COLUMN_NAME = 'IsActive')
begin
	update znodeGiftcard set IsActive = 0 where IsActive is null
end
go
if exists(select * from INFORMATION_SCHEMA.columns where TABLE_NAME = 'znodeGiftcard' and COLUMN_NAME = 'IsActive')
begin
	alter table ZnodeGiftCard alter column IsActive BIT not null
end
go