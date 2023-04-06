CREATE TABLE [dbo].[ZnodeImportLog] (
    [ImportLogId]        INT            IDENTITY (1, 1) NOT NULL,
    [ImportProcessLogId] INT            NOT NULL,
    [ErrorDescription]   NVARCHAR (MAX) NULL,
    [ColumnName]         NVARCHAR (500) NULL,
    [Data]               NVARCHAR (MAX) NULL,
    [DefaultErrorValue]  NVARCHAR (300) NULL,
    [RowNumber]          BIGINT         NULL,
    [Guid]               NVARCHAR (200) NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeImportErrorLog] PRIMARY KEY CLUSTERED ([ImportLogId] ASC),
    CONSTRAINT [FK_ZnodeImportLog_ZnodeImportProcessLog] FOREIGN KEY ([ImportProcessLogId]) REFERENCES [dbo].[ZnodeImportProcessLog] ([ImportProcessLogId])
);






GO
CREATE NONCLUSTERED INDEX [Ind_ZnodeImportLog]
    ON [dbo].[ZnodeImportLog]([ImportProcessLogId] ASC, [Guid] ASC);

