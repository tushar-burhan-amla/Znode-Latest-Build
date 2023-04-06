CREATE TABLE [dbo].[ZnodePublishProductAttributeXML] (
    [PublishProductAttributeXMLId] INT           IDENTITY (1, 1) NOT NULL,
    [PimProductId]                 INT           NULL,
    [Attributes]                   XML           NULL,
    [LocaleId]                     INT           NULL,
    [CreatedBy]                    INT           NOT NULL,
    [CreatedDate]                  DATETIME      NOT NULL,
    [ModifiedBy]                   INT           NOT NULL,
    [ModifiedDate]                 DATETIME      NOT NULL,
    [AttributeCode]                VARCHAR (600) NULL,
	[IsUpdateLocaleWise]        BIT             NULL,
    CONSTRAINT [PK_ZnodePublishProductAttributeXML] PRIMARY KEY CLUSTERED ([PublishProductAttributeXMLId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishProductAttributeXML_PimProductId_LocaleId]
    ON [dbo].[ZnodePublishProductAttributeXML]([PimProductId] ASC, [LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishProductAttributeXML_PimProductId_ModifiedDate]
    ON [dbo].[ZnodePublishProductAttributeXML]([PimProductId] ASC, [ModifiedDate] ASC);

