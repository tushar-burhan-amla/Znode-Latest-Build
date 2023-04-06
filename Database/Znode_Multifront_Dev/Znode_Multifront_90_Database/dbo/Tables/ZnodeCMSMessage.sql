CREATE TABLE [dbo].[ZnodeCMSMessage] (
    [CMSMessageId]   INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]       INT            NOT NULL,
    [Message]        NVARCHAR (MAX) NULL,
    [IsPublished]    BIT            NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    [PublishStateId] TINYINT        CONSTRAINT [DF_ZnodeCMSMessage_PublishStateId] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ZnodeCMSMessage] PRIMARY KEY CLUSTERED ([CMSMessageId] ASC),
    CONSTRAINT [FK_ZnodeCMSMessage_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);





