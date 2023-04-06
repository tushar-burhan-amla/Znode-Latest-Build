CREATE TABLE [dbo].[ZnodeCMSAreaMessageKey] (
    [AreaCMSMessageKeyId] INT      IDENTITY (1, 1) NOT NULL,
    [CMSMessageKeyId]     INT      NULL,
    [CMSAreaId]           INT      NULL,
    [CreatedBy]           INT      NOT NULL,
    [CreatedDate]         DATETIME NOT NULL,
    [ModifiedBy]          INT      NOT NULL,
    [ModifiedDate]        DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeAreaCMSMessageKey] PRIMARY KEY CLUSTERED ([AreaCMSMessageKeyId] ASC),
    CONSTRAINT [FK_ZnodeAreaCMSMessageKey_ZnodeCMSArea] FOREIGN KEY ([CMSAreaId]) REFERENCES [dbo].[ZnodeCMSArea] ([CMSAreaId]),
    CONSTRAINT [FK_ZnodeAreaCMSMessageKey_ZnodeCMSMessageKey] FOREIGN KEY ([CMSMessageKeyId]) REFERENCES [dbo].[ZnodeCMSMessageKey] ([CMSMessageKeyId])
);

