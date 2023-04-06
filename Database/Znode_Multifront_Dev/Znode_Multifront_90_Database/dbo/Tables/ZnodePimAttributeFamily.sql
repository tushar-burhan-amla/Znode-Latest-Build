CREATE TABLE [dbo].[ZnodePimAttributeFamily] (
    [PimAttributeFamilyId] INT           IDENTITY (1, 1) NOT NULL,
    [FamilyCode]           VARCHAR (200) NULL,
    [IsSystemDefined]      BIT           CONSTRAINT [DF_ZnodePimAttributeFamily_IsSystemDefined] DEFAULT ((0)) NOT NULL,
    [IsDefaultFamily]      BIT           CONSTRAINT [DF_ZnodePimAttributeFamily_IsDefaulyFamily] DEFAULT ((0)) NOT NULL,
    [IsCategory]           BIT           CONSTRAINT [DF_ZnodePimAttributeFamily_IsProductCategory] DEFAULT ((0)) NOT NULL,
    [CreatedBy]            INT           NOT NULL,
    [CreatedDate]          DATETIME      NOT NULL,
    [ModifiedBy]           INT           NOT NULL,
    [ModifiedDate]         DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeFamily] PRIMARY KEY CLUSTERED ([PimAttributeFamilyId] ASC)
);







