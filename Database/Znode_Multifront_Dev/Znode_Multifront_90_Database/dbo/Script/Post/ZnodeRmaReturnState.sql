
INSERT [dbo].[ZnodeRmaReturnState] ([RmaReturnStateId], [ReturnStateName], [DisplayOrder], [Description], [IsReturnState], [IsReturnLineItemState], [IsSendEmail], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate])
select 10, N'SUBMITTED', 1, N'SUBMITTED', 1, 1, 1, 2, getdate(), 2, getdate()
where not exists(select * from [ZnodeRmaReturnState] where [RmaReturnStateId]= 10)
GO
INSERT [dbo].[ZnodeRmaReturnState] ([RmaReturnStateId], [ReturnStateName], [DisplayOrder], [Description], [IsReturnState], [IsReturnLineItemState], [IsSendEmail], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
select 20, N'NOT SUBMITTED', 2, N'NOT SUBMITTED', 1, 1, 0, 2, getdate(), 2, getdate()
where not exists(select * from [ZnodeRmaReturnState] where [RmaReturnStateId] = 20)
GO
GO
INSERT [dbo].[ZnodeRmaReturnState] ([RmaReturnStateId], [ReturnStateName], [DisplayOrder], [Description], [IsReturnState], [IsReturnLineItemState], [IsSendEmail], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
 select 30, N'IN REVIEW', 3, N'IN REVIEW', 1, 0, 1, 2, getdate(), 2, getdate()
where not exists(select * from [ZnodeRmaReturnState] where [RmaReturnStateId] = 30)
GO
INSERT [dbo].[ZnodeRmaReturnState] ([RmaReturnStateId], [ReturnStateName], [DisplayOrder], [Description], [IsReturnState], [IsReturnLineItemState], [IsSendEmail], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
select 40, N'RECEIVED', 4, N'RECEIVED', 1, 1, 1,2, getdate(), 2, getdate()
where not exists(select * from [ZnodeRmaReturnState] where [RmaReturnStateId] = 40)
GO
INSERT [dbo].[ZnodeRmaReturnState] ([RmaReturnStateId], [ReturnStateName], [DisplayOrder], [Description], [IsReturnState], [IsReturnLineItemState], [IsSendEmail], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
select 50, N'APPROVED', 5, N'APPROVED', 1, 1, 1, 2, getdate(), 2, getdate()
where not exists(select * from [ZnodeRmaReturnState] where [RmaReturnStateId] = 50)
GO
INSERT [dbo].[ZnodeRmaReturnState] ([RmaReturnStateId], [ReturnStateName], [DisplayOrder], [Description], [IsReturnState], [IsReturnLineItemState], [IsSendEmail], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
select 60, N'REJECTED', 6, N'REJECTED', 1, 1,1, 2, getdate(), 2, getdate()
where not exists(select * from [ZnodeRmaReturnState] where [RmaReturnStateId] = 60)
GO
INSERT [dbo].[ZnodeRmaReturnState] ([RmaReturnStateId], [ReturnStateName], [DisplayOrder], [Description], [IsReturnState], [IsReturnLineItemState], [IsSendEmail], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
select 70, N'PARTIALLY APPROVED', 7, N'PARTIALLY APPROVED', 1, 1, 1, 2, getdate(), 2, getdate()
where not exists(select * from [ZnodeRmaReturnState] where [RmaReturnStateId] = 70)
GO
INSERT [dbo].[ZnodeRmaReturnState] ([RmaReturnStateId], [ReturnStateName], [DisplayOrder], [Description], [IsReturnState], [IsReturnLineItemState], [IsSendEmail], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
select 80, N'REFUND PROCESSED', 8, N'REFUND PROCESSED', 1, 0, 1, 2, getdate(), 2, getdate()
where not exists(select * from [ZnodeRmaReturnState] where [RmaReturnStateId] = 80)
go

Update znodermareturnstate set [Description] = dbo.Fn_CamelCase([Description])

--dt 03-07-2020 ZPD-11260 
update ZnodeRmaReturnState set IsReturnState = 0, IsReturnLineItemState = 0
where RmaReturnStateId = 70

--dt 06-07-2020 ZPD-10809
update [ZnodeRmaReturnState] set IsReturnLineItemState = 1 where RmaReturnStateId = 30

update [ZnodeRmaReturnState] set IsReturnLineItemState = 0 where RmaReturnStateId = 20

--dt 09-07-2020 ZPD-11385
update ZnodeRmaReturnState set ReturnStateName = dbo.Fn_CamelCase(ReturnStateName)