CREATE TABLE [dbo].[ZnodePublishGlobalVersionEntity] (
    [PublishGlobalVersionEntityId] INT          IDENTITY (1, 1) NOT NULL,
    [VersionId]                    INT          NOT NULL,
    [PublishStartTime]             DATETIME     NULL,
    [PublishState]                 VARCHAR (30) NOT NULL,
    [LocaleId]                     INT          NOT NULL,
    CONSTRAINT [PK_ZnodePublishGlobalVersionEntity] PRIMARY KEY CLUSTERED ([PublishGlobalVersionEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishGlobalVersionEntityVersionId]
    ON [dbo].[ZnodePublishGlobalVersionEntity]([VersionId] ASC);

