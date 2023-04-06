CREATE PROCEDURE [dbo].[Znode_GetEligibleOrderNumberListForReturn]
( @UserId			int,
  @PortalId         INT,
  @MaxDays          INT,
  @OrderNumber      varchar(200) ='',
  @WhereClause		varchar(2000)= '',
  @PageSize			Int = 50
)
AS
/*
     Summary:- This Procedure is used to Get Eligible OrderNumber List For Return
     Unit Testing 
	 begin tran
     EXEC Znode_GetEligibleOrderNumberListForReturn @UserId=2,@PortalId=7,@MaxDays=90,@OrderNumber='',@WhereClause='',@PageSize=10
	 rollback tran
*/
     BEGIN
         BEGIN TRY
            SET NOCOUNT ON;
			DECLARE @SQL NVARCHAR(MAX), @OrderLineItemRelationshipTypeId int
			DECLARE @GetDate DATE = dbo.Fn_GetDate();

			select @OrderLineItemRelationshipTypeId = OrderLineItemRelationshipTypeId 
			from ZnodeOmsOrderLineItemRelationshipType where Name = 'Bundles' 
			
			select OmsReturnOrderLineItemsId,RRD.OmsOrderId, RRD.OrderNumber, RRD.OmsOrderDetailsId, RRLI.OmsOrderLineItemsId , 
			      sum(case when RRD.RmaReturnStateId  in (50,60,70,80) then isnull(RRLI.ReturnedQuantity,0) else isnull(RRLI.ExpectedReturnQuantity,0) end) ReturnQuantity
			into #RmaReturnDetails
			from ZnodeRmaReturnDetails RRD
			inner join ZnodeRmaReturnLineItems RRLI on RRD.RmaReturnDetailsId = RRLI.RmaReturnDetailsId
			WHERE 
			--case when RRD.RmaReturnStateId  in (50,70,80) then RRLI.RmaReturnStateId else 0 end <> case when RRD.RmaReturnStateId  in (50,70,80) then 60 else 1 end and
			OmsReturnOrderLineItemsId is null AND RRD.RmaReturnStateId  <> 20
			group by RRD.OmsOrderId, RRD.OrderNumber, RRD.OmsOrderDetailsId, RRLI.OmsOrderLineItemsId,OmsReturnOrderLineItemsId


			----considering child entry other than bundle products
			select distinct zo.OrderNumber, zo.CreatedDate  
			into #cte_OrderData
			from ZnodeOmsOrder zo
			inner join ZnodeOmsOrderDetails zod on zo.OmsOrderId = zod.OmsOrderId 
			inner join ZnodeOmsOrderLineItems zoli on zoli.OmsOrderDetailsId = zod.OmsOrderDetailsId
			left join ZnodeRmaReturnDetails zrd on zo.OmsOrderId = zrd.OmsOrderId 
			and RmaReturnStateId = (select RmaReturnStateId from ZnodeRmaReturnState where RmaReturnStateId = 60)
			where  zod.UserId = @UserId and zod.PortalId = @PortalId and zod.IsActive = 1
			and case when zoli.ShipDate is not null and zoli.OrderLineItemStateId = (Select top 1 OmsOrderStateId from ZnodeOmsOrderState where OrderStateName='Shipped') then CAST(DATEADD(DD,@MaxDays,zoli.ShipDate) AS Date) else CAST(DATEADD(DD,@MaxDays,zod.OrderDate) AS Date) end > @GetDate
			and zoli.OrderLineItemStateId in (SELECT OmsOrderStateId FROM ZnodeOmsOrderState ZOS where ZOS.OmsOrderStateId in(10, 20) ) 
			and zoli.OrderLineItemRelationshipTypeId is not NULL and zoli.OrderLineItemRelationshipTypeId <> @OrderLineItemRelationshipTypeId
			and zoli.IsActive=1 and (zo.OrderNumber = @OrderNumber or @OrderNumber = '')
			and not exists(SELECT * FROM ZnodeOmsOrderState ZOS where ZOS.OmsOrderStateId in (40,140) and zod.OmsOrderStateId = ZOS.OmsOrderStateId)  
			and not exists(select * from #RmaReturnDetails RD where zod.OmsOrderDetailsId = RD.OmsOrderDetailsId AND RD.OmsOrderLineItemsId=zoli.OmsOrderLineItemsId and zoli.Quantity = RD.ReturnQuantity)
			and not exists(select * from ZnodeOMSOrderLineItemRelationshipType Addon where Name = 'AddOns' and Addon.OrderLineItemRelationshipTypeId = zoli.OrderLineItemRelationshipTypeId) 
			
			-----Considering only parent entry in return for bundle products
			insert into #cte_OrderData
			select distinct zo.OrderNumber, zo.CreatedDate  
			from ZnodeOmsOrder zo
			inner join ZnodeOmsOrderDetails zod on zo.OmsOrderId = zod.OmsOrderId 
			inner join ZnodeOmsOrderLineItems zoli on zoli.OmsOrderDetailsId = zod.OmsOrderDetailsId
			inner join ZnodeOmsOrderLineItems zoli1 on zoli1.ParentOmsOrderLineItemsId = zoli.OmsOrderLineItemsId
			left join ZnodeRmaReturnDetails zrd on zo.OmsOrderId = zrd.OmsOrderId 
			and RmaReturnStateId = (select RmaReturnStateId from ZnodeRmaReturnState where RmaReturnStateId = 60)
			where  zod.UserId = @UserId and zod.PortalId = @PortalId and zod.IsActive = 1
			and case when zoli.ShipDate is not null and zoli.OrderLineItemStateId = (Select top 1 OmsOrderStateId from ZnodeOmsOrderState where OrderStateName='Shipped') then CAST(DATEADD(DD,@MaxDays,zoli.ShipDate) AS Date) else CAST(DATEADD(DD,@MaxDays,zod.OrderDate) AS Date) end > @GetDate
			and zoli.OrderLineItemStateId in (SELECT OmsOrderStateId FROM ZnodeOmsOrderState ZOS where ZOS.OmsOrderStateId in(10, 20) ) 
			and zoli.OrderLineItemRelationshipTypeId is NULL and isnull(zoli1.OrderLineItemRelationshipTypeId,0) = @OrderLineItemRelationshipTypeId
			and zoli.IsActive=1 and (zo.OrderNumber = @OrderNumber or @OrderNumber = '')
			and not exists(SELECT * FROM ZnodeOmsOrderState ZOS where ZOS.OmsOrderStateId in (40,140) and zod.OmsOrderStateId = ZOS.OmsOrderStateId)  
			and not exists(select * from #RmaReturnDetails RD where zod.OmsOrderDetailsId = RD.OmsOrderDetailsId AND RD.OmsOrderLineItemsId=zoli.OmsOrderLineItemsId and zoli.Quantity = RD.ReturnQuantity)
			and not exists(select * from #cte_OrderData OD where zo.OrderNumber = OD.OrderNumber )

			set @SQL ='
			Select top '+cast(@PageSize as varchar(10))+' OrderNumber
			from #cte_OrderData
			WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+'
			order by CreatedDate desc'
			print @SQL
			EXEC (@SQL)

         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
			 --select Error_message(), Error_line();
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetEligibleOrderNumberListForReturn @UserId = '+ cast (@UserId as varchar(10)) +',@PortalId='+cast (@PortalId as varchar(10)) +',@MaxDays='+cast (@MaxDays as varchar(10)) +',@OrderNumber='+@OrderNumber;
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetEligibleOrderNumberListForReturn',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
