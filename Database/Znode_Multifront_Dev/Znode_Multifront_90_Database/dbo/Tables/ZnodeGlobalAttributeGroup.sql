CREATE TABLE [dbo].[ZnodeGlobalAttributeGroup] (
    [GlobalAttributeGroupId] INT           IDENTITY (1, 1) NOT NULL,
    [GroupCode]              VARCHAR (200) NULL,
    [DisplayOrder]           INT           NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    [IsSystemDefined]        BIT           DEFAULT ((0)) NOT NULL,
	[GlobalEntityId]		 INT		   NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeGroup] PRIMARY KEY CLUSTERED ([GlobalAttributeGroupId] ASC)
);



