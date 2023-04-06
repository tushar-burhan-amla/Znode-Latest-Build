CREATE TABLE [dbo].[ZnodePriceListDiscount] (
    [PriceListDiscountId] INT             IDENTITY (1, 1) NOT NULL,
    [PriceListId]         INT             NOT NULL,
    [DiscountAmount]      NUMERIC (28, 6) NOT NULL,
    [IsPercentage]        BIT             CONSTRAINT [DF__ZnodePric__IsPer__161A357F] DEFAULT ((0)) NOT NULL,
    [ActivationDate]      DATETIME        NOT NULL,
    [ExpirationDate]      DATETIME        NOT NULL,
    [CreatedBy]           INT             NOT NULL,
    [CreatedDate]         DATETIME        NOT NULL,
    [ModifiedBy]          INT             NOT NULL,
    [ModifiedDate]        DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePriceListDiscount] PRIMARY KEY CLUSTERED ([PriceListDiscountId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePriceListDiscount_ZnodePriceList] FOREIGN KEY ([PriceListId]) REFERENCES [dbo].[ZnodePriceList] ([PriceListId])
);



