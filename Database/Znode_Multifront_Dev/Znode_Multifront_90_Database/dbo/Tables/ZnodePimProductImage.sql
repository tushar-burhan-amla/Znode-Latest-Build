CREATE TABLE [dbo].[ZnodePimProductImage] (
    [PimProductImageId] INT      IDENTITY (1, 1) NOT NULL,
    [PimProductId]      INT      NULL,
    [PimAttributeId]    INT      NULL,
    [MediaId]           INT      NULL,
    [CreatedBy]         INT      NULL,
    [CreatedDate]       DATETIME NULL,
    [ModifiedBy]        INT      NULL,
    [ModifiedDate]      DATETIME NULL,
    CONSTRAINT [PK_ZnodePimProductImage] PRIMARY KEY CLUSTERED ([PimProductImageId] ASC),
    CONSTRAINT [FK_ZnodePimProductImage_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId]),
    CONSTRAINT [FK_ZnodePimProductImage_ZnodePimProduct] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);



