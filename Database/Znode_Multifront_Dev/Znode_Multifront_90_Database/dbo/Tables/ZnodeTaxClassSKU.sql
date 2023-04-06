CREATE TABLE [dbo].[ZnodeTaxClassSKU] (
    [TaxClassSKUId] INT           IDENTITY (1, 1) NOT NULL,
    [TaxClassId]    INT           NULL,
    [SKU]           VARCHAR (300) NULL,
    [CreatedBy]     INT           NOT NULL,
    [CreatedDate]   DATETIME      NOT NULL,
    [ModifiedBy]    INT           NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeTaxClassSKU] PRIMARY KEY CLUSTERED ([TaxClassSKUId] ASC),
    CONSTRAINT [FK_ZnodeTaxClassSKU_ZnodeTaxClass] FOREIGN KEY ([TaxClassId]) REFERENCES [dbo].[ZnodeTaxClass] ([TaxClassId])
);

