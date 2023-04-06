CREATE TABLE [dbo].[ZnodePublishPortalLog] (
    [PublishPortalLogId]  INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]            INT            NOT NULL,
    [IsPortalPublished]   BIT            NULL,
    [PublishCategoryId]   VARCHAR (MAX)  NULL,
    [IsCategoryPublished] BIT            NULL,
    [UserId]              INT            NULL,
    [LogDateTime]         DATETIME       NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    [Token]               NVARCHAR (MAX) NULL,
    [PublishStateId]      TINYINT        CONSTRAINT [DF_ZnodePublishPortalLog_PublishStateId] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ZnodePublishPortalLog] PRIMARY KEY CLUSTERED ([PublishPortalLogId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePublishPortalLog_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);





