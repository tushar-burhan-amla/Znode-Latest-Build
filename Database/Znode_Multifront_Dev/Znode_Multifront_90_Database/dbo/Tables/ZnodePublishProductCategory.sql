CREATE TABLE [dbo].[ZnodePublishProductCategory] (
    [PublishProductCategoryId] INT      IDENTITY (1, 1) NOT NULL,
    [PublishProductId]         INT      NOT NULL,
    [PublishCategoryId]        INT      NOT NULL,
    [CreatedBy]                INT      NOT NULL,
    [CreatedDate]              DATETIME NOT NULL,
    [ModifiedBy]               INT      NOT NULL,
    [ModifiedDate]             DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePublishProductCategory] PRIMARY KEY CLUSTERED ([PublishProductCategoryId] ASC)
);

