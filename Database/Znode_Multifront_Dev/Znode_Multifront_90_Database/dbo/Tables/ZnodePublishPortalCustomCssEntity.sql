CREATE TABLE [dbo].[ZnodePublishPortalCustomCssEntity] (
    [PublishPortalCustomCssEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                      INT            NOT NULL,
    [PublishStartTime]               DATETIME       NULL,
    [PortalId]                       INT            NOT NULL,
    [DynamicStyle]                   NVARCHAR (MAX) NULL,
    [PublishState]                   VARCHAR (50)   NOT NULL,
    [LocaleId]                       INT            NOT NULL,
    CONSTRAINT [PK_ZnodePublishPortalCustomCssEntity] PRIMARY KEY CLUSTERED ([PublishPortalCustomCssEntityId] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishPortalCustomCssEntityVersionId]
    ON [dbo].[ZnodePublishPortalCustomCssEntity]([VersionId] ASC);

