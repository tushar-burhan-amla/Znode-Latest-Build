CREATE TABLE [dbo].[ZnodeImportXMLData] (
    [ID]       INT             IDENTITY (1, 1) NOT NULL,
    [XMLVALUE] XML             NULL,
    [CsvHeder] NVARCHAR (2000) NULL,
    [NewGUID]  NVARCHAR (MAX)  NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

