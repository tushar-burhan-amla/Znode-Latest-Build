CREATE PROCEDURE [dbo].[Znode_GetZnodePortalProfile](
	  @UserId INT = null)

AS 
/*
Summary:
EXEC Znode_GetZnodePortalProfile 11

*/


begin
    set nocount on
	begin try
	

declare @tbl_Portalmatch table(PortalId int,ShippingId int)
declare @tbl_Profilematch table(ProfileId int,ShippingId int)

insert into @tbl_Portalmatch
select a.PortalId,b.ShippingId from ZnodeUserPortal a 
inner join ZnodePortalShipping b on (a.PortalId = b.PortalId)
where UserId = 11

insert into @tbl_Profilematch
select a.ProfileId,b.ShippingId from ZnodeUserProfile a 
inner join ZnodeProfileShipping b on (a.ProfileId = b.ProfileId)
where UserId = 11

--select * from @tbl_Portalmatch ta inner join @tbl_Profilematch tb on (ta.ShippingId = tb.ShippingId)
--where exists (select ShippingId from @tbl_Profilematch)

select ShippingId from @tbl_Profilematch
where exists (select * from @tbl_Portalmatch ta inner join @tbl_Profilematch tb on (ta.ShippingId = tb.ShippingId))

--select * from @tbl_Portalmatch ta inner join @tbl_Profilematch tb on (ta.ShippingId = tb.ShippingId)

if exists(select 1 from @tbl_Profilematch tpm
				 where tpm.ShippingId not in (select ta.ShippingId from @tbl_Portalmatch ta 
				 inner join @tbl_Profilematch tb on (ta.ShippingId = tb.ShippingId)))
begin

select '' as ShippingId from @tbl_Profilematch
end

else if not exists (select 1 from @tbl_Profilematch tprm
				 where tprm.ShippingId not in (select ta.ShippingId from @tbl_Portalmatch ta 
				 inner join @tbl_Profilematch tb on (ta.ShippingId = tb.ShippingId)))
begin
select * from @tbl_Portalmatch
end

	end try
	begin catch
		select error_message()
	end catch

end