CREATE TABLE [dbo].[ZnodeOmsCookieMapping] (
    [OmsCookieMappingId] INT      IDENTITY (1, 1) NOT NULL,
    [UserId]             INT      NULL,
    [PortalId]           INT      NOT NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeOmsCookieMapping] PRIMARY KEY CLUSTERED ([OmsCookieMappingId] ASC),
    CONSTRAINT [FK_ZnodeOmsCookieMapping_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);



