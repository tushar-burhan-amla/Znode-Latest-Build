CREATE TABLE [dbo].[ZnodePublishMediaWidgetEntity] (
    [PublishMediaWidgetEntityId] INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]                  INT           NOT NULL,
    [PublishStartTime]           DATETIME      NULL,
    [MediaWidgetConfigurationId] INT           NOT NULL,
    [MappingId]                  INT           NOT NULL,
    [PortalId]                   INT           NOT NULL,
    [TypeOFMapping]              VARCHAR (300) NOT NULL,
    [MediaPath]                  VARCHAR (300) NULL,
    [WidgetsKey]                 VARCHAR (100) NULL,
    CONSTRAINT [PK_ZnodePublishMediaWidgetEntity] PRIMARY KEY CLUSTERED ([PublishMediaWidgetEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishMediaWidgetEntityVersionId]
    ON [dbo].[ZnodePublishMediaWidgetEntity]([VersionId] ASC);

