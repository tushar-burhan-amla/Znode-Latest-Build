CREATE TABLE [dbo].[ZnodeCMSContentPageGroupLocale] (
    [CMSContentPageGroupLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [CMSContentPageGroupId]       INT            NOT NULL,
    [Name]                        NVARCHAR (100) NOT NULL,
    [LocaleId]                    INT            NOT NULL,
    [CreatedBy]                   INT            NOT NULL,
    [CreatedDate]                 DATETIME       NOT NULL,
    [ModifiedBy]                  INT            NOT NULL,
    [ModifiedDate]                DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSContentPageGroupLocale] PRIMARY KEY CLUSTERED ([CMSContentPageGroupLocaleId] ASC),
    CONSTRAINT [FK_ZnodeCMSContentPageGroupLocale_ZnodeCMSContentPageGroup] FOREIGN KEY ([CMSContentPageGroupId]) REFERENCES [dbo].[ZnodeCMSContentPageGroup] ([CMSContentPageGroupId])
);

