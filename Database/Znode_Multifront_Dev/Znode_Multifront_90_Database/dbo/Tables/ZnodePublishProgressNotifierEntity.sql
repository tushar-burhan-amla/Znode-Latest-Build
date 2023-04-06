CREATE TABLE [dbo].[ZnodePublishProgressNotifierEntity] (
    [PublishProgressNotifierEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                       INT            NOT NULL,
    [JobId]                           NVARCHAR (500) NOT NULL,
    [JobName]                         VARCHAR (MAX)  NOT NULL,
    [ProgressMark]                    INT            NOT NULL,
    [IsCompleted]                     BIT            NOT NULL,
    [IsFailed]                        BIT            NOT NULL,
    [ExceptionMessage]                VARCHAR (1000) NULL,
    [StartedBy]                       INT            NOT NULL,
    [StartedByFriendlyName]           VARCHAR (50)   NOT NULL,
    CONSTRAINT [PK_ZnodePublishProgressNotifierEntity] PRIMARY KEY CLUSTERED ([PublishProgressNotifierEntityId] ASC)
);

