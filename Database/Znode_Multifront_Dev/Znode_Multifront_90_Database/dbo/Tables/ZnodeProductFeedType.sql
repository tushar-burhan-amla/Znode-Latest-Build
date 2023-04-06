CREATE TABLE [dbo].[ZnodeProductFeedType] (
    [ProductFeedTypeId]   INT            IDENTITY (1, 1) NOT NULL,
    [ProductFeedTypeCode] NVARCHAR (50)  NOT NULL,
    [ProductFeedTypeName] NVARCHAR (100) NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeProductFeedType] PRIMARY KEY CLUSTERED ([ProductFeedTypeId] ASC)
);

