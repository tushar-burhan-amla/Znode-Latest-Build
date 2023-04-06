CREATE TABLE [dbo].[ZnodePublishCatalogAttributeEntity] (
    [PublishCatalogAttributeEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                       INT            NOT NULL,
    [ZnodeCatalogId]                  INT            NOT NULL,
    [AttributeCode]                   VARCHAR (300)  NOT NULL,
    [AttributeTypeName]               VARCHAR (300)  NOT NULL,
    [IsPromoRuleCondition]            BIT            NOT NULL,
    [IsComparable]                    BIT            NOT NULL,
    [IsHtmlTags]                      BIT            NOT NULL,
    [IsFacets]                        BIT            NOT NULL,
    [IsUseInSearch]                   BIT            NOT NULL,
    [IsPersonalizable]                BIT            NOT NULL,
    [IsConfigurable]                  BIT            NOT NULL,
    [AttributeName]                   VARCHAR (300)  NOT NULL,
    [LocaleId]                        INT            NOT NULL,
    [DisplayOrder]                    INT            NOT NULL,
    [SelectValues]                    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_ZnodePublishCatalogAttributeEntity] PRIMARY KEY CLUSTERED ([PublishCatalogAttributeEntityId] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishCatalogAttributeEntity_CatalogId]
    ON [dbo].[ZnodePublishCatalogAttributeEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);

