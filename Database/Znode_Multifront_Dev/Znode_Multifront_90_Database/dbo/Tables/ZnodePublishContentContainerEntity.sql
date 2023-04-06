CREATE TABLE [dbo].[ZnodePublishContentContainerEntity](
	[PublishContentContainerEntityId] [int] IDENTITY(1,1) NOT NULL,
	[VersionId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ContainerKey] [nvarchar](100) NOT NULL,
	[FamilyId] [int] NOT NULL,
	[Tags] [nvarchar](1000) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ZnodePublishContentContainerEntity] PRIMARY KEY CLUSTERED 
(
	[PublishContentContainerEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO