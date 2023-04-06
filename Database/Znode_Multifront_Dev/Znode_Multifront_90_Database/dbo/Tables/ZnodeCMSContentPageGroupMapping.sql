CREATE TABLE [dbo].[ZnodeCMSContentPageGroupMapping] (
    [CMSContentPageGroupMappingId] INT      IDENTITY (1, 1) NOT NULL,
    [CMSContentPageGroupId]        INT      NOT NULL,
    [CMSContentPagesId]            INT      NOT NULL,
    [CreatedBy]                    INT      NOT NULL,
    [CreatedDate]                  DATETIME NOT NULL,
    [ModifiedBy]                   INT      NOT NULL,
    [ModifiedDate]                 DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCMSContentPageGroupMapping] PRIMARY KEY CLUSTERED ([CMSContentPageGroupMappingId] ASC),
    CONSTRAINT [FK_ZnodeCMSContentPageGroupMapping_ZnodeCMSContentPageGroup] FOREIGN KEY ([CMSContentPageGroupId]) REFERENCES [dbo].[ZnodeCMSContentPageGroup] ([CMSContentPageGroupId]),
    CONSTRAINT [FK_ZnodeCMSContentPageGroupMapping_ZnodeCMSContentPages] FOREIGN KEY ([CMSContentPagesId]) REFERENCES [dbo].[ZnodeCMSContentPages] ([CMSContentPagesId])
);

