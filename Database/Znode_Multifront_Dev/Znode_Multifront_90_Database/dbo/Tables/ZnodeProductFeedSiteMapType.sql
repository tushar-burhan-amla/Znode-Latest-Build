CREATE TABLE [dbo].[ZnodeProductFeedSiteMapType] (
    [ProductFeedSiteMapTypeId]   INT            IDENTITY (1, 1) NOT NULL,
    [ProductFeedSiteMapTypeCode] NVARCHAR (50)  NOT NULL,
    [ProductFeedSiteMapTypeName] NVARCHAR (100) NULL,
    [CreatedBy]                  INT            NOT NULL,
    [CreatedDate]                DATETIME       NOT NULL,
    [ModifiedBy]                 INT            NOT NULL,
    [ModifiedDate]               DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeProductFeedSiteMapType] PRIMARY KEY CLUSTERED ([ProductFeedSiteMapTypeId] ASC)
);

