CREATE TABLE [dbo].[ZnodeCMSContentPages] (
    [CMSContentPagesId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]          INT            NOT NULL,
    [CMSTemplateId]     INT            NOT NULL,
    [PageName]          NVARCHAR (100) NOT NULL,
    [ActivationDate]    DATETIME       NULL,
    [ExpirationDate]    DATETIME       NULL,
    [IsActive]          BIT            NOT NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    [IsPublished]       BIT            NULL,
    [PublishStateId]    TINYINT        CONSTRAINT [DF_ZnodeCMSContentPages_PublishStateId] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ZnodeCMSContentPages] PRIMARY KEY CLUSTERED ([CMSContentPagesId] ASC),
    CONSTRAINT [FK_ZnodeCMSContentPages_ZnodeCMSTemplate] FOREIGN KEY ([CMSTemplateId]) REFERENCES [dbo].[ZnodeCMSTemplate] ([CMSTemplateId]),
    CONSTRAINT [FK_ZnodeCMSContentPages_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);





















