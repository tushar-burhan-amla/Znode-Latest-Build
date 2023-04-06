CREATE TABLE [dbo].[ZnodeGiftCard] (
    [GiftCardId]                INT             IDENTITY (1, 1) NOT NULL,
    [PortalId]                  INT             NOT NULL,
    [Name]                      NVARCHAR (300)  NOT NULL,
    [CardNumber]                NVARCHAR (300)  NOT NULL,
    [Amount]                    NUMERIC (28, 6) NOT NULL,
    [UserId]                    INT             NULL,
    [ExpirationDate]            DATETIME        NULL,
    [IsReferralCommission]      BIT             NULL,
    [CreatedBy]                 INT             NOT NULL,
    [CreatedDate]               DATETIME        NOT NULL,
    [ModifiedBy]                INT             NOT NULL,
    [ModifiedDate]              DATETIME        NOT NULL,
    [IsActive]                  BIT             CONSTRAINT [DF_ZnodeGiftCard_IsActive] DEFAULT ((0)) NOT NULL,
    [RemainingAmount]           NUMERIC (26, 6) NULL,
    [RestrictToCustomerAccount] BIT             CONSTRAINT [DF_ZnodeGiftCard_RestrictToCustomerAccount] DEFAULT ((0)) NOT NULL,
    [StartDate]                 DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeGiftCard] PRIMARY KEY CLUSTERED ([GiftCardId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeGiftCard_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);











