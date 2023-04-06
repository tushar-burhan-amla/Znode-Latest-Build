CREATE TABLE [dbo].[ZnodePromotionBrand] (
    [PromotionBrandId] INT            IDENTITY (1, 1) NOT NULL,
    [PromotionId]      INT            NOT NULL,
    [BrandId]          INT            NULL,
    [BrandCode]        NVARCHAR (100) NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifedBy]        INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePromotionBrand] PRIMARY KEY CLUSTERED ([PromotionBrandId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePromotionBrand_ZnodeBrandDetails] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[ZnodeBrandDetails] ([BrandId]),
    CONSTRAINT [FK_ZnodePromotionBrand_ZnodePromotion] FOREIGN KEY ([PromotionId]) REFERENCES [dbo].[ZnodePromotion] ([PromotionId])
);





