CREATE TABLE [dbo].[ZnodeGiftCardLocale] (
    [GiftCardLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [GiftCardId]       INT            NOT NULL,
    [LocaleId]         INT            NOT NULL,
    [Name]             NVARCHAR (MAX) NOT NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeGiftCardLocale] PRIMARY KEY CLUSTERED ([GiftCardLocaleId] ASC),
    CONSTRAINT [FK_ZnodeGiftCardLocale_ZnodeGiftCard] FOREIGN KEY ([GiftCardId]) REFERENCES [dbo].[ZnodeGiftCard] ([GiftCardId])
);



