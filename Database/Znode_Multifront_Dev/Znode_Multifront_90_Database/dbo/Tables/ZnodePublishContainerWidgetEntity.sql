CREATE TABLE [dbo].[ZnodePublishContainerWidgetEntity] (
    [PublishContainerWidgetEntity] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                 INT            NOT NULL,
    [PublishStartTime]          DATETIME       NULL,
    [ContainerConfigurationId] INT            NOT NULL,
    [MappingId]                 INT            NOT NULL,
    [PortalId]                  INT            NOT NULL,
    [TypeOFMapping]             VARCHAR (50)   NOT NULL,
    [LocaleId]                  INT            NOT NULL,
    [WidgetsKey]                VARCHAR (50)   NULL,
    [ContainerKey]              NVARCHAR (100) NULL,
    CONSTRAINT [PK_ZnodePublishContainerWidgetEntity] PRIMARY KEY CLUSTERED ([PublishContainerWidgetEntity] ASC) WITH (FILLFACTOR = 90)
);
GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishTextWidgetEntityVersionId]
    ON [dbo].[ZnodePublishContainerWidgetEntity]([VersionId] ASC);
	