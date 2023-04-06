CREATE TABLE [dbo].[ZnodePromotionDiscountAttributeMapper] (
    [PromotionDiscountAttributeMapperId] INT            IDENTITY (1, 1) NOT NULL,
    [DiscountTypeName]                   NVARCHAR (500) NULL,
    [PromotionAttributeId]               INT            NULL,
    [CreatedBy]                          INT            NOT NULL,
    [CreatedDate]                        DATETIME       NOT NULL,
    [ModifiedBy]                         INT            NOT NULL,
    [ModifiedDate]                       DATETIME       NOT NULL,
    CONSTRAINT [FK_ZnodePromotionDiscountAttributeMapper_ZnodePromotionAttribute] FOREIGN KEY ([PromotionAttributeId]) REFERENCES [dbo].[ZnodePromotionAttribute] ([PromotionAttributeId])
);





