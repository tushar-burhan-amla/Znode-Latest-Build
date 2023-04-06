CREATE TABLE [dbo].[ZnodeCMSSEODetailLocale] (
    [CMSSEODetailLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [CMSSEODetailId]       INT            NOT NULL,
    [LocaleId]             INT            NULL,
    [SEOTitle]             NVARCHAR (MAX) NULL,
    [SEODescription]       NVARCHAR (MAX) NULL,
    [SEOKeywords]          NVARCHAR (MAX) NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    [CanonicalURL]         VARCHAR (200)  NULL,
    [RobotTag]             VARCHAR (50)   NULL,
    CONSTRAINT [PK_ZnodeCMSSEODetailLocale] PRIMARY KEY CLUSTERED ([CMSSEODetailLocaleId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeCMSSEODetailLocale_ZnodeCMSSEODetail] FOREIGN KEY ([CMSSEODetailId]) REFERENCES [dbo].[ZnodeCMSSEODetail] ([CMSSEODetailId])
);
















GO
CREATE NONCLUSTERED INDEX [IX_ZnodeCMSSEODetailLocale_CMSSEODetailId_LocaleId]
    ON [dbo].[ZnodeCMSSEODetailLocale]([CMSSEODetailId] ASC, [LocaleId] ASC)
    INCLUDE([SEOTitle], [SEODescription], [SEOKeywords], [CanonicalURL], [RobotTag]);

