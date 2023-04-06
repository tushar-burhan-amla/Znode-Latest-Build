CREATE TABLE [dbo].[ZnodePriceTier] (
    [PriceTierId]  INT             IDENTITY (1, 1) NOT NULL,
    [PriceListId]  INT             NULL,
    [SKU]          VARCHAR (300)   NOT NULL,
    [Price]        NUMERIC (28, 6) NOT NULL,
    [Quantity]     NUMERIC (28, 6) NOT NULL,
    [UomId]        INT             NULL,
    [UnitSize]     NUMERIC (28, 6) NULL,
    [CreatedBy]    INT             NOT NULL,
    [CreatedDate]  DATETIME        NOT NULL,
    [ModifiedBy]   INT             NOT NULL,
    [ModifiedDate] DATETIME        NOT NULL,
    [Custom1]      NVARCHAR (MAX)  NULL,
    [Custom2]      NVARCHAR (MAX)  NULL,
    [Custom3]      NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ZnodePriceTier] PRIMARY KEY CLUSTERED ([PriceTierId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePriceTier_ZnodePriceList] FOREIGN KEY ([PriceListId]) REFERENCES [dbo].[ZnodePriceList] ([PriceListId]),
    CONSTRAINT [FK_ZnodePriceTier_ZnodeUom] FOREIGN KEY ([UomId]) REFERENCES [dbo].[ZnodeUom] ([UomId])
);







