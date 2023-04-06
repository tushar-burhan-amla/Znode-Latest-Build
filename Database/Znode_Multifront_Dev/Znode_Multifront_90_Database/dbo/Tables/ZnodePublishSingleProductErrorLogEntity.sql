CREATE TABLE [dbo].[ZnodePublishSingleProductErrorLogEntity] (
    [PublishSingleProductErrorLogEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [EntityName]                           VARCHAR (100)  NULL,
    [ErrorDescription]                     NVARCHAR (MAX) NULL,
    [ProcessStatus]                        VARCHAR (50)   NULL,
    [CreatedDate]                          DATETIME       NULL,
    [CreatedBy]                            INT            NULL,
    [VersionId]                            VARCHAR (100)  NULL,
    CONSTRAINT [PK_PublishSingleProductErrorLogEntity] PRIMARY KEY CLUSTERED ([PublishSingleProductErrorLogEntityId] ASC)
);

