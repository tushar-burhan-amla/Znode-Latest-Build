CREATE TABLE [dbo].[ZnodeBlogCommentLocale] (
    [BlogCommentLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [BlogCommentId]       INT            NULL,
    [BlogComment]         NVARCHAR (MAX) NULL,
    [LocaleId]            INT            NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeBlogCommentLocale] PRIMARY KEY CLUSTERED ([BlogCommentLocaleId] ASC),
    CONSTRAINT [FK_ZnodeBlogCommentLocale_ZnodeBlogComment] FOREIGN KEY ([BlogCommentId]) REFERENCES [dbo].[ZnodeBlogComment] ([BlogCommentId])
);

