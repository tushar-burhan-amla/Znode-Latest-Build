CREATE TABLE [dbo].[ZnodeCMSPortalMessageKeyTag] (
    [CMSPortalMessageKeyTagId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]                 INT            NULL,
    [CMSMessageKeyId]          INT            NULL,
    [TagXML]                   NVARCHAR (MAX) NULL,
    [CreatedBy]                INT            NOT NULL,
    [CreatedDate]              DATETIME       NOT NULL,
    [ModifiedBy]               INT            NOT NULL,
    [ModifiedDate]             DATETIME       NULL,
    CONSTRAINT [PK_ZnodeCMSPortalMessageKeyTag] PRIMARY KEY CLUSTERED ([CMSPortalMessageKeyTagId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeCMSPortalMessageKeyTag_ZnodeCMSMessageKey] FOREIGN KEY ([CMSMessageKeyId]) REFERENCES [dbo].[ZnodeCMSMessageKey] ([CMSMessageKeyId]),
    CONSTRAINT [FK_ZnodeCMSPortalMessageKeyTag_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);





