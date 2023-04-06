CREATE TABLE [dbo].[ZnodeCMSPortalProductPage] (
    [CMSPortalProductPageId] INT           IDENTITY (1, 1) NOT NULL,
    [PortalId]               INT           NOT NULL,
    [ProductType]            VARCHAR (500) NOT NULL,
    [TemplateName]           VARCHAR (500) NOT NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCMSPortalProductPage] PRIMARY KEY CLUSTERED ([CMSPortalProductPageId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeCMSPortalProductPage_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);



