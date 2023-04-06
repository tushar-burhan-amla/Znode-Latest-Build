
----dt 19-12-2019 ZPD-7550 --> ZPD-8289
;with cte as
(
	select * from ZnodePortalPageSetting zpps
	where IsDefault = 0 
	and not exists(select * from ZnodePortalPageSetting zpps1 where zpps.PortalId = zpps1.PortalId and IsDefault = 1)
)
update a set IsDefault = 1 
from ZnodePortalPageSetting a
inner join cte b on a.PortalId = b.PortalId
where not exists (select top 1 PageSettingId from cte zps where a.PortalId = zps.PortalId 
				and zps.PageSettingId = (select top 1 PageSettingId from ZnodePageSetting where PageName = 'Show 16') )
and a.PageSettingId = (select min(c.PageSettingId) from cte c where a.PortalId = c.PortalId )

;with cte as
(
	select * from ZnodePortalPageSetting zpps
	where IsDefault = 0 
	and not exists(select * from ZnodePortalPageSetting zpps1 where zpps.PortalId = zpps1.PortalId and IsDefault = 1)
)
update a set IsDefault = 1 
from ZnodePortalPageSetting a
inner join cte b on a.PortalId = b.PortalId
where a.PageSettingId = (select top 1 PageSettingId from ZnodePageSetting where PageName = 'Show 16') 

insert into ZnodePortalPageSetting(PortalId,PageSettingId,PageDisplayName,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDefault)
select PortalId,PageSettingId,
PageName,1,2,getdate(),2,getdate(),case when PageName = 'Show 16' then 1 else 0 end
from ZnodePortal ZP
cross apply ZnodePageSetting zps 
where not exists(select * from ZnodePortalPageSetting ZPPS where ZP.PortalId = ZPPS.PortalId)