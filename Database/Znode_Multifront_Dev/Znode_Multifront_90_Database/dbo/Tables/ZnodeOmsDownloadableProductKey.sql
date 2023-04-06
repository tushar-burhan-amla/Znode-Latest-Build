CREATE TABLE [dbo].[ZnodeOmsDownloadableProductKey] (
    [OmsDownloadableProductKeyId] INT      IDENTITY (1, 1) NOT NULL,
    [OmsOrderLineItemsId]         INT      NOT NULL,
    [PimDownloadableProductKeyId] INT      NOT NULL,
    [CreatedBy]                   INT      NOT NULL,
    [CreatedDate]                 DATETIME NOT NULL,
    [ModifiedBy]                  INT      NOT NULL,
    [ModifiedDate]                DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeOmsDownloadableProductKey] PRIMARY KEY CLUSTERED ([OmsDownloadableProductKeyId] ASC),
    CONSTRAINT [FK_ZnodeOmsDownloadableProductKey_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId]),
    CONSTRAINT [FK_ZnodeOmsDownloadableProductKey_ZnodePimDownloadableProductKey] FOREIGN KEY ([PimDownloadableProductKeyId]) REFERENCES [dbo].[ZnodePimDownloadableProductKey] ([PimDownloadableProductKeyId])
);

