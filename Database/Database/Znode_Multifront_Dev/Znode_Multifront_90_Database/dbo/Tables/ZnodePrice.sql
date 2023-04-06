CREATE TABLE [dbo].[ZnodePrice] (
    [PriceId]        INT             IDENTITY (1, 1) NOT NULL,
    [PriceListId]    INT             NULL,
    [SKU]            VARCHAR (300)   NULL,
    [SalesPrice]     NUMERIC (28, 6) NULL,
    [RetailPrice]    NUMERIC (28, 6) NOT NULL,
    [UomId]          INT             NULL,
    [UnitSize]       NUMERIC (28, 6) NULL,
    [ActivationDate] DATETIME        NULL,
    [ExpirationDate] DATETIME        NULL,
    [ExternalId]     NVARCHAR (1000) NULL,
    [CreatedBy]      INT             NOT NULL,
    [CreatedDate]    DATETIME        NOT NULL,
    [ModifiedBy]     INT             NOT NULL,
    [ModifiedDate]   DATETIME        NOT NULL,
    [CostPrice]      NUMERIC (28, 6) NULL,
    CONSTRAINT [PK_ZnodePrice] PRIMARY KEY CLUSTERED ([PriceId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePrice_ZnodePriceList] FOREIGN KEY ([PriceListId]) REFERENCES [dbo].[ZnodePriceList] ([PriceListId]),
    CONSTRAINT [FK_ZnodePrice_ZnodeUom] FOREIGN KEY ([UomId]) REFERENCES [dbo].[ZnodeUom] ([UomId])
);












GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePrice_SKU]
    ON [dbo].[ZnodePrice]([SKU] ASC)
    INCLUDE([PriceListId], [SalesPrice], [RetailPrice]);

