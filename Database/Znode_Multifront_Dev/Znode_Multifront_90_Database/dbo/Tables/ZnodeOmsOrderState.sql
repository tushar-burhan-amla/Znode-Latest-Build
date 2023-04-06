CREATE TABLE [dbo].[ZnodeOmsOrderState] (
    [OmsOrderStateId]      INT            NOT NULL,
    [OrderStateName]       NVARCHAR (MAX) NULL,
    [IsShowToCustomer]     BIT            CONSTRAINT [DF_ZnodeOmsOrderState_IsShowToCustomer] DEFAULT ((1)) NOT NULL,
    [IsAccountStatus]      BIT            CONSTRAINT [DF_ZnodeOmsOrderState_IsAccountStatus] DEFAULT ((0)) NULL,
    [DisplayOrder]         INT            NOT NULL,
    [Description]          NVARCHAR (MAX) NULL,
    [IsEdit]               BIT            CONSTRAINT [DF__ZnodeOmsO__IsEdi__007FFA1B] DEFAULT ((0)) NOT NULL,
    [IsSendEmail]          BIT            CONSTRAINT [DF__ZnodeOmsO__IsSen__01741E54] DEFAULT ((0)) NOT NULL,
    [IsOrderState]         BIT            CONSTRAINT [DF__ZnodeOmsO__IsOrd__07EC11B9] DEFAULT ((0)) NULL,
    [IsOrderLineItemState] BIT            CONSTRAINT [DF_ZnodeOmsOrderState_IsOrderLineItemState] DEFAULT ((0)) NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    [IsQuoteState]         BIT            NULL,
    CONSTRAINT [PK_ZnodeOmsOrderState] PRIMARY KEY CLUSTERED ([OmsOrderStateId] ASC) WITH (FILLFACTOR = 90)
);











