CREATE TABLE [dbo].[ZnodeAttributeType] (
    [AttributeTypeId]      INT           IDENTITY (1, 1) NOT NULL,
    [AttributeTypeName]    VARCHAR (300) NULL,
    [IsMediaAttributeType] BIT           CONSTRAINT [DF_ZnodeAttributeType_IsMediaAttributeType] DEFAULT ((0)) NULL,
    [IsPimAttributeType]   BIT           CONSTRAINT [DF_ZnodeAttributeType_IsPimAttributeType] DEFAULT ((0)) NULL,
    [CreatedBy]            INT           NOT NULL,
    [CreatedDate]          DATETIME      NOT NULL,
    [ModifiedBy]           INT           NOT NULL,
    [ModifiedDate]         DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeAttributeType] PRIMARY KEY CLUSTERED ([AttributeTypeId] ASC)
);







