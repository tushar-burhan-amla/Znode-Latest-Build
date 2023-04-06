CREATE TABLE [dbo].[ZnodePublishPortalErrorLogEntity] (
    [PublishPortalErrorLogEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [EntityName]                    VARCHAR (100)  NULL,
    [ErrorDescription]              NVARCHAR (MAX) NULL,
    [ProcessStatus]                 VARCHAR (50)   NULL,
    [CreatedDate]                   DATETIME       NULL,
    [CreatedBy]                     INT            NULL,
    [VersionId]                     VARCHAR (100)  NULL,
    CONSTRAINT [PK_ZnodePublishPortalErrorLogEntity] PRIMARY KEY CLUSTERED ([PublishPortalErrorLogEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishPortalErrorLogEntityVersionId]
    ON [dbo].[ZnodePublishPortalErrorLogEntity]([VersionId] ASC);

