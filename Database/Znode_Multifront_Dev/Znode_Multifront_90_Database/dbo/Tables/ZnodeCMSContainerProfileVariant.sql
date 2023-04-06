CREATE TABLE [dbo].[ZnodeCMSContainerProfileVariant](
	[CMSContainerProfileVariantId] [int] IDENTITY(1,1) NOT NULL,
	[CMSContentContainerId] [int] NOT NULL,
	[ProfileId] [int] NULL,
	[PortalId] [int] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[PublishStateId] [tinyint] NULL,
 CONSTRAINT [PK_ZnodeCMSContainerProfileVariant] PRIMARY KEY CLUSTERED 
(
	[CMSContainerProfileVariantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZnodeCMSContainerProfileVariant]  WITH CHECK ADD  CONSTRAINT [FK_ZnodeCMSContainerProfileVariant_ZnodeProfile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[ZnodeProfile] ([ProfileId])
GO

ALTER TABLE [dbo].[ZnodeCMSContainerProfileVariant] CHECK CONSTRAINT [FK_ZnodeCMSContainerProfileVariant_ZnodeProfile]
GO

ALTER TABLE [dbo].[ZnodeCMSContainerProfileVariant]  WITH CHECK ADD  CONSTRAINT [FK_ZnodeCMSContainerProfileVariant_ZnodePublishState] FOREIGN KEY([PublishStateId])
REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
GO

ALTER TABLE [dbo].[ZnodeCMSContainerProfileVariant] CHECK CONSTRAINT [FK_ZnodeCMSContainerProfileVariant_ZnodePublishState]
GO