CREATE TABLE [dbo].[ZnodePimFrontendProperties] (
    [ZnodePimFrontendPropertiesId] INT      IDENTITY (1, 1) NOT NULL,
    [PimAttributeId]               INT      NOT NULL,
    [IsComparable]                 BIT      NOT NULL,
    [IsUseInSearch]                BIT      NOT NULL,
    [IsHtmlTags]                   BIT      NOT NULL,
    [IsFacets]                     BIT      CONSTRAINT [DF_ZnodePimFrontendProperties_IsFacets] DEFAULT ((0)) NOT NULL,
    [CreatedBy]                    INT      NOT NULL,
    [CreatedDate]                  DATETIME NOT NULL,
    [ModifiedBy]                   INT      NOT NULL,
    [ModifiedDate]                 DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeMediaFrontProperties] PRIMARY KEY CLUSTERED ([ZnodePimFrontendPropertiesId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimFrontProperties_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId])
);










GO
