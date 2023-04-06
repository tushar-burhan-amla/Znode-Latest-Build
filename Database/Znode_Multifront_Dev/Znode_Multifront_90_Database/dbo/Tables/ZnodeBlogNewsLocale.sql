CREATE TABLE [dbo].[ZnodeBlogNewsLocale] (
    [BlogNewsLocaleId] INT             IDENTITY (1, 1) NOT NULL,
    [BlogNewsId]       INT             NULL,
    [LocaleId]         INT             NULL,
    [BlogNewsTitle]    NVARCHAR (600)  NULL,
    [BodyOverview]     NVARCHAR (MAX)  NULL,
    [Tags]             NVARCHAR (1000) NULL,
    [CreatedBy]        INT             NOT NULL,
    [CreatedDate]      DATETIME        NOT NULL,
    [ModifiedBy]       INT             NOT NULL,
    [ModifiedDate]     DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeBlogNewsLocale] PRIMARY KEY CLUSTERED ([BlogNewsLocaleId] ASC),
    CONSTRAINT [FK_ZnodeBlogNewsLocale_ZnodeBlogNews] FOREIGN KEY ([BlogNewsId]) REFERENCES [dbo].[ZnodeBlogNews] ([BlogNewsId])
);



