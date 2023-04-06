CREATE TABLE [dbo].[ZnodePriceListAccount] (
    [PriceListAccountId] INT      IDENTITY (1, 1) NOT NULL,
    [PriceListId]        INT      NOT NULL,
    [AccountId]          INT      NOT NULL,
    [Precedence]         INT      NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePriceListAccount_PriceListAccountId] PRIMARY KEY CLUSTERED ([PriceListAccountId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePriceListAccount_ZnodeAccount_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId]),
    CONSTRAINT [FK_ZnodePriceListAccount_ZnodePriceList_PriceListId] FOREIGN KEY ([PriceListId]) REFERENCES [dbo].[ZnodePriceList] ([PriceListId])
);



