CREATE TABLE [dbo].[ZnodeBlogNews] (
    [BlogNewsId]          INT             IDENTITY (1, 1) NOT NULL,
    [PortalId]            INT             NULL,
    [MediaId]             INT             NULL,
    [CMSContentPagesId]   INT             NULL,
    [BlogNewsType]        VARCHAR (300)   NULL,
    [IsBlogNewsActive]    BIT             NULL,
    [IsAllowGuestComment] BIT             NULL,
    [CreatedBy]           INT             NOT NULL,
    [CreatedDate]         DATETIME        NOT NULL,
    [ModifiedBy]          INT             NOT NULL,
    [ModifiedDate]        DATETIME        NOT NULL,
    [ActivationDate]      DATETIME        NULL,
    [ExpirationDate]      DATETIME        NULL,
    [BlogNewsCode]        NVARCHAR (2000) NULL,
	[PublishStateId]	  TINYINT NOT NULL,
    CONSTRAINT [PK_ZnodeBlogNews] PRIMARY KEY CLUSTERED ([BlogNewsId] ASC),
    CONSTRAINT [FK_ZnodeBlogNews_ZnodeCMSContentPages] FOREIGN KEY ([CMSContentPagesId]) REFERENCES [dbo].[ZnodeCMSContentPages] ([CMSContentPagesId]),
    CONSTRAINT [FK_ZnodeBlogNews_ZnodeMedia] FOREIGN KEY ([MediaId]) REFERENCES [dbo].[ZnodeMedia] ([MediaId]),
    CONSTRAINT [FK_ZnodeBlogNews_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);





