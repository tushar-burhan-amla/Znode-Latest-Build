CREATE TYPE [dbo].[OMSDownloadableProduct] AS TABLE (
    [OmsOrderLineItemsId] INT             NULL,
    [SKU]                 VARCHAR (300)   NULL,
    [Quantity]            NUMERIC (26, 8) NULL);

