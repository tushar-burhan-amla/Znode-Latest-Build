CREATE TABLE [dbo].[ZnodePublishContetContainerVersionEntity](
	[VersionId] [int] IDENTITY(1,1) NOT NULL,
	[PublishStartTime] [datetime] NULL,
	[PublishState] [varchar](100) NULL
) ON [PRIMARY]
GO