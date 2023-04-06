CREATE TABLE [dbo].[ZnodeCMSUrlRedirect] (
    [CMSUrlRedirectId] INT            IDENTITY (1, 1) NOT NULL,
    [RedirectFrom]     NVARCHAR (MAX) NOT NULL,
    [RedirectTo]       NVARCHAR (MAX) NOT NULL,
    [IsActive]         BIT            NOT NULL,
    [PortalId]         INT            NOT NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSUrlRedirect] PRIMARY KEY CLUSTERED ([CMSUrlRedirectId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeCMSUrlRedirect_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);







