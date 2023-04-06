CREATE TABLE [dbo].[ZnodePublishWidgetProductEntity] (
    [PublishWidgetProductEntityId] INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]                    INT           NOT NULL,
    [PublishStartTime]             DATETIME      NULL,
    [WidgetProductId]              INT           NOT NULL,
    [ZnodeProductId]               INT           NOT NULL,
    [PortalId]                     INT           NOT NULL,
    [MappingId]                    INT           NOT NULL,
    [WidgetsKey]                   VARCHAR (50)  NULL,
    [TypeOFMapping]                VARCHAR (50)  NULL,
    [DisplayOrder]                 INT           NULL,
    [SKU]                          VARCHAR (300) NULL,
    CONSTRAINT [PK_ZnodePublishWidgetProductEntity] PRIMARY KEY CLUSTERED ([PublishWidgetProductEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishWidgetProductEntityVersionId]
    ON [dbo].[ZnodePublishWidgetProductEntity]([VersionId] ASC);

