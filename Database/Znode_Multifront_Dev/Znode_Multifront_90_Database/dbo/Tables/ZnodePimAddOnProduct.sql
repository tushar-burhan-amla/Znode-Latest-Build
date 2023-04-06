CREATE TABLE [dbo].[ZnodePimAddOnProduct] (
    [PimAddOnProductId] INT          IDENTITY (1, 1) NOT NULL,
    [PimAddonGroupId]   INT          NOT NULL,
    [PimProductId]      INT          NOT NULL,
    [DisplayOrder]      INT          NOT NULL,
    [RequiredType]      VARCHAR (50) CONSTRAINT [DF_ZnodePimAddOnProduct_IsRequired] DEFAULT ((0)) NOT NULL,
    [CreatedBy]         INT          NOT NULL,
    [CreatedDate]       DATETIME     NOT NULL,
    [ModifiedBy]        INT          NOT NULL,
    [ModifiedDate]      DATETIME     NOT NULL,
    CONSTRAINT [PK_ZnodePimAddOnDetail] PRIMARY KEY CLUSTERED ([PimAddOnProductId] ASC),
    CONSTRAINT [FK_ZnodePimAddOnDetail_ZnodePimAddonGroup] FOREIGN KEY ([PimAddonGroupId]) REFERENCES [dbo].[ZnodePimAddonGroup] ([PimAddonGroupId]),
    CONSTRAINT [FK_ZnodePimAddOnDetail_ZnodePimProduct_PimParentProductId] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);





