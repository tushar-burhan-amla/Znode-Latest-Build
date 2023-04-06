CREATE TABLE [dbo].[ZnodeCMSContainerProfileVariantLocale](
	[CMSContainerProfileVariantLocaleId] [int] IDENTITY(1,1) NOT NULL,
	[CMSContainerProfileVariantId] [int] NOT NULL,
	[CMSContainerTemplateId] [int] NULL,
	[LocaleId] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,	
 CONSTRAINT [PK_ZnodeCMSContainerProfileVariantLocale] PRIMARY KEY CLUSTERED 
(
	[CMSContainerProfileVariantLocaleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZnodeCMSContainerProfileVariantLocale] ADD  DEFAULT ((1)) FOR [IsActive]
GO


ALTER TABLE [dbo].[ZnodeCMSContainerProfileVariantLocale]  WITH CHECK ADD  CONSTRAINT [FK_ZnodeCMSContainerProfileVariantLocale_ZnodeCMSContainerProfileVariant] FOREIGN KEY([CMSContainerProfileVariantId])
REFERENCES [dbo].[ZnodeCMSContainerProfileVariant] ([CMSContainerProfileVariantId])
GO

ALTER TABLE [dbo].[ZnodeCMSContainerProfileVariantLocale] CHECK CONSTRAINT [FK_ZnodeCMSContainerProfileVariantLocale_ZnodeCMSContainerProfileVariant]
GO

ALTER TABLE [dbo].[ZnodeCMSContainerProfileVariantLocale]  WITH CHECK ADD  CONSTRAINT [FK_ZnodeCMSContainerProfileVariantLocale_ZnodeLocale] FOREIGN KEY([LocaleId])
REFERENCES [dbo].[ZnodeLocale] ([LocaleId])
GO

ALTER TABLE [dbo].[ZnodeCMSContainerProfileVariantLocale] CHECK CONSTRAINT [FK_ZnodeCMSContainerProfileVariantLocale_ZnodeLocale]
GO