CREATE TYPE [dbo].[SearchRuleTriggerDetail] AS TABLE(
	[SearchTriggerKeyword] [varchar](100) NULL,
	[SearchTriggerCondition] [varchar](50) NULL,
	[SearchTriggerValue] [varchar](600) NULL,
	[SearchCatalogRuleId] [int] NULL,
	[SearchTriggerRuleId] [int] NULL
)