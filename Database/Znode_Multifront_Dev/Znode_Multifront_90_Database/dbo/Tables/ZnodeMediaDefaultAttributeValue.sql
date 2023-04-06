CREATE TABLE [dbo].[ZnodeMediaDefaultAttributeValue] (
    [MediaDefaultAttributeValueId] INT      IDENTITY (1, 1) NOT NULL,
    [MediaAttributeId]             INT      NULL,
    [IsEditable]                   BIT      CONSTRAINT [DF_ZNodeMediaDefaultAttributeValue_Iseditable] DEFAULT ((0)) NULL,
    [CreatedBy]                    INT      NOT NULL,
    [CreatedDate]                  DATETIME NOT NULL,
    [ModifiedBy]                   INT      NOT NULL,
    [ModifiedDate]                 DATETIME NOT NULL,
    CONSTRAINT [ZNodeMediaDefaultAttributeValue_PK] PRIMARY KEY CLUSTERED ([MediaDefaultAttributeValueId] ASC),
    CONSTRAINT [ZnodeMediaAttribute_ZNodeMediaDefaultAttributeValue_FK1] FOREIGN KEY ([MediaAttributeId]) REFERENCES [dbo].[ZnodeMediaAttribute] ([MediaAttributeId])
);

