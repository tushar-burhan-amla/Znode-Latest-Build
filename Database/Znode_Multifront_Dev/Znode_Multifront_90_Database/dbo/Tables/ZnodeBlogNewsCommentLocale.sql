CREATE TABLE [dbo].[ZnodeBlogNewsCommentLocale] (
    [BlogNewsCommentLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [BlogNewsCommentId]       INT            NULL,
    [BlogComment]             NVARCHAR (MAX) NULL,
    [LocaleId]                INT            NULL,
    [CreatedBy]               INT            NOT NULL,
    [CreatedDate]             DATETIME       NOT NULL,
    [ModifiedBy]              INT            NOT NULL,
    [ModifiedDate]            DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeBlogCommentLocale] PRIMARY KEY CLUSTERED ([BlogNewsCommentLocaleId] ASC),
    CONSTRAINT [FK_ZnodeBlogCommentLocale_ZnodeBlogComment] FOREIGN KEY ([BlogNewsCommentId]) REFERENCES [dbo].[ZnodeBlogNewsComment] ([BlogNewsCommentId])
);

