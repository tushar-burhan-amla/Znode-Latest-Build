CREATE TABLE [dbo].[ZnodeOmsTaxOrderDetails] (
    [OmsTaxOrderDetailsId] INT             IDENTITY (1, 1) NOT NULL,
    [OmsOrderDetailsId]    INT             NOT NULL,
    [SalesTax]             NUMERIC (28, 6) NULL,
    [VAT]                  NUMERIC (28, 6) NULL,
    [GST]                  NUMERIC (28, 6) NULL,
    [PST]                  NUMERIC (28, 6) NULL,
    [HST]                  NUMERIC (28, 6) NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
    [ImportDuty] NUMERIC(28,6) NULL,
    CONSTRAINT [PK_ZnodeOmsTaxOrderDetails] PRIMARY KEY CLUSTERED ([OmsTaxOrderDetailsId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsTaxOrderDetails_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId])
);





