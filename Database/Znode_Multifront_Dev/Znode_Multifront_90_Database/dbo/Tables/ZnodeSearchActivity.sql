CREATE TABLE [dbo].[ZnodeSearchActivity] (
    [SearchActivityId]      INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]              INT            NOT NULL,
    [UserId]                INT            NULL,
    [SearchProfileId]       INT            NULL,
    [SearchKeyword]         VARCHAR (2000) NOT NULL,
    [TransformationKeyword] VARCHAR (2000) NULL,
    [ResultCount]           INT            NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    [UserProfileId]         INT            NULL,
    CONSTRAINT [PK_ZnodeSearchActivity] PRIMARY KEY CLUSTERED ([SearchActivityId] ASC),
    CONSTRAINT [FK_ZnodeSearchActivity_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodeSearchActivity_ZnodeSearchProfile] FOREIGN KEY ([SearchProfileId]) REFERENCES [dbo].[ZnodeSearchProfile] ([SearchProfileId]),
    CONSTRAINT [FK_ZnodeSearchActivity_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);

