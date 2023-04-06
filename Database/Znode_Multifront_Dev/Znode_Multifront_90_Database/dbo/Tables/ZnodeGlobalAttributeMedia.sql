CREATE TABLE [dbo].[ZnodeGlobalAttributeMedia] (
    [GlobalAttributeMediaId] INT           IDENTITY (1, 1) NOT NULL,
    [GlobalAttributeValueId] INT           NOT NULL,
    [MediaPath]              VARCHAR (300) NULL,
    [MediaId]                INT           NULL,
    [LocaleId]               INT           NOT NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeMedia] PRIMARY KEY CLUSTERED ([GlobalAttributeMediaId] ASC),
    CONSTRAINT [FK_ZnodeGlobalAttributeMedia_ZnodeGlobalAttributeValueId] FOREIGN KEY ([GlobalAttributeValueId]) REFERENCES [dbo].[ZnodeGlobalAttributeValue] ([GlobalAttributeValueId])
);

