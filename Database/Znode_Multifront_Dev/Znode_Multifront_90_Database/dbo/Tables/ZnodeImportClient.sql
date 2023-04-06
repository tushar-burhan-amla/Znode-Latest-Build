CREATE TABLE [dbo].[ZnodeImportClient] (
    [ImportClientId]       INT            IDENTITY (1, 1) NOT NULL,
    [ImportClientName]     NVARCHAR (200) NULL,
    [ImportClientIsActive] BIT            NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeImportClient] PRIMARY KEY CLUSTERED ([ImportClientId] ASC)
);

