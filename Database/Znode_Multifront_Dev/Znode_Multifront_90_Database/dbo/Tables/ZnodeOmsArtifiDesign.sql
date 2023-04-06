CREATE TABLE [dbo].[ZnodeOmsArtifiDesign] (
    [OmsArtifiDesignId]      INT      IDENTITY (1, 1) NOT NULL,
    [OmsOrderLineItemsId]    INT      NULL,
    [OmsSavedCartLineItemId] INT      NULL,
    [ArtifiDesignId]         INT      NOT NULL,
    [CreatedBy]              INT      NOT NULL,
    [CreatedDate]            DATETIME NOT NULL,
    [ModifiedBy]             INT      NOT NULL,
    [ModifiedDate]           DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeOmsArtifiDesign] PRIMARY KEY CLUSTERED ([OmsArtifiDesignId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsArtifiDesign_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId])
);



