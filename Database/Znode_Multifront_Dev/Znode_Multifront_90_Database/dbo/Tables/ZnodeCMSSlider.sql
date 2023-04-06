CREATE TABLE [dbo].[ZnodeCMSSlider] (
    [CMSSliderId]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (100) NOT NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    [IsPublished]    BIT            NULL,
    [PublishStateId] TINYINT        CONSTRAINT [DF_ZnodeCMSSlider_PublishStateId] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ZnodeCMSSlider] PRIMARY KEY CLUSTERED ([CMSSliderId] ASC),
    CONSTRAINT [FK_ZnodeCMSSlider_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);







