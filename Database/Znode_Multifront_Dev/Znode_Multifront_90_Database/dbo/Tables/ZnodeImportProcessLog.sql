CREATE TABLE [dbo].[ZnodeImportProcessLog] (
    [ImportProcessLogId]    INT           IDENTITY (1, 1) NOT NULL,
    [ImportTemplateId]      INT           NULL,
    [Status]                NVARCHAR (50) NULL,
    [ProcessStartedDate]    DATETIME      NOT NULL,
    [ProcessCompletedDate]  DATETIME      NULL,
    [CreatedBy]             INT           NOT NULL,
    [CreatedDate]           DATETIME      NOT NULL,
    [ModifiedBy]            INT           NOT NULL,
    [ModifiedDate]          DATETIME      NOT NULL,
    [ERPTaskSchedulerId]    INT           NULL,
    [SuccessRecordCount]    BIGINT        NULL,
    [FailedRecordcount]     BIGINT        NULL,
    [TotalProcessedRecords] BIGINT        NULL,
    CONSTRAINT [PK_ZnodeImportProcessLog] PRIMARY KEY CLUSTERED ([ImportProcessLogId] ASC),
    CONSTRAINT [FK_ZnodeImportProcessLog_ZnodeImportTemplate] FOREIGN KEY ([ImportTemplateId]) REFERENCES [dbo].[ZnodeImportTemplate] ([ImportTemplateId])
);

GO

CREATE NONCLUSTERED INDEX NC_Idx_ZnodeImportProcessLog_ImportTemplateId ON ZnodeImportProcessLog (ImportTemplateId);







