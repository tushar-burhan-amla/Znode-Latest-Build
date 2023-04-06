CREATE TABLE [dbo].[ZnodeReportStyleSheets] (
    [StyleSheetId]  INT            IDENTITY (1, 1) NOT NULL,
    [StyleSheetXml] NVARCHAR (MAX) NULL,
    [IsDefault]     BIT            NULL,
    [CreatedBy]     INT            NOT NULL,
    [CreatedDate]   DATETIME       NOT NULL,
    [ModifiedBy]    INT            NOT NULL,
    [ModifiedDate]  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeReportStyleSheets] PRIMARY KEY CLUSTERED ([StyleSheetId] ASC)
);

