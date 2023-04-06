CREATE PROCEDURE [dbo].[Znode_InsertUpdate_SalesRepCustomerUserPortal]
(
	@SalesRepUserId Int,
	@CustomerUserId Int,
	@CustomerPortalId int,
	@UserId int,
	@Status bit = 0 out 
)
as
begin

	set nocount on
	declare @GetDate datetime =dbo.Fn_GetDate()
	declare @SalesRepPortalId int = 0
	declare @UserPortalId int

	begin try
	begin tran
		if @SalesRepPortalId = 0 and isnull(@CustomerPortalId,0)=0 ----all portal
		begin
			select @UserPortalId = UserPortalId,@SalesRepPortalId = PortalId 
			from ZnodeUserPortal where UserId = @SalesRepUserId and PortalId is null

			if @SalesRepPortalId = 0 and not exists(select top 1 PortalId from ZnodeUserPortal where UserId = @SalesRepUserId  and PortalId is null )
			begin
				set @SalesRepPortalId = (select top 1 PortalId from ZnodeUserPortal where UserId = @SalesRepUserId )
				set @UserPortalId = (select top 1 UserPortalId from ZnodeUserPortal where UserId = @SalesRepUserId and PortalId = @SalesRepPortalId)
			end
		end
		else if @SalesRepPortalId = 0 and isnull(@CustomerPortalId,0) <> 0 
		begin
			select @UserPortalId=UserPortalId, @SalesRepPortalId=PortalId from ZnodeUserPortal where UserId = @SalesRepUserId and (PortalId = @CustomerPortalId or PortalId is null)
		end

		update SRCUP 
		set SalesRepUserId = @SalesRepUserId, UserPortalId = @UserPortalId
		from ZnodeSalesRepCustomerUserPortal SRCUP
		where CustomerUserid = @CustomerUserId and isnull(CustomerPortalId,0) = isnull(@CustomerPortalId,0)

		if isnull(@CustomerPortalId,0) = 0 and exists(select *  from ZnodeSalesRepCustomerUserPortal where CustomerUserid = @CustomerUserId and CustomerPortalId is not null)
		begin
			delete from ZnodeSalesRepCustomerUserPortal
			where CustomerUserid = @CustomerUserId and CustomerPortalId is not null
		end

		if isnull(@CustomerPortalId,0) <> 0 and exists(select *  from ZnodeSalesRepCustomerUserPortal where CustomerUserid = @CustomerUserId and CustomerPortalId is null)
		begin
			delete from ZnodeSalesRepCustomerUserPortal
			where CustomerUserid = @CustomerUserId and CustomerPortalId is null
		end

		insert into ZnodeSalesRepCustomerUserPortal (UserPortalId,SalesRepUserId,CustomerUserid,CustomerPortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		select UP.UserPortalId, UP.UserId as SalesRepUserId,@CustomerUserId as CustomerUserid, case when @CustomerPortalId = 0 then null else @CustomerPortalId end as CustomerPortalId,
			   @UserId, @GetDate, @UserId, @GetDate
		from ZnodeUserPortal UP
		where UP.UserPortalId = @UserPortalId 
		and not exists(select * from ZnodeSalesRepCustomerUserPortal SRCUP where SRCUP.UserPortalId = UP.UserPortalId and SRCUP.CustomerUserid = @CustomerUserId and isnull(SRCUP.CustomerPortalId,0) = isnull(@CustomerPortalId,0))
	
	SET @Status = 1;
	SELECT 1 AS ID,@Status AS Status; 
	commit tran
	end try
	begin catch
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdate_SalesRepCustomerUserPortal @SalesRepUserId ='+CAST(@SalesRepUserId AS VARCHAR(50))+',@CustomerUserId = '+CAST(@CustomerUserId AS VARCHAR(50))+',@CustomerPortalId='+CAST(@CustomerPortalId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,@Status AS Status;                     
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName    = 'Znode_InsertUpdate_SalesRepCustomerUserPortal',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage     = @ErrorMessage,
				@ErrorLine        = @ErrorLine,
				@ErrorCall        = @ErrorCall;
			rollback tran
	end catch

end