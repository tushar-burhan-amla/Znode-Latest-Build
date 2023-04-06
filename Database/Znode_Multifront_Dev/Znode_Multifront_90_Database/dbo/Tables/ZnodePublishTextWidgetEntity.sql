CREATE TABLE [dbo].[ZnodePublishTextWidgetEntity] (
    [PublishTextWidgetEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                 INT            NOT NULL,
    [PublishStartTime]          DATETIME       NULL,
    [TextWidgetConfigurationId] INT            NOT NULL,
    [MappingId]                 INT            NOT NULL,
    [PortalId]                  INT            NOT NULL,
    [TypeOFMapping]             VARCHAR (50)   NOT NULL,
    [LocaleId]                  INT            NOT NULL,
    [WidgetsKey]                VARCHAR (50)   NULL,
    [Text]                      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ZnodePublishTextWidgetEntity] PRIMARY KEY CLUSTERED ([PublishTextWidgetEntityId] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishTextWidgetEntityVersionId]
    ON [dbo].[ZnodePublishTextWidgetEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishTextWidgetEntity_PortalId_40EF9]
    ON [dbo].[ZnodePublishTextWidgetEntity]([PortalId] ASC)
    INCLUDE([VersionId]);

