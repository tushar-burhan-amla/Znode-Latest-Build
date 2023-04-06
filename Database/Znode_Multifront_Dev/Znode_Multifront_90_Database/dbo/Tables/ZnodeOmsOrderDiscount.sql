CREATE TABLE [dbo].[ZnodeOmsOrderDiscount] (
    [OmsOrderDiscountId]        INT             IDENTITY (1, 1) NOT NULL,
    [OmsOrderDetailsId]         INT             NULL,
    [OmsOrderLineItemId]        INT             NULL,
    [OmsDiscountTypeId]         INT             NULL,
    [DiscountCode]              VARCHAR (MAX)   NULL,
    [DiscountAmount]            NUMERIC (28, 6) NULL,
    [Description]               NVARCHAR (MAX)  NULL,
    [CreatedBy]                 INT             NOT NULL,
    [CreatedDate]               DATETIME        NOT NULL,
    [ModifiedBy]                INT             NOT NULL,
    [ModifiedDate]              DATETIME        NOT NULL,
    [PerQuantityDiscount]       NUMERIC (28, 6) NULL,
    [DiscountMultiplier]        NUMERIC (28, 6) NULL,
    [ParentOmsOrderLineItemsId] INT             NULL,
    [DiscountLevelTypeId]       INT             NULL,
    [PromotionName]             NVARCHAR (600)  NULL,
	[PromotionTypeId]           INT             NULL,
	[DiscountAppliedSequence]   INT             NULL,
	[PromotionMessage]          NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ZnodeOmsOrderDiscount] PRIMARY KEY CLUSTERED ([OmsOrderDiscountId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsOrderDiscount_ZnodeOmsDiscountType] FOREIGN KEY ([OmsDiscountTypeId]) REFERENCES [dbo].[ZnodeOmsDiscountType] ([OmsDiscountTypeId]),
    CONSTRAINT [FK_ZnodeOmsOrderDiscount_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId]),
    CONSTRAINT [FK_ZnodeOmsOrderDiscount_ZnodeOmsOrderLineItem] FOREIGN KEY ([OmsOrderLineItemId]) REFERENCES [dbo].[ZnodeOmsOrderLineItems] ([OmsOrderLineItemsId]),
    CONSTRAINT [FK_ZnodeOmsOrderDiscount_ZnodePromotionType] FOREIGN KEY ([PromotionTypeId]) REFERENCES [dbo].[ZnodePromotionType] ([PromotionTypeId])
);











