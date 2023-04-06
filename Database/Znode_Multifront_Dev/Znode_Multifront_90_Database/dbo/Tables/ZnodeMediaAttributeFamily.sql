CREATE TABLE [dbo].[ZnodeMediaAttributeFamily] (
    [MediaAttributeFamilyId] INT           IDENTITY (1, 1) NOT NULL,
    [FamilyCode]             VARCHAR (200) NULL,
    [IsSystemDefined]        BIT           CONSTRAINT [DF_ZnodeMediaAttributeFamily_IsSystemDefined] DEFAULT ((0)) NOT NULL,
    [IsDefaultFamily]        BIT           CONSTRAINT [DF_ZnodeMediaAttributeFamily_IsDefaulyFamily] DEFAULT ((0)) NOT NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttributeFamily] PRIMARY KEY CLUSTERED ([MediaAttributeFamilyId] ASC)
);





