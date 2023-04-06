CREATE TABLE [dbo].[ZnodePublishGlobalMessageEntity] (
    [PublishGlobalMessageEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                    INT            NOT NULL,
    [PublishStartTime]             DATETIME       NULL,
    [LocaleId]                     INT            NOT NULL,
    [MessageKey]                   VARCHAR (300)  NULL,
    [Message]                      NVARCHAR (MAX) NULL,
    [Area]                         VARCHAR (300)  NULL,
    CONSTRAINT [PK_ZnodePublishGlobalMessageEntity] PRIMARY KEY CLUSTERED ([PublishGlobalMessageEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishGlobalMessageEntityVersionId]
    ON [dbo].[ZnodePublishGlobalMessageEntity]([VersionId] ASC);

