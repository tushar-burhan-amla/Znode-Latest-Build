CREATE TABLE [dbo].[ZnodePimAttributeJson] (
    [PimAttributeJsonId] INT            IDENTITY (1, 1) NOT NULL,
    [PimAttributeId]     INT            NULL,
    [AttributeCode]      VARCHAR (300)  NULL,
    [AttributeJson]      NVARCHAR (MAX) NULL,
    [LocaleId]           INT            NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimAttributeJSON_PimAttributeId_LocaleId]
    ON [dbo].[ZnodePimAttributeJson]([PimAttributeId] ASC, [LocaleId] ASC);

