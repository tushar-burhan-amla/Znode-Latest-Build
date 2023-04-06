CREATE TABLE [dbo].[ZnodeImportHead] (
    [ImportHeadId]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]                  NVARCHAR (50) NOT NULL,
    [IsUsedInImport]        BIT           NOT NULL,
    [IsUsedInDynamicReport] BIT           NOT NULL,
    [IsActive]              BIT           NOT NULL,
    [CreatedBy]             INT           NOT NULL,
    [CreatedDate]           DATETIME      NOT NULL,
    [ModifiedBy]            INT           NOT NULL,
    [ModifiedDate]          DATETIME      NOT NULL,
    [IsCsvUploader]         BIT           NULL,
    CONSTRAINT [PK_ZnodeImportHead] PRIMARY KEY CLUSTERED ([ImportHeadId] ASC) WITH (FILLFACTOR = 90)
);





