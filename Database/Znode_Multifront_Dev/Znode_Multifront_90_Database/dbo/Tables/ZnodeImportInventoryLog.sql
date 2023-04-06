CREATE TABLE [dbo].[ZnodeImportInventoryLog] (
    [ImportInventoryLogId] INT            IDENTITY (1, 1) NOT NULL,
    [SKU]                  VARCHAR (300)  NULL,
    [ReOrderLevel]         VARCHAR (300)  NULL,
    [Quantity]             VARCHAR (300)  NULL,
    [WarehouseCode]        VARCHAR (300)  NULL,
    [RowNumber]            BIGINT         NULL,
    [ErrorDescription]     VARCHAR (MAX)  NULL,
    [GUID]                 NVARCHAR (200) NULL,
    [ImportProcessLogId]   INT            NULL,
    [SourceColumnName]     NVARCHAR (200) NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeImportInventoryLog] PRIMARY KEY CLUSTERED ([ImportInventoryLogId] ASC) WITH (FILLFACTOR = 90)
);

