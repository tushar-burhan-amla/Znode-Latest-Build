
----dt- 10/03/2019 ZPD-7415 and ZPD-7141
if not exists(select * from sys.indexes where name = 'ZnodeUserPortal_UserId')
begin 
CREATE NONCLUSTERED INDEX [ZnodeUserPortal_UserId] ON [dbo].[ZnodeUserPortal] ([UserId])
end

--dt 21-02-2020 ZPD-9146
delete from ZnodeUserProfile where UserId = 2

--dt 18-12-2020 ZPD-13064
update a set a.UserName = c.UserName 
from znodeuser a
inner join AspNetUsers b on a.AspNetUserId = b.id
inner join AspNetZnodeUser c on b.username = c.AspNetZnodeUserId
where a.UserName is null