CREATE TABLE [dbo].[ZnodeOmsOrder] (
    [OmsOrderId]     INT             IDENTITY (1, 1) NOT NULL,
    [IsQuoteOrder]   BIT             CONSTRAINT [DF_ZnodeOmsOrder_IsQuoteOrder] DEFAULT ((0)) NOT NULL,
    [OrderNumber]    VARCHAR (200)   NULL,
    [ExternalId]     NVARCHAR (1000) NULL,
    [CreatedBy]      INT             NOT NULL,
    [CreatedDate]    DATETIME        NOT NULL,
    [ModifiedBy]     INT             NOT NULL,
    [ModifiedDate]   DATETIME        NOT NULL,
    [OMSQuoteId]     INT             NULL,
    [PublishStateId] TINYINT         NULL,
    [IsOldOrder]     BIT             CONSTRAINT [DF_ZnodeOmsOrder_IsOldOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ZnodeOmsOrder] PRIMARY KEY CLUSTERED ([OmsOrderId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOMSOrder_ZnodeOMSQuote] FOREIGN KEY ([OMSQuoteId]) REFERENCES [dbo].[ZnodeOmsQuote] ([OmsQuoteId]),
    CONSTRAINT [FK_ZnodeOmsOrder_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);













