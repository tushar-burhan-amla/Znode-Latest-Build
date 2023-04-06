CREATE TYPE [dbo].[SearchRuleItemDetail] AS TABLE(
	[SearchItemKeyword] [varchar](100) NULL,
	[SearchItemCondition] [varchar](50) NULL,
	[SearchItemValue] [varchar](600) NULL,
	[SearchItemBoostValue] [varchar](10) NULL,
	[SearchCatalogRuleId] [int] NULL,
	[SearchItemRuleId] [int] NULL
)