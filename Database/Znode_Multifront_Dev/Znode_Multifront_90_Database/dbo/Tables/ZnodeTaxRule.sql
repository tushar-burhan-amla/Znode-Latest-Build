CREATE TABLE [dbo].[ZnodeTaxRule] (
    [TaxRuleId]              INT             IDENTITY (1, 1) NOT NULL,
    [TaxRuleTypeId]          INT             NULL,
    [TaxClassId]             INT             NULL,
    [DestinationCountryCode] NVARCHAR (10)   NULL,
    [DestinationStateCode]   NVARCHAR (255)  NULL,
    [CountyFIPS]             NVARCHAR (50)   NULL,
    [Precedence]             INT             NOT NULL,
    [SalesTax]               DECIMAL (18, 2) NULL,
    [VAT]                    DECIMAL (18, 2) NULL,
    [GST]                    DECIMAL (18, 2) NULL,
    [PST]                    DECIMAL (18, 2) NULL,
    [HST]                    DECIMAL (18, 2) NULL,
    [TaxShipping]            BIT             NOT NULL,
    [Custom1]                NVARCHAR (MAX)  NULL,
    [Custom2]                NVARCHAR (MAX)  NULL,
    [Custom3]                NVARCHAR (MAX)  NULL,
    [ExternalID]             VARCHAR (50)    NULL,
    [ZipCode]                NVARCHAR (MAX)  NULL,
    [CreatedBy]              INT             NOT NULL,
    [CreatedDate]            DATETIME        NOT NULL,
    [ModifiedBy]             INT             NOT NULL,
    [ModifiedDate]           DATETIME        NOT NULL,
    CONSTRAINT [PK_SC_TaxRule] PRIMARY KEY CLUSTERED ([TaxRuleId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeTaxRule_ZnodeTaxClass] FOREIGN KEY ([TaxClassId]) REFERENCES [dbo].[ZnodeTaxClass] ([TaxClassId]),
    CONSTRAINT [FK_ZnodeTaxRule_ZnodeTaxRuleType] FOREIGN KEY ([TaxRuleTypeId]) REFERENCES [dbo].[ZnodeTaxRuleTypes] ([TaxRuleTypeId])
);















