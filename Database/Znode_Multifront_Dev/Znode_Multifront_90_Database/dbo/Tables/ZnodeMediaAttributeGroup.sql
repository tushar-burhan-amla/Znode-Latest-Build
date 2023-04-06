CREATE TABLE [dbo].[ZnodeMediaAttributeGroup] (
    [MediaAttributeGroupId] INT           IDENTITY (1, 1) NOT NULL,
    [GroupCode]             VARCHAR (200) NULL,
    [IsSystemDefined]       BIT           CONSTRAINT [DF__ZnodeAttr__IsSys__636EBA21] DEFAULT ((0)) NOT NULL,
    [DisplayOrder]          INT           NULL,
    [IsHidden]              BIT           CONSTRAINT [DF_ZnodeMediaAttributeGroup_IsHiden] DEFAULT ((0)) NOT NULL,
    [CreatedBy]             INT           NOT NULL,
    [CreatedDate]           DATETIME      NOT NULL,
    [ModifiedBy]            INT           NOT NULL,
    [ModifiedDate]          DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttributeGroup] PRIMARY KEY CLUSTERED ([MediaAttributeGroupId] ASC)
);









