CREATE TABLE [dbo].[ZnodeCustomReportTemplate] (
    [CustomReportTemplateId] INT           IDENTITY (1, 1) NOT NULL,
    [ImportHeadId]           INT           NOT NULL,
    [ReportName]             VARCHAR (250) NOT NULL,
    [LocaleId]               INT           NOT NULL,
    [CatalogId]              INT           NULL,
    [PriceId]                INT           NULL,
    [WarehouseId]            INT           NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCustomReportTemplete] PRIMARY KEY CLUSTERED ([CustomReportTemplateId] ASC),
    CONSTRAINT [FK_ZnodeCustomReportTemplate_ZnodeImportHead] FOREIGN KEY ([ImportHeadId]) REFERENCES [dbo].[ZnodeImportHead] ([ImportHeadId])
);

