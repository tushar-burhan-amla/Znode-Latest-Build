CREATE PROCEDURE [dbo].[Znode_InsertUpdate_UserPortal]
(
	@UserId Int,
	@PortalId Varchar(2000),
	@Status bit = 0 out 
)
as
begin
	set nocount on
	declare @PortalId_Tbl table(PortalId int)
	declare @GetDate datetime =dbo.Fn_GetDate()

	begin try
	begin tran
		insert into @PortalId_Tbl(PortalId)
		select Item from dbo.Split(@PortalId,',')

		if ISNULL(@PortalId,'') = '' or @PortalId = '0'
		begin 

			insert into ZnodeUserPortal (UserId,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			select @UserId, case when PortalId = 0 then null else PortalId end , @UserId, @GetDate, @UserId, @GetDate
			from @PortalId_Tbl PT
			where not exists(select * from ZnodeUserPortal  where isnull(ZnodeUserPortal.PortalId,0) = isnull(PT.PortalId,0) and ZnodeUserPortal.UserId = @UserId )

			update ZnodeSalesRepCustomerUserPortal 
			set UserPortalId = (select top 1 UserPortalId from ZnodeUserPortal where UserId = @UserId and PortalId is null)
			where SalesRepUserId = @UserId
			and exists (select top 1 UserPortalId from ZnodeUserPortal where UserId = @UserId and PortalId is null)

			delete from ZnodeSalesRepCustomerUserPortal 
			where exists(select *  from ZnodeUserPortal 
					where ZnodeUserPortal.UserId = @UserId --and PortalId is not null
					and not exists(select * from @PortalId_Tbl PT where isnull(ZnodeUserPortal.PortalId,0) = isnull(PT.PortalId,0) )
					and ZnodeSalesRepCustomerUserPortal.UserPortalId = ZnodeUserPortal.UserPortalId)

			delete from ZnodeUserPortal 
			where ZnodeUserPortal.UserId = @UserId --and PortalId is not null
			and not exists(select * from @PortalId_Tbl PT where isnull(ZnodeUserPortal.PortalId,0) = isnull(PT.PortalId,0) )

		end
		else if isnull(@PortalId,'0') <> '0' or  ISNULL(@PortalId,'') <> ''
		begin
			
			insert into ZnodeUserPortal (UserId,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			select @UserId, PortalId, @UserId, @GetDate, @UserId, @GetDate
			from @PortalId_Tbl PT
			where not exists(select * from ZnodeUserPortal  where ZnodeUserPortal.PortalId = PT.PortalId and ZnodeUserPortal.UserId = @UserId )

			update SRCUP 
			set UserPortalId = ZUP.UserPortalId 
			from ZnodeSalesRepCustomerUserPortal SRCUP
			inner join ZnodeUserPortal ZUP on SRCUP.SalesRepUserId = ZUP.UserId and SRCUP.CustomerPortalId = ZUP.PortalId
			where SalesRepUserId = @UserId
			and exists (select * from @PortalId_Tbl PT where ZUP.PortalId = PT.PortalId )
			and isnull(ZUP.PortalId,0) <> 0 

			delete from ZnodeSalesRepCustomerUserPortal 
			where exists(select *  from ZnodeUserPortal 
					where ZnodeUserPortal.UserId = @UserId
					and not exists(select * from @PortalId_Tbl PT where isnull(ZnodeUserPortal.PortalId,0) = isnull(PT.PortalId,0) )
					and ZnodeSalesRepCustomerUserPortal.UserPortalId = ZnodeUserPortal.UserPortalId)

			delete from ZnodeUserPortal 
			where ZnodeUserPortal.UserId = @UserId
			and not exists(select * from @PortalId_Tbl PT where isnull(ZnodeUserPortal.PortalId,0) = isnull(PT.PortalId,0) )

		end

		SET @Status = 1;
		SELECT 1 AS ID,@Status AS Status;    
	commit tran
	end try
	begin catch
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdate_UserPortal @UserId='+CAST(@UserId AS VARCHAR(50))+',@PortalId='+CAST(@PortalId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,@Status AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName    = 'Znode_InsertUpdate_UserPortal',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage     = @ErrorMessage,
				@ErrorLine        = @ErrorLine,
				@ErrorCall        = @ErrorCall;
			rollback tran
	end catch
end