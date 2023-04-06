CREATE TABLE [dbo].[ZnodeHighlightProduct] (
    [HighlightProductId] INT      IDENTITY (1, 1) NOT NULL,
    [HighlightId]        INT      NOT NULL,
    [PublishProductId]   INT      NOT NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeHighlightProduct] PRIMARY KEY CLUSTERED ([HighlightProductId] ASC),
    CONSTRAINT [FK_ZnodeHighlightProduct_ZnodeHighlight] FOREIGN KEY ([HighlightId]) REFERENCES [dbo].[ZnodeHighlight] ([HighlightId]),
    CONSTRAINT [FK_ZnodeHighlightProduct_ZNodePublishProduct] FOREIGN KEY ([PublishProductId]) REFERENCES [dbo].[ZNodePublishProduct] ([PublishProductId])
);

