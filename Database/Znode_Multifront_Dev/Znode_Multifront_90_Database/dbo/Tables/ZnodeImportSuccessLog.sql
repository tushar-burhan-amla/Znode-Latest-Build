CREATE TABLE [dbo].[ZnodeImportSuccessLog] (
    [ImportSuccessLogId] INT            IDENTITY (1, 1) NOT NULL,
    [ImportedSku]        NVARCHAR (300) NULL,
    [ImportedProductId]  INT            NULL,
    [ImportedGuId]       NVARCHAR (400) NOT NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    [IsProductPublish]   BIT            NULL,
    CONSTRAINT [PK_ZnodeImportSuccessLog] PRIMARY KEY CLUSTERED ([ImportSuccessLogId] ASC)
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodeImportSuccessLog]
    ON [dbo].[ZnodeImportSuccessLog]([IsProductPublish] ASC, [ImportedGuId] ASC);

