CREATE TABLE ZnodeSearchItemRule
	(
		SearchItemRuleId	Int Identity(1,1),
		SearchCatalogRuleId	Int NOT NULL,-- [FK ZnodeSearchCatalogRule]
		SearchItemKeyword Varchar(100),
		SearchItemCondition	Varchar(50),
		SearchItemValue	Varchar(600),
		SearchItemBoostValue DECIMAL(18, 5),
		CreatedBy Int NOT NULL,
		CreatedDate	DateTime NOT NULL,
		ModifiedBy	Int NOT NULL,
		ModifiedDate DateTime NOT NULL,
		CONSTRAINT [PK_ZnodeSearchItemRule] PRIMARY KEY CLUSTERED ( SearchItemRuleId ASC),
		CONSTRAINT FK_ZnodeSearchItemRule_ZnodeSearchCatalogRule FOREIGN KEY( SearchCatalogRuleId ) REFERENCES ZnodeSearchCatalogRule (SearchCatalogRuleId)
	);