CREATE TABLE [dbo].[ZnodeMediaAttributeDefaultValue] (
    [MediaAttributeDefaultValueId] INT           IDENTITY (1, 1) NOT NULL,
    [MediaAttributeId]             INT           NULL,
    [AttributeDefaultValueCode]    VARCHAR (100) NULL,
    [IsEditable]                   BIT           CONSTRAINT [DF_ZNodeMediaDefaultAttributeValue_Iseditable] DEFAULT ((0)) NULL,
    [CreatedBy]                    INT           NOT NULL,
    [CreatedDate]                  DATETIME      NOT NULL,
    [ModifiedBy]                   INT           NOT NULL,
    [ModifiedDate]                 DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttributeDefaultValue] PRIMARY KEY CLUSTERED ([MediaAttributeDefaultValueId] ASC),
    CONSTRAINT [FK_ZnodeMediaAttributeDefaultValue_ZnodeMediaAttribute] FOREIGN KEY ([MediaAttributeId]) REFERENCES [dbo].[ZnodeMediaAttribute] ([MediaAttributeId])
);





