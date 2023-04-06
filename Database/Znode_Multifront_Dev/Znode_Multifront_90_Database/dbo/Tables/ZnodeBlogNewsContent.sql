CREATE TABLE [dbo].[ZnodeBlogNewsContent] (
    [BlogNewsContentId] INT            IDENTITY (1, 1) NOT NULL,
    [BlogNewsId]        INT            NULL,
    [LocaleId]          INT            NULL,
    [BlogNewsContent]   NVARCHAR (MAX) NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeBlogNewsContent] PRIMARY KEY CLUSTERED ([BlogNewsContentId] ASC),
    CONSTRAINT [FK_ZnodeBlogNewsContent_ZnodeBlogNews] FOREIGN KEY ([BlogNewsId]) REFERENCES [dbo].[ZnodeBlogNews] ([BlogNewsId])
);

