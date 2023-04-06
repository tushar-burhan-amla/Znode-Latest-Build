CREATE TABLE [dbo].[ZnodeImportUpdatableColumns] (
    [UpdatableColumnsId] INT           IDENTITY (1, 1) NOT NULL,
    [ImportHeadId]       INT           NULL,
    [ColumnName]         VARCHAR (500) NULL,
    CONSTRAINT [PK_ZnodeImportUpdatableColumns] PRIMARY KEY CLUSTERED ([UpdatableColumnsId] ASC)
);


