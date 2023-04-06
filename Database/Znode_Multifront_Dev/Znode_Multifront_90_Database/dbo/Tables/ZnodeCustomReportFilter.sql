CREATE TABLE [dbo].[ZnodeCustomReportFilter] (
    [CustomReportFilterId]   INT           IDENTITY (1, 1) NOT NULL,
    [CustomReportTemplateId] INT           NOT NULL,
    [FilterName]             VARCHAR (250) NOT NULL,
    [Action]                 VARCHAR (250) NOT NULL,
    [FilterValue]            VARCHAR (250) NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCustomReportFilter] PRIMARY KEY CLUSTERED ([CustomReportFilterId] ASC),
    CONSTRAINT [FK_ZnodeCustomReportFilter_ZnodeCustomReportTemplate] FOREIGN KEY ([CustomReportTemplateId]) REFERENCES [dbo].[ZnodeCustomReportTemplate] ([CustomReportTemplateId])
);

