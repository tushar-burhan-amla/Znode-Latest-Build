CREATE TABLE [dbo].[ZnodeCMSOfferPageProduct] (
    [CMSOfferPageProductId] INT      IDENTITY (1, 1) NOT NULL,
    [CMSContentPagesId]     INT      NOT NULL,
    [PublishProductId]      INT      NOT NULL,
    [CreatedBy]             INT      NOT NULL,
    [CreatedDate]           DATETIME NOT NULL,
    [ModifiedBy]            INT      NOT NULL,
    [ModifiedDate]          DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCMSOfferPageProduct] PRIMARY KEY CLUSTERED ([CMSOfferPageProductId] ASC),
    CONSTRAINT [FK_ZnodeCMSOfferPageProduct_ZnodeCMSContentPages] FOREIGN KEY ([CMSContentPagesId]) REFERENCES [dbo].[ZnodeCMSContentPages] ([CMSContentPagesId]),
    CONSTRAINT [FK_ZnodeCMSOfferPageProduct_ZnodePublishProduct] FOREIGN KEY ([PublishProductId]) REFERENCES [dbo].[ZNodePublishProduct] ([PublishProductId])
);

