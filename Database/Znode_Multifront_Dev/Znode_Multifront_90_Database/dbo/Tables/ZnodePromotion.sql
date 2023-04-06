CREATE TABLE [dbo].[ZnodePromotion] (
    [PromotionId]               INT             IDENTITY (1, 1) NOT NULL,
    [PromoCode]                 VARCHAR (300)   NOT NULL,
    [Name]                      NVARCHAR (300)  NOT NULL,
    [Description]               NVARCHAR (MAX)  NULL,
    [PromotionTypeId]           INT             NULL,
    [Discount]                  NUMERIC (28, 6) NULL,
    [StartDate]                 DATETIME        NULL,
    [EndDate]                   DATETIME        NULL,
    [OrderMinimum]              NUMERIC (28, 6) NULL,
    [QuantityMinimum]           NUMERIC (28, 6) NULL,
    [IsCouponRequired]          BIT             NULL,
    [IsAllowedWithOtherCoupons] BIT             CONSTRAINT [DF_ZnodePromotion_IsAllowedWithOtherCoupons] DEFAULT ((0)) NOT NULL,
    [PromotionMessage]          NVARCHAR (MAX)  NULL,
    [DisplayOrder]              INT             NULL,
    [IsUnique]                  BIT             CONSTRAINT [DF_ZnodePromotion_IsUnique] DEFAULT ((0)) NOT NULL,
    [PortalId]                  INT             NULL,
    [ProfileId]                 INT             NULL,
    [PromotionProductQuantity]  NUMERIC (28, 6) NULL,
    [ReferralPublishProductId]  INT             NULL,
    [CreatedBy]                 INT             NOT NULL,
    [CreatedDate]               DATETIME        NOT NULL,
    [ModifiedBy]                INT             NOT NULL,
    [ModifiedDate]              DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePromotion] PRIMARY KEY CLUSTERED ([PromotionId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePromotion_ZnodeProfile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId]),
    CONSTRAINT [FK_ZnodePromotion_ZnodePromotionType] FOREIGN KEY ([PromotionTypeId]) REFERENCES [dbo].[ZnodePromotionType] ([PromotionTypeId])
);
























GO

CREATE TRIGGER [dbo].[ZnodePromotion_AspNet_SqlCacheTablesForChangeNotification_Trigger] ON [dbo].[ZnodePromotion]
                       FOR INSERT, UPDATE, DELETE AS BEGIN
                       SET NOCOUNT ON
                       EXEC dbo.AspNet_SqlCacheUpdateChangeIdStoredProcedure N'ZnodePromotion'
                       END