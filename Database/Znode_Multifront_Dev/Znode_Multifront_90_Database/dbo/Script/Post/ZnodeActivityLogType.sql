

--dt 20-01-2020 ZPD-8291 --> ZPD-8921
declare @ActivityLogTypeId int
select @ActivityLogTypeId = max(ActivityLogTypeId) from ZnodeActivityLogType
insert ZnodeActivityLogType(ActivityLogTypeId, [Name],TypeCategory)
select @ActivityLogTypeId+1,'CLOUDFLARE_PURGE_ALL','CLOUDFLARE'
where not exists(select * from ZnodeActivityLogType where [Name] = 'CLOUDFLARE_PURGE_ALL')

insert ZnodeActivityLogType(ActivityLogTypeId, [Name],TypeCategory)
select @ActivityLogTypeId+2,'CLOUDFLARE_PURGE_CUSTOM','CLOUDFLARE'
where not exists(select * from ZnodeActivityLogType where [Name] = 'CLOUDFLARE_PURGE_CUSTOM')