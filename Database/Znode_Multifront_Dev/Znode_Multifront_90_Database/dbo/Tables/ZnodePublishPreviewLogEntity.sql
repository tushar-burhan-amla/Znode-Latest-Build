CREATE TABLE [dbo].[ZnodePublishPreviewLogEntity] (
    [PublishPreviewLogEntityId] INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]                 INT           NULL,
    [PublishStartTime]          DATETIME      NULL,
    [IsDisposed]                BIT           NULL,
    [SourcePublishState]        VARCHAR (30)  NULL,
    [EntityId]                  INT           NULL,
    [EntityType]                VARCHAR (30)  NULL,
    [LogMessage]                VARCHAR (MAX) NULL,
    [LogCreatedDate]            DATETIME      NULL,
    [PreviousVersionId]         INT           NULL,
    [LocaleId]                  INT           NULL,
    [LocaleDisplayValue]        VARCHAR (100) NULL,
    CONSTRAINT [PK_ZnodePublishPreviewLogEntity] PRIMARY KEY CLUSTERED ([PublishPreviewLogEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishPreviewLogEntityVersionId]
    ON [dbo].[ZnodePublishPreviewLogEntity]([VersionId] ASC);

