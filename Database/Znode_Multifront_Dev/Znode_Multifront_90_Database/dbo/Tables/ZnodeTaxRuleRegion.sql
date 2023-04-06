CREATE TABLE [dbo].[ZnodeTaxRuleRegion] (
    [TaxRuleRegionId] INT          IDENTITY (1, 1) NOT NULL,
    [TaxRuleId]       INT          NOT NULL,
    [CountryCode]     VARCHAR (50) NOT NULL,
    [StateCode]       VARCHAR (50) NULL,
    [CityId]          INT          NULL,
    [CreatedBy]       INT          NOT NULL,
    [CreatedDate]     DATETIME     NOT NULL,
    [ModifiedBy]      INT          NOT NULL,
    [ModifiedDate]    DATETIME     NOT NULL,
    CONSTRAINT [PK_ZnodeTaxRuleRegion] PRIMARY KEY CLUSTERED ([TaxRuleRegionId] ASC),
    CONSTRAINT [FK_ZnodeTaxRuleRegion_ZnodeTaxRule] FOREIGN KEY ([TaxRuleId]) REFERENCES [dbo].[ZnodeTaxRule] ([TaxRuleId])
);

