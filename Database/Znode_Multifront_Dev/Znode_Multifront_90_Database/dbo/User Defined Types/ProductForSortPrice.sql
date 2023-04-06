CREATE TYPE [dbo].[ProductForSortPrice] AS TABLE (
    [Id]                INT           NOT NULL,
    [ProductType]       VARCHAR (200) NULL,
    [OutOfStockOptions] VARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC));



