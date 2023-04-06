CREATE TABLE [dbo].[ZnodePimDownloadableProduct] (
    [PimDownloadableProductId] INT            IDENTITY (1, 1) NOT NULL,
    [SKU]                      NVARCHAR (300) NOT NULL,
    [ProductName]              NVARCHAR (300) NOT NULL,
    [CreatedBy]                INT            NOT NULL,
    [CreatedDate]              DATETIME       NOT NULL,
    [ModifiedBy]               INT            NOT NULL,
    [ModifiedDate]             DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimDownloadableProduct] PRIMARY KEY CLUSTERED ([PimDownloadableProductId] ASC)
);



