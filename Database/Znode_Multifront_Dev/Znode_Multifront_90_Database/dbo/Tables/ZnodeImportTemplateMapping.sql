CREATE TABLE [dbo].[ZnodeImportTemplateMapping] (
    [ImportTemplateMappingId] INT            IDENTITY (1, 1) NOT NULL,
    [ImportTemplateId]        INT            NOT NULL,
    [SourceColumnName]        NVARCHAR (300) NOT NULL,
    [TargetColumnName]        NVARCHAR (300) NOT NULL,
    [DisplayOrder]            INT            NOT NULL,
    [IsActive]                BIT            NOT NULL,
    [IsAllowNull]             BIT            NOT NULL,
    [CreatedBy]               INT            NOT NULL,
    [CreatedDate]             DATETIME       NOT NULL,
    [ModifiedBy]              INT            NOT NULL,
    [ModifiedDate]            DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeImportTemplateMapping] PRIMARY KEY CLUSTERED ([ImportTemplateMappingId] ASC),
    CONSTRAINT [FK_ZnodeImportTemplateMapping_ZnodeImportTemplate] FOREIGN KEY ([ImportTemplateId]) REFERENCES [dbo].[ZnodeImportTemplate] ([ImportTemplateId])
);

