CREATE TABLE [dbo].[ZnodeOmsPersonalizeItem] (
    [OmsPersonalizeItemId] INT             IDENTITY (1, 1) NOT NULL,
    [OmsOrderLineItemsId]  INT             NOT NULL,
    [PersonalizeCode]      NVARCHAR (200)  NOT NULL,
    [PersonalizeValue]     NVARCHAR (MAX)  NOT NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
    [DesignId]             NVARCHAR (2000) NULL,
    [ThumbnailURL]         NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ZnodeOmsPersonalizeItem] PRIMARY KEY CLUSTERED ([OmsPersonalizeItemId] ASC),
    CONSTRAINT [FK_ZnodeOmsPersonalizeItem_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId])
);



