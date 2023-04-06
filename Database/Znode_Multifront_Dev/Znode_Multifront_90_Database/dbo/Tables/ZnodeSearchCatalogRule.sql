CREATE TABLE [dbo].[ZnodeSearchCatalogRule](
	[SearchCatalogRuleId] [int] IDENTITY(1,1) NOT NULL,
	[PublishCatalogId] [int] NOT NULL,
	[RuleName] [varchar](600) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[IsTriggerForAll] [bit] NOT NULL,
	[IsItemForAll] [bit] NOT NULL,
	[IsGlobalRule] [bit] NOT NULL CONSTRAINT [DF_ZnodeSearchCatalogRule_IsGlobalRule]  DEFAULT ((0)),
	[IsPause] [bit] NOT NULL CONSTRAINT [DF_ZnodeSearchCatalogRule_IsPause]  DEFAULT ((0)),
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ZnodeSearchCatalogRule] PRIMARY KEY CLUSTERED ([SearchCatalogRuleId] ASC),
 CONSTRAINT [UK_ZnodeSearchCatalogRule_RuleName] UNIQUE NONCLUSTERED ([RuleName] ASC)
 );