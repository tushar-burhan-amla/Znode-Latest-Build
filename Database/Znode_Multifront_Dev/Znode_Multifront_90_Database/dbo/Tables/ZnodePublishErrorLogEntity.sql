CREATE TABLE [dbo].[ZnodePublishErrorLogEntity] (
    [PublishErrorLogEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [EntityName]              VARCHAR (100)  NULL,
    [ErrorDescription]        NVARCHAR (MAX) NULL,
    [ProcessStatus]           VARCHAR (50)   NULL,
    [CreatedDate]             DATETIME       NULL,
    [CreatedBy]               INT            NULL,
    [VersionId]               VARCHAR (100)  NULL,
    CONSTRAINT [PK_ZnodePublishErrorLogEntity] PRIMARY KEY CLUSTERED ([PublishErrorLogEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishErrorLogEntityVersionId]
    ON [dbo].[ZnodePublishErrorLogEntity]([VersionId] ASC);

