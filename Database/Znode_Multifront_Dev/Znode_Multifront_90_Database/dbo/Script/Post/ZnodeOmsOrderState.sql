--dt 17-01-2020 ZPD-8160 --> ZPD-8726
update ZnodeOMSOrderState set IsAccountStatus =0,isOrderstate=1,isorderlineitemstate=1
where OrderStateName = 'PENDING APPROVAL'

--dt 22-01-2020 ZPD-8160 --> ZPD-8726
update ZnodeOMSOrderState set IsAccountStatus =1,isOrderstate=1,isorderlineitemstate=1
where OrderStateName = 'PENDING APPROVAL'

--dt 30-03-2020 ZPD-7723 --> ZPD-9367
GO
DISABLE TRIGGER Trg_ZNodeCountry_GlobalSetting ON ZnodeCountry;
go
update ZnodeCountry set CountryName = [dbo].[Fn_CamelCase](CountryName)

update ZnodeOmsOrderState set Description = [dbo].[Fn_CamelCase](Description)

update ZnodeOmsOrderStateShowToCustomer set Description = [dbo].[Fn_CamelCase](Description)
go
update ZnodeOmsPaymentState set Name = [dbo].[Fn_CamelCase](Name)
go
update ZnodeState set StateName = [dbo].[Fn_CamelCase](StateName)
go
enable TRIGGER Trg_ZNodeCountry_GlobalSetting ON ZnodeCountry;

--dt 29-05-2020 ZPD-10241
insert into ZnodeOmsOrderState (OmsOrderStateId,	OrderStateName,	IsShowToCustomer,	IsAccountStatus,	DisplayOrder,
Description,	IsEdit,	IsSendEmail,	IsOrderState,	IsOrderLineItemState,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	IsQuoteState)
select 200,	'EXPIRED',	0,	0	,99,	'Expired',	0,	0,	0,	0,	2,	'2020-05-05 16:10:46.490',	2,	'2020-05-05 16:10:46.490',	1
where not exists(select * from ZnodeOmsOrderState where OmsOrderStateId = 200 and OrderStateName = 'EXPIRED')

update ZnodeOmsOrderState set ISQuoteState = 1 where OrderStateName in ('Submitted','In Review','Expired','Cancelled','Approved')
and ISQuoteState is null

--dt 17-06-2020 ZPD-10236 --> ZPD-9739
update ZnodeOMSOrderState set Description = 'In Review' where OrderStateName = 'IN REVIEW' 

update ZnodeOmsOrderState
set IsOrderLineItemState =0
where OrderStateName in ('RETURNED','PARTIAL REFUND')

--ZPD-16462
Update ZnodeOmsOrderState set OrderStateName = 'CANCELED' , Description = 'Canceled' where OrderStateName = 'CANCELLED'