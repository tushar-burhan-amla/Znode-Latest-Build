CREATE TABLE [dbo].[ZnodeOmsPaymentRefund] (
    [OmsPaymentRefundId]  INT             IDENTITY (1, 1) NOT NULL,
    [OmsOrderDetailsId]   INT             NULL,
    [OmsOrderLineItemsId] INT             NULL,
    [RefundAmount]        NUMERIC (28, 6) NOT NULL,
    [RefundTaxAmount]     NCHAR (10)      NULL,
    [Notes]               NVARCHAR (MAX)  NULL,
    [OmsRefundTypeId]     INT             NOT NULL,
    [CreatedBy]           INT             NOT NULL,
    [CreatedDate]         DATETIME        NOT NULL,
    [ModifiedBy]          INT             NOT NULL,
    [ModifiedDate]        DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeOmsPaymentRefund] PRIMARY KEY CLUSTERED ([OmsPaymentRefundId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsPaymentRefund_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId]),
    CONSTRAINT [FK_ZnodeOmsPaymentRefund_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId]),
    CONSTRAINT [FK_ZnodeOmsPaymentRefund_ZnodeOmsRefundType] FOREIGN KEY ([OmsRefundTypeId]) REFERENCES [dbo].[ZnodeOmsRefundType] ([OmsRefundTypeId])
);





