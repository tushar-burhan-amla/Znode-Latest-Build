CREATE TABLE [dbo].[ZnodeCMSContentPageGroup] (
    [CMSContentPageGroupId]       INT            IDENTITY (1, 1) NOT NULL,
    [ParentCMSContentPageGroupId] INT            NULL,
    [Code]                        NVARCHAR (100) NOT NULL,
    [CreatedBy]                   INT            NOT NULL,
    [CreatedDate]                 DATETIME       NOT NULL,
    [ModifiedBy]                  INT            NOT NULL,
    [ModifiedDate]                DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSContentPageGroup] PRIMARY KEY CLUSTERED ([CMSContentPageGroupId] ASC),
    CONSTRAINT [FK_ZnodeCMSContentPageGroup_ZnodeCMSContentPageGroup] FOREIGN KEY ([ParentCMSContentPageGroupId]) REFERENCES [dbo].[ZnodeCMSContentPageGroup] ([CMSContentPageGroupId])
);



