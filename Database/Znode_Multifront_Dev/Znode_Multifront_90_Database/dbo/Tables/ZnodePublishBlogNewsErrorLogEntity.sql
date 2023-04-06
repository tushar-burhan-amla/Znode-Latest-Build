CREATE TABLE [dbo].[ZnodePublishBlogNewsErrorLogEntity](
	[PublishBlogNewsErrorLogEntityId] [int] IDENTITY(1,1) NOT NULL,
	[EntityName] [varchar](100) NULL,
	[ErrorDescription] [nvarchar](max) NULL,
	[ProcessStatus] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[VersionId] [varchar](100) NULL,
 CONSTRAINT [PK_ZnodePublishBlogNewsErrorLogEntity] PRIMARY KEY CLUSTERED 
(
	[PublishBlogNewsErrorLogEntityId] ASC
)) 
GO