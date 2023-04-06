CREATE TABLE [dbo].[ZnodePriceList] (
    [PriceListId]    INT           IDENTITY (1, 1) NOT NULL,
    [ListCode]       VARCHAR (200) NOT NULL,
    [ListName]       VARCHAR (600) NULL,
    [CurrencyId]     INT           NULL,
    [ActivationDate] DATETIME      NULL,
    [ExpirationDate] DATETIME      NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     INT           NOT NULL,
    [ModifiedDate]   DATETIME      NOT NULL,
    [CultureId]      INT           NULL,
    CONSTRAINT [PK_ZnodePriceList] PRIMARY KEY CLUSTERED ([PriceListId] ASC),
    CONSTRAINT [FK_ZnodePriceList_ZnodeCulture] FOREIGN KEY ([CultureId]) REFERENCES [dbo].[ZnodeCulture] ([CultureId]),
    CONSTRAINT [FK_ZnodePriceList_ZnodeCurrency] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[ZnodeCurrency] ([CurrencyId])
);











