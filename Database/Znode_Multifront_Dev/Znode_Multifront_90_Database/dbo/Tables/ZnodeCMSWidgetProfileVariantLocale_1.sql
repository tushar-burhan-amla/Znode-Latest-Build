CREATE TABLE [dbo].[ZnodeCMSWidgetProfileVariantLocale](
	[CMSWidgetProfileVariantLocaleId] [int] IDENTITY(1,1) NOT NULL,
	[CMSWidgetProfileVariantId] [int] NOT NULL,
	[CMSWidgetTemplateId] [int] NULL,
	[LocaleId] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ZnodeCMSWidgetProfileVariantLocale] PRIMARY KEY CLUSTERED 
(
	[CMSWidgetProfileVariantLocaleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZnodeCMSWidgetProfileVariantLocale]  WITH CHECK ADD  CONSTRAINT [FK_ZnodeCMSWidgetProfileVariantLocale_ZnodeCMSWidgetProfileVariant] FOREIGN KEY([CMSWidgetProfileVariantId])
REFERENCES [dbo].[ZnodeCMSWidgetProfileVariant] ([CMSWidgetProfileVariantId])
GO

ALTER TABLE [dbo].[ZnodeCMSWidgetProfileVariantLocale] CHECK CONSTRAINT [FK_ZnodeCMSWidgetProfileVariantLocale_ZnodeCMSWidgetProfileVariant]
GO

ALTER TABLE [dbo].[ZnodeCMSWidgetProfileVariantLocale]  WITH CHECK ADD  CONSTRAINT [FK_ZnodeCMSWidgetProfileVariantLocale_ZnodeCMSWidgetTemplate] FOREIGN KEY([CMSWidgetTemplateId])
REFERENCES [dbo].[ZnodeCMSWidgetTemplate] ([CMSWidgetTemplateId])
GO

ALTER TABLE [dbo].[ZnodeCMSWidgetProfileVariantLocale] CHECK CONSTRAINT [FK_ZnodeCMSWidgetProfileVariantLocale_ZnodeCMSWidgetTemplate]
GO

ALTER TABLE [dbo].[ZnodeCMSWidgetProfileVariantLocale]  WITH CHECK ADD  CONSTRAINT [FK_ZnodeCMSWidgetProfileVariantLocale_ZnodeLocale] FOREIGN KEY([LocaleId])
REFERENCES [dbo].[ZnodeLocale] ([LocaleId])
GO

ALTER TABLE [dbo].[ZnodeCMSWidgetProfileVariantLocale] CHECK CONSTRAINT [FK_ZnodeCMSWidgetProfileVariantLocale_ZnodeLocale]
GO
