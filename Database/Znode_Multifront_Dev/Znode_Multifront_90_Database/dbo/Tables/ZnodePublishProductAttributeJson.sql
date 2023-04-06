CREATE TABLE [dbo].[ZnodePublishProductAttributeJson] (
    [PublishProductAttributeJsonId] INT            IDENTITY (1, 1) NOT NULL,
    [PimProductId]                  INT            NULL,
    [Attributes]                    NVARCHAR (MAX) NULL,
    [LocaleId]                      INT            NULL,
    [CreatedBy]                     INT            NOT NULL,
    [CreatedDate]                   DATETIME       NOT NULL,
    [ModifiedBy]                    INT            NOT NULL,
    [ModifiedDate]                  DATETIME       NOT NULL,
    [AttributeCode]                 VARCHAR (600)  NULL,
    [IsUpdateLocaleWise]            BIT            NULL,
    CONSTRAINT [PK_ZnodePublishProductAttributeJson] PRIMARY KEY CLUSTERED ([PublishProductAttributeJsonId] ASC) WITH (FILLFACTOR = 90)
);








GO
CREATE NONCLUSTERED INDEX [ZnodePublishProductAttributeJson_PimProductId]
    ON [dbo].[ZnodePublishProductAttributeJson]([PimProductId] ASC)
    INCLUDE([ModifiedDate]);


GO
CREATE NONCLUSTERED INDEX [ZnodePublishProductAttributeJson_ProdId_Attributecode_LocaleId]
    ON [dbo].[ZnodePublishProductAttributeJson]([PimProductId] ASC, [AttributeCode] ASC, [LocaleId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [ZnodePublishProductAttributeJson_LocaleId]
    ON [dbo].[ZnodePublishProductAttributeJson]([LocaleId] ASC)
    INCLUDE([PimProductId], [AttributeCode]);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishProductAttributeJson_PimProductId_ModifiedDate]
    ON [dbo].[ZnodePublishProductAttributeJson]([PimProductId] ASC, [ModifiedDate] ASC) WITH (FILLFACTOR = 90);

