CREATE TABLE [dbo].[ZnodePimCustomeFieldJson] (
    [PimCustomeFieldJsonId] INT            IDENTITY (1, 1) NOT NULL,
    [PimProductId]          INT            NULL,
    [CustomCode]            VARCHAR (300)  NULL,
    [CustomeFiledJson]      NVARCHAR (MAX) NULL,
    [LocaleId]              INT            NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimCustomeFieldJSON] PRIMARY KEY CLUSTERED ([PimCustomeFieldJsonId] ASC)
);

