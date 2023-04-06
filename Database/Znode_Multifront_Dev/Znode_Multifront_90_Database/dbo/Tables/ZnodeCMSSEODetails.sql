CREATE TABLE [dbo].[ZnodeCMSSEODetails] (
    [CMSSEODetailsId] INT            NOT NULL,
    [CMSSEOTypeId]    INT            IDENTITY (1, 1) NOT NULL,
    [SEOId]           INT            NOT NULL,
    [SEOTitle]        NVARCHAR (100) NULL,
    [SEODescription]  NVARCHAR (MAX) NULL,
    [SEOKeywords]     NVARCHAR (MAX) NULL,
    [SEOUrl]          NVARCHAR (MAX) NULL,
    [IsRedirect]      BIT            NULL,
    [MetaInformation] NVARCHAR (MAX) NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSSEODetails] PRIMARY KEY CLUSTERED ([CMSSEOTypeId] ASC),
    CONSTRAINT [FK_ZnodeCMSSEODetails_ZnodeCMSSEOType] FOREIGN KEY ([CMSSEOTypeId]) REFERENCES [dbo].[ZnodeCMSSEOType] ([CMSSEOTypeId])
);

