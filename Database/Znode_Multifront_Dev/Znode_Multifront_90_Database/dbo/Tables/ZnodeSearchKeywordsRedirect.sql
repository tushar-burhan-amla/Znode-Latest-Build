CREATE TABLE [dbo].[ZnodeSearchKeywordsRedirect] (
    [SearchKeywordsRedirectId] INT             IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId]         INT             NULL,
    [Keywords]                 NVARCHAR (1000) NULL,
    [URL]                      NVARCHAR (2000) NULL,
    [LocaleId]                 INT             NULL,
    [CreatedBy]                INT             NOT NULL,
    [CreatedDate]              DATETIME        NOT NULL,
    [ModifiedBy]               INT             NOT NULL,
    [ModifiedDate]             DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeSearchKeywordsRedirect] PRIMARY KEY CLUSTERED ([SearchKeywordsRedirectId] ASC) WITH (FILLFACTOR = 90)
);





