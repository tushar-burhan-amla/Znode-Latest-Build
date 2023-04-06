CREATE TABLE [dbo].[ZnodePimAttributeGroup] (
    [PimAttributeGroupId] INT           IDENTITY (1, 1) NOT NULL,
    [GroupCode]           VARCHAR (200) NULL,
    [IsSystemDefined]     BIT           CONSTRAINT [DF__ZnodePIMAttr__IsSys__636EBA21] DEFAULT ((0)) NOT NULL,
    [IsCategory]          BIT           CONSTRAINT [DF_ZnodePimAttributeGroup_IsProductCategory] DEFAULT ((0)) NOT NULL,
    [DisplayOrder]        INT           NULL,
    [IsNonEditable]       BIT           CONSTRAINT [DF_ZnodePimAttributeGroup_IsNonEditable] DEFAULT ((0)) NOT NULL,
    [CreatedBy]           INT           NOT NULL,
    [CreatedDate]         DATETIME      NOT NULL,
    [ModifiedBy]          INT           NOT NULL,
    [ModifiedDate]        DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeGroup] PRIMARY KEY CLUSTERED ([PimAttributeGroupId] ASC)
);











