CREATE TABLE [dbo].[ZnodePublishWidgetBrandEntity] (
    [PublishWidgetBrandEntityId] INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]                  INT           NOT NULL,
    [PublishStartDate]           DATETIME      NULL,
    [BrandId]                    INT           NOT NULL,
    [MappingId]                  INT           NOT NULL,
    [PortalId]                   INT           NOT NULL,
    [WidgetsKey]                 VARCHAR (500) NULL,
    [TypeOfMapping]              VARCHAR (50)  NULL,
    [DisplayOrder]               INT           NULL,
    CONSTRAINT [PK_ZnodePublishWidgetBrandEntity] PRIMARY KEY CLUSTERED ([PublishWidgetBrandEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishWidgetBrandEntityVersionId]
    ON [dbo].[ZnodePublishWidgetBrandEntity]([VersionId] ASC);

