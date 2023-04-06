CREATE TABLE [dbo].[ZnodeCMSContentContainer](
	[CMSContentContainerId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ContainerKey] [nvarchar](100) NOT NULL,
	[FamilyId] [int] NOT NULL,
	[Tags] [nvarchar](1000) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[PublishStateId] [tinyint] NULL,
 CONSTRAINT [PK_ZnodeCMSContentContainer] PRIMARY KEY CLUSTERED 
(
	[CMSContentContainerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZnodeCMSContentContainer] ADD  DEFAULT ((2)) FOR [PublishStateId]
GO

ALTER TABLE [dbo].[ZnodeCMSContentContainer]  WITH CHECK ADD  CONSTRAINT [FK_ZnodeCMSContentContainer_PublishStateId] FOREIGN KEY([PublishStateId])
REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
GO

ALTER TABLE [dbo].[ZnodeCMSContentContainer] CHECK CONSTRAINT [FK_ZnodeCMSContentContainer_PublishStateId]
GO
