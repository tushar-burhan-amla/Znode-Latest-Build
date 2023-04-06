CREATE TABLE [dbo].[ZnodePimAttribute] (
    [PimAttributeId]       INT            IDENTITY (1, 1) NOT NULL,
    [ParentPimAttributeId] INT            NULL,
    [AttributeTypeId]      INT            NULL,
    [AttributeCode]        NVARCHAR (300) NULL,
    [IsRequired]           BIT            NOT NULL,
    [IsLocalizable]        BIT            NOT NULL,
    [IsFilterable]         BIT            NOT NULL,
    [IsSystemDefined]      BIT            CONSTRAINT [DF_ZnodePimAttribute_IsSystemDefined] DEFAULT ((0)) NOT NULL,
    [IsConfigurable]       BIT            NOT NULL,
    [IsPersonalizable]     BIT            NOT NULL,
    [IsShowOnGrid]         BIT            NULL,
    [DisplayOrder]         INT            NULL,
    [HelpDescription]      NVARCHAR (MAX) NULL,
    [IsCategory]           BIT            CONSTRAINT [DF_ZnodePimAttribute_IsProductCategory] DEFAULT ((0)) NOT NULL,
    [IsHidden]             BIT            NULL,
    [IsSwatch]             BIT            CONSTRAINT [DF_ZnodePimAttribute_IsSwatch] DEFAULT ((0)) NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimAttribute] PRIMARY KEY CLUSTERED ([PimAttributeId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimAttribute_ZnodeAttributeType] FOREIGN KEY ([AttributeTypeId]) REFERENCES [dbo].[ZnodeAttributeType] ([AttributeTypeId]),
    CONSTRAINT [FK_ZnodePimAttribute_ZnodePimAttribute] FOREIGN KEY ([ParentPimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId])
);
































GO
