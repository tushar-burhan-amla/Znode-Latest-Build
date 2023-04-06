CREATE TABLE [dbo].[ZnodePimCustomeFieldXML] (
    [PimCustomeFieldXMLId] INT            IDENTITY (1, 1) NOT NULL,
    [PimProductId]         INT            NULL,
    [CustomCode]           VARCHAR (300)  NULL,
    [CustomeFiledXML]      NVARCHAR (MAX) NULL,
    [LocaleId]             INT            NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimCustomeFieldXML] PRIMARY KEY CLUSTERED ([PimCustomeFieldXMLId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170521-002403]
    ON [dbo].[ZnodePimCustomeFieldXML]([PimProductId] ASC);

