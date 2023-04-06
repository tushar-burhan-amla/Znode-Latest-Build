CREATE TABLE [dbo].[ZnodeImportTableDetail] (
    [ImportTableId]       INT            IDENTITY (1, 1) NOT NULL,
    [ImportClientId]      INT            NULL,
    [ImportHeadId]        INT            NULL,
    [ImportTableName]     NVARCHAR (200) NULL,
    [ImportTableIsActive] BIT            NULL,
    [ImportTableNature]   NVARCHAR (50)  NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    CONSTRAINT [ZnodeImportTable] PRIMARY KEY CLUSTERED ([ImportTableId] ASC),
    CONSTRAINT [FK_ZnodeImportTable_ZnodeImportClient] FOREIGN KEY ([ImportClientId]) REFERENCES [dbo].[ZnodeImportClient] ([ImportClientId]),
    CONSTRAINT [FK_ZnodeImportTable_ZnodeImportHead] FOREIGN KEY ([ImportHeadId]) REFERENCES [dbo].[ZnodeImportHead] ([ImportHeadId])
);



