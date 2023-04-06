CREATE TABLE [dbo].[ZnodeProductFeed] (
    [ProductFeedId]            INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                 INT            NOT NULL,
    [ProductFeedSiteMapTypeId] INT            NOT NULL,
    [ProductFeedTypeId]        INT            NOT NULL,
    [Title]                    NVARCHAR (MAX) NULL,
    [Link]                     NVARCHAR (MAX) NULL,
    [Description]              NVARCHAR (MAX) NULL,
    [FileName]                 NVARCHAR (100) NULL,
    [CreatedBy]                INT            NOT NULL,
    [CreatedDate]              DATETIME       NOT NULL,
    [ModifiedBy]               INT            NOT NULL,
    [ModifiedDate]             DATETIME       NOT NULL,
    [PortalId] INT NOT NULL, 
    [FileCount] INT NOT NULL, 
    CONSTRAINT [PK_ZnodeProductFeed] PRIMARY KEY CLUSTERED ([ProductFeedId] ASC),
    CONSTRAINT [FK_ZnodeProductFeed_ZnodeLocale1] FOREIGN KEY ([LocaleId]) REFERENCES [dbo].[ZnodeLocale] ([LocaleId]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_ZnodeProductFeed_ZnodeProductFeedSiteMapType] FOREIGN KEY ([ProductFeedSiteMapTypeId]) REFERENCES [dbo].[ZnodeProductFeedSiteMapType] ([ProductFeedSiteMapTypeId]),
    CONSTRAINT [FK_ZnodeProductFeed_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodeProductFeed_ZnodeProductFeedType] FOREIGN KEY ([ProductFeedTypeId]) REFERENCES [dbo].[ZnodeProductFeedType] ([ProductFeedTypeId]) ON DELETE CASCADE ON UPDATE CASCADE
);





