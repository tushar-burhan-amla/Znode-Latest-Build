CREATE TABLE [dbo].[ZnodeCMSWidgetTitleConfigurationLocale] (
    [CMSWidgetTitleConfigurationLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [CMSWidgetTitleConfigurationId]       INT            NOT NULL,
    [MediaId]                             INT            CONSTRAINT [DF_ZnodeCMSWidgetTitleConfigurationLocale_MediaId] DEFAULT (NULL) NULL,
    [Title]                               NVARCHAR (300) NOT NULL,
    [Url]                                 NVARCHAR (300) NULL,
    [LocaleId]                            INT            NOT NULL,
    [CreatedBy]                           INT            NOT NULL,
    [CreatedDate]                         DATETIME       NOT NULL,
    [ModifiedBy]                          INT            NOT NULL,
    [ModifiedDate]                        DATETIME       NOT NULL,
    [IsNewTab]                            BIT            CONSTRAINT [DF_ZnodeCMSWidgetTitleConfigurationLocale] DEFAULT ((0)) NOT NULL,
    [DisplayOrder]                        INT            CONSTRAINT [DF_ZnodeCMSWidgetTitleConfigurationLocale_DisplayOrder] DEFAULT ((999)) NOT NULL,
    CONSTRAINT [PK_ZnodeCMSWidgetTitleConfigurationLocale] PRIMARY KEY CLUSTERED ([CMSWidgetTitleConfigurationLocaleId] ASC),
    CONSTRAINT [FK_ZnodeCMSWidgetTitleConfigurationLocale_ZnodeCMSWidgetTitleConfiguration] FOREIGN KEY ([CMSWidgetTitleConfigurationId]) REFERENCES [dbo].[ZnodeCMSWidgetTitleConfiguration] ([CMSWidgetTitleConfigurationId])
);











