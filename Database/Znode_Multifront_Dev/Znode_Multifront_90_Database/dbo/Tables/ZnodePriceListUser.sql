CREATE TABLE [dbo].[ZnodePriceListUser] (
    [PriceListUserId] INT      IDENTITY (1, 1) NOT NULL,
    [PriceListId]     INT      NOT NULL,
    [UserId]          INT      NOT NULL,
    [Precedence]      INT      NULL,
    [CreatedBy]       INT      NOT NULL,
    [CreatedDate]     DATETIME NOT NULL,
    [ModifiedBy]      INT      NOT NULL,
    [ModifiedDate]    DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePriceListAccount] PRIMARY KEY CLUSTERED ([PriceListUserId] ASC),
    CONSTRAINT [FK_ZnodePriceListAccount_ZnodePriceList] FOREIGN KEY ([PriceListId]) REFERENCES [dbo].[ZnodePriceList] ([PriceListId]),
    CONSTRAINT [FK_ZnodePriceListUser_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);



