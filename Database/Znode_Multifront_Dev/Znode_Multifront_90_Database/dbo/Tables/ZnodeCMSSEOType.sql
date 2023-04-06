CREATE TABLE [dbo].[ZnodeCMSSEOType] (
    [CMSSEOTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedBy]   INT            NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSSEOType] PRIMARY KEY CLUSTERED ([CMSSEOTypeId] ASC)
);



