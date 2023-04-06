CREATE TABLE [dbo].[ZnodeCMSPortalSEOSetting] (
    [CMSPortalSEOSettingId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]              INT            NOT NULL,
    [CategoryTitle]         NVARCHAR (MAX) NULL,
    [CategoryDescription]   NVARCHAR (MAX) NULL,
    [CategoryKeyword]       NVARCHAR (MAX) NULL,
    [ProductTitle]          NVARCHAR (MAX) NULL,
    [ProductDescription]    NVARCHAR (MAX) NULL,
    [ProductKeyword]        NVARCHAR (MAX) NULL,
    [ContentTitle]          NVARCHAR (MAX) NULL,
    [ContentDescription]    NVARCHAR (MAX) NULL,
    [ContentKeyword]        NVARCHAR (MAX) NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSPortalSEOSetting] PRIMARY KEY CLUSTERED ([CMSPortalSEOSettingId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeCMSPortalSEOSetting_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);







