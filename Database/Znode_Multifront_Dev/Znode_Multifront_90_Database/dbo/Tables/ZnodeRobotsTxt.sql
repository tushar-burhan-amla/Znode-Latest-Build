CREATE TABLE [dbo].[ZnodeRobotsTxt] (
    [RobotsTxtId]      INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]         INT            NULL,
    [RobotsTxtContent] NVARCHAR (MAX) NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    [DefaultRobotTag]  VARCHAR (50)   NULL,
    CONSTRAINT [PK_ZnodeRobotsTxt] PRIMARY KEY CLUSTERED ([RobotsTxtId] ASC),
    CONSTRAINT [FK_ZnodeRobotsTxt_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);



