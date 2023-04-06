CREATE TABLE [dbo].[ZnodeCMSSearchIndex] (
    [CMSSearchIndexId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]         INT            NOT NULL,
    [IndexName]        NVARCHAR (100) NOT NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([CMSSearchIndexId] ASC)
);

