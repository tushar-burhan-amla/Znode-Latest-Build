CREATE TABLE [dbo].[ZnodePimAddonGroupProduct] (
    [PimAddonGroupProductId] INT      IDENTITY (1, 1) NOT NULL,
    [PimAddonGroupId]        INT      NULL,
    [PimChildProductId]      INT      NULL,
    [CreatedBy]              INT      NULL,
    [CreatedDate]            DATETIME NULL,
    [ModifiedBy]             INT      NULL,
    [ModifiedDate]           DATETIME NULL,
    CONSTRAINT [PK_ZnodePimAddonGroupProduct] PRIMARY KEY CLUSTERED ([PimAddonGroupProductId] ASC),
    CONSTRAINT [FK_ZnodePimAddonGroupProduct_ZnodePimAddonGroup] FOREIGN KEY ([PimAddonGroupId]) REFERENCES [dbo].[ZnodePimAddonGroup] ([PimAddonGroupId]),
    CONSTRAINT [FK_ZnodePimAddonGroupProduct_ZnodePimProduct] FOREIGN KEY ([PimChildProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);



