CREATE TABLE [dbo].[ZnodeRmaRequestItem] (
    [RmaRequestItemId]     INT             IDENTITY (1, 1) NOT NULL,
    [RmaRequestId]         INT             NULL,
    [OmsOrderLineItemsId]  INT             NULL,
    [IsReceived]           BIT             CONSTRAINT [DF_ZnodeRmaRequestItem_IsReceived] DEFAULT ((0)) NOT NULL,
    [IsReturnable]         BIT             CONSTRAINT [DF_ZnodeRmaRequestItem_IsReturnable] DEFAULT ((0)) NOT NULL,
    [GiftCardId]           INT             NULL,
    [Quantity]             NUMERIC (28, 6) NULL,
    [Price]                NUMERIC (28, 6) NULL,
    [RmaReasonForReturnId] INT             NULL,
    [TransactionId]        NVARCHAR (200)  NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeRmaRequestItem] PRIMARY KEY CLUSTERED ([RmaRequestItemId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeRmaRequestItem_ZnodeGiftCard] FOREIGN KEY ([GiftCardId]) REFERENCES [dbo].[ZnodeGiftCard] ([GiftCardId]),
    CONSTRAINT [FK_ZnodeRmaRequestItem_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId]),
    CONSTRAINT [FK_ZnodeRmaRequestItem_ZnodeRmaReasonForReturn] FOREIGN KEY ([RmaReasonForReturnId]) REFERENCES [dbo].[ZnodeRmaReasonForReturn] ([RmaReasonForReturnId]),
    CONSTRAINT [FK_ZnodeRmaRequestItem_ZnodeRmaRequest] FOREIGN KEY ([RmaRequestId]) REFERENCES [dbo].[ZnodeRmaRequest] ([RmaRequestId])
);







