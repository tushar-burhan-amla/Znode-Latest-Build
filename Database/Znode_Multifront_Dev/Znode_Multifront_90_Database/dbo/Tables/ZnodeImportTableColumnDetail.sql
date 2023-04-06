CREATE TABLE [dbo].[ZnodeImportTableColumnDetail] (
    [ImportTableColumnId]       INT            IDENTITY (1, 1) NOT NULL,
    [ImportTableId]             INT            NULL,
    [ImportTableColumnName]     NVARCHAR (200) NULL,
    [ImportTableColumnIsActive] BIT            NULL,
    [CreatedBy]                 INT            NOT NULL,
    [CreatedDate]               DATETIME       NOT NULL,
    [ModifiedBy]                INT            NOT NULL,
    [ModifiedDate]              DATETIME       NOT NULL,
    [BaseImportColumn]          NVARCHAR (255) NULL,
    [ColumnSequence]            INT            NULL,
    CONSTRAINT [ZnodeImportTableColumnDetails] PRIMARY KEY CLUSTERED ([ImportTableColumnId] ASC),
    CONSTRAINT [FK_ZnodeImportTableColumnDetail_ZnodeImportTable] FOREIGN KEY ([ImportTableId]) REFERENCES [dbo].[ZnodeImportTableDetail] ([ImportTableId])
);



