CREATE TABLE [dbo].[ZnodeImportAccountDefaultTemplate] (
    [ImportAccountDefaultTemplateId] INT            IDENTITY (1, 1) NOT NULL,
    [TemplateName]                   NVARCHAR (MAX) NULL,
    [ImportHeadId]                   INT            NOT NULL,
    [CreatedBy]                      INT            NOT NULL,
    [CreatedDate]                    DATETIME       NOT NULL,
    [ModifiedBy]                     INT            NOT NULL,
    [ModifiedDate]                   DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeImportAccountDefaultTemplate] PRIMARY KEY CLUSTERED ([ImportAccountDefaultTemplateId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeImportAccountDefaultTemplate_ZnodeImportHead] FOREIGN KEY ([ImportHeadId]) REFERENCES [dbo].[ZnodeImportHead] ([ImportHeadId])
);

