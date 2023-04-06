CREATE TABLE [dbo].[ZnodeCMSPortalSlider] (
    [CMSPortalSliderId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]          INT            NOT NULL,
    [Name]              NVARCHAR (100) NOT NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSPortalSlider] PRIMARY KEY CLUSTERED ([CMSPortalSliderId] ASC)
);

