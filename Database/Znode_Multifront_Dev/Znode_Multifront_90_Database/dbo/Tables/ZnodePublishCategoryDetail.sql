CREATE TABLE [dbo].[ZnodePublishCategoryDetail] (
    [PublishCategoryInfoId] INT             IDENTITY (1, 1) NOT NULL,
    [PublishCategoryId]     INT             NULL,
    [PublishCategoryName]   NVARCHAR (2000) NULL,
    [IsActive]              BIT             CONSTRAINT [DF_ZnodePublishCategoryDetail_IsActive] DEFAULT ((0)) NULL,
    [LocaleId]              INT             NULL,
    [CreatedBy]             INT             NOT NULL,
    [CreatedDate]           DATETIME        NOT NULL,
    [ModifiedBy]            INT             NOT NULL,
    [ModifiedDate]          DATETIME        NOT NULL,
    [CategoryCode]          NVARCHAR (4000) NULL,
    CONSTRAINT [PK_ZnodePublishCategoryDetail] PRIMARY KEY CLUSTERED ([PublishCategoryInfoId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePublishCategoryDetail_ZnodePublishCategory] FOREIGN KEY ([PublishCategoryId]) REFERENCES [dbo].[ZnodePublishCategory] ([PublishCategoryId])
);














GO



GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryDetail_PublishCategoryId_LocaleId]
    ON [dbo].[ZnodePublishCategoryDetail]([PublishCategoryId] ASC, [LocaleId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryDetail_PublishCategoryId]
    ON [dbo].[ZnodePublishCategoryDetail]([PublishCategoryId] ASC);

GO


