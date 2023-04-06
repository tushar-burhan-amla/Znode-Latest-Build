CREATE TABLE [dbo].[ZnodeGlobalAttribute] (
    [GlobalAttributeId] INT            IDENTITY (1, 1) NOT NULL,
    [AttributeTypeId]   INT            NULL,
    [AttributeCode]     NVARCHAR (300) NULL,
    [IsRequired]        BIT            NOT NULL,
    [IsLocalizable]     BIT            NOT NULL,
    [IsActive]          BIT            CONSTRAINT [DF_ZnodeGlobalAttribute_IsActive] DEFAULT ((1)) NOT NULL,
    [DisplayOrder]      INT            NULL,
    [HelpDescription]   NVARCHAR (MAX) NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    [IsSystemDefined]   BIT            DEFAULT ((0)) NOT NULL,
	[GlobalEntityId]	INT			   NULL,
    CONSTRAINT [PK_ZnodeGlobalAttribute] PRIMARY KEY CLUSTERED ([GlobalAttributeId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeGlobalAttribute_ZnodeAttributeType] FOREIGN KEY ([AttributeTypeId]) REFERENCES [dbo].[ZnodeAttributeType] ([AttributeTypeId])
);





