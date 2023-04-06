CREATE TABLE [dbo].[ZnodeTaxClass] (
    [TaxClassId]   INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NOT NULL,
    [IsActive]     BIT            NOT NULL,
    [DisplayOrder] INT            NOT NULL,
    [ExternalId]   VARCHAR (50)   NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedBy]   INT            NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeTaxClass] PRIMARY KEY CLUSTERED ([TaxClassId] ASC) WITH (FILLFACTOR = 90)
);



