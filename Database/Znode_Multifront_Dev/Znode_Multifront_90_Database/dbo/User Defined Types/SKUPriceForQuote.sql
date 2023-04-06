CREATE TYPE [dbo].[SKUPriceForQuote] AS TABLE (
    [SKU]                    VARCHAR (600)   NULL,
    [OmsSavedCartLineItemId] INT             NULL,
    [Price]                  NUMERIC (28, 6) NULL,
    [ShippingCost]           NUMERIC (28, 6) NULL,
    [InitialPrice]           NUMERIC (28, 6) NULL,
    [InitialShippingCost]    NUMERIC (28, 6) NULL,
    [IsPriceEdit]            BIT             NULL,
    [Custom1]                NVARCHAR (MAX)  NULL,
    [Custom2]                NVARCHAR (MAX)  NULL,
    [Custom3]                NVARCHAR (MAX)  NULL,
    [Custom4]                NVARCHAR (MAX)  NULL,
    [Custom5]                NVARCHAR (MAX)  NULL);


GO