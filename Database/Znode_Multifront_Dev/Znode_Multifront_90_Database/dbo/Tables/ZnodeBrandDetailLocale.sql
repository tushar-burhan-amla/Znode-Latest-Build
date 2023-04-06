CREATE TABLE [dbo].[ZnodeBrandDetailLocale] (
    [BrandDetailLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [BrandId]             INT            NULL,
    [Description]         NVARCHAR (MAX) NULL,
    [SEOFriendlyPageName] NVARCHAR (600) NULL,
    [LocaleId]            INT            NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    [BrandName]           NVARCHAR (100) NULL,
	[Custom1] nvarchar(max),
    [Custom2] nvarchar(max),
    [Custom3] nvarchar(max),
    [Custom4] nvarchar(max),
    [Custom5] nvarchar(max)
    CONSTRAINT [PK_ZnodeBrandDetailLocale] PRIMARY KEY CLUSTERED ([BrandDetailLocaleId] ASC),
    CONSTRAINT [FK_ZnodeBrandDetailLocale_ZnodeBrandDetails] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[ZnodeBrandDetails] ([BrandId])
);



