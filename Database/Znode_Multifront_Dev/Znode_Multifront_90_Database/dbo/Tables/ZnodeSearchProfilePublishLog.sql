CREATE TABLE [dbo].[ZnodeSearchProfilePublishLog](
	[SearchProfilePublishLogId] [int] IDENTITY(1,1) NOT NULL,
	[SearchProfileId] [int] NOT NULL,
	[PublishstateId] [int] NOT NULL,
	[PublishStartDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ZnodeSearchProfilePublishLog] PRIMARY KEY CLUSTERED 
(
	[SearchProfilePublishLogId] ASC
)
) ON [PRIMARY];