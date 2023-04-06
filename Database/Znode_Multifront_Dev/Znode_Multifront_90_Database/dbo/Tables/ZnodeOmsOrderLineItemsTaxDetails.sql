CREATE TABLE [dbo].[ZnodeOmsOrderLineItemsTaxDetails] (
    [OmsOrderLineTaxDetailsId] INT            NOT NULL,
    [OmsOrderLineItemId]       INT            NOT NULL,
    [TaxRuleId]                INT            NOT NULL,
    [TaxTransactionNumber]     NVARCHAR (100) NULL,
    [Comments]                 NVARCHAR (100) NULL,
    [CreatedBy]                INT            NOT NULL,
    [CreatedDate]              DATETIME       NOT NULL,
    [ModifiedBy]               INT            NOT NULL,
    [ModifiedDate]             DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeOmsOrderLineTaxDetails] PRIMARY KEY CLUSTERED ([OmsOrderLineTaxDetailsId] ASC),
    CONSTRAINT [FK_ZnodeOmsOrderLineTaxDetails_ZnodeTaxRule] FOREIGN KEY ([TaxRuleId]) REFERENCES [dbo].[ZnodeTaxRule] ([TaxRuleId]) ON DELETE CASCADE ON UPDATE CASCADE
);

