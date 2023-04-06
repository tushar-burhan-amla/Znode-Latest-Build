CREATE TABLE [dbo].[ZnodePromotionAttribute] (
    [PromotionAttributeId] INT           IDENTITY (1, 1) NOT NULL,
    [AttributeTypeId]      INT           NULL,
    [AttributeCode]        VARCHAR (300) NULL,
    [AttributeName]        VARCHAR (300) NULL,
    [IsRequired]           BIT           NOT NULL,
    [IsLocalizable]        BIT           NOT NULL,
    [HelpDescription]      VARCHAR (MAX) NULL,
    [CreatedBy]            INT           NOT NULL,
    [CreatedDate]          DATETIME      NOT NULL,
    [ModifiedBy]           INT           NOT NULL,
    [ModifiedDate]         DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePromotionAttribute] PRIMARY KEY CLUSTERED ([PromotionAttributeId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePromotionAttribute_ZnodeAttributeType] FOREIGN KEY ([AttributeTypeId]) REFERENCES [dbo].[ZnodeAttributeType] ([AttributeTypeId])
);



