CREATE TABLE ZnodeCMSWidgetProfileVariantLocale
(
	CMSWidgetProfileVariantLocaleId INT      IDENTITY (1, 1) NOT NULL,
	CMSWidgetProfileVariantId INT NOT NULL,
	CMSWidgetTemplateId INT NULL,
	LocaleId INT NOT NULL, 
	[CreatedBy] INT            NOT NULL,
    [CreatedDate] DATETIME       NOT NULL,
    [ModifiedBy] INT            NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
	CONSTRAINT [PK_ZnodeCMSWidgetProfileVariantLocale] PRIMARY KEY CLUSTERED ([CMSWidgetProfileVariantLocaleId] ASC),
	CONSTRAINT [FK_ZnodeCMSWidgetProfileVariantLocale_ZnodeCMSWidgetProfileVariant] FOREIGN KEY ([CMSWidgetProfileVariantId]) REFERENCES [dbo].[ZnodeCMSWidgetProfileVariant] (CMSWidgetProfileVariantId),
	CONSTRAINT [FK_ZnodeCMSWidgetProfileVariantLocale_ZnodeLocale] FOREIGN KEY ([LocaleId]) REFERENCES [dbo].[ZnodeLocale] ([LocaleId]),
	CONSTRAINT [FK_ZnodeCMSWidgetProfileVariantLocale_ZnodeCMSWidgetTemplate] FOREIGN KEY ([CMSWidgetTemplateId]) REFERENCES [dbo].[ZnodeCMSWidgetTemplate] ([CMSWidgetTemplateId])
)