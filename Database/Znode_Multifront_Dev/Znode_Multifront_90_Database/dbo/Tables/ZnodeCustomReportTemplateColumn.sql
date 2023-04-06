CREATE TABLE [dbo].[ZnodeCustomReportTemplateColumn] (
    [CustomReportTemplateColumnId] INT           IDENTITY (1, 1) NOT NULL,
    [CustomReportTemplateId]       INT           NOT NULL,
    [ColumnName]                   VARCHAR (250) NULL,
    [CreatedBy]                    INT           NOT NULL,
    [CreatedDate]                  DATETIME      NOT NULL,
    [ModifiedBy]                   INT           NOT NULL,
    [ModifiedDate]                 DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCustomReportTempleteColumn] PRIMARY KEY CLUSTERED ([CustomReportTemplateColumnId] ASC),
    CONSTRAINT [FK_ZnodeCustomReportTemplateColumn_ZnodeCustomReportTemplate] FOREIGN KEY ([CustomReportTemplateId]) REFERENCES [dbo].[ZnodeCustomReportTemplate] ([CustomReportTemplateId])
);

