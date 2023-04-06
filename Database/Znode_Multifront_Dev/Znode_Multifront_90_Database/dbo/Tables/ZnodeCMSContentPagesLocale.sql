CREATE TABLE [dbo].[ZnodeCMSContentPagesLocale] (
    [CMSContentPagesLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [CMSContentPagesId]       INT            NULL,
    [LocaleId]                INT            NULL,
    [PageTitle]               NVARCHAR (100) NULL,
    [CreatedBy]               INT            NOT NULL,
    [CreatedDate]             DATETIME       NOT NULL,
    [ModifiedBy]              INT            NOT NULL,
    [ModifiedDate]            DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSContentPagesLocale] PRIMARY KEY CLUSTERED ([CMSContentPagesLocaleId] ASC),
    CONSTRAINT [FK_ZnodeCMSContentPagesLocale_ZnodeCMSContentPages] FOREIGN KEY ([CMSContentPagesId]) REFERENCES [dbo].[ZnodeCMSContentPages] ([CMSContentPagesId])
);



