CREATE TABLE [dbo].[ZnodePublishSearchProfileEntity](
	[PublishSearchProfileEntityId] [int] IDENTITY(1,1) NOT NULL,
	[SearchProfileId] [int] NULL,
	[SearchProfileName] [nvarchar](800) NULL,
	[ZnodeCatalogId] [int] NULL,
	[FeaturesList] [nvarchar](max) NULL,
	[QueryTypeName] [nvarchar](800) NULL,
	[SearchQueryType] [nvarchar](800) NULL,
	[QueryBuilderClassName] [nvarchar](800) NULL,
	[SubQueryType] [nvarchar](800) NULL,
	[FieldValueFactor] [nvarchar](max) NULL,
	[Operator] [varchar](20) NULL,
	[PublishStateId] [int] NULL,
	[SearchProfileAttributeMappingJson] [nvarchar](max) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[SearchProfilePublishLogId] [int] NOT NULL,
 CONSTRAINT [PK_ZnodePublishSearchProfileEntity] PRIMARY KEY CLUSTERED 
(
	[PublishSearchProfileEntityId] ASC
)
) ON [PRIMARY];


