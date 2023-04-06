CREATE TYPE [dbo].[ProductDetailsFromWebStore] AS TABLE (
    [Id]                INT            NULL,
    [ProductType]       NVARCHAR (100) NULL,
    [OutOfStockOptions] NVARCHAR (100) NULL,
    [SKU]               NVARCHAR (MAX) NULL);



