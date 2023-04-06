CREATE TABLE [dbo].[ZnodePimVersioning] (
    [ZnodePimVersioningId] INT      IDENTITY (1, 1) NOT NULL,
    [Version]              INT      NULL,
    [PimCatalogId]         INT      NULL,
    [IsPublish]            BIT      NULL,
    [IsDraft]              BIT      NULL,
    [PublishDate]          DATETIME NULL,
    [DraftDate]            DATETIME NULL,
    [CreatedBy]            INT      NOT NULL,
    [CreatedDate]          DATETIME NOT NULL,
    [ModifiedBy]           INT      NOT NULL,
    [ModifiedDate]         DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimVersioning] PRIMARY KEY CLUSTERED ([ZnodePimVersioningId] ASC),
    CONSTRAINT [FK_ZnodePimVersioning_ZnodePimCatalog] FOREIGN KEY ([PimCatalogId]) REFERENCES [dbo].[ZnodePimCatalog] ([PimCatalogId])
);



