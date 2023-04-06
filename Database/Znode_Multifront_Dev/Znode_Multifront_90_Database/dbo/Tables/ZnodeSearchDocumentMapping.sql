CREATE TABLE [dbo].[ZnodeSearchDocumentMapping] (
    [SearchDocumentMappingId] INT             IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId]        INT             NOT NULL,
    [PropertyName]            NVARCHAR (200)  NOT NULL,
    [FieldBoostable]          BIT             CONSTRAINT [DF_ZNodeSearchDocumentMapping_FieldBoostable] DEFAULT ((0)) NOT NULL,
    [Boost]                   NUMERIC (28, 6) NOT NULL,
    [CreatedBy]               INT             NOT NULL,
    [CreatedDate]             DATETIME        NOT NULL,
    [ModifiedBy]              INT             NOT NULL,
    [ModifiedDate]            DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeSearchDocumentMapping] PRIMARY KEY CLUSTERED ([SearchDocumentMappingId] ASC) WITH (FILLFACTOR = 90)
);









