CREATE TABLE [dbo].[ZnodeOmsTaxOrderLineDetails] (
    [OmsTaxOrderLineDetailsId] INT             IDENTITY (1, 1) NOT NULL,
    [OmsOrderLineItemsId]      INT             NOT NULL,
    [TaxRuleId]                INT             NULL,
    [TaxTransactionNumber]     NVARCHAR (600)  NULL,
    [Comments]                 NVARCHAR (MAX)  NULL,
    [SalesTax]                 NUMERIC (28, 6) NULL,
    [VAT]                      NUMERIC (28, 6) NULL,
    [GST]                      NUMERIC (28, 6) NULL,
    [PST]                      NUMERIC (28, 6) NULL,
    [HST]                      NUMERIC (28, 6) NULL,
    [CreatedBy]                INT             NOT NULL,
    [CreatedDate]              DATETIME        NOT NULL,
    [ModifiedBy]               INT             NOT NULL,
    [ModifiedDate]             DATETIME        NOT NULL,
    [ImportDuty] NUMERIC(28,6) NULL, 
    CONSTRAINT [PK_ZnodeOmsTaxOrderLineDetails] PRIMARY KEY CLUSTERED ([OmsTaxOrderLineDetailsId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsTaxOrderLineDetails_ZnodeOmsOrderLineItems] FOREIGN KEY ([OmsOrderLineItemsId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId])
);





