CREATE TABLE ZnodeSearchTriggerRule
	(
		SearchTriggerRuleId	Int Identity(1,1),
		SearchCatalogRuleId	Int NOT NULL,-- [FK ZnodeSearchCatalogRule]
		SearchTriggerKeyword	Varchar(100),
		SearchTriggerCondition	Varchar(50),
		SearchTriggerValue	Varchar(600),
		CreatedBy	Int NOT NULL,
		CreatedDate	DateTime NOT NULL,
		ModifiedBy	Int NOT NULL,
		ModifiedDate	DateTime NOT NULL,
		CONSTRAINT [PK_ZnodeSearchTriggerRule] PRIMARY KEY CLUSTERED ( SearchTriggerRuleId ASC), 
		CONSTRAINT FK_ZnodeSearchTriggerRule_ZnodeSearchCatalogRule FOREIGN KEY( SearchCatalogRuleId ) REFERENCES [dbo].[ZnodeSearchCatalogRule] ( SearchCatalogRuleId )
	);