CREATE TABLE [dbo].[ZnodeEmailTemplateLocale] (
    [EmailTemplateLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [EmailTemplateId]       INT            NULL,
    [Subject]               NVARCHAR (600) NULL,
    [Descriptions]          NVARCHAR (600) NULL,
    [Content]               NVARCHAR (MAX) NULL,
    [LocaleId]              INT            NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
	[SmsContent]			NVARCHAR  (MAX) NULL,
    CONSTRAINT [PK_ZnodeEmailTemplateLocale] PRIMARY KEY CLUSTERED ([EmailTemplateLocaleId] ASC),
    CONSTRAINT [FK_ZnodeEmailTemplateLocale_ZnodeEmailTemplate] FOREIGN KEY ([EmailTemplateId]) REFERENCES [dbo].[ZnodeEmailTemplate] ([EmailTemplateId])
);

