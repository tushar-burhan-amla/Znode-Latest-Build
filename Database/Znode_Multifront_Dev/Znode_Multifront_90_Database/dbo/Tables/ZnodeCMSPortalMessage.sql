CREATE TABLE [dbo].[ZnodeCMSPortalMessage] (
    [CMSPortalMessageId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]           INT      NULL,
    [CMSMessageKeyId]    INT      NOT NULL,
    [CMSMessageId]       INT      NOT NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCMSPortalMessage] PRIMARY KEY NONCLUSTERED ([CMSPortalMessageId] ASC),
    CONSTRAINT [FK_ZnodeCMSPortalMessage_ZnodeCMSMessage] FOREIGN KEY ([CMSMessageId]) REFERENCES [dbo].[ZnodeCMSMessage] ([CMSMessageId]),
    CONSTRAINT [FK_ZnodeCMSPortalMessage_ZnodeCMSMessageKey] FOREIGN KEY ([CMSMessageKeyId]) REFERENCES [dbo].[ZnodeCMSMessageKey] ([CMSMessageKeyId]),
    CONSTRAINT [FK_ZnodeCMSPortalMessage_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);











