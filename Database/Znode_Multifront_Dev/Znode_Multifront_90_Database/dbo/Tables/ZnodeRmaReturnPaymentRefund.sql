CREATE TABLE [dbo].[ZnodeRmaReturnPaymentRefund] (
    [RmaReturnPaymentRefundId] INT             IDENTITY (1, 1) NOT NULL,
    [RmaReturnDetailsId]       INT             NOT NULL,
    [RmaReturnLineItemsId]     INT             NOT NULL,
    [RefundAmount]             NUMERIC (28, 6) NOT NULL,
    [RefundTaxAmount]          NUMERIC (28, 6) NULL,
    [Notes]                    NVARCHAR (MAX)  NULL,
    [OmsRefundTypeId]          INT             NOT NULL,
    [CreatedBy]                INT             NOT NULL,
    [CreatedDate]              DATETIME        NOT NULL,
    [ModifiedBy]               INT             NOT NULL,
    [ModifiedDate]             DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeRmaReturnPaymentRefund] PRIMARY KEY CLUSTERED ([RmaReturnPaymentRefundId] ASC),
    CONSTRAINT [FK_ZnodeRmaReturnPaymentRefund_ZnodeOmsRefundType] FOREIGN KEY ([OmsRefundTypeId]) REFERENCES [dbo].[ZnodeOmsRefundType] ([OmsRefundTypeId]),
    CONSTRAINT [FK_ZnodeRmaReturnPaymentRefund_ZnodeRmaReturnDetails] FOREIGN KEY ([RmaReturnDetailsId]) REFERENCES [dbo].[ZnodeRmaReturnDetails] ([RmaReturnDetailsId]),
    CONSTRAINT [FK_ZnodeRmaReturnPaymentRefund_ZnodeRmaReturnLineItems] FOREIGN KEY ([RmaReturnLineItemsId]) REFERENCES [dbo].[ZnodeRmaReturnLineItems] ([RmaReturnLineItemsId])
);

