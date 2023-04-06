CREATE TABLE [dbo].[ZnodePublishMessageEntity] (
    [PublishMessageEntityId] INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]              INT           NOT NULL,
    [PublishStartTime]       DATETIME      NULL,
    [LocaleId]               INT           NOT NULL,
    [PortalId]               INT           NULL,
    [MessageKey]             VARCHAR (200) NULL,
    [Message]                VARCHAR (MAX) NULL,
    [Area]                   VARCHAR (100) NULL,
    CONSTRAINT [PK_ZnodePublishMessageEntity] PRIMARY KEY CLUSTERED ([PublishMessageEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishMessageEntityVersionId]
    ON [dbo].[ZnodePublishMessageEntity]([VersionId] ASC);

