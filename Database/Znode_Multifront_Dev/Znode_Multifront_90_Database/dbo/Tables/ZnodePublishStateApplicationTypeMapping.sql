CREATE TABLE [dbo].[ZnodePublishStateApplicationTypeMapping] (
    [PublishStateMappingId] INT            IDENTITY (1, 1) NOT NULL,
    [PublishStateId]        TINYINT        NOT NULL,
    [ApplicationType]       VARCHAR (50)   NOT NULL,
    [Description]           NVARCHAR (MAX) NULL,
    [IsEnabled]             BIT            CONSTRAINT [DF_ZnodePublishStateApplicationTypeMapping_IsEnabled] DEFAULT ((0)) NOT NULL,
    [IsActive]              BIT            CONSTRAINT [DF_ZnodePublishStateApplicationTypeMapping_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    [DisplayOrder]          INT            CONSTRAINT [DF_ZnodePublishStateApplicationTypeMapping_DisplayOrder] DEFAULT ((99)) NOT NULL,
    PRIMARY KEY CLUSTERED ([PublishStateMappingId] ASC),
    CONSTRAINT [FK_ZnodePublishStateApplicationTypeMapping_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);



