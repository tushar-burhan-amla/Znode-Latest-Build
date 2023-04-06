CREATE TABLE [dbo].[ZnodePimAddOnDetail] (
    [PimAddOnDetailId]   INT      IDENTITY (1, 1) NOT NULL,
    [PimParentProductId] INT      NULL,
    [PimAddonGroupId]    INT      NULL,
    [PimProductId]       INT      NULL,
    [CreatedBy]          INT      NULL,
    [CreatedDate]        DATETIME NULL,
    [ModifiedBy]         INT      NULL,
    [ModifiedDate]       DATETIME NULL,
    CONSTRAINT [PK_ZnodePimAddOnDetail] PRIMARY KEY CLUSTERED ([PimAddOnDetailId] ASC),
    CONSTRAINT [FK_ZnodePimAddOnDetail_ZnodePimAddonGroup] FOREIGN KEY ([PimAddonGroupId]) REFERENCES [dbo].[ZnodePimAddonGroup] ([PimAddonGroupId]),
    CONSTRAINT [FK_ZnodePimAddOnDetail_ZnodePimProduct] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId]),
    CONSTRAINT [FK_ZnodePimAddOnDetail_ZnodePimProduct_PimParentProductId] FOREIGN KEY ([PimParentProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);



