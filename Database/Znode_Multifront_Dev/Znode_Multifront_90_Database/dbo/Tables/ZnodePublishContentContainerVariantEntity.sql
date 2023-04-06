CREATE TABLE [dbo].[ZnodePublishContentContainerVariantEntity](
	[PublishContentContainerVariantEntityId] [int] IDENTITY(1,1) NOT NULL,
	[VersionId] [int] NOT NULL,
	[PortalId] [int] NULL,
	[LocaleId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ContainerKey] [nvarchar](100) NOT NULL,
	[CMSContentContainerId] [int] NOT NULL,
	[ProfileId] [int] NULL,
	[CMSContainerTemplateId] [int] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[CMSContainerProfileVariantId] [int] NULL,
	[GlobalAttributes] [nvarchar](max) NULL,
	[IsActive] [BIT] NULL,
 CONSTRAINT [PK_ZnodePublishContentContainerVariantEntity] PRIMARY KEY CLUSTERED 
(
	[PublishContentContainerVariantEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZnodePublishContentContainerVariantEntity]  WITH CHECK ADD  CONSTRAINT [FK_ZnodePublishContentContainerVariantEntity_LocaleId] FOREIGN KEY([LocaleId])
REFERENCES [dbo].[ZnodeLocale] ([LocaleId])
GO

ALTER TABLE [dbo].[ZnodePublishContentContainerVariantEntity] CHECK CONSTRAINT [FK_ZnodePublishContentContainerVariantEntity_LocaleId]
GO