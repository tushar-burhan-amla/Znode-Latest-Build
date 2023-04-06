CREATE TABLE [dbo].[ZnodePublishCatalogLog] (
    [PublishCatalogLogId] INT            IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId]    INT            NULL,
    [PimCatalogId]        INT            NOT NULL,
    [IsCatalogPublished]  BIT            CONSTRAINT [DF__ZnodePubl__IsCat__0CA5D9DE] DEFAULT ((0)) NULL,
    [PublishCategoryId]   VARCHAR (MAX)  NULL,
    [IsCategoryPublished] BIT            CONSTRAINT [DF__ZnodePubl__IsCat__0E8E2250] DEFAULT ((0)) NULL,
    [PublishProductId]    VARCHAR (MAX)  NULL,
    [IsProductPublished]  BIT            CONSTRAINT [DF__ZnodePubl__IsPro__0D99FE17] DEFAULT ((0)) NULL,
    [UserId]              INT            NULL,
    [LogDateTime]         DATETIME       NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    [Token]               NVARCHAR (MAX) NULL,
    [LocaleId]            INT            NULL,
    [PublishStateId]      TINYINT        NULL,
    [PublishType]         VARCHAR (500)  NULL,
    CONSTRAINT [PK_ZnodePublishCatalogLog] PRIMARY KEY CLUSTERED ([PublishCatalogLogId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePublishCatalogLog_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId]),
    CONSTRAINT [FK_ZnodePublishCatalogLog_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);


















GO
CREATE NONCLUSTERED INDEX [idx_ZnodePublishCatalogLog_PublishCatalogId_LocaleId]
    ON [dbo].[ZnodePublishCatalogLog]([PublishCatalogId] ASC, [LocaleId] ASC);

