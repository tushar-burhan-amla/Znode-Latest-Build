CREATE TABLE [dbo].[ZnodeImportPricingLog] (
    [ImportPricingLogId] INT            IDENTITY (1, 1) NOT NULL,
    [SKU]                VARCHAR (300)  NULL,
    [TierStartQuantity]  VARCHAR (300)  NULL,
    [RetailPrice]        VARCHAR (300)  NULL,
    [SalesPrice]         VARCHAR (300)  NULL,
    [TierPrice]          VARCHAR (300)  NULL,
    [Uom]                VARCHAR (300)  NULL,
    [UnitSize]           VARCHAR (300)  NULL,
    [PriceListCode]      VARCHAR (200)  NULL,
    [PriceListName]      VARCHAR (600)  NULL,
    [CurrencyId]         VARCHAR (300)  NULL,
    [ActivationDate]     VARCHAR (300)  NULL,
    [ExpirationDate]     VARCHAR (300)  NULL,
    [SKUActivationDate]  VARCHAR (300)  NULL,
    [SKUExpirationDate]  VARCHAR (300)  NULL,
    [RowNumber]          BIGINT         NULL,
    [ErrorDescription]   VARCHAR (MAX)  NULL,
    [GUID]               NVARCHAR (200) NULL,
    [ImportProcessLogId] INT            NULL,
    [SourceColumnName]   NVARCHAR (200) NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeImportPricingLog] PRIMARY KEY CLUSTERED ([ImportPricingLogId] ASC) WITH (FILLFACTOR = 90)
);



