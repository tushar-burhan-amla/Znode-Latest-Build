CREATE TABLE [dbo].[ZnodeCMSContentPagesProfile] (
    [CMSContentPagesProfileId] INT      IDENTITY (1, 1) NOT NULL,
    [ProfileId]                INT      NULL,
    [CMSContentPagesId]        INT      NOT NULL,
    [CreatedBy]                INT      NOT NULL,
    [CreatedDate]              DATETIME NOT NULL,
    [ModifiedBy]               INT      NOT NULL,
    [ModifiedDate]             DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeCMSContentPagesProfile] PRIMARY KEY CLUSTERED ([CMSContentPagesProfileId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeCMSContentPagesProfile_ZnodeCMSContentPages] FOREIGN KEY ([CMSContentPagesId]) REFERENCES [dbo].[ZnodeCMSContentPages] ([CMSContentPagesId]),
    CONSTRAINT [FK_ZnodeCMSContentPagesProfile_ZnodeProfile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId])
);



