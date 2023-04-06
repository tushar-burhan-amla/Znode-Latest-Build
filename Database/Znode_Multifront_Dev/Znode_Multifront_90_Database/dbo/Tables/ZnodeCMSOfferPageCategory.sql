CREATE TABLE [dbo].[ZnodeCMSOfferPageCategory] (
    [CMSOfferPageCategoryId] INT      IDENTITY (1, 1) NOT NULL,
    [CMSContentPagesId]      INT      NOT NULL,
    [PublishCategoryId]      INT      NOT NULL,
    [CreatedBy]              INT      NOT NULL,
    [CreatedDate]            DATETIME NOT NULL,
    [ModifiedBy]             INT      NOT NULL,
    [ModifiedDate]           DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCMSOfferPageCategory] PRIMARY KEY CLUSTERED ([CMSOfferPageCategoryId] ASC),
    CONSTRAINT [FK_ZnodeCMSOfferPageCategory_ZnodeCMSContentPages] FOREIGN KEY ([CMSContentPagesId]) REFERENCES [dbo].[ZnodeCMSContentPages] ([CMSContentPagesId]),
    CONSTRAINT [FK_ZnodeCMSOfferPageCategory_ZnodePublishCategory] FOREIGN KEY ([PublishCategoryId]) REFERENCES [dbo].[ZnodePublishCategory] ([PublishCategoryId])
);

