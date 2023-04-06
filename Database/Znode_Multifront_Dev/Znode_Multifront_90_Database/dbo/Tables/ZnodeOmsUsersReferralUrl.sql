CREATE TABLE [dbo].[ZnodeOmsUsersReferralUrl] (
    [OmsUsersReferralUrlId] INT            IDENTITY (1, 1) NOT NULL,
    [UserId]                INT            NOT NULL,
    [PortalId]              INT            NOT NULL,
    [DomainId]              INT            NOT NULL,
    [URL]                   NVARCHAR (MAX) NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeOmsUsersReferralUrl] PRIMARY KEY CLUSTERED ([OmsUsersReferralUrlId] ASC),
    CONSTRAINT [FK_ZnodeOmsReferralCommission_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeOmsUsersReferralUrl_ZnodeDomain] FOREIGN KEY ([DomainId]) REFERENCES [dbo].[ZnodeDomain] ([DomainId])
);

