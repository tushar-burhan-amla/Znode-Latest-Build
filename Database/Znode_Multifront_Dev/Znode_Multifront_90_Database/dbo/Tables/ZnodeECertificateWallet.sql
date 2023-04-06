CREATE TABLE [dbo].[ZnodeECertificateWallet] (
    [ECertificateWalletId] INT             IDENTITY (1, 1) NOT NULL,
    [ECertificateId]       INT             NULL,
    [IssuedByUserId]       INT             NULL,
    [IssuedToUserId]       INT             NULL,
    [BalanceAmount]        NUMERIC (28, 6) NOT NULL,
    [Custom1]              NVARCHAR (MAX)  NULL,
    [Custom2]              NVARCHAR (MAX)  NULL,
    [Custom3]              NVARCHAR (MAX)  NULL,
    [Custom4]              NVARCHAR (MAX)  NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeECertificateWallet] PRIMARY KEY CLUSTERED ([ECertificateWalletId] ASC),
    CONSTRAINT [FK_ZnodeECertificateWallet_ZnodeECertificate] FOREIGN KEY ([ECertificateId]) REFERENCES [dbo].[ZnodeECertificate] ([ECertificateId]),
    CONSTRAINT [FK_ZnodeECertificateWallet_ZnodeUser_IssuedByUserId] FOREIGN KEY ([IssuedByUserId]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeECertificateWallet_ZnodeUser_IssuedToUserId] FOREIGN KEY ([IssuedToUserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);

